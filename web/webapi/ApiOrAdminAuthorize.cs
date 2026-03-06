using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repositories;

namespace webapi
{
    /// <summary>
    /// Allows the request if the caller has a valid API key (Bearer token) or is authenticated as Admin.
    /// </summary>
    public class ApiOrAdminAuthorize : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authTokens);
            var basicAuthentication = authTokens.FirstOrDefault() ?? "";

            if (!string.IsNullOrEmpty(basicAuthentication))
            {
                var token = basicAuthentication.Contains(' ') ? basicAuthentication.Split(' ')[1] : basicAuthentication;
                var userRepository = context.HttpContext.RequestServices.GetService<UserRepository>();
                var user = userRepository?.FindByKey(token);
                if (user?.Verified ?? false)
                {
                    return;
                }
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>()!.ReasonPhrase = "Not Authorized";
                context.Result = new JsonResult("NotAuthorized")
                {
                    Value = new
                    {
                        Status = "Error",
                        Message = "Invalid Token"
                    },
                };
                return;
            }

            if (context.HttpContext.User?.Identity?.IsAuthenticated == true && context.HttpContext.User.IsInRole("Admin"))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Result = new JsonResult("NotAuthorized")
            {
                Value = new
                {
                    Status = "Error",
                    Message = "API token or Admin role required."
                },
            };
        }
    }
}
