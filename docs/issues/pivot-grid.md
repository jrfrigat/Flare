# FlarePivotGrid: cross-tabulation on the DataGrid engine

**Status: OPEN. Phase 2, large. First of the three heavy data widgets.**

Radzen has `RadzenPivotDataGrid`, Blazorise has `PivotGrid`; MudBlazor and Fluent UI have nothing. A
pivot is the one analytical surface a business application asks for that a plain grid cannot fake, and
Flare has an unusually strong starting position for it.

## What already exists and must be reused

This issue is mostly assembly, not invention:

- **Grouping and aggregation.** `FlareDataGrid.Grouping.cs` already groups by a selector and computes
  aggregates per level, with per-level aggregates overriding grid-level ones (`ComputeAggregate`,
  `DataGridGroup.Aggregates`). A pivot is that machinery applied on two axes instead of one.
- **Column model.** `FlareColumnBase` / `FlareColumn` / `FlareColumnBand` / `FlareColumnRow` already model
  multi-level headers. A pivot's column axis *is* a band tree - it is generated from data rather than
  declared, which is a source difference, not a rendering difference.
- **Export.** `IDataGridExporter` with the Excel, CSV and PDF writers works on a rendered table model.
  Target that same model and pivot export is free.
- **Filtering.** The Query package's condition model (`QueryDraft`, `QueryConditionDraft`) and
  `DataGridFilterBuilder` already express filter trees; the field chooser reuses them rather than growing
  a second filter dialect.
- **Persistence.** `DataGridPersistence` already round-trips grid state; pivot state is the same problem.

Anything in the list above that turns out not to be reusable as-is should be **generalised in place**, not
forked. A copy of the aggregation code inside a satellite package is a failure of this issue.

## Shape

Satellite package `Flare.Components.Pivot`, referencing `Flare.Components` - same arrangement as Kanban
and Query. Declarative axes, matching how the DataGrid reads:

```razor
<FlarePivotGrid Data="sales">
    <RowFields>
        <FlarePivotField For="x => x.Region" />
        <FlarePivotField For="x => x.City" />
    </RowFields>
    <ColumnFields>
        <FlarePivotField For="x => x.Date" GroupBy="PivotDateGrouping.Quarter" />
    </ColumnFields>
    <Values>
        <FlarePivotValue For="x => x.Amount" Aggregate="PivotAggregate.Sum" Format="C0" />
        <FlarePivotValue For="x => x.Id"     Aggregate="PivotAggregate.Count" />
    </Values>
</FlarePivotGrid>
```

Capabilities, in priority order:

1. Row and column axes with unlimited nesting; expand and collapse per node with the state persisted.
2. Aggregates: sum, count, distinct count, min, max, average, plus a custom delegate. Multiple value
   fields render as adjacent measure columns under each column node.
3. Subtotals and grand totals per axis, independently toggled, with position (before / after) themeable.
4. Date grouping - year / quarter / month / week / day - because a date column is the single most common
   pivot axis and re-projecting it by hand defeats the purpose.
5. A field chooser panel: drag a field between the filter / row / column / value lists. Reuse the Kanban
   drag model rather than a new one; the interaction is identical.
6. Drill-through: clicking a cell raises the source rows behind it, so the application can open them in a
   dialog with a plain DataGrid.
7. Virtualization on both axes. A pivot over real data is wide as well as tall - this is the perf
   requirement that decides whether the component is usable, and it is a first-class goal, not a later
   optimisation.
8. Export to Excel preserving the header tree, through the existing exporter.

## Perf constraints, stated up front

- Aggregate once into a cell dictionary keyed by (row path, column path, measure), not per render.
- Recompute incrementally on expand / collapse - never re-aggregate the whole set to open one node.
- No LINQ allocation in the cell-render path; the render loop reads the precomputed cube.
- `IQueryable` source support so aggregation can be pushed to the database when the caller supplies one -
  which is the case that makes a pivot usable over a million rows. Radzen and Blazorise both aggregate in
  memory; doing it server-side is where Flare can be strictly better rather than equal.

## Tokens

`PivotTokens.cs`, `required`, no literals - but reuse `DataGridTokens` for anything the grid already
defines (cell padding, border, stripe, hover, header background). New surface is only what a pivot has
that a grid does not: axis-header background and depth-indent step, subtotal and grand-total row/column
background and weight, measure-header treatment, expander glyph size and gap, field-chooser panel surface,
and the drag placeholder. Roadmap rule 3 - a pivot cell is a grid cell.

## Done when

- A three-level row axis by two-level column axis with two measures renders correctly, subtotals included,
  verified against a hand-computed fixture.
- Expanding a node does not re-aggregate the source; a test asserts the aggregate function call count.
- 100k rows aggregate and render under an interaction budget documented in the test, with virtualization on.
- `IQueryable` path pushes grouping to the provider; verified by an expression-tree assertion.
- Excel export reproduces the header tree and the totals.
- Field chooser drag works with keyboard alone as well as pointer.
- Every string localized; Gallery page with a realistic dataset and a theme switch.
