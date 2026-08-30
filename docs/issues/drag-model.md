# Flare has no drag model - it has four of them

**Status: OPEN. Foundation. Blocks [TileLayout](tile-layout.md); overlaps the touch half of
[the mobile audit](mobile-responsive-audit.md).**

Raised as "the drag model is baked into Kanban". Measured, it is worse than that: **four components
implement drag-and-drop independently, and three of the four do not work on a touch screen at all.**

## What is actually there

| Surface | Mechanism | State it keeps | Works on touch |
| :-- | :-- | :-- | :-- |
| `FlareKanban` | HTML5 DnD | one `_dragging` string | **yes** - its own `ontouchstart`/`ontouchend` pair plus `getKanbanColumnAtPoint` |
| `FlareTreeItem` | HTML5 DnD | `TreeDragDropCoordinator`, tree-specific | **no** - zero touch handlers |
| `FlareDataGrid` rows | HTML5 DnD | one `_dragRow` field | **no** |
| `FlareDataGrid` columns | HTML5 DnD | one `_dragColumn` field | **no** |
| `FlareFileUploadZone` | HTML5 DnD | - | n/a: it receives files from outside the page |

Four implementations, three shapes of state, one of them a coordinator class that only a tree can use.
`FlareTileLayout` would be the fifth.

## The root cause, and it is not sloppiness

**Native HTML5 drag-and-drop does not fire on touch.** No `dragstart`, no `drop`, on any mobile browser.
Every one of these surfaces reached for HTML5 DnD because it is the obvious API, and then each one had
to decide separately what to do about phones. Kanban wrote a touch path. The other three did not, so
**reordering a data-grid column or a tree node is impossible on a phone today** - which is a mobile
defect, discovered while auditing the drag model rather than while auditing mobile.

That is also why the shared JS module already contains two hit-test helpers that belong to a model that
does not exist: `getDropZone(row, clientY)` for the tree's before/inside/after thirds, and
`getKanbanColumnAtPoint(x, y)` for the board. Two components each grew half a model.

## What Flare already has that must be reused

`flare-drag.js` exports `startDrag(handle, opts)` - a **pointer**-based gesture primitive with pointer
capture, touch-action handling and a teardown closure. It backs `FlareResizable`, `FlareSplitter`, the
dialog move and resize, and the colour-picker canvas, and it is the right foundation: pointer events
cover mouse, pen and touch in one code path, which is exactly the problem HTML5 DnD cannot solve.

So the gesture layer exists and is shared. What is missing is the **transfer layer above it**: who is
being dragged, where it may be dropped, and what happens when it lands.

## Scope

A drag model in the core, built on `startDrag`, with pointer events rather than HTML5 DnD. Three pieces:

### 1. `FlareDragContext` - the coordinator

A cascading coordinator in the shape `ZoneCollection` already uses for `FlareZone` / `FlareMeterSegment`:
sources and targets register with it, it owns the in-flight drag, and it pokes re-renders. Not a DI
service - a drag is scoped to the subtree it happens in, and a singleton would let two boards on one page
share one drag.

### 2. `<FlareDraggable>` and `<FlareDropZone>`

```razor
<FlareDragContext TPayload="Card" OnDrop="Moved">
    @foreach (var col in columns)
    {
        <FlareDropZone Target="col.Id">
            @foreach (var card in col.Cards)
            {
                <FlareDraggable Payload="card">@card.Title</FlareDraggable>
            }
        </FlareDropZone>
    }
</FlareDragContext>
```

- **`Group`** so a kanban card cannot land in a tree. Two independent boards on one page must not see
  each other's drags.
- **`Placement`** on a drop zone: `Into` (a column accepts a card) or `Between` (an ordered list, where
  the drop resolves to an index). `Between` is what the tree's before/inside/after thirds generalise to,
  so `getDropZone` becomes part of the model rather than a tree helper.
- **`DragPreview`** - the thing that follows the pointer. HTML5 DnD hands this to the browser and gives
  almost no control; a pointer implementation has to draw it, which is also what lets it be themed.

### 3. One hit-test in JS, not two

`getKanbanColumnAtPoint` generalises to `dropTargetAt(x, y)` returning the registered target id under the
pointer, and `getDropZone` folds in as the "which third" refinement for `Between`. Everything else -
which target is valid for this payload, what index the drop resolves to, what re-renders - is C#.

## Migration

`FlareKanban`, `FlareTreeItem` and both `FlareDataGrid` reorders move onto it, and each loses its own
drag state. That is the point: the value is not a fifth implementation available to `FlareTileLayout`,
it is four fewer.

`FlareFileUploadZone` stays on HTML5 DnD and must not be migrated - it receives files dragged in from
the operating system, which is the one thing HTML5 DnD does that pointer events cannot.

## Tokens

`DragTokens.cs`, `required`: the dragged item's opacity and elevation while in flight, the drop-zone
active background and outline, the insertion-line colour and width for `Between`, and the preview's
radius and shadow. Today Kanban, Tree and DataGrid each paint their own dragging state from their own
component tokens, which is why a dragged card and a dragged row do not look related.

## Done when

- One model backs Kanban, Tree, both DataGrid reorders and TileLayout, and no component keeps drag state
  of its own.
- **Every one of them works on a touch screen**, verified on a real 375px viewport, not inferred.
- Keyboard reorder exists: pick up with Space, move with the arrows, drop with Space, cancel with Escape.
  None of the four has this today, and a reorder that only a mouse can perform is not accessible.
- A theme can restyle the drag preview, the drop zone and the insertion line through tokens alone.
- Gallery page showing a board, a tree and a grid reordering through the same model.
