using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Shared
{
    /// <summary>
    /// Operation captured for a partner-tagging audit row. Inferable from prior/new
    /// pair, but denormalised for ergonomic queries.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PartnerAuditOperation
    {
        Tag,
        Update,
        Clear,
    }

    /// <summary>
    /// Append-only record of a tag / update / clear operation on a vendor's
    /// Partner+ExternalKey linkage. Written by VendorController.SetPartnerCore
    /// after a successful SetPartnerLink. No UPDATE or DELETE paths exposed.
    /// </summary>
    [Table("partner_audit")]
    public class PartnerAudit
    {
        [ExplicitKey]
        public string Id { get; set; } = "";

        public string VendorId { get; set; } = "";

        public PartnerAuditOperation Operation { get; set; }

        public string? PriorPartner { get; set; }
        public string? PriorExternalKey { get; set; }
        public string? NewPartner { get; set; }
        public string? NewExternalKey { get; set; }

        public string ActingUserId { get; set; } = "";

        public DateTime OccurredAt { get; set; }
    }
}
