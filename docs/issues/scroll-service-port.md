# A scroll port: one service for observing and driving scroll

**Status: OPEN. Queued behind the 0.18.1 field report, ahead of the density work.**

## Why

Flare already has the right shape for this and only for the OTHER axis. `IBrowserViewportService` is the
single, documented, DI-injected port for everything resize-related: one JS listener shared across all
subscribers, `IAsyncDisposable` tokens instead of an observer interface, a fallback on a disconnected
circuit. Scroll has no such port. It has five private ones.

Read from the JS as it stands today:

| Module | What it does | Owns |
| :-- | :-- | :-- |
| `flare-ui.js` | `registerScrollTopHandler` - watches window or a selector for `FlareScrollTop` | its own `Map`, its own listener |
| `flare-ui.js` | `registerTabScroller` - watches a tab bar for overflow/edges | a second `Map`, a listener + a `ResizeObserver` |
| `flare-overlay.js` | `window.addEventListener('scroll', place, { capture: true })` - repositions anchored panels | a third registry |
| `flare-collision.js` | `window.addEventListener('scroll', handleReposition)` - the collision engine | a fourth |
| `flare-components.js` | a per-target `scroll` listener for the grid | a fifth |

Five listeners, five disposal paths, five chances to leak one, and no throttling shared between them: on a
page with an open menu, a sticky TOC and a scroll-top button, one wheel gesture fans out to several
independent handlers that each cross the interop boundary on their own schedule.

And the part that actually reached us as a request: **an application cannot observe scroll at all.** There
is no public API. A consumer who wants "hide the app bar while scrolling down", "load more at 80%", "show
a progress rail", or "mark the section the reader is in" has to write their own JS module and their own
interop, next to the five Flare already ships.

## What to build

`IScrollService` in `Flare.Abstractions`, implementation in `Flare.Infrastructure`, registered by
`AddFlare` - the same three-ring shape as `IBrowserViewportService`, and deliberately modelled on it so
there is one idiom to learn rather than two.

```csharp
public interface IScrollService : IAsyncDisposable
{
    // One-shot reads.
    ValueTask<ScrollPosition> GetPositionAsync(ElementReference? target = null, CancellationToken ct = default);

    // Subscriptions. Dispose the token to unsubscribe; one JS listener per target is shared.
    ValueTask<IAsyncDisposable> SubscribeAsync(
        Func<ScrollChange, Task> handler, ScrollSubscribeOptions? options = null, CancellationToken ct = default);

    ValueTask<IAsyncDisposable> ObserveElementAsync(
        ElementReference target, Func<ScrollChange, Task> handler,
        ScrollSubscribeOptions? options = null, CancellationToken ct = default);

    // Driving it.
    ValueTask ScrollToAsync(double top, ScrollBehavior behavior = ScrollBehavior.Smooth, ElementReference? target = null);
    ValueTask ScrollToTopAsync(ElementReference? target = null, ScrollBehavior behavior = ScrollBehavior.Smooth);
    ValueTask ScrollToEndAsync(ElementReference? target = null, ScrollBehavior behavior = ScrollBehavior.Smooth);
    ValueTask ScrollIntoViewAsync(string elementId, ScrollAlign block = ScrollAlign.Nearest);

    // The lock the overlay family already needs, promoted out of IOverlayJsService and REFERENCE-COUNTED,
    // so a dialog opened over a drawer does not release the body when the inner one closes.
    ValueTask<IAsyncDisposable> LockAsync();
}
```

`ScrollChange` carries what a subscriber actually branches on, so nobody recomputes it from raw offsets:
`Top`, `Left`, `Delta`, `Direction` (Up/Down/None), `AtStart`, `AtEnd`, `Progress` (0..1), and
`ScrollHeight`/`ClientHeight`. `ScrollSubscribeOptions` carries `ThrottleMs` (default 100, trailing),
`FireImmediately`, and `DirectionOnly` - the last so a handler that only cares about up-versus-down is not
woken for every pixel.

Design rules, all of them lifted from what `IBrowserViewportService` already got right:

- **One JS listener per target**, fanned out server-side. Ten subscribers on the window cost one listener.
- **Throttled in JS**, not in C#: the interop boundary is the expensive part, so the trailing throttle has
  to happen before the crossing, not after.
- **`IAsyncDisposable` tokens**, no observer interface, no subscription ids for the caller to track.
- **Prerender-safe**: getters return a zero position, subscriptions attach lazily once JS exists, and every
  call catches `JSException` alongside the disconnect cases.

## Then move the five onto it

The port only pays for itself once the existing users go through it. In order, because each one is a
smaller change than the last:

1. **`FlareScrollTop`** - the closest fit; deletes `registerScrollTopHandler` and its map outright.
2. **`FlareOnThisPage`** - currently an `IntersectionObserver`; keep the observer for *which* heading, use
   the port for the progress rail and the "scrolled past the top" state.
3. **`FlarePopup` / `FlareMenu` / `FlareTooltip` / the collision engine** - the two window listeners in
   `flare-overlay.js` and `flare-collision.js` become one subscription each. This is the one that removes
   duplicated work on a real page.
4. **Body scroll lock** - `IOverlayJsService.LockBodyScrollAsync` moves to `IScrollService.LockAsync` and
   gains the reference count it is missing today.
5. **`FlareTabs`** - the tab-bar scroller is a per-element case with its own `ResizeObserver`; it moves last
   because it is the least like the others and gains the least.

`FlareDataGrid`'s sticky-header sync and `FlareInfiniteScroll` stay on `IntersectionObserver`: an observer
that fires on a threshold crossing is strictly cheaper than a scroll subscription that has to compute the
same thing, and the port is not an excuse to make them worse.

## What this unlocks for applications

Named because they are the requests that made this issue, not as speculation: a scroll-linked app bar
(hide on the way down, show on the way up), a reading-progress rail, "load more" tied to a percentage
rather than a sentinel, restoring scroll position across navigation, and scroll-spy over arbitrary
content rather than only the headings `FlareOnThisPage` knows about. Two of those - the app bar and the
progress rail - are small enough to ship as components in the same batch once the port exists, and both
are on the parity roadmap already.

## Queue

After `density-and-discoverability`, the last structural item of the 0.18.1 field report,
and before the mobile/PWA shells in `bottom-nav-and-pwa-shell`, which want the scroll-linked app bar this
port makes possible.
