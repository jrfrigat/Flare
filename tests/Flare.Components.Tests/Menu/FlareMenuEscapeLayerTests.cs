using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The menu owns Escape while it is open: it registers as the newest overlay layer on open and removes the layer
/// on close. The ordering itself lives in the overlay script and is covered by its jsdom tests; this pins the C#
/// half of the contract.
/// </summary>
public sealed class FlareMenuEscapeLayerTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareMenuEscapeLayerTests()
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
    public void OpeningRegistersTheLayerAndClosingRemovesIt()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, ActivatorButton())
            .AddChildContent<FlareMenuItem>(mi => mi.AddChildContent("Save")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", _runtime.Calls));

        cut.Find($".{Css.Classes.Menu.Backdrop}").Click();
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", _runtime.Calls));

        var pushAt = _runtime.Calls.IndexOf("pushPopupLayer");
        var removeAt = _runtime.Calls.IndexOf("removePopupLayer");
        Assert.True(removeAt > pushAt,
            "The menu removed its layer before registering it: " + string.Join(", ", _runtime.Calls));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "pushPopupLayer"));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "removePopupLayer"));
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
