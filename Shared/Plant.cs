using Dapper.Contrib.Extensions;

using System.Text.Json.Serialization;
namespace Shared
{
    [Table("plant")]
    public class Plant
    {
        [ExplicitKey]
        public string Id { get; set; }

        public string Symbol { get; set; }

        public string Blurb { get; set; }
        public decimal RecommendationScore { get; set; }
        
        public string ScientificName { get; set; }
         public string CommonName { get; set; }
        public bool Showy { get; set; }

        public string? FloweringMonths { get; set; }
        public string Height { get; set; }
        [JsonPropertyName("Superplant")]
        public bool SuperPlant { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("hasImage")]
        public bool HasImage { get; set; }

        [JsonPropertyName("hasPreview")]
        public bool HasPreview { get; set; }
        [JsonPropertyName("source")]

        public string Source { get; set; }
        [JsonPropertyName("attribution")]
        public string Attribution { get; set; }

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