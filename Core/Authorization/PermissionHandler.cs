using Microsoft.AspNetCore.Authorization;

namespace Core.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Claims.Any(c => c.Type == "permissions" && c.Value.Contains(requirement.Permission)))
            context.Succeed(requirement);

        context.Fail(new AuthorizationFailureReason(this,
            $"Client does not have the required permission '{requirement.Permission}'."));
        return Task.CompletedTask;
    }
}