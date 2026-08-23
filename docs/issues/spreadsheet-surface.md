# Spreadsheet: finish the surface Flare already half-owns

**Status: OPEN. Phase 3, large. The one item on this roadmap that is not really about parity.**

Radzen ships `RadzenSpreadsheet`; nobody else does. Flare ships `FlareFormulaBar` and `FlareSheetTabs` in
`Flare.Components.IDE` - a formula bar and a sheet-tab strip **with nothing behind them**. That is the
odd situation: Flare has the chrome of a spreadsheet and not the grid.

The case for building it is not "Radzen has one". It is that Flare already owns the IDE/Office shell
nobody else has - Ribbon, Backstage, QuickAccessToolbar, DocumentTabs, StatusBar, ToolPanel, MenuBar -
and a spreadsheet grid is the missing document surface for it. This is the component that makes the whole
IDE package coherent rather than decorative.

## Scope

New satellite package `Flare.Components.Spreadsheet`, referencing `Flare.Components` and integrating with
`Flare.Components.IDE` rather than absorbing it.

### Cell grid

- Virtualized in both axes from the start; a spreadsheet is 10^4 by 10^2 minimum.
- Column and row headers with resize and freeze panes.
- Selection: cell, range, multi-range, whole row and column, with the fill handle.
- Editing in place with an editor overlay that agrees with the cell metrics - the same two-layer contract
  trap as `FlareCodeBlock`; reuse whatever primitive comes out of the markdown editor issue.
- Clipboard: copy, cut, paste of a rectangular range, including TSV interchange with Excel, through the
  existing `IFlareClipboard` port.
- Undo and redo as a command stack over the document model, not over the DOM.

### Formula engine

The part to decide deliberately, because it is the expensive half:

- A tokenizer and evaluator over cell references, ranges, operators and a function library. Start with the
  50 functions that cover real use - arithmetic, `SUM`/`AVERAGE`/`COUNT` family, `IF`/`IFS`, lookup,
  text, date.
- A dependency graph with topological recalculation and cycle detection. Recalculate only the dirty
  closure, never the sheet.
- **Flare already has a query engine with an expression model.** Before writing a parser, check whether
  the Query package's expression infrastructure generalises. If it does, the formula engine is much
  smaller than it looks; if it does not, say so in the commit and move on - do not force it.
- Errors are values (`#REF!`, `#DIV/0!`, `#VALUE!`), not exceptions.

### Formatting

Number formats, alignment, fonts, fills, borders, conditional formatting rules. All rendering through
tokens for the *chrome*; cell-level formatting is document data, not theme data - that distinction has to
be explicit or the token mandate and the document model will fight.

### Import and export

`.xlsx` read and write. Flare already writes `.xlsx` for DataGrid export (`ExcelGridExporter`); that
writer is the starting point, extended with formats and formulas. CSV both ways is trivial by comparison.

## Reuse

`FlareFormulaBar` and `FlareSheetTabs` (they get their engine at last), `FlareDataGrid`'s virtualization
approach, `ExcelGridExporter`, `IFlareClipboard`, `IFlareDownload`, `FlareContextMenu` via `FlareMenu`
right-click activation, and the IDE Ribbon for the command surface.

## Sequencing

Strictly staged, each stage shippable and useful on its own:

1. Grid, selection, in-place editing, undo/redo. No formulas. Already useful as a data-entry surface.
2. Formula engine over literals and references, with the dependency graph.
3. Function library, staged by frequency.
4. Formatting and conditional formatting.
5. `.xlsx` round-trip.
6. Charts embedded from `FlareChart` over a range.

## Done when

Per stage: virtualization holds at 100k cells, the recalculation touches only the dirty closure (asserted
by a counter in tests), the function library matches a fixture of published examples, `.xlsx` round-trips
without losing formulas, and the whole surface is operable from the keyboard - because a spreadsheet that
needs a mouse is a table.
