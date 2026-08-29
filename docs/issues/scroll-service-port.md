# A scroll port: the remaining listeners

**Status: DONE. The port shipped in 0.21.0; the three listeners this issue still wanted moved were
audited in 0.22.0 and none of them should be moved. What they needed instead was done.**

## What landed in 0.21.0

`IScrollService` (`Flare.Abstractions`), `ScrollService` (`Flare.Infrastructure`), `flare-scroll.js`,
registered by `AddFlare`. Modelled on `IBrowserViewportService` so there is one idiom, not two:
`IAsyncDisposable` tokens rather than an observer interface, throttling in JS before the interop
crossing, prerender-safe getters, and one target per subscription addressed by `ScrollTarget` - the page
by default, or an `ElementReference` or CSS selector, both of which convert implicitly.

`ScrollChange` carries `Position`, `Delta`, `Direction`, `DirectionChanged` and `IsImmediate`, with
`Top` / `Progress` / `AtStart` / `AtEnd` forwarded off the position, so no subscriber recomputes them.
`ScrollSubscribeOptions` carries `ThrottleMs`, `FireImmediately`, `DirectionOnly` and
`DirectionThreshold`.

Migrated then: **`FlareScrollTop`** (its old handler crossed interop on *every* scroll event with no
throttle) and the **body scroll lock** (`IScrollService.LockAsync()` returning a token that cannot be
released twice, plus scrollbar-width compensation).

## The audit that closed it

The plan listed three more listeners to migrate. Reading them established that the plan's premise was
wrong: it assumed each one crossed interop on every scroll event, the way `FlareScrollTop` did. Only
one of the three did. There are six `scroll` listeners in the whole JS bundle:

| Listener | Crosses interop? | Verdict |
| :-- | :-- | :-- |
| `flare-scroll.js` | yes, throttled | the port itself |
| `flare-ui.js` tab scroller | **yes, every event, unthrottled** | **fixed - see below** |
| `flare-components.js` scroll-spy | yes, rAF-coalesced and change-gated | already correct |
| `flare-overlay.js` anchored panel | **no - pure JS** | must not move |
| `flare-highlight.js` editor sync | **no - pure JS** | must not move |
| `flare-collision.js` | never ran at all | deleted |

**`positionAnchoredPanel` must not be migrated.** It repositions a fixed panel under its anchor entirely
inside JS. Routing it through the port would insert a C#-to-JS round trip into a reposition that has to
land in the same frame as the scroll; a dropdown lagging its field by a throttle interval is visibly
broken. It is already capture-phase (so nested scrollers count), already removes both its listeners in
`removeAnchoredPanel`, and at most one such panel is open at a time. Nothing to gain.

**`flare-collision.js` had no listener in practice.** `setupCollision` added a window `scroll` and a
window `resize` listener per call and removed neither in its own `destroy()` - a genuine leak, except
that nothing ever called it. `ICollisionService` only ever invoked the pure `calculatePlacement`.
Deleted, 46 lines, with a note in the module header saying where anchored repositioning actually lives.

**The DataGrid sticky-header listener the plan named does not exist.** Frozen columns are CSS
`position: sticky` with cumulative `left` offsets recomputed on resize (`updateFrozenOffsets`). There is
no scroll handler to move.

**The tab scroller was the one real defect**, and the port was still the wrong tool for it: the bar
needs `scrollWidth` / `clientWidth` / `scrollLeft` horizontal metrics that `ScrollPosition` does not
carry, and a `ResizeObserver` besides. It reported three booleans to C# on *every* scroll event.
Fixed in place - coalesce to a frame, and only invoke when one of the three actually flips. Measured
against a synthetic overflowing bar: **60 scroll events that change nothing now produce 1 interop call
instead of 60**, reaching either end produces exactly 1, and disposal produces 0.

**`FlareOnThisPage` and `FlareInfiniteScroll`** stay where they are, as the 0.21.0 write-up already
argued: an `IntersectionObserver` firing on a threshold crossing is strictly cheaper than a scroll
subscription recomputing the same thing. The scroll-spy that remains is rAF-coalesced and only crosses
interop when its visible set changes, which is the same shape as the tab-scroller fix.

## What the port unlocks

The application-facing half is the point, and it is done: a scroll-linked app bar (`DirectionOnly` +
`DirectionThreshold`), a reading-progress rail (`Progress`), "load more" on a percentage (`AtEnd`),
scroll restoration across navigation, and scroll-spy over arbitrary content. The app bar and the
progress rail are small enough to ship as components and are on the parity roadmap.

## The lesson worth keeping

"Move every listener onto the port" was the wrong goal. A port earns its keep where a listener crosses
interop on a stream of events; where the work is pure JS, routing it through C# is a regression. The
number that mattered was never the count of listeners - it was interop crossings per gesture.
