namespace Repositories;

using Dapper;
using Shared;
using System.Data;

public class ZipRepository : Repository<ZipCode>
{
    public ZipRepository(IDbConnection connection) : base(connection)
    {
    }

    public ZipCode? GetZipCode(string zipcode)
    {
        return conn.QueryFirstOrDefault<ZipCode>("select * from zip where Code = @zipcode", new { zipcode });
    }
}