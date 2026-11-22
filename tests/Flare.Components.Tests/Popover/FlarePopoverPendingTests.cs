using Flare.Components.Services;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlarePopoverPendingTests : FlareTestContext
{
    public FlarePopoverPendingTests() => Services.AddScoped<IOverlayJsService>(_ =>
        new OverlayJsService(new RemoteOverlayRuntime(JSInterop.JSRuntime)));

    [Fact]
    public void PanelStaysHiddenUntilDismissalIsRegistered()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid("registerDismiss", _ => true);

        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.AnchorContent, Markup("<span>a</span>"))
            .Add(x => x.ChildContent, Markup("<span>c</span>")));

        // A visible panel must be a dismissable one: the paper stays pending until the JS dismissal
        // registration completes, so an outside click cannot land in that window.
        var hiddenWhilePending = cut.Find($".{Css.Classes.Popover.Paper}")
            .HasAttribute("data-flare-popover-pending");
        pending.SetVoidResult();

        Assert.True(hiddenWhilePending);
        cut.WaitForAssertion(() => Assert.False(
            cut.Find($".{Css.Classes.Popover.Paper}").HasAttribute("data-flare-popover-pending")));
    }

    private static RenderFragment Markup(string html) => b => b.AddMarkupContent(0, html);
}
