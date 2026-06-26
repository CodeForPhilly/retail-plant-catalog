namespace Repositories;

using Dapper;
using Shared;
using System.Data;

/// <summary>
/// Read-mostly lookup of <see cref="Partner"/> rows (Xerces, Jupiter, ...).
/// Used by the export filter to validate <c>?partner=</c> input and by the
/// partner-aware vendor create path to enforce that <c>vendor.Partner</c> only
/// references active partners.
/// </summary>
public class PartnerRepository : Repository<Partner>
{
    public PartnerRepository(IDbConnection connection) : base(connection)
    {
    }

    /// <summary>Returns true iff a partner row with this Id exists (regardless of Active state).</summary>
    public bool Exists(string partnerId)
    {
        if (string.IsNullOrWhiteSpace(partnerId)) return false;
        return conn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM partner WHERE Id = @partnerId",
            new { partnerId }) > 0;
    }

    /// <summary>Returns true iff a partner row with this Id exists AND Active = 1.</summary>
    public bool IsActive(string partnerId)
    {
        if (string.IsNullOrWhiteSpace(partnerId)) return false;
        return conn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM partner WHERE Id = @partnerId AND Active = 1",
            new { partnerId }) > 0;
    }

    /// <summary>Returns all partners ordered by DisplayName.</summary>
    public IEnumerable<Partner> List()
    {
        return conn.Query<Partner>("SELECT * FROM partner ORDER BY DisplayName");
    }
}
