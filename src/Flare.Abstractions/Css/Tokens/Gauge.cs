namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for <c>FlareGauge</c>. A gauge draws in two languages at once, like the chart:
/// colors resolve through CSS while geometry is written into the SVG, so sizes here are plain numbers in
/// viewBox units rather than lengths. The viewBox is 200 wide and the component scales to its container,
/// so a 6 here is 6/200 of the gauge's width at any rendered size.
///
/// Band colors are deliberately absent: a band is a <c>FlareZone</c> and carries its own
/// <c>FlareColor</c>, and "red means bad" is an application's statement about its data, not a token.
/// </summary>
public static class Gauge
{
    /// <summary>CSS custom-property name for the unfilled track color.</summary>
    public const string TrackColor = "--flare-gauge-track-color";
    /// <summary>CSS custom-property name for the track thickness, in viewBox units.</summary>
    public const string TrackWidth = "--flare-gauge-track-width";
    /// <summary>CSS custom-property name for the end treatment of the track and fill (<c>butt</c>/<c>round</c>).</summary>
    public const string TrackCap = "--flare-gauge-track-cap";

    /// <summary>CSS custom-property name for the color of the filled portion.</summary>
    public const string FillColor = "--flare-gauge-fill-color";
    /// <summary>CSS custom-property name for the thickness of the filled portion, in viewBox units.</summary>
    public const string FillWidth = "--flare-gauge-fill-width";

    /// <summary>CSS custom-property name for the needle color.</summary>
    public const string NeedleColor = "--flare-gauge-needle-color";
    /// <summary>CSS custom-property name for the needle width, in viewBox units.</summary>
    public const string NeedleWidth = "--flare-gauge-needle-width";
    /// <summary>CSS custom-property name for the needle length, as a fraction of the gauge radius.</summary>
    public const string NeedleLength = "--flare-gauge-needle-length";
    /// <summary>CSS custom-property name for the color of the disc the needle pivots on.</summary>
    public const string PivotColor = "--flare-gauge-pivot-color";
    /// <summary>CSS custom-property name for the radius of the pivot disc, in viewBox units.</summary>
    public const string PivotRadius = "--flare-gauge-pivot-radius";

    /// <summary>CSS custom-property name for the major tick color.</summary>
    public const string TickColor = "--flare-gauge-tick-color";
    /// <summary>CSS custom-property name for the major tick width, in viewBox units.</summary>
    public const string TickWidth = "--flare-gauge-tick-width";
    /// <summary>CSS custom-property name for the major tick length, in viewBox units.</summary>
    public const string TickLength = "--flare-gauge-tick-length";
    /// <summary>CSS custom-property name for the minor tick color.</summary>
    public const string TickMinorColor = "--flare-gauge-tick-minor-color";
    /// <summary>CSS custom-property name for the minor tick width, in viewBox units.</summary>
    public const string TickMinorWidth = "--flare-gauge-tick-minor-width";
    /// <summary>CSS custom-property name for the minor tick length, in viewBox units.</summary>
    public const string TickMinorLength = "--flare-gauge-tick-minor-length";
    /// <summary>CSS custom-property name for the gap between the track and the ticks, in viewBox units.</summary>
    public const string TickGap = "--flare-gauge-tick-gap";

    /// <summary>CSS custom-property name for the scale label color.</summary>
    public const string LabelColor = "--flare-gauge-label-color";
    /// <summary>CSS custom-property name for the scale label size, in viewBox units.</summary>
    public const string LabelSize = "--flare-gauge-label-size";
    /// <summary>CSS custom-property name for the readout color.</summary>
    public const string ValueColor = "--flare-gauge-value-color";
    /// <summary>CSS custom-property name for the readout size. A CSS length: the readout is DOM text
    /// beside the SVG, not inside it, so a viewBox unit would mean nothing there.</summary>
    public const string ValueSize = "--flare-gauge-value-size";
    /// <summary>CSS custom-property name for the readout font weight.</summary>
    public const string ValueWeight = "--flare-gauge-value-weight";

    /// <summary>CSS custom-property name for the opacity a band is painted at behind the fill.</summary>
    public const string BandOpacity = "--flare-gauge-band-opacity";
    /// <summary>CSS custom-property name for the target marker's color.</summary>
    public const string TargetColor = "--flare-gauge-target-color";
    /// <summary>CSS custom-property name for the target marker's width, in viewBox units.</summary>
    public const string TargetWidth = "--flare-gauge-target-width";
}
