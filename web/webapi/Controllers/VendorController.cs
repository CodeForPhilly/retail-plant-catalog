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
    private readonly UserRepository userRepository;
    private readonly PlantRepository plantRepository;
    private readonly ZipRepository zipRepository;
    private readonly PlantCrawler plantCrawler;
    private readonly AmazonSimpleEmailServiceClient amazonSes;
    private readonly ILog logger;

    public VendorController(
        VendorRepository vendorRepository,
        UserRepository userRepository,
        PlantRepository plantRepository,
        ZipRepository zipRepository,
        PlantCrawler plantCrawler,
        AmazonSimpleEmailServiceClient amazonSes,
        ILog logger)
    {
        this.vendorRepository = vendorRepository;
        this.userRepository = userRepository;
        this.plantRepository = plantRepository;
        this.zipRepository = zipRepository;
        this.plantCrawler = plantCrawler;
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
        vendorRepository.Insert(vendor);
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
        var vendor = vendorRepository.Get(id);
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
        var vendor = vendorRepository.Get(id);
        if (vendor == null) return false;
        plantCrawler.Init();
        plantCrawler.Crawl(vendor).Wait();
        var plants = plantRepository.FindByVendor(vendor.Id);
        vendor.PlantCount = plants.Count();
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
            var populatedVendor = vendorRepository.Get(vendor.Id); //must get the plantlistingUrls
            if (!populatedVendor.PlantListingUrls.Any()) continue;
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



}