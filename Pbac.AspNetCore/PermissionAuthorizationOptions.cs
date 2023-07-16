namespace Pbac.AspNetCore;

public class PermissionAuthorizationOptions
{
    public string PermissionsClaimName { get; set; } = ClaimNames.Permissions;
}
