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

        public string Uri { get; set; }

        public string? VendorId { get; set; }

        public DateTime? LastSucceeded { get; set; }

        public DateTime? LastFailed { get; set; }

        public CrawlStatus LastStatus { get; set; }
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