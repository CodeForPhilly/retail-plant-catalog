using Dapper;
using System.Data;
using Dapper.Contrib.Extensions;
using Shared;
using System.Text.Json.Serialization;

namespace Shared
{
    [Table("vendor_urls")]
    public class VendorUrl
    {
        [ExplicitKey]
        public string? Id { get; set; }

        public string Uri { get; set; } = null!;

        public string? VendorId { get; set; }

        public DateTime? LastSucceeded { get; set; }

        public DateTime? LastFailed { get; set; }

        public CrawlStatus LastStatus { get; set; }

        /// <summary>
        /// Number of distinct plants found when this URL was last successfully crawled.
        /// </summary>
        public int? PlantCount { get; set; }

        /// <summary>
        /// True while this URL is being crawled (drives per-URL spinner in UI).
        /// </summary>
        public bool CrawlInProgress { get; set; } = false;
    }

    /// <summary>
    /// Export row for vendor_plant junction table (PKs only).
    /// </summary>
    public class VendorPlantExportRow
    {
        public string VendorId { get; set; } = "";
        public string PlantId { get; set; } = "";
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
   // [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum CrawlStatus
    {
        Ok,
        UrlParsingError,
        Timeout,
        DnsFailure,
        Missing,
        RobotDenied,
        Redirect,
        None
    }

}