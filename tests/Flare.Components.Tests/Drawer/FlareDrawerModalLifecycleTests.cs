using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// The drawer follows ONE modal state - open, rendered as an overlay, not hidden by AutoCloseBelow - and
/// reconciles the Escape handler and the focus trap with it on every transition: open, close, variant
/// switch, breakpoint crossing and AutoCloseBelow hiding. A modal that appears under an already open
/// dialog must not steal the keyboard, so it registers as a background trap.
/// </summary>
public sealed class FlareDrawerModalLifecycleTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;

    public FlareDrawerModalLifecycleTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
        Services.AddScoped<IBrowserViewportService>(_ => new BrowserViewportService(_runtime));
    }

    // The breakpoint subscription is attached in OnAfterRenderAsync; resizing before it exists would be
    // lost. The subscribe round trip itself is the signal that the drawer is now listening.
    private void WaitForSubscription(IRenderedComponent<FlareDrawer> cut)
        => cut.WaitForAssertion(() => Assert.Contains("ensureViewportListener", _runtime.Calls));

    private async Task ResizeAsync(double width, double height)
    {
        var viewport = (BrowserViewportService)Services.GetRequiredService<IBrowserViewportService>();
        await viewport.OnViewportResized(width, height);
    }

    private static RenderFragment Inside() => b =>
    {
        b.OpenElement(0, "button");
        b.AddAttribute(1, "id", "inside");
        b.AddContent(2, "Inside");
        b.CloseElement();
    };

    [Fact]
    public async Task ResponsiveBelowTheBreakpoint_BecomesAModalDialogWithEscapeAndATrap()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Responsive)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));

        // The immediate breakpoint notification reports Md without a browser width, which is the wide side.
        Assert.Equal("navigation", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("role"));
        Assert.DoesNotContain("trapFocus", _runtime.Calls);
        WaitForSubscription(cut);

        await ResizeAsync(320, 800);

        cut.WaitForAssertion(() =>
        {
            var drawer = cut.Find($".{Css.Classes.Drawer.Root}");
            Assert.Equal("dialog", drawer.GetAttribute("role"));
            Assert.Equal("true", drawer.GetAttribute("aria-modal"));
            Assert.Contains("registerDialogEscHandler", _runtime.Calls);
            Assert.Contains("trapFocus", _runtime.Calls);
        });
    }

    [Fact]
    public async Task ResponsiveAboveTheBreakpoint_ReleasesTheTrapAndReturnsToNavigation()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Responsive)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));
        WaitForSubscription(cut);

        await ResizeAsync(320, 800);
        cut.WaitForAssertion(() => Assert.Contains("trapFocus", _runtime.Calls));

        await ResizeAsync(1280, 900);

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("navigation", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("role"));
            Assert.Contains("releaseFocusTrap", _runtime.Calls);
            Assert.Contains("removeDialogEscHandler", _runtime.Calls);
        });
    }

    [Fact]
    public async Task SwitchingFromTemporaryToPersistentWhileOpen_ReleasesTheTrapAndEscape()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));
        cut.WaitForAssertion(() => Assert.Contains("trapFocus", _runtime.Calls));

        cut.Render(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("navigation", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("role"));
            Assert.Contains("releaseFocusTrap", _runtime.Calls);
            Assert.Contains("removeDialogEscHandler", _runtime.Calls);
        });
    }

    [Fact]
    public async Task AutoCloseBelow_ReleasesTheTrapWhenTheDrawerIsHidden()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.AutoCloseBelow, Breakpoint.Md)
            .Add(x => x.ChildContent, Inside()));
        cut.WaitForAssertion(() => Assert.Contains("trapFocus", _runtime.Calls));
        WaitForSubscription(cut);

        await ResizeAsync(320, 800);

        cut.WaitForAssertion(() => Assert.Contains("releaseFocusTrap", _runtime.Calls));
    }

    [Fact]
    public async Task TurningCloseOnEscOffWhileOpen_RemovesOnlyTheEscapeHandler()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.CloseOnEsc, true)
            .Add(x => x.ChildContent, Inside()));
        cut.WaitForAssertion(() => Assert.Contains("registerDialogEscHandler", _runtime.Calls));

        cut.Render(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.CloseOnEsc, false)
            .Add(x => x.ChildContent, Inside()));

        cut.WaitForAssertion(() => Assert.Contains("removeDialogEscHandler", _runtime.Calls));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "trapFocus"));

        cut.Render(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.CloseOnEsc, true)
            .Add(x => x.ChildContent, Inside()));

        cut.WaitForAssertion(() =>
            Assert.Equal(2, _runtime.Calls.Count(c => c == "registerDialogEscHandler")));
    }

    [Fact]
    public void OpeningAModalDrawer_TrapsInTheForeground()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));

        cut.WaitForAssertion(() => Assert.Contains(_runtime.Invocations, i => i.Id == "trapFocus"));
        Assert.False(BackgroundFlag());
    }

    [Fact]
    public async Task ABreakpointTurnsAnOpenDrawerIntoABackgroundModal()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Responsive)
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, Inside()));
        WaitForSubscription(cut);

        await ResizeAsync(320, 800);

        cut.WaitForAssertion(() => Assert.Contains(_runtime.Invocations, i => i.Id == "trapFocus"));
        Assert.True(BackgroundFlag());
    }

    // The third trapFocus argument: true means "stack under an open modal instead of taking the keyboard".
    private bool? BackgroundFlag()
        => _runtime.Invocations.FirstOrDefault(i => i.Id == "trapFocus").Args is { Length: >= 3 } args
            ? args[2] as bool?
            : null;

}
