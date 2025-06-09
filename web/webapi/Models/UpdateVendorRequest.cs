namespace webapi.Models;

using System;
using System.ComponentModel.DataAnnotations;

public class UpdateVendorRequest
{
    [Required]
    public required string Id { get; init; }

    [Required]
    public required string StoreName { get; init; }

    [Required]
    public required string Address { get; init; }

    [Required]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "State must be a two-letter code.")]
    public required string State { get; init; }

    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public required decimal Lat { get; init; }

    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public required decimal Lng { get; init; }

    [Required]
    [Url(ErrorMessage = "Store URL must be a valid URL.")]
    [StringLength(500)]
    public required string StoreUrl { get; init; }

    [Required]
    [EmailAddress(ErrorMessage = "Public email must be a valid email address.")]
    public required string PublicEmail { get; init; }

    [Required]
    [Phone(ErrorMessage = "Public phone must be a valid phone number.")]
    [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Phone must be in the format (XXX) XXX-XXXX.")]
    public required string PublicPhone { get; init; }

    public bool AllNative { get; set; }

    public string Notes { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "At least one plant listing URL is required.")]
    public string[] PlantListingUrls { get; set; } = Array.Empty<string>();
}
