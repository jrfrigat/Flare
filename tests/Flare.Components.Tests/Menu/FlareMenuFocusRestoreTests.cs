using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// An item's action runs before its menu closes, so the close must not pull focus back from a dialog or
/// a field that action moved it into. An item-triggered close asks the overlay for a guarded restore;
/// Escape and an outside click keep the unconditional one. The browser-side decision itself is covered
/// by the vitest suite for <c>flare-overlay.js</c>.
/// </summary>
public sealed class FlareMenuFocusRestoreTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareMenuFocusRestoreTests()
    {
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private static RenderFragment Item(string label) => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, label)));
        b.CloseComponent();
    };

    private static RenderFragment SubmenuWithItem() => b =>
    {
        b.OpenComponent<FlareSubMenu>(0);
        b.AddAttribute(1, nameof(FlareSubMenu.Label), "More");
        b.AddAttribute(2, nameof(FlareSubMenu.HideDelay), 0);
        b.AddAttribute(3, nameof(FlareSubMenu.ChildContent), Item("Deep"));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareMenu> RenderMenu(RenderFragment? content = null) =>
        Render<FlareMenu>(p => p
            .Add(m => m.Activator, ActivatorButton())
            .Add(m => m.ChildContent, content ?? Item("About")));

    // Only the two focus restores matter here; the module calls that open, place and close the panel are
    // noise for every assertion below.
    private List<string> FocusCalls() => _runtime.Calls
        .Where(c => c is "focusFirstInDialog" or "focusFirstIfUnclaimed").ToList();

    [Fact]
    public async Task AnItemActivation_AsksForTheGuardedRestore()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click());
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find($"button.{Css.Classes.Menu.Item}").Click());

        cut.WaitForAssertion(() => Assert.Equal(["focusFirstIfUnclaimed"], FocusCalls()));
    }

    [Fact]
    public async Task Escape_KeepsTheUnconditionalRestore()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button")
            .Click(new MouseEventArgs { Detail = 0 }));
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(new KeyboardEventArgs { Key = "Escape" }));

        cut.WaitForAssertion(() => Assert.Equal(["focusFirstInDialog"], FocusCalls()));
    }

    [Fact]
    public async Task AnOutsideClick_KeepsTheUnconditionalRestore()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click());
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Backdrop}").Click());

        cut.WaitForAssertion(() => Assert.Equal(["focusFirstInDialog"], FocusCalls()));
    }

    // A submenu item closes its own panel and the menu that holds it. The submenu leaves the tree with
    // the closing panel, so the surviving menu is the one that restores - and it has to use the guarded
    // path, or it would still steal the focus back from the dialog the item opened.
    [Fact]
    public async Task AnItemInsideASubmenu_GuardsTheChainClose()
    {
        var cut = RenderMenu(SubmenuWithItem());
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click());
        await cut.InvokeAsync(() => cut.Find("[role=menu] [aria-haspopup=menu]").Click());
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.SubmenuPanel} [role=menuitem]").Click());

        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}")));
        cut.WaitForAssertion(() => Assert.Equal(["focusFirstIfUnclaimed"], FocusCalls()));
    }

    // Records the module method calls in order; the ordinary bUnit module reference does not keep it.
    private sealed class RecordingRuntime(IJSRuntime runtime) : IJSRuntime
    {
        public List<string> Calls { get; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
            object?[]? args)
        {
            if (typeof(TValue) != typeof(IJSObjectReference))
                return await runtime.InvokeAsync<TValue>(identifier, cancellationToken, args);

            var reference = await runtime.InvokeAsync<IJSObjectReference>("import", cancellationToken, args);
            return (TValue)(object)new RecordingReference(reference, Calls);
        }

        private sealed class RecordingReference(IJSObjectReference inner, List<string> calls) : IJSObjectReference
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            {
                calls.Add(identifier);
                return inner.InvokeAsync<TValue>(identifier, args);
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
                object?[]? args)
            {
                calls.Add(identifier);
                return inner.InvokeAsync<TValue>(identifier, cancellationToken, args);
            }

            public ValueTask DisposeAsync() => inner.DisposeAsync();
        }
    }
}
