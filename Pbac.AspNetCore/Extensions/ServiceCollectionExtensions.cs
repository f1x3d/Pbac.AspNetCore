using Pbac.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up authorization services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionBasedAuthorization<T>(
        this IServiceCollection services,
        Action<PermissionAuthorizationOptions>? configureOptions = null)
        where T : struct, Enum
    {
        if (configureOptions is not null)
            services.Configure(configureOptions);

        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IAuthorizationHandler, PermissionAuthorizationHandler<T>>());

        services.Configure<AuthorizationOptions>(options =>
            options.AddPermissionBasedPolicies<T>());

        return services;
    }
}
