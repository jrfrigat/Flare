using System.Text.Json;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// A host that owns a shared context menu - the tree row's <c>OnContextMenu</c> - opens one
/// <see cref="FlareMenu"/> at the pointer with <see cref="FlareMenu.OpenAtAsync"/>, and the menu has no
/// activator of its own. These tests pin the point that reaches the placement engine, the toggle, the
/// bar coordination, the untouched activator paths and the two focus restores.
/// </summary>
public sealed class FlareMenuOpenAtTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareMenuOpenAtTests()
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

    private IRenderedComponent<FlareMenu> RenderMenu(RenderFragment? content = null,
        RenderFragment<FlareMenuActivatorContext>? activator = null, EventCallback<bool> onToggle = default) =>
        Render<FlareMenu>(p =>
        {
            p.Add(m => m.ChildContent, content ?? Item("About"));
            if (activator is not null) p.Add(m => m.Activator, activator);
            if (onToggle.HasDelegate) p.Add(m => m.OnToggle, onToggle);
        });

    private List<string> Calls() => _runtime.Calls.Select(c => c.Identifier).ToList();

    private JsonElement PlacementOptions()
    {
        var arguments = _runtime.Calls.Last(c => c.Identifier == "positionAnchoredPanel").Arguments;
        using var document = JsonDocument.Parse((string)arguments![3]!);
        return document.RootElement.Clone();
    }

    [Fact]
    public async Task OpensWithoutAnActivatorAtTheGivenPoint()
    {
        var toggles = new List<bool>();
        var cut = RenderMenu(onToggle: EventCallback.Factory.Create<bool>(this, toggles.Add));

        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(120, 240));

        Assert.Equal([true], toggles);
        Assert.Single(cut.FindAll($".{Css.Classes.Menu.Panel}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.AtCursor}"));
        var rect = PlacementOptions().GetProperty("anchorRect");
        Assert.Equal(120, rect.GetProperty("x").GetDouble());
        Assert.Equal(240, rect.GetProperty("y").GetDouble());
    }

    [Fact]
    public async Task RemembersFocusAndFocusesThePanelOnTheOpenRender()
    {
        var cut = RenderMenu();

        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(10, 20));

        var calls = Calls();
        var remember = calls.IndexOf("rememberFocus");
        Assert.True(remember >= 0, "rememberFocus was not called: " + string.Join(", ", calls));
        // The capture happens in the opening render, after the panel was placed - not in OpenAtAsync,
        // where the panel does not exist yet and there is nothing to hand focus to.
        Assert.True(remember > calls.IndexOf("positionAnchoredPanel"));
        JSInterop.VerifyFocusAsyncInvoke();
    }

    [Fact]
    public async Task EscapeReturnsFocusToTheRememberedElement()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(120, 240));
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(new KeyboardEventArgs { Key = "Escape" }));

        var restore = Assert.Single(_runtime.Calls, c => c.Identifier == "restoreRememberedFocus");
        Assert.False(Assert.IsType<bool>(restore.Arguments![1]));
        Assert.DoesNotContain(_runtime.Calls, c => c.Identifier is "focusFirstInDialog" or "focusFirstIfUnclaimed");
    }

    [Fact]
    public async Task AnItemCloseKeepsTheGuardedRestore()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(120, 240));
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find($"button.{Css.Classes.Menu.Item}").Click());

        var restore = Assert.Single(_runtime.Calls, c => c.Identifier == "restoreRememberedFocus");
        Assert.True(Assert.IsType<bool>(restore.Arguments![1]));
    }

    [Fact]
    public async Task AnActivatorOpenKeepsTheActivatorRestore()
    {
        var cut = RenderMenu(activator: ActivatorButton());
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click());

        Assert.DoesNotContain(_runtime.Calls, c => c.Identifier == "rememberFocus");
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(new KeyboardEventArgs { Key = "Escape" }));

        Assert.Contains(_runtime.Calls, c => c.Identifier == "focusFirstInDialog");
        Assert.DoesNotContain(_runtime.Calls, c => c.Identifier == "restoreRememberedFocus");
    }

    [Fact]
    public async Task AnActivatorCloseAfterAPointOpenKeepsTheActivatorRestore()
    {
        var cut = RenderMenu(activator: ActivatorButton());
        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(10, 20));
        await cut.InvokeAsync(() => cut.Instance.CloseAsync());

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click());
        _runtime.Calls.Clear();
        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(new KeyboardEventArgs { Key = "Escape" }));

        Assert.Contains(_runtime.Calls, c => c.Identifier == "focusFirstInDialog");
        Assert.DoesNotContain(_runtime.Calls, c => c.Identifier == "restoreRememberedFocus");
    }

    [Fact]
    public async Task OpeningTwiceWhileOpenRaisesOneToggleAndKeepsTheFirstPoint()
    {
        var toggles = new List<bool>();
        var cut = RenderMenu(onToggle: EventCallback.Factory.Create<bool>(this, toggles.Add));

        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(10, 20));
        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(30, 40));

        Assert.Equal([true], toggles);
        var rect = PlacementOptions().GetProperty("anchorRect");
        Assert.Equal(10, rect.GetProperty("x").GetDouble());
        Assert.Equal(20, rect.GetProperty("y").GetDouble());
    }

    [Theory]
    [InlineData(double.NaN, 10)]
    [InlineData(10, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, 10)]
    public async Task RejectsNonFiniteCoordinates(double clientX, double clientY)
    {
        var cut = RenderMenu();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => cut.InvokeAsync(() => cut.Instance.OpenAtAsync(clientX, clientY)));

        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public async Task APointOpenKeepsTheBackdropDismissal()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(10, 20));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
        _runtime.Calls.Clear();

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Backdrop}").Click());

        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
        var restore = Assert.Single(_runtime.Calls, c => c.Identifier == "restoreRememberedFocus");
        Assert.False(Assert.IsType<bool>(restore.Arguments![1]));
    }

    [Fact]
    public async Task ACoordinatedOpenJoinsTheBarAndDropsItsOwnBackdrop()
    {
        var bar = new FlareMenuBarContext();
        var cut = Render<FlareMenu>(p => p
            .AddCascadingValue(bar)
            .Add(m => m.ChildContent, Item("About")));

        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(10, 20));

        Assert.True(bar.HasOpenMenu);
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));

        await cut.InvokeAsync(() => cut.Instance.CloseAsync());

        Assert.False(bar.HasOpenMenu);
    }

    [Fact]
    public async Task ATakenOverPointOpenForgetsTheRememberedElement()
    {
        var bar = new FlareMenuBarContext();
        var first = Render<FlareMenu>(p => p
            .AddCascadingValue(bar)
            .Add(m => m.ChildContent, Item("About")));
        var second = Render<FlareMenu>(p => p
            .AddCascadingValue(bar)
            .Add(m => m.ChildContent, Item("Other")));
        await first.InvokeAsync(() => first.Instance.OpenAtAsync(10, 20));
        _runtime.Calls.Clear();

        await second.InvokeAsync(() => second.Instance.OpenAsync());

        Assert.Contains(_runtime.Calls, c => c.Identifier == "forgetRememberedFocus");
        Assert.DoesNotContain(_runtime.Calls, c => c.Identifier == "restoreRememberedFocus");
    }

    [Fact]
    public async Task DisposalForgetsTheRememberedElement()
    {
        var cut = RenderMenu();
        await cut.InvokeAsync(() => cut.Instance.OpenAtAsync(10, 20));
        _runtime.Calls.Clear();

        await cut.Instance.DisposeAsync();

        Assert.Contains(_runtime.Calls, c => c.Identifier == "forgetRememberedFocus");
    }

    // Records the overlay module calls in order and keeps their arguments, so a test can read both the
    // placement point and the focus-restore flag from one sequence.
    private sealed class RecordingRuntime(IJSRuntime runtime) : IJSRuntime
    {
        public List<(string Identifier, object?[]? Arguments)> Calls { get; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
            object?[]? args)
        {
            Calls.Add((identifier, args));
            if (typeof(TValue) != typeof(IJSObjectReference))
                return await runtime.InvokeAsync<TValue>(identifier, cancellationToken, args);

            var reference = await runtime.InvokeAsync<IJSObjectReference>("import", cancellationToken, args);
            return (TValue)(object)new RecordingReference(reference, Calls);
        }

        private sealed class RecordingReference(IJSObjectReference inner, List<(string, object?[]?)> calls) : IJSObjectReference
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
