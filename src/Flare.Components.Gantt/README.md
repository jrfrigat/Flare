# Flare.Components.Gantt

A universal timeline for Flare: Gantt charts, resource lanes, bookings, roadmaps — anything laid out over time in rows.

> Incubated in the Alva repository (Alva STORY-21) and to be moved into Flare (Flare EPIC-4). It references Flare
> only and knows nothing about the application that uses it. The Russian README (`README.ru.md`) is the primary one.

## Boundary
- **Draws and reports; does not compute the domain.** Capacity, overload, risk and people's schedules are the
  consumer's — it passes ready dates, values and statuses.
- **Never mutates the consumer's data.** Every user action arrives as a cancellable event carrying the changes;
  the consumer applies them.
- **Built-in calculations** are only generic graph geometry (`GanttGraph`): dependency cascade, critical path,
  cycle checks; plus summary roll-up (`GanttRollup`).

## Setup
```xml
<ProjectReference Include="..\Flare.Components.Gantt\Flare.Components.Gantt.csproj" />
```
```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
```

## Model
- `GanttSpan` — half-open [Start, End). Whole days use `Day` and `FromDays`; instants use `FromOffsets`.
- `GanttScale` — Hour, Day, Week, Month, Quarter or Year.
- Layers:
  - `GanttSegment`;
  - `GanttMarker`;
  - `GanttRegion`;
  - `GanttCell`;
  - `GanttLink` (FS/SS/FF/SF with lag);
  - `GanttChange`.
- Rows and items are **your own types** with selectors.
- A **Kind** is a string key, rendered as `flare-gantt--kind-{kind}` and `data-kind`. You decide how it looks.

## Components
- `FlareGantt` — the shared axis: range, scale, geometry, culture, non-working days, global markers and
  regions, selection, compact mode.
- `FlareGanttPanel<TRow, TItem>` — one projection: rows (list or tree), items, cells, regions, markers, links,
  critical path, suggestions, roll-up, and editing.

Put several panels in one `FlareGantt` to get a synchronised multi-projection view: they share one scale, one
scroll and one selection.

## Interaction
- **Pointer:** move and resize, with a snapped preview. Leaving the surface cancels the gesture.
- **Keyboard:**
  - Ctrl+←/→ moves;
  - Ctrl+Shift+←/→ resizes the end;
  - Ctrl+↑/↓ changes the row;
  - Enter/Space selects;
  - L starts a link, Enter or a click on the target creates it.
- **`OnChange`** carries the primary change plus the cascade.
- **`DropGroup`** makes every row one `FlareDropZone` for an outside `FlareDragContext`; the axis lights the unit under the
  pointer, and `await gantt.ResolveDrop(e.TargetId)` (a `@ref` to `FlareGantt`) returns the row and unit as `GanttDropTarget`.
- **`ItemDropGroup`** makes each bar (not summaries or suggestions) a drop zone for dropping onto the item itself, such
  as a person onto a task; read it with `GanttItemDropTarget.TryParse`. **`DropAccepts`** and **`ItemDropAccepts`** say
  which payloads units and bars take. During a drag, whatever in a track does not take the payload lets the pointer through,
  so a payload only units take still lands on a day under a bar.
- **`ItemCover`** is the span an item must keep covering, such as its children's work: resizing stops at its edges.
- **`FillWidth`** on `FlareGantt` stretches units to the visible width (`UnitWidth` is the floor); **`ResizableSide`**
  with **`SideWidthChanged`** lets the user drag the row-header edge; **`ScrollTo`** scrolls the axis to a moment;
  **`Virtualize`** draws only the rows and columns near what is on screen, for charts of hundreds of rows and long axes;
  **`Endless`** with **`@bind-Range`** grows the axis when the view nears an edge (keeping the same moment in view when
  it grows on the left), and `ScrollTo` grows it to a moment outside the range. Alt + wheel scrolls the axis sideways.

## Styling
Every look is a `--flare-gantt-*` variable falling back to shared Flare tokens. Set the variables on a kind's
class to style that kind.

## Deferred to the move into Flare
- `GanttTokens` in the theme.
- Rebasing the axis and lane engine on Flare's scheduler (Flare STORY-21).
- Row virtualisation.
- Baseline UI.
- Through a Flare JS port:
  - arrow-key focus;
  - scroll suppression;
  - pointer capture;
  - drag-to-link.
- Touch verification, Gallery pages and `docs/api`.
