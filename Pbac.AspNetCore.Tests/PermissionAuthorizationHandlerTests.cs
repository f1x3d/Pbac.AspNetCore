using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Pbac.AspNetCore.Tests;

public class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithNoPermissionClaim_DoesNotSucceed()
    {
        var options = Options.Create<PermissionAuthorizationOptions>(new() { PermissionsClaimName = "Permissions" });
        var handler = new PermissionAuthorizationHandler<Permissions>(options);
        var user = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(options.Value.PermissionsClaimName + "Other", "1"),
            })
        });
        var permissionRequirement = new PermissionAuthorizationRequirement(nameof(Permissions.Zero));
        var context = new AuthorizationHandlerContext(new[] { permissionRequirement }, user, null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WithNonHexPermissionClaim_DoesNotSucceed()
    {
        var options = Options.Create<PermissionAuthorizationOptions>(new() { PermissionsClaimName = "Permissions" });
        var handler = new PermissionAuthorizationHandler<Permissions>(options);
        var user = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(options.Value.PermissionsClaimName, "AG"),
            })
        });
        var permissionRequirement = new PermissionAuthorizationRequirement(nameof(Permissions.Zero));
        var context = new AuthorizationHandlerContext(new[] { permissionRequirement }, user, null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WithUndefinedRequestedPermission_DoesNotSucceed()
    {
        var options = Options.Create<PermissionAuthorizationOptions>(new() { PermissionsClaimName = "Permissions" });
        var handler = new PermissionAuthorizationHandler<Permissions>(options);
        var user = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(options.Value.PermissionsClaimName, "1"),
            })
        });
        var permissionRequirement = new PermissionAuthorizationRequirement("MinusOne");
        var context = new AuthorizationHandlerContext(new[] { permissionRequirement }, user, null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WithoutMatchingPermission_DoesNotSucceed()
    {
        var options = Options.Create<PermissionAuthorizationOptions>(new() { PermissionsClaimName = "Permissions" });
        var handler = new PermissionAuthorizationHandler<Permissions>(options);
        var user = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(options.Value.PermissionsClaimName, "1"),
            })
        });
        var permissionRequirement = new PermissionAuthorizationRequirement(nameof(Permissions.One));
        var context = new AuthorizationHandlerContext(new[] { permissionRequirement }, user, null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WithMatchingPermission_Succeeds()
    {
        var options = Options.Create<PermissionAuthorizationOptions>(new() { PermissionsClaimName = "Permissions" });
        var handler = new PermissionAuthorizationHandler<Permissions>(options);
        var user = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(options.Value.PermissionsClaimName, "2"),
            })
        });
        var permissionRequirement = new PermissionAuthorizationRequirement(nameof(Permissions.One));
        var context = new AuthorizationHandlerContext(new[] { permissionRequirement }, user, null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }
}
