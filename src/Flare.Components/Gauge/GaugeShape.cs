namespace Flare.Components;

/// <summary>
/// How a <see cref="FlareGauge"/> lays its scale out. One component with three shapes rather than three
/// components: the scale, bands, ticks, target marker and readout are the same in all three, and only the
/// projection from value to screen differs.
/// </summary>
public enum GaugeShape
{
    /// <summary>
    /// A near-full dial with the gap at the bottom, read with a needle. The speedometer reading, and the
    /// one shape where the needle carries the value rather than the fill.
    /// </summary>
    Radial,
    /// <summary>
    /// A half or quarter arc filled up to the value - the KPI dial. Narrowing the sweep to a full turn
    /// gives the progress-ring degenerate case.
    /// </summary>
    Arc,
    /// <summary>A straight track with the scale beside it, horizontal or vertical.</summary>
    Linear,
}
