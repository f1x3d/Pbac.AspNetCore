using Pbac.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Pbac.AspNetCore;

public class PermissionAuthorizationHandler<T> : AuthorizationHandler<PermissionAuthorizationRequirement>
    where T : struct, Enum
{
    private readonly PermissionAuthorizationOptions _options;

    public PermissionAuthorizationHandler(IOptions<PermissionAuthorizationOptions> options)
    {
        _options = options.Value;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        var permissionClaim = context.User.FindFirst(c => c.Type == _options.PermissionsClaimName);

        if (permissionClaim is null)
            return Task.CompletedTask;

        if (!permissionClaim.Value.IsHexString())
            return Task.CompletedTask;

        if (!Enum.TryParse<T>(requirement.PermissionName, out var requestedPermission))
            return Task.CompletedTask;

        if (PermissionSet<T>.HasPermission(permissionClaim.Value, requestedPermission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
