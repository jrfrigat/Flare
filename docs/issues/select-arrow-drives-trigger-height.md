# Field family: whatever icon a well holds sets its height - there is no height token

**Status: FIXED in 0.27.0. The `--flare-input-height-{xs..xl}` ramp exists, the well takes it as a
definite height, and every control in the family measures the same at every size in all seven themes.
Re-scoped 2026-09-03 after a per-icon fix was tried and REVERTED (it made the gap worse); the fix that
landed is the one this document asked for.**

With `--flare-input-padding-md` applied to the combobox trigger (0.26.3), a default `FlareSelect` still
stands 2px taller than the `FlareField` beside it. Measured at 1400x950, MD3 Expressive,
`/components/barcode`:

| | content | padding | border | total |
| :-- | --: | --: | --: | --: |
| `.flare-input__control` (text field) | 20 | 32 | 0 (+2 on the well) | **54** |
| `.flare-select__control` | 22 | 32 | 2 | **56** |

The trigger is a flex row holding `.flare-select__value` (20px, the body-large line box) and
`.flare-input__arrow`. The arrow is taller, so it - not the text - sets the flex line.

## Why "size the arrow properly" does not work

`.flare-input__arrow` sets no `--_flare-icon-size`, so its glyph falls back to the theme's
`--flare-typescale-title-large-size`, which is a TYPOGRAPHY token with no business sizing an icon (see
the contract written into `icon.css`: every host that holds an icon sets `--_flare-icon-size`). That
part is a genuine defect. But giving it the field's own icon token does not fix the height, it
worsens it - the values are:

| token | value |
| :-- | --: |
| body-large text line box | 20px |
| `--flare-typescale-title-large-size` (today's accidental fallback) | 22px |
| `--flare-input-icon-size` (what the leading/trailing field icons use) | **24px** |

Applied, the trigger went 56 -> 58 and the gap 2px -> 4px. Reverted. **Every icon size the family
offers is larger than the text line**, so no per-icon value makes a well holding an icon match a well
holding only text. The clear button, the picker toggle and the numeric stepper all sit in the same row
and have the same effect wherever they appear.

## What the whole family actually measured

The 2px above was the visible corner of it. Every field the library ships, at every size, in both
reference themes, measured in the browser before the fix:

| | xs | sm | md | lg | xl |
| :-- | --: | --: | --: | --: | --: |
| **Material** text field | 28 | 34 | 54 | 62 | 77 |
| **Material** select / multiselect | 30 | 36 | 56 | 64 | 72 |
| **Fluent** text field | 28 | 34 | 46 | 50 | 58 |
| **Fluent** select / multiselect | 24 | 30 | 42 | 46 | 54 |

Three findings the original report did not have:

1. **The gap is at every size, not just Md** - 2px under Material, 4px under Fluent.
2. **Xl inverts the sign.** Under Material the text field is 5px TALLER at Xl, because the text line
   grows with the type step and the glyph does not. So "the trigger is the tall one" was never the
   rule; "whatever is in the well is the rule" was.
3. **The same control has two heights.** A Fluent `FlareSelect` measures 42 without a leading icon and
   46 with one. Nothing about the caller's data should change a control's height.

And two more, found by sweeping the rest of the family rather than the two controls in the report:

4. `FlareTagField`'s well named a `min-height: 2.75rem` literal of its own: 44px against the family's
   54/56, and **inverted between its own Sm (50) and Md (44)**.
5. `.flare-autocomplete__icon` draws the SAME `expand_more` glyph as the select arrow at a `0.75rem`
   literal - 12px against the arrow's 22px.

## The fix that landed

- `--flare-input-height-{xs..xl}`, five `required` properties on `InputTokens`, beside the existing
  padding ramp.
- `.flare-input__field` takes the step as a **definite** `block-size`, not a `min-block-size`. That is
  the part that makes the guarantee structural rather than arithmetic: a definite height is not measured
  from its content, so no icon, button, stepper or type step can move it, and no theme has to name a
  number "big enough" for whatever might end up in the well. Padding places the content inside that
  height instead of defining it.
- The two wells whose height legitimately IS their content - `FlareTextArea` (rows) and `FlareTagField`
  (rows of chips) - take the same step as a floor, marked by `flare-input__field--grow`.
- The arrow, the picker toggle, the clear button and the combobox chevron all state
  `--_flare-icon-size: var(--flare-input-icon-size)`. The arrow's declaration sits AFTER its
  `font: inherit` reset, which would otherwise wipe it.
- Theme values are measured off each theme's own padding ramp, taking the tallest control at each step,
  so nothing shrinks. Material lands on 56px at Md - the M3 field height, which the TEXT FIELD had been
  missing by 2px, not the select.

Verified in the browser at every size across `FlareField`, `FlarePasswordField`, `FlareNumericField`,
`FlareMaskedField`, `FlareTextArea`, `FlareSelect`, `FlareMultiSelect`, `FlareCombobox`, `FlareTagField`,
`FlareDatePicker`, `FlareTimePicker`, `FlareDateTimePicker`, `FlareDateRangePicker`, in all seven themes:
one height per size, everywhere.

## Guards

This is the geometry guard the audit asks for in `flare-audit.md` §8. No such test existed, which is why
a 6px and a 4px mismatch both shipped. Equal heights are now structural rather than asserted - one
definite height drives every control - so the tests guard the ways that could come undone:

- `FieldHeightRampTests` (core): the ramp grows Xs..Xl; every step has room for its own padding; every
  step is an absolute length; **no stylesheet outside the shared rule sizes a well**. The last one was
  exercised against the removed `2.75rem` literal - it fails with the file and the value named.
- `FieldGeometryContractTests` (components): every field renders exactly one shared well, and only the
  two grow wells carry the grow marker.

## Left open

`FlareTimeSpanPicker` renders its segments straight into the chrome's `Field` slot with **no well at
all** - no `.flare-input__field`, so no border, no background, no padding and no height. It is the one
member of the family that does not look like a field, it has no Gallery page, and it is not covered by
the guards above (they enumerate the wells-bearing controls). Giving it the shared well is a visual
change to the component, not a geometry fix, so it is left for its own issue.
