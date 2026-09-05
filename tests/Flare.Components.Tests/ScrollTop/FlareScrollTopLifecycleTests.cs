using Flare.Components;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareScrollTopLifecycleTests : FlareTestContext
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
