# MD3 Expressive: slider default (Medium) size is off the canonical 16/44 dp

**Source:** MD3-Expressive theme fidelity audit (`docs/audits/md3-expressive-theme-fidelity.md`, slider
section). Recorded as a deliberate deviation pending a call.

**Severity:** low. The slider is otherwise a faithful MD3 Expressive slider (thick track, active/inactive
gap with rounded inner+outer corners, morphing thin handle, stop dots, value bubble). Only the default
size is off.

## What the spec says

The MD3 Expressive slider has a single canonical geometry: **track 16dp / handle height 44dp / handle
width 4dp** (narrowing to 2dp when pressed).

## What Flare does today

Flare offers a size ramp; the **Medium (default)** size renders **track 40dp / handle 52dp**, while the
canonical **16/44/4** values live at the **XS** size. So the out-of-the-box slider is larger than the MD3
Expressive reference; you only get the spec proportions by explicitly choosing XS.

## Decision needed

- **Keep** the current ramp (Medium 40/52), treating XS as the spec baseline - document it.
- **Retune** so Medium == the 16/44dp canonical, and rebalance the other steps around it (this shifts the
  default look of every existing slider, so it is a visible change).

**UPDATE (0.4.0) - this is now a pure token change, and the old numbers were measured on a broken build.**

This file used to say "the geometry is driven by the core size classes in `slider.css` (the theme parks most
slider geometry at `initial`, deferring to those classes), so this is a core-CSS retune, not a token or
capability change". Both halves are obsolete:

- **It is a token change now.** 0.4.0 moved the ramp out of the core: the theme supplies
  `--flare-slider-track-height-xs..-xl` (and track radius / handle height / icon size). Retuning Medium is
  editing `MaterialDesignTokens.Slider` - no core CSS, no capability work.
- **Re-measure before deciding.** The 40/52dp figure predates the regression fixed in 0.4.0, during which the
  slider rail computed to **0px at every size** under MD3. Any number recorded while that was live is
  suspect; take fresh measurements first.

The decision itself (keep the ramp vs retune Medium to the canonical 16/44dp) is unchanged and still needs a
product call - it shifts the default look of every existing slider.
