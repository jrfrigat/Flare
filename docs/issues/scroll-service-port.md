# A scroll port: the remaining listeners

**Status: PARTIALLY DONE. The port ships in 0.21.0; three of the five internal users still have their
own listener.**

## What landed

`IScrollService` (`Flare.Abstractions`), `ScrollService` (`Flare.Infrastructure`), `flare-scroll.js`,
registered by `AddFlare`. Modelled on `IBrowserViewportService` so there is one idiom, not two:
`IAsyncDisposable` tokens rather than an observer interface, throttling in JS before the interop
crossing, prerender-safe getters, and one target per subscription addressed by `ScrollTarget` - the page
by default, or an `ElementReference` or CSS selector, both of which convert implicitly.

`ScrollChange` carries `Position`, `Delta`, `Direction`, `DirectionChanged` and `IsImmediate`, with
`Top` / `Progress` / `AtStart` / `AtEnd` forwarded off the position, so no subscriber recomputes them.
`ScrollSubscribeOptions` carries `ThrottleMs`, `FireImmediately`, `DirectionOnly` and
`DirectionThreshold`.

Migrated:

1. **`FlareScrollTop`** - `registerScrollTopHandler` and its map are gone from `flare-ui.js`, along with
   `IUiJsService.RegisterScrollTopAsync` / `RemoveScrollTopAsync` / `ScrollToTopAsync`. The old handler
   crossed interop on *every* scroll event with no throttle at all; it now shares the service's.
2. **Body scroll lock** - `IOverlayJsService.LockBodyScrollAsync` / `UnlockBodyScrollAsync` are gone,
   replaced by `IScrollService.LockAsync()` returning a token. The reference count the original issue
   asked for already existed in `flare-overlay.js`; what was actually missing was a token that cannot
   be released twice, and scrollbar-width compensation so fixed chrome does not jump sideways when the
   lock lands. `FlareDialog` and `FlareLayoutDrawer` hold the token.

## What is left

Each of these still owns a private `scroll` listener. In order, largest win first:

3. **`FlarePopup` / `FlareMenu` / `FlareTooltip` / the collision engine** - the two capture-phase window
   listeners in `flare-overlay.js` and `flare-collision.js`. This is the one that removes duplicated
   work on a real page: with a menu open and a sticky TOC, one wheel gesture currently fans out to
   several independent handlers that each cross interop on their own schedule. Note these use
   `{ capture: true }`, which the port does not expose yet - an anchored panel has to reposition when
   ANY ancestor scrolls, not only the window. Adding a `Capture` option to `ScrollSubscribeOptions` is
   a prerequisite.
4. **`FlareDataGrid`** - the per-target listener in `flare-components.js` for sticky-header sync.
5. **`FlareTabs`** - the tab-bar scroller in `flare-ui.js`, a per-element case with its own
   `ResizeObserver`. Moves last: least like the others, gains the least.

**`FlareOnThisPage`** was listed in the original plan and is deliberately dropped from it: an
`IntersectionObserver` firing on a threshold crossing is strictly cheaper than a scroll subscription
recomputing the same thing. The same reasoning keeps `FlareInfiniteScroll` where it is. The port is not
an excuse to make either worse.

## What it unlocks

The application-facing half is done: a scroll-linked app bar (`DirectionOnly` + `DirectionThreshold` is
exactly that), a reading-progress rail (`Progress`), "load more" on a percentage (`AtEnd`), scroll
restoration across navigation, and scroll-spy over arbitrary content. The app bar and the progress rail
are small enough to ship as components and are on the parity roadmap.
