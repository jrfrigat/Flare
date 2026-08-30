using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for <c>FlareGauge</c>. Geometry that the renderer writes into SVG is expressed in
/// viewBox units - plain numbers, not lengths - because the gauge scales to its container and a length
/// here would not scale with it.
///
/// There are no band colors in this record on purpose. A band is a <c>FlareZone</c> with its own
/// <c>FlareColor</c>: which range of a scale counts as bad is a statement about the data, and a theme
/// that shipped a red/amber/green ramp would be making that statement for every application at once.
/// </summary>
public sealed record GaugeTokens
{
    /// <summary>Color of the unfilled track.</summary>
    [CssVar(Gauge.TrackColor)] public required string TrackColor { get; init; }
    /// <summary>Thickness of the track, in viewBox units.</summary>
    [CssVar(Gauge.TrackWidth)] public required string TrackWidth { get; init; }
    /// <summary>End treatment of the track and the fill (<c>butt</c>, <c>round</c>, <c>square</c>).</summary>
    [CssVar(Gauge.TrackCap)] public required string TrackCap { get; init; }

    /// <summary>Color of the filled portion when the gauge does not take an explicit one.</summary>
    [CssVar(Gauge.FillColor)] public required string FillColor { get; init; }
    /// <summary>Thickness of the filled portion, in viewBox units. Exceeding the track width makes the
    /// fill overhang, which is how a theme draws a fill that sits proud of its track.</summary>
    [CssVar(Gauge.FillWidth)] public required string FillWidth { get; init; }

    /// <summary>Color of the needle.</summary>
    [CssVar(Gauge.NeedleColor)] public required string NeedleColor { get; init; }
    /// <summary>Width of the needle, in viewBox units.</summary>
    [CssVar(Gauge.NeedleWidth)] public required string NeedleWidth { get; init; }
    /// <summary>Length of the needle as a fraction of the gauge radius (<c>0.8</c> stops short of the track).</summary>
    [CssVar(Gauge.NeedleLength)] public required string NeedleLength { get; init; }
    /// <summary>Color of the disc the needle pivots on.</summary>
    [CssVar(Gauge.PivotColor)] public required string PivotColor { get; init; }
    /// <summary>Radius of the pivot disc, in viewBox units. Zero removes it.</summary>
    [CssVar(Gauge.PivotRadius)] public required string PivotRadius { get; init; }

    /// <summary>Color of a major tick.</summary>
    [CssVar(Gauge.TickColor)] public required string TickColor { get; init; }
    /// <summary>Width of a major tick, in viewBox units.</summary>
    [CssVar(Gauge.TickWidth)] public required string TickWidth { get; init; }
    /// <summary>Length of a major tick, in viewBox units.</summary>
    [CssVar(Gauge.TickLength)] public required string TickLength { get; init; }
    /// <summary>Color of a minor tick.</summary>
    [CssVar(Gauge.TickMinorColor)] public required string TickMinorColor { get; init; }
    /// <summary>Width of a minor tick, in viewBox units.</summary>
    [CssVar(Gauge.TickMinorWidth)] public required string TickMinorWidth { get; init; }
    /// <summary>Length of a minor tick, in viewBox units.</summary>
    [CssVar(Gauge.TickMinorLength)] public required string TickMinorLength { get; init; }
    /// <summary>Gap between the outside of the track and the ticks, in viewBox units.</summary>
    [CssVar(Gauge.TickGap)] public required string TickGap { get; init; }

    /// <summary>Color of a scale label.</summary>
    [CssVar(Gauge.LabelColor)] public required string LabelColor { get; init; }
    /// <summary>Size of a scale label, in viewBox units.</summary>
    [CssVar(Gauge.LabelSize)] public required string LabelSize { get; init; }
    /// <summary>Color of the value readout.</summary>
    [CssVar(Gauge.ValueColor)] public required string ValueColor { get; init; }
    /// <summary>Size of the value readout, as a CSS length - the readout is DOM text beside the SVG
    /// rather than inside it, so it scales with the page and not with the viewBox.</summary>
    [CssVar(Gauge.ValueSize)] public required string ValueSize { get; init; }
    /// <summary>Font weight of the value readout.</summary>
    [CssVar(Gauge.ValueWeight)] public required string ValueWeight { get; init; }

    /// <summary>Opacity a band is painted at, so the fill drawn over it stays legible.</summary>
    [CssVar(Gauge.BandOpacity)] public required string BandOpacity { get; init; }
    /// <summary>Color of the target marker.</summary>
    [CssVar(Gauge.TargetColor)] public required string TargetColor { get; init; }
    /// <summary>Width of the target marker, in viewBox units.</summary>
    [CssVar(Gauge.TargetWidth)] public required string TargetWidth { get; init; }
}
