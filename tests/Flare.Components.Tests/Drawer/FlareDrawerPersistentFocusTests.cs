using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// A persistent drawer collapses to zero width instead of leaving the screen, so its contents are inert
/// while closed; focus that was inside at close time is handed back to the page rather than dropped on
/// the body.
/// </summary>
public sealed class FlareDrawerPersistentFocusTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;

    public FlareDrawerPersistentFocusTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    private static RenderFragment Inside() => b =>
    {
        b.OpenElement(0, "button");
        b.AddAttribute(1, "id", "inside");
        b.AddContent(2, "Inside");
        b.CloseElement();
    };

    private static void FocusInside(IRenderedComponent<FlareDrawer> cut)
        => cut.Find("#inside").TriggerEvent("onfocusin", new FocusEventArgs());

    private static void Close(IRenderedComponent<FlareDrawer> cut, DrawerVariant variant)
        => cut.Render(p => p
            .Add(x => x.Variant, variant)
            .Add(x => x.Open, false)
            .Add(x => x.ChildContent, Inside()));

    [Fact]
    public void ClosedPersistentDrawer_IsInert()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, false)
            .Add(x => x.ChildContent, Inside()));

        Assert.True(cut.Find($".{Css.Classes.Drawer.Root}").HasAttribute("inert"));
    }

    [Fact]
    public void OpenPersistentDrawer_IsNotInert()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));

        Assert.False(cut.Find($".{Css.Classes.Drawer.Root}").HasAttribute("inert"));
    }

    [Fact]
    public void ClosingAPersistentDrawerWithFocusInside_HandsFocusBackToThePage()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));
        FocusInside(cut);

        Close(cut, DrawerVariant.Persistent);

        cut.WaitForAssertion(() => Assert.Contains("restoreFocusOutside", _runtime.Calls));
    }

    [Fact]
    public void ClosingAPersistentDrawerWithoutFocusInside_LeavesFocusAlone()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));

        Close(cut, DrawerVariant.Persistent);

        Assert.True(cut.Find($".{Css.Classes.Drawer.Root}").HasAttribute("inert"));
        Assert.DoesNotContain("restoreFocusOutside", _runtime.Calls);
    }

    [Fact]
    public void ClosingAModalDrawer_LeavesFocusToTheFocusTrap()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));
        FocusInside(cut);

        Close(cut, DrawerVariant.Temporary);

        cut.WaitForAssertion(() => Assert.Contains("releaseFocusTrap", _runtime.Calls));
        Assert.DoesNotContain("restoreFocusOutside", _runtime.Calls);
    }
}
