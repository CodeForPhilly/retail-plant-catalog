using Dapper.Contrib.Extensions;
namespace Shared
{
    public class ZipCode
    {
        [ExplicitKey]
        public required string Code { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

        public required string City { get; set; }
        public required string State { get; set; }
        public double Distance { get; set; }
    }
}