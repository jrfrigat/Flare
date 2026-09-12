using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareFabMenuTests : FlareTestContext
{
    private IRenderedComponent<FlareFloatingActionButton> RenderFabMenu() =>
        Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "Actions")
            .Add(x => x.Position, FabPosition.Static)
            .AddChildContent<FlareFloatingActionMenu>(menu => menu
                .Add(m => m.Placement, Placement.Top)
                .AddChildContent<FlareFloatingActionMenuItem>(item => item
                    .Add(i => i.Icon, FlareIcons.Edit)
                    .Add(i => i.Label, "Edit"))));

    [Fact]
    public void PlainFab_NoMenu_RendersFabWithoutWrapper()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.AriaLabel, "Add"));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Fab.Root}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.FabMenu.Wrapper}"));
    }

    [Fact]
    public void MenuMode_RendersWrapperAndTriggerFab()
    {
        var cut = RenderFabMenu();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FabMenu.Wrapper}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}"));
    }

    [Fact]
    public void MenuMode_RendersMenuList()
    {
        var cut = RenderFabMenu();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FabMenu.List}"));
    }

    [Fact]
    public void RendersActionItem_AsSmallFab()
    {
        var cut = RenderFabMenu();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FabMenu.Item}"));
        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.FabMenu.Btn}.{Css.Classes.Fab.Sm}"));
    }

    [Fact]
    public void ClosedByDefault_NoOpenClass()
    {
        var cut = RenderFabMenu();

        Assert.DoesNotContain(Css.Classes.FabMenu.Open, cut.Find($".{Css.Classes.FabMenu.Wrapper}").ClassName ?? "");
        Assert.DoesNotContain(Css.Classes.FabMenu.ListOpen, cut.Find($".{Css.Classes.FabMenu.List}").ClassName ?? "");
    }

    [Fact]
    public void ClickTrigger_OpensMenu()
    {
        var cut = RenderFabMenu();

        cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}").Click();

        Assert.Contains(Css.Classes.FabMenu.Open, cut.Find($".{Css.Classes.FabMenu.Wrapper}").ClassName ?? "");
        Assert.Contains(Css.Classes.FabMenu.ListOpen, cut.Find($".{Css.Classes.FabMenu.List}").ClassName ?? "");
    }

    [Fact]
    public void Trigger_HasAriaExpanded()
    {
        var cut = RenderFabMenu();

        var trigger = cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}");
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));

        trigger.Click();
        Assert.Equal("true", cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}").GetAttribute("aria-expanded"));
    }

    // Each Placement is a side and an alignment. A bare side centres, which is the side class's own
    // layout, so it must carry NO alignment class - a stray one would pull a centred list to an edge.
    [Theory]
    [InlineData(Placement.Top,         Css.Classes.FabMenu.Up,    null)]
    [InlineData(Placement.TopStart,    Css.Classes.FabMenu.Up,    Css.Classes.FabMenu.AlignStart)]
    [InlineData(Placement.TopEnd,      Css.Classes.FabMenu.Up,    Css.Classes.FabMenu.AlignEnd)]
    [InlineData(Placement.Bottom,      Css.Classes.FabMenu.Down,  null)]
    [InlineData(Placement.BottomStart, Css.Classes.FabMenu.Down,  Css.Classes.FabMenu.AlignStart)]
    [InlineData(Placement.BottomEnd,   Css.Classes.FabMenu.Down,  Css.Classes.FabMenu.AlignEnd)]
    [InlineData(Placement.Left,        Css.Classes.FabMenu.Left,  null)]
    [InlineData(Placement.LeftStart,   Css.Classes.FabMenu.Left,  Css.Classes.FabMenu.AlignStart)]
    [InlineData(Placement.LeftEnd,     Css.Classes.FabMenu.Left,  Css.Classes.FabMenu.AlignEnd)]
    [InlineData(Placement.Right,       Css.Classes.FabMenu.Right, null)]
    [InlineData(Placement.RightStart,  Css.Classes.FabMenu.Right, Css.Classes.FabMenu.AlignStart)]
    [InlineData(Placement.RightEnd,    Css.Classes.FabMenu.Right, Css.Classes.FabMenu.AlignEnd)]
    public void Placement_MapsToOneSideAndAtMostOneAlignment(Placement placement, string side, string? align)
    {
        var cut = Render<FlareFloatingActionMenu>(p => p.Add(m => m.Placement, placement));
        var classes = cut.Find($".{Css.Classes.FabMenu.List}").ClassList;

        string[] sides = [Css.Classes.FabMenu.Up, Css.Classes.FabMenu.Down, Css.Classes.FabMenu.Left, Css.Classes.FabMenu.Right];
        Assert.Equal([side], sides.Where(classes.Contains));

        string[] aligns = [Css.Classes.FabMenu.AlignStart, Css.Classes.FabMenu.AlignEnd];
        Assert.Equal(align is null ? [] : [align], aligns.Where(classes.Contains));
    }
}

// ------------------------------------------------------------------------------
// FlareToggleGroup  (8 tests from Wave7)
// ------------------------------------------------------------------------------
