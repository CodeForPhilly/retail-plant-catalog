using System.Globalization;
using System.Text;
using FluentLogger.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Shared;

namespace webapi.Controllers;

/// <summary>
/// Bulk export endpoints returning CSV. Allowed for verified API key (Authorization header, same as other API routes),
/// authenticated Admin, or unauthenticated in Development only.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize(Policy = "ExportPolicy")]
public class ExportController : BaseController
{
    private readonly PlantRepository plantRepository;
    private readonly VendorRepository vendorRepository;
    private readonly VendorUrlRepository vendorUrlRepository;
    private readonly ILog logger;

    public ExportController(
        PlantRepository plantRepository,
        VendorRepository vendorRepository,
        VendorUrlRepository vendorUrlRepository,
        ILog logger)
    {
        this.plantRepository = plantRepository;
        this.vendorRepository = vendorRepository;
        this.vendorUrlRepository = vendorUrlRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Returns CSV of all plants.
    /// </summary>
    [HttpGet]
    [Route("Plants")]
    [Produces("text/csv")]
    public IActionResult Plants()
    {
        var plants = plantRepository.GetAll().ToList();
        var csv = BuildPlantsCsv(plants);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "Plants.csv");
    }

    /// <summary>
    /// Returns CSV of all vendors (nurseries). Optional filters: state, approved.
    /// </summary>
    [HttpGet]
    [Route("Nurseries")]
    [Produces("text/csv")]
    public IActionResult Nurseries([FromQuery] string? state = null, [FromQuery] bool? approved = null)
    {
        bool unapprovedOnly = approved == false;
        var vendors = vendorRepository.Find(null, state ?? "ALL", unapprovedOnly, showDeleted: false, "StoreName", true, 0, int.MaxValue).ToList();
        if (approved == true)
            vendors = vendors.Where(v => v.Approved).ToList();
        var csv = BuildNurseriesCsv(vendors);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "Nurseries.csv");
    }

    /// <summary>
    /// Returns CSV of vendor_urls table. Optional filters: state, approved.
    /// </summary>
    [HttpGet]
    [Route("NurseryPlantUrls")]
    [Produces("text/csv")]
    public IActionResult NurseryPlantUrls([FromQuery] string? state = null, [FromQuery] bool? approved = null)
    {
        var urls = vendorUrlRepository.GetAllForExport(state ?? "ALL", approved, includeDeleted: false).ToList();
        var csv = BuildNurseryPlantUrlsCsv(urls);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "Nursery_Plant_URLs.csv");
    }

    /// <summary>
    /// Returns CSV of vendor_plant junction table (VendorId, PlantId). Optional filters: state, approved.
    /// </summary>
    [HttpGet]
    [Route("PlantNursery")]
    [Produces("text/csv")]
    public IActionResult PlantNursery([FromQuery] string? state = null, [FromQuery] bool? approved = null)
    {
        var rows = plantRepository.GetAllVendorPlant(state ?? "ALL", approved, includeDeleted: false).ToList();
        var csv = BuildPlantNurseryCsv(rows);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "Plant_Nursery.csv");
    }

    private static string EscapeCsv(string? value)
    {
        if (value == null) return "";
        if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0)
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }

    private static string EscapeCsv(bool value) => value ? "true" : "false";
    private static string EscapeCsv(int value) => value.ToString(CultureInfo.InvariantCulture);
    private static string EscapeCsv(int? value) => value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : "";
    private static string EscapeCsv(decimal value) => value.ToString(CultureInfo.InvariantCulture);
    private static string EscapeCsv(DateTime value) => value.ToString("o", CultureInfo.InvariantCulture);
    private static string EscapeCsv(DateTime? value) => value.HasValue ? value.Value.ToString("o", CultureInfo.InvariantCulture) : "";
    private static string EscapeCsv(Enum value) => value.ToString();

    private static string BuildPlantsCsv(List<Plant> plants)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Symbol,ScientificName,CommonName,Blurb,RecommendationScore,Showy,FloweringMonths,Height,SuperPlant,ImageUrl,HasImage,HasPreview,Source,Attribution");
        foreach (var p in plants)
        {
            sb.AppendLine(string.Join(",",
                EscapeCsv(p.Id),
                EscapeCsv(p.Symbol),
                EscapeCsv(p.ScientificName),
                EscapeCsv(p.CommonName),
                EscapeCsv(p.Blurb),
                EscapeCsv(p.RecommendationScore),
                EscapeCsv(p.Showy),
                EscapeCsv(p.FloweringMonths),
                EscapeCsv(p.Height),
                EscapeCsv(p.SuperPlant),
                EscapeCsv(p.ImageUrl),
                EscapeCsv(p.HasImage),
                EscapeCsv(p.HasPreview),
                EscapeCsv(p.Source),
                EscapeCsv(p.Attribution)));
        }
        return sb.ToString();
    }

    private static string BuildNurseriesCsv(List<Vendor> vendors)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,UserId,StoreName,Address,State,Lat,Lng,StoreUrl,PublicEmail,PublicPhone,AllNative,Approved,PlantCount,Notes,CrawlErrors,CreatedAt,LastCrawled,LastChanged,LastCrawlStatus");
        foreach (var v in vendors)
        {
            sb.AppendLine(string.Join(",",
                EscapeCsv(v.Id),
                EscapeCsv(v.UserId),
                EscapeCsv(v.StoreName),
                EscapeCsv(v.Address),
                EscapeCsv(v.State),
                EscapeCsv(v.Lat),
                EscapeCsv(v.Lng),
                EscapeCsv(v.StoreUrl),
                EscapeCsv(v.PublicEmail),
                EscapeCsv(v.PublicPhone),
                EscapeCsv(v.AllNative),
                EscapeCsv(v.Approved),
                EscapeCsv(v.PlantCount),
                EscapeCsv(v.Notes),
                EscapeCsv(v.CrawlErrors),
                EscapeCsv(v.CreatedAt),
                EscapeCsv(v.LastCrawled),
                EscapeCsv(v.LastChanged),
                EscapeCsv(v.LastCrawlStatus)));
        }
        return sb.ToString();
    }

    private static string BuildNurseryPlantUrlsCsv(List<VendorUrl> urls)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,VendorId,Uri,LastSucceeded,LastFailed,LastStatus,PlantCount");
        foreach (var u in urls)
        {
            sb.AppendLine(string.Join(",",
                EscapeCsv(u.Id),
                EscapeCsv(u.VendorId),
                EscapeCsv(u.Uri),
                EscapeCsv(u.LastSucceeded),
                EscapeCsv(u.LastFailed),
                EscapeCsv(u.LastStatus),
                EscapeCsv(u.PlantCount)));
        }
        return sb.ToString();
    }

    private static string BuildPlantNurseryCsv(List<VendorPlantExportRow> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("VendorId,PlantId");
        foreach (var r in rows)
        {
            sb.AppendLine(string.Join(",", EscapeCsv(r.VendorId), EscapeCsv(r.PlantId)));
        }
        return sb.ToString();
    }
}
