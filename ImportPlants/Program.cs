// See https://aka.ms/new-console-template for more information


//Parse the JSON

using Newtonsoft.Json;
using Repositories;
using Shared;

using MySqlConnector;
using SavvyCrawler;

using var conn = new MySqlConnection("Server=127.0.0.1;Port=3306;Database=pac;Uid=root;Pwd=password;SslMode=none;");

//ParsePlants(conn);
//ParseNurseries(conn);
var vendorRepository = new VendorRepository(conn);
var plantRepository = new PlantRepository(conn);

await AssociatePlantsToVendors(vendorRepository, plantRepository);

static void ParseNurseries(MySqlConnection conn)
{
    var vendorRepository = new VendorRepository(conn);
    var vendorUrlRepository = new VendorUrlRepository(conn);
    var json = File.ReadAllText("nurseries.json");
    json = json.Replace("Plant List  (Raw)", "PlantListRaw")
               .Replace("Plant List ", "PlantList");

    var lines = json.Split("\n");
    foreach (var line in lines)
    {
        if (string.IsNullOrEmpty(line)) continue;
        var nursery = JsonConvert.DeserializeObject<Nursery>(line);
        var vendor = new Vendor
        {
            Id = Guid.NewGuid().ToString(),
            PublicPhone = nursery.PHONE,
            Address = $"{nursery.ADDRESS}, {nursery.CITY} {nursery.STATE} {nursery.ZIP}",
            State = nursery.STATE,
            Approved = true,
            CreatedAt = DateTime.UtcNow,
            StoreName = nursery.SOURCE,
            PublicEmail = nursery.EMAIL,
            StoreUrl = nursery.URL,
            UserId = null,
            Lat = nursery.Lat,
            Lng = nursery.Long
        };

        vendor.PlantListingUris = new List<VendorUrl> { new VendorUrl{ Id=Guid.NewGuid().ToString(), VendorId = vendor.Id, Uri= nursery.PlantList }, new VendorUrl { Id=Guid.NewGuid().ToString(), VendorId = vendor.Id, Uri = nursery.PlantListRaw } }.ToArray();
        vendorRepository.Insert(vendor);
        foreach (var vendorUrl in vendor.PlantListingUris)
        {
            vendorUrlRepository.Insert(vendorUrl);
        }
    }
}
static void ParsePlants(MySqlConnection conn)
{
    var plantRepository = new PlantRepository(conn);

    var json = File.ReadAllText("plants_trimmed.json");
    json = json.Replace("USDA Symbol", "symbol")
    .Replace("Scientific Name", "scientificName")
    .Replace("Common Name", "commonName")
    .Replace("Flowering Months", "floweringMonths")
    .Replace("Height (feet)", "height")
    .Replace("Recommendation Score", "recommendationScore");

    ///Console.WriteLine(json);
    var lines = json.Split("\n");
    foreach (var line in lines)
    {
        if (string.IsNullOrEmpty(line)) continue;
        var plant = JsonConvert.DeserializeObject<Plant>(line);
        plant.Id = Guid.NewGuid().ToString();
        plantRepository.Insert(plant);
    }
}

static async Task AssociatePlantsToVendors(VendorRepository vendorRepository, PlantRepository plantRepository)
{
    var terms = plantRepository.GetTerms();
    var plants = plantRepository.GetAll();
    var plantLookup = new Dictionary<string, string>(); //term to plantId
    foreach (var plant in plants)
    {
        plantLookup[plant.CommonName] = plant.Id;
        plantLookup[plant.ScientificName] = plant.Id;
        plantLookup[plant.Symbol] = plant.Id;
    }

    foreach (var vendor in vendorRepository.GetAll())
    {
        var v2 = vendorRepository.Get(vendor.Id);
        await Crawl(plantRepository, terms, plantLookup, v2);

    }
}

static async Task Crawl(PlantRepository plantRepository, string[] terms, Dictionary<string, string> plantLookup, Vendor v2)
{
    var termCounter = new TermCounter(terms);
    var crawler = new Crawler(termCounter);
    if (v2.PlantListingUrls != null)
    {
        foreach (var uri in v2.PlantListingUrls.Distinct())
        {
            await crawler.Start(uri, 1);
            var termsFound = termCounter.Terms.Where(t => t.Value > 0).Select(t => t.Key);
            foreach (var term in termsFound)
            {
                var plantId = plantLookup[term];
                plantRepository.Associate(plantId, v2.Id);
            }
        }
    }
}