# `Sortable` + `Frozen` on one column - reported runtime error

**Status: OPEN, NEEDS THE EXCEPTION TEXT. Tier 0. Reported item 2.**

## The report

> in datagrid if for a column you set both Sortable and Frozen, there is a runtime error

## What was tested

A bUnit render of `FlareDataGrid<T>` with a column carrying `Sortable`, `Frozen`, both, and both plus
`Resizable`, each followed by a header click that runs the full sort path. All four render and sort
without throwing. Reading the source agrees: `Sortable` and `Frozen` never meet - `_thClass` concatenates
two independent modifier classes, `GridColumn` copies the two flags into independent fields, and
`OnHeaderClick` does not read the frozen flags at all.

So the failure is not in the C# render path that bUnit exercises. What bUnit does *not* exercise is JS
interop, and that is the only thing `Frozen` adds:

```csharp
// FlareDataGrid.razor.cs:524
if (_columns.Any(c => c.Frozen || c.FrozenRight))
{
    try { await Grid.UpdateFrozenOffsetsAsync(_tableRef); }
    catch (InvalidOperationException) { }
    catch (JSDisconnectedException) { }
}
```

Two real defects are visible here regardless of whether they are *this* defect:

1. **`JSException` is not caught.** Every other failure mode of a JS call is, but the one that fires when
   the browser has an older `flare-components.js` than the assembly - the exact PWA/service-worker skew
   this repo has hit before - is not. A frozen column in an app with a stale cached script therefore
   throws into the renderer, and the circuit dies. Sorting makes it *more* likely to be seen, because the
   sort re-renders the grid and the frozen sync is re-attempted.
2. **The sync only runs when `_layoutSignature` changes.** Sorting does not change the signature, so the
   offsets are not recomputed after a sort - correct as an optimization, but it means the frozen offsets
   are stale if the sort changed a column's rendered width (autosized columns, longer values on top).

## What is needed to close this

The exception type and message, and the hosting model (WASM / Server / SSR-interactive). Without them
any change here is a guess dressed as a fix. In the meantime the two defects above are fixed on their own
merits:

- catch `JSException` alongside the other two at every DataGrid interop call site, and audit the rest of
  the library for interop calls that catch `InvalidOperationException`/`JSDisconnectedException` but not
  `JSException`;
- recompute frozen offsets when the *rendered* layout can have changed, not only when the declared
  layout signature changes.

Plus a regression test covering the flag combination, which did not exist - the Gallery has a frozen
demo and a sortable demo and no demo with both, which is why the combination shipped untested.
