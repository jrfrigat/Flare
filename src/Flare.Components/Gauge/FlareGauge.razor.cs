using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// One value read against a marked scale: a needle dial, a filled KPI arc, or a straight bar with ticks.
/// <see cref="FlareProgress"/> and <see cref="FlareMeter"/> are both bars and
/// answer "how far along"; a gauge answers "where does this sit on the scale, and is that good", which is
/// the dashboard reading.
/// </summary>
/// <remarks>
/// Coloured bands are <see cref="FlareZone"/> children - the same <c>Start</c>/<c>End</c>-on-a-host-scale
/// primitive <see cref="FlareSlider"/> and <see cref="FlareProgress"/> already take, rather than a
/// gauge-specific range type. Which part of a scale counts as bad belongs to the application's data, so it
/// arrives as a zone with a colour rather than as a token a theme would have to guess at.
///
/// No JS: the whole gauge is one SVG whose geometry is computed in C# and whose every colour and thickness
/// is a token read by the stylesheet.
/// </remarks>
public partial class FlareGauge : FlareComponentBase
{
    /// <summary>Zones drawn as coloured bands behind the fill, declared as <see cref="FlareZone"/> children.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>The value to read against the scale. Values outside the scale pin to its ends.</summary>
    [Parameter] public double Value { get; set; }

    /// <summary>Low end of the scale. Default 0.</summary>
    [Parameter] public double Min { get; set; }

    /// <summary>High end of the scale. Default 100.</summary>
    [Parameter] public double Max { get; set; } = 100;

    /// <summary>How the scale is laid out. Default <see cref="GaugeShape.Arc"/>.</summary>
    [Parameter] public GaugeShape Shape { get; set; } = GaugeShape.Arc;

    /// <summary>
    /// Where the scale starts, in degrees clockwise from twelve o'clock. Ignored by
    /// <see cref="GaugeShape.Linear"/>. Defaults to -90 (nine o'clock) for an arc and -135 for a radial
    /// dial, which puts the dial's gap at the bottom.
    /// </summary>
    [Parameter] public double? StartAngle { get; set; }

    /// <summary>Where the scale ends, in degrees clockwise from twelve o'clock. Defaults to 90 for an arc
    /// and 135 for a radial dial. A 360-degree sweep gives the progress-ring case.</summary>
    [Parameter] public double? EndAngle { get; set; }

    /// <summary>Lays a <see cref="GaugeShape.Linear"/> gauge bottom-to-top instead of left-to-right.</summary>
    [Parameter] public bool Vertical { get; set; }

    /// <summary>
    /// Reads the value with a needle rather than by filling the track. Defaults to true for
    /// <see cref="GaugeShape.Radial"/> and false for the other two - a needle is the dial's own idiom, and
    /// a filled arc is the KPI one.
    /// </summary>
    [Parameter] public bool? Needle { get; set; }

    /// <summary>Colour of the fill (and of the needle when one is drawn). Default takes the theme's.</summary>
    [Parameter] public FlareColor Color { get; set; } = FlareColor.Default;

    /// <summary>Distance between major ticks, on the scale's own units. Default: a fifth of the scale.</summary>
    [Parameter] public double TickInterval { get; set; }

    /// <summary>Distance between minor ticks. Zero (default) draws none.</summary>
    [Parameter] public double MinorTickInterval { get; set; }

    /// <summary>Draws the tick marks. Default true.</summary>
    [Parameter] public bool ShowTicks { get; set; } = true;

    /// <summary>Labels the major ticks with their values. Default false - a dial with a big readout in the
    /// middle rarely needs the scale spelled out as well.</summary>
    [Parameter] public bool ShowScaleLabels { get; set; }

    /// <summary>Draws the value in the middle of the gauge. Default true.</summary>
    [Parameter] public bool ShowValue { get; set; } = true;

    /// <summary>Format applied to the value and the scale labels (for example <c>"N0"</c>, <c>"P0"</c>).</summary>
    [Parameter] public string? Format { get; set; }

    /// <summary>Replaces the readout with custom content, receiving the current value.</summary>
    [Parameter] public RenderFragment<double>? ValueTemplate { get; set; }

    /// <summary>Caption under the readout - what the gauge is measuring.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// A second value marked on the scale: the target, the budget, last month's figure. Drawn as a marker
    /// across the track rather than as a second needle, because a gauge answers one question.
    /// </summary>
    [Parameter] public double? Target { get; set; }

    /// <summary>Sweeps the fill and needle to the value on first render and on every change. Default true;
    /// a reduced-motion preference suppresses it regardless.</summary>
    [Parameter] public bool Animate { get; set; } = true;

    /// <summary>
    /// Describes the gauge for assistive technology. Falls back to <see cref="Label"/>; the value, range
    /// and band the value falls in are announced from the ARIA meter attributes either way.
    /// </summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Gauge.Root;

    private readonly ZoneCollection _zones;

    /// <summary>Creates the gauge and the collection its <see cref="FlareZone"/> children register with.</summary>
    public FlareGauge() => _zones = new ZoneCollection(StateHasChanged);

    // Defaults that depend on the shape, resolved here so the parameters can stay null and mean "the
    // idiomatic angle for this shape" rather than carrying a number that is wrong for the other two.
    private double DefaultStart => Shape == GaugeShape.Radial ? -135 : -90;
    private double DefaultEnd => Shape == GaugeShape.Radial ? 135 : 90;
    private double Start => StartAngle ?? DefaultStart;
    private double End => EndAngle ?? DefaultEnd;
    private bool UseNeedle => Needle ?? Shape == GaugeShape.Radial;

    private string ShapeClass => Shape switch
    {
        GaugeShape.Radial => Css.Classes.Gauge.Radial,
        GaugeShape.Linear => Vertical ? Css.Classes.Gauge.LinearVertical : Css.Classes.Gauge.Linear,
        _ => Css.Classes.Gauge.Arc,
    };

    private string Display(double value) => value.ToString(Format, CultureInfo.CurrentCulture);

    // The bands, ordered and clipped to the scale. A zone with a missing bound collapses and is dropped by
    // the shared collection, which is why the ordering is on the pair rather than on Start alone.
    private IReadOnlyList<FlareZone> Bands => _zones
        .Typed<FlareZone>(nameof(FlareGauge), "<FlareZone Start=\"..\" End=\"..\" /> children")
        .Where(z => z.Start is not null && z.End is not null)
        .OrderBy(z => Math.Min(z.Start!.Value, z.End!.Value))
        .ToList();

    // What a screen reader is told the value means: the band it falls in, when the application named one.
    private string? ValueText
    {
        get
        {
            var band = Bands.FirstOrDefault(z =>
                ClampedValue >= Math.Min(z.Start!.Value, z.End!.Value) &&
                ClampedValue <= Math.Max(z.Start!.Value, z.End!.Value));
            var shown = Display(ClampedValue);
            return band?.Label is { Length: > 0 } name ? $"{shown} ({name})" : shown;
        }
    }
}
