using Flare.Components;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// IScrollService (ScrollService). Under bUnit's loose JS interop the module reports no position, so
// what these exercise is the part that matters: the C#-side derivation of direction, delta and the
// reversal flag, the filtering each subscription asked for, and the disposable-token contract. The
// browser half is one addEventListener; the half that can be wrong is this one.
// ------------------------------------------------------------------------------

public class C_ScrollServiceTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-scroll.js";

    private IScrollService Scroll => Services.GetRequiredService<IScrollService>();

    private static ScrollService.ScrollPositionDto At(double top) => new(top, 0, 2000, 500, 0, 0);

    // The subscription id is generated inside the service; the JS call it made carries it. The module
    // handler is registered up front so the invocation is recorded rather than swallowed by loose mode.
    private readonly Bunit.BunitJSModuleInterop _module;

    public C_ScrollServiceTests() => _module = JSInterop.SetupModule(Module);

    private string LastSubscriptionId() =>
        (string)_module.Invocations["subscribe"].Last().Arguments[0]!;

    [Fact]
    public async Task GetPosition_ReturnsZero_WhenNoBrowser()
    {
        var pos = await Scroll.GetPositionAsync(cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(0, pos.Top);
        Assert.Equal(0, pos.Progress);
    }

    [Fact]
    public async Task Subscribe_FiresImmediately_AndReturnsDisposableToken()
    {
        ScrollChange? seen = null;
        var token = await Scroll.SubscribeAsync(
            c => seen = c, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.NotNull(token);
        Assert.NotNull(seen);
        Assert.True(seen!.Value.IsImmediate);
        Assert.Equal(ScrollDirection.None, seen.Value.Direction);

        await token.DisposeAsync();
    }

    [Fact]
    public async Task Subscribe_SuppressesImmediate_WhenNotRequested()
    {
        var fired = false;
        var token = await Scroll.SubscribeAsync(
            _ => fired = true, options: new ScrollSubscribeOptions { FireImmediately = false },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.False(fired);
        await token.DisposeAsync();
    }

    [Fact]
    public async Task Direction_And_Delta_Come_From_Successive_Positions()
    {
        var seen = new List<ScrollChange>();
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            c => seen.Add(c), options: new ScrollSubscribeOptions { FireImmediately = false },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        await service.OnScrolled(id, At(100));
        await service.OnScrolled(id, At(340));
        await service.OnScrolled(id, At(300));

        Assert.Equal(3, seen.Count);
        Assert.Equal(ScrollDirection.Down, seen[1].Direction);
        Assert.Equal(240, seen[1].Delta);
        Assert.Equal(ScrollDirection.Up, seen[2].Direction);
        Assert.Equal(-40, seen[2].Delta);
        Assert.True(seen[2].DirectionChanged);

        await token.DisposeAsync();
    }

    [Fact]
    public async Task DirectionOnly_Delivers_Only_The_Reversals()
    {
        var seen = new List<ScrollChange>();
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            c => seen.Add(c),
            options: new ScrollSubscribeOptions { FireImmediately = false, DirectionOnly = true },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        await service.OnScrolled(id, At(100));   // first move down - a reversal from None
        await service.OnScrolled(id, At(200));   // still down - suppressed
        await service.OnScrolled(id, At(300));   // still down - suppressed
        await service.OnScrolled(id, At(250));   // up - delivered

        Assert.Equal(2, seen.Count);
        Assert.Equal(ScrollDirection.Down, seen[0].Direction);
        Assert.Equal(ScrollDirection.Up, seen[1].Direction);

        await token.DisposeAsync();
    }

    [Fact]
    public async Task DirectionThreshold_Ignores_Jitter_And_Does_Not_Become_The_Baseline()
    {
        var seen = new List<ScrollChange>();
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            c => seen.Add(c),
            options: new ScrollSubscribeOptions { FireImmediately = false, DirectionOnly = true, DirectionThreshold = 8 },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        await service.OnScrolled(id, At(100));   // +100 down, clears the threshold - delivered
        await service.OnScrolled(id, At(97));    // -3 up, jitter - dropped, and must NOT flip the baseline
        await service.OnScrolled(id, At(200));   // back down; still "down", so nothing to report

        Assert.Single(seen);
        Assert.Equal(ScrollDirection.Down, seen[0].Direction);

        await token.DisposeAsync();
    }

    [Fact]
    public async Task A_disposed_token_stops_delivery()
    {
        var count = 0;
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            _ => count++, options: new ScrollSubscribeOptions { FireImmediately = false },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        await service.OnScrolled(id, At(50));
        await token.DisposeAsync();
        await service.OnScrolled(id, At(120));

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Two_subscribers_on_one_target_each_keep_their_own_baseline()
    {
        var service = (ScrollService)Scroll;
        var a = new List<double>();
        var b = new List<double>();

        var tokenA = await service.SubscribeAsync(
            c => a.Add(c.Delta), options: new ScrollSubscribeOptions { FireImmediately = false },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var idA = LastSubscriptionId();
        var tokenB = await service.SubscribeAsync(
            c => b.Add(c.Delta), options: new ScrollSubscribeOptions { FireImmediately = false },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var idB = LastSubscriptionId();

        Assert.NotEqual(idA, idB);
        await service.OnScrolled(idA, At(100));
        await service.OnScrolled(idA, At(150));
        await service.OnScrolled(idB, At(400));

        Assert.Equal([0, 50], a);
        Assert.Equal([0], b);   // B's own first notification, not a 400-pixel jump inherited from A

        await tokenA.DisposeAsync();
        await tokenB.DisposeAsync();
    }

    [Fact]
    public async Task Lock_returns_a_token_that_is_safe_to_dispose_twice()
    {
        var token = await Scroll.LockAsync();
        await token.DisposeAsync();
        await token.DisposeAsync();   // the second must not decrement the shared count again

        var calls = _module.Invocations.Select(i => i.Identifier).ToList();
        Assert.Equal(["lock", "unlock"], calls);
    }

    [Fact]
    public void Position_reports_progress_and_the_edges()
    {
        var top = new ScrollPosition(0, 0, 2000, 500, 0, 0);
        var mid = new ScrollPosition(750, 0, 2000, 500, 0, 0);
        var end = new ScrollPosition(1500, 0, 2000, 500, 0, 0);

        Assert.True(top.AtStart);
        Assert.False(top.AtEnd);
        Assert.Equal(0.5, mid.Progress, 3);
        Assert.False(mid.AtStart);
        Assert.True(end.AtEnd);
        Assert.Equal(1, end.Progress, 3);
    }

    [Fact]
    public void Content_that_does_not_overflow_reads_zero_progress()
    {
        var fits = new ScrollPosition(0, 0, 400, 500, 0, 0);
        Assert.Equal(0, fits.Progress);
        Assert.True(fits.AtStart);
        Assert.True(fits.AtEnd);   // nothing to scroll: both ends are here
    }

    [Fact]
    public void A_target_converts_from_a_selector_or_an_element()
    {
        Assert.True(ScrollTarget.Page.IsPage);
        Assert.True(((ScrollTarget)(string?)null).IsPage);
        Assert.True(((ScrollTarget)"   ").IsPage);   // a blank selector is the page, not a broken query

        ScrollTarget bySelector = ".app-content";
        Assert.Equal(".app-content", bySelector.Selector);
        Assert.Null(bySelector.Element);
        Assert.False(bySelector.IsPage);

        ScrollTarget byElement = default(ElementReference);
        Assert.NotNull(byElement.Element);
        Assert.Null(byElement.Selector);
    }

    // ---- horizontal axis ------------------------------------------------------
    // The raw Left / ScrollWidth / ClientWidth were always reported; what these cover is the derived
    // half that used to exist for the vertical axis alone.

    private static ScrollService.ScrollPositionDto AtX(double left) => new(0, left, 0, 0, 1200, 400);

    [Fact]
    public void Horizontal_progress_and_edges_mirror_the_vertical_ones()
    {
        var start = new ScrollPosition(0, 0, 0, 0, 1200, 400);
        Assert.Equal(0, start.HorizontalProgress);
        Assert.True(start.AtHorizontalStart);
        Assert.False(start.AtHorizontalEnd);
        Assert.True(start.OverflowsHorizontally);

        var middle = new ScrollPosition(0, 400, 0, 0, 1200, 400);
        Assert.Equal(0.5, middle.HorizontalProgress);

        var end = new ScrollPosition(0, 800, 0, 0, 1200, 400);
        Assert.True(end.AtHorizontalEnd);
        Assert.Equal(1, end.HorizontalProgress);

        // A bar whose content fits has nowhere to go, so an overflow affordance stays off.
        var fits = new ScrollPosition(0, 0, 0, 0, 300, 400);
        Assert.False(fits.OverflowsHorizontally);
        Assert.Equal(0, fits.HorizontalProgress);
    }

    [Fact]
    public void A_right_to_left_container_reports_progress_from_distance_travelled()
    {
        // Browsers hand back a negative scrollLeft in RTL. Progress is how far the reader has gone,
        // which is the magnitude - not a negative fraction.
        var rtl = new ScrollPosition(0, -800, 0, 0, 1200, 400);
        Assert.Equal(1, rtl.HorizontalProgress);
        Assert.True(rtl.AtHorizontalEnd);
        Assert.False(rtl.AtHorizontalStart);
    }

    [Fact]
    public async Task Horizontal_delta_and_direction_are_derived_alongside_the_vertical_ones()
    {
        var seen = new List<ScrollChange>();
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            c => seen.Add(c),
            options: new ScrollSubscribeOptions { FireImmediately = false },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        // The first notification only establishes the baseline, exactly as it does for the vertical axis.
        await service.OnScrolled(id, AtX(0));
        await service.OnScrolled(id, AtX(120));
        await service.OnScrolled(id, AtX(80));

        Assert.Equal(3, seen.Count);
        Assert.Equal(0, seen[0].HorizontalDelta);
        Assert.Equal(120, seen[1].HorizontalDelta);
        Assert.Equal(ScrollDirection.Right, seen[1].HorizontalDirection);
        Assert.Equal(-40, seen[2].HorizontalDelta);
        Assert.Equal(ScrollDirection.Left, seen[2].HorizontalDirection);

        // The vertical axis stayed still throughout and says so, rather than borrowing the other's answer.
        Assert.All(seen, c => Assert.Equal(ScrollDirection.None, c.Direction));

        await token.DisposeAsync();
    }

    [Fact]
    public async Task DirectionOnly_watches_the_axis_the_subscription_named()
    {
        var seen = new List<ScrollChange>();
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            c => seen.Add(c),
            options: new ScrollSubscribeOptions
            {
                FireImmediately = false,
                DirectionOnly = true,
                Axis = ScrollAxis.Horizontal,
            },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        await service.OnScrolled(id, AtX(100));   // first move right - a reversal from None
        await service.OnScrolled(id, AtX(200));   // still right - suppressed
        await service.OnScrolled(id, AtX(150));   // left - delivered

        Assert.Equal(2, seen.Count);
        Assert.Equal(ScrollDirection.Right, seen[0].HorizontalDirection);
        Assert.Equal(ScrollDirection.Left, seen[1].HorizontalDirection);

        await token.DisposeAsync();
    }

    [Fact]
    public async Task A_vertical_only_filter_ignores_horizontal_reversals()
    {
        var seen = new List<ScrollChange>();
        var service = (ScrollService)Scroll;
        var token = await service.SubscribeAsync(
            c => seen.Add(c),
            options: new ScrollSubscribeOptions { FireImmediately = false, DirectionOnly = true },
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var id = LastSubscriptionId();

        // Pure horizontal movement. The default axis is vertical, so none of it reaches the handler.
        await service.OnScrolled(id, AtX(100));
        await service.OnScrolled(id, AtX(50));
        await service.OnScrolled(id, AtX(300));

        Assert.Empty(seen);

        await token.DisposeAsync();
    }
}

// ------------------------------------------------------------------------------
// FlareScrollTop's subscription LIFECYCLE. The utility tests cover its markup and defaults; nothing
// covered what happens when a parameter moves after the first render, and the answer was "nothing" -
// the subscription was built once in OnAfterRenderAsync(firstRender) and never revisited.
// ------------------------------------------------------------------------------

public class C_FlareScrollTopLifecycleTests : FlareTestContext
{
    // Records what was subscribed to and lets a test push a position at the component.
    private sealed class FakeScrollService : IScrollService
    {
        public readonly List<string?> Subscribed = [];
        public readonly List<string?> Disposed = [];
        public readonly List<int> Throttles = [];
        private readonly List<(string? Selector, Func<ScrollChange, Task> Handler)> _live = [];

        public Task PushAsync(double top)
        {
            var change = new ScrollChange(new ScrollPosition(top, 0, 2000, 500, 0, 0),
                top, ScrollDirection.Down, false, false);
            return Task.WhenAll(_live.Select(l => l.Handler(change)));
        }

        public ValueTask<IAsyncDisposable> SubscribeAsync(
            Func<ScrollChange, Task> handler, ScrollTarget target = default,
            ScrollSubscribeOptions? options = null, CancellationToken cancellationToken = default)
        {
            var selector = target.Selector;
            Subscribed.Add(selector);
            Throttles.Add(options?.ThrottleMs ?? 0);
            var entry = (selector, handler);
            _live.Add(entry);
            return ValueTask.FromResult<IAsyncDisposable>(
                new Token(() => { Disposed.Add(selector); _live.Remove(entry); }));
        }

        public ValueTask<IAsyncDisposable> SubscribeAsync(
            Action<ScrollChange> handler, ScrollTarget target = default,
            ScrollSubscribeOptions? options = null, CancellationToken cancellationToken = default)
            => SubscribeAsync(c => { handler(c); return Task.CompletedTask; }, target, options, cancellationToken);

        private sealed class Token(Action onDispose) : IAsyncDisposable
        {
            public ValueTask DisposeAsync() { onDispose(); return ValueTask.CompletedTask; }
        }

        public ValueTask<ScrollPosition> GetPositionAsync(ScrollTarget target = default, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(default(ScrollPosition));
        public ValueTask ScrollToAsync(double top, ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth) => default;
        public ValueTask ScrollToTopAsync(ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth) => default;
        public ValueTask ScrollToEndAsync(ScrollTarget target = default, ScrollBehavior behavior = ScrollBehavior.Smooth) => default;
        public ValueTask ScrollIntoViewAsync(string elementId, ScrollAlign block = ScrollAlign.Nearest, ScrollBehavior behavior = ScrollBehavior.Smooth) => default;
        public ValueTask<IAsyncDisposable> LockAsync() => ValueTask.FromResult<IAsyncDisposable>(new Token(() => { }));
        public ValueTask DisposeAsync() => default;
    }

    private FakeScrollService UseFake()
    {
        var fake = new FakeScrollService();
        Services.AddSingleton<IScrollService>(fake);
        return fake;
    }

    // Moving Selector used to leave the listener on the old container while the click scrolled the new
    // one, so the button showed and hid against something it no longer controlled.
    [Fact]
    public void ChangingSelector_ResubscribesToTheNewContainer()
    {
        var fake = UseFake();
        var cut = Render<FlareScrollTop>(p => p.Add(x => x.Selector, ".left"));
        Assert.Equal([".left"], fake.Subscribed);

        cut.Render(p => p.Add(x => x.Selector, ".right"));

        Assert.Equal([".left", ".right"], fake.Subscribed);
        Assert.Equal([".left"], fake.Disposed);   // the old listener is not left behind
    }

    [Fact]
    public void ChangingThrottle_ResubscribesWithTheNewRate()
    {
        var fake = UseFake();
        var cut = Render<FlareScrollTop>(p => p.Add(x => x.ThrottleMs, 100));
        cut.Render(p => p.Add(x => x.ThrottleMs, 400));

        Assert.Equal([100, 400], fake.Throttles);
    }

    // A re-render that touches neither must not churn the subscription - resubscribing on every render
    // would trade one bug for a worse one.
    [Fact]
    public void AnUnrelatedRerender_KeepsTheSameSubscription()
    {
        var fake = UseFake();
        var cut = Render<FlareScrollTop>(p => p.Add(x => x.Selector, ".left"));
        cut.Render(p => p.Add(x => x.Selector, ".left").Add(x => x.Threshold, 300));

        Assert.Single(fake.Subscribed);
        Assert.Empty(fake.Disposed);
    }

    // Threshold needs no resubscribe, but it does need the position already known to be re-tested:
    // lowering it below the current offset must show the button now, not at the next scroll event.
    [Fact]
    public async Task ChangingThreshold_ReevaluatesTheKnownPosition()
    {
        var fake = UseFake();
        var cut = Render<FlareScrollTop>(p => p.Add(x => x.Threshold, 500));
        await cut.InvokeAsync(() => fake.PushAsync(300));
        Assert.DoesNotContain(Css.Classes.Scroll.TopVisible, cut.Markup);

        cut.Render(p => p.Add(x => x.Threshold, 200));
        Assert.Contains(Css.Classes.Scroll.TopVisible, cut.Markup);
    }
}
