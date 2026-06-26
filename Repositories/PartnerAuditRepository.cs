namespace Repositories;

using Dapper;
using Shared;
using System.Data;

/// <summary>
/// Append-only repository for the <c>partner_audit</c> table. The Insert method
/// is the only write path; reads are limited to vendor-scoped lookups and the
/// future admin-read endpoint.
/// </summary>
public class PartnerAuditRepository : Repository<PartnerAudit>
{
    public PartnerAuditRepository(IDbConnection connection) : base(connection)
    {
    }

    /// <summary>
    /// Records a partner-tag operation. Auto-assigns Id and OccurredAt if not set.
    /// Uses DynamicParameters with explicit DbType.String for the enum column so
    /// Dapper sends the enum name ("Tag"/"Update"/"Clear") rather than the
    /// underlying int — matches the pattern used by CrawlJobRepository and
    /// VendorUrlRepository for MySQL ENUM columns.
    /// </summary>
    public new void Insert(PartnerAudit row)
    {
        if (string.IsNullOrEmpty(row.Id))
            row.Id = Guid.NewGuid().ToString();
        if (row.OccurredAt == default)
            row.OccurredAt = DateTime.UtcNow;

        var parameters = new DynamicParameters();
        parameters.Add("@Id", row.Id);
        parameters.Add("@VendorId", row.VendorId);
        parameters.Add("@Operation", row.Operation.ToString(), System.Data.DbType.String);
        parameters.Add("@PriorPartner", row.PriorPartner);
        parameters.Add("@PriorExternalKey", row.PriorExternalKey);
        parameters.Add("@NewPartner", row.NewPartner);
        parameters.Add("@NewExternalKey", row.NewExternalKey);
        parameters.Add("@ActingUserId", row.ActingUserId);
        parameters.Add("@OccurredAt", row.OccurredAt);

        conn.Execute(
            @"INSERT INTO partner_audit
                (Id, VendorId, Operation, PriorPartner, PriorExternalKey,
                 NewPartner, NewExternalKey, ActingUserId, OccurredAt)
              VALUES
                (@Id, @VendorId, @Operation, @PriorPartner, @PriorExternalKey,
                 @NewPartner, @NewExternalKey, @ActingUserId, @OccurredAt)",
            parameters);
    }

    /// <summary>Returns the most-recent audit rows for a vendor, newest first.</summary>
    public IEnumerable<PartnerAudit> RecentForVendor(string vendorId, int limit = 50)
    {
        if (string.IsNullOrWhiteSpace(vendorId)) return Enumerable.Empty<PartnerAudit>();
        if (limit <= 0) limit = 50;
        return conn.Query<PartnerAudit>(
            "SELECT * FROM partner_audit WHERE VendorId = @vendorId ORDER BY OccurredAt DESC LIMIT @limit",
            new { vendorId, limit });
    }
}
