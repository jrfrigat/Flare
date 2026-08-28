# Density controls, and APIs that need the source to understand

**Status: OPEN. Tier 2. From the app user's review.**

Three related observations, all of them about a caller having to know more than the API tells them.

## 1. MD3 Expressive is spacious, and there is no local density control

> MD3 Expressive is fairly spacious by default. For large recipe editors the interface gets long, and
> there are not many ways to make individual regions denser through component parameters.

Both halves are true and only the second is a defect. Expressive *is* spacious - that is the
specification, and a theme that quietly tightened it would be the wrong fix. What is missing is a way to
say "this region is dense" without switching themes or writing CSS.

`Size` exists on the field family and on buttons, but it is per-component and does not cover padding,
gaps, row heights or the type scale together. The shape that does:

- `FlareDensity` - a cascading scope component (`<FlareDensity Level="Density.Compact">`) that repoints
  the spacing and control-height tokens for its subtree. Three levels: `Comfortable` (the theme's own),
  `Compact`, `Dense`.
- a `Density` parameter on the containers where it matters most - `FlareDataGrid`, `FlareList`,
  `FlareForm`, `FlareCard` - which is the same scope applied to one component.

This is a token remap, not a per-component branch: the scope emits a handful of `--flare-spacing-*` and
`--flare-*-height` overrides on its own element and everything inside inherits them. It therefore costs
nothing at render time and works for every component at once, including ones that never heard of it.

## 2. `data-testid` and the nested input

> Some APIs are not obvious without reading the XML documentation and the actual DOM. For example, it is
> not always immediately clear where data-testid lands and how a component organises its nested input.

The field family renders a chrome wrapper around a real `<input>`, and `AdditionalAttributes` - which is
where an unmatched `data-testid` goes - splats onto the root, not the input. So a test that writes
`data-testid="email"` and then types into it is targeting a `div`. That is a reasonable design and a
terrible surprise.

Two changes:

- **Make it explicit.** `InputAttributes` on the field family: a dictionary splatted onto the inner
  `<input>`, so a caller can put `data-testid`, `autocomplete`, `inputmode` or `maxlength` exactly where
  they belong instead of guessing whether the splat reached them.
- **Document the DOM.** Each component page in the Gallery gets a short "rendered structure" block - the
  element skeleton with the class names and which element receives the splat. This is generated from the
  component, not hand-written, so it cannot drift.

## 3. `role="combobox"` with a nested button

> In the DOM FlareSelect uses role="combobox" with a separate nested arrow button. This is worth checking
> with VoiceOver and TalkBack on physical devices.

A fair flag rather than a bug report. A `role="combobox"` element containing a focusable `<button>` is
allowed by ARIA 1.2 but is read differently by every screen reader, and Flare's own accessibility tests
are static assertions over the markup - they cannot tell us how VoiceOver announces it.

Action: make the arrow non-focusable and `aria-hidden` (it duplicates the combobox's own activation, so
it is decorative), which removes the ambiguity without a device lab. Then verify the select, the date
picker and the autocomplete against NVDA and VoiceOver, and write down what was tested and when - the
current a11y test file implies more coverage than it has.
