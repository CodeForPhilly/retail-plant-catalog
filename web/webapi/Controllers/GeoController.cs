using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration configuration;

        public GeoController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        /// <summary>
        /// Validates an address using Google Maps Geocoding API.
        /// </summary>
        /// <param name="address">The address to be validated e.g., "[street_number] [street_name] [city],[state] [zipcode]"</param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize]
        [Route("ValidateAddress")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateAddress([FromQuery] string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return BadRequest("Address is required");
            
            string apiKey = configuration["API_KEY"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return StatusCode(500, "Google Maps API is not configured");
            }

            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={System.Net.WebUtility.UrlEncode(address)}&key={apiKey}";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to contact geocoding service");

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("error_message", out var errorMessage))
            {
                // Can occur if restrictions are set on the Google MapsAPI key
                return StatusCode(400, $"Google Maps API error: {errorMessage.GetString()}");
            }

            if (root.GetProperty("status").GetString() != "OK")
            {
                var status = root.GetProperty("status").GetString();
                
                if (status == "ZERO_RESULTS")
                {
                    return NotFound("No address found");
                }
                
                return StatusCode(400, $"Google Maps API returned status: {status}");
            }

            var result = root.GetProperty("results")[0];
            var resolvedAddress = result.GetProperty("formatted_address").GetString();
            var location = result.GetProperty("geometry").GetProperty("location");
            var lat = location.GetProperty("lat").GetDouble();
            var lng = location.GetProperty("lng").GetDouble();

            return Ok(new
            {
                resolvedAddress,
                lat,
                lng
            });
        }
    }
}