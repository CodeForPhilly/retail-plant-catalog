using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repositories;

namespace webapi
{
    /// <summary>
    /// Allows the request if the caller has a valid API key (Bearer token) or is authenticated with one of the given roles (e.g. cookie/session).
    /// Use when an endpoint must support both API clients and browser/client auth.
    /// </summary>
    public class ApiOrRolesAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public ApiOrRolesAuthorize(params string[] roles)
        {
            _roles = roles ?? System.Array.Empty<string>();
        }

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

            if (context.HttpContext.User?.Identity?.IsAuthenticated == true &&
                _roles.Length > 0 &&
                _roles.Any(r => context.HttpContext.User.IsInRole(r)))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Result = new JsonResult("NotAuthorized")
            {
                Value = new
                {
                    Status = "Error",
                    Message = "API token or one of the required roles is required."
                },
            };
        }
    }
}
