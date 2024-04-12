using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace webapi.Controllers;

public class BaseController : ControllerBase
{
    public IIdentity CurrentUser {  get
        {
            return HttpContext.User.Identity;
        }
    }
    public string UserId
    {
        get
        {
            var claim = User.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid");
            if (claim == null) return "";
            return claim.Value;
        }
    }
}