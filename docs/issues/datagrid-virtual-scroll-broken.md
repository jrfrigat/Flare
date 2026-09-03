# DataGrid: Virtual renders ~10 rows and never scrolls

**Status: OPEN. Hard bug. Found in a real app (OrderingPlatform) on Flare 0.26.2,
Blazor WASM, .NET 10, Chromium.**


**ROOT CAUSE FOUND 2026-09-03 - see [datagrid-virtual-paged-source.md](datagrid-virtual-paged-source.md).**
The "Suspected mechanism" below is WRONG: `Virtualize`, `border-collapse` and `SpacerElement="tr"` are all
innocent. The plain client virtual path is fed `Sorted()`, which is paged, so the grid is handed exactly
`PageSize` (default 10) rows and correctly reports that it has rendered all of them.

`FlareDataGrid` with `Virtual="true"` renders only the initial window of about 10 rows and
never expands it, regardless of `Height`:

- `Height="420px"` (the Gallery demo pattern) - about 10 rows, then nothing.
- `Height="100%"` inside a full flex chain - the same.

## Evidence

Both Virtualize spacers (the `aria-hidden` `tr` elements at the two ends of `tbody`) end up
with `style="height: 0px; flex-shrink: 0; display: table-row;"`. A zero after-spacer means
the component believes every item is already rendered: there is no scroll range, so nothing
scrolls and rows 11..N are unreachable. Sorting and filtering operate on the same rendered
window, so the grid effectively caps any dataset at ~10 rows.

Repro: any client-side `Items` grid with `Virtual`, 50+ rows. Open DevTools and look at the
last `tbody tr[aria-hidden]` - its inline height stays `0px` after load and after any
interaction.

## Suspected mechanism

The .NET 10 `Virtualize` derives its window from spacer measurements
(`data-blazor-virtualize-reserved-height`, the IntersectionObserver pair). Inside a
`border-collapse: collapse` table with `SpacerElement="tr"` those measurements appear to
degenerate, the capacity calc concludes "everything already fits", and the after-spacer
collapses to zero. The exact JS-side path is not confirmed; the DOM outcome is.

## Impact and workaround

`Virtual` is unusable in this setup. The app fell back to full rendering: `PageSize` large
enough to hold every row (the pager hides itself at one page) plus a small app-CSS scroll
rule for the plain wrapper - see `datagrid-scroll-without-pagination.md` for the feature
ask that this workaround motivates.
