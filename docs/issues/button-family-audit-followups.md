# Button-family audit - deferred follow-ups

**Source:** family-wide audit of Flare's button family vs the locally-installed MudBlazor / Blazorise /
Fluent UI Blazor (2026-07-11). Flare has the broadest family of the four (only one with a speed-dial;
native single-component SplitButton; full IconButton/Toggle/ToggleGroup/FAB set).

## Shipped on `main`
- **`FocusAsync()` across the family** - `FlareIconButton`, `FlareToggleButton`, `FlareSplitButton`
  (focuses the primary action), `FlareFloatingActionButton` (was only on `FlareButton`).
- **`ButtonEdge`** (`None`/`Start`/`End`) on `FlareIconButton` + `FlareToggleButton` - optical
  edge alignment (negative inline margin) for app bars/toolbars/list slots.
- **`FlareToggleButton.Toggle()` / `SetToggledAsync(bool)`** public methods.
- **`FlareToggleGroup.Mandatory`** (single-select cannot be cleared) + **`CheckMark`** (leading check on
  selected). Note: deselectable single-select ("toggle" mode) already existed as the default.
- **Consistency:** `FlareFloatingActionButton`/`FlareFloatingActionMenuItem` `OnClick` aligned to
  `EventCallback<MouseEventArgs>` (was the arg-less `EventCallback`); `FlareIconButton` gained `OnColor`
  and a `Rel` override (the `_blank` -> `noopener` default already flowed through composition).

## API-consistency check (siblings vs `FlareButton`)
- **`FlareIconButton`** composes `FlareButton`, so it inherits the state layer, shapes, custom color,
  loading spinner and the `rel`/`_blank` default. Intentionally omits label-only props (`TrailingIcon`,
  `LoadingText`/`LoadingTemplate`, `FullWidth`, `Typo`) - not meaningful for an icon-only button. OK.
- **`FlareSplitButton`** composes `FlareButton` for both the primary and the trigger, so `Variant`/`Size`/
  `Color`/`Disabled` are consistent. Missing (see G10): `Loading`, `FullWidth`, `Href` on the primary.
- **`FlareToggleButton`** is a distinct component (tonal-selected model). Aligned: `Size`/`Color`/
  `Disabled`/`AriaLabel`/`Edge`/`FocusAsync`. Divergent by design: no `Variant` (see G4).
- **`FlareFloatingActionButton`** is distinct. Aligned now on `OnClick`/`FocusAsync`. Missing (see G9):
  `Href`, `Loading`.

## Deferred

### G4 - `FlareToggleButton.Variant` + G3 `ToggledColor`/`ToggledVariant`/`ToggledSize` - LOW/MEDIUM
MudBlazor's `MudToggleIconButton` styles the toggled state independently (`ToggledColor`, `ToggledVariant`,
`ToggledSize`). Flare's toggle uses a fixed **tonal-selected** model where `Color` already *is* the
selected tint and `OnIcon`/`OffIcon` swap the glyph - so `ToggledColor` is largely redundant and a full
`Variant` axis would need a `togglebutton.css` rework (5 variant looks x pressed/unpressed). Revisit only
if a concrete design needs, e.g., outlined-when-off / filled-when-on toggles.

### G9 - `FlareFloatingActionButton`: `Href` (link FAB), `Loading`, trailing icon - LOW
MudFab has no link mode either; a link FAB + a loading FAB are occasionally useful. Additive.

### G10 - `FlareSplitButton`: `Loading`, `FullWidth`, `Href`, menu `Placement`, public `Open()`/`Close()` - DONE
Shipped. `Loading` (spinner on the primary), `FullWidth`, `Href`/`Target`/`Rel` (primary as a link, with the
`_blank` -> `noopener` default), `Placement` (`MenuAnchor`), and public `Open()`/`Close()`. Also gave
`FlareMenu` public `OpenAsync()`/`CloseAsync()` (the split button delegates to them).

### CloseButton - LOW (only Blazorise has it)
A dedicated dismiss "x" button that auto-closes its parent (Alert/Dialog/Snackbar/Drawer). Flare handles
close inside those components today; a reusable `FlareCloseButton` (icon button preset + optional
`AutoClose` of the nearest dismissible parent) would match Blazorise's `CloseButton`. Evaluate demand.
