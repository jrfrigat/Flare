# FlareGauge: radial, arc and linear gauges on the existing chart renderer

**Status: OPEN. Phase 1. No longer blocked - chart tokenization shipped in 0.19.**

Radzen is the only one of the four reference frameworks with gauges - `RadzenRadialGauge`,
`RadzenArcGauge` and `RadzenLinearGauge`, each with scales, ranges and pointers. MudBlazor, Blazorise and
Fluent UI have nothing. Flare has `FlareProgress` (indeterminate and determinate bars), `FlareMeter` with
`FlareMeterSegment` (a segmented track with a legend) and `FlareTrack`, all of which are *bars*. None of
them answers "show me one value against a marked scale", which is the dashboard primitive.

This is the cheapest large-visibility win on the roadmap: the SVG renderer, the animation modifier, the
accessible-table pattern and the tooltip are all already in `FlareChart`.

## Scope

One component, `FlareGauge`, with a `GaugeShape` enum rather than three components - Radzen splits them
because its scales are separate components; Flare does not need that shape. The three renderings:

| Shape | Geometry |
| :-- | :-- |
| `Radial` | Full or near-full circle, needle or filled arc, ticks around the rim. |
| `Arc` | Half or quarter arc, the common KPI dial. Includes the "progress ring" degenerate case. |
| `Linear` | Horizontal or vertical bar with a scale, ticks and threshold markers. |

Child content declares the scale rather than parameters carrying six parallel arrays - this matches how
`FlareChart`, `FlareDataGrid` and `FlareMeter` already read:

```razor
<FlareGauge Shape="GaugeShape.Arc" Value="72" Min="0" Max="100">
    <FlareGaugeRange From="0"  To="50"  Color="..." />
    <FlareGaugeRange From="50" To="80"  Color="..." />
    <FlareGaugeRange From="80" To="100" Color="..." />
    <FlareGaugePointer Value="90" Kind="GaugePointerKind.Marker" />
</FlareGauge>
```

Surface: `Min`, `Max`, `Value`, `Shape`, `StartAngle` / `EndAngle` for radial and arc, `Thickness`,
`TickInterval` / `MinorTickInterval`, `Format`, `ShowValue`, `ValueTemplate`, `Label`, `Orientation` for
linear, plus `Animate`. Ranges and pointers are the two child components.

## Reuse, explicitly

- The SVG path and arc math from `FlareChart` (the pie and donut renderers already compute arcs); factor
  the arc helper out of the razor file into a shared internal geometry class rather than copying it. That
  refactor is part of this issue.
- `FlareMeter`'s segment model is conceptually the same thing as a gauge range. Converge the two on one
  record instead of shipping `MeterSegment` and `GaugeRange` side by side - roadmap rule 3, and the
  mandate's "unified tokens across similar components" clause applies to models too.
- The visually-hidden data table (`Css.Classes.Chart.Table`) is how `FlareChart` stays accessible. A gauge
  needs the same treatment plus `role="meter"` with `aria-valuenow` / `min` / `max` / `valuetext`.
- Motion tokens: reuse the chart enter-animation tokens; do not add a second easing scale.

## Tokens

`GaugeTokens.cs`, `required` throughout: track color and width, fill color and width, needle color /
width / length / pivot size, tick color / width / length for major and minor, label typescale and color,
value typescale and color, range opacity, threshold-marker shape and size, and the gap between arc and
ticks. Range *colors* come from the chart series palette introduced by the tokenization issue - a gauge
must not invent its own red/amber/green, since "red means bad" is a theme opinion.

## Done when

- All three shapes render from one component with no JS at all.
- Zero literals in the component: no `stroke-width="2"`, no degree constants beyond the geometric ones.
- A theme can turn the arc gauge into a Fluent-flat ring and into an MD3-Expressive thick rounded arc
  with token values only.
- Screen reader announces value and range; reduced-motion suppresses the sweep animation.
- Gallery page with all three shapes, a live-updating value, and a dashboard example combining gauge and
  sparkline.
