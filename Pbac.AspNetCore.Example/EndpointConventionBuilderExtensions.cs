using Pbac.AspNetCore;
using Pbac.AspNetCore.Example;

namespace Microsoft.AspNetCore.Builder;

public static class EndpointConventionBuilderExtensions
{
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, Permissions permission)
        where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder.RequireAuthorization(new AuthorizePermissionAttribute<Permissions>(permission));
    }
}
