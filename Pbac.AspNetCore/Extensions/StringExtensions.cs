using System.Text.RegularExpressions;

namespace Pbac.AspNetCore.Extensions;

internal static partial class StringExtensions
{
    [GeneratedRegex(@"^[0-9A-F]*$", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex HexStringRegex();

    public static bool IsHexString(this string? input)
        => input is not null
        && HexStringRegex().IsMatch(input);
}
