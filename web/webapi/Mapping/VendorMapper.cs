using Shared;
using webapi.Models;

namespace webapi.Mapping;

public static class VendorMapper
{
    public static Vendor MapToVendor(CreateVendorRequest request, string userId)
    {
        return new Vendor
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            StoreName = request.StoreName,
            Address = request.Address,
            State = request.State,
            Lat = request.Lat,
            Lng = request.Lng,
            StoreUrl = request.StoreUrl,
            PublicEmail = request.PublicEmail,
            PublicPhone = request.PublicPhone,
            AllNative = request.AllNative,
            Notes = request.Notes ?? string.Empty,
            PlantListingUrls = request.PlantListingUrls.ToArray() ?? Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public static Vendor MapUpdateToVendor(UpdateVendorRequest request, Vendor existingVendor)
    {
        existingVendor.StoreName = request.StoreName;
        existingVendor.Address = request.Address;
        existingVendor.State = request.State;
        existingVendor.Lat = request.Lat;
        existingVendor.Lng = request.Lng;
        existingVendor.StoreUrl = request.StoreUrl;
        existingVendor.PublicEmail = request.PublicEmail;
        existingVendor.PublicPhone = request.PublicPhone;
        existingVendor.AllNative = request.AllNative;
        existingVendor.Notes = request.Notes;
        existingVendor.PlantListingUrls = request.PlantListingUrls.ToArray() ?? Array.Empty<string>();
        return existingVendor;
    }
}
