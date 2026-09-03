# DataGrid: the client virtual path is fed the PAGED list, so it caps at PageSize rows

**Status: OPEN. Confirmed root cause of [datagrid-virtual-scroll-broken.md](datagrid-virtual-scroll-broken.md),
which reported the symptom and guessed the mechanism wrong. One-line fix. Found 2026-09-03 by reading the
three `Virtualize` call sites in `FlareDataGrid.razor`; the reporting app (OrderingPlatform) hit it on 0.26.2.**

`FlareDataGrid` has three `Virtualize` call sites. Two are correct. The third is not:

| Line | Path | Items source | ItemSize |
| :-- | :-- | :-- | :-- |
| 214 | banded composite, client | `SortedUnpaged()` | `_recordItemSize` |
| 238 | `ItemsProvider` (server) | provider | `_effectiveVirtualItemSize` |
| 247 | **plain client `Items`** | **`Sorted()`** | **none** |

`Sorted()` runs `DataGridPipeline.Execute(..., _page, _effectivePageSize, ...)`, whose step 6 is
`Skip(page * pageSize).Take(pageSize)` (`Core/DataGridPipeline.cs`, lines 77-96). So the most common
configuration - client-side `Items` with `Virtual="true"` and no banded composite - hands `Virtualize`
exactly one page of rows. `PageSize` defaults to **10** (`FlareDataGrid.Parameters.cs`, line 27), which is
the "about 10 rows" every report of this bug describes.

The library already states the rule it is breaking, in the doc comment on the method the other two paths
use (`FlareDataGrid.State.cs`, line 342):

> Full sorted/filtered list with NO paging. Client-side virtualization renders its own window, so paging
> here would hide every record past the first page.

## Why the reported DOM evidence follows

The after-spacer measuring `height: 0px` is not a `Virtualize` measurement failure and has nothing to do
with `border-collapse` or `SpacerElement="tr"`: `Virtualize` is correct. It was given 10 items, it
rendered 10 items, so there is nothing left to reserve space for. Sorting and filtering appearing to
"operate on the rendered window" is the same cause - they run over the full set and then get paged back
down to ten.

The Gallery does not show it. Its two virtual demos are the provider path and the banded-composite path,
both of which take an unpaged source. Measured on 0.26.2 at `/components/datagrid`: the provider grid
reports an after-spacer of `238656px` over 28 rendered rows and scrolls correctly.

## Second defect at the same line

Line 247 also passes no `ItemSize`, so `VirtualItemSize` - a public, documented `[Parameter]` - is
silently ignored on this path, and `Virtualize` falls back to its own default of 50. The other two call
sites both pass one.

## Why 2351 tests pass

`DataGridTests.Virtual_WithoutItemSize_RendersWithoutThrowing` is the only test that sets `Virtual`. It
builds 100 rows and asserts `Assert.NotEmpty(cut.FindAll(".flare-datagrid"))` - that the component
rendered at all. It never counts rows. bUnit has no JS measurement, so `Virtualize` there renders every
item it is handed: a row-count assertion would have read 10 against an expected 100 and failed from the
day the bug landed.

## Fix

1. `Items="@Sorted()"` -> `Items="@SortedUnpaged()"` at `FlareDataGrid.razor` line 247.
2. Add `ItemSize="@_effectiveVirtualItemSize"` at the same line.
3. Test: client `Items` + `Virtual` with N rows and a default `PageSize` renders N rows under bUnit, for
   the plain, banded-composite and provider paths alike. Assert on the row count, not on non-emptiness.
4. Re-check `datagrid-scroll-without-pagination.md` afterwards: part of its motivation is that virtual
   mode "cannot cover" a few hundred to a few thousand rows. Once this is fixed, decide how much of that
   feature ask survives.
