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

    /// <summary>
    /// List plants with optional name/scientificName filter and limit. Used by MCP and API.
    /// </summary>
    public IEnumerable<Plant> List(string? name = null, string? scientificName = null, int limit = 100)
    {
        var sql = "select * from plant p where 1=1";
        var parameters = new DynamicParameters();
        if (!string.IsNullOrWhiteSpace(name))
        {
            var nameTerm = $"%{name}%";
            sql += " and (p.CommonName like @nameTerm or p.ScientificName like @nameTerm)";
            parameters.Add("nameTerm", nameTerm);
        }
        if (!string.IsNullOrWhiteSpace(scientificName))
        {
            var sciTerm = $"%{scientificName}%";
            sql += " and p.ScientificName like @sciTerm";
            parameters.Add("sciTerm", sciTerm);
        }
        sql += " order by p.ScientificName limit @limit";
        parameters.Add("limit", Math.Min(Math.Max(limit, 1), 500));
        return conn.Query<Plant>(sql, parameters);
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

    /// <summary>
    /// Returns all vendor_plant rows, optionally filtered by vendor state/approved/deleted.
    /// </summary>
    public IEnumerable<VendorPlantExportRow> GetAllVendorPlant(string? state = null, bool? approved = null, bool includeDeleted = false)
    {
        if (string.IsNullOrEmpty(state) && approved == null)
        {
            return conn.Query<VendorPlantExportRow>("SELECT VendorId, PlantId FROM vendor_plant");
        }
        var sql = @"SELECT vp.VendorId, vp.PlantId FROM vendor_plant vp
INNER JOIN vendor v ON v.Id = vp.VendorId
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
        return conn.Query<VendorPlantExportRow>(sql, parameters);
    }
}