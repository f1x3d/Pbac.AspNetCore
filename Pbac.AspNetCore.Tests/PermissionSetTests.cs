namespace Pbac.AspNetCore.Tests;

public class PermissionSetTests
{
    [Theory]
    [InlineData("")]
    [InlineData("01234567890abcdefABCDEF")]
    public void Construct_WithHexString_Succeeds(string permissionString)
    {
        _ = new PermissionSet<Permissions>(permissionString);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("AG")]
    [InlineData("ag")]
    public void Construct_WithNonHexString_Throws(string permissionString)
    {
        var constructor = () => new PermissionSet<Permissions>(permissionString);

        constructor.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(new Permissions[] { })]
    [InlineData(new[] { Permissions.Zero })]
    [InlineData(new[] { Permissions.One, Permissions.NinetyNine })]
    public void Construct_WithNonNullPermissionList_Succeeds(IEnumerable<Permissions> permissionList)
    {
        _ = new PermissionSet<Permissions>(permissionList);
    }

    [Theory]
    [InlineData(null)]
    public void Construct_WithNullPermissionList_Fails(IEnumerable<Permissions> permissionList)
    {
        var constructor = () => new PermissionSet<Permissions>(permissionList);

        constructor.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(new[] { Permissions.Zero }, Permissions.Zero)]
    [InlineData(new[] { Permissions.NinetyNine }, Permissions.NinetyNine)]
    [InlineData(new[] { Permissions.One, Permissions.Two, Permissions.Thirteen }, Permissions.Two)]
    public void HasPermission_WithMatchingPermissionList_ReturnsTrue(
        IEnumerable<Permissions> permissionList,
        Permissions targetPermission)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionList);

        permissionSet.HasPermission(targetPermission).Should().BeTrue();
    }

    [Theory]
    [InlineData(new[] { (Permissions)(-1), Permissions.Zero, Permissions.One }, (Permissions)(-1))]
    [InlineData(new[] { Permissions.Zero, Permissions.NinetyNine, (Permissions)100 }, (Permissions)100)]
    public void HasPermission_WithUndefinedPermissionList_ReturnsFalse(
        IEnumerable<Permissions> permissionList,
        Permissions targetPermission)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionList);

        permissionSet.HasPermission(targetPermission).Should().BeFalse();
    }

    [Theory]
    [InlineData(new Permissions[] { }, Permissions.Zero)]
    [InlineData(new[] { Permissions.Zero }, Permissions.NinetyNine)]
    [InlineData(new[] { Permissions.NinetyNine }, Permissions.Zero)]
    [InlineData(new[] { Permissions.One, Permissions.Thirteen }, Permissions.Two)]
    public void HasPermission_WithoutMatchingPermissionList_ReturnsFalse(
        IEnumerable<Permissions> permissionList,
        Permissions targetPermission)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionList);

        permissionSet.HasPermission(targetPermission).Should().BeFalse();
    }

    [Theory]
    [InlineData("1"  /* 0000 0001 */, Permissions.Zero)]
    [InlineData("2"  /* 0000 0010 */, Permissions.One)]
    [InlineData("4"  /* 0000 0100 */, Permissions.Two)]
    [InlineData("8"  /* 0000 1000 */, Permissions.Three)]
    [InlineData("10" /* 0001 0000 */, Permissions.Four)]
    [InlineData("20" /* 0010 0000 */, Permissions.Five)]
    [InlineData("02" /* 0000 0010 */, Permissions.One)]
    [InlineData("3F" /* 0011 1111 */, Permissions.Three)]
    [InlineData("8000000000000000000000000", Permissions.NinetyNine)]
    public void HasPermission_WithMatchingPermissionString_ReturnsTrue(
        string permissionString,
        Permissions targetPermission)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionString);

        permissionSet.HasPermission(targetPermission).Should().BeTrue();
    }

    [Theory]
    [InlineData("10000000000000000000000000", (Permissions)100)]
    public void HasPermission_WithUndefinedPermissionString_ReturnsFalse(
        string permissionString,
        Permissions targetPermission)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionString);

        permissionSet.HasPermission(targetPermission).Should().BeFalse();
    }

    [Theory]
    [InlineData(""   /* 0000 0000 */, Permissions.Zero)]
    [InlineData("0"  /* 0000 0000 */, Permissions.Zero)]
    [InlineData("1"  /* 0000 0001 */, Permissions.NinetyNine)]
    [InlineData("01" /* 0000 0001 */, Permissions.One)]
    [InlineData("3"  /* 0000 0011 */, Permissions.Two)]
    [InlineData("7"  /* 0000 0111 */, Permissions.Three)]
    [InlineData("F"  /* 0000 1111 */, Permissions.Four)]
    [InlineData("1F" /* 0001 1111 */, Permissions.Five)]
    [InlineData("5"  /* 0000 0101 */, Permissions.One)]
    [InlineData("37" /* 0011 0111 */, Permissions.Three)]
    [InlineData("8000000000000000000000000", Permissions.Zero)]
    public void HasPermission_WithoutMatchingPermissionString_ReturnsFalse(
        string permissionString,
        Permissions targetPermission)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionString);

        permissionSet.HasPermission(targetPermission).Should().BeFalse();
    }

    [Theory]
    [InlineData("1"  /* 0000 0001 */, Permissions.Zero)]
    [InlineData("2"  /* 0000 0010 */, Permissions.One)]
    [InlineData("4"  /* 0000 0100 */, Permissions.Two)]
    [InlineData("8"  /* 0000 1000 */, Permissions.Three)]
    [InlineData("10" /* 0001 0000 */, Permissions.Four)]
    [InlineData("20" /* 0010 0000 */, Permissions.Five)]
    [InlineData("02" /* 0000 0010 */, Permissions.One)]
    [InlineData("3F" /* 0011 1111 */, Permissions.Three)]
    [InlineData("8000000000000000000000000", Permissions.NinetyNine)]
    public void HasPermissionStatic_WithMatchingPermission_ReturnsTrue(
        string permissionString,
        Permissions targetPermission)
    {
        var hasPermission = PermissionSet<Permissions>.HasPermission(permissionString, targetPermission);

        hasPermission.Should().BeTrue();
    }

    [Theory]
    [InlineData("10000000000000000000000000", (Permissions)100)]
    public void HasPermissionStatic_WithUndefinedPermission_ReturnsFalse(
        string permissionString,
        Permissions targetPermission)
    {
        var hasPermission = PermissionSet<Permissions>.HasPermission(permissionString, targetPermission);

        hasPermission.Should().BeFalse();
    }

    [Theory]
    [InlineData(""   /* 0000 0000 */, Permissions.Zero)]
    [InlineData("0"  /* 0000 0000 */, Permissions.Zero)]
    [InlineData("1"  /* 0000 0001 */, Permissions.NinetyNine)]
    [InlineData("01" /* 0000 0001 */, Permissions.One)]
    [InlineData("3"  /* 0000 0011 */, Permissions.Two)]
    [InlineData("7"  /* 0000 0111 */, Permissions.Three)]
    [InlineData("F"  /* 0000 1111 */, Permissions.Four)]
    [InlineData("1F" /* 0001 1111 */, Permissions.Five)]
    [InlineData("5"  /* 0000 0101 */, Permissions.One)]
    [InlineData("37" /* 0011 0111 */, Permissions.Three)]
    [InlineData("8000000000000000000000000", Permissions.Zero)]
    public void HasPermissionStatic_WithoutMatchingPermission_ReturnsFalse(
        string permissionString,
        Permissions targetPermission)
    {
        var hasPermission = PermissionSet<Permissions>.HasPermission(permissionString, targetPermission);

        hasPermission.Should().BeFalse();
    }

    [Theory]
    [InlineData(null, Permissions.One)]
    [InlineData("AG", Permissions.One)]
    [InlineData("ag", Permissions.One)]
    public void HasPermissionStatic_WithNonHexString_Throws(
        string permissionString,
        Permissions targetPermission)
    {
        var action = () => PermissionSet<Permissions>.HasPermission(permissionString, targetPermission);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(new Permissions[] { }, ""        /* 0000 0000 */)]
    [InlineData(new[] { Permissions.Zero }, "1"  /* 0000 0001 */)]
    [InlineData(new[] { Permissions.One }, "2"   /* 0000 0010 */)]
    [InlineData(new[] { Permissions.Two }, "4"   /* 0000 0100 */)]
    [InlineData(new[] { Permissions.Three }, "8" /* 0000 1000 */)]
    [InlineData(new[] { Permissions.Four }, "10" /* 0001 0000 */)]
    [InlineData(new[] { Permissions.Five }, "20" /* 0010 0000 */)]
    [InlineData(new[] { Permissions.Zero, Permissions.One, Permissions.Two, Permissions.Three }, "F" /* 1111 */)]
    [InlineData(new[] { Permissions.Three, Permissions.Seven }, "88" /* 1000 1000 */)]
    [InlineData(new[] { Permissions.Three, Permissions.Four }, "18" /* 0001 1000 */)]
    [InlineData(
        new[] { Permissions.Zero, Permissions.One, Permissions.Two, Permissions.Three, Permissions.Four },
        "1F" /* 0001 1111 */)]
    [InlineData(new[] { Permissions.NinetyNine }, "8000000000000000000000000")]
    public void ToCompactString_ReturnsPermissionListInHex(
        IEnumerable<Permissions> permissionList,
        string expectedPermissionString)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionList);

        var actualPermissionString = permissionSet.ToCompactString();

        actualPermissionString.Should().BeEquivalentTo(expectedPermissionString);
    }

    [Theory]
    [InlineData(new[] { Permissions.One, (Permissions)(-1) }, "2" /* 0010 */)]
    [InlineData(new[] { Permissions.One, (Permissions)100 }, "2"  /* 0010 */)]
    public void ToCompactString_WithUndefinedPermission_SkipsUndefinedPermission(
        IEnumerable<Permissions> permissionList,
        string expectedPermissionString)
    {
        var permissionSet = new PermissionSet<Permissions>(permissionList);

        var actualPermissionString = permissionSet.ToCompactString();

        actualPermissionString.Should().BeEquivalentTo(expectedPermissionString);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("0", "")]
    [InlineData("000", "")]
    [InlineData("1234567890abcdefABCDEF", "1234567890abcdefABCDEF")]
    [InlineData("01234567890abcdefABCDEF", "1234567890abcdefABCDEF")]
    [InlineData("0001234567890abcdefABCDEF", "1234567890abcdefABCDEF")]
    [InlineData("1234567890abcdefABCDEF0", "1234567890abcdefABCDEF0")]
    [InlineData("1234567890abcdefABCDEF000", "1234567890abcdefABCDEF000")]
    public void ToCompactString_TruncatesLeadingZeros(
        string inputPermissionString,
        string expectedPermissionString)
    {
        var permissionSet = new PermissionSet<Permissions>(inputPermissionString);

        var actualPermissionString = permissionSet.ToCompactString();

        actualPermissionString.Should().BeEquivalentTo(expectedPermissionString);
    }
}
