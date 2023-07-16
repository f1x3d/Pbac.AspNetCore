# Pbac.AspNetCore

## Description

A helper library for implementing the [permission/attribute based access control (ABAC)](https://en.wikipedia.org/wiki/Attribute-based_access_control) with [JSON web tokens](https://en.wikipedia.org/wiki/JSON_Web_Token) using the ASP.NET Core's [policy-based access control (PBAC)](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies).

* Scalable & compact - when converting a permission list to a string, it encodes 4 permissions in a single character ([see the implementation details below](#implementation)). This allows you to store [up to 1,024 permissions](https://learn.microsoft.com/en-us/graph/api/resources/extensionproperty?view=graph-rest-1.0#properties) in a single JWT claim in Azure AD B2C, for example.
* Performant and has low memory allocations ([see the benchmarks below](#benchmarks))

## Usage

1. Create the permissions enum:
    ```csharp
    public enum Permissions
    {
        Create = 0,
        Read = 1,
        Update = 2,
        Delete = 3,
    }
    ```

    Note that having the exact underlying value for each permission is important - messing it up will mess the permissions stored!

    Also, only enums with the `int` as an underlying type (the default) are currently supported.

1. Set the user permission values inside your identity provider using the `PermissionSet`:
    ```csharp
    user.Claims[ClaimNames.Permissions] = new PermissionSet<Permissions>(new[]
        {
            Permissions.Create,
            Permissions.Read,
        })
        .ToCompactString();
    ```

1. Add the permission policy requirement handler with the same claim name as above:
    ```csharp
    services.AddPermissionBasedAuthorization<Permissions>(options =>
        options.PermissionsClaimName = ClaimNames.Permissions);
    ```

    Note that if you re-define the authorization `DefaultPolicy` in your app then the code above should be inserted after your logic to make the permission policies inherit the new default policy.

1. (Optionally) Inherit the `AuthorizePermission<T>` with the specific permissions enum type:
    ```csharp
    public class AuthorizePermissionAttribute : AuthorizePermissionAttribute<Permissions>
    {
        public AuthorizePermissionAttribute(Permissions permission)
            : base(permission)
        { }
    }
    ```

    Or create an extension method if using the Minimal APIs:
    ```csharp
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, Permissions permission)
        where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder.RequireAuthorization(new AuthorizePermissionAttribute<Permissions>(permission));
    }
    ```

1. Decorate the controllers/endpoints with the `AuthorizePermission<T>` (or the one created above) attribute:
    ```csharp
    [HttpGet]
    [Authorize(Permissions.Read)]
    public IActionResult Get()
        => Ok();
    ```

    Or if using the minimal APIs:
    ```csharp
    app.MapGet("/", () => Results.Ok())
    .RequirePermission(Permissions.Read);
    ```

## Implementation

The permission set is being serialized into a string as a hex number where _N_-th bit represents the presence of a permission with the underlying value of _N_.

For example, consider the following permission enum:

```
public enum Permissions
{
    Create = 0,
    Read = 1,
    Update = 2,
    Delete = 3,
    Manage = 4,
}
```

Then, the binary representation of a set containing all permissions above would look like this:

```
   1    F - permission string
┌┬┬┤ ┌┬┬┤
0001 1111 - permission bit values
││││ ││││
7654 3210 - bit positions
└┬┘│ ││││
 │ │ │││└ Create
 │ │ ││└ Read
 │ │ │└ Update
 │ │ └ Delete
 │ └ Manage
 └ Not used
```

For deserialization, the above is done in the reversed order.

Also, because based on the enum value we can calculate the specific bit position we need to check, we don't need to deserialize the entire string when we verify a single permission.

## Benchmarks

* Checking a single permission value from the compact string:

    |                      Method |     Mean |    Error |   StdDev | Allocated |
    |---------------------------- |---------:|---------:|---------:|----------:|
    | PermissionSet_HasPermission | 74.17 ns | 1.382 ns | 1.225 ns |         - |

* Validating a user has a required permission in their claim list:

    |                                                         Method |     Mean |   Error |   StdDev | Allocated |
    |--------------------------------------------------------------- |---------:|--------:|---------:|----------:|
    | PermissionAuthorizationHandlerBenchmark_HandleRequirementAsync | 359.4 ns | 6.79 ns | 13.25 ns |     144 B |
