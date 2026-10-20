using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// A dialog can close while its open is still waiting for JS. The close must run after the open has finished
/// registering the Escape handler and the focus trap - otherwise it releases nothing, and the open then registers
/// handlers that stay behind after the dialog is gone.
/// </summary>
public sealed class FlareDialogModalRaceTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareDialogModalRaceTests()
    {
        // Pending JS calls model Server interop; bUnit's in-process module completes synchronously.
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
        Services.AddScoped<IScrollService>(_ => new ScrollService(_runtime));
        Services.AddScoped<IDialogDragJsService>(_ => new DialogDragJsService(_runtime));
    }

    // Exposes the protected lifecycle so the test can call it while the renderer's own call is still awaiting JS -
    // the overlap Blazor permits, and the one bUnit's renderer does not produce between two renders by itself.
    private sealed class ExposedDialog : FlareDialog
    {
        public Task RunAfterRenderAsync() => base.OnAfterRenderAsync(false);
    }

    [Fact]
    public async Task ClosingWhileTheOpenWaitsForJs_RemovesTheEscapeRegistration()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var register = module.SetupVoid("registerDialogEscHandler", _ => true);

        var cut = Render<ExposedDialog>(p => p.Add(x => x.Visible, true));
        Assert.Single(cut.FindAll("[role=dialog]"));
        cut.WaitForAssertion(() => Assert.Contains("registerDialogEscHandler", _runtime.Calls));

        // The open is waiting for JS; the parent closes the dialog and a render asks the component to catch up.
        // Called directly, not through InvokeAsync: bUnit waits for pending after-render work before it runs that,
        // which would hide exactly the overlap this test is about.
#pragma warning disable BL0005 // The lifecycle is driven concurrently on purpose; no render may intervene.
        cut.Instance.Visible = false;
#pragma warning restore BL0005
        var close = cut.Instance.RunAfterRenderAsync();
        register.SetVoidResult();
        await close;

        cut.WaitForAssertion(() => Assert.Contains("removeDialogEscHandler", _runtime.Calls));

        var registerAt = _runtime.Calls.IndexOf("registerDialogEscHandler");
        var removeAt = _runtime.Calls.IndexOf("removeDialogEscHandler");
        Assert.True(registerAt >= 0,
            "The dialog never registered its Escape handler: " + string.Join(", ", _runtime.Calls));
        Assert.True(removeAt > registerAt,
            "The close ran before the open finished, leaving the Escape handler registered: "
            + string.Join(", ", _runtime.Calls));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "registerDialogEscHandler"));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "removeDialogEscHandler"));

        // The scroll lock is taken after the Escape and focus registrations. A close that runs beside the open
        // releases nothing and then the open leaves the page locked.
        var lockAt = _runtime.Calls.IndexOf("lock");
        var unlockAt = _runtime.Calls.IndexOf("unlock");
        Assert.True(lockAt >= 0, "The open never took the scroll lock: " + string.Join(", ", _runtime.Calls));
        Assert.True(unlockAt > lockAt,
            "The dialog stayed scroll-locked after the close: " + string.Join(", ", _runtime.Calls));
    }

    [Fact]
    public void TogglingCloseOnEscDragAndResizeWhileOpen_ReconcilesWithoutReopening()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.CloseOnEsc, false)
            .Add(x => x.Draggable, false)
            .Add(x => x.Resizable, false)
            .AddChildContent("<p>body</p>"));
        cut.WaitForAssertion(() => Assert.Contains("trapFocus", _runtime.Calls));
        Assert.DoesNotContain("registerDialogEscHandler", _runtime.Calls);
        Assert.DoesNotContain("registerDialogDrag", _runtime.Calls);
        Assert.DoesNotContain("registerDialogResize", _runtime.Calls);

        cut.Render(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.CloseOnEsc, true)
            .Add(x => x.Draggable, true)
            .Add(x => x.Resizable, true)
            .AddChildContent("<p>body</p>"));
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("registerDialogEscHandler", _runtime.Calls);
            Assert.Contains("registerDialogDrag", _runtime.Calls);
            Assert.Contains("registerDialogResize", _runtime.Calls);
        });

        cut.Render(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.CloseOnEsc, false)
            .Add(x => x.Draggable, false)
            .Add(x => x.Resizable, false)
            .AddChildContent("<p>body</p>"));
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("removeDialogEscHandler", _runtime.Calls);
            Assert.Contains("removeDialogDrag", _runtime.Calls);
            Assert.Contains("removeDialogResize", _runtime.Calls);
        });

        // None of those toggles reopened the dialog: one trap and one scroll lock for one Visible=true.
        Assert.Equal(1, _runtime.Calls.Count(c => c == "trapFocus"));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "lock"));
    }

    // Records the module method calls in order; the ordinary bUnit module reference does not keep their sequence.
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
