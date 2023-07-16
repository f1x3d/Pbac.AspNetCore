using Pbac.AspNetCore;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up authorization services in an <see cref="IServiceCollection" />.
/// </summary>
public static class AuthorizationOptionsExtensions
{
    /// <summary>
    /// Adds permission-based authorization policies to the specified <see cref="AuthorizationOptions" />.
    /// </summary>
    /// <param name="options">The <see cref="AuthorizationOptions" /> to add policies to.</param>
    /// <returns>The <see cref="AuthorizationOptions"/> so that additional calls can be chained.</returns>
    public static AuthorizationOptions AddPermissionBasedPolicies<T>(this AuthorizationOptions options)
        where T : struct, Enum
    {
        var permissionNames = Enum.GetNames<T>();

        foreach (var permissionName in permissionNames)
        {
            options.AddPolicy(
                permissionName + PolicyNames.PermissionPolicyNameSuffix,
                policy => policy.AddRequirements(new PermissionAuthorizationRequirement(permissionName)));
        }

        return options;
    }
}
