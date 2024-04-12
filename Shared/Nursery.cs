namespace Shared
{
    /* {
      "_id": { "$oid": "658c0e2d651b0a3b15331f51" },
      "SOURCE": "Big Mulberry",
      "Source (How we found this supplier)": "Nursery Tree",
      "URL": "bigmulberrynursery.com/",
      "Plant List  (Raw)": "https://bigmulberrynursery.com/plants/",
      "Plant List ": "https://docs.google.com/document/d/1f6HIdv7wy_Tk_fslMnkQg4KBpqkEjFZCpEgFJlNo9-A/edit",
      "Type": "Google Doc",
      "ADDRESS": "9320 Al. Hwy 14",
      "CITY": "East Selma",
      "STATE": "AL",
      "ZIP": "36701",
      "PHONE": "(334) 418-4813",
      "EMAIL": "",
      "Lat": "32.43109",
      "Long": "-86.96039",
      "County": "Dallas",
      "lon": -86.96039,
      "lat": 32.43109
    }*/
    public class Nursery
    {
        public string Id { get; set; }
        public string SOURCE { get; set; }
        public string URL { get; set; }
        public string Type { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public string County { get; set; }
        public string PlantListRaw { get; set; }
        public string PlantList { get; set; }

    }
}