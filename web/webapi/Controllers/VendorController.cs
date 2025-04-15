using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Shared;
using webapi.Models;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using webapi.Services;
using RobotsParser;
using FluentLogger.Interfaces;

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

    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "User,Vendor,Admin")]
    [Route("Create")]
    public GenericResponse Create([FromBody] Vendor vendor)
    {
        logger.Info("Vendor creating", vendor);
        if (vendor.Id == null || vendor.Id == Guid.Empty.ToString())
            vendor.Id = Guid.NewGuid().ToString();
        vendor.UserId = UserId;
        var user = userRepository.Get(UserId);
        vendor.Approved = user.RoleEnum == UserType.Admin;
        if (user.RoleEnum == UserType.User)
        {
            user.RoleEnum = UserType.Vendor;
            userRepository.Update(user);
        }
        vendorService.CreateAsync(vendor).Wait();
        return new GenericResponse { Success = true, Id = vendor.Id, RedirectUrl = User.IsInRole("Admin") ? "/#/vendors" : "/#/"};
    }

    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,Vendor")]
    [Route("Update")]
    public GenericResponse Update([FromBody] Vendor vendor)
    {
        logger.Info("Updating vendor", vendor);
        var existingVendor = vendorRepository.Get(vendor.Id);
        if (existingVendor == null) return null;
        existingVendor.PublicEmail = vendor.PublicEmail;
        existingVendor.PublicPhone = vendor.PublicPhone;
        existingVendor.StoreName = vendor.StoreName;
        existingVendor.StoreUrl = vendor.StoreUrl;
        existingVendor.PlantListingUrls = vendor.PlantListingUrls;
        existingVendor.Address = vendor.Address;
        existingVendor.AllNative = vendor.AllNative;
        existingVendor.Lat = vendor.Lat;
        existingVendor.Lng = vendor.Lng;
        vendorRepository.Update(existingVendor);
        vendorService.SaveUrls(vendor.Id, vendor.PlantListingUris.Select(u => u.Uri).ToArray()).Wait();
        return new GenericResponse { Success = true, Message="Vendor update successful", RedirectUrl = User.IsInRole("Admin") ? "/#/vendors" : "/#/" };
    }


    [HttpGet]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize(Roles = "Admin,User,Vendor")] //Needs user for initial registration validation
    [Route("IsAllowed")]
    public async Task<GenericResponse> IsAllowed([FromQuery]string url)
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
        return vendor ?? new Vendor { UserId = UserId};
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
    public IEnumerable<Vendor> Search(string? storeName, string state, string sortBy="StoreName", bool sortAsc=true, int skip = 0, int take = 20, bool unapprovedOnly=false, bool showDeleted = false)
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
        
        // Count errors before crawling
        if (vendor.PlantListingUris != null)
        {
            vendor.CrawlErrors = vendor.PlantListingUris.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok);
            vendorRepository.Update(vendor);
        }
        
        plantCrawler.Init();
        plantCrawler.Crawl(vendor).Wait();
        var plants = plantRepository.FindByVendor(vendor.Id);
        vendor.PlantCount = plants.Count();
        vendor.CrawlErrors = vendor.PlantListingUris?.Select(u => u.LastStatus != CrawlStatus.Ok)?.Count() ?? 0 ;
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
                populatedVendor.CrawlErrors = populatedVendor.PlantListingUris.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok);
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
    public IEnumerable<VendorPlus> FindByRadius([FromQuery]double lat, [FromQuery]double lng, [FromQuery]int radius)
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
    public IEnumerable<VendorPlus> FindByZip([FromQuery] string zipcode, [FromQuery] int radius )
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
                LastStatus = result.Status
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
                
                vendor.CrawlErrors = allUrls.Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok);
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

    public class TestUrlRequest
    {
        public string Url { get; set; }
        public string VendorId { get; set; }
        public string? UrlId { get; set; }
    }
}