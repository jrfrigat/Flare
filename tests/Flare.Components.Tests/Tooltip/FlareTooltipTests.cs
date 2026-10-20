using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareTooltipTests : FlareTestContext
{
    [Fact]
    public void EscapeHidesTheBubbleUntilThePointerLeavesAndComesBack()
    {
        var openStates = new List<bool>();
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "help")
            .Add(x => x.Open, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openStates.Add(v))));

        var root = cut.Find($".{Css.Classes.Tooltip.Root}");
        root.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Escape" });

        Assert.Contains(Css.Classes.Tooltip.Dismissed,
            cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
        Assert.Equal(new[] { false }, openStates);

        root.TriggerEvent("onmouseleave", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        Assert.DoesNotContain(Css.Classes.Tooltip.Dismissed,
            cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
    }

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
            .Add(x => x.Placement, Placement.Top));

        Assert.Contains(Css.Classes.Tooltip.Top, cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
    }

    [Fact]
    public void PlacementBottom_HasBottomClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .Add(x => x.Placement, Placement.Bottom));

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
