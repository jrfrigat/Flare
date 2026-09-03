# DataGrid: Striped hides the selected-row background on even rows

**Status: OPEN. CSS specificity bug, datagrid.css, 0.26.2.**

On a `Striped` grid the selection highlight only ever shows on ODD rows. Even rows keep
their stripe when selected, so clicking them changes state but not appearance - to the
user, selection "does not work" on half the grid.

## Cause

The stripe rule carries a higher specificity than the selected rule and wins the cascade:

- stripe: `.flare-datagrid--striped tbody tr.flare-datagrid__row:nth-of-type(even)`
  -> specificity (0, 3, 2)
- selected: `.flare-datagrid__row--selected` -> (0, 2, 0)

The hover pair is handled correctly - `.flare-datagrid--hoverable ... :hover` is written
after the stripe with equal specificity and also comments why the row is the right element
to paint. The selected rule never got the same treatment.

## Fix direction

Either scope a striped variant of the selected rule (same pattern as hover), or lower the
stripe rule's specificity with `:where()` so both state layers can stay simple. The
selected-hover compound (`.flare-datagrid--hoverable ... __row--selected:hover`) needs the
same pass so a hovered selected row does not flip back to the stripe.
