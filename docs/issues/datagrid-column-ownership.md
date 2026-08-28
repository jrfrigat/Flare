# Column ownership: `FlareColumn` describes, `FlareDataGrid` draws

**Status: OPEN, PARTIALLY DISPUTED. Tier 2. Reported items 9 and 3.**

## The report

> In DataGrid I do not like that column management etc. happens inside the DataGrid itself. Properly it
> should be a central place, and other components should drive it. Right now when I declare a
> FlareColumn, the datagrid is responsible for drawing it, and I want FlareColumn itself to provide the
> render template etc.

## Where I disagree, and why

Taken literally - each `FlareColumn` renders its own cells - this is worse than what exists, for reasons
that are not stylistic:

- **Cost.** A grid of 20 columns x 200 rows would become 4000 component instances instead of 4000
  render-tree frames emitted by one component. Every one gets a parameter-diff pass, a lifecycle and a
  disposal. Flare's benchmark position depends on not doing that; MudBlazor's grid does not do it
  either.
- **Virtualization.** The rows are the virtualized unit. If the cell renderer is owned by a column
  component that lives outside the virtualized region, cells cannot be created and destroyed with their
  rows.
- **Column order, bands and composites.** The grid re-orders leaves, spans bands and lays composite
  fields into a CSS grid. That is a whole-table layout decision; a column cannot make it from inside.

The declarative-description model - `FlareColumn` is markup that registers a description, the grid draws
it - is the right one and is what every fast grid does.

## What the reporter actually needs, and which parts are real

Underneath the wording there are three complaints that *are* real:

### a. The description is not a first-class object

`FlareColumn<T>` is a component; there is no way to build a column in C#, keep it in a list, share it
between two grids, or generate columns from metadata. `GridColumn<T>` exists internally and is not
public.

**Fix.** Make the column model public and buildable: `FlareColumnDefinition<T>` (the current internal
`GridColumn<T>`, promoted and documented) plus a `ColumnDefinitions` parameter on `FlareDataGrid<T>`
that takes `IEnumerable<FlareColumnDefinition<T>>` as an alternative to the markup form. The markup form
becomes a thin builder over it - which it almost is already. That gives the reporter columns as data,
which is what "FlareColumn itself provides the render template" is reaching for.

### b. Column management chrome is welded into the grid

The picker, the export menu, the filter builder, the quick filter and the pager are all rendered by the
grid's own toolbar. There is no way to put the column picker in a page header, or the pager in a card
footer, or to drive two grids from one filter bar.

**Fix.** The state already lives in one place - `FlareDataGrid` - so what is missing is only a way to
reach it from outside. Cascade a `FlareDataGridState` handle (visible columns, order, widths, sort
stack, filters, page, selection) and rebuild `DataGridColumnPicker`, `FlareDataGridPager`,
`FlareDataGridQuickFilter`, `DataGridExport` and `DataGridFilterBuilder` as standalone components that
bind to it - by cascade when nested, or by an explicit `For="@grid"` when placed anywhere on the page.
The grid keeps rendering them by default so nothing breaks; they simply stop being the only option.

This is the reporter's "central place, driven by other components", and it is achievable without moving
a single cell render.

### c. No example of several columns under one heading (item 3)

> I could not work out how to put 3 columns under one header. Add an example to the gallery.

`FlareColumnBand` does exactly this and the Gallery has no demo showing the simple case - the two
existing band demos are a multi-level header and a banded virtual grid, both of which look complicated
enough to be a different feature. A demo titled for the actual question ("three columns under one
heading") goes in, plus the API-reference page cross-links `FlareColumnBand` from `FlareColumn`, which it
does not today.

That is a documentation defect, not a code one, and it is the cheapest item in this whole batch.
