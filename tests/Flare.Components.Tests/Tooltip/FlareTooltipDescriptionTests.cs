using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// aria-describedby is not inherited, so the tooltip id has to be linked to the focusable element the
/// user reaches: the component asks the overlay script on first render and when the content/disabled
/// state changes, and takes the id away when the tooltip can no longer describe anything.
/// </summary>
public sealed class FlareTooltipDescriptionTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareTooltipDescriptionTests()
    {
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    [Fact]
    public void Content_LinksTheDescriptionToTheTrigger()
    {
        var cut = RenderTooltip("help", disabled: false);

        cut.WaitForAssertion(() => Assert.Contains("syncTooltipDescription", Names()));

        var call = _runtime.Calls.Last(c => c.Name == "syncTooltipDescription");
        var tooltipId = cut.Find($".{Css.Classes.Tooltip.Content}").Id;
        Assert.Equal(tooltipId, call.Args![1]);
        Assert.Equal(true, call.Args[2]);
        Assert.Equal(tooltipId, cut.Find($".{Css.Classes.Tooltip.Root}").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Disabled_RemovesTheDescription()
    {
        var cut = RenderTooltip("help", disabled: false);
        cut.WaitForAssertion(() => Assert.Contains("syncTooltipDescription", Names()));

        cut.Render(p => p
            .Add(x => x.Content, "help")
            .Add(x => x.Disabled, true)
            .AddChildContent("<button>go</button>"));

        cut.WaitForAssertion(() =>
        {
            var call = _runtime.Calls.Last(c => c.Name == "syncTooltipDescription");
            Assert.Equal(false, call.Args![2]);
        });
        Assert.Null(cut.Find($".{Css.Classes.Tooltip.Root}").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void EmptyContent_RemovesTheDescription()
    {
        var cut = RenderTooltip("help", disabled: false);
        cut.WaitForAssertion(() => Assert.Contains("syncTooltipDescription", Names()));

        cut.Render(p => p
            .Add(x => x.Content, (string?)null)
            .AddChildContent("<button>go</button>"));

        cut.WaitForAssertion(() =>
        {
            var call = _runtime.Calls.Last(c => c.Name == "syncTooltipDescription");
            Assert.Equal(false, call.Args![2]);
        });
        Assert.Null(cut.Find($".{Css.Classes.Tooltip.Root}").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void DisabledFromTheStart_RendersNoRootDescription()
    {
        var cut = RenderTooltip("help", disabled: true);

        Assert.Null(cut.Find($".{Css.Classes.Tooltip.Root}").GetAttribute("aria-describedby"));
        Assert.DoesNotContain("syncTooltipDescription", Names());
    }

    private IRenderedComponent<FlareTooltip> RenderTooltip(string? content, bool disabled)
        => Render<FlareTooltip>(p => p
            .Add(x => x.Content, content)
            .Add(x => x.Disabled, disabled)
            .AddChildContent("<button>go</button>"));

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
