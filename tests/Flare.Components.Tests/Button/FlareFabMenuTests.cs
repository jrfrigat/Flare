using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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
        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.FabMenu.Btn}.{Css.Classes.Fab.SizeSm}"));
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

    private IRenderedComponent<FlareFloatingActionButton> RenderFabMenu(Action onClick) =>
        Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "Actions")
            .Add(x => x.Position, FabPosition.Static)
            .AddChildContent<FlareFloatingActionMenu>(menu => menu
                .AddChildContent<FlareFloatingActionMenuItem>(item => item
                    .Add(i => i.Icon, FlareIcons.Edit)
                    .Add(i => i.Label, "Edit")
                    .Add(i => i.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs _) => onClick())))));

    [Fact]
    public void ActionClick_ClosesTheMenuAndRaisesTheCallbackOnce()
    {
        var clicks = 0;
        var cut = RenderFabMenu(() => clicks++);

        cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}").Click();
        cut.Find($"button.{Css.Classes.FabMenu.Btn}").Click();

        Assert.Equal(1, clicks);
        Assert.DoesNotContain(Css.Classes.FabMenu.Open, cut.Find($".{Css.Classes.FabMenu.Wrapper}").ClassName ?? string.Empty);
        Assert.DoesNotContain(Css.Classes.FabMenu.ListOpen, cut.Find($".{Css.Classes.FabMenu.List}").ClassName ?? string.Empty);
        Assert.Equal("false", cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ActionClick_ReturnsFocusToTheTrigger()
    {
        var cut = RenderFabMenu(() => { });
        var trigger = cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}");
        var triggerId = trigger.GetAttribute("blazor:elementreference");

        trigger.Click();
        cut.Find($"button.{Css.Classes.FabMenu.Btn}").Click();

        cut.WaitForAssertion(() =>
        {
            var focus = JSInterop.Invocations.Last(i => i.Identifier == "Blazor._internal.domWrapper.focus");
            var target = Assert.IsType<ElementReference>(focus.Arguments[0]);
            Assert.Equal(triggerId, target.Id);
        });
    }

    [Fact]
    public async Task EscapeClose_ReturnsFocusToTheTrigger()
    {
        var cut = RenderFabMenu(() => { });
        var trigger = cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}");
        var triggerId = trigger.GetAttribute("blazor:elementreference");
        trigger.Click();

        await cut.InvokeAsync(() => cut.Instance.CloseFromEscape());

        Assert.Equal("false", cut.Find($".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}").GetAttribute("aria-expanded"));
        cut.WaitForAssertion(() =>
        {
            var focus = JSInterop.Invocations.Last(i => i.Identifier == "Blazor._internal.domWrapper.focus");
            var target = Assert.IsType<ElementReference>(focus.Arguments[0]);
            Assert.Equal(triggerId, target.Id);
        });
    }

    private static string TriggerSelector => $".{Css.Classes.FabMenu.Wrapper} > button.{Css.Classes.Fab.Root}";
    private static string ActionSelector => $"button.{Css.Classes.FabMenu.Btn}";

    private static void AddAction(RenderTreeBuilder builder, int seq, string label, bool disabled)
    {
        builder.OpenComponent<FlareFloatingActionMenuItem>(seq);
        builder.AddAttribute(seq + 1, nameof(FlareFloatingActionMenuItem.Icon), FlareIcons.Edit);
        builder.AddAttribute(seq + 2, nameof(FlareFloatingActionMenuItem.Label), label);
        builder.AddAttribute(seq + 3, nameof(FlareFloatingActionMenuItem.Disabled), disabled);
        builder.CloseComponent();
    }

    // Three actions in a fixed order, the middle one disabled: every focus assertion needs to know
    // which button comes next, and a disabled one has to be skipped on the way.
    private IRenderedComponent<FlareFloatingActionButton> RenderThreeActions() =>
        Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "Actions")
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<FlareFloatingActionMenu>(0);
                builder.AddAttribute(1, nameof(FlareFloatingActionMenu.Placement), Placement.Top);
                builder.AddAttribute(2, nameof(FlareFloatingActionMenu.ChildContent), (RenderFragment)(items =>
                {
                    AddAction(items, 0, "Edit", false);
                    AddAction(items, 10, "Archived", true);
                    AddAction(items, 20, "Delete", false);
                }));
                builder.CloseComponent();
            })));

    private int FocusCallCount() =>
        JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    // bUnit never assigns element-reference ids, so it can count a focus request but not say which
    // element received it; the browser suite checks the target through document.activeElement.
    private void AssertFocusRequested(IRenderedComponent<FlareFloatingActionButton> cut, int expectedCalls) =>
        cut.WaitForAssertion(() => Assert.Equal(expectedCalls, FocusCallCount()));

    private void OpenWithMouse(IRenderedComponent<FlareFloatingActionButton> cut)
    {
        cut.Find(TriggerSelector).Click(new MouseEventArgs { Detail = 1 });
        Assert.Equal("true", cut.Find(TriggerSelector).GetAttribute("aria-expanded"));
        Assert.Equal(0, FocusCallCount());
    }

    [Fact]
    public void Trigger_AnnouncesTheMenuItControls()
    {
        var cut = RenderFabMenu();
        var trigger = cut.Find(TriggerSelector);
        var list = cut.Find($".{Css.Classes.FabMenu.List}");

        Assert.Equal("menu", trigger.GetAttribute("aria-haspopup"));
        Assert.Equal(list.GetAttribute("id"), trigger.GetAttribute("aria-controls"));
        Assert.Equal("menu", list.GetAttribute("role"));
        Assert.Equal(trigger.GetAttribute("id"), list.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void AProvidedTriggerId_IsTheOneTheMenuNames()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "Actions")
            .Add(x => x.Position, FabPosition.Static)
            .AddUnmatched("id", "custom-fab")
            .AddChildContent<FlareFloatingActionMenu>(menu => menu
                .AddChildContent<FlareFloatingActionMenuItem>(item => item
                    .Add(i => i.Icon, FlareIcons.Edit)
                    .Add(i => i.Label, "Edit"))));

        Assert.Equal("custom-fab", cut.Find(TriggerSelector).GetAttribute("id"));
        Assert.Equal("custom-fab", cut.Find($".{Css.Classes.FabMenu.List}").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void Actions_AreMenuItems()
    {
        var cut = RenderFabMenu();

        var action = cut.Find(ActionSelector);
        Assert.Equal("menuitem", action.GetAttribute("role"));
        Assert.Equal("-1", action.GetAttribute("tabindex"));
        Assert.Equal("none", cut.Find($".{Css.Classes.FabMenu.Item}").GetAttribute("role"));
    }

    [Fact]
    public void KeyboardClick_OpensAndRequestsFocus()
    {
        var cut = RenderThreeActions();

        cut.Find(TriggerSelector).Click(new MouseEventArgs { Detail = 0 });

        Assert.Equal("true", cut.Find(TriggerSelector).GetAttribute("aria-expanded"));
        AssertFocusRequested(cut, 1);
    }

    [Fact]
    public void MouseClick_OpensWithoutMovingFocus()
    {
        var cut = RenderThreeActions();

        cut.Find(TriggerSelector).Click(new MouseEventArgs { Detail = 1 });

        Assert.Equal("true", cut.Find(TriggerSelector).GetAttribute("aria-expanded"));
        Assert.Equal(0, FocusCallCount());
    }

    [Fact]
    public void ArrowDown_OnTheTriggerOpensAndRequestsFocus()
    {
        var cut = RenderThreeActions();

        cut.Find(TriggerSelector).KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find(TriggerSelector).GetAttribute("aria-expanded")));
        AssertFocusRequested(cut, 1);
    }

    [Fact]
    public void ArrowUp_OnTheTriggerOpensAndRequestsFocus()
    {
        var cut = RenderThreeActions();

        cut.Find(TriggerSelector).KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });

        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find(TriggerSelector).GetAttribute("aria-expanded")));
        AssertFocusRequested(cut, 1);
    }

    [Fact]
    public void ArrowKeys_RequestFocusOncePerPress()
    {
        var cut = RenderThreeActions();
        OpenWithMouse(cut);

        cut.FindAll(ActionSelector)[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        AssertFocusRequested(cut, 1);

        cut.FindAll(ActionSelector)[2].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        AssertFocusRequested(cut, 2);

        cut.FindAll(ActionSelector)[0].KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });
        AssertFocusRequested(cut, 3);
    }

    [Fact]
    public void HomeAndEnd_RequestFocus()
    {
        var cut = RenderThreeActions();
        OpenWithMouse(cut);

        cut.FindAll(ActionSelector)[2].KeyDown(new KeyboardEventArgs { Key = "Home" });
        AssertFocusRequested(cut, 1);

        cut.FindAll(ActionSelector)[0].KeyDown(new KeyboardEventArgs { Key = "End" });
        AssertFocusRequested(cut, 2);
    }

    [Fact]
    public void EscapeFromAnAction_ClosesTheMenuAndRequestsFocusOnTheTrigger()
    {
        var cut = RenderThreeActions();
        OpenWithMouse(cut);

        cut.FindAll(ActionSelector)[0].KeyDown(new KeyboardEventArgs { Key = "Escape" });

        cut.WaitForAssertion(() => Assert.Equal("false", cut.Find(TriggerSelector).GetAttribute("aria-expanded")));
        AssertFocusRequested(cut, 1);
    }

    [Fact]
    public void TabFromAnAction_ClosesTheMenuWithoutPullingFocusBack()
    {
        var cut = RenderThreeActions();
        OpenWithMouse(cut);

        cut.FindAll(ActionSelector)[0].KeyDown(new KeyboardEventArgs { Key = "Tab" });

        cut.WaitForAssertion(() => Assert.Equal("false", cut.Find(TriggerSelector).GetAttribute("aria-expanded")));
        Assert.Equal(0, FocusCallCount());
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
