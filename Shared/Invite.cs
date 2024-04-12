using Dapper.Contrib.Extensions;
namespace Shared
{
    [Table("user_invite")]
    public class Invite
    {
        public Invite(string id, string userId, DateTime expiresAt, string? path)
        {
            Id = id;
            UserId = userId;
            ExpiresAt = expiresAt;
            Path = path ?? "#/login";
        }
        [ExplicitKey]
        public string Id { get; set; }

        public string Path { get; set; }

        public DateTime ExpiresAt { get; set; }
        public string UserId { get; set; }
    }
}