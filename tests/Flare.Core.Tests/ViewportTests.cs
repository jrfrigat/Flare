using Flare.Components;

namespace Flare.Core.Tests;

/// <summary>
/// Unit coverage for the responsive/viewport value types and helpers in Flare.Abstractions:
/// breakpoint resolution, range comparisons, viewport/element geometry.
/// </summary>
public class ViewportTests
{
    [Theory]
    [InlineData(0, Breakpoint.Xs)]
    [InlineData(599, Breakpoint.Xs)]
    [InlineData(600, Breakpoint.Sm)]
    [InlineData(959, Breakpoint.Sm)]
    [InlineData(960, Breakpoint.Md)]
    [InlineData(1279, Breakpoint.Md)]
    [InlineData(1280, Breakpoint.Lg)]
    [InlineData(1919, Breakpoint.Lg)]
    [InlineData(1920, Breakpoint.Xl)]
    [InlineData(2559, Breakpoint.Xl)]
    [InlineData(2560, Breakpoint.Xxl)]
    [InlineData(4000, Breakpoint.Xxl)]
    public void FromWidth_ResolvesTierAtEachBoundary(double width, Breakpoint expected) =>
        Assert.Equal(expected, FlareBreakpoints.FromWidth(width));

    [Fact]
    public void Defaults_CoverEveryTier_InAscendingOrder()
    {
        Assert.Equal(0, FlareBreakpoints.MinWidth(Breakpoint.Xs));
        Assert.Equal(600, FlareBreakpoints.MinWidth(Breakpoint.Sm));
        Assert.Equal(960, FlareBreakpoints.MinWidth(Breakpoint.Md));
        Assert.Equal(1280, FlareBreakpoints.MinWidth(Breakpoint.Lg));
        Assert.Equal(1920, FlareBreakpoints.MinWidth(Breakpoint.Xl));
        Assert.Equal(2560, FlareBreakpoints.MinWidth(Breakpoint.Xxl));
        Assert.Equal(FlareBreakpoints.Tiers.Count, FlareBreakpoints.Defaults.Count);
    }

    [Fact]
    public void FromWidth_HonorsCustomDefinitions()
    {
        var custom = new Dictionary<Breakpoint, int> { [Breakpoint.Xs] = 0, [Breakpoint.Md] = 500 };
        Assert.Equal(Breakpoint.Xs, FlareBreakpoints.FromWidth(499, custom));
        Assert.Equal(Breakpoint.Md, FlareBreakpoints.FromWidth(500, custom));
    }

    [Theory]
    [InlineData(Breakpoint.Md, Breakpoint.Sm, true)]
    [InlineData(Breakpoint.Md, Breakpoint.Md, true)]
    [InlineData(Breakpoint.Sm, Breakpoint.Md, false)]
    public void IsAtLeast(Breakpoint current, Breakpoint other, bool expected) =>
        Assert.Equal(expected, current.IsAtLeast(other));

    [Theory]
    [InlineData(Breakpoint.Sm, Breakpoint.Md, true)]
    [InlineData(Breakpoint.Md, Breakpoint.Md, true)]
    [InlineData(Breakpoint.Lg, Breakpoint.Md, false)]
    public void IsAtMost(Breakpoint current, Breakpoint other, bool expected) =>
        Assert.Equal(expected, current.IsAtMost(other));

    [Theory]
    [InlineData(Breakpoint.Md, true)]
    [InlineData(Breakpoint.Sm, true)]
    [InlineData(Breakpoint.Lg, true)]
    [InlineData(Breakpoint.Xs, false)]
    [InlineData(Breakpoint.Xl, false)]
    public void IsBetween_SmToLg(Breakpoint current, bool expected) =>
        Assert.Equal(expected, current.IsBetween(Breakpoint.Sm, Breakpoint.Lg));

    [Fact]
    public void ToMinWidthQuery_FormatsCssMediaQuery()
    {
        Assert.Equal("(min-width: 960px)", Breakpoint.Md.ToMinWidthQuery());
        Assert.Equal("(min-width: 2560px)", Breakpoint.Xxl.ToMinWidthQuery());
    }

    [Fact]
    public void ViewportSize_DerivesOrientationAndBreakpoint()
    {
        var landscape = new ViewportSize(1000, 800);
        Assert.True(landscape.IsLandscape);
        Assert.False(landscape.IsPortrait);
        Assert.Equal(Breakpoint.Md, landscape.Breakpoint);

        var portrait = new ViewportSize(400, 900);
        Assert.True(portrait.IsPortrait);
        Assert.Equal(Breakpoint.Xs, portrait.Breakpoint);

        Assert.Equal(Breakpoint.Xxl, new ViewportSize(2600, 1000).Breakpoint);
    }

    [Fact]
    public void ElementBoundingRect_ComputesEdgesAndVisibility()
    {
        var rect = new ElementBoundingRect(
            Top: 10, Left: 20, Width: 100, Height: 50,
            WindowWidth: 800, WindowHeight: 40, ScrollX: 5, ScrollY: 7);

        Assert.Equal(60, rect.Bottom);
        Assert.Equal(120, rect.Right);
        Assert.Equal(25, rect.AbsoluteLeft);
        Assert.Equal(17, rect.AbsoluteTop);
        Assert.True(rect.IsOutsideBottom);   // bottom 60 > window height 40
        Assert.False(rect.IsOutsideTop);
        Assert.False(rect.IsOutsideRight);   // right 120 <= window width 800
    }
}
