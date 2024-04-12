namespace Repositories;

using Dapper;
using Dapper.Contrib.Extensions;
using Shared;
using System.Data;

public class UserRepository : Repository<User>
{
    public UserRepository(IDbConnection connection) :base(connection)
    {
    }

    public IEnumerable<User> Find(bool showAdminOnly, int skip, int take)
    {
        if (showAdminOnly)
            return conn.Query<User>("select u.*, api.IntendedUse from user u left join api_info api on api.UserId = u.Id where Role = 'Admin' order by email limit @skip, @take", new { skip, take });
        return conn.Query<User>("select u.*, api.IntendedUse from user u left join api_info api on api.Userid = u.Id order by email limit @skip, @take", new { skip, take });
    }

    public IEnumerable<User> Find(bool showAdminOnly, string? email, int skip, int take)
    {
        if (string.IsNullOrEmpty(email)) return Find(showAdminOnly, skip, take);
        email = $"%{email}%";
        if (showAdminOnly)
            return conn.Query<User>("select * from user where Role = 'Admin' and email like @email order by email limit @skip, @take", new { email, skip, take });
        return conn.Query<User>("select * from user where email like @email order by email limit @skip, @take", new { email, skip, take });
    }

    public User FindByEmail(string email)
    {
        return conn.QueryFirstOrDefault<User>("select * from user where Email = @email", new { email });
    }

    public User? FindByKey(string apiKey)
    {
        return conn.QueryFirstOrDefault<User>("select * from user where ApiKey = @apiKey", new { apiKey });
    }

    public string? GenApiKey(string id)
    {
        var apiKey = Guid.NewGuid().ToString();
        var rows = conn.Execute("update user set ApiKey = @apiKey where id = @id", new { id , apiKey});
        if (rows > 0)
            return apiKey;
        return null;
    }
}
