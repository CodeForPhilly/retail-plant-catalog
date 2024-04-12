using System.ComponentModel;
using System;
using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;
namespace Shared
{
    public class UserRequest
    {
        public User User { get; set; }
        public string RedirectUrl { get; set; }
    }
    [Table("user")]
    public class User
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }
        [ExplicitKey]
        public string? Id { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string? HashedPassword { get; set; }

        [Computed]
        public string? Password { get; set; }

        public string? Role { get; set; }

        [JsonIgnore]
        [Computed]
        public UserType? RoleEnum
        {
            get
            {
                if (string.IsNullOrEmpty(Role)) return null;
                return (UserType)Enum.Parse(typeof(UserType), Role);
            }
            set => Role = value?.ToString();
        }
        [Computed]
        public string? IntendedUse { get; set; }

        public string? ApiKey { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public bool Verified { get; set; }
    }
    public enum UserType
    {
        [Description("User")]
        User = 0,
        [Description("Admin")]
        Admin = 1,
        [Description("Vendor")]
        Vendor =2
    }
}