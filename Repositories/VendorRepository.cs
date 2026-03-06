namespace Repositories;

using Dapper;
using Shared;
using System.Data;
using System.Threading.Tasks;

public class VendorRepository : Repository<Vendor>
{
    public VendorRepository(IDbConnection connection) : base(connection)
    {

    }
    public IEnumerable<Vendor> Find(bool unapprovedOnly, bool showDeleted, string state, string sortBy, int skip, int take)
    {
        var deleteConstraint = showDeleted ? "1=1" : "IsDeleted = false";
        string stateConstraint = " 1=1";
        if (state != "ALL")
            stateConstraint = " State = @state ";
        if (unapprovedOnly)
            return conn.Query<Vendor>($"select * from vendor where Approved = false and {deleteConstraint} and {stateConstraint} order by {sortBy} limit @skip, @take", new { skip, take, state });
        return conn.Query<Vendor>($"select * from vendor where {stateConstraint} and {deleteConstraint} order by {sortBy} limit @skip, @take", new { skip, take, state });
    }
    public override async Task<long> InsertAsync(Vendor obj)
    {
        obj.CreatedAt = DateTime.UtcNow;
        if (obj.Id == null)
            obj.Id = Guid.NewGuid().ToString();
        string point = $"POINT({obj.Lng}, {obj.Lat})";
        var param = VendorInsertParams(obj);
        int recordsAffected = await conn.ExecuteAsync(
            "insert into vendor (Id, UserId, StoreName, Lat, Lng, Geo, Approved, Address, AllNative, State, StoreUrl, PublicEmail, PublicPhone, PlantCount, CreatedAt, Notes, LastCrawled, LastChanged, LastCrawlStatus, LivePlant, Seed) " +
            "values (@Id, @UserId, @StoreName, @Lat, @Lng, " + point + ", @Approved, @Address, @AllNative, @State, @StoreUrl, @PublicEmail, @PublicPhone, @PlantCount, @CreatedAt, @Notes, @LastCrawled, @LastChanged, @LastCrawlStatus, @LivePlant, @Seed)", param);
        return recordsAffected;
    }
    public override long Insert(Vendor obj)
    {
        obj.CreatedAt = DateTime.UtcNow;
        if (obj.Id == null)
            obj.Id = Guid.NewGuid().ToString();
        string point = $"POINT({obj.Lng}, {obj.Lat})";
        var param = VendorInsertParams(obj);
        var recordsAffected = conn.Execute(
            "insert into vendor (Id, UserId, StoreName, Lat, Lng, Geo, Approved, Address, AllNative, State, StoreUrl, PublicEmail, PublicPhone, PlantCount, CreatedAt, Notes, LastCrawled, LastChanged, LastCrawlStatus, LivePlant, Seed) " +
            "values (@Id, @UserId, @StoreName, @Lat, @Lng, " + point + ", @Approved, @Address, @AllNative, @State, @StoreUrl, @PublicEmail, @PublicPhone, @PlantCount, @CreatedAt, @Notes, @LastCrawled, @LastChanged, @LastCrawlStatus, @LivePlant, @Seed)", param);
        return recordsAffected;
    }

    private static object VendorInsertParams(Vendor obj)
    {
        return new
        {
            obj.Id,
            obj.UserId,
            obj.StoreName,
            obj.Lat,
            obj.Lng,
            obj.Approved,
            obj.Address,
            obj.AllNative,
            obj.State,
            obj.StoreUrl,
            obj.PublicEmail,
            obj.PublicPhone,
            obj.PlantCount,
            obj.CreatedAt,
            obj.Notes,
            obj.LastCrawled,
            obj.LastChanged,
            LastCrawlStatus = obj.LastCrawlStatus.ToString(),
            obj.LivePlant,
            obj.Seed
        };
    }
    public override bool Update(Vendor obj)
    {
        string  point = $"POINT({obj.Lng}, {obj.Lat})";
        var param = new
        {
            obj.StoreName,
            obj.Address,
            obj.Lng,
            obj.Lat,
            obj.StoreUrl,
            obj.PublicEmail,
            obj.PublicPhone,
            obj.Approved,
            obj.PlantCount,
            obj.AllNative,
            obj.CrawlErrors,
            obj.Notes,
            LastCrawlStatus = obj.LastCrawlStatus.ToString(),
            obj.LastCrawled,
            obj.LastChanged,
            obj.CrawlInProgress,
            obj.LivePlant,
            obj.Seed,
            obj.Id
        };
        conn.Execute(
            $"update vendor set StoreName=@StoreName, Address=@Address, Lng=@Lng, Lat=@Lat, Geo={point}, StoreUrl=@StoreUrl, PublicEmail=@PublicEmail, PublicPhone=@PublicPhone, Approved=@Approved, PlantCount=@PlantCount, AllNative=@AllNative, CrawlErrors=@CrawlErrors, Notes=@Notes, LastCrawlStatus=@LastCrawlStatus, LastCrawled=@LastCrawled, LastChanged=@LastChanged, CrawlInProgress=@CrawlInProgress, LivePlant=@LivePlant, Seed=@Seed where id = @Id",
            param);
        return true;
    }
    
    public IEnumerable<Vendor> Find(string? storeName, string state, bool unapprovedOnly, bool showDeleted, string sortBy, bool sortAsc, int skip, int take)
    {
        var deleteConstraint = showDeleted ? "1=1" : "IsDeleted = false";
        sortBy = $"{sortBy}" + (sortAsc ? "" : " desc");
        if (string.IsNullOrEmpty(storeName)) return Find(unapprovedOnly, showDeleted, state, sortBy, skip, take);
        string stateConstraint = "";
        if (state != "ALL")
            stateConstraint = " and State = @state ";
        storeName = $"%{storeName}%";
        if (unapprovedOnly)
            return conn.Query<Vendor>($"select * from vendor where StoreName like @storeName {stateConstraint} and Approved = false and {deleteConstraint} order by {sortBy} limit @skip, @take", new { storeName, skip, take, state });
        return conn.Query<Vendor>($"select * from vendor where StoreName like @storeName and {deleteConstraint} {stateConstraint} order by {sortBy} limit @skip, @take", new { storeName, skip, take, state });
    }

    public void Approve(string id, bool approved)
    {
        var rowsAffected = conn.Execute("update vendor set Approved = @approved where Id = @id", new { id, approved });

    }

     public int Delete(string id, bool deleteStatus = true)
    {
        var rowsAffected = conn.Execute("update vendor set IsDeleted = @deleteStatus where Id = @id", new { id, deleteStatus });
        return rowsAffected;
    }

    private void ClearAndInsertUrls(string? vendorId, string[] urls)
    {
        if (vendorId == null) return;
        var rowsAffected = conn.Execute("delete from vendor_urls where VendorId = @vendorId", new { vendorId });
        if (urls.Any())
        {
            foreach (var url in urls)
            {
                conn.Execute("insert into vendor_urls (Id, VendorId, Uri) values (@id, @vendorId, @uri)", new { id = Guid.NewGuid().ToString(), vendorId, uri = url });
            }
        }
    }

    /// <summary>
    /// Returns a vendor that already has this store URL (case-insensitive, trim). Excludes deleted vendors.
    /// Use for duplicate detection before create.
    /// </summary>
    public Vendor? GetByStoreUrl(string storeUrl)
    {
        if (string.IsNullOrWhiteSpace(storeUrl)) return null;
        return conn.QueryFirstOrDefault<Vendor>(
            "SELECT * FROM vendor WHERE LOWER(TRIM(StoreUrl)) = LOWER(TRIM(@storeUrl)) AND IsDeleted = false LIMIT 1",
            new { storeUrl });
    }

    /// <summary>
    /// Returns a vendor that has this store URL and is not the given vendor (case-insensitive, trim). Excludes deleted vendors.
    /// Use for duplicate detection on update so the current vendor can keep its own store URL.
    /// </summary>
    public Vendor? GetByStoreUrlExcluding(string storeUrl, string excludeVendorId)
    {
        if (string.IsNullOrWhiteSpace(storeUrl) || string.IsNullOrWhiteSpace(excludeVendorId)) return null;
        return conn.QueryFirstOrDefault<Vendor>(
            "SELECT * FROM vendor WHERE LOWER(TRIM(StoreUrl)) = LOWER(TRIM(@storeUrl)) AND IsDeleted = false AND Id != @excludeVendorId LIMIT 1",
            new { storeUrl, excludeVendorId });
    }

    public Vendor? FindByUserId(string userId)
    {
        var vendor = conn.QueryFirstOrDefault<Vendor>("select * from vendor where UserId = @userId and IsDeleted = false", new { userId });
        if (vendor == null) return null;
        return vendor;
    }

    public IEnumerable<VendorPlus> FindByRadius(double lng, double lat, int radius = 10000){ // 10km
        var point = $"POINT({lng}, {lat})";
        /*
        SELECT ST_DISTANCE_SPHERE(POINT(33.5207,  -86.8025), Geo) / 1000 distance, lat, lng,  address, storename
        FROM vendor
        WHERE ST_DISTANCE_SPHERE(POINT(33.5207, -86.8025), Geo) <= 50000
        order by distance;
        */
        return conn.Query<VendorPlus>($"SELECT ST_DISTANCE_SPHERE({point}, Geo) / 1000 distance, v.* FROM vendor v WHERE v.Approved and not v.IsDeleted and ST_DISTANCE_SPHERE({point}, Geo) <= @radius order by distance;", new { radius, lat, lng });
    }

    public ZipCode? NearestZip(double lng, double lat)
    {
        var point = $"POINT({lng}, {lat})";
        /*
        SELECT `Code`, City, State, ST_DISTANCE_SPHERE(POINT( -86.7461, 33.42872), Geo) / 1000 distance FROM zip 
        WHERE ST_DISTANCE_SPHERE(POINT( -86.7461, 33.42872), Geo) / 1000 <= 10
        order by distance 
        */
        var sql = $"SELECT `Code`, City, State, ST_DISTANCE_SPHERE({point}, Geo) / 1000 distance FROM zip WHERE ST_DISTANCE_SPHERE({point}, Geo) / 1000 <= 10 order by distance;";
        return conn.QueryFirstOrDefault<ZipCode>(sql);

    }

    public IEnumerable<Vendor> FindByState(string state)
    {
        if (state == "ALL")
            return conn.Query<Vendor>("select * from vendor v where Approved and not IsDeleted order by StoreName", new { state });
        return conn.Query<Vendor>("select * from vendor v where Approved and not IsDeleted and state = @state order by StoreName", new { state });
    }

    public IEnumerable<Vendor> FindByPlant(string plantName)
    {
        var term = $"%{plantName}%";
        return conn.Query<Vendor>("select * from vendor v inner join vendor_plant vp on vp.VendorId = v.Id inner join plant p on p.id = vp.PlantId where v.Approved and not v.IsDeleted and p.ScientificName like @term or p.CommonName like @term", new { term });
    }

    /// <summary>
    /// Returns vendor IDs that have at least one plant listing URL that is uncrawled or stale
    /// (LastSucceeded IS NULL or LastSucceeded &lt; 1 day ago). Only approved, non-deleted vendors.
    /// </summary>
    public IEnumerable<string> GetVendorIdsWithUncrawledUrls()
    {
        return conn.Query<string>(@"
            SELECT DISTINCT v.Id
            FROM vendor v
            INNER JOIN vendor_urls u ON u.VendorId = v.Id
            WHERE v.Approved AND NOT v.IsDeleted
              AND (u.LastSucceeded IS NULL OR u.LastSucceeded < DATE_SUB(NOW(), INTERVAL 1 DAY))
            ORDER BY v.Id");
    }
}