using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The popover owns Escape through the shared overlay layer: opening pushes the layer, closing removes
/// it, and a panel disposed while open takes its layer away. The ordering between layers lives in the
/// overlay script and is covered by its jsdom tests.
/// </summary>
public sealed class FlarePopoverEscapeLayerTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlarePopoverEscapeLayerTests()
    {
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    [Theory]
    [InlineData(PopoverTrigger.Click)]
    [InlineData(PopoverTrigger.Manual)]
    [InlineData(PopoverTrigger.Hover)]
    public void OpeningRegistersTheLayerAndClosingRemovesIt(PopoverTrigger trigger)
    {
        var cut = RenderPopover(trigger, open: true);

        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        cut.Render(p => p
            .Add(x => x.Trigger, trigger)
            .Add(x => x.Open, false)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .AddChildContent("<p>plain text</p>"));

        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));

        var pushAt = Names().IndexOf("pushPopupLayer");
        var removeAt = Names().IndexOf("removePopupLayer");
        Assert.True(removeAt > pushAt,
            "The popover removed its layer before registering it: " + string.Join(", ", Names()));
        Assert.Equal(1, Names().Count(n => n == "pushPopupLayer"));
        Assert.Equal(1, Names().Count(n => n == "removePopupLayer"));
    }

    [Fact]
    public async Task DisposingWhileOpenRemovesTheLayer()
    {
        var cut = RenderPopover(PopoverTrigger.Click, open: true);

        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        await cut.Instance.DisposeAsync();

        Assert.Contains("removePopupLayer", Names());
        Assert.True(Names().IndexOf("removePopupLayer") > Names().IndexOf("pushPopupLayer"),
            "The popover went away without removing its layer: " + string.Join(", ", Names()));
    }

    [Fact]
    public async Task CloseFromEscape_RequestsCloseAndRemovesTheLayer()
    {
        var states = new List<bool>();
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Click)
            .Add(x => x.Open, true)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .AddChildContent("<input id=\"pop-input\" />")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));

        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        var push = _runtime.Calls.Last(c => c.Name == "pushPopupLayer");
        Assert.Equal("CloseFromEscape", push.Args![2]);
        var layer = Assert.IsType<DotNetObjectReference<FlarePopover>>(push.Args[1]);
        await cut.InvokeAsync(() => layer.Value.CloseFromEscape());

        Assert.Contains(false, states);

        cut.Render(p => p
            .Add(x => x.Trigger, PopoverTrigger.Click)
            .Add(x => x.Open, false)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .AddChildContent("<input id=\"pop-input\" />"));
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));
    }

    private IRenderedComponent<FlarePopover> RenderPopover(PopoverTrigger trigger, bool open)
        => Render<FlarePopover>(p => p
            .Add(x => x.Trigger, trigger)
            .Add(x => x.Open, open)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .AddChildContent("<p>plain text</p>"));

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
