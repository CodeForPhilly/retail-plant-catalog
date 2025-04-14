namespace Repositories;

using Dapper;
using Shared;
using System.Data;

public class VendorUrlRepository : Repository<VendorUrl>
{
    public VendorUrlRepository(IDbConnection connection): base(connection)
    {

    }

    public IEnumerable<VendorUrl> FindForVendor(string vendorId)
    {
        return conn.Query<VendorUrl>("select * from vendor_urls where VendorId = @vendorId", new { vendorId });
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
        parameters.Add("@LastStatus", obj.LastStatus, DbType.String);
        
        return conn.Execute(
            "insert into vendor_urls (Id, Uri, VendorId, LastSucceeded, LastFailed, LastStatus) values (@Id, @Uri, @VendorId, @LastSucceeded, @LastFailed, @LastStatus)", 
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
        
        return await conn.ExecuteAsync(
            "insert into vendor_urls (Id, Uri, VendorId, LastSucceeded, LastFailed, LastStatus) values (@Id, @Uri, @VendorId, @LastSucceeded, @LastFailed, @LastStatus)", 
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

         conn.Execute(
            "update vendor_urls set Uri=@Uri, LastSucceeded=@LastSucceeded, LastFailed=@LastFailed, LastStatus=@LastStatus where Id =@Id",
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

        await conn.ExecuteAsync(
             "update vendor_urls set Uri=@Uri, LastSucceeded=@LastSucceeded, LastFailed=@LastFailed, LastStatus=@LastStatus where Id =@Id",
           parameters);
        return true;
    }
}

