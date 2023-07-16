using Microsoft.AspNetCore.Authorization;

namespace Pbac.AspNetCore;

public class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public PermissionAuthorizationRequirement(string permissionName)
    {
        PermissionName = permissionName;
    }
}
