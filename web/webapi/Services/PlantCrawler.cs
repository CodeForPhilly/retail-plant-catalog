using System;
using Repositories;
using SavvyCrawler;
using Shared;

namespace webapi.Services
{
    public class PlantCrawler
    {
        private readonly PlantRepository plantRepository;
        private readonly VendorService vendorService;
        private readonly VendorUrlRepository vendorUrlRepository;
        private readonly Dictionary<string, string> plantLookup = new Dictionary<string, string>(); //term to plantId
        private string[] terms = new string[] { };

        private readonly VendorRepository vendorRepository;

        public PlantCrawler(PlantRepository plantRepository, VendorService vendorService, VendorUrlRepository vendorUrlRepository, VendorRepository vendorRepository)
        {
            this.plantRepository = plantRepository;
            this.vendorService = vendorService;
            this.vendorUrlRepository = vendorUrlRepository;
            this.vendorRepository = vendorRepository;
        }

        public void Init()
        {
            terms = plantRepository.GetTerms();
            var plants = plantRepository.GetAll();
            foreach (var plant in plants)
            {
                plantLookup[plant.CommonName] = plant.Id;
                plantLookup[plant.ScientificName] = plant.Id;
               // plantLookup[plant.Symbol] = plant.Id;
            }
        }

        public async Task<(CrawlStatus Status, Dictionary<string, int> Terms)> TestUrl(string url)
        {
            var termCounter = new TermCounter(terms);
            var crawler = new Crawler(termCounter);
            
            try
            {
                await crawler.Start(url, 1, true);
                return (CrawlStatus.Ok, termCounter.Terms);
            }
            catch (CrawlFailException cfex)
            {
                return (cfex.CrawlStatus, termCounter.Terms);
            }
            catch (Exception)
            {
                return (CrawlStatus.Missing, termCounter.Terms);
            }
        }

        public async Task Crawl(Vendor vendor)
        {
            if (vendor?.Id == null) return; //vendor must have an id to be associated
            plantRepository.ClearAssociations(vendor.Id);
            if (vendor.PlantListingUris != null)
            {
                foreach (var plu in vendor.PlantListingUris)
                {
                    plu.CrawlInProgress = true;
                    await vendorUrlRepository.UpdateAsync(plu);
                    try
                    {
                        var termCounter = new TermCounter(terms);
                        var crawler = new Crawler(termCounter);
                        try
                        {
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
                            plu.LastSucceeded = DateTime.Now;
                        }
                        catch (CrawlFailException cfex)
                        {
                            plu.LastStatus = cfex.CrawlStatus;
                            plu.LastFailed = DateTime.Now;
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
            }
            vendor.LastCrawled = DateTime.UtcNow;
        }

        /// <summary>
        /// Crawls a single plant listing URL for a vendor. Does not clear other URL associations; only adds/updates for this URL.
        /// </summary>
        public async Task CrawlSingleUrl(string vendorId, string urlId)
        {
            var vendor = vendorService.GetPopulatedVendor(vendorId);
            if (vendor?.Id == null || vendor.PlantListingUris == null) return;
            var plu = vendor.PlantListingUris.FirstOrDefault(u => u.Id == urlId);
            if (plu == null) return;
            plu.CrawlInProgress = true;
            await vendorUrlRepository.UpdateAsync(plu);
            try
            {
                var termCounter = new TermCounter(terms);
                var crawler = new Crawler(termCounter);
                try
                {
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
                }
                catch (CrawlFailException cfex)
                {
                    plu.LastStatus = cfex.CrawlStatus;
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
        }
    }
}