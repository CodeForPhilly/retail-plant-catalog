namespace webapi.Models;

/// <summary>
/// Request body for <c>POST /Vendor/SetPartner</c>. <see cref="Partner"/> is the partner slug
/// (e.g. "Xerces") or null/blank to clear the link. <see cref="ExternalKey"/> is the partner-side
/// id for this vendor (optional).
/// </summary>
public class SetPartnerRequest
{
    public string VendorId { get; set; } = "";
    public string? Partner { get; set; }
    public string? ExternalKey { get; set; }
}
