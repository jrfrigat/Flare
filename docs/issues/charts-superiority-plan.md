# Charts: make FlareChart best-in-class (phased plan)

**Source:** cross-framework charting audit vs MudBlazor / Blazorise / Fluent UI Blazor (2026-07-11).
Goal (user): "Flare must be better than everything." Approved: the full phased plan 1-4.

**Landscape.** Fluent UI Blazor has **no** charts (Flare already ahead). MudBlazor: **11** native-SVG
types with deep `ChartOptions`. Blazorise: **8** Chart.js (canvas) types + a native-SVG package + 5 addon
plugins (Streaming, Annotation, DataLabels, Trendline, Zoom). Flare today: 4 types, minimal config.

**Strategy.** Keep and lean into Flare's moat - **native SVG, zero-JS, token-themed colors, a11y
(role/title/desc), SSR, no license row caps** - which Chart.js (Blazorise) structurally can't match and
MudBlazor only half-matches (weaker theming, no animations). Then close breadth + config, and leapfrog
where **nobody** is strong: first-class **sparkline**, **token-driven CSS/SVG animation** (Mud has none),
and **C#/SignalR streaming** with no JS plugin.

## Phase 1 - chrome control + area + sparkline - DONE (2026-07-11)
`Sparkline` preset (chromeless + `preserveAspectRatio=none` + `vector-effect=non-scaling-stroke`), `Area`
(per-series gradient fade fill), `Smooth` (Catmull-Rom curve), `ShowMarkers`, granular `ShowGrid` /
`ShowYAxisLabels` / `ShowXAxisLabels` / `ShowLegend`, `LegendPosition` (Top/Bottom/None), `Padding`
override. Line unified to a single `<path>` (straight/smooth/area share it). Closes
[flarechart-sparkline-mode.md](flarechart-sparkline-mode.md).

## Phase 2 - breadth of core types - DONE (2026-07-11)
Added native-SVG types: **`Area`** (first-class type = line + fill), **`StackedBar`** (positive stacking),
**`Scatter`** (new `ChartPoint`/`ChartSeries.Points` X-Y model), **`Radar`** (N-axis spider with grid rings
+ axis labels + filled polygons). `ChartType` extended; `_scale`/hit-zones adapted (Area+StackedBar get
category hit-zones). **Horizontal bar moved to Phase 3** (it flips the axis geometry, which lives with the
axis-config work). **TimeSeries** dropped as a dedicated type - a Line with time-string `Labels` covers it;
revisit only if DateTime-aware tick spacing is needed.

## Phase 3 - config depth + interactivity - DONE (2026-07-11)
- Axis config: `YMin`/`YMax` bounds, `YAxisFormat` (.NET numeric format), `XAxisTitle`/`YAxisTitle`.
- `LegendPosition` Left/Right (row `__body` layout) + **click-to-toggle series visibility** (interactive
  legend, hidden series excluded from scale/draw).
- **`Horizontal`** bar (categories down the Y axis, value axis on X).
- **`ShowValues`**: value labels on bars/stacked segments + percentage labels on pie/donut slices.
- **`OnPointClick`** (`EventCallback<int>` with the category/slice index) on the transparent hit zones.
- Pie/Donut: `DonutRingRatio`; Bar: `BarWidthRatio`.
- Deferred (low value): `YAxisTicks` count control, `XAxisLabelRotation`, `TooltipTemplate` (the text
  tooltip is enough for now), horizontal *stacked* bar, dual axis.

## Phase 4 - leapfrog / advanced - TODO
- **Token-driven CSS/SVG animation** (enter + data-update transitions) - Mud has none, and unlike Chart.js
  it's CSS/token-native (respects `prefers-reduced-motion`). A differentiator.
- **HeatMap**, **Bubble**, **Combo** (mixed bar+line via per-series type), **PolarArea** / **Rose**.
- **C#/SignalR streaming** (append/shift data + smooth transition, no JS plugin) - Blazorise needs
  chartjs-plugin-streaming; Flare can do it in managed code.
- Annotations / trend line (linear regression overlay).
- a11y: an opt-in visually-hidden `<table>` data fallback for screen readers.
- Responsive `MatchBoundsToSize` (fill container height, not just width), RTL, `CustomContent` SVG overlay.

## Not planned (deliberately)
- **Wrapping Chart.js** - would forfeit the zero-JS / token / a11y / SSR moat and add a ~200KB canvas
  dependency + (in Blazorise's case) license row caps. Flare stays native SVG.
- **Sankey** - niche; revisit only on demand.
