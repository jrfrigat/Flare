using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>
/// Draws a data set as SVG: line, area, bar, stacked bar, combo, scatter, bubble, pie, donut, radar,
/// rose, polar area or heat map. The drawing is written by the component itself - there is no charting
/// library behind it and no JS on the render path - so it prerenders, themes from tokens like every
/// other component, and costs one render tree rather than a canvas handoff.
/// <para>
/// Sizing has two modes: <c>Fluid</c> (the default) measures the plot and fills it exactly, so the
/// chart is <c>Height</c> px tall at any width; <c>Fluid="false"</c> fixes the <c>Width</c> x
/// <c>Height</c> aspect ratio and scales the drawing into its container with no interop at all.
/// </para>
/// </summary>
// Parameters, shared state and the geometry constants every renderer reads.
public partial class FlareChart
{
    /// <summary>Chart rendering type: Line, Bar, Pie, or Donut.</summary>
    [Parameter] public ChartType Type { get; set; } = ChartType.Line;
    /// <summary>Data series and labels to visualize in the chart.</summary>
    [Parameter] public ChartData? Data { get; set; }
    /// <summary>Optional title displayed above the chart and used as the SVG accessible name.</summary>
    [Parameter] public string? Title { get; set; }
    /// <summary>Optional description for the SVG &lt;desc&gt; element, improving screen-reader context.</summary>
    [Parameter] public string? Description { get; set; }
    /// <summary>Plot height in pixels. In fluid mode this is the rendered height exactly; otherwise it is
    /// the height half of the authored aspect ratio.</summary>
    [Parameter] public int Height { get; set; } = 220;

    /// <summary>
    /// Authored drawing width, in viewBox units. With <see cref="Fluid"/> off this and
    /// <see cref="Height"/> fix the chart's ASPECT RATIO: the drawing then scales like an image into
    /// whatever width its container gives it, so a 1200 x 220 chart stays wide and short at any size.
    /// In fluid mode it is only the width used until the first measurement arrives.
    /// </summary>
    [Parameter] public int Width { get; set; } = 400;

    /// <summary>
    /// Makes the chart fill its container instead of scaling into it. The plot is measured and the
    /// viewBox width is set to match, so one viewBox unit is one CSS pixel: the chart is exactly
    /// <see cref="Height"/> px tall at any width, and nothing - text, markers, stroke widths - is
    /// stretched. Default true.
    /// <para>
    /// Measuring needs the browser, so before the first measurement (prerender, static SSR, JS
    /// unavailable) the chart falls back to the authored <see cref="Width"/> x <see cref="Height"/>
    /// aspect ratio and renders exactly as <c>Fluid="false"</c> would. Set it to false to keep a chart
    /// completely free of JS interop, or when a fixed aspect ratio is the point.
    /// </para>
    /// Ignored in <see cref="Sparkline"/> mode, which already fills its box.
    /// </summary>
    [Parameter] public bool Fluid { get; set; } = true;

    /// <summary>Fills the area under each line series with a soft fade from the series color to transparent
    /// (line charts only). A series can override this through <see cref="ChartSeries.Area"/>.</summary>
    [Parameter] public bool Area { get; set; }
    /// <summary>Draws line series as smooth curves instead of straight segments. A series can override this
    /// through <see cref="ChartSeries.Smooth"/>, so one chart can mix straight and smoothed lines.</summary>
    [Parameter] public bool Smooth { get; set; }
    /// <summary>Stroke pattern of line series. A series can override it through
    /// <see cref="ChartSeries.LineStyle"/>.</summary>
    [Parameter] public ChartLineStyle LineStyle { get; set; } = ChartLineStyle.Solid;
    /// <summary>Draws a marker dot at each line data point. A series can override this through
    /// <see cref="ChartSeries.ShowMarkers"/>.</summary>
    [Parameter] public bool ShowMarkers { get; set; }
    /// <summary>Shows the horizontal grid lines behind an axis chart. Default true.</summary>
    [Parameter] public bool ShowGrid { get; set; } = true;
    /// <summary>Shows the numeric Y-axis labels. Default true.</summary>
    [Parameter] public bool ShowYAxisLabels { get; set; } = true;
    /// <summary>Shows the category X-axis labels. Default true.</summary>
    [Parameter] public bool ShowXAxisLabels { get; set; } = true;
    /// <summary>Shows the series legend. Default true.</summary>
    [Parameter] public bool ShowLegend { get; set; } = true;
    /// <summary>Where the legend sits relative to the plot.</summary>
    [Parameter] public ChartLegendPosition LegendPosition { get; set; } = ChartLegendPosition.Bottom;
    /// <summary>Overrides the plot padding (all sides, in viewBox units). Null keeps the default axis padding.</summary>
    [Parameter] public int? Padding { get; set; }
    /// <summary>Sparkline preset: a compact, chromeless line (no grid, axes, legend or title) that stretches
    /// edge-to-edge with a crisp stroke - for inline metric cards. Combine with <see cref="Area"/> for a fill.</summary>
    [Parameter] public bool Sparkline { get; set; }

    /// <summary>Overrides the automatic minimum of the value axis.</summary>
    [Parameter] public double? YMin { get; set; }
    /// <summary>Overrides the automatic maximum of the value axis.</summary>
    [Parameter] public double? YMax { get; set; }
    /// <summary>.NET numeric format for the Y-axis labels (e.g. "N0", "C0", "P0"); null uses a general format.</summary>
    [Parameter] public string? YAxisFormat { get; set; }
    /// <summary>
    /// Number of horizontal grid lines on the value axis, counting the top and bottom line - so 5 draws
    /// the four bands a spreadsheet shows by default. Null (the default) derives the count from the plot
    /// size, so a short chart draws fewer lines than a tall one instead of both drawing five. Clamped to 2..24.
    /// With <see cref="NiceScale"/> on, round numbers win over an exact count and the drawn total can
    /// differ by one; turn NiceScale off to get exactly this many.
    /// </summary>
    [Parameter] public int? YAxisTickCount { get; set; }
    /// <summary>
    /// Rounds the value axis outward to whole steps of the 1/2/2.5/5/10 progression, so the labels read
    /// 0, 100, 200 rather than 0, 94, 188, and the data no longer touches the top edge of the plot.
    /// Default true. An axis pinned by both <see cref="YMin"/> and <see cref="YMax"/> is taken literally
    /// and never rounded; pinning one end rounds only the other.
    /// </summary>
    [Parameter] public bool NiceScale { get; set; } = true;
    /// <summary>Number of lighter divisions drawn between two major grid lines. 0 (the default) draws none.
    /// Minor lines carry their own color and width tokens, so a theme can make them near-invisible. Clamped to 0..10.</summary>
    [Parameter] public int YAxisMinorTicks { get; set; }
    /// <summary>Draws a grid line down the plot at each labelled category, the layout a spreadsheet uses.
    /// The lines share the X projection and the label budget of the axis labels, so they stay under their
    /// own label and track a zoom.</summary>
    [Parameter] public bool ShowVerticalGrid { get; set; }
    /// <summary>Title drawn under the X axis.</summary>
    [Parameter] public string? XAxisTitle { get; set; }
    /// <summary>Title drawn rotated beside the Y axis.</summary>
    [Parameter] public string? YAxisTitle { get; set; }
    /// <summary>Renders bar / stacked-bar charts horizontally (categories down the Y axis).</summary>
    [Parameter] public bool Horizontal { get; set; }
    /// <summary>Shows the numeric value on each bar and the percentage on each pie/donut slice.</summary>
    [Parameter] public bool ShowValues { get; set; }
    /// <summary>Donut hole size as a fraction of the radius (0..1). Default 0.55.</summary>
    [Parameter] public double DonutRingRatio { get; set; } = 0.55;
    /// <summary>Bar width as a fraction of its slot (0..1). Default 0.85.</summary>
    [Parameter] public double BarWidthRatio { get; set; } = 0.85;
    /// <summary>Raised with the category (or slice) index when a data point is clicked.</summary>
    [Parameter] public EventCallback<int> OnPointClick { get; set; }
    /// <summary>Plays a CSS enter animation (bars grow, lines draw in). Honors <c>prefers-reduced-motion</c>.</summary>
    [Parameter] public bool Animate { get; set; }
    /// <summary>Also renders a visually-hidden data <c>&lt;table&gt;</c> after the chart, so screen readers
    /// can read the underlying values (an accessibility fallback for the SVG).</summary>
    [Parameter] public bool DataTable { get; set; }
    /// <summary>Overlays a linear-regression trend line on each line/area/scatter series.</summary>
    [Parameter] public bool TrendLine { get; set; }
    /// <summary>Threshold, band, segment, arrow and point overlays drawn over a cartesian chart. Build them
    /// with the <see cref="ChartAnnotation"/> factories.</summary>
    [Parameter] public IReadOnlyList<ChartAnnotation>? Annotations { get; set; }

    /// <summary>
    /// Enables zooming and panning of the horizontal axis: drag across the plot to zoom into that range,
    /// <c>Ctrl</c>+wheel to zoom around the pointer, drag to pan once zoomed, double-click to reset. The
    /// axis re-derives its ticks from the visible window, so zooming in shows more labels rather than the
    /// same labels stretched.
    /// </summary>
    [Parameter] public bool Zoomable { get; set; }

    /// <summary>Shows the zoom in / out / reset buttons above the plot. Default true when
    /// <see cref="Zoomable"/> is set.</summary>
    [Parameter] public bool ShowZoomToolbar { get; set; } = true;

    /// <summary>The visible horizontal window, in category index (or scatter X) units. Null shows the full
    /// domain. Supports two-way binding (<c>@bind-Zoom</c>) for a chart whose window the host owns.</summary>
    [Parameter] public ChartZoom? Zoom { get; set; }

    /// <summary>Fired with the new window whenever the user zooms or pans.</summary>
    [Parameter] public EventCallback<ChartZoom?> ZoomChanged { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Chart.Root;

    private bool _bubble => Type == ChartType.Bubble;

    // Series toggled off via the interactive legend.
    private readonly HashSet<int> _hidden = new();
    private bool IsHidden(int si) => _hidden.Contains(si);
    private void ToggleSeries(int si) { if (!_hidden.Remove(si)) _hidden.Add(si); }
    private bool _legendRow => _showLegend && LegendPosition is ChartLegendPosition.Left or ChartLegendPosition.Right;
    private Task HandlePointClick(int index) => OnPointClick.InvokeAsync(index);

    // Applies the YMin/YMax overrides to an auto-computed (min,max) pair.
    /// <summary>A resolved value axis: the window the data is projected into and how many grid lines span it.</summary>
    /// <param name="Min">Lower bound of the axis, after any rounding and any <see cref="YMin"/> override.</param>
    /// <param name="Max">Upper bound of the axis, after any rounding and any <see cref="YMax"/> override.</param>
    /// <param name="Lines">Number of major grid lines to draw, counting both ends.</param>
    private readonly record struct AxisScale(double Min, double Max, int Lines);

    // One grid line per _tickSlot units of plot, the same budget rule the X axis uses for its labels: the
    // count is then a consequence of the chart's size rather than a constant, so a 120px chart draws three
    // lines where a 600px one draws nine. A default 220px chart still lands on five - what it drew before.
    private const double _tickSlot = 44;

    private int TickLines(double extent) => YAxisTickCount is { } n
        ? Math.Clamp(n, 2, 24)
        : Math.Clamp((int)Math.Round(extent / _tickSlot) + 1, 3, 9);

    // The 1/2/2.5/5/10 progression every spreadsheet axis is built from, snapped to the NEAREST member
    // rather than up. Always rounding up coarsens the axis badly at the boundaries - a 0..470 range asking
    // for five lines lands on a step of 200 and an axis running to 600, when 100 and 0..500 is both rounder
    // and tighter. Rounding down costs at most one extra line, which the caller is told to expect.
    private static double NiceStep(double rough)
    {
        if (rough <= 0 || double.IsNaN(rough) || double.IsInfinity(rough)) return 1;
        double pow = Math.Pow(10, Math.Floor(Math.Log10(rough)));
        double f = rough / pow;
        return (f < 1.5 ? 1 : f < 2.25 ? 2 : f < 3.75 ? 2.5 : f < 7.5 ? 5 : 10) * pow;
    }

    // The next member of the progression above a step already on it.
    private static double NextNiceStep(double step)
    {
        double pow = Math.Pow(10, Math.Floor(Math.Log10(step)));
        double f = step / pow;
        return (f < 1.5 ? 2 : f < 2.25 ? 2.5 : f < 3.75 ? 5 : 10) * pow;
    }

    // Decimals the axis actually carries - enough to write its origin and its step without lying about
    // either, capped so a floating-point artefact cannot ask for twenty digits.
    private static int AxisDecimals(double origin, double step)
    {
        int d = 0;
        while (d < 6 && !(Exact(origin, d) && Exact(step, d))) d++;
        return d;

        static bool Exact(double v, int digits) =>
            Math.Abs(v - Math.Round(v, digits)) <= 1e-9 * Math.Max(1, Math.Abs(v));
    }

    // Bounds, rounding and line count are one calculation, not three: the nice step depends on how many
    // lines were asked for, and the count that is finally drawn depends on where the rounding landed. Split
    // them across the projection and the grid writer and the two drift out of alignment - which is the
    // defect that made a tick-count parameter useless on its own.
    private AxisScale ResolveAxis(double min, double max, double extent, bool honorMin = true)
    {
        bool pinLo = honorMin && YMin.HasValue;
        if (pinLo) min = YMin!.Value;
        if (YMax.HasValue) max = YMax.Value;
        if (max <= min) max = min + 1;
        int lines = TickLines(extent);
        // An axis pinned at both ends is the caller's window verbatim; rounding it would move a bound
        // they set on purpose.
        if (!NiceScale || (pinLo && YMax.HasValue)) return new AxisScale(min, max, lines);

        // Two effects push the count up: snapping the step DOWN to a rounder value, and then extending
        // both bounds outward to whole multiples of it. Together they can overshoot badly - a 94..470 range
        // asking for seven lines lands on a step of 50 and draws ten. So the step climbs the progression
        // until the overshoot is at most the one extra line the parameter documents.
        double step = NiceStep((max - min) / Math.Max(1, lines - 1));
        double lo, hi;
        int drawn;
        for (int guard = 0; ; guard++)
        {
            lo = pinLo ? min : Math.Floor(min / step) * step;
            hi = YMax.HasValue ? max : Math.Ceiling(max / step) * step;
            if (hi <= lo) hi = lo + step;
            drawn = (int)Math.Round((hi - lo) / step) + 1;
            if (drawn <= lines + 1 || guard >= 3) break;
            step = NextNiceStep(step);
        }
        return new AxisScale(lo, hi, Math.Clamp(drawn, 2, 40));
    }

    // The plot dimension the VALUE axis runs along - width on a horizontal bar chart, height everywhere
    // else - so the derived line count is budgeted against the axis it actually belongs to.
    private double _valueExtent => Horizontal && Type is ChartType.Bar ? _plotW : _plotH;

    private (double Min, double Max) ApplyBounds(double min, double max)
    {
        var axis = ResolveAxis(min, max, _valueExtent);
        return (axis.Min, axis.Max);
    }

    // SVG numeric attributes require '.' as the decimal separator; never use the current
    // culture (e.g. ru-RU formats 192.0 as "192,0", which is invalid in SVG).
    private static readonly System.Globalization.CultureInfo _inv = System.Globalization.CultureInfo.InvariantCulture;

    // Unique per-instance id prefix so area gradients from multiple charts on a page never collide.
    private readonly string _uid = $"fc{Guid.NewGuid():N}";

    // The drawing width in viewBox units. Fluid mode swaps the authored Width for the element's measured
    // CSS width, which is what makes one viewBox unit one pixel - so the SVG can be pinned to Height px
    // and still fill the container without stretching anything.
    private int _svgW => _fluidActive && _measuredWidth is { } w ? w : Math.Max(1, Width);
    private int _svgH => Height;

    // Both fluid and sparkline pin the CSS height, for different reasons: the sparkline distorts on
    // purpose (preserveAspectRatio="none"), fluid never distorts because the viewBox already matches the
    // box. Unmeasured fluid falls back to the authored aspect ratio (CSS height:auto), so a prerendered
    // or JS-less page draws a correct chart rather than a letterboxed one.
    private string? _svgStyle => _spark || (_fluidActive && _measuredWidth is not null) ? $"height:{Height}px" : null;

    private bool _spark => Sparkline;
    private bool _fluidActive => Fluid && !_spark;
    // Area fill is on for the explicit Area flag or the Area chart type.
    private bool _area => Area || Type == ChartType.Area;
    // Sparkline zeroes the chrome; otherwise a Padding override applies to all sides, else the axis defaults.
    private int _padL => _spark ? 2 : (Padding ?? 36);
    private int _padR => _spark ? 2 : (Padding ?? 12);
    private int _padT => _spark ? 2 : (Padding ?? 12);
    private int _padB => _spark ? 2 : (Padding ?? 28);
    private int _plotW => _svgW - _padL - _padR;
    private int _plotH => _svgH - _padT - _padB;

    // Chrome toggles - all forced off in sparkline mode.
    private bool _showGrid => !_spark && ShowGrid;
    private bool _showY => !_spark && ShowYAxisLabels;
    private bool _showX => !_spark && ShowXAxisLabels;
    private bool _showLegend => !_spark && ShowLegend && LegendPosition != ChartLegendPosition.None
        && Type != ChartType.HeatMap && Data?.Series is { Count: > 0 };

    // Tooltip state. Position is a percentage of the SVG box so it tracks the scaled
    // viewBox without needing the rendered pixel size.
    private bool _tooltipVisible;
    private double _hoverXPct;
    private double _hoverYPct;
    private string _tooltipText = string.Empty;

    private string F(double d) => d.ToString("F1", _inv);

    // Series color comes from the theme's CATEGORICAL palette, never from the semantic roles: reusing the
    // roles paints the fourth series in the error color and leaves a theme unable to widen the ramp
    // without repainting every button on the page. A series that carries its own meaning overrides it
    // with a FlareColor - a role (FlareColor.Error) or any CSS color, which converts implicitly.
    private string GetColor(int idx) =>
        (Data?.Series[idx].Color.CssValue) ?? Css.Tokens.Chart.SeriesVar(idx);

    // Per-series overrides of the chart-wide line switches. Null on the series means "follow the chart",
    // which is what lets one chart mix straight and smoothed lines, or dash one series and not another.
    private bool SeriesSmooth(ChartSeries s) => s.Smooth ?? Smooth;
    private bool SeriesArea(ChartSeries s) => s.Area ?? _area;
    private bool SeriesMarkers(ChartSeries s) => s.ShowMarkers ?? ShowMarkers;
    private ChartLineStyle SeriesLineStyle(ChartSeries s) => s.LineStyle ?? LineStyle;

    // Dash arrays are the theme's, so a style resolves to a token reference rather than a literal.
    private static string DashOf(ChartLineStyle style) => style switch
    {
        ChartLineStyle.Dashed => $"var({Css.Tokens.Chart.LineDashDashed})",
        ChartLineStyle.Dotted => $"var({Css.Tokens.Chart.LineDashDotted})",
        ChartLineStyle.DashDot => $"var({Css.Tokens.Chart.LineDashDashDot})",
        _ => "none",
    };

    // The stroke-dasharray fragment for a line style, empty for Solid so the common case writes nothing.
    private static string DashStyle(ChartLineStyle style) =>
        style == ChartLineStyle.Solid ? "" : $";stroke-dasharray:{DashOf(style)}";

    // pathLength rescales the unit that EVERY dash length on the path is measured in, which is what lets
    // the draw-on animation stroke a "1 long" dash across a path of any real length. That rescaling is not
    // opt-in per property: a themed dash array lands in the same units, so "6 4" would read as six whole
    // path lengths of ink - one solid line - and "0.1 4" as a tenth of the line and nothing after it.
    // A line that carries a real pattern therefore keeps its own units and forgoes the draw-on.
    private static string DrawLength(ChartLineStyle style) =>
        style == ChartLineStyle.Solid ? " pathLength=\"1\"" : "";

    // Geometry the SVG writer needs as NUMBERS - r and rx attributes take user units, not var() references,
    // so these are read from the theme in C#. Everything a CSS property can carry (fill, stroke, opacity,
    // stroke-width, font-size, dash) stays a var() inside a style attribute instead, which costs no parse
    // and lets the theme resolve it. Each renderer reads what it needs ONCE, not once per mark.
    //
    // On the fallbacks: SHAPE falls back to nothing (radius 0) - an unthemed component is meant to render
    // unstyled. Anything that ENCODES the data - point and bubble radius, the ramp - falls back to a
    // readable size instead, because a chart that hides its values is not unstyled, it is broken.
    private double BarRadius() => ReadTokenNum(Css.Tokens.Chart.BarRadius, 0);
    private double CellRadius() => ReadTokenNum(Css.Tokens.Chart.CellRadius, 0);
    private double CellGap() => ReadTokenNum(Css.Tokens.Chart.CellGap, 1);
    private double PointRadius() => ReadTokenNum(Css.Tokens.Chart.PointRadius, 2.5);
    private double BubbleMinRadius() => ReadTokenNum(Css.Tokens.Chart.BubbleMinRadius, 4);
    private double BubbleMaxRadius() => ReadTokenNum(Css.Tokens.Chart.BubbleMaxRadius, 24);
    private double RampMinOpacity() => ReadTokenNum(Css.Tokens.Chart.RampMinOpacity, 0.12);
    private double RampMaxOpacity() => ReadTokenNum(Css.Tokens.Chart.RampMaxOpacity, 1);

    // Style fragments shared by the renderers, so a grid line drawn by the cartesian, radar and polar
    // renderers cannot end up with three different treatments.
    private const string _gridStyle =
        "stroke:var(--flare-chart-grid-color);stroke-width:var(--flare-chart-grid-width);stroke-dasharray:var(--flare-chart-grid-dash)";
    // A minor line reuses the major dash so a theme that dashes its grid dashes all of it, and carries its
    // own color and width so it can be pushed as far back as the theme wants - or hidden outright.
    private const string _gridMinorStyle =
        "stroke:var(--flare-chart-grid-minor-color);stroke-width:var(--flare-chart-grid-minor-width);stroke-dasharray:var(--flare-chart-grid-dash)";
    private const string _labelStyle =
        "fill:var(--flare-chart-label-color);font-size:var(--flare-chart-label-size)";
    private const string _valueStyle =
        "fill:var(--flare-chart-value-color);font-size:var(--flare-chart-value-size)";
    private const string _valueOnFillStyle =
        "fill:var(--flare-chart-value-on-fill-color);font-size:var(--flare-chart-value-size)";
    private const string _axisTitleStyle =
        "fill:var(--flare-chart-axis-title-color);font-size:var(--flare-chart-axis-title-size)";
    private const string _sliceStrokeStyle =
        "stroke:var(--flare-chart-slice-stroke-color);stroke-width:var(--flare-chart-slice-stroke-width)";

    /// <summary>Clamps NaN/Infinity data values to 0 to prevent SVG coordinate corruption.</summary>
    private static double SafeValue(double v) =>
        double.IsNaN(v) || double.IsInfinity(v) ? 0.0 : v;

    private int _pts => Data?.Series is { Count: > 0 } s ? s.Max(x => x.Values.Count) : 0;
}
