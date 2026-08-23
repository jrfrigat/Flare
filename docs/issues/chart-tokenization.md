# Charts: give the chart engine a token record

**Status: DONE (2026-08-23). `ChartTokens` ships with 58 tokens; `FlareChart` and `chart.css` hold zero
semantic-role literals and zero geometry literals; MD and Fluent bases supply the full record and
MD3-Expressive overrides the shape half. Verified in the Gallery across two themes.** The record of what
was wrong and why the shape is what it is stays below, because the gauge family and the new series types
build directly on it.

What shipped, against the plan in this document:

- `Flare.Css.Tokens.Chart` (name registry) + `ChartTokens` (values, every property `required`), wired
  through `DesignTokens.Chart` and `CssVarMap.FlattenDesign`.
- The palette is 12 categorical colors built in each theme from its OWN hues (three source hues crossed
  with four tonal treatments) rather than fixed ink, so a dynamic palette and a light/dark switch both
  carry into the plot. **The error role is no longer in the ramp.**
- Colors, opacities, stroke widths, dashes and font sizes resolve as `var()` inside style attributes -
  no C# parse per mark. Only the values SVG takes as numbers (`r`, `rx`) are read in C#, once per
  renderer rather than once per mark.
- `ReadTokenNum` / `ReadTokenStr` and the per-theme flatten cache moved from `FlareProgress` to
  `FlareComponentBase`; both components now ask the theme the same way.
- The CSS audit was taught to resolve token CONSTANT references (`Css.Tokens.Chart.BarRadius`) and
  same-class accessor use. It previously only saw tokens spelled out as string literals, so going through
  the constants - the practice the conventions require - reported the whole surface as dead.

Two follow-ups this work did not do: the chart tooltip still styles itself from the shared color scales
rather than reusing `TooltipTokens` beyond max-width and offset, and `DonutRingRatio` / `BarWidthRatio`
remain parameters with numeric defaults rather than tokens.

`FlareChart` is the only substantial component in the library with **no token record**. There are 56
files in `Flare.Abstractions/Tokens/Components/`; `ChartTokens.cs` is not one of them. What a theme can
change about a chart today is therefore whatever the eight hardcoded color roles happen to resolve to.

## What is actually hardcoded

`src/Flare.Components/Chart/FlareChart.razor:214` - the series palette is a static array baked into the
component:

```csharp
private static readonly string[] _palette =
[
    "var(--flare-color-primary)",
    "var(--flare-color-secondary)",
    "var(--flare-color-tertiary)",
    "var(--flare-color-error)",
    "var(--flare-color-primary-container)",
    ...
];

private string GetColor(int idx) =>
    Data?.Series[idx].Color ?? _palette[idx % _palette.Length];
```

Three things are wrong with this under the token mandate, in increasing order of severity:

1. **The palette is a component decision, not a theme decision.** A theme cannot say "my categorical
   ramp is these seven hues"; it can only re-point `--flare-color-primary`, which also repaints every
   button in the application. Categorical data color is a *different* design axis from brand color, and
   every serious design system treats it that way.
2. **Series four is the error role.** A four-series chart draws its fourth series in the color the design
   system reserves for failure. That is a visual bug that only shows up on real data.
3. **The ramp for heat maps is a literal.** `FlareChart.razor:746` computes intensity as
   `0.12 + 0.88 * t` opacity over `--flare-color-primary`, with `rx="2"` on the cell. Both are theme
   opinions living in component code, which is exactly what the mandate forbids.

The class registry (`Flare.Abstractions/Css/Classes/Chart.cs`) is in good shape - 20 registered names,
BEM-correct. The gap is purely the token surface.

## What to build

A `ChartTokens` record in `Flare.Abstractions/Tokens/Components/ChartTokens.cs`, `required` throughout,
no literals, following the shape of `DataGridTokens`. The surface should be at least:

| Group | Tokens | Why |
| :-- | :-- | :-- |
| Series palette | `--flare-chart-series-1` .. `-12`, plus `--flare-chart-series-fallback` | The categorical ramp, owned by the theme. Twelve because that is where MD3 and Fluent both stop being distinguishable; wrap after that. |
| Series treatment | `--flare-chart-series-opacity`, `--flare-chart-area-opacity`, `--flare-chart-line-width`, `--flare-chart-point-radius`, `--flare-chart-bar-radius`, `--flare-chart-bar-gap` | Line weight and corner radius are shape decisions. MD3 Expressive and Fluent 2 disagree about all of them. |
| Sequential ramp | `--flare-chart-ramp-from`, `--flare-chart-ramp-to`, `--flare-chart-ramp-steps` | Replaces the hardcoded heat-map opacity math. Also what treemap and the future contour series need. |
| Axis and grid | `--flare-chart-axis-color`, `--flare-chart-axis-width`, `--flare-chart-grid-color`, `--flare-chart-grid-width`, `--flare-chart-grid-dash`, `--flare-chart-tick-length`, `--flare-chart-label-color`, `--flare-chart-label-typescale` | A grid line is not an outline-variant border; themes want it lighter and often dashed. |
| Plot surface | `--flare-chart-plot-background`, `--flare-chart-plot-radius`, `--flare-chart-band-color` | Alternating bands are a per-theme choice. |
| Legend | `--flare-chart-legend-gap`, `--flare-chart-legend-dot-size`, `--flare-chart-legend-dot-shape`, `--flare-chart-legend-typescale`, `--flare-chart-legend-off-opacity` | The dot is square in Fluent and round in MD3. |
| Tooltip | reuse `TooltipTokens` | Rule 3 of the roadmap: do not invent a second tooltip. |
| Emphasis | `--flare-chart-hover-brightness`, `--flare-chart-selected-outline`, `--flare-chart-dim-opacity` | Hover and cross-series dimming are currently not themeable at all. |
| Motion | `--flare-chart-enter-duration`, `--flare-chart-enter-easing` | `flare-chart--animate` exists; its timing is not exposed. |

## Notes on doing it in Flare's terms

- `GetColor` keeps the per-series `Color` override (it is the escape hatch and it is correct), but the
  fallback becomes `var(--flare-chart-series-N)` with N wrapping over the palette length token, not an
  array of role names.
- The three shipped themes (MD3, MD3-Expressive, FluentUI2) each need a palette. Do not derive it from
  the brand color in the component - derive it in the *theme*, where `IPaletteGenerator` already lives.
  MD3-Expressive should use its tonal ramp; FluentUI2 has a documented categorical set.
- `ThemeIndependenceTests` and the `cssaudit tokens` report must both pass. Note the known blind spots in
  that report - it will not catch a token that is registered but never referenced, so check the diff by
  eye as well.
- No new JS. This is a rendering-source change inside the existing SVG writer.

## Done when

- `ChartTokens.cs` exists with every property `required` and no theme-specific literal.
- No `var(--flare-color-*)` literal remains in `FlareChart.razor` for series, grid, axis or ramp.
- The heat-map ramp reads from the ramp tokens; `rx` reads from the bar-radius token.
- All three themes define the full record; an unthemed chart renders unstyled without throwing.
- Gallery chart pages show a theme-switch that visibly changes the categorical palette, not just the hue.
