using Microsoft.AspNetCore.Mvc;
using Repositories;
using Shared;
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")] 
public class ApiInfoController: BaseController
{
    private readonly ApiInfoRepository apiInfoRepository;

    public ApiInfoController(ApiInfoRepository apiInfoRepository)
    {
        this.apiInfoRepository = apiInfoRepository;
    }

    [Authorize]
    [HttpPost]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("Create")]
    public async Task<GenericResponse> Create([FromBody] ApiInfo apiInfo)
    {
        var existingInfo = apiInfoRepository.FindByUserId(UserId);
        if (existingInfo == null)
        {
            apiInfo.UserId = UserId; //TODO:  Move api email logic etc to this from the user creation?
            apiInfo.CreatedAt = DateTime.Now;
            apiInfoRepository.Insert(apiInfo);
        }
        else
        {
            existingInfo.OrganizationName = apiInfo.OrganizationName;
            existingInfo.Phone = apiInfo.Phone;
            existingInfo.Lng = apiInfo.Lng;
            existingInfo.Lat = apiInfo.Lat;
            existingInfo.IntendedUse = apiInfo.IntendedUse?.Trim();
            existingInfo.Name = apiInfo.Name;
            apiInfoRepository.Update(existingInfo);
        }

        return new GenericResponse { Success = true, Message = "Api Info created Successfully" };
    }
}
