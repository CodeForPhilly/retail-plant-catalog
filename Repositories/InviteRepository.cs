namespace Repositories;

using Dapper;
using Shared;
using System.Data;

public class InviteRepository : Repository<Invite>
{
    public InviteRepository(IDbConnection connection): base(connection)
    {
    }
    public Invite Get(string id)
    {
        return conn.QueryFirstOrDefault<Invite>("select * from user_invite where Id = @id", new { id });
    }
    public override long Insert(Invite obj)
    {
        if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
        return base.Insert(obj);
    }
}