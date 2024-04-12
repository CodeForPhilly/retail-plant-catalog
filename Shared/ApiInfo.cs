using Dapper.Contrib.Extensions;
namespace Shared
{
    [Table("api_info")]
    public class ApiInfo
    {
        [ExplicitKey]
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? OrganizationName { get; set; }

        public string? Phone { get; set; }

        public string? IntendedUse { get; set; }
        public string? Name { get; set; }

        public string? Address { get; set; }
        public decimal Lng { get; set; }
        public decimal Lat { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}