# FlareChart: sizing, per-series styling, colors, zoom and annotations

**Status: OPEN. Tier 1. Reported items 4, 5, 6, 8, 12, 13.**

Six reports against one component. They are one issue because four of them are the same underlying
mistake: **chart-wide parameters where the data model should carry the value.** The stated goal from the
reporter is "as close to Excel as possible in capability".

---

## 1. The chart cannot be made wide (item 4)

`FlareChart` draws into a fixed `viewBox="0 0 400 {Height}"` and the stylesheet says
`.flare-chart__svg { width: 100%; height: auto }`. That pins the **aspect ratio** at `400 : Height`. In a
1200px-wide container the chart renders 1200x660, not 1200x220 - the drawing scales like an image, so a
wide chart is also a tall one, and there is no parameter that changes it. Sparkline mode escapes it
(`_svgStyle` pins `height:{Height}px` and `preserveAspectRatio="none"`), which is why the reporter saw a
sparkline go narrow and a normal chart refuse to.

Distorting the whole drawing the way the sparkline does is not the fix: it stretches the text, the
markers and the stroke widths with it.

**Design - two parameters, and the default changes.**

- `Width` (int, viewBox units, default 400). Authored aspect ratio. With `Fluid="false"` this is the
  whole story and the component stays 100% JS-free: `Width="1200" Height="220"` gives a wide chart that
  scales proportionally into any container.
- `Fluid` (bool, default **true**). The chart measures its own plot element through the existing
  `IBrowserViewportService.ObserveElementAsync` port and sets the viewBox width to the measured CSS
  pixel width, with the SVG at `height:{Height}px`. One viewBox unit is then one device-independent
  pixel: the chart fills the box it is given, the height is exactly `Height`, and *nothing distorts* -
  drag a splitter and the chart reflows the way a spreadsheet chart does.

Before the first measurement - prerender, SSR, JS unavailable - the component renders exactly as it does
today (`Width` x `Height` viewBox, `height:auto`), so there is no letterboxing and no blank frame; the
first measurement is the only reflow. `Fluid` is ignored in sparkline mode, which already fills its box.

The measurement is throttled and only re-renders when the integer width actually changes, so a drag
costs one render per ~100ms, not one per pointer move.

## 2. `Smooth` is a chart parameter, not a series parameter (item 5)

> Smooth should be set per series, so I can draw broken lines and smoothed lines on one chart.

Correct, and the same is true of `Area`. Both move onto `ChartSeries` as nullable overrides that fall
back to the chart-level parameter:

```csharp
public sealed record ChartSeries(
    string Label,
    IReadOnlyList<double> Values,
    FlareColor Color = default,
    IReadOnlyList<ChartPoint>? Points = null,
    ChartSeriesKind? Kind = null,
    bool? Smooth = null,
    bool? Area = null,
    ChartLineStyle? LineStyle = null,
    bool? ShowMarkers = null);
```

The chart-level `Smooth` / `Area` / `ShowMarkers` keep working and become the default for series that do
not state their own. Nothing needs a migration.

## 3. Colors are `string`, not `FlareColor` (item 6)

> the color can be set through string. Why is FlareColor not used? it was made exactly for this.

Right - and `ChartAnnotation.Color` has the same defect. Both become `FlareColor`, which:

- accepts a role (`FlareColor.Error` for a threshold line, `FlareColor.Success` for a revenue series)
  and resolves it through the theme rather than freezing a literal;
- still accepts a raw CSS string, because `FlareColor` has an implicit `string` conversion - so existing
  positional call sites that pass `"#ff0000"` keep compiling;
- sanitizes the value, which the current `string` path does not.

`FlareColor` needs one addition for this: a `CssValue` accessor returning `var(--flare-color-{role})` for
a role and the sanitized literal for a custom color, because an SVG `stroke`/`fill` needs a color
expression and not a class name. Series that leave `Color` at `default` keep drawing from the
categorical palette (`Css.Tokens.Chart.SeriesVar(i)`), which is the correct behaviour and unchanged.

## 4. No line style (item 13)

New enum, per series with a chart-level default, and the dash patterns come from tokens so a theme can
retune them:

```csharp
public enum ChartLineStyle { Solid, Dashed, Dotted, DashDot }
```

Rendered as `stroke-dasharray: var(--flare-chart-line-dash-dashed)` etc. rather than a literal array, so
the values are the theme's. Applies to line, area, combo-line and the trend line. Gallery demo added.

## 5. No directional annotation (item 12)

`ChartAnnotationKind` today is `HorizontalLine | VerticalLine | HorizontalBand` - all axis-parallel, so a
trend or a callout cannot be drawn. Added:

- `Segment` - a free line from (X1,Y1) to (X2,Y2) in data coordinates;
- `Arrow` - the same with an arrowhead at the end point;
- `Point` - a marked callout at (X,Y) with a label;
- `VerticalBand` - the missing half of the band pair.

That forces the record open. `ChartAnnotation` becomes a record with named data coordinates
(`X`, `Y`, `X2`, `Y2`) instead of `Value`/`Value2`, plus `Color`, `Label`, `LabelPosition` and
`LineStyle`. The existing three kinds keep static factory methods (`ChartAnnotation.Threshold(...)`,
`.Marker(...)`, `.Band(...)`) so the common cases stay one line. The arrowhead is an SVG `<marker>`
scoped by the existing per-instance `_uid`, so two charts on a page cannot collide.

Note `TrendLine` already exists as a chart-wide linear regression overlay. It stays; it answers a
different question (fit the data) from `Arrow` (say something about the data).

## 6. No zoom (item 8)

> add the ability to enlarge the chart - a zoom / scale button etc. I want the charts to be as close to
> Excel as possible in capability.

**Design - domain windowing, not a CSS transform.** Zoom sets the visible X range (and optionally Y);
the chart re-projects into that range and redraws at full quality. A transform would blur the strokes and
scale the axis text, which is what a zoom must not do.

- `Zoomable` (bool) turns it on. State is `(XFrom, XTo)` over category index or scatter X.
- **Drag to select** a range on the plot: a pointer-drag on the existing hit layer paints a selection
  rectangle and, on release, zooms to it. Pointer events only - no JS.
- **Wheel to zoom** around the cursor, with `Ctrl` held (so the page still scrolls), and **drag to pan**
  once zoomed.
- **Toolbar** (`ShowZoomToolbar`, default true when `Zoomable`): zoom in, zoom out, reset. Built from
  `FlareButton` + `FlareIconView`, no new chrome primitives.
- `ZoomChanged` reports the window so a host can persist it, and `Zoom` is settable for a controlled
  chart.
- Axis ticks re-derive from the visible window, so zooming in shows *more* labels, not the same labels
  stretched - the behaviour that makes a spreadsheet chart useful.

Out of scope here and filed separately if wanted: brush/overview strip under the chart, and
zoom-synchronised multi-chart groups.

---

## Order of work

The six are independent but touch one file, so they land in one branch in this order: colors (3), then
per-series overrides (2) and line style (4), which together settle the `ChartSeries` shape; then sizing
(1); then annotations (5); then zoom (6), which depends on the sizing work because the hit layer and the
axis projection both change there.

`FlareChart.razor` is already 1063 lines and every one of these adds to it. It gets split along the way -
models, geometry/projection, the cartesian renderers, the radial renderers, and interaction - to stay
under the 500-line rule.
