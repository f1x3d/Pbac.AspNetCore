using Microsoft.AspNetCore.Authorization;

namespace Pbac.AspNetCore;

/// <summary>
/// Specifies that the class or method that this attribute is applied to requires the specified authorization.
/// </summary>
public class AuthorizePermissionAttribute<T> : AuthorizeAttribute
    where T : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class.
    /// </summary>
    public AuthorizePermissionAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class with the specified permission.
    /// </summary>
    /// <param name="permission">The permission to require for authorization.</param>
    public AuthorizePermissionAttribute(T permission)
        : base(permission.ToString() + PolicyNames.PermissionPolicyNameSuffix)
    { }
}
