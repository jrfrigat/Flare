# A local density control

**Status: CLOSED, WILL NOT BUILD. The capability already ships as `Size`; what was missing was a
working ramp behind it, and that is fixed. From the app user's review.**

The other two items this issue carried - where `data-testid` lands, and the arrow button inside
`role="combobox"` - shipped in 0.20.0 (`InputAttributes` on the field family, an `aria-hidden` arrow).
This entry is what remains of the third, kept for the measurements.

## The report

> MD3 Expressive is fairly spacious by default. For large recipe editors the interface gets long, and
> there are not many ways to make individual regions denser through component parameters.

The first half is the specification working as intended. The second half was wrong on the facts, and it
took two rounds of measurement to establish that - both are recorded below, because the first round
argued for a large refactor that the second round showed was pointing at the wrong tokens entirely.

## Round one: a spacing scope, built and reverted

A `FlareDensity` scope repointing the 13 `--flare-spacing-*` tokens for its subtree, via a
capture-and-rebuild element pair (a custom property cannot be defined in terms of itself). Implemented
end to end and measured on the case from the report - a four-field card with a header and an action row:

| Level | Card height |
| :-- | :-- |
| Comfortable | 516px |
| Compact (x0.75) | 497px |
| Dense (x0.5) | 478px |

**Seven percent at the tightest setting**, or about 9.5px per field. Reverted rather than shipped: a
control named "density" that moves nothing is a misleading parameter.

The write-up then blamed the token shape - 148 `--flare-*-padding/gap/height` tokens, shorthands that
`calc()` cannot scale - and proposed splitting every one into block/inline axes.

## Round two: both premises were wrong

**The refactor it asked for does not exist.** Of the 148:

- **113** are `-height` and `-gap` - single values, already scalable, nothing to split.
- **25** are already axis-split (`--flare-card-padding-top` and siblings, split since that write-up).
- **10** are genuine shorthands, and only four of those sit anywhere near a form.

**And it was aiming at the wrong tokens anyway.** The spacing scale is what sits *between* controls; a
form's height is inside them. Every field in the family already inherits `Size` (Xs..Xl) from
`FlareFieldBase`, and the size grid sets block padding - the thing the scope could not reach. Measured
control heights, MD3 Expressive:

| Size | Height | vs Md |
| :-- | --: | --: |
| Xs | 26px | -26px |
| Sm | 32px | -20px |
| **Md (default)** | **52px** | - |
| Lg | 60px | +8px |
| Xl | 75px | +23px |

`Size="FieldSize.Sm"` takes 20px off every field. **The parameter that already shipped is worth more
than twice the feature that was proposed**, and `Xs` is worth nearly three times it.

## What was actually broken

The measurement turned up a real defect behind the parameter. Four of the five sizes had their padding
written as literals in `input.css` (`0.1875rem`, `var(--flare-spacing-3)`, `0.875rem 1.125rem`,
`1.125rem`), while the fifth - Medium - came from `--flare-input-padding`. Core owned four steps of a
five-step ramp and the theme owned the middle one, so the ramp could not stay ordered:

- MD3 Medium is 16px of block padding (the 56dp M3 field); core's Large was 14px. **Large rendered a
  field 4px SHORTER than Medium**, at the same font size - the Large size did nothing but shrink.
- Under FluentUI2, whose Medium is 12px, the same literals happened to be ordered. The bug was
  theme-dependent, which is the failure mode the token mandate exists to prevent.

Fixed: five per-size padding tokens owned by the theme (`--flare-input-padding-xs/-sm/-lg/-xl` beside
the existing `--flare-input-padding`), no lengths left in the size grid, and `FieldSizeRampTests`
asserting the block half grows Xs->Xl in every theme. The MD3 ramp now reads 26/32/52/60/75.

## What is genuinely still missing

One thing, and it is small: `Size` has to be repeated on every field. There is no way to say "the
fields in this region are small" once. If that is ever built it is a cascade of `FieldSize` picked up
by `FlareFieldBase` when the parameter was not supplied explicitly - roughly thirty lines, driving the
grid that now works - and **not** a spacing multiplier. None of MudBlazor, Radzen, Blazorise or
FluentUI-Blazor offers a region-level density scope, so it would be a real differentiator.

It is deliberately not built here. Ambient sizing is invisible at the call site, and the repetition it
removes is repetition of an explicit, greppable parameter. That trade is worth making only on a real
request, not on a hypothetical one.
