# Field report against 0.18.1 - defect roll-up and work plan

**Status: OPEN. This is the parent issue for the first real third-party app built on Flare.**

Two sources: the app author's own list of thirteen defects, and a separate review comment from a user of
that app. Every item below is either reproduced against the source, or marked as *needs the exception
text* where the report could not be reproduced from the code as written. Nothing here is filed on
speculation alone.

The mandate for this batch is the one the reporter set: **the public API may change freely - old shapes
need not survive - but the replacement has to be right, so the same class of defect cannot come back.**
That rules out patch-on-top fixes for the structural items (9, chart sizing, controlled state) and
requires the underlying contract to be restated instead.

## Still open

Tier 0 - broken output or a crash in an app that follows the documented API.

| # | Issue | File |
| :-- | :-- | :-- |
| 7 | Implicit child content reported to throw at runtime | [implicit-child-content.md](implicit-child-content.md) |
| 2 | `Sortable` + `Frozen` on one column reported to throw | [datagrid-frozen-sortable-crash.md](datagrid-frozen-sortable-crash.md) |

Tier 1 - the API forces a workaround. The app shipped, but with code the library should have provided.

| # | Issue | File |
| :-- | :-- | :-- |
| - | `FlareBottomNav` has no fixed/PWA mode | [bottom-nav-and-pwa-shell.md](bottom-nav-and-pwa-shell.md) |

Tier 2 - structure and documentation. Real, but nothing is blocked on them.

| # | Issue | File |
| :-- | :-- | :-- |
| - | Density and API discoverability (`data-testid`, nested input, MD3E spacing) | closed - `Size` already carried it; the broken size ramp behind it was the real defect, fixed in 0.22.0 |

Raised separately and since delivered: `IScrollService` (0.21.0), which replaced the private scroll
listeners in the JS and gave an application a public way to observe scroll at all.

## Closed

| # | Item | Shipped in |
| :-- | :-- | :-- |
| 1 | DataGrid icons render as glyphs again; every remaining Material Symbols ligature span is gone, with a guard test | 0.19.0 |
| - | `FlareCollapse` no longer collapses itself; the two-way contract is written down and enforced by `ControlledStateContractTests` | 0.19.0 |
| 10, 11 | `AddFlare` is sufficient on its own (`TimeProvider`), and Flare ships a `:where()` document reset | 0.19.0 |
| 4, 5, 6, 8, 12, 13 | Chart: fluid width, per-series `Smooth`/`Area`/`LineStyle`, `FlareColor` series colors, Excel-style zoom, directional annotations | 0.19.0 |
| - | `FlareSelect<T>` expresses "no value" without a sentinel (`NullOption`) | 0.19.0 |
| - | `FlareCardActions` gained wrap, vertical, full-width, reverse and a stack breakpoint | 0.19.0 |
| 9 | Column management is drivable from outside: `DataGridContext<TItem>`, `FlareDataGridControl<TItem>`, `ColumnDefinitions` | 0.20.0 |
| 3 | Gallery example of three columns under one heading | 0.19.0 |

## What is NOT accepted as reported

- **Item 9** asked for `FlareColumn` to render its own cells. Taken literally that is slower than what
  the grid does now and breaks virtualization, column reorder and the composite header: a 20x200 grid
  would become 4000 component instances instead of 4000 render-tree frames. What shipped instead is what
  the request was reaching for - the column model as public data (`DataGridColumn<TItem>` plus
  `ColumnDefinitions`) and a context that lets any component read, drive and observe the grid - with
  cell rendering left where it performs.
- **Items 2 and 7** could not be reproduced from the source. Both were tested at the level the report
  describes (see the files); the generated Razor for the implicit and explicit child-content forms is
  byte-identical, and a `Sortable`+`Frozen` column renders and sorts. Each file lists the narrower
  conditions that could still produce the reported failure, and both need the exception text before a
  fix can be called a fix.
