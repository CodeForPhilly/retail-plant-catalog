using System;
using Microsoft.Extensions.Logging;
using Repositories;
using SavvyCrawler;
using Shared;

namespace webapi.Services
{
    public class PlantCrawler
    {
        private static readonly object PlantCacheLock = new object();
        private static Dictionary<string, string>? _cachedPlantLookup;
        private static string[]? _cachedTerms;

        private readonly PlantRepository plantRepository;
        private readonly VendorService vendorService;
        private readonly VendorUrlRepository vendorUrlRepository;

        private readonly VendorRepository vendorRepository;
        private readonly ILogger<PlantCrawler> logger;

        public PlantCrawler(PlantRepository plantRepository, VendorService vendorService, VendorUrlRepository vendorUrlRepository, VendorRepository vendorRepository, ILogger<PlantCrawler> logger)
        {
            this.plantRepository = plantRepository;
            this.vendorService = vendorService;
            this.vendorUrlRepository = vendorUrlRepository;
            this.vendorRepository = vendorRepository;
            this.logger = logger;
        }

        /// <summary>Clears the shared plant term cache so the next crawl reloads from the database.</summary>
        public static void InvalidatePlantCache()
        {
            lock (PlantCacheLock)
            {
                _cachedPlantLookup = null;
                _cachedTerms = null;
            }
        }

        public void Init()
        {
            EnsurePlantCacheLoaded();
        }

        private void EnsurePlantCacheLoaded()
        {
            if (_cachedPlantLookup != null && _cachedTerms != null)
                return;
            lock (PlantCacheLock)
            {
                if (_cachedPlantLookup != null && _cachedTerms != null)
                    return;

                var terms = plantRepository.GetTerms();
                var lookup = new Dictionary<string, string>();
                foreach (var plant in plantRepository.GetAll())
                {
                    lookup[plant.CommonName] = plant.Id;
                    lookup[plant.ScientificName] = plant.Id;
                    // lookup[plant.Symbol] = plant.Id;
                }

                _cachedTerms = terms;
                _cachedPlantLookup = lookup;
            }
        }

        public async Task<(CrawlStatus Status, Dictionary<string, int> Terms)> TestUrl(string url)
        {
            EnsurePlantCacheLoaded();
            var terms = _cachedTerms!;
            var termCounter = new TermCounter(terms);
            try
            {
                using (var crawler = new Crawler(termCounter))
                    await crawler.Start(url, 1, true);
                return (CrawlStatus.Ok, termCounter.Terms);
            }
            catch (CrawlFailException cfex)
            {
                if (cfex.CrawlStatus == CrawlStatus.Missing)
                    logger.LogWarning(cfex, "TestUrl CrawlFail Missing for {Url}", url);
                return (cfex.CrawlStatus, termCounter.Terms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "TestUrl unexpected exception for {Url}; treating as Missing", url);
                return (CrawlStatus.Missing, termCounter.Terms);
            }
        }

        public async Task Crawl(Vendor vendor)
        {
            if (vendor?.Id == null) return; //vendor must have an id to be associated
            EnsurePlantCacheLoaded();
            var plantLookup = _cachedPlantLookup!;
            // We deliberately do NOT clear associations up front. A crawl that fails partway, is
            // interrupted, or parses zero plants must never destroy a vendor's existing catalog.
            // Instead we accumulate everything we find and replace the catalog at the very end, and
            // only when the result is trustworthy (at least one URL crawled OK and plants were found).
            var anyUrlOk = false;
            // Accumulate plant IDs across every URL in this crawl run. TermCounter.Examine
            // OVERWRITES per-term counts on each Examine call (see SavvyCrawler/TermCounter.cs:24),
            // so the counter only ever reflects the most recently examined content. Reading the
            // cumulative state off the counter after the loop yields only the LAST URL's plants
            // and ReplaceAssociations then wipes all others — under the prior implementation
            // every multi-URL vendor's catalog collapsed to whatever was on the last URL crawled.
            var foundPlantIds = new HashSet<string>();
            if (vendor.PlantListingUris != null)
            {
                // One HttpClient per vendor reuses connections/DNS to the same store host (avoids socket churn from new client per URL).
                var termCounter = new TermCounter(_cachedTerms!);
                using (var crawler = new Crawler(termCounter))
                {
                    foreach (var plu in vendor.PlantListingUris)
                    {
                        plu.CrawlInProgress = true;
                        await vendorUrlRepository.UpdateAsync(plu);
                        try
                        {
                            try
                            {
                                await crawler.Start(plu.Uri, 1);
                                // termCounter now reflects ONLY this URL (Examine overwrites).
                                // Capture this URL's plant hits BEFORE moving to the next iteration.
                                var plantIdsThisUrl = termCounter.Terms
                                    .Where(t => t.Value > 0)
                                    .Select(t => t.Key)
                                    .Where(plantLookup.ContainsKey)
                                    .Select(term => plantLookup[term])
                                    .Distinct()
                                    .ToList();
                                foreach (var pid in plantIdsThisUrl)
                                    foundPlantIds.Add(pid);
                                plu.PlantCount = plantIdsThisUrl.Count;
                                plu.LastStatus = CrawlStatus.Ok;
                                plu.LastSucceeded = DateTime.UtcNow;
                                plu.LastFailed = null;
                                anyUrlOk = true;
                            }
                            catch (CrawlFailException cfex)
                            {
                                plu.LastStatus = cfex.CrawlStatus;
                                plu.LastFailed = DateTime.UtcNow;
                                plu.PlantCount = null;
                                if (cfex.CrawlStatus == CrawlStatus.Missing)
                                    logger.LogWarning(cfex, "Crawl Missing for vendor {VendorId} URL {Uri}", vendor.Id, plu.Uri);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Crawl unexpected exception for vendor {VendorId} URL {Uri}", vendor.Id, plu.Uri);
                                plu.LastStatus = CrawlStatus.Missing;
                                plu.LastFailed = DateTime.UtcNow;
                                plu.PlantCount = null;
                            }
                            await vendorUrlRepository.UpdateAsync(plu);
                        }
                        finally
                        {
                            plu.CrawlInProgress = false;
                            await vendorUrlRepository.UpdateAsync(plu);
                        }
                    }

                    // Only replace the catalog when the crawl is trustworthy. Otherwise keep what the
                    // vendor already has, so a failed/empty crawl can't silently wipe a good catalog.
                    if (anyUrlOk && foundPlantIds.Count > 0)
                    {
                        plantRepository.ReplaceAssociations(vendor.Id, foundPlantIds.ToList());
                    }
                    else
                    {
                        var existing = plantRepository.CountAssociations(vendor.Id);
                        logger.LogWarning(
                            "Crawl for vendor {VendorId} produced {Found} plants (anyUrlOk={AnyOk}); keeping {Existing} existing associations rather than clearing.",
                            vendor.Id, foundPlantIds.Count, anyUrlOk, existing);
                    }
                }
            }
            vendor.LastCrawled = DateTime.UtcNow;
            // PlantCount reflects the actual association count (real links), not the per-URL term
            // tallies, so it stays correct whether we replaced the catalog or kept the existing one.
            vendor.PlantCount = plantRepository.CountAssociations(vendor.Id);
        }

        /// <summary>
        /// Crawls a single plant listing URL for a vendor. Does not clear other URL associations; only adds/updates for this URL.
        /// Returns <c>true</c> if the crawl ran (regardless of HTTP-level success); returns <c>false</c> if the URL was not found,
        /// the vendor was not found, or another crawl already holds the URL-level lock.
        /// </summary>
        public async Task<bool> CrawlSingleUrl(string vendorId, string urlId)
        {
            var vendor = vendorService.GetPopulatedVendor(vendorId);
            if (vendor?.Id == null || vendor.PlantListingUris == null) return false;
            var plu = vendor.PlantListingUris.FirstOrDefault(u => u.Id == urlId);
            if (plu == null) return false;
            // Atomically claim the URL-level crawl lock. If we can't, another crawl is in progress.
            if (!vendorUrlRepository.TryMarkCrawlInProgress(urlId)) return false;
            plu.CrawlInProgress = true;
            EnsurePlantCacheLoaded();
            var plantLookup = _cachedPlantLookup!;
            try
            {
                var termCounter = new TermCounter(_cachedTerms!);
                try
                {
                    using (var crawler = new Crawler(termCounter))
                        await crawler.Start(plu.Uri, 1);
                    var termsFound = termCounter.Terms.Where(t => t.Value > 0).Select(t => t.Key);
                    var plantCountThisUrl = 0;
                    foreach (var term in termsFound)
                    {
                        if (plantLookup.ContainsKey(term))
                        {
                            var plantId = plantLookup[term];
                            plantRepository.Associate(plantId, vendor.Id);
                            plantCountThisUrl++;
                        }
                    }
                    plu.PlantCount = plantCountThisUrl;
                    plu.LastStatus = CrawlStatus.Ok;
                    plu.LastSucceeded = DateTime.UtcNow;
                    plu.LastFailed = null;
                }
                catch (CrawlFailException cfex)
                {
                    plu.LastStatus = cfex.CrawlStatus;
                    plu.LastFailed = DateTime.UtcNow;
                    plu.PlantCount = null;
                    if (cfex.CrawlStatus == CrawlStatus.Missing)
                        logger.LogWarning(cfex, "CrawlSingleUrl Missing for vendor {VendorId} URL {Uri}", vendorId, plu.Uri);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "CrawlSingleUrl unexpected exception for vendor {VendorId} URL {Uri}", vendorId, plu.Uri);
                    plu.LastStatus = CrawlStatus.Missing;
                    plu.LastFailed = DateTime.UtcNow;
                    plu.PlantCount = null;
                }
                await vendorUrlRepository.UpdateAsync(plu);
                vendor.LastCrawled = DateTime.UtcNow;
                var mostRecent = vendor.PlantListingUris
                    .Select(u => new { u.LastStatus, Time = u.LastSucceeded ?? u.LastFailed })
                    .Where(x => x.Time != null)
                    .OrderByDescending(x => x.Time)
                    .FirstOrDefault();
                vendor.LastCrawlStatus = mostRecent?.LastStatus ?? CrawlStatus.None;
                vendor.CrawlErrors = vendor.PlantListingUris.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok);
                vendor.PlantCount = vendor.PlantListingUris.Sum(u => u.PlantCount ?? 0);
                var vendorToUpdate = vendorRepository.Get(vendorId);
                if (vendorToUpdate != null)
                {
                    vendorToUpdate.LastCrawled = vendor.LastCrawled;
                    vendorToUpdate.LastCrawlStatus = vendor.LastCrawlStatus;
                    vendorToUpdate.CrawlErrors = vendor.CrawlErrors;
                    vendorToUpdate.PlantCount = vendor.PlantCount;
                    vendorRepository.Update(vendorToUpdate);
                }
            }
            finally
            {
                plu.CrawlInProgress = false;
                await vendorUrlRepository.UpdateAsync(plu);
            }
            return true;
        }
    }
}