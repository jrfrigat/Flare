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

## Priority order

Tier 0 - broken output or a crash in an app that follows the documented API.

| # | Issue | File |
| :-- | :-- | :-- |
| 1 | DataGrid icons render as literal text (`check_box`, `edit`) | [datagrid-icon-regression.md](datagrid-icon-regression.md) |
| 7 | Implicit child content reported to throw at runtime | [implicit-child-content.md](implicit-child-content.md) |
| 2 | `Sortable` + `Frozen` on one column reported to throw | [datagrid-frozen-sortable-crash.md](datagrid-frozen-sortable-crash.md) |
| - | `FlareCollapse` collapses itself on any parent re-render | [controlled-state-contract.md](controlled-state-contract.md) |
| 10, 11 | Flare owns only part of the app setup (no CSS reset, no `TimeProvider`) | [flare-app-setup-completeness.md](flare-app-setup-completeness.md) |

Tier 1 - the API forces a workaround. The app shipped, but with code the library should have provided.

| # | Issue | File |
| :-- | :-- | :-- |
| 4, 5, 6, 8, 12, 13 | Chart: fixed aspect ratio, chart-wide `Smooth`, `string` colors, no zoom, no directional annotation, no line style | [chart-excel-parity.md](chart-excel-parity.md) |
| - | `FlareSelect<T>` cannot express "no value" without a sentinel | [select-null-and-placeholder.md](select-null-and-placeholder.md) |
| - | `FlareCardActions` has alignment and nothing else | [card-actions-layout.md](card-actions-layout.md) |
| - | `FlareBottomNav` has no fixed/PWA mode | [bottom-nav-and-pwa-shell.md](bottom-nav-and-pwa-shell.md) |

Tier 2 - structure and documentation. Real, but nothing is blocked on them.

| # | Issue | File |
| :-- | :-- | :-- |
| 9 | `FlareColumn` describes, `FlareDataGrid` draws - the reporter wants the column to own its rendering | [datagrid-column-ownership.md](datagrid-column-ownership.md) |
| 3 | No Gallery example of three columns under one heading | folded into [datagrid-column-ownership.md](datagrid-column-ownership.md) |
| - | Density and API discoverability (`data-testid`, nested input, MD3E spacing) | [density-and-discoverability.md](density-and-discoverability.md) |

Raised separately, queued with them: [scroll-service-port.md](scroll-service-port.md) - five private scroll
listeners in the JS and no public way for an application to observe scroll at all.

## What is NOT accepted as reported

Two items are disputed, and the reasoning is in their own files rather than here:

- **Item 9** asks for `FlareColumn` to render its own cells. Taken literally that is slower than what
  the grid does now and breaks virtualization, column reorder and the composite header. The issue file
  proposes what the reporter actually needs - a column *strategy* object the grid consumes, and
  standalone components that drive a shared grid state - without moving cell rendering into the
  column component.
- **Items 2 and 7** could not be reproduced from the source. Both were tested at the level the report
  describes (see the files); the generated Razor for the implicit and explicit child-content forms is
  byte-identical, and a `Sortable`+`Frozen` column renders and sorts. Each file lists the narrower
  conditions that could still produce the reported failure, and both need the exception text before a
  fix can be called a fix.
