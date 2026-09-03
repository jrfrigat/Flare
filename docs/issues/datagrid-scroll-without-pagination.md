# DataGrid: first-class "all rows, own scroll" mode

**Status: OPEN. Feature request, motivated by a real app (OrderingPlatform, 0.26.2).**

Today a `FlareDataGrid` is either paginated (plain mode) or broken-by-bug virtual mode
(see `datagrid-virtual-scroll-broken.md`). There is no supported middle ground that many
data screens want: render every row, no pager, the grid scrolls inside its own box with a
sticky header. `Height` only affects the virtual wrapper; in plain mode the wrapper has
`overflow-x: auto` alone and the table grows the page.

## The workaround apps are forced into

1. `PageSize="100000"` so `_pageItems` holds the whole set (the pager hides itself at one
   page - that part works out of the box).
2. A custom class replicating the library's own `--virtual` recipe on the plain wrapper:

```css
.grid-fill > .flare-datagrid__wrapper {
    flex: 1 1 0;
    min-height: 0;
    overflow-y: auto;
}
```

3. `Class="grid-fill" Style="flex:1; min-height:0"` on the grid so the wrapper has a
   definite height to fill.

## What did not work: the sticky header

Copying the `--virtual` sticky rule
(`thead { position: sticky; top: 0; z-index: 3 }`) onto a plain `border-collapse: collapse`
table made clicks on the FIRST data row fire only intermittently - the header box floats a
few pixels over the row depending on scroll position and rounding, so the hit test misses.
We shipped without the sticky header; the header scrolls away with the body. If the library
owns this mode it can also own the fix (separate header table, `border-separate`, or th-level
sticky the way the virtual wrapper intends).

## Ask

A `ScrollMode`/`Virtual="Scroll"`-style switch (or a documented `FillHeight` + `Scroll`
pair) that renders all rows, hides the pager, gives the wrapper flex-based fill plus a
scrollbar, and provides a sticky header that does not eat clicks. For datasets of a few
hundred to a few thousand rows this is the sweet spot the virtual mode cannot currently
cover anyway.
