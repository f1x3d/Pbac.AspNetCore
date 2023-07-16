using System.Security.Claims;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Pbac.AspNetCore.Benchmarks;

[MemoryDiagnoser(displayGenColumns: false)]
public class PermissionAuthorizationHandlerBenchmark : PermissionAuthorizationHandler<Permissions>
{
    private readonly Random _random = new(Seed: 1_000_009);
    private readonly AuthorizationHandlerContext _context;
    private readonly PermissionAuthorizationRequirement _requirement;

    public PermissionAuthorizationHandlerBenchmark()
        : base(new Options())
    {
        var permissionString = _random.Next().ToString("X");
        var permissionsList = Enum.GetValues<Permissions>();
        var requestedPermission = permissionsList[_random.Next(0, permissionsList.Length)];
        var handlerOptions = new Options();
        var user = new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(handlerOptions.Value.PermissionsClaimName, permissionString)
            })
        });

        _context = new(Enumerable.Empty<IAuthorizationRequirement>(), user, null);
        _requirement = new(requestedPermission.ToString());
    }

    [Benchmark]
    public async Task PermissionAuthorizationHandlerBenchmark_HandleRequirementAsync()
    {
        await HandleRequirementAsync(_context, _requirement);
    }

    private class Options : IOptions<PermissionAuthorizationOptions>
    {
        public PermissionAuthorizationOptions Value { get; } = new();
    }
}
