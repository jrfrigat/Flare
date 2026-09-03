# Field family: whatever icon a well holds sets its height - there is no height token

**Status: OPEN. Re-scoped 2026-09-03 after a per-icon fix was tried and REVERTED (it made the gap
worse). The residual 2px left by the 0.26.3 padding fix has no per-component answer.**

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

## What the fix has to be

A height for the shared well, so the family's height is a family constant and what sits inside it
becomes irrelevant:

- a `--flare-input-height-{xs..xl}` ramp beside the existing `--flare-input-padding-*` ramp, applied as
  `min-height` on `.flare-input__field` and on the two combobox triggers;
- padding then centres the content inside that height instead of defining it;
- both shipped themes supply values (MD3 says 56dp for text field AND select, so today the text field
  is the one off-spec at 54, not the select at 56);
- and the arrow still gets an explicit `--_flare-icon-size`, because the typography fallback is wrong
  regardless of whether it drives height.

This is the geometry guard the audit asks for in `flare-audit.md` §8: assert that every structurally
distinct control in the family resolves to the SAME height at each size, including the default `Md`.
No such test exists, which is why a 6px and a 4px mismatch both shipped.
