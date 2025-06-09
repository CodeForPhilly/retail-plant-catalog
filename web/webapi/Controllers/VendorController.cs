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

    public VendorController(
        VendorRepository vendorRepository,
        VendorUrlRepository vendorUrlRepository,
        VendorService vendorService,
        UserRepository userRepository,
        PlantRepository plantRepository,
        ZipRepository zipRepository,
        PlantCrawler plantCrawler,
        AmazonSimpleEmailServiceClient amazonSes,
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
        this.logger = logger;
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
    /// <returns>A 201 Created response with the created vendor ID, or an appropriate error.</returns>
    /// <remarks>
    /// <b>Request Body:</b>
    /// <list type="bullet">
    /// <item><term><c>StoreName</c> (string, required)</term><description>: Name of the vendor's store.</description></item>
    /// <item><term><c>Address</c> (string, required)</term><description>: Street address of the vendor.</description></item>
    /// <item><term><c>State</c> (string, required)</term><description>: Two-letter state abbreviation (e.g., "TX").</description></item>
    /// <item><term><c>Lat</c> (decimal, required)</term><description>: Latitude coordinate.</description></item>
    /// <item><term><c>Lng</c> (decimal, required)</term><description>: Longitude coordinate.</description></item>
    /// <item><term><c>StoreUrl</c> (string, optional)</term><description>: Public-facing store URL (must be valid).</description></item>
    /// <item><term><c>PublicEmail</c> (string, optional)</term><description>: Contact email (must be valid).</description></item>
    /// <item><term><c>PublicPhone</c> (string, optional)</term><description>: Contact phone number (e.g., "123-456-7890").</description></item>
    /// <item><term><c>AllNative</c> (bool)</term><description>: Indicates if the vendor only sells native plants.</description></item>
    /// <item><term><c>Notes</c> (string, optional)</term><description>: Additional vendor notes.</description></item>
    /// <item><term><c>PlantListingUrls</c> (string[], optional)</term><description>: List of plant listing URLs.</description></item>
    /// </list>
    ///
    /// <b>Response Codes:</b>
    /// <list type="bullet">
    /// <item><term>201 Created</term><description>: Vendor created successfully.</description></item>
    /// <item><term>400 Bad Request</term><description>: Invalid input data.</description></item>
    /// <item><term>401 Unauthorized</term><description>: Authentication required.</description></item>
    /// <item><term>403 Forbidden</term><description>: User lacks permission to perform this action.</description></item>
    /// <item><term>404 Not Found</term><description>: Related resource not found (if applicable).</description></item>
    /// <item><term>500 Internal Server Error</term><description>: An unexpected server error occurred.</description></item>
    /// </list>
    /// </remarks>
    [HttpPost]
    [ApiAuthorize]
    [Authorize(Roles = "User,Vendor,Admin")]
    [Route("Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Create([FromBody] CreateVendorRequest request)
    {
        return await CreateClientHelper(request);
    }
    /// <summary>
    /// Creates a new vendor.
    /// </summary>
    /// <param name="request">The request containing the vendor's information.</param>
    /// <returns>A 201 Created response with the created vendor ID, or an appropriate error.</returns>
    /// <remarks>
    /// <b>Request Body:</b>
    /// <list type="bullet">
    /// <item><term><c>StoreName</c> (string, required)</term><description>: Name of the vendor's store.</description></item>
    /// <item><term><c>Address</c> (string, required)</term><description>: Street address of the vendor.</description></item>
    /// <item><term><c>State</c> (string, required)</term><description>: Two-letter state abbreviation (e.g., "TX").</description></item>
    /// <item><term><c>Lat</c> (decimal, required)</term><description>: Latitude coordinate.</description></item>
    /// <item><term><c>Lng</c> (decimal, required)</term><description>: Longitude coordinate.</description></item>
    /// <item><term><c>StoreUrl</c> (string, optional)</term><description>: Public-facing store URL (must be valid).</description></item>
    /// <item><term><c>PublicEmail</c> (string, optional)</term><description>: Contact email (must be valid).</description></item>
    /// <item><term><c>PublicPhone</c> (string, optional)</term><description>: Contact phone number (e.g., "123-456-7890").</description></item>
    /// <item><term><c>AllNative</c> (bool)</term><description>: Indicates if the vendor only sells native plants.</description></item>
    /// <item><term><c>Notes</c> (string, optional)</term><description>: Additional vendor notes.</description></item>
    /// <item><term><c>PlantListingUrls</c> (string[], optional)</term><description>: List of plant listing URLs.</description></item>
    /// </list>
    ///
    /// <b>Response Codes:</b>
    /// <list type="bullet">
    /// <item><term>201 Created</term><description>: Vendor created successfully.</description></item>
    /// <item><term>400 Bad Request</term><description>: Invalid input data.</description></item>
    /// <item><term>401 Unauthorized</term><description>: Authentication required.</description></item>
    /// <item><term>403 Forbidden</term><description>: User lacks permission to perform this action.</description></item>
    /// <item><term>404 Not Found</term><description>: Related resource not found (if applicable).</description></item>
    /// <item><term>500 Internal Server Error</term><description>: An unexpected server error occurred.</description></item>
    /// </list>
    /// </remarks>
    [HttpPost]
    [Authorize]
    [Authorize(Roles = "User,Vendor,Admin")]
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
        return await CreateClientHelper(request);

    }

    private async Task<IActionResult> CreateClientHelper(CreateVendorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // 400

        var userId = UserId;

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
                user.RoleEnum = UserType.Vendor;
                userRepository.Update(user);
            }

            vendor.Approved = user.RoleEnum == UserType.Admin;

            // Capture URLs before creating the vendor
            var submittedUrls = vendor.PlantListingUris?.Select(u => u.Uri).ToArray() ?? Array.Empty<string>(); ;

            // Create the vendor first
            await vendorService.CreateAsync(vendor);

            // Now that we have a vendor ID, properly test and add URLs with validation status
            if (submittedUrls.Any())
            {
                plantCrawler.Init();
                foreach (var url in submittedUrls)
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
                redirectUrl = User.IsInRole("Admin") ? "/#/vendors" : "/#/"

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
    /// <returns>
    /// Returns:
    /// - 200 OK on successful update,
    /// - 400 Bad Request for invalid input,
    /// - 401 Unauthorized if the user is unauthenticated,
    /// - 403 Forbidden if the user does not have permission,
    /// - 404 Not Found if the vendor does not exist,
    /// - 500 Internal Server Error for unhandled exceptions.
    /// </returns>
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
    [Authorize(Roles = "User,Vendor,Admin")]
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
    [Authorize(Roles = "User,Vendor,Admin")]
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

            // Save the submitted URLs
            var uri = await vendorService.TestAndSaveUrls(existingVendor.Id!, submittedUrls.ToArray(), plantCrawler);
            existingVendor.PlantListingUris = uri.ToArray();
            existingVendor.CrawlErrors = existingVendor.PlantListingUris?.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
            vendorRepository.Update(existingVendor);

            return Ok(new
            {
                message = "Vendor update successful",
                redirectUrl = User.IsInRole("Admin") ? "/#/vendors" : "/#/"
            }); // 200
        }
        catch (Exception ex)
        {
            logger.Error("Unhandled exception during vendor update", ex);
            return StatusCode(500, "An unexpected server error occurred."); // 500
        }
    }

    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,User,Vendor")] //Needs user for initial registration validation
    [Route("IsAllowed")]
    public async Task<GenericResponse> IsAllowed([FromQuery] string url)
    {
        try
        {
            string host;
            bool robotsLoaded = false;
            robotsLoaded = false;
            host = new Uri(url).Host;

            var robotsUrl = $"http://{host}/robots.txt";
            var robots = new Robots("PAC Agent");
            await robots.LoadRobotsFromUrl(robotsUrl);
            var disallowByDefault = robots?.GetDisallowedPaths()?.Any(u => u == "/") ?? false;
            var allowed = !disallowByDefault || (robots?.IsPathAllowed(url) ?? true);
            return new GenericResponse { Success = allowed };
        }
        catch (Exception ex)
        {
            return new GenericResponse { Success = true };
        }
    }
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,Vendor,User")]
    [Route("Current")]
    public Vendor Current()
    {
        var vendor = vendorRepository.FindByUserId(UserId);
        return vendor ?? new Vendor { UserId = UserId };
    }
    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Get")]
    public Vendor Get([FromQuery] string id)
    {
        var vendor = vendorService.GetPopulatedVendor(id);
        return vendor;
    }

    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Search")]
    public IEnumerable<Vendor> Search(string? storeName, string state, string sortBy = "StoreName", bool sortAsc = true, int skip = 0, int take = 20, bool unapprovedOnly = false, bool showDeleted = false)
    {
        return vendorRepository.Find(storeName, state, unapprovedOnly, showDeleted, sortBy, sortAsc, skip, take);
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin")]
    [Route("Approve")]
    public async Task<bool> Approve([FromBody] ApprovalRequest approvalRequest)
    {
        var vendor = vendorRepository.Get(approvalRequest.Id);
        vendorRepository.Approve(approvalRequest.Id, approvalRequest.Approved);
        if (approvalRequest.Approved)
        {
            await SendApprovalStatus(vendor.PublicEmail, "The Plant Agents Collective approves your status!");
        }
        else
        {
            await SendApprovalStatus(vendor.PublicEmail, "Unfortunately the Plant Agents Collective has denied your status as a vendor for the following reason: " + approvalRequest.DenialReason);
        }
        return true;
    }
    private async Task SendApprovalStatus(string email, string copy)
    {
        await amazonSes.SendEmailAsync(new SendEmailRequest
        {
            Source = "fintech@savvyotter.net",
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
    public async Task<bool> Crawl([FromQuery] string id)
    {
        var vendor = vendorService.GetPopulatedVendor(id);
        if (vendor == null) return false;



        plantCrawler.Init();
        plantCrawler.Crawl(vendor).Wait();
        var plants = plantRepository.FindByVendor(vendor.Id);
        vendor.PlantCount = plants.Count();
        vendor.CrawlErrors = vendor.PlantListingUris?.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
        vendorRepository.Update(vendor);
        return true;
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]

    [Route("CrawlAll")]
    public async Task<bool> CrawlAll()
    {
        plantCrawler.Init();
        foreach (var vendor in vendorRepository.GetAll())
        {
            if (vendor?.Id == null || !vendor.Approved) continue;
            var populatedVendor = vendorService.GetPopulatedVendor(vendor.Id); //must get the plantlistingUrls
            if (!populatedVendor.PlantListingUrls.Any()) continue;

            // Count errors before crawling
            if (populatedVendor.PlantListingUris != null)
            {
                populatedVendor.CrawlErrors = populatedVendor.PlantListingUris?.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
                vendorRepository.Update(populatedVendor);
            }

            plantCrawler.Crawl(populatedVendor).Wait();
            var plants = plantRepository.FindByVendor(vendor.Id);
            populatedVendor.PlantCount = plants.Count();
            vendorRepository.Update(populatedVendor);
        }
        return true;
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
    /// Find Vendors by State
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
    /// Find Vendors by plant name
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
    /// Find vendors within radius of zipcode
    /// </summary>
    /// <param name="zipcode">5 digit zipcode</param>
    /// <param name="radius">Radius in miles</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByZip")]
    public IEnumerable<VendorPlus> FindByZip([FromQuery] string zipcode, [FromQuery] int radius)
    {
        var zip = zipRepository.GetZipCode(zipcode);
        if (zip == null) return new List<VendorPlus> { };
        return FindByRadius(zip.Lat, zip.Lng, radius);
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,Vendor")]
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
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,User,Vendor")]
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

    public class TestUrlRequest
    {
        public string Url { get; set; }
        public string VendorId { get; set; }
        public string? UrlId { get; set; }
    }

    public class ValidateUrlRequest
    {
        public string Url { get; set; }
    }
}
