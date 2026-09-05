using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareTooltipTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "Tooltip text"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Tooltip.Root}"));
    }

    [Fact]
    public void RendersTooltipContent()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "Helpful tip"));

        Assert.Contains("Helpful tip", cut.Find($".{Css.Classes.Tooltip.Content}").TextContent);
    }

    [Fact]
    public void PlacementTop_HasTopClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .Add(x => x.Placement, TooltipPlacement.Top));

        Assert.Contains(Css.Classes.Tooltip.Top, cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
    }

    [Fact]
    public void PlacementBottom_HasBottomClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .Add(x => x.Placement, TooltipPlacement.Bottom));

        Assert.Contains(Css.Classes.Tooltip.Bottom, cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .AddChildContent("<button class=\"trigger-btn\">Hover me</button>"));

        Assert.NotEmpty(cut.FindAll(".trigger-btn"));
    }
}

// ------------------------------------------------------------------------------
// FlareEmptyState  (6 tests from Wave1)
// ------------------------------------------------------------------------------
