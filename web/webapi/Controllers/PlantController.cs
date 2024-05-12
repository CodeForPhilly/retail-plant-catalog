using FluentLogger.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Shared;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantController : BaseController
{
    private readonly PlantRepository plantRepository;
    private readonly ZipRepository zipRepository;
    private readonly ILog logger;

    public PlantController(PlantRepository plantRepository, ZipRepository zipRepository, ILog logger)
    {
        this.plantRepository = plantRepository;
        this.zipRepository = zipRepository;
        this.logger = logger;
    }
    /// <summary>
    /// Finds native plants by name
    /// </summary>
    /// <param name="plantName">Common or Scientific Name (partial names supported)</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByName")]
    public IEnumerable<Plant> FindByName([FromQuery] string plantName)
    {
        return plantRepository.FindAllByName(plantName);
    }
    /// <summary>
    /// Finds all the vendors that have the plant designated.
    /// </summary>
    /// <param name="plantId">uuid for plant</param>
    /// <param name="zipCode">5 digit zipcode</param>
    /// <param name="radius">in miles</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindVendorsForPlantId")]
    public IEnumerable<VendorPlus> FindVendorsForPlantId([FromQuery] string plantId, [FromQuery] string zipCode, [FromQuery] int radius)
    {
        var meters = (int)(radius * 1609.34);
        var zip = zipRepository.GetZipCode(zipCode);
        if (zip == null) return new List<VendorPlus> { };
        return plantRepository.FindVendorsForPlantId(plantId, zip.Lat, zip.Lng, meters);
    }
   


    /// <summary>
    /// Find vendors for plants by name and location
    /// </summary>
    /// <param name="plantName">Common or Scientific name of plant</param>
    /// <param name="zipCode">5 digit zipcode</param>
    /// <param name="radius">Radius of zipcode in miles</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindVendorsForPlantName")]
    public IEnumerable<VendorPlus> FindVendorsForPlantName([FromQuery] string plantName, [FromQuery] string zipCode, [FromQuery] int radius, [FromQuery]int limit=20)
    {
        var plants = FindByName(plantName);
        var vendors = new List<VendorPlus>();
        foreach (var plant in plants)
        {
            vendors.AddRange(FindVendorsForPlantId(plant.Id, zipCode, radius));
        }
        return vendors.Take(limit).DistinctBy(v => v.Id).OrderBy(v => v.Distance); 
    }
    /// <summary>
    /// Finds all plants by vendor.  See Vendor methods in order to get id
    /// </summary>
    /// <param name="vendorId">VendorId of vendor.  Use vendor methods to lookup vendor id</param>
    /// <returns></returns>
    [ApiAuthorize]
    [HttpGet]
    [Route("FindByVendor")]
    public IEnumerable<Plant> FindByVendor([FromQuery] string vendorId)
    {
        return plantRepository.FindByVendor(vendorId);
    }

    /// <summary>
    /// This is duplicated so that THIS application can use it.  Cannot use both APIAuthorize and Authorize attributes on the same method.
    /// </summary>
    /// <param name="vendorId"></param>
    /// <returns></returns>
    [Authorize(Roles ="Admin")]
    [ApiExplorerSettings(GroupName = "v2")]
    [HttpGet]
    [Route("FindByVendorInternal")]
    public IEnumerable<Plant> FindByVendorInternal([FromQuery] string vendorId)
    {
        return plantRepository.FindByVendor(vendorId);
    }


}