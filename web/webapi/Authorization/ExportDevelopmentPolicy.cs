using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace webapi.Authorization;

/// <summary>
/// In Development, allows unauthenticated access to Export endpoints for testing (e.g. curl).
/// In non-Development, requires Admin role.
/// </summary>
public class ExportDevelopmentRequirement : IAuthorizationRequirement { }

public class ExportDevelopmentHandler : AuthorizationHandler<ExportDevelopmentRequirement>
{
    private readonly IWebHostEnvironment _env;

    public ExportDevelopmentHandler(IWebHostEnvironment env)
    {
        _env = env;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ExportDevelopmentRequirement requirement)
    {
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
