namespace Repositories;

using Dapper;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;

public class PlantRepository : Repository<Plant>
{
    public PlantRepository(IDbConnection connection) : base(connection)
    {
    }

    public void Associate(string plantId, string vendorId)
    {
        conn.Execute("insert into vendor_plant (PlantId, VendorId) values(@plantId, @vendorId) on duplicate key update PlantId = @plantId", new { plantId, vendorId });
    }
    public void ClearAssociations(string vendorId)
    {
        conn.Execute("delete from vendor_plant where VendorId = @vendorId", new { vendorId });
    }

    public IEnumerable<Plant> FindAllByName(string plantName)
    {
        var term = $"%{plantName}%";
        return conn.Query<Plant>("select * from plant p where p.ScientificName like @term or p.CommonName like @term order by p.ScientificName", new { term });
    }

    public IEnumerable<Plant> FindByVendor(string vendorId)
    {
        return conn.Query<Plant>("select * from plant p inner join vendor_plant vp on vp.PlantId = p.Id where vp.VendorId = @vendorId order by p.CommonName", new { vendorId });
    }

    public IEnumerable<VendorPlus> FindVendorsForPlantId(string plantId, double lat, double lng, int radius)
    {
        var point = $"POINT({lng}, {lat})";
        var sql = $"SELECT ST_DISTANCE_SPHERE({point}, Geo) / 1000 distance, v.* FROM vendor v inner join vendor_plant vp on vp.VendorId = v.Id WHERE v.Approved and not v.IsDeleted and vp.PlantId = @plantId and ST_DISTANCE_SPHERE({point}, Geo) <= @radius order by distance;";
        return conn.Query<VendorPlus>(sql, new { radius, lat, lng, plantId });

    }

    public string[] GetTerms()
    {
        var query = @"select Symbol from plant union
                      select ScientificName from plant union
                      select CommonName from plant";
        return conn.Query<string>(query).ToArray();
    }
}