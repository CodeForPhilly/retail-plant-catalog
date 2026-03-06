using Dapper.Contrib.Extensions;

using System.Text.Json.Serialization;
namespace Shared
{
    [Table("plant")]
    public class Plant
    {
        [ExplicitKey]
        public string Id { get; set; } = null!;

        public string Symbol { get; set; } = null!;

        public string Blurb { get; set; } = null!;
        public decimal RecommendationScore { get; set; }
        
        public string ScientificName { get; set; } = null!;
        public string CommonName { get; set; } = null!;
        public bool Showy { get; set; }

        public string? FloweringMonths { get; set; }
        public string Height { get; set; } = null!;
        [JsonPropertyName("Superplant")]
        public bool SuperPlant { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = null!;
        [JsonPropertyName("hasImage")]
        public bool HasImage { get; set; }

        [JsonPropertyName("hasPreview")]
        public bool HasPreview { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; } = null!;
        [JsonPropertyName("attribution")]
        public string Attribution { get; set; } = null!;

        [Computed]
        public string[] Aliases { get
            {
                return new List<string> { Symbol, ScientificName, CommonName }.ToArray();
            }
        }
    }


    public class PlantImport
    {

    }
}