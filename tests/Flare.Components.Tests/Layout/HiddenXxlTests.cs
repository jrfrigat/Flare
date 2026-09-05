using Flare.Components;
using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class HiddenXxlTests : FlareTestContext
{
    [Theory]
    [InlineData(Css.Classes.Hidden.OnlyXxl)]
    public void Only_Xxl_EmitsClass(string expected)
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Only, Breakpoint.Xxl).AddChildContent("<i>x</i>"));
        Assert.Contains(expected, cut.Markup);
    }

    [Fact]
    public void Below_Xxl_EmitsClass()
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Below, Breakpoint.Xxl).AddChildContent("<i>x</i>"));
        Assert.Contains(Css.Classes.Hidden.BelowXxl, cut.Markup);
    }

    [Fact]
    public void Above_Xl_EmitsClass()
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Above, Breakpoint.Xl).AddChildContent("<i>x</i>"));
        Assert.Contains(Css.Classes.Hidden.AboveXl, cut.Markup);
    }

    // The class used to be assembled from the breakpoint name, which could assemble one that does not
    // exist: there is no tier below the smallest and none above the largest, so neither rule is in the
    // stylesheet. The element then stayed visible with nothing on it to say why - and no test could
    // have caught it, because the name never appeared in the source AS a name.
    [Fact]
    public void Below_TheSmallestTier_EmitsNothing()
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Below, Breakpoint.Xs).AddChildContent("<i>x</i>"));
        Assert.DoesNotContain("flare-hidden--below-xs", cut.Markup);
    }

    [Fact]
    public void Above_TheLargestTier_EmitsNothing()
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Above, Breakpoint.Xxl).AddChildContent("<i>x</i>"));
        Assert.DoesNotContain("flare-hidden--above-xxl", cut.Markup);
    }

    [Fact]
    public void Invert_Only_Xxl_EmitsClass()
    {
        var cut = Render<FlareHidden>(p => p
            .Add(x => x.Only, Breakpoint.Xxl).Add(x => x.Invert, true).AddChildContent("<i>x</i>"));
        Assert.Contains(Css.Classes.Hidden.InvertOnlyXxl, cut.Markup);
    }
}
