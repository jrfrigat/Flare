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
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Linear));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Root}"));
    }

    [Fact]
    public void LinearVariant_HasLinearClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Linear)
            .Add(x => x.Value, 50.0));

        Assert.Contains(Css.Classes.Progress.Linear, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void CircularVariant_HasCircularClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 50.0));

        Assert.Contains(Css.Classes.Progress.Circular, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void IndeterminateMode_WhenValueIsNull()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, (double?)null));

        Assert.Contains(Css.Classes.Progress.Indeterminate, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void AriaValueNow_ReflectsValue()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, 75.0));

        Assert.Equal("75", cut.Find("[role='progressbar']").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void CircularVariant_RendersSvg()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 50.0));

        Assert.NotEmpty(cut.FindAll($"svg.{Css.Classes.Progress.Svg}"));
    }
}

// ------------------------------------------------------------------------------
// FlareSkeleton  (6 tests from Wave3)
// ------------------------------------------------------------------------------
