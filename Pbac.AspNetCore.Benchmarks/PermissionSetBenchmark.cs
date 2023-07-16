using BenchmarkDotNet.Attributes;

namespace Pbac.AspNetCore.Benchmarks;

[MemoryDiagnoser(displayGenColumns: false)]
public class PermissionSetBenchmark
{
    private readonly Random _random = new(Seed: 1_000_009);
    private readonly string _permissionString;
    private readonly Permissions _requestedPermission;

    public PermissionSetBenchmark()
    {
        _permissionString = _random.Next().ToString("X");
        _requestedPermission = (Permissions)_random.Next();
    }

    [Benchmark]
    public bool PermissionSet_HasPermission()
    {
        return PermissionSet<Permissions>.HasPermission(_permissionString, _requestedPermission);
    }

    private enum Permissions
    {
    }
}
