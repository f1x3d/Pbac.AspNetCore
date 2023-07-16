namespace Pbac.AspNetCore.Example;

public class AuthorizePermissionAttribute : AuthorizePermissionAttribute<Permissions>
{
    public AuthorizePermissionAttribute(Permissions permission)
        : base(permission)
    { }
}
