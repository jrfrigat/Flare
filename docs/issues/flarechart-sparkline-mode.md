# FlareChart: sparkline / chromeless line mode (+ area fill)

> **RESOLVED (2026-07-11, charts Phase 1).** `FlareChart` now has `Sparkline` (chromeless + edge-to-edge
> stretch + `non-scaling-stroke`), `Area` (gradient fade fill), `Smooth`, `ShowMarkers`, and granular
> `ShowGrid`/`ShowLegend`/`ShowXAxisLabels`/`ShowYAxisLabels`/`LegendPosition`/`Padding` toggles. The Weir
> dashboard can delete `ToSpark()` + both inline `<svg>` blocks and use
> `<FlareChart Type="ChartType.Line" Sparkline Area Height="120" Data="_throughput" />`. Broader charts
> roadmap: [charts-superiority-plan.md](charts-superiority-plan.md).

**Source:** the "Weir" admin dashboard (same app as
[weir-admin-flare-gaps.md](weir-admin-flare-gaps.md); this is a fifth gap found later, while wiring up
the live-metrics panel). Flare `0.1.4`, Command Center + Visual Studio themes. `FlareChart` inspected
on `main` (`src/Flare.Components/Chart/FlareChart.razor`).

**Severity:** low-to-medium. One place in the app, not a blocker, but it is the last remaining bespoke
SVG in the admin: the dashboard's two live "THROUGHPUT" and "LATENCY p50" mini-charts are still
hand-authored `<svg>` (a `<polyline>` plus a gradient `<polygon>` area), because `FlareChart` cannot
render as a compact sparkline. Everything else on the dashboard is Flare. This keeps the app from being
100% Flare (the project's stated rule) and is why the roadmap's "live FlareChart metrics" line had to be
corrected to "inline SVG sparklines".

---

## Need

A **sparkline**: a small (~100-120px tall), full-card-width line that shows the shape of a metric over a
short window, with an optional soft area fill under the line, and **no chart chrome** - no axes, no grid,
no value labels, no legend, no title. It sits inside a `FlareCard` whose header already shows the metric
name and current value, so the chart itself must be pure line + fill.

The data is already in the right shape - a single series of `double` values (a `Weir.Contracts.TimeSeries`
of `{ Timestamp, Value }` points), which maps directly onto `ChartData` with one series. **There is no
data-model gap; the gap is purely rendering/chrome.**

## What FlareChart offers today (and why it does not fit)

`FlareChart Type="Line"` always renders full-chart chrome, with no way to turn it off:

1. **Grid + Y value labels** - `RenderLine()` calls `GridLines(min, max)` unconditionally
   (`FlareChart.razor:246`), which draws 5 horizontal rules and 5 numeric Y labels.
2. **X-axis labels** - `AxisLabels(...)` is always emitted (`:265`); it is empty only if no `Labels` are
   supplied, but the Y grid/labels remain regardless.
3. **Legend** - a legend row renders whenever any series exists (`:59-71`), i.e. even for a single-series
   line. A one-line sparkline should have no legend.
4. **Axis padding is fixed** - `_padL=36, _padR=12, _padT=12, _padB=28` (`:94`) reserves space for the
   left Y labels and bottom X labels. There is no way to zero it, so the line cannot fill the box
   edge-to-edge the way a sparkline needs.
5. **No area fill** - `RenderLine()` draws only a stroked `<polyline>` (`:262-263`); there is no
   `Area`/`Fill` option to shade the region under the line (the Weir sparkline fills it with a gradient
   `<polygon>` that fades to transparent).
6. **Aspect / stretch** - `FlareChart` uses `viewBox="0 0 400 Height"` and default `preserveAspectRatio`.
   The Weir sparkline uses `preserveAspectRatio="none"` + `vector-effect="non-scaling-stroke"` so a thin
   strip stretches to the card width while the stroke stays 1.5px crisp. A sparkline mode would want the
   same (stretch horizontally, do not fatten the stroke).

Already fine, for reference (not gaps): `Height` is a parameter (default 220; I would pass ~120); `Title`
is opt-in (omit it); per-series `Color` exists (I set `--flare-color-primary` for throughput,
`--flare-color-success` for latency).

## Fallback used

Hand-written inline SVG in `Dashboard.razor` (~18 lines across two cards) plus a `ToSpark(TimeSeries)`
helper that projects the points onto a `0 0 100 30` viewBox and builds the polyline + area point strings.
Theme-correct (colors are `var(--flare-color-*)`), but it is bespoke SVG - exactly what the app is trying
to avoid.

## Proposed enhancement

A chromeless / sparkline mode on `FlareChart`. Either shape works; the preset is the ergonomic win:

- **Preset:** `Sparkline="true"` (or `ChartType.Sparkline`) that, for a line, suppresses grid, axis
  labels, legend and title, and zeroes the axis padding so the line fills the plot box.
- **Area fill:** `Area="true"` (or `Fill`) that shades under each line series from the series color down
  to transparent (a soft gradient like the Weir version, or a low-opacity solid). Useful outside
  sparklines too. (Related but not the same as the brand-gradient work in
  [flare-gradient-fill-proposal.md](flare-gradient-fill-proposal.md), which is about background/text
  fills, not chart area fills.)
- **Or granular toggles**, if you prefer composability over a preset: `ShowGrid`, `ShowAxisLabels`,
  `ShowLegend` (default true) plus a `Padding`/`Inset` override and the `Area` flag - with `Sparkline`
  simply flipping all four off. Granular is more flexible and would also let a normal chart drop just the
  legend.
- **Stretch:** in sparkline mode, render with `preserveAspectRatio="none"` and
  `vector-effect="non-scaling-stroke"` so the strip fills width without distorting the stroke.

## Acceptance

With the above I can delete `ToSpark()` and both inline `<svg>` blocks and write, per card:

```razor
<FlareChart Type="ChartType.Line" Sparkline Area Height="120"
            Data="_throughput" />   @* one series, Color = var(--flare-color-primary) *@
```

and get the same result as today: a full-width line with a soft fade-to-transparent fill beneath it, no
axes / grid / legend, tracking the theme. That removes the last bespoke SVG from the Weir admin and makes
the dashboard 100% Flare.
