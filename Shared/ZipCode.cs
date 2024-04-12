using Dapper.Contrib.Extensions;
namespace Shared
{
    public class ZipCode
    {
        [ExplicitKey]
        public required string Code { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}