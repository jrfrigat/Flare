# `FlareBottomNav` has no fixed mode, and there are no mobile/PWA examples

**Status: PARTIALLY DONE. Tier 1. From the app user's review.**

## The report

> FlareBottomNav has no built-in fixed mode. That is a deliberate library decision, but a PWA still
> needs shell CSS with safe-area.
>
> [and, in the summary] add more ready-made mobile/PWA examples.

The reporter is being generous. The decision *was* deliberate - `bottomnav.css` says so in a comment -
but "the consumer positions it" means every consumer writes the same twenty lines of shell CSS, gets the
safe-area inset wrong, and forgets the body padding that stops the bar covering the last row of content.
The component already handles half of it (`padding-bottom: var(--flare-bottom-nav-safe-area-padding)`),
which makes the split arbitrary: it knows about the notch but not about being at the bottom of the
screen.

## Design

`Position` on `FlareBottomNav`: `Static` (today's behaviour, still the default for existing layouts),
`Sticky`, `Fixed`. Fixed and sticky both:

- pin the bar with `position: fixed/sticky; inset-inline: 0; bottom: 0`, at a token-driven z-index that
  sits under dialogs and over content;
- keep the existing safe-area padding, so a notched phone is right without an app-side rule;
- publish the bar's total height as a custom property on the root
  (`--flare-bottom-nav-height`), so a layout can reserve space with
  `padding-block-end: var(--flare-bottom-nav-height)` and content is never hidden behind the bar. A
  boolean `ReserveSpace` (default true) does that automatically by emitting the padding on the layout
  container when the bar is inside a `FlareLayout`.

That is the whole component change, and it deletes the shell CSS the reporter had to write.

## The examples half

The bigger request is "more ready-made mobile/PWA examples". Today the Gallery demonstrates components,
one at a time, on a desktop-width page. There is no page that shows an *app shell*: fixed bottom nav,
scrollable content, safe areas, a sticky app bar, a phone-width viewport.

The component half is DONE, with a bottom-nav shell demo in the Gallery. The rest of this list is still
OPEN - add a Gallery section with the shells a mobile app actually needs, each a working page rather than
a snippet:

1. **Bottom-nav shell** - fixed bar, five destinations, routed content, safe area, reserved space.
2. **Drawer + app bar shell** - the tablet form of the same app.
3. **List / detail drill-down** - the pattern the nav redesign deferred.
4. **Form on a phone** - fields, a stacked `FlareCardActions`, a sticky submit bar above the keyboard.
5. **Offline / update** - `IVersionCheckService` wired to a snackbar, which exists and is undocumented.

These also give the density work in [density-and-discoverability.md](density-and-discoverability.md) a
place to be judged: MD3 Expressive at default spacing on a 375px viewport is the case the reporter says
is too loose, and until the Gallery shows it, nobody sees it.
