using System.Data;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using FluentLogger.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RobotsParser;
using Shared;
using webapi.Mapping;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class VendorController : BaseController
{
    private readonly VendorRepository vendorRepository;
    private readonly VendorUrlRepository vendorUrlRepository;
    private readonly UserRepository userRepository;
    private readonly PlantRepository plantRepository;
    private readonly ZipRepository zipRepository;
    private readonly VendorService vendorService;
    private readonly PlantCrawler plantCrawler;
    private readonly AmazonSimpleEmailServiceClient amazonSes;
    private readonly ILog logger;
    // Queue-backed crawl execution. Controllers enqueue instead of awaiting
    // the crawl inline; the worker pool processes them at MaxConcurrentWorkers rate.
    private readonly ICrawlQueue crawlQueue;
    private readonly CrawlJobRepository crawlJobRepository;
    private readonly PartnerRepository partnerRepository;
    private readonly PartnerAuditRepository partnerAuditRepository;

    public VendorController(
        VendorRepository vendorRepository,
        VendorUrlRepository vendorUrlRepository,
        VendorService vendorService,
        UserRepository userRepository,
        PlantRepository plantRepository,
        ZipRepository zipRepository,
        PlantCrawler plantCrawler,
        AmazonSimpleEmailServiceClient amazonSes,
        ICrawlQueue crawlQueue,
        CrawlJobRepository crawlJobRepository,
        PartnerRepository partnerRepository,
        PartnerAuditRepository partnerAuditRepository,
        ILog logger)
    {
        this.vendorRepository = vendorRepository;
        this.vendorUrlRepository = vendorUrlRepository;
        this.userRepository = userRepository;
        this.plantRepository = plantRepository;
        this.zipRepository = zipRepository;
        this.plantCrawler = plantCrawler;
        this.vendorService = vendorService;
        this.amazonSes = amazonSes;
        this.crawlQueue = crawlQueue;
        this.crawlJobRepository = crawlJobRepository;
        this.partnerRepository = partnerRepository;
        this.partnerAuditRepository = partnerAuditRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Admin UI: list partner directories for the partner-tag dropdown on the vendor editor.
    /// Role-gated (cookie auth) so the admin SPA can call it.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,VolunteerPlus")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Partners")]
    public ActionResult<IEnumerable<Partner>> Partners()
    {
        return Ok(partnerRepository.List());
    }

    /// <summary>
    /// REST/MCP: list partner directories (Bearer auth). Same data as the UI's /Vendor/Partners,
    /// exposed to API consumers and the MCP <c>mcp_partners_list</c> tool to discover valid ids.
    /// </summary>
    [HttpGet]
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("PartnersApi")]
    public ActionResult<IEnumerable<Partner>> PartnersApi()
    {
        return Ok(partnerRepository.List());
    }

    /// <summary>
    /// Admin UI: set (or clear) a vendor's partner directory link (cookie auth). Kept separate from
    /// the normal vendor save (UpdateClient) so the regular save flow can never accidentally clear
    /// partner linkage. Empty/blank partner clears the link (vendor becomes unaffiliated).
    /// Admin and VolunteerPlus may operate on any vendor; Volunteer may operate only on the vendor
    /// record they own (matches existing UpdateClient scope rules).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,VolunteerPlus,Volunteer")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("SetPartner")]
    public IActionResult SetPartner([FromBody] SetPartnerRequest request)
    {
        var isAdminOrPlus = User.IsInRole("Admin") || User.IsInRole("VolunteerPlus");
        return SetPartnerCore(request, UserId, isAdminOrPlus);
    }

    /// <summary>
    /// REST/MCP: set (or clear) a vendor's partner directory link via Bearer auth. Pass an empty or
    /// blank <c>Partner</c> to clear the link. Used by external REST consumers and the MCP
    /// <c>set_vendor_partner</c> tool. Returns 400 for an unknown partner id, 404 if the vendor does
    /// not exist, 409 if the (partner, external key) pair collides with another vendor, 403 if the
    /// caller's role does not permit modifying this vendor.
    /// </summary>
    [HttpPost]
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("SetPartnerApi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult SetPartnerApi([FromBody] SetPartnerRequest request)
    {
        if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authTokens))
            return Unauthorized();
        var token = authTokens.ToString().Replace("Bearer ", "").Trim();
        var user = userRepository.FindByKey(token);
        if (user == null || !user.Verified)
            return StatusCode(403, new { success = false, message = "Invalid token." });

        var role = user.RoleEnum;
        if (role != UserType.Admin && role != UserType.VolunteerPlus && role != UserType.Volunteer)
            return StatusCode(403, new { success = false, message = "This endpoint requires Admin, VolunteerPlus, or Volunteer role." });

        var isAdminOrPlus = role == UserType.Admin || role == UserType.VolunteerPlus;
        return SetPartnerCore(request, user.Id!, isAdminOrPlus);
    }

    /// <summary>
    /// Shared partner-link logic for the cookie (UI) and Bearer (REST/MCP) endpoints.
    /// Enforces ownership for non-Admin/non-VolunteerPlus callers and writes an audit row
    /// to <c>partner_audit</c> on every successful change.
    /// </summary>
    private IActionResult SetPartnerCore(SetPartnerRequest request, string actingUserId, bool isAdminOrPlus)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.VendorId))
            return BadRequest(new { success = false, message = "VendorId is required." });

        // Fetch the vendor up front so we can (a) reject non-existent vendor with 404, (b) enforce
        // ownership for Volunteer-tier callers, (c) capture prior partner+key for the audit row.
        var vendor = vendorRepository.Get(request.VendorId);
        if (vendor == null)
            return NotFound(new { success = false, message = "Vendor not found." });

        if (!isAdminOrPlus && vendor.UserId != actingUserId)
            return StatusCode(403, new { success = false, message = "You may only set the partner on a vendor you own." });

        var partner = string.IsNullOrWhiteSpace(request.Partner) ? null : request.Partner.Trim();
        var externalKey = string.IsNullOrWhiteSpace(request.ExternalKey) ? null : request.ExternalKey.Trim();

        // Validate the partner id up front so callers get a clear 400 instead of a FK violation.
        if (partner != null && !partnerRepository.List().Any(p => string.Equals(p.Id, partner, StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { success = false, message = $"Unknown partner '{partner}'. Call /Vendor/PartnersApi for valid ids." });

        var priorPartner = vendor.Partner;
        var priorExternalKey = vendor.ExternalKey;

        try
        {
            var ok = vendorRepository.SetPartnerLink(request.VendorId, partner, externalKey);
            if (!ok)
                return NotFound(new { success = false, message = "Vendor not found." });

            // Append-only audit. Operation is denormalised from prior/new pair for ergonomic queries.
            PartnerAuditOperation op =
                partner == null      ? PartnerAuditOperation.Clear
              : priorPartner == null ? PartnerAuditOperation.Tag
                                     : PartnerAuditOperation.Update;
            try
            {
                partnerAuditRepository.Insert(new PartnerAudit
                {
                    VendorId         = request.VendorId,
                    Operation        = op,
                    PriorPartner     = priorPartner,
                    PriorExternalKey = priorExternalKey,
                    NewPartner       = partner,
                    NewExternalKey   = externalKey,
                    ActingUserId     = actingUserId,
                });
            }
            catch (Exception auditEx)
            {
                // Audit failure must not roll back a successful partner change. Log loudly and continue.
                logger.Error($"partner_audit insert failed for vendor {request.VendorId}; partner change applied but not audited.", auditEx);
            }

            return Ok(new { success = true, vendorId = request.VendorId, partner, externalKey });
        }
        catch (Exception ex)
        {
            // Most likely a duplicate (Partner, ExternalKey) hitting ux_vendor_partner_external_key.
            logger.Error($"SetPartner failed for vendor {request.VendorId}", ex);
            return Conflict(new { success = false, message = "Could not set partner link. The (partner, external key) pair may already be in use by another vendor." });
        }
    }

    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("List")]
    public IEnumerable<Vendor> List()
    {
        return vendorRepository.GetAll();
    }

    /// <summary>
    /// Creates a new vendor.
    /// </summary>
    /// <param name="request">The request containing the vendor's information.</param>
    /// <remarks>
    /// The request body should include:
    /// - <c>StoreName</c> (string, required): Name of the vendor's store.
    /// - <c>Address</c> (string, required): Street address of the vendor.
    /// - <c>State</c> (string, required): Two-letter state abbreviation (e.g., "TX").
    /// - <c>Lat</c> (decimal, required): Latitude coordinate.
    /// - <c>Lng</c> (decimal, required): Longitude coordinate.
    /// - <c>StoreUrl</c> (string, optional): Public-facing store URL (must be valid).
    /// - <c>PublicEmail</c> (string, optional): Contact email (must be valid).
    /// - <c>PublicPhone</c> (string, optional): Contact phone number (e.g., "123-456-7890").
    /// - <c>AllNative</c> (bool): Indicates if the vendor only sells native plants.
    /// - <c>Notes</c> (string, optional): Additional vendor notes.
    /// - <c>PlantListingUrls</c> (string[], optional): List of plant listing URLs.
    /// </remarks>
    [HttpPost]
    [ApiAuthorize]
    [Authorize(Roles = "User,Volunteer,VolunteerPlus,Admin")]
    [Route("Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Create([FromBody] CreateVendorRequest request)
    {
        return await CreateClientHelper(request, UserId);
    }
 
    [HttpPost]
    [Authorize]
    [Authorize(Roles = "User,Volunteer,VolunteerPlus,Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("/Vendor/CreateClient")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> CreateClient([FromBody] CreateVendorRequest request)
    {
        return await CreateClientHelper(request, UserId);
    }

    /// <summary>
    /// Create a vendor (Bearer auth). Validates for duplicate URLs; returns created vendor with populated URLs.
    /// </summary>
    [HttpPost]
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("/Vendor/CreateClientApi")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateClientApi([FromBody] CreateVendorRequest request)
    {
        if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authTokens))
            return StatusCode(403, new { message = "No Authorization token." });
        var token = authTokens.FirstOrDefault()?.Split(' ').LastOrDefault() ?? "";
        var user = userRepository.FindByKey(token);
        if (user == null || !user.Verified)
            return StatusCode(403, new { message = "Invalid token." });
        return await CreateClientHelper(request, user.Id!);
    }

    private async Task<IActionResult> CreateClientHelper(CreateVendorRequest request, string userId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // 400

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(); // 401

        try
        {
            var vendor = VendorMapper.MapToVendor(request, userId);

            var user = userRepository.Get(userId);

            if (user == null)
                return NotFound("User not found"); // 404

            if (user.RoleEnum == UserType.User)
            {
                user.RoleEnum = UserType.Volunteer;
                userRepository.Update(user);
            }

            vendor.Approved = user.RoleEnum == UserType.Admin || user.RoleEnum == UserType.VolunteerPlus;

            // Capture URLs before creating the vendor (CreateVendorRequest uses PlantListingUrls)
            var submittedUrls = (vendor.PlantListingUrls ?? Array.Empty<string>()).Where(u => !string.IsNullOrWhiteSpace(u)).ToArray();

            // Reject duplicate URLs in request
            var distinctUrls = submittedUrls.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (distinctUrls.Length != submittedUrls.Length)
                return BadRequest(new { message = "Duplicate plant listing URLs are not allowed. Please remove duplicates and try again." });

            // Reject if store URL is already used by another vendor
            var existingByStoreUrl = vendorRepository.GetByStoreUrl(request.StoreUrl?.Trim() ?? "");
            if (existingByStoreUrl != null)
                return BadRequest(new { message = "A vendor with this store URL already exists." });

            // Reject if any plant listing URL is already registered to another vendor
            foreach (var url in distinctUrls)
            {
                var existingByUri = vendorUrlRepository.GetByUriAnyVendor(url);
                if (existingByUri != null)
                    return BadRequest(new { message = $"A plant listing URL is already registered to another vendor: {url}" });
            }

            // Create the vendor first
            await vendorService.CreateAsync(vendor);

            // Now that we have a vendor ID, properly test and add URLs with validation status
            if (distinctUrls.Any())
            {
                plantCrawler.Init();
                foreach (var url in distinctUrls)
                {
                    try
                    {
                        // Test each URL
                        var result = await plantCrawler.TestUrl(url);

                        // Create VendorUrl with test results
                        string urlId = Guid.NewGuid().ToString();
                        var vendorUrl = new VendorUrl
                        {
                            Id = urlId,
                            Uri = url,
                            VendorId = vendor.Id,
                            LastStatus = result.Status,
                            LastFailed = result.Status != CrawlStatus.Ok ? DateTime.Now : null,
                            LastSucceeded = result.Status == CrawlStatus.Ok ? DateTime.UtcNow : null
                        };

                        // Save to database
                        await vendorUrlRepository.InsertAsync(vendorUrl);

                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error testing URL during vendor creation: {url}", ex);
                        // Still create the URL entry but mark it with an error status
                        await vendorUrlRepository.InsertAsync(new VendorUrl
                        {
                            Id = Guid.NewGuid().ToString(),
                            Uri = url,
                            VendorId = vendor.Id,
                            LastStatus = CrawlStatus.UrlParsingError,
                            LastFailed = DateTime.UtcNow
                        });
                    }
                }

                // Update the vendor with the proper crawl error count
                var updatedVendor = vendorService.GetPopulatedVendor(vendor.Id!);
                if (updatedVendor.PlantListingUris != null)
                {
                    updatedVendor.CrawlErrors = updatedVendor.PlantListingUris.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok);
                    vendorRepository.Update(updatedVendor);
                }
            }
            return Created(string.Empty, new
            {
                id = vendor.Id,
                success = true,
                redirectUrl = User.IsInRole("Admin") || User.IsInRole("VolunteerPlus") ? "/#/vendors" : "/#/"

            }); // 201
        }
        catch (Exception glex)
        {
            logger.Error("Unhandled exception during vendor creation", glex);

            return StatusCode(500, "An unexpected error occurred."); // 500
        }
    }

    /// <summary>
    /// Updates an existing vendor.
    /// </summary>
    /// <param name="request">The request containing the updated vendor information.</param>
    /// <returns></returns>
    /// <remarks>
    /// The request body should include:
    /// - <c>Id</c> (string, required): Unique identifier of the vendor to update.
    /// - <c>StoreName</c> (string, required): Name of the vendor's store.
    /// - <c>Address</c> (string, required): Street address of the vendor.
    /// - <c>State</c> (string, required): Two-letter state abbreviation (e.g., "TX").
    /// - <c>Lat</c> (decimal, required): Latitude coordinate of the location.
    /// - <c>Lng</c> (decimal, required): Longitude coordinate of the location.
    /// - <c>StoreUrl</c> (string, optional): Public-facing store URL (must be a valid URL).
    /// - <c>PublicEmail</c> (string, optional): Public email for contact (must be a valid email).
    /// - <c>PublicPhone</c> (string, optional): Public phone number (e.g., "123-456-7890").
    /// - <c>AllNative</c> (bool): Whether the vendor only sells native plants.
    /// - <c>Notes</c> (string, optional): Additional notes or metadata.
    /// - <c>PlantListingUrls</c> (string[], optional): List of URLs containing plant listings.
    /// </remarks>
    [HttpPut]
    [ApiAuthorize]
    [Route("Update")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] UpdateVendorRequest request)
    {
        return await VendorUpdate(request);
    }

    [HttpPut]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "User,Volunteer,VolunteerPlus,Admin")]
    [Route("UpdateClient")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateClient([FromBody] UpdateVendorRequest request)
    {
        return await VendorUpdate(request);
    }

    /// <summary>
    /// Update a vendor (Bearer auth). Validates duplicate URLs in list; returns updated vendor.
    /// </summary>
    [HttpPut]
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("UpdateClientApi")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClientApi([FromBody] UpdateVendorRequest request)
    {
        return await VendorUpdate(request);
    }

    private async Task<IActionResult> VendorUpdate(UpdateVendorRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            logger.Error($"Vendor update failed: invalid model state: {errors}");
            return BadRequest(ModelState); // 400
        }

        try
        {
            logger.Info("Updating vendor", request);
            Vendor existingVendor = vendorRepository.Get(request.Id);
            if (existingVendor == null)
            {
                logger.Warn($"Vendor not found: {request.Id}");
                return NotFound("Vendor not found."); // 404
            }

            // Update basic vendor information
            VendorMapper.MapUpdateToVendor(request, existingVendor);

            // Get existing URLs for this vendor
            var existingUrls = vendorUrlRepository.FindForVendor(existingVendor.Id!).ToList();

            // Get the list of URLs being submitted
            var submittedUrls = existingVendor.PlantListingUrls?.ToArray();
            if (submittedUrls == null) submittedUrls = Array.Empty<string>();

            // Find URLs that need to be removed (exist in DB but not in submission)
            var urlsToRemove = existingUrls.Where(u => !submittedUrls.Contains(u.Uri));
            foreach (var url in urlsToRemove)
            {
                vendorUrlRepository.Delete(url);
            }

            // Reject duplicate URLs in request
            var distinctSubmitted = submittedUrls.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (distinctSubmitted.Length != submittedUrls.Length)
            {
                logger.Warn($"Vendor update rejected: duplicate plant listing URLs for vendor {existingVendor.Id}");
                return BadRequest(new { message = "Duplicate plant listing URLs are not allowed. Please remove duplicates and try again." });
            }

            // Reject if store URL is already used by another vendor (exclude current vendor so it can keep its URL)
            var existingByStoreUrl = vendorRepository.GetByStoreUrlExcluding(request.StoreUrl?.Trim() ?? "", request.Id);
            if (existingByStoreUrl != null)
                return BadRequest(new { message = "A vendor with this store URL already exists." });

            // Reject if any plant listing URL is already registered to another vendor
            foreach (var url in distinctSubmitted)
            {
                var existingByUri = vendorUrlRepository.GetByUriAnyVendor(url);
                if (existingByUri != null && existingByUri.VendorId != request.Id)
                    return BadRequest(new { message = $"A plant listing URL is already registered to another vendor: {url}" });
            }

            // Save the submitted URLs
            var uri = await vendorService.TestAndSaveUrls(existingVendor.Id!, submittedUrls.ToArray(), plantCrawler);
            existingVendor.PlantListingUris = uri.ToArray();
            existingVendor.CrawlErrors = existingVendor.PlantListingUris?.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
            vendorRepository.Update(existingVendor);

            return Ok(new
            {
                message = "Vendor update successful",
                redirectUrl = User.IsInRole("Admin") || User.IsInRole("VolunteerPlus") ? "/#/vendors" : "/#/"
            }); // 200
        }
        catch (Exception ex)
        {
            logger.Error("Unhandled exception during vendor update", ex);
            return StatusCode(500, "An unexpected server error occurred."); // 500
        }
    }

    [HttpGet]
    [ApiOrRolesAuthorize("Admin", "User", "Volunteer", "VolunteerPlus")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("IsAllowed")]
    public async Task<GenericResponse> IsAllowed([FromQuery] string url)
    {
        try
        {
            var host = new Uri(url).Host;

            var robotsUrl = $"http://{host}/robots.txt";
            var robots = new Robots("PAC Agent");
            await robots.LoadRobotsFromUrl(robotsUrl);
            var disallowByDefault = robots?.GetDisallowedPaths()?.Any(u => u == "/") ?? false;
            var allowed = !disallowByDefault || (robots?.IsPathAllowed(url) ?? true);
            return new GenericResponse { Success = allowed };
        }
        catch (Exception)
        {
            return new GenericResponse { Success = true };
        }
    }
    /// <summary>
    /// Gets the vendor record for the currently authenticated user.
    /// </summary>
    /// <returns>Vendor for the current user, or a new Vendor with UserId set if none. Includes <c>LastCrawled</c> (UTC) when the vendor has been crawled.</returns>
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,Volunteer,VolunteerPlus,User")]
    [Route("Current")]
    public Vendor Current()
    {
        var vendor = vendorRepository.FindByUserId(UserId);
        return vendor ?? new Vendor { UserId = UserId };
    }
    /// <summary>
    /// Gets a vendor by id with populated plant listing URLs and crawl metadata.
    /// Includes overall LastCrawlStatus, LastCrawled, CrawlErrors, and per-URL LastStatus and PlantCount.
    /// </summary>
    /// <param name="id">Unique identifier for the vendor.</param>
    /// <returns>Vendor including <c>LastCrawled</c> (UTC), <c>LastCrawlStatus</c>, <c>CrawlErrors</c>, and per-URL status in <c>PlantListingUris</c> (LastStatus, PlantCount).</returns>
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Get")]
    public Vendor Get([FromQuery] string id)
    {
        var vendor = vendorService.GetPopulatedVendor(id);
        return vendor;
    }

    /// <summary>
    /// List vendors with optional state/approved filter and limit. For MCP and API clients (Bearer auth).
    /// </summary>
    [ApiAuthorize]
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("ListClient")]
    public IEnumerable<Vendor> ListClient([FromQuery] string? state = null, [FromQuery] bool? approved = null, [FromQuery] int limit = 50)
    {
        var unapprovedOnly = approved == false;
        return vendorRepository.Find(null, state ?? "ALL", unapprovedOnly, false, "StoreName", true, 0, Math.Min(Math.Max(limit, 1), 200));
    }

    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Search")]
    public IEnumerable<Vendor> Search(string? storeName, string state, string sortBy = "StoreName", bool sortAsc = true, int skip = 0, int take = 20, bool unapprovedOnly = false, bool showDeleted = false, string? partner = null)
    {
        return vendorRepository.Find(storeName, state, unapprovedOnly, showDeleted, sortBy, sortAsc, skip, take, partner);
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Approve")]
    public async Task<bool> Approve([FromBody] ApprovalRequest approvalRequest)
    {
        var vendor = vendorRepository.Get(approvalRequest.Id);
        vendorRepository.Approve(approvalRequest.Id, approvalRequest.Approved);
        if (!string.IsNullOrWhiteSpace(vendor?.PublicEmail))
        {
            try
            {
                if (approvalRequest.Approved)
                    await SendApprovalStatus(vendor.PublicEmail, "The Plant Agents Collective approves your status!");
                else
                    await SendApprovalStatus(vendor.PublicEmail, "Unfortunately the Plant Agents Collective has denied your status as a vendor for the following reason: " + (approvalRequest.DenialReason ?? ""));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to send approval email; vendor approval still applied.", ex);
            }
        }
        return true;
    }

    /// <summary>
    /// Approve or deny a vendor (Bearer auth). Caller must be Admin. Sends email notification.
    /// </summary>
    [HttpPost]
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("ApproveClient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveClient([FromBody] ApprovalRequest approvalRequest)
    {
        if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authTokens))
            return StatusCode(403, new { message = "No Authorization token." });
        var token = authTokens.FirstOrDefault()?.Split(' ').LastOrDefault() ?? "";
        var user = userRepository.FindByKey(token);
        if (user == null || !user.Verified)
            return StatusCode(403, new { message = "Invalid token." });
        if (user.RoleEnum != Shared.UserType.Admin)
            return StatusCode(403, new { message = "Admin role required." });

        var vendor = vendorRepository.Get(approvalRequest.Id);
        if (vendor == null) return NotFound("Vendor not found");

        vendorRepository.Approve(approvalRequest.Id, approvalRequest.Approved);
        if (!string.IsNullOrWhiteSpace(vendor.PublicEmail))
        {
            try
            {
                if (approvalRequest.Approved)
                    await SendApprovalStatus(vendor.PublicEmail, "The Plant Agents Collective approves your status!");
                else
                    await SendApprovalStatus(vendor.PublicEmail, "Unfortunately the Plant Agents Collective has denied your status as a vendor for the following reason: " + (approvalRequest.DenialReason ?? ""));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to send approval email; vendor approval still applied.", ex);
            }
        }
        return Ok(new { success = true });
    }
    private async Task SendApprovalStatus(string email, string copy)
    {
        if (string.IsNullOrWhiteSpace(email)) return;
        await amazonSes.SendEmailAsync(new SendEmailRequest
        {
            Source = "no-reply@plantagents.org",
            Destination = new Destination
            {
                ToAddresses = new[] { email }.ToList()
            },
            Message = new Message(new Content("Plant Agents Collective Vendor Status"),
            new Body
            {
                Html =
            new Content(copy
            )
            })
        });
    }
    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Crawl")]
    [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crawl([FromQuery] string id)
    {
        // Enqueue rather than running inline. Returns 202 + jobId; client polls
        // /Vendor/CrawlStatus for progress. Existing Vue admin "Crawl Site(s)" button
        // fires-and-redirects so the change is transparent to it.
        var vendor = vendorService.GetPopulatedVendor(id);
        if (vendor == null) return NotFound("Vendor not found");
        var jobId = await crawlQueue.EnqueueAsync(new CrawlJob
        {
            VendorId = id,
            Source = CrawlJobSource.UI,
            RequestedBy = UserId,
        });
        return Accepted(new
        {
            jobId,
            status = "queued",
            queueDepth = crawlQueue.QueueDepth,
        });
    }

    /// <summary>
    /// Trigger crawl for a vendor. If urlId is provided, crawls only that plant listing URL; otherwise crawls all URLs.
    /// Returns 409 if a crawl is already in progress.
    /// </summary>
    [HttpPost]
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("CrawlByUrl")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrawlByUrl([FromQuery] string id, [FromQuery] string? urlId = null)
    {
        // Enqueue either a full-vendor or single-URL job. Caller polls /CrawlStatus.
        var vendor = vendorService.GetPopulatedVendor(id);
        if (vendor == null) return NotFound("Vendor not found");

        if (!string.IsNullOrWhiteSpace(urlId))
        {
            // Validate the URL belongs to this vendor before enqueuing
            if (vendor.PlantListingUris == null || !vendor.PlantListingUris.Any(u => u.Id == urlId))
                return NotFound("URL not found on this vendor.");
        }

        var jobId = await crawlQueue.EnqueueAsync(new CrawlJob
        {
            VendorId = id,
            UrlId = string.IsNullOrWhiteSpace(urlId) ? null : urlId,
            Source = CrawlJobSource.UI,
            RequestedBy = UserId,
        });
        return Accepted(new
        {
            jobId,
            status = "queued",
            queueDepth = crawlQueue.QueueDepth,
        });
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]

    [Route("CrawlAll")]
    public Task<bool> CrawlAll()
    {
        plantCrawler.Init();
        foreach (var vendor in vendorRepository.GetAll())
        {
            if (vendor?.Id == null || !vendor.Approved) continue;
            var populatedVendor = vendorService.GetPopulatedVendor(vendor.Id); //must get the plantlistingUrls
            if (populatedVendor.PlantListingUrls == null || !populatedVendor.PlantListingUrls.Any()) continue;

            plantCrawler.Crawl(populatedVendor).Wait();
            populatedVendor.LastCrawled = DateTime.UtcNow;
            if (populatedVendor.PlantListingUris != null && populatedVendor.PlantListingUris.Length > 0)
            {
                var mostRecent = populatedVendor.PlantListingUris
                    .Select(u => new { u.LastStatus, Time = u.LastSucceeded ?? u.LastFailed })
                    .Where(x => x.Time != null)
                    .OrderByDescending(x => x.Time)
                    .FirstOrDefault();
                populatedVendor.LastCrawlStatus = mostRecent?.LastStatus ?? CrawlStatus.None;
            }
            else
            {
                populatedVendor.LastCrawlStatus = CrawlStatus.None;
            }
            // PlantCount is set inside Crawl() from the real association count, so it stays correct
            // even when a failed/empty crawl keeps the existing catalog instead of clearing it.
            populatedVendor.CrawlErrors = populatedVendor.PlantListingUris?.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
            vendorRepository.Update(populatedVendor);
        }
        return Task.FromResult(true);
    }


    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Delete")]
    public bool Delete(string id)
    {
        var vendor = vendorRepository.Get(id);
        if (vendor != null)
        {
            vendorRepository.Delete(id, true);
            return true;
        }
        return false;
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("UnDelete")]
    public bool UnDelete(string id)
    {
        var vendor = vendorRepository.Get(id);
        if (vendor != null)
        {
            vendorRepository.Delete(id, false);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Find vendors by location
    /// </summary>
    /// <param name="lat">Latitude centerpoint of the radius</param>
    /// <param name="lng">Longitude centerpoint of the radius</param>
    /// <param name="radius">Radius in miles</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByRadius")]
    public IEnumerable<VendorPlus> FindByRadius([FromQuery] double lat, [FromQuery] double lng, [FromQuery] int radius)
    {
        var meters = (int)(radius * 1609.34);
        return vendorRepository.FindByRadius(lng, lat, meters);
    }

    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v2")]
    [HttpGet]
    [Route("FindZip")]
    public ZipCode? FindZip([FromQuery] double lat, [FromQuery] double lng)
    {
        return vendorRepository.NearestZip(lng, lat);
    }
    [ApiAuthorize]
    [ApiExplorerSettings(GroupName = "v2")]
    [HttpGet]
    [Route("FindCity")]
    public ZipCode? FindCity([FromQuery] string zipCode)
    {
        return zipRepository.GetZipCode(zipCode);
    }




    /// <summary>
    /// Find vendors by State
    /// </summary>
    /// <param name="state">2 character abbr. of state</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByState")]
    public IEnumerable<Vendor> FindByState([FromQuery] string state)
    {
        return vendorRepository.FindByState(state);
    }
    /// <summary>
    /// Find vendors by plant name
    /// </summary>
    /// <param name="plantName">Common or Scientific name of plant</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByPlant")]
    public IEnumerable<Vendor> FindByPlant([FromQuery] string plantName)
    {
        return vendorRepository.FindByPlant(plantName);
    }
    /// <summary>
    /// Find vendors within radius of zipcode.
    /// </summary>
    /// <param name="zipcode">5 digit zipcode</param>
    /// <param name="radius">Radius in miles</param>
    /// <param name="includePlants">When true, each item includes a plants array for that vendor (default false)</param>
    /// <returns>Vendors, or when includePlants is true, array of { vendor, plants }</returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByZip")]
    public IActionResult FindByZip([FromQuery] string zipcode, [FromQuery] int radius, [FromQuery] bool includePlants = false)
    {
        if (string.IsNullOrWhiteSpace(zipcode) || zipcode.Length != 5 || !zipcode.All(char.IsDigit))
            return BadRequest("Invalid zipcode format. Must be 5 digits.");

        var zip = zipRepository.GetZipCode(zipcode);
        if (zip == null)
        {
            if (includePlants)
                return Ok(Array.Empty<object>());
            return Ok(new List<VendorPlus>());
        }

        var meters = (int)(radius * 1609.34);
        var vendors = vendorRepository.FindByRadius(zip.Lng, zip.Lat, meters).ToList();

        if (includePlants)
        {
            var result = vendors.Select(v => new { vendor = v, plants = plantRepository.FindByVendor(v.Id!) });
            return Ok(result);
        }

        return Ok(vendors);
    }

    [HttpPost]
    [ApiOrRolesAuthorize("Admin", "Volunteer", "VolunteerPlus")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("TestUrl")]
    public async Task<GenericResponse> TestUrl([FromBody] TestUrlRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Url))
                return new GenericResponse { Success = false, Message = "URL cannot be empty" };

            var vendor = vendorService.GetPopulatedVendor(request.VendorId);
            if (vendor == null)
                return new GenericResponse { Success = false, Message = "Vendor not found" };

            // Initialize the plant crawler (needed for term lookup)
            plantCrawler.Init();

            // Test the URL without doing the full crawl
            var result = await plantCrawler.TestUrl(request.Url);

            // Create or update VendorUrl with test results
            string urlId = request.UrlId ?? Guid.NewGuid().ToString();

            var vendorUrl = new VendorUrl
            {
                Id = urlId,
                Uri = request.Url,
                VendorId = vendor.Id,
                LastStatus = result.Status,
                LastFailed = result.Status != CrawlStatus.Ok ? DateTime.Now : null,
                LastSucceeded = null
            };

            if (result.Status == CrawlStatus.Ok)
            {
                vendorUrl.LastSucceeded = DateTime.UtcNow;
            }
            else
            {
                vendorUrl.LastFailed = DateTime.UtcNow;
            }

            // Duplicate check: if this URI already exists for vendor (and is a different row when updating), return error
            var existingByVendorAndUri = vendorUrlRepository.GetByVendorAndUri(vendor.Id!, request.Url);
            if (existingByVendorAndUri != null &&
                (string.IsNullOrEmpty(request.UrlId) || existingByVendorAndUri.Id != request.UrlId))
            {
                return new GenericResponse { Success = false, Message = "This plant listing URL is already registered for this vendor.", Id = existingByVendorAndUri.Id };
            }

            // Save the URL in the database by inserting or updating
            var existingUrl = vendorUrlRepository.GetByUrlOrId(vendorUrl);
            if (existingUrl != null)
            {
                vendorUrl.Id = existingUrl.Id;
                await vendorUrlRepository.UpdateAsync(vendorUrl);
            }
            else
            {
                await vendorUrlRepository.InsertAsync(vendorUrl);
            }

            // Update CrawlErrors count for the vendor
            if (vendor.PlantListingUris != null)
            {
                // Add this URL to the list if it's new
                var allUrls = vendor.PlantListingUris.ToList();
                if (!allUrls.Any(u => u.Id == urlId))
                {
                    allUrls.Add(vendorUrl);
                }
                else
                {
                    // Update the status in the list
                    var existingUrlInList = allUrls.First(u => u.Id == urlId);
                    existingUrlInList.LastStatus = result.Status;
                }

                vendor.CrawlErrors = allUrls?.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
                vendorRepository.Update(vendor);
            }

            return new GenericResponse
            {
                Success = result.Status == CrawlStatus.Ok,
                Message = result.Status.ToString(),
                Id = urlId
            };
        }
        catch (Exception ex)
        {
            logger.Error("Error testing URL", ex);
            return new GenericResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    [HttpPost]
    [ApiOrRolesAuthorize("Admin", "User", "Volunteer", "VolunteerPlus")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("ValidateUrl")]
    public async Task<GenericResponse> ValidateUrl([FromBody] ValidateUrlRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Url))
                return new GenericResponse { Success = false, Message = "URL cannot be empty" };

            // Check if URL is valid format
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
                return new GenericResponse { Success = false, Message = "Invalid URL format" };

            // Initialize the plant crawler (needed for term lookup)
            plantCrawler.Init();

            // Test the URL without doing the full crawl
            var result = await plantCrawler.TestUrl(request.Url);

            // Generate a temporary ID for the URL to be referenced in the UI
            string tempUrlId = Guid.NewGuid().ToString();

            return new GenericResponse
            {
                Success = result.Status == CrawlStatus.Ok,
                Message = result.Status.ToString(),
                Id = tempUrlId
            };
        }
        catch (Exception ex)
        {
            logger.Error("Error validating URL", ex);
            return new GenericResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    /// <summary>
    /// Finds vendor by Id
    /// </summary>
    /// <param name="id">Unique identifier for a vendor</param>
    /// <returns></returns>
    [HttpGet]
    [ApiAuthorize]
    [Route("FindById")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult FindById([FromQuery] string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest("Vendor Id is required");

        var vendor = vendorService.GetPopulatedVendor(id);

        if (vendor == null) return NotFound("Vendor does not exist");

        return Ok(vendor);
    }

    /// <summary>
    /// Crawls a vendor by its unique identifier.
    /// </summary>
    /// <param name="id">Unique identifier for a vendor</param>
    /// <returns></returns>
    [HttpPost]
    [ApiOrAdminAuthorize]
    [Route("CrawlByID")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrawlById([FromQuery] string id)
    {
        // Enqueue. Caller polls /CrawlStatus for progress.
        var vendor = vendorService.GetPopulatedVendor(id);
        if (vendor == null) return NotFound();
        var jobId = await crawlQueue.EnqueueAsync(new CrawlJob
        {
            VendorId = id,
            Source = CrawlJobSource.UI,
            RequestedBy = UserId,
        });
        return Accepted(new
        {
            jobId,
            status = "queued",
            queueDepth = crawlQueue.QueueDepth,
        });
    }

    /// <summary>
    /// Returns crawl status for a vendor (for polling). Includes <c>crawlStatus</c>, <c>lastCrawled</c>, CrawlInProgress, and per-URL CrawlInProgress.
    /// </summary>
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,Volunteer,VolunteerPlus")]
    [Route("CrawlStatus")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCrawlStatus([FromQuery] string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest("id is required");
        var vendor = vendorService.GetPopulatedVendor(id);
        if (vendor == null) return NotFound();
        // Surface the open crawl_job (if any) so callers can poll a single
        // endpoint to see "is something pending or running for this vendor?". The
        // legacy vendor.CrawlInProgress + per-URL flags are still returned for
        // backward compatibility with existing UI polling.
        var openJob = crawlJobRepository.GetOpenJobForVendor(id);
        return Ok(new
        {
            vendorId = vendor.Id,
            crawlInProgress = vendor.CrawlInProgress || openJob != null,
            crawlStatus = vendor.LastCrawlStatus,
            lastCrawled = vendor.LastCrawled,
            urlCrawlInProgress = vendor.PlantListingUris?.Select(u => new { urlId = u.Id, crawlInProgress = u.CrawlInProgress }).ToArray(),
            currentJob = openJob == null ? null : new
            {
                jobId = openJob.Id,
                status = openJob.Status.ToString(),
                source = openJob.Source.ToString(),
                urlId = openJob.UrlId,
                enqueuedAt = openJob.EnqueuedAt,
                startedAt = openJob.StartedAt,
            },
        });
    }

    /// <summary>
    /// FInds vendors and their plants within radius of zipcode
    /// </summary>
    /// <param name="zipcode">5 digit zipcode</param>
    /// <param name="radius">Radius in miles</param>
    /// <returns></returns>
    [HttpGet]
    [ApiAuthorize]
    [Route("WithPlantsByZip")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetVendorWithPlantsByZip([FromQuery] string zipcode, [FromQuery] int radius)
    {
        if (string.IsNullOrWhiteSpace(zipcode) || zipcode.Length != 5 || !zipcode.All(char.IsDigit))
        {
            return BadRequest("Invalid zipcode format. Must be 5 digits.");
        }

        var zip = zipRepository.GetZipCode(zipcode);
        if (zip == null) return NotFound("The zipcode was not found");

        // Debug output
        Console.WriteLine($"Zip: {zipcode}, Lat: {zip.Lat}, Lng: {zip.Lng}");

        var meters = (int)(radius * 1609.34);
        var vendors = vendorRepository.FindByRadius(zip.Lng, zip.Lat, meters).ToList();
       
        if (vendors.Count == 0) return NotFound("No vendors found in this radius");

        var result = vendors.Select(v => new {
            vendor = v,
            plants = plantRepository.FindByVendor(v.Id!)
        });
        return Ok(result);
    }

    /// <summary>
    /// Finds vendors and their plants by state
    /// </summary>
    /// <param name="state">2 character abbreviation of state</param>
    /// <returns>
    /// Returns:
    /// - 200 OK on successful update,
    /// - 400 Bad Request for invalid input,
    /// - 401 Unauthorized if the user is unauthenticated,
    /// - 403 Forbidden if the user does not have permission,
    /// - 404 Not Found if the vendor does not exist,
    /// - 500 Internal Server Error for unhandled exceptions.
    /// </returns>
    [HttpGet]
    [ApiAuthorize]
    [Route("WithPlantsByState")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetVendorWithPlantsByState([FromQuery] string state)
    {
        if (string.IsNullOrWhiteSpace(state) || state.Length != 2 || !state.All(char.IsLetter))
        {
            return BadRequest("Invalid state format. Must be 2 letters.");
        }

        var vendors = vendorRepository.FindByState(state);

        if (vendors.Count() == 0) return NotFound("No vendors found in this state");

        var result = vendors.Select(v => new {
            vendor = v,
            plants = plantRepository.FindByVendor(v.Id!)
        });
        return Ok(result);
    }
    public class TestUrlRequest
    {
        public string Url { get; set; } = null!;
        public string VendorId { get; set; } = null!;
        public string? UrlId { get; set; }
    }

    public class ValidateUrlRequest
    {
        public string Url { get; set; } = null!;
    }
}
