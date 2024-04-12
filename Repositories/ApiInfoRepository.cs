namespace Repositories;

using Dapper;
using Shared;
using System.Data;

public class ApiInfoRepository : Repository<ApiInfo>
{
    public ApiInfoRepository(IDbConnection connection) : base(connection)
    {
    }

    public ApiInfo? FindByUserId(string userId)
    {
        return conn.QueryFirstOrDefault<ApiInfo>("select * from api_info where UserId = @userId", new { userId });
    }
}