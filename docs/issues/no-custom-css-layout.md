# No-CSS promise vs. layout reality: standard patterns force app CSS

**Status: OPEN. Positioning / UX issue. Evidence: a real app (OrderingPlatform, 0.26.2)
whose entire custom CSS exists only because standard layout patterns have no component-level
switch.**

Flare's pitch includes "no side CSS, everything is tokens" (README: "Без Bootstrap / без
сторонних CSS - все стили используют только токены var(--flare-*)"). The audience that
makes this valuable is the developer who does not want to learn CSS at all - the backend
developer assembling an admin screen. That promise holds for coloring, spacing and
typography, and it breaks precisely at layout.

Building one ordinary screen - a task card: two equal halves, a 250px chart that squeezes
one half, grids that fill their boxes and scroll inside - required the app to write and
maintain these rules, each with a comment explaining a Flare internal:

```css
.flare-layout-content > .flare-layout__content-frame { height: 100%; }  /* FlareLayout gap */
.tabs-fill .flare-tabs__panels { flex: 1 1 0; min-height: 0; display: flex; flex-direction: column; }
.tabs-fill .flare-tabs__panel:not(.flare-tab-panel--hidden) { flex: 1 1 0; min-height: 0; display: flex; flex-direction: column; }
.grid-fill > .flare-datagrid__wrapper { flex: 1 1 0; min-height: 0; overflow-y: auto; }
```

Every line of it is knowledge a Flare user should not need: `min-height: 0` on a flex
item, `:not(.flare-tab-panel--hidden)` to keep inactive panels hidden, percentage heights
against definite parents. Get any of it wrong and the failure mode is nasty (a grid that
silently disappears, or paints over its neighbors - see `tabs-full-height.md`).

## Ask

Close the gap at the component level so this class of app CSS goes away:

- `FlareTabs.FillHeight` (stretch panels, panel becomes a flex column) - see
  `tabs-full-height.md`.
- A screen-fit pattern for `FlareLayout` / `FlareLayoutContent` (frame height, or a
  documented `Fit="Screen"` mode) - see `tabs-full-height.md`.
- A grid scroll mode with fill behavior and a sticky header - see
  `datagrid-scroll-without-pagination.md`.

The test is simple: a backend developer builds the screen above from parameters alone,
zero custom CSS, without opening DevTools.
