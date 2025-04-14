using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;
namespace Shared
{
    [Table("vendor")]
    public class Vendor
    {
        [ExplicitKey]   
        public string? Id { get; set; }

        public string? UserId { get; set; }

        public string StoreName { get; set; } = "";

        public string Address { get; set; } = "";

        public string State { get; set; } = "";

        public decimal Lng { get; set; } = 0;

        public decimal Lat { get; set; } = 0;

        public string StoreUrl { get; set; } = "";

        public string PublicEmail { get; set; } = "";

        public string PublicPhone { get; set; } = "";

        /// <summary>
        /// If true, then this nursery has a commitment to only carry native plants to their state.
        /// </summary>
        public bool AllNative { get; set; } = false;

        public bool Approved { get; set; }

        public int PlantCount { get; set; }

        public string Notes { get; set; } = "";
        public int CrawlErrors { get;set; }


        [Computed]
        public VendorUrl[]? PlantListingUris { get; set; }
        /// <summary>
        /// Used to submit urls from the UI
        /// </summary>
        [Computed]
        public string[]? PlantListingUrls { get; set; } = new string[] { };

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted {get;set;}

    }

    public class VendorPlus : Vendor
    {
        public decimal Distance { get; set; }
    }
    
}