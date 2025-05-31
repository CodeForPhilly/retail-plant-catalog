using System;
using Repositories;
using Shared;

namespace webapi.Services
{
    public class VendorService
    {
        private readonly VendorRepository vendorRepository;
        private readonly VendorUrlRepository urlRepository;

        public VendorService(VendorRepository vendorRepository, VendorUrlRepository urlRepository)
        {
            this.vendorRepository = vendorRepository;
            this.urlRepository = urlRepository;
        }

        public Vendor GetPopulatedVendor(string vendorId)
        {
            var vendor = vendorRepository.Get(vendorId);
            var urls = urlRepository.FindForVendor(vendorId);
            vendor.PlantListingUris = urls.ToArray();
            vendor.PlantListingUrls = urls.Select(u => u.Uri).ToArray();
            vendor.CrawlErrors = urls.Where(u => u.LastStatus != CrawlStatus.Ok).Count();
            return vendor;
        }
        public async Task<Vendor> GetPopulatedVendorAsync(string vendorId)
        {
            var vendorTask = vendorRepository.GetAsync(vendorId);
            var urlsTask = urlRepository.FindForVendorAsync(vendorId);
            
            await Task.WhenAll(vendorTask, urlsTask);
            
            var vendor = await vendorTask;
            var urls = await urlsTask;
            vendor.PlantListingUris = urls.ToArray();
            vendor.PlantListingUrls = urls.Select(u => u.Uri).ToArray();
            return vendor;
        }

        public async Task<Vendor> CreateAsync(Vendor vendor)
        {
            vendor.CrawlErrors = vendor.PlantListingUris?.Where(u => u.LastStatus != CrawlStatus.Ok)?.Count() ?? 0;
            await vendorRepository.InsertAsync(vendor);
            return vendor;
        }
       
        
        public async Task<List<VendorUrl>> TestAndSaveUrls(string vendorId, IEnumerable<string> urls, PlantCrawler plantCrawler)
        {
            var results = new List<VendorUrl>();
            var existingUrls = await urlRepository.FindForVendorAsync(vendorId);
            
            foreach (var url in urls)
            {
                try
                {
                    // Test each URL
                    var result = await plantCrawler.TestUrl(url);
                    
                    // Find existing URL or create new one
                    var vendorUrl = existingUrls.FirstOrDefault(u => u.Uri == url) ?? 
                                   new VendorUrl { Id = Guid.NewGuid().ToString(), VendorId = vendorId, Uri = url };
                    
                    // Update status
                    vendorUrl.LastStatus = result.Status;
                    vendorUrl.LastFailed = result.Status != CrawlStatus.Ok ? DateTime.UtcNow : vendorUrl.LastFailed;
                    vendorUrl.LastSucceeded = result.Status == CrawlStatus.Ok ? DateTime.UtcNow : vendorUrl.LastSucceeded;
                    
                    // Save to database
                    if (existingUrls.Any(u => u.Id == vendorUrl.Id))
                    {
                        await urlRepository.UpdateAsync(vendorUrl);
                    }
                    else
                    {
                        await urlRepository.InsertAsync(vendorUrl);
                    }
                    
                    results.Add(vendorUrl);
                }
                catch (Exception)
                {
                    // Create URL with error status
                    var vendorUrl = new VendorUrl { 
                        Id = Guid.NewGuid().ToString(), 
                        Uri = url, 
                        VendorId = vendorId, 
                        LastStatus = CrawlStatus.UrlParsingError,
                        LastFailed = DateTime.UtcNow
                    };
                    
                    await urlRepository.InsertAsync(vendorUrl);
                    results.Add(vendorUrl);
                }
            }
            
            return results;
        }
        
        public async Task UpdateCrawlErrorCount(string vendorId)
        {
            var vendor = await GetPopulatedVendorAsync(vendorId);
            if (vendor.PlantListingUris != null)
            {
                vendor.CrawlErrors = vendor.PlantListingUris.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok);
                await vendorRepository.UpdateAsync(vendor);
            }
        }
      
        public async Task SaveUrls(string vendorId, string[] plantListingUrls)
        {
            var urlsForVendor = await urlRepository.FindForVendorAsync(vendorId);
            foreach (var uri in plantListingUrls)
            {
                //if exist ignore, otherwise add
                var lookupUri = urlsForVendor.FirstOrDefault(u => u.Uri == uri);
                if (lookupUri != null) continue;
                await urlRepository.InsertAsync(new VendorUrl { Id = Guid.NewGuid().ToString(), VendorId = vendorId, Uri = uri, LastStatus= CrawlStatus.None});
            }
        }
    }
}

