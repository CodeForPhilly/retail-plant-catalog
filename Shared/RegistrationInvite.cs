using Dapper.Contrib.Extensions;

namespace Shared;

public enum RegistrationInviteStatus
{
    Pending,
    Accepted,
    Expired
}

[Table("registration_invite")]
public class RegistrationInvite
{
    [ExplicitKey]
    public string Id { get; set; } = "";

    public string Token { get; set; } = "";

    public string Email { get; set; } = "";

    /// <summary>Target <see cref="UserType"/> name (e.g. User, Admin, Volunteer, VolunteerPlus).</summary>
    public string Role { get; set; } = UserType.User.ToString();

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string Status { get; set; } = RegistrationInviteStatus.Pending.ToString();

    public string? InvitedByUserId { get; set; }

    public string? AcceptedUserId { get; set; }
}
