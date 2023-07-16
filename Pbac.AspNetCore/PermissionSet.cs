using System.Globalization;
using System.Runtime.CompilerServices;
using Pbac.AspNetCore.Extensions;

namespace Pbac.AspNetCore;

public class PermissionSet<T>
    where T : struct, Enum
{
    private const int BitsInHexChar = 4;

    private readonly ISet<T> _permissions;

    public PermissionSet(string permissionString)
    {
        _permissions = ParsePermissions(permissionString);
    }

    public PermissionSet(IEnumerable<T> permissions)
    {
        _permissions = new HashSet<T>(permissions.Where(Enum.IsDefined));
    }

    public bool HasPermission(T permission)
        => _permissions.Contains(permission);

    public static bool HasPermission(string permissionString, T permission)
    {
        if (!permissionString.IsHexString())
            throw new ArgumentException("The input is not a valid hex string.", nameof(permissionString));

        if (!Enum.IsDefined(permission))
            return false;

        var permissionIndex = Unsafe.As<T, int>(ref permission);
        var permissionCharIndex = permissionIndex / BitsInHexChar;
        var permissionByteOffset = permissionIndex % BitsInHexChar;

        if (permissionCharIndex >= permissionString.Length)
            return false;

        var permissionChar = permissionString.AsSpan().Slice(permissionString.Length - 1 - permissionCharIndex, 1);
        var permissionByte = byte.Parse(permissionChar, NumberStyles.HexNumber);

        return (permissionByte & (1 << permissionByteOffset)) != 0;
    }

    public string ToCompactString()
    {
        var result = new List<string>(); // TODO: Use Span<char> with fixed size
        var orderedPermissions = _permissions.OrderBy(x => Unsafe.As<T, int>(ref x));
        var currentByte = 0;

        foreach (var permission in orderedPermissions)
        {
            var permissionValue = Convert.ToInt32(permission);

            while (permissionValue / BitsInHexChar > result.Count)
            {
                result.Add($"{currentByte:X}");
                currentByte = 0;
            }

            var permissionValueOffset = permissionValue % BitsInHexChar;
            currentByte |= 1 << permissionValueOffset;
        }

        if (currentByte != 0)
            result.Add($"{currentByte:X}");

        result.Reverse();

        return string.Concat(result);
    }

    public override string ToString()
        => string.Join(", ", _permissions);

    private static ISet<T> ParsePermissions(string permissionString)
    {
        if (!permissionString.IsHexString())
            throw new ArgumentException("The input is not a valid hex string.", nameof(permissionString));

        var result = new HashSet<T>();
        var permissionValues = Enum.GetValues<T>().ToDictionary(x => Unsafe.As<T, int>(ref x));
        var permissionStringSpan = permissionString.AsSpan();

        for (var permissionIndex = 0; permissionIndex < permissionString.Length * BitsInHexChar; ++permissionIndex)
        {
            if (!permissionValues.TryGetValue(permissionIndex, out var permissionValue))
                continue;

            var permissionCharIndex = permissionIndex / BitsInHexChar;
            var permissionByteOffset = permissionIndex % BitsInHexChar;

            var permissionChar = permissionStringSpan.Slice(permissionString.Length - 1 - permissionCharIndex, 1);
            var permissionByte = byte.Parse(permissionChar, NumberStyles.HexNumber);

            if ((permissionByte & (1 << permissionByteOffset)) != 0)
                result.Add(permissionValue);
        }

        return result;
    }
}
