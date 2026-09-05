using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareDividerInsetTests : FlareTestContext
{
    [Theory]
    [InlineData(DividerInset.Inset, Css.Classes.Divider.Inset)]
    [InlineData(DividerInset.MiddleInset, Css.Classes.Divider.MiddleInset)]
    public void Inset_AppliesModifier(DividerInset inset, string expected)
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Inset, inset));
        Assert.Contains(expected, cut.Find("hr").ClassName);
    }

    [Fact]
    public void None_HasNoInsetModifier()
    {
        var cut = Render<FlareDivider>();
        Assert.DoesNotContain("inset", cut.Find("hr").ClassName);
    }
}
