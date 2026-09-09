namespace Flare.Css.Tokens.Md3e;

/// <summary>Material Design 3 Expressive CSS tokens for progress waves.</summary>
public static class Progress
{
    /// <summary>Total height of a linear wave.</summary>
    public const string Height = "--flare-progress-wavy-height";
    /// <summary>Wavelength of a determinate linear wave.</summary>
    public const string Length = "--flare-progress-wave-length";
    /// <summary>Wavelength of an indeterminate linear wave.</summary>
    public const string IndeterminateLength = "--flare-progress-indeterminate-wave-length";
    /// <summary>Amplitude of a linear wave.</summary>
    public const string Amplitude = "--flare-progress-wave-amplitude";
    /// <summary>Duration for a linear wave to travel by one wavelength.</summary>
    public const string Speed = "--flare-progress-wave-speed";
    /// <summary>Preferred circular wavelength; the renderer chooses a whole number of waves.</summary>
    public const string RingLength = "--flare-progress-ring-wave-length";
    /// <summary>Radial circular-wave amplitude as a CSS length or legacy unitless pixel value.</summary>
    public const string RingAmplitude = "--flare-progress-ring-wave-amplitude";
}
