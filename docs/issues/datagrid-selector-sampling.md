# DataGrid: Auto type resolution invokes Field selectors - one throwing row kills the render

**Status: OPEN. Robustness bug found via a real crash (OrderingPlatform, 0.26.2).**

`ResolveColumnDataType` (Filtering.cs), when a column's type is `Auto`, samples the data:

```csharp
var sel = ResolveSelector(key);
object? sample = null;
if (sel is not null && Items is not null)
    foreach (var i in Items) { sample = sel(i); if (sample is not null) break; }
```

The column's `Field` lambda is therefore EXECUTED during a normal render (header class
resolution calls `AlignClass` -> `ResolveColumnDataType`). A selector that dereferences a
nullable - the natural way to bind "when was this row last touched" onto a model with an
optional parent (`p => p.Row!.UpdatedDate` where `Row` is null for unset values) throws
`NullReferenceException` and takes down the whole render batch.

What makes it worse than a usual app bug:

- The failure is data-dependent: the loop stops at the first non-null sample, so the grid
  works until a null-parent row appears before the first populated one - then the page dies
  with no hint of which column is responsible.
- Nothing in the docs says `Field` must be null-safe for EVERY row; the null-forgiving
  operator (`!`) compiles happily and reads as an honest binding.

## Ask

Any of: catch exceptions in the sampling loop and treat the sample as null (with a one-line
console warn); fall back to the expression's static return type instead of runtime sampling
when available; or document loudly that `Field` selectors run for every row at render time
and must be null-safe.
