using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareFloatingActionButtonTests : FlareTestContext
{
    [Fact]
    public void RendersDefault()
    {
        var cut = Render<FlareFloatingActionButton>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Fab.Root}"));
    }

    [Fact]
    public void RendersSmall()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Size, FabSize.Sm));

        Assert.Contains(Css.Classes.Fab.Sm, cut.Find($".{Css.Classes.Fab.Root}").ClassName);
    }

    [Fact]
    public void RendersLarge()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Size, FabSize.Lg));

        Assert.Contains(Css.Classes.Fab.Lg, cut.Find($".{Css.Classes.Fab.Root}").ClassName);
    }

    [Fact]
    public void RendersSecondaryColorClass()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Color, FlareColor.Secondary));

        Assert.Contains(Css.Classes.Color.Secondary, cut.Find($".{Css.Classes.Fab.Root}").ClassName);
    }

    [Fact]
    public void RendersCustomFabColorInline()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Color, FlareColor.Custom("#123456")));

        Assert.Contains(Css.Tokens.LocalColor.Container, cut.Find($".{Css.Classes.Fab.Root}").GetAttribute("style"));
    }

    [Fact]
    public void DefaultFabColorHasNoColorClass()
    {
        var cut = Render<FlareFloatingActionButton>();

        Assert.DoesNotContain("flare-color-", cut.Find($".{Css.Classes.Fab.Root}").ClassName);
    }

    [Fact]
    public void RendersAnchorBottomRight()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.BottomRight));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Fab.AnchorBottomRight}"));
    }

    [Fact]
    public void RendersLabelSlot()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Label, "Create")
            .Add(x => x.Position, FabPosition.Static));

        var label = cut.Find($".{Css.Classes.Fab.Label}");
        Assert.Equal("Create", label.TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareToggleButton  (5 tests from Wave3)
// ------------------------------------------------------------------------------
