using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The select family shares one popup, and that popup registers itself as an overlay layer: opening pushes the
/// layer, closing removes it, and a popup disposed while still open takes its layer away with it. The ordering of
/// Escape between layers lives in the overlay script and is covered by its jsdom tests.
/// </summary>
public sealed class FlarePopupEscapeLayerTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlarePopupEscapeLayerTests()
    {
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    [Fact]
    public void OpeningRegistersTheLayerAndClosingRemovesIt()
    {
        var cut = Render<FlarePopup>(p => p
            .Add(x => x.Open, true)
            .AddChildContent("<div>option</div>"));

        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", _runtime.Calls));

        cut.Render(p => p
            .Add(x => x.Open, false)
            .AddChildContent("<div>option</div>"));

        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", _runtime.Calls));

        var pushAt = _runtime.Calls.IndexOf("pushPopupLayer");
        var removeAt = _runtime.Calls.IndexOf("removePopupLayer");
        Assert.True(removeAt > pushAt,
            "The popup removed its layer before registering it: " + string.Join(", ", _runtime.Calls));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "pushPopupLayer"));
        Assert.Equal(1, _runtime.Calls.Count(c => c == "removePopupLayer"));
    }

    [Fact]
    public async Task DisposingWhileOpenRemovesTheLayer()
    {
        var cut = Render<FlarePopup>(p => p
            .Add(x => x.Open, true)
            .AddChildContent("<div>option</div>"));

        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", _runtime.Calls));

        await cut.Instance.DisposeAsync();

        Assert.Contains("removePopupLayer", _runtime.Calls);
        Assert.True(_runtime.Calls.IndexOf("removePopupLayer") > _runtime.Calls.IndexOf("pushPopupLayer"),
            "The popup went away without removing its layer: " + string.Join(", ", _runtime.Calls));
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
