using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// A speed-dial menu is a dismissible popup. The menu is passive markup and the button owns the open state, so the
/// button registers the Escape layer on open and removes it on close and disposal; Escape then closes the menu
/// instead of a dialog around it. The ordering between layers lives in the overlay script.
/// </summary>
public sealed class FlareFloatingActionButtonEscapeLayerTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareFloatingActionButtonEscapeLayerTests()
    {
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    private static RenderFragment SpeedDial() => b =>
    {
        b.OpenComponent<FlareFloatingActionMenu>(0);
        b.AddAttribute(1, "ChildContent", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareFloatingActionMenuItem>(2);
            inner.AddAttribute(3, "Label", "Action");
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void OpeningRegistersTheLayerAndClosingRemovesIt()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.ChildContent, SpeedDial()));

        var trigger = cut.Find("button[aria-expanded]");
        trigger.Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        cut.Find("button[aria-expanded]").Click();
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));

        var pushAt = Names().IndexOf("pushPopupLayer");
        var removeAt = Names().IndexOf("removePopupLayer");
        Assert.True(removeAt > pushAt,
            "The speed dial removed its layer before registering it: " + string.Join(", ", Names()));
        Assert.Equal(1, Names().Count(n => n == "pushPopupLayer"));
        Assert.Equal(1, Names().Count(n => n == "removePopupLayer"));
    }

    [Fact]
    public async Task DisposingWhileOpenRemovesTheLayer()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.ChildContent, SpeedDial()));

        cut.Find("button[aria-expanded]").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        await cut.Instance.DisposeAsync();

        Assert.Contains("removePopupLayer", Names());
        Assert.True(Names().IndexOf("removePopupLayer") > Names().IndexOf("pushPopupLayer"),
            "The speed dial went away without removing its layer: " + string.Join(", ", Names()));
    }

    [Fact]
    public async Task EscapeClosesTheMenuAndRemovesTheLayer()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.ChildContent, SpeedDial()));

        cut.Find("button[aria-expanded]").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        var push = _runtime.Calls.Last(c => c.Name == "pushPopupLayer");
        Assert.Equal("CloseFromEscape", push.Args![2]);
        var layer = Assert.IsType<DotNetObjectReference<FlareFloatingActionButton>>(push.Args[1]);
        await layer.Value.CloseFromEscape();

        cut.WaitForAssertion(() => Assert.Equal("false", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded")));
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));
    }

    private List<string> Names() => _runtime.Calls.Select(c => c.Name).ToList();

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
