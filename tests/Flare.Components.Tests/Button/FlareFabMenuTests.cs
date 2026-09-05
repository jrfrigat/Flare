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
                .Add(m => m.Direction, FabMenuDirection.Up)
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
}

// ------------------------------------------------------------------------------
// FlareToggleGroup  (8 tests from Wave7)
// ------------------------------------------------------------------------------
