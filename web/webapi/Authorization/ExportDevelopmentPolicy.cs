using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Repositories;

namespace webapi.Authorization;

/// <summary>
/// Allows Export when any of: verified API key in Authorization (same as <see cref="webapi.ApiAuthorize"/>),
/// authenticated Admin, or Development environment (unauthenticated for local testing).
/// </summary>
public class ExportDevelopmentRequirement : IAuthorizationRequirement { }

public class ExportDevelopmentHandler : AuthorizationHandler<ExportDevelopmentRequirement>
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExportDevelopmentHandler(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ExportDevelopmentRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request.Headers.TryGetValue("Authorization", out var authTokens) == true)
        {
            var header = authTokens.FirstOrDefault() ?? "";
            if (!string.IsNullOrEmpty(header))
            {
                var token = header.Contains(' ', StringComparison.Ordinal)
                    ? header.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]
                    : header;
                if (!string.IsNullOrEmpty(token))
                {
                    var userRepository = httpContext.RequestServices.GetService<UserRepository>();
                    var user = userRepository?.FindByKey(token);
                    if (user?.Verified ?? false)
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }

                    return Task.CompletedTask;
                }
            }
        }

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (_env.IsDevelopment())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
