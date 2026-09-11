using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareProgressTests : FlareTestContext
{
    [Fact]
    public void RendersLinearRootElement()
    {
        var cut = Render<FlareProgressLinear>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Root}"));
    }

    [Fact]
    public void LinearVariant_HasLinearClass()
    {
        var cut = Render<FlareProgressLinear>(p => p
            .Add(x => x.Value, 50.0));

        Assert.Contains(Css.Classes.Progress.Linear, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void CircularVariant_HasCircularClass()
    {
        var cut = Render<FlareProgressCircular>(p => p
            .Add(x => x.Value, 50.0));

        Assert.Contains(Css.Classes.Progress.Circular, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void IndeterminateMode_WhenValueIsNull()
    {
        var cut = Render<FlareProgressLinear>(p => p
            .Add(x => x.Value, (double?)null));

        Assert.Contains(Css.Classes.Progress.Indeterminate, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void IndeterminateLinear_RendersThemeExtensionSegments()
    {
        var cut = Render<FlareProgressLinear>(p => p
            .Add(x => x.Class, "theme-progress-variant"));

        Assert.Single(cut.FindAll($".{Css.Classes.Progress.Remain}"));
        var first = Assert.Single(cut.FindAll($".{Css.Classes.Progress.IndeterminateFirst}"));
        var second = Assert.Single(cut.FindAll($".{Css.Classes.Progress.IndeterminateSecond}"));
        Assert.DoesNotContain(Css.Classes.Progress.Root, first.ClassList);
        Assert.DoesNotContain(Css.Classes.Progress.Root, second.ClassList);
        Assert.DoesNotContain("theme-progress-variant", first.ClassList);
        Assert.DoesNotContain("theme-progress-variant", second.ClassList);
        Assert.Empty(cut.FindAll("svg"));
    }

    [Fact]
    public void AriaValueNow_ReflectsValue()
    {
        var cut = Render<FlareProgressLinear>(p => p
            .Add(x => x.Value, 75.0));

        Assert.Equal("75", cut.Find("[role='progressbar']").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void CircularVariant_RendersSvg()
    {
        var cut = Render<FlareProgressCircular>(p => p
            .Add(x => x.Value, 50.0));

        Assert.NotEmpty(cut.FindAll($"svg.{Css.Classes.Progress.Svg}"));
    }
}

// ------------------------------------------------------------------------------
// FlareSkeleton  (6 tests from Wave3)
// ------------------------------------------------------------------------------
