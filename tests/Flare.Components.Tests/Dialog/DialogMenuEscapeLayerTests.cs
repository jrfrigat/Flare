using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// A menu opened inside a dialog registers as a popup layer above the dialog's own Escape handler, and the callback
/// that layer carries closes the menu alone. The ordering itself lives in the overlay script and is covered there;
/// this pins what the two components hand the engine.
/// </summary>
public sealed class DialogMenuEscapeLayerTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public DialogMenuEscapeLayerTests()
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

    [Fact]
    public async Task MenuInsideDialog_RegistersAboveItAndClosesOnlyItself()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .AddChildContent<FlareMenu>(m => m
                .Add(x => x.Activator, ActivatorButton())
                .AddChildContent<FlareMenuItem>(mi => mi.AddChildContent("Save"))));

        cut.WaitForAssertion(() => Assert.Contains("registerDialogEscHandler", Names()));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        var dialogAt = _runtime.Calls.FindIndex(c => c.Name == "registerDialogEscHandler");
        var pushAt = _runtime.Calls.FindIndex(c => c.Name == "pushPopupLayer");
        Assert.True(pushAt > dialogAt,
            "The menu registered its layer before the dialog did: " + Names());

        // The engine hands Escape to the newest layer, so run what the menu registered: it has to close the menu
        // and leave the dialog in place.
        var layer = Assert.IsType<DotNetObjectReference<FlareMenu>>(_runtime.Calls[pushAt].Args![1]);
        await layer.Value.CloseFromEscape();

        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}")));
        Assert.NotEmpty(cut.FindAll("[role=dialog]"));
    }

    private string Names() => string.Join(", ", _runtime.Calls.Select(c => c.Name));

    // Records the module method calls with their arguments; the ordinary bUnit module reference keeps neither.
    private sealed class RecordingRuntime(IJSRuntime runtime) : IJSRuntime
    {
        public List<(string Name, object?[]? Args)> Calls { get; } = [];

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

        private sealed class RecordingReference(IJSObjectReference inner, List<(string Name, object?[]? Args)> calls)
            : IJSObjectReference
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            {
                calls.Add((identifier, args));
                return inner.InvokeAsync<TValue>(identifier, args);
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
                object?[]? args)
            {
                calls.Add((identifier, args));
                return inner.InvokeAsync<TValue>(identifier, cancellationToken, args);
            }

            public ValueTask DisposeAsync() => inner.DisposeAsync();
        }
    }
}
