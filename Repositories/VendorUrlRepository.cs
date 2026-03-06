namespace Repositories;

using Dapper;
using Shared;
using System.Data;

public class VendorUrlRepository : Repository<VendorUrl>
{
    public VendorUrlRepository(IDbConnection connection): base(connection)
    {

    }

    public VendorUrl GetByUrlOrId(VendorUrl url)
    {
        if (url == null) return null!;
        return conn.QueryFirstOrDefault<VendorUrl>(
            "SELECT * FROM vendor_urls WHERE VendorId = @VendorId AND (Id = @Id OR LOWER(TRIM(Uri)) = LOWER(TRIM(@Uri))) LIMIT 1",
            new { url.VendorId, url.Id, url.Uri });
    }

    /// <summary>
    /// Returns an existing VendorUrl if any (non-deleted) vendor already has this plant listing URI (case-insensitive, trim).
    /// Use for duplicate detection across vendors before adding a URL.
    /// </summary>
    public VendorUrl? GetByUriAnyVendor(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri)) return null;
        return conn.QueryFirstOrDefault<VendorUrl>(
            "SELECT u.* FROM vendor_urls u INNER JOIN vendor v ON v.Id = u.VendorId WHERE v.IsDeleted = false AND LOWER(TRIM(u.Uri)) = LOWER(TRIM(@uri)) LIMIT 1",
            new { uri });
    }

    /// <summary>
    /// Returns an existing row if the same vendor already has this URI (case-insensitive, trim).
    /// Use for duplicate detection before insert.
    /// </summary>
    public VendorUrl GetByVendorAndUri(string vendorId, string uri)
    {
        if (string.IsNullOrWhiteSpace(vendorId) || string.IsNullOrWhiteSpace(uri)) return null!;
        return conn.QueryFirstOrDefault<VendorUrl>(
            "SELECT * FROM vendor_urls WHERE VendorId = @vendorId AND LOWER(TRIM(Uri)) = LOWER(TRIM(@uri)) LIMIT 1",
            new { vendorId, uri });
    }

    public IEnumerable<VendorUrl> FindForVendor(string vendorId)
    {
        return conn.Query<VendorUrl>("select * from vendor_urls where VendorId = @vendorId", new { vendorId });
    }

    /// <summary>
    /// Returns all vendor URLs, optionally filtered by vendor state/approved/deleted.
    /// </summary>
    public IEnumerable<VendorUrl> GetAllForExport(string? state = null, bool? approved = null, bool includeDeleted = false)
    {
        if ((string.IsNullOrEmpty(state) || state == "ALL") && !approved.HasValue && includeDeleted)
        {
            return conn.Query<VendorUrl>("SELECT * FROM vendor_urls");
        }
        var sql = @"SELECT u.* FROM vendor_urls u
INNER JOIN vendor v ON v.Id = u.VendorId
WHERE 1=1";
        var parameters = new DynamicParameters();
        if (state != null && state != "ALL")
        {
            sql += " AND v.State = @state";
            parameters.Add("state", state);
        }
        if (approved.HasValue)
        {
            sql += " AND v.Approved = @approved";
            parameters.Add("approved", approved.Value);
        }
        if (!includeDeleted)
        {
            sql += " AND v.IsDeleted = false";
        }
        return conn.Query<VendorUrl>(sql, parameters);
    }
    public async Task<IEnumerable<VendorUrl>> FindForVendorAsync(string vendorId)
    {
        return await conn.QueryAsync<VendorUrl>("select * from vendor_urls where VendorId = @vendorId", new { vendorId });
    }

    public override long Insert(VendorUrl obj)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", obj.Id);
        parameters.Add("@Uri", obj.Uri);
        parameters.Add("@VendorId", obj.VendorId);
        parameters.Add("@LastSucceeded", obj.LastSucceeded);
        parameters.Add("@LastFailed", obj.LastFailed);
        parameters.Add("@LastStatus", obj.LastStatus.ToString(), DbType.String);
        parameters.Add("@PlantCount", obj.PlantCount);
        parameters.Add("@CrawlInProgress", obj.CrawlInProgress);
        
        return conn.Execute(
            "insert into vendor_urls (Id, Uri, VendorId, LastSucceeded, LastFailed, LastStatus, PlantCount, CrawlInProgress) values (@Id, @Uri, @VendorId, @LastSucceeded, @LastFailed, @LastStatus, @PlantCount, @CrawlInProgress)", 
            parameters);
    }

    public async override Task<long> InsertAsync(VendorUrl obj)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", obj.Id);
        parameters.Add("@Uri", obj.Uri);
        parameters.Add("@VendorId", obj.VendorId);
        parameters.Add("@LastSucceeded", obj.LastSucceeded);
        parameters.Add("@LastFailed", obj.LastFailed);
        parameters.Add("@LastStatus", obj.LastStatus.ToString(), DbType.String);
        parameters.Add("@PlantCount", obj.PlantCount);
        parameters.Add("@CrawlInProgress", obj.CrawlInProgress);
        
        return await conn.ExecuteAsync(
            "insert into vendor_urls (Id, Uri, VendorId, LastSucceeded, LastFailed, LastStatus, PlantCount, CrawlInProgress) values (@Id, @Uri, @VendorId, @LastSucceeded, @LastFailed, @LastStatus, @PlantCount, @CrawlInProgress)", 
            parameters);
    }
    public override bool Update(VendorUrl obj)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", obj.Id);
        parameters.Add("@Uri", obj.Uri);
        parameters.Add("@LastSucceeded", obj.LastSucceeded);
        parameters.Add("@LastFailed", obj.LastFailed);
        parameters.Add("@LastStatus", obj.LastStatus.ToString(), DbType.String);
        parameters.Add("@PlantCount", obj.PlantCount);
        parameters.Add("@CrawlInProgress", obj.CrawlInProgress);

         conn.Execute(
            "update vendor_urls set Uri=@Uri, LastSucceeded=@LastSucceeded, LastFailed=@LastFailed, LastStatus=@LastStatus, PlantCount=@PlantCount, CrawlInProgress=@CrawlInProgress where Id =@Id",
            parameters);
        return true;
    }
    public async override Task<bool> UpdateAsync(VendorUrl obj)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", obj.Id);
        parameters.Add("@Uri", obj.Uri);
        parameters.Add("@LastSucceeded", obj.LastSucceeded);
        parameters.Add("@LastFailed", obj.LastFailed);
        parameters.Add("@LastStatus", obj.LastStatus.ToString(), DbType.String);
        parameters.Add("@PlantCount", obj.PlantCount);
        parameters.Add("@CrawlInProgress", obj.CrawlInProgress);

        await conn.ExecuteAsync(
             "update vendor_urls set Uri=@Uri, LastSucceeded=@LastSucceeded, LastFailed=@LastFailed, LastStatus=@LastStatus, PlantCount=@PlantCount, CrawlInProgress=@CrawlInProgress where Id =@Id",
           parameters);
        return true;
    }
}

