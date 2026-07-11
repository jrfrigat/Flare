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

## Phase 4 - leapfrog / advanced - DONE (2026-07-11, high-value items)
- **`Animate`**: token-driven, CSS-only enter animation (bars grow via `scaleY`, lines draw-on via
  `stroke-dasharray`/`pathLength=1`) that honors `prefers-reduced-motion`. **The differentiator** - Mud
  has no animation, and unlike Chart.js's JS animation this is CSS/token-native.
- **`ChartType.HeatMap`**: a colored grid (each series = a row, each label = a column; intensity via
  primary-color opacity), with optional `ShowValues` cell labels.
- **`DataTable`**: an opt-in visually-hidden `<table>` fallback so screen readers can read the underlying
  values - an a11y leapfrog none of the three offer.
- **Streaming** needs no dedicated API: FlareChart re-renders when `Data` changes, so a consumer updates
  `Data` on a timer/SignalR push and (with `Animate`) it transitions - vs Blazorise's chartjs-plugin-streaming.

## Phase 4b - deferred-set items 1-5 - DONE (2026-07-11)
The cheap-win + medium tier from the deferred triage (all native SVG):
- **`ChartType.Combo`** - per-series `ChartSeriesKind` (Bar/Line/Area) on one shared Y axis (Mud has no
  combo; Blazorise needs a per-dataset type override).
- **`TrendLine`** - least-squares regression overlay on line/area/scatter series (Mud has none).
- **`ChartType.Bubble`** - scatter sized by the new `ChartPoint.R` weight (sqrt-scaled pixel radius).
- **`ChartType.Rose`** (area-proportional sqrt sectors) + **`ChartType.PolarArea`** (linear wedges on a
  radial grid).
- **`Annotations`** - `ChartAnnotation` list: HorizontalLine (threshold/target), VerticalLine, HorizontalBand.
  Mud has no annotations; Blazorise needs chartjs-plugin-annotation.

Model additions (all back-compat, optional/defaulted): `ChartPoint.R`, `ChartSeries.Kind`,
`ChartSeriesKind`, `ChartAnnotation`/`ChartAnnotationKind`.

## Still deferred (low value / tradeoff; open for on-demand pickup)
- **`MatchBoundsToSize`** (fill container height) - the clean version wants a ResizeObserver (JS), which
  conflicts with the zero-JS moat; only worth it for fixed-height dashboard tiles.
- **RTL** - mirror the X axis / legend / text-anchors; mechanical, do when an RTL locale needs charts.
- **Horizontal *stacked* bar** - combine the horizontal + stacking code paths (~40 lines); low demand.
- **Windowed streaming** (auto-shift time window + smooth scroll) - basic streaming already works by
  updating `Data` on a timer/SignalR push with `Animate`; the fixed-window auto-shift is niche.
- **Sankey**, per-axis tick-count control, dual/secondary Y axis, `TooltipTemplate` - niche.

---

**Result:** FlareChart now spans **13 types** (Line, Area, Bar, StackedBar, Pie, Donut, Scatter, Bubble,
Radar, HeatMap, Rose, PolarArea, Combo) with axis config, interactivity, animation, trend lines,
annotations and an a11y fallback - all native SVG / zero-JS / token-themed, which no competitor matches on
all axes (Fluent has no charts; Chart.js/Blazorise is a canvas dep with license row caps; MudBlazor has a
couple more exotic types - Sankey - but no animation/trend/annotations and weaker theming/a11y).

## Not planned (deliberately)
- **Wrapping Chart.js** - would forfeit the zero-JS / token / a11y / SSR moat and add a ~200KB canvas
  dependency + (in Blazorise's case) license row caps. Flare stays native SVG.
- **Sankey** - niche; revisit only on demand.
