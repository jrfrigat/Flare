# Select: the dropdown arrow, not the text, sets the trigger height

**Status: OPEN. 2px residual, measured 2026-09-03 after the 0.26.3 padding fix. Small.**

With `--flare-input-padding-md` now applied to the combobox trigger (0.26.3), a default `FlareSelect`
still stands 2px taller than the `FlareField` beside it. The padding is no longer the cause.

Measured at 1400x950, MD3 Expressive, on `/components/barcode`:

| | content | padding | border | total |
| :-- | --: | --: | --: | --: |
| `.flare-input__control` (text field) | 20 | 32 | 0 (+2 on the well) | 54 |
| `.flare-select__control` | **22** | 32 | 2 | 56 |

The trigger is a flex row holding `.flare-select__value` (20px, the body-large line box) and
`.flare-input__arrow` (**22px**). The arrow is the taller item, so it - not the text - sets the flex
line, and the whole control inherits its 2px.

## Ask

Make the arrow stop contributing height: give it the same line box as the value it sits beside, or size
it from a token that resolves to the value's line-height rather than to an icon font's own metrics. The
same check applies to every other icon that shares a row with text inside a field well (the clear button,
the leading/trailing icons, the numeric stepper) - the audit's field-family geometry test would catch all
of them at once, and there is no such test today.

Related: `docs/issues/flare-audit.md` §7 (the two causes fixed in 0.26.3) and the field-size guard
proposed in its §8.
