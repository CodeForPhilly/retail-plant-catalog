namespace Repositories;

using Dapper;
using Shared;
using System.Data;

public class VendorRepository : Repository<Vendor>
{
    public VendorRepository(IDbConnection connection) : base(connection)
    {

    }
    public override Vendor Get(string id)
    {
        var vendor = base.Get(id);
        vendor.PlantListingUrls = conn.Query<string>("select Uri from vendor_urls where VendorId = @vendorId", new { vendorId = vendor.Id }).ToArray();
        return vendor;
    }
    public IEnumerable<Vendor> Find(bool unapprovedOnly, string state, string sortBy, int skip, int take)
    {
        string stateConstraint = " 1=1";
        if (state != "ALL")
            stateConstraint = " State = @state ";
        if (unapprovedOnly)
            return conn.Query<Vendor>($"select * from vendor where Approved = false and {stateConstraint} order by {sortBy} limit @skip, @take", new { skip, take, state });
        return conn.Query<Vendor>($"select * from vendor where {stateConstraint} order by {sortBy} limit @skip, @take", new { skip, take, state });
    }

    public override long Insert(Vendor obj)
    {
        obj.CreatedAt = DateTime.UtcNow;
        if (obj.Id == null)
            obj.Id = Guid.NewGuid().ToString();
        string point = $"POINT({obj.Lat}, {obj.Lng})";
        var recordsAffected = conn.Execute("insert into vendor (Id, UserId, StoreName,  Lat, Lng, Geo,Approved, Address, State, StoreUrl, PublicEmail, PublicPhone, PlantCount, CreatedAt)" +
            $" values (@Id, @UserId, @StoreName, @Lat, @Lng, {point}, @Approved,@Address, @State,@StoreUrl, @PublicEmail, @PublicPhone, @PlantCount, @CreatedAt)", obj);
        ClearAndInsertUrls(obj.Id, obj.PlantListingUrls);
        return recordsAffected;
    }
    public override bool Update(Vendor obj)
    {
        ClearAndInsertUrls(obj.Id, obj.PlantListingUrls);
        string  point = $"POINT({obj.Lng}, {obj.Lat})";

        conn.Execute($"update vendor set StoreName=@StoreName, Address=@Address, Lng=@Lng, Lat=@Lat, Geo={point}, StoreUrl=@StoreUrl, PublicEmail=@PublicEmail, PublicPhone=@PublicPhone, Approved=@Approved, PlantCount=@PlantCount where id = @Id", obj);
        return true;
    }

    public IEnumerable<Vendor> Find(string? storeName, string state, bool unapprovedOnly, string sortBy, bool sortAsc, int skip, int take)
    {
        sortBy = $"{sortBy}" + (sortAsc ? "" : " desc");
        if (string.IsNullOrEmpty(storeName)) return Find(unapprovedOnly,state, sortBy, skip, take);
        string stateConstraint = "";
        if (state != "ALL")
            stateConstraint = " and State = @state ";
        storeName = $"%{storeName}%";
        if (unapprovedOnly)
            return conn.Query<Vendor>($"select * from vendor where StoreName like @storeName {stateConstraint} and Approved = false order by {sortBy} limit @skip, @take", new { storeName, skip, take, state });
        return conn.Query<Vendor>($"select * from vendor where StoreName like @storeName {stateConstraint} order by {sortBy} limit @skip, @take", new { storeName, skip, take, state });
    }

    public void Approve(string id, bool approved)
    {
        var rowsAffected = conn.Execute("update vendor set Approved = @approved where Id = @id", new { id, approved });

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

    public Vendor? FindByUserId(string userId)
    {
        var vendor = conn.QueryFirstOrDefault<Vendor>("select * from vendor where UserId = @userId", new { userId });
        if (vendor == null) return null;
        vendor.PlantListingUrls = conn.Query<string>("select Uri from vendor_urls where VendorId = @vendorId", new { vendorId = vendor.Id }).ToArray();
        return vendor;
    }

    public IEnumerable<VendorPlus> FindByRadius(double lng, double lat, int radius = 10000){ // 10km
        var point = $"POINT({lat}, {lng})";
        /*
        SELECT ST_DISTANCE_SPHERE(POINT(33.5207,  -86.8025), Geo) / 1000 distance, lat, lng,  address, storename
        FROM vendor
        WHERE ST_DISTANCE_SPHERE(POINT(33.5207, -86.8025), Geo) <= 50000
        order by distance;
        */
        return conn.Query<VendorPlus>($"SELECT ST_DISTANCE_SPHERE({point}, Geo) / 1000 distance, v.* FROM vendor v WHERE v.Approved and ST_DISTANCE_SPHERE({point}, Geo) <= @radius order by distance;", new { radius, lat, lng });
    }

    public IEnumerable<Vendor> FindByState(string state)
    {
        return conn.Query<Vendor>("select * from vendor v where Approved and state = @state order by StoreName", new { state });
    }

    public IEnumerable<Vendor> FindByPlant(string plantName)
    {
        var term = $"%{plantName}%";
        return conn.Query<Vendor>("select * from vendor v inner join vendor_plant vp on vp.VendorId = v.Id inner join plant p on p.id = vp.PlantId where v.Approved and p.ScientificName like @term or p.CommonName like @term", new { term });
    }
}