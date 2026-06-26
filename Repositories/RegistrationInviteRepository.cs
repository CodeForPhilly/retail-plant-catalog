using Dapper;
using Shared;
using System.Data;

namespace Repositories;

public class RegistrationInviteRepository : Repository<RegistrationInvite>
{
    public RegistrationInviteRepository(IDbConnection connection) : base(connection)
    {
    }

    public RegistrationInvite? GetByToken(string token)
    {
        return conn.QueryFirstOrDefault<RegistrationInvite>(
            "SELECT * FROM registration_invite WHERE Token = @token",
            new { token });
    }

    public void ExpirePendingForEmail(string email)
    {
        conn.Execute(
            @"UPDATE registration_invite
              SET Status = @expired
              WHERE Email = @email AND Status = @pending",
            new
            {
                email,
                expired = RegistrationInviteStatus.Expired.ToString(),
                pending = RegistrationInviteStatus.Pending.ToString()
            });
    }
}
