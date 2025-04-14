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
            await vendorRepository.InsertAsync(vendor);
            foreach (var vendorUrl in vendor.PlantListingUrls)
            {
                var vu = new VendorUrl { Id = Guid.NewGuid().ToString(), Uri = vendorUrl, VendorId = vendor.Id };
                urlRepository.Insert(vu);
            }
            return vendor;
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

