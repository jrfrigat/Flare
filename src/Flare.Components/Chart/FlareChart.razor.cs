using Flare.Components.Resources;
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
    /// <summary>Data series and labels to visualize in the chart.
    /// <para>
    /// Replacing it re-derives the value axis from the new data, so a chart fed a fresh dataset on a
    /// timer rescales on every tick and one new point among twenty moves the other nineteen. That is
    /// what <see cref="StickyDomain"/> is for; <see cref="YMin"/> and <see cref="YMax"/> are the other
    /// answer where the range is known in advance.
    /// </para></summary>
    [Parameter] public ChartData? Data { get; set; }
    /// <summary>Optional title displayed above the chart and used as the SVG accessible name.</summary>
    [Parameter] public string? Title { get; set; }
    /// <summary>Optional description for the SVG &lt;desc&gt; element, improving screen-reader context.</summary>
    [Parameter] public string? Description { get; set; }
    /// <summary>Plot height in pixels. In fluid mode this is the rendered height exactly; otherwise it is
    /// the height half of the authored aspect ratio. Values below 1 are rendered as 1.</summary>
    [Parameter] public int Height { get; set; } = 220;

    /// <summary>
    /// Authored drawing width, in viewBox units. With <see cref="Fluid"/> off this and
    /// <see cref="Height"/> fix the chart's ASPECT RATIO: the drawing then scales like an image into
    /// whatever width its container gives it, so a 1200 x 220 chart stays wide and short at any size.
    /// In fluid mode it is only the width used until the first measurement arrives. Values below 1
    /// are rendered as 1.
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
    /// <summary>
    /// Whether hiding a series through the legend refits the plot or leaves it standing. The default
    /// <see cref="ChartScaleMode.FitVisible"/> measures the value axis from the series still shown, so
    /// one click moves the axis window, the grid lines and every remaining mark;
    /// <see cref="ChartScaleMode.FitAll"/> measures it from the whole dataset, so the series disappears
    /// and nothing else moves. Applies to the cartesian types and to radar; the radial types
    /// (<see cref="ChartType.Pie"/>, <see cref="ChartType.Donut"/>, <see cref="ChartType.Rose"/>,
    /// <see cref="ChartType.PolarArea"/>) divide a total rather than span an axis, so hiding a slice
    /// necessarily changes the rest and this has no effect on them. <see cref="YMin"/> and
    /// <see cref="YMax"/> still win where they are set.
    /// </summary>
    [Parameter] public ChartScaleMode ScaleMode { get; set; }
    /// <summary>Overrides the plot padding (all sides, in viewBox units). Null keeps the default axis padding.
    /// Each side is clamped to leave at least one viewBox unit for the plot; negative values become zero.</summary>
    [Parameter] public int? Padding { get; set; }
    /// <summary>Sparkline preset: a compact, chromeless line (no grid, axes, legend or title) that stretches
    /// edge-to-edge with a crisp stroke - for inline metric cards. Combine with <see cref="Area"/> for a fill.</summary>
    [Parameter] public bool Sparkline { get; set; }

    /// <summary>
    /// Keeps the value axis from shrinking back, so a chart fed live data only rescales when it has to.
    /// The domain grows to fit whatever arrives and holds the widest range it has seen; a new dataset
    /// that fits inside it leaves the plot exactly where it was, and one new point among twenty stops
    /// moving the other nineteen.
    /// <para>
    /// It does give the space back, but only when the data has genuinely moved into a smaller range -
    /// less than half the range being held. A domain that never shrinks is ruined for good by a single
    /// spike; one that shrinks on any decrease is the jitter this exists to remove. Call
    /// <see cref="ResetDomain"/> when the caller knows the scale has changed for good - a different
    /// metric in the same chart, say. <see cref="YMin"/> and <see cref="YMax"/> still win where they are
    /// set, and pinning both makes this moot.
    /// </para>
    /// </summary>
    [Parameter] public bool StickyDomain { get; set; }

    /// <summary>Overrides the automatic minimum of the value axis. Nonfinite values are ignored.
    /// When both bounds are finite but the minimum is not below the maximum, both are ignored.</summary>
    [Parameter] public double? YMin { get; set; }
    /// <summary>Overrides the automatic maximum of the value axis. Nonfinite values are ignored.
    /// When both bounds are finite but the minimum is not below the maximum, both are ignored.
    /// Stacked bars ignore nonpositive upper bounds because their stacks start at zero.</summary>
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

    /// <summary>
    /// Moves the drawing from one dataset to the next instead of replacing it, so a value that changed
    /// reads as a movement rather than as a different picture. For a chart fed on a timer this is what
    /// makes an update legible - and it is NOT <see cref="Animate"/>, which replays the enter animation
    /// and so redraws the whole series from nothing on every refresh.
    /// <para>
    /// It covers everything the chart draws, not just the line: bars, slices, radar spokes and the value
    /// labels that travel with them. A drawing whose SHAPE changed still jumps - a series that gained or
    /// lost a point is a different path, and walking one into the other would slide every point into its
    /// neighbour's place and read as data that did not actually change.
    /// </para>
    /// <para>
    /// Duration and easing come from the theme's motion scale, and a reader who asked for reduced motion
    /// gets the jump. This is the one part of the chart that needs the browser: it costs one call when
    /// the chart appears and nothing per update.
    /// </para>
    /// </summary>
    [Parameter] public bool AnimateUpdates { get; set; }
    /// <summary>Also renders a visually-hidden data <c>&lt;table&gt;</c> after the chart, so screen readers
    /// can read all underlying values, including hidden series and points outside the zoom window.
    /// Scatter and bubble tables include X/Y coordinates and bubble weights. Missing values are named
    /// explicitly; an empty data set displays a message (an accessibility fallback for the SVG).</summary>
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
    private string AccessibleName => string.IsNullOrWhiteSpace(Title) ? FlareStrings.Chart_AccessibleName : Title;
    private string DescriptionId => $"{_uid}-description";
    private string TooltipId => $"{_uid}-tooltip";

    // Series toggled off via the interactive legend.
    private readonly HashSet<int> _hidden = new();
    private bool IsHidden(int si) => _hidden.Contains(si);

    // Whether a hidden series still counts when the plot is measured. Under FitAll it does, which is
    // what keeps the axis, the grid and the bar slots still while a series is switched off; the
    // per-mark IsHidden guards are untouched either way, so a hidden series is never drawn.
    private bool CountsTowardsScale(int si) => ScaleMode == ChartScaleMode.FitAll || !IsHidden(si);

    // The values the value axis is measured from.
    private List<double> ScaleValues(IReadOnlyList<ChartSeries> series) =>
        Plotted(series.Where((_, i) => CountsTowardsScale(i)).SelectMany(s => s.Values));

    // Indices that own a slot in a grouped bar chart. Under FitVisible only the drawn series do, so the
    // group closes over a hidden one; under FitAll every series keeps its slot and the gap stays.
    private List<int> SlotIndices(int count) =>
        [.. Enumerable.Range(0, count).Where(CountsTowardsScale)];
    private void ToggleSeries(int si) { if (!_hidden.Remove(si)) _hidden.Add(si); DismissDetail(); }
    private bool _legendRow => _showLegend && LegendPosition is ChartLegendPosition.Left or ChartLegendPosition.Right;
    private Task HandlePointClick(int index) => OnPointClick.InvokeAsync(index);

    // SVG numeric attributes require '.' as the decimal separator; never use the current
    // culture (e.g. ru-RU formats 192.0 as "192,0", which is invalid in SVG).
    private static readonly System.Globalization.CultureInfo _inv = System.Globalization.CultureInfo.InvariantCulture;

    // Unique per-instance id prefix so area gradients from multiple charts on a page never collide.
    private readonly string _uid = $"fc{Guid.NewGuid():N}";

    // The drawing width in viewBox units. Fluid mode swaps the authored Width for the element's measured
    // CSS width, which is what makes one viewBox unit one pixel - so the SVG can be pinned to Height px
    // and still fill the container without stretching anything.
    private int _svgW => Math.Max(1, _fluidActive && _measuredWidth is { } w ? w : Width);
    private int _svgH => Math.Max(1, Height);

    // Both fluid and sparkline pin the CSS height, for different reasons: the sparkline distorts on
    // purpose (preserveAspectRatio="none"), fluid never distorts because the viewBox already matches the
    // box. Unmeasured fluid falls back to the authored aspect ratio (CSS height:auto), so a prerendered
    // or JS-less page draws a correct chart rather than a letterboxed one.
    private string? _svgStyle => _spark || (_fluidActive && _measuredWidth is not null) ? $"height:{_svgH}px" : null;

    private bool _spark => Sparkline;
    private bool _fluidActive => Fluid && !_spark;
    // Area fill is on for the explicit Area flag or the Area chart type.
    private bool _area => Area || Type == ChartType.Area;
    // Sparkline zeroes the chrome; otherwise a Padding override applies to all sides, else the axis defaults.
    private int PlotPadding(int fallback, int dimension) =>
        Math.Clamp(_spark ? 2 : (Padding ?? fallback), 0, (dimension - 1) / 2);
    private int _padL => PlotPadding(36, _svgW);
    private int _padR => PlotPadding(12, _svgW);
    private int _padT => PlotPadding(12, _svgH);
    private int _padB => PlotPadding(28, _svgH);
    private int _plotW => _svgW - _padL - _padR;
    private int _plotH => _svgH - _padT - _padB;

    // Chrome toggles - all forced off in sparkline mode.
    private bool _showGrid => !_spark && ShowGrid;
    private bool _showY => !_spark && ShowYAxisLabels;
    private bool _showX => !_spark && ShowXAxisLabels;
    private bool _showLegend => !_spark && ShowLegend && LegendPosition != ChartLegendPosition.None
        && Type != ChartType.HeatMap && Data?.Series is { Count: > 0 };

    // Tooltip position uses the pointer's viewport coordinates in CSS pixels.
    private bool _tooltipVisible;
    private double _hoverX;
    private double _hoverY;
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
        $"stroke:var({Css.Tokens.Chart.GridColor});stroke-width:var({Css.Tokens.Chart.GridWidth});stroke-dasharray:var({Css.Tokens.Chart.GridDash})";
    // A minor line reuses the major dash so a theme that dashes its grid dashes all of it, and carries its
    // own color and width so it can be pushed as far back as the theme wants - or hidden outright.
    private const string _gridMinorStyle =
        $"stroke:var({Css.Tokens.Chart.GridMinorColor});stroke-width:var({Css.Tokens.Chart.GridMinorWidth});stroke-dasharray:var({Css.Tokens.Chart.GridDash})";
    private const string _labelStyle =
        $"fill:var({Css.Tokens.Chart.LabelColor});font-size:var({Css.Tokens.Chart.LabelSize})";
    private const string _valueStyle =
        $"fill:var({Css.Tokens.Chart.ValueColor});font-size:var({Css.Tokens.Chart.ValueSize})";
    private const string _valueOnFillStyle =
        $"fill:var({Css.Tokens.Chart.ValueOnFillColor});font-size:var({Css.Tokens.Chart.ValueSize})";
    private const string _axisTitleStyle =
        $"fill:var({Css.Tokens.Chart.AxisTitleColor});font-size:var({Css.Tokens.Chart.AxisTitleSize})";
    private const string _sliceStrokeStyle =
        $"stroke:var({Css.Tokens.Chart.SliceStrokeColor});stroke-width:var({Css.Tokens.Chart.SliceStrokeWidth})";

}
