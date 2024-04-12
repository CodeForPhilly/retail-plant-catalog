using System.Net;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repositories;

namespace webapi
{
    public class ApiAuthorize : Attribute, IAuthorizationFilter
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
                context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";
                context.Result = new JsonResult("NotAuthorized")
                {
                    Value = new
                    {
                        Status = "Error",
                        Message = "Invalid Token"
                    },
                };
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Result = new JsonResult("NotAuthorized")
                {
                    Value = new
                    {
                        Status = "Error",
                        Message = "No Authorization token detected."
                    },
                };
                return;
            }

        }
    }
}