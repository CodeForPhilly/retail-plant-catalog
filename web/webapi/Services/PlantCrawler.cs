using System;
using Repositories;
using SavvyCrawler;
using Shared;

namespace webapi.Services
{
    public class PlantCrawler
    {
        private readonly PlantRepository plantRepository;
        private readonly Dictionary<string, string> plantLookup = new Dictionary<string, string>(); //term to plantId
        private string[] terms = new string[] { };

        public PlantCrawler(PlantRepository plantRepository)
        {
            this.plantRepository = plantRepository;
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

        public async Task Crawl(Vendor vendor)
        {
            if (vendor?.Id == null) return; //vendor must have an id to be associated
            plantRepository.ClearAssociations(vendor.Id);
            var termCounter = new TermCounter(terms);
            var crawler = new Crawler(termCounter);
            if (vendor.PlantListingUrls != null)
            {
                foreach (var uri in vendor.PlantListingUrls.Distinct())
                {
                    await crawler.Start(uri, 1);
                    var termsFound = termCounter.Terms.Where(t => t.Value > 0).Select(t => t.Key);
                    foreach (var term in termsFound)
                    {
                        if (plantLookup.ContainsKey(term))
                        {
                            var plantId = plantLookup[term];
                            plantRepository.Associate(plantId, vendor.Id);
                        }
                    }
                }
            }
        }
    }
}