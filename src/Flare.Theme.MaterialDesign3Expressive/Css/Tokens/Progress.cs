namespace Flare.Css.Tokens.Md3e;

/// <summary>Material Design 3 Expressive CSS tokens for progress waves.</summary>
public static class Progress
{
    /// <summary>Wavelength of a determinate linear wave.</summary>
    public const string Length = "--flare-progress-wave-length";
    /// <summary>Wavelength of an indeterminate linear wave.</summary>
    public const string IndeterminateLength = "--flare-progress-indeterminate-wave-length";
    /// <summary>
    /// Height of a linear wave's crest above the centre line. The bar is sized from it - one
    /// indicator thickness plus a crest and a trough - so this alone sets how tall a wave is.
    /// </summary>
    public const string Amplitude = "--flare-progress-wave-amplitude";
    /// <summary>Duration for a linear wave to travel by one wavelength.</summary>
    public const string Speed = "--flare-progress-wave-speed";
    /// <summary>
    /// Mask that shapes the circular wave: the whole ring drawing in one value, so replacing this
    /// token replaces the wave. Its geometry and the ring's are tied together - the mask's mean
    /// radius is 41% of the box and the CSS sets the ring's stroke to 18% to match - so a
    /// replacement must keep mean radius = (100% - stroke) / 2 or the wave slides off the ring.
    /// </summary>
    public const string RingMask = "--flare-progress-ring-wave-mask";
}
