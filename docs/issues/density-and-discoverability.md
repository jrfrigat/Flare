# A local density control

**Status: OPEN, BLOCKED ON A TOKEN REFACTOR. Tier 2. From the app user's review.**

The other two items this issue carried - where `data-testid` lands, and the arrow button inside
`role="combobox"` - shipped in 0.20.0 (`InputAttributes` on the field family, an `aria-hidden` arrow).
What remains is density, and an implementation attempt turned the vague half of the report into a
measured answer.

## The report

> MD3 Expressive is fairly spacious by default. For large recipe editors the interface gets long, and
> there are not many ways to make individual regions denser through component parameters.

Both halves are true and only the second is a defect. Expressive *is* spacious - that is the
specification, and a theme that quietly tightened it would be the wrong fix. What is missing is a way to
say "this region is dense" without switching themes or writing CSS.

## What was built, measured and reverted

The obvious design is a `FlareDensity` scope that repoints the spacing scale for its subtree: a
multiplier the theme owns (`--flare-density-factor-compact` / `-dense`), applied to the 13
`--flare-spacing-*` tokens by a two-element capture-and-rebuild pair (a custom property cannot be
defined in terms of itself), both `display: contents` so the scope adds no box.

That was implemented end to end and measured in the browser on the exact case from the report - a
four-field card with a header and an action row:

| Level | Card height |
| :-- | :-- |
| Comfortable | 516px |
| Compact (x0.75) | 497px |
| Dense (x0.5) | 478px |

**Seven percent at the tightest setting.** For a control called "density" that is not a feature, it is a
misleading parameter, so it was reverted rather than shipped.

## Why it fails, and what it would take

The measurement says exactly where a form's vertical budget lives, and it is not in the spacing scale:

- `.flare-card__content` padding stayed at `16px` at every level. Card padding is
  `--flare-card-padding`, not a spacing step.
- Field height is `--flare-input-padding`, likewise untouched.

The spacing scale is what components use *between* things; each component's own padding comes from its
own token. There are **148 distinct `--flare-*-padding/gap/height` tokens** in the component CSS, so
reaching them by enumeration means roughly 450 extra declarations in the bundle, plus a permanent trap:
every token added later silently escapes density.

And enumeration would not even work, because those tokens are shorthands:

```csharp
Padding = "0.875rem 1rem",   // InputTokens
Padding = "1rem 1rem",       // CardTokens
```

`calc()` cannot scale a two-value shorthand. There is no arithmetic route from here.

## The two real options

1. **Split the padding tokens.** Every `*-padding` shorthand becomes `*-padding-block` /
   `*-padding-inline` across both theme packages, and the density scope scales the block axis (the one
   that makes a form long) and leaves the inline axis alone. This is a cross-cutting token refactor with
   a guard test to keep new tokens split, and it makes density work for every component at once - the
   property the failed attempt was reaching for.

2. **Themes ship a dense token set.** Each theme authors a second set of values, and the scope emits it.
   Architecturally the cleanest under the token mandate (Flare knows no theme; themes own every length),
   and how MD3 itself defines density - but it doubles the authoring cost of a theme, and a third-party
   theme that skips it silently has no density at all.

Option 1 is preferred: one refactor inside Flare, no new obligation on theme authors, and the block/inline
split is worth having on its own for RTL and for vertical rhythm work.

Neither is a session's work, and neither should be started before the block/inline split is agreed - the
whole point of this write-up is that the cheap version was tried and measured and does not deliver.
