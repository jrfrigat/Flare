# FlareTileLayout: a dashboard grid the user can rearrange

**Status: OPEN. Phase 3, medium.**

Radzen has `RadzenTileLayout` with draggable, resizable tiles; none of the other three has anything.
Flare has `FlareGrid` (static), `FlareResizable` (a single element, one edge at a time - `Edge`,
`InitialSize`, `MinSize`, `MaxSize`, `OnResized`) and `FlareKanban` (drag between columns). The missing
piece is the composition: a grid of panels the end user can reorder and resize, with the arrangement
persisted.

This matters more than its size suggests, because it is the surface every internal business application
eventually asks for, and it pairs directly with the gauge and chart work in Phase 1.

## Scope

`FlareTileLayout` plus `FlareTile`, in the core package - it is layout, it has no heavy dependency, and it
belongs beside `FlareGrid`.

```razor
<FlareTileLayout Columns="12" RowHeight="80" @bind-State="dashboardState">
    <FlareTile Title="Revenue" ColSpan="6" RowSpan="2">...</FlareTile>
    <FlareTile Title="Signups" ColSpan="3" RowSpan="2" Resizable="false">...</FlareTile>
</FlareTileLayout>
```

Behaviour:

1. **Column grid with span-based placement**, not free pixel positioning. Free positioning looks flexible
   and produces unusable layouts; a column grid keeps a dashboard aligned and makes the responsive story
   possible at all.
2. **Drag to reorder** with a live placeholder showing the landing slot, and reflow of the tiles behind it.
3. **Resize by span**, snapping to grid cells, with `MinColSpan` / `MaxColSpan` per tile.
4. **Responsive collapse**: below a breakpoint, tiles stack to full width in order. Drive it from the
   existing `Breakpoint` enum and `FlareMediaQuery` - no new breakpoint system.
5. **State in and out** - `State` is a serializable record of tile id, span and order, two-way bound, so
   an application persists it wherever it likes. Optionally through `IBrowserStorage`, which already
   exists as a port; the component must not reach for storage itself.
6. **Per-tile chrome**: header with title, an actions slot, optional collapse and close, all optional.
   Built from `FlarePaper` plus `FlareIconButton`, not new elements.
7. **Locked mode** - `AllowDrag` / `AllowResize` off - because most dashboards are edited rarely and
   viewed constantly, and a view-mode dashboard must not jiggle under the cursor.

## Implementation notes

- **Pointer events, not HTML5 drag.** `FlareKanban` uses `ondragstart` / `ondragover`, which do not fire
  on touch. The tile layout must work on a tablet, so it uses pointer events - and this is the second
  issue after the Scheduler to need them, which makes the shared pointer-drag primitive worth extracting
  properly rather than writing a third time.
- Layout math (which tiles move when one is dropped) is a pure function over the state record. Test it
  without a DOM.
- No JS: CSS grid does the placement, the drag reads pointer coordinates and maps them to cells in C#.
- Reduced-motion suppresses the reflow transition.

## Tokens

Shares the Scheduler/Gantt surface family per roadmap rule 3 where it genuinely overlaps (drag
placeholder, resize handle, selection outline). Tile-specific and `required`: gap, tile surface / radius /
elevation, header height / typescale / gap, hover and dragging elevation, placeholder fill and border,
resize-handle size and color, and the locked-state treatment.

## Done when

- Reorder and resize work with pointer and touch, and with keyboard alone (grab, move by arrows, drop).
- The arrangement round-trips through `State` across a page reload.
- Collapse-to-stack happens at the breakpoint with no layout jump.
- The layout function has unit tests for insert, displace and swap.
- Gallery dashboard page composed of charts, gauges and stat tiles, editable and lockable.
