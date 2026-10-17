# Flare.Components.Gantt

Gantt timeline for the [Flare](https://github.com/jrfrigat/Flare) Blazor component library: a task tree, people,
rooms or machines laid out in time on one axis. Add-on package that extends `Flare.Components`.

```sh
dotnet add package Flare.Components.Gantt
```

## Setup

```csharp
builder.Services.AddFlare();
builder.Services.AddFlareGantt();
```

```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
```

Requires a `Flare.Theme.*` package: the timeline's look is the theme's `GanttTokens`.

## What it does and does not do

- **Draws and reports, never computes the domain.** Capacity, overload, risks and schedules are the application's;
  it passes ready dates, values and statuses.
- **Never changes the application's data.** Every user action arrives as a cancellable event with the proposed
  positions; the application applies them (or not).
- **Built-in calculations** are graph geometry only (`GanttGraph`): pushing successors, the critical path, cycle
  checks; plus the summary of child bars (`GanttRollup`).

## Model

| Type | What it is |
| --- | --- |
| `GanttSpan` | Half-open interval [Start, End). `Day`, `FromDays(first, last)` for whole days, `FromOffsets(..., zone)` for instants. |
| `GanttScale` | Axis step: Hour, Day, Week, Month, Quarter, Year. |
| `GanttSegment(Kind, Span, Label?)` | Part of a bar: work, waiting, a stage. |
| `GanttMarker(Kind, At, RowKey?, Label?)` | A point in time: a milestone, a deadline, today. Without `RowKey` it spans every row. |
| `GanttRegion(Kind, Span, RowKey?, Label?)` | A highlighted range: a sprint, a holiday, maintenance. |
| `GanttCell(RowKey, Span, Kind, Text?, Title?)` | A row's value over a stretch of the axis with a status: hours per day, occupancy. |
| `GanttLink(From, To, Type, Lag, Kind?)` | A FS / SS / FF / SF dependency with a lag. |
| `GanttChange(ItemKey, Span, RowKey?)` | A new position of an item; suggestions (`Suggested`) use it too. |

Rows and items are **your own types** with selectors - no wrappers. A **Kind** is a string key rendered as the
`flare-gantt--kind-{kind}` class and `data-kind`.

## Components

- `FlareGantt` - the shared axis: `Range`, `Scale`, `UnitWidth`, `RowHeight`, `BarInset`, `SideWidth`, `Culture`,
  `NonWorking`, chart-wide `Markers` and `Regions`, `SelectedKey`/`SelectedKeyChanged`, `Compact`, `Corner`, `Texts`,
  `FillWidth`, `Virtualize` (draws only the rows and columns near the view), `Endless` + `@bind-Range`,
  `ResizableSide` + `SideWidthChanged` (the column edge is a `FlareSplitter`), `ScrollTo(moment)`.
- `FlareGanttPanel<TRow, TItem>` - one projection. Any number of panels share the axis, the scroll and the
  selection by item key. Rows (`Rows`, `RowKey`, `RowChildren`, `RowHeader`, `RowClass`), items (`Items`, `ItemKey`,
  `ItemRow`, `ItemSpan`, `ItemKind`, `ItemSegments`, `ItemProgress`, `ItemLabel`, `ItemTemplate`, `ItemTooltip`,
  `OnItemClick`), layers (`Cells`, `Regions`, `Markers`, `Links`, `ShowCriticalPath`, `Suggested`, `RollupParents`),
  editing (`Editable`, `ScheduleMode`, `AllowRowChange`, `ItemCover`, `NewLinkType`, `OnChange`, `OnLinkCreate`,
  `OnLinkRejected`, `DropGroup`, `ItemDropGroup`).
- `FlareGanttLegend` - a key whose samples are the same elements the chart draws.

## Interaction

- **Pointer:** drag a bar (time and row), drag its edges to resize, drag the dot at its end onto another bar to
  link them. The browser draws the preview; .NET hears one call when the gesture ends. Escape cancels.
- **Keyboard:** Ctrl+Left/Right moves, Ctrl+Shift+Left/Right moves the end, Ctrl+Up/Down changes the row (the focus
  stays on the bar), Enter/Space selects, L starts a link and Enter or a click on the target creates it, Escape cancels.
  Announcements go to an `aria-live` region.
- **Outside drag and drop:** with `DropGroup` every row is one `FlareDropZone`; resolve the unit under the pointer
  with `await gantt.ResolveDrop(e.TargetId)`. `ItemDropGroup` makes the bars drop zones (`GanttItemDropTarget`).

## Look and texts

- Every look is a `--flare-gantt-*` token (`GanttTokens`) the theme sets. Restyle one kind by setting the same
  variables on its class:

```css
.flare-gantt--kind-waiting {
    --flare-gantt-bar-bg: transparent;
    --flare-gantt-bar-border: var(--flare-color-error);
}
```

- Texts come from the package resources (English and Russian) in the chart's `Culture`. Replace any of them with
  `Texts="new GanttTexts { Moved = ... }"`; the rest stay.

## Coming from the Alva incubator

The package grew in the Alva repository. Differences from that copy:
`AddFlareGantt()` is required; `GanttTexts` holds optional overrides instead of whole `English`/`Russian` sets;
the row-header column is resized with `FlareSplitter` (the narrower/wider/reset buttons are gone, Home or a double
click resets, and `SideWidthChanged` reports pixels); leaving the chart no longer cancels a drag (Escape does);
the look is the theme's `GanttTokens` instead of CSS fallbacks.

Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
