using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareProgress sizing: one TrackSize step drives the linear thickness AND the circular diameter, and every
// value behind it belongs to the theme. The component's only job is to say WHICH step is in play.
public class FlareProgressSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(TrackSize.Xs, Css.Classes.Progress.Xs)]
    [InlineData(TrackSize.Sm, Css.Classes.Progress.Sm)]
    [InlineData(TrackSize.Md, Css.Classes.Progress.Md)]
    [InlineData(TrackSize.Lg, Css.Classes.Progress.Lg)]
    [InlineData(TrackSize.Xl, Css.Classes.Progress.Xl)]
    public void Size_AppliesSizeClass_OnLinear(TrackSize size, string expected)
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 50d).Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    // The same step has to reach the circular variant too, or one Size would mean two different things.
    [Theory]
    [InlineData(TrackSize.Xs, Css.Classes.Progress.Xs)]
    [InlineData(TrackSize.Xl, Css.Classes.Progress.Xl)]
    public void Size_AppliesSizeClass_OnCircular(TrackSize size, string expected)
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 50d)
            .Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void DefaultSize_IsMd()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 50d));

        Assert.Contains(Css.Classes.Progress.Md, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    // Regression: the circular variant used to write width/height px inline from an int Size that defaulted
    // to 40. Inline style beats the stylesheet, so --flare-progress-circular-size never rendered and a theme
    // that set its own diameter was silently ignored - the core's 40 always won. Nothing about the geometry
    // may come back inline; the size class picks a theme token and the theme decides the number.
    [Fact]
    public void Circular_WritesNoInlineGeometry_SoTheThemeTokenDecides()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 50d)
            .Add(x => x.Size, TrackSize.Xl));

        var style = cut.Find($".{Css.Classes.Progress.Root}").GetAttribute("style") ?? "";
        Assert.DoesNotContain("width:", style);
        Assert.DoesNotContain("height:", style);
    }

    // The linear track thickness is a theme token as well - no inline override survives from the old
    // pixel-valued Thickness parameter.
    [Fact]
    public void Linear_WritesNoInlineHeight()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Value, 50d).Add(x => x.Size, TrackSize.Xl));

        var style = cut.Find($".{Css.Classes.Progress.Root}").GetAttribute("style") ?? "";
        Assert.DoesNotContain("--flare-progress-linear-height", style);
        Assert.DoesNotContain("height:", style);
    }
}
