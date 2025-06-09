using System.ComponentModel.DataAnnotations;

namespace webapi.Models;

public class CreateVendorRequest
{
    [Required]
    public required string StoreName { get; init; }

    [Required]
    public required string Address { get; init; }

    [Required]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "State must be exactly 2 characters.")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "State must be two uppercase letters (e.g., 'TX').")]
    public required string State { get; init; }

    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public decimal Lat { get; init; }

    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public decimal Lng { get; init; }

    [Required]
    [Url(ErrorMessage = "StoreUrl must be a valid URL.")]
    public required string StoreUrl { get; init; }

    [Required]
    [EmailAddress(ErrorMessage = "Public email must be a valid email address.")]
    public required string PublicEmail { get; init; }

    [Required]
    [Phone(ErrorMessage = "Public phone must be a valid phone number.")]
    [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Phone must match format (###) ###-####.")]
    public required string PublicPhone { get; init; }

    public bool AllNative { get; init; }

    public string Notes { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least one PlantListingUrl is required.")]
    [MinLength(1, ErrorMessage = "At least one PlantListingUrl is required.")]
    public string[] PlantListingUrls { get; init; } = Array.Empty<string>();
}
