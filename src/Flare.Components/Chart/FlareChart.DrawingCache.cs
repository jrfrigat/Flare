using System.Globalization;

namespace Flare.Components;

public partial class FlareChart
{
    private readonly record struct DrawingOptions(ChartType Type, string? Title, string? Description,
        int Width, int Height, bool Fluid, bool Area, bool Smooth, ChartLineStyle LineStyle, bool ShowMarkers,
        bool ShowGrid, bool ShowYAxisLabels, bool ShowXAxisLabels, ChartScaleMode ScaleMode, int? Padding,
        bool Sparkline, bool StickyDomain, double? YMin, double? YMax, string? YAxisFormat, int? YAxisTickCount,
        bool NiceScale, int YAxisMinorTicks, bool ShowVerticalGrid, string? XAxisTitle, string? YAxisTitle,
        bool Horizontal, bool ShowValues, double DonutRingRatio, double BarWidthRatio, bool TrendLine,
        bool Zoomable, ChartZoom? Zoom);

    private readonly record struct DrawingTokens(string? BarRadius, string? CellRadius, string? CellGap,
        string? PointRadius, string? BubbleMinRadius, string? BubbleMaxRadius, string? RampMinOpacity,
        string? RampMaxOpacity, string? AnnotationArrowSize, string? AnnotationPointRadius);

    private DrawingOptions _drawingOptions;
    private DrawingTokens _drawingTokens;
    private ChartData? _drawingData;
    private ChartAnnotation[]? _drawingAnnotations;
    private FlareLocalizationState? _drawingLocalization;
    private CultureInfo? _drawingCulture;
    private CultureInfo? _drawingUiCulture;

    private void UpdateDrawingRevision()
    {
        var options = new DrawingOptions(Type, Title, Description, Width, Height, Fluid, Area, Smooth,
            LineStyle, ShowMarkers, ShowGrid, ShowYAxisLabels, ShowXAxisLabels, ScaleMode, Padding, Sparkline,
            StickyDomain, YMin, YMax, YAxisFormat, YAxisTickCount, NiceScale, YAxisMinorTicks, ShowVerticalGrid,
            XAxisTitle, YAxisTitle, Horizontal, ShowValues, DonutRingRatio, BarWidthRatio, TrendLine, Zoomable, _internalZoom);
        var tokens = new DrawingTokens(ReadTokenRaw(Css.Tokens.Chart.BarRadius), ReadTokenRaw(Css.Tokens.Chart.CellRadius),
            ReadTokenRaw(Css.Tokens.Chart.CellGap), ReadTokenRaw(Css.Tokens.Chart.PointRadius), ReadTokenRaw(Css.Tokens.Chart.BubbleMinRadius),
            ReadTokenRaw(Css.Tokens.Chart.BubbleMaxRadius), ReadTokenRaw(Css.Tokens.Chart.RampMinOpacity),
            ReadTokenRaw(Css.Tokens.Chart.RampMaxOpacity), ReadTokenRaw(Css.Tokens.Chart.AnnotationArrowSize),
            ReadTokenRaw(Css.Tokens.Chart.AnnotationPointRadius));
        var culture = FormattingCulture;
        var uiCulture = InterfaceCulture;
        var changed = options != _drawingOptions || tokens != _drawingTokens ||
            !ReferenceEquals(LocalizationState, _drawingLocalization) ||
            !ReferenceEquals(culture, _drawingCulture) || !culture.IsReadOnly ||
            !ReferenceEquals(uiCulture, _drawingUiCulture) || !uiCulture.IsReadOnly;

        // Collections can be edited in place. Compare with the values we drew, including the order
        // and the series settings, and only take a new snapshot when those values change.
        if (!DataMatches(Data, _drawingData))
        {
            _drawingData = SnapshotData(Data, _drawingData);
            changed = true;
        }
        if (!ListMatches(Annotations, _drawingAnnotations))
        {
            _drawingAnnotations = CopyList(Annotations, _drawingAnnotations);
            changed = true;
        }

        if (!changed) return;
        _drawingOptions = options;
        _drawingTokens = tokens;
        _drawingLocalization = LocalizationState;
        _drawingCulture = culture;
        _drawingUiCulture = uiCulture;
        _drawingRevision++;
    }

    private static bool DataMatches(ChartData? current, ChartData? previous)
    {
        if (current is null || previous is null) return current is null && previous is null;
        if (current.Series.Count != previous.Series.Count || !ListMatches(current.Labels, previous.Labels))
            return false;

        for (var i = 0; i < current.Series.Count; i++)
        {
            var a = current.Series[i];
            var b = previous.Series[i];
            if (a.Label != b.Label || a.Color != b.Color || a.Kind != b.Kind || a.Smooth != b.Smooth ||
                a.Area != b.Area || a.LineStyle != b.LineStyle || a.ShowMarkers != b.ShowMarkers ||
                !ListMatches(a.Values, b.Values, SameNumber) ||
                !ListMatches(a.Points, b.Points, static (x, y) => SameNumber(x.X, y.X) && SameNumber(x.Y, y.Y) && SameNumber(x.R, y.R)))
                return false;
        }
        return true;
    }

    private static bool SameNumber(double first, double second) =>
        BitConverter.DoubleToInt64Bits(first) == BitConverter.DoubleToInt64Bits(second);

    private static ChartData? SnapshotData(ChartData? data, ChartData? previous)
    {
        if (data is null) return null;

        var series = previous?.Series as ChartSeries[];
        if (series?.Length != data.Series.Count)
            series = new ChartSeries[data.Series.Count];

        for (var i = 0; i < series.Length; i++)
        {
            var last = series[i];
            var current = data.Series[i];
            series[i] = current with
            {
                Values = CopyList(current.Values, last?.Values)!,
                Points = CopyList(current.Points, last?.Points),
            };
        }
        return new ChartData(series, CopyList(data.Labels, previous?.Labels));
    }

    private static T[]? CopyList<T>(IReadOnlyList<T>? source, IReadOnlyList<T>? previous)
    {
        if (source is null) return null;

        var copy = previous as T[];
        if (copy?.Length != source.Count)
            copy = new T[source.Count];
        for (var i = 0; i < copy.Length; i++)
            copy[i] = source[i];
        return copy;
    }

    private static bool ListMatches<T>(IReadOnlyList<T>? first, IReadOnlyList<T>? second, Func<T, T, bool>? matches = null)
    {
        if (first is null || second is null) return first is null && second is null;
        if (first.Count != second.Count) return false;

        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < first.Count; i++)
            if (!(matches?.Invoke(first[i], second[i]) ?? comparer.Equals(first[i], second[i])))
                return false;
        return true;
    }
}
