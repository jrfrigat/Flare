namespace Flare.Theme.LiquidGlass;

/// <summary>
/// Liquid Glass theme-private CSS custom-property names (the gloss/rim/tint hooks). These are the theme's
/// OWN tokens (not part of the Flare core contract), so they live in the theme project - the core stays
/// theme-agnostic. The scoped Liquid Glass CSS reads them and <see cref="LiquidGlassTokens"/> assigns their
/// values per mode.
/// </summary>
public static class LiquidGlassCssVars
{
    /// <summary>Ambient glow color.</summary>
    public const string Glow = "--flare-liquid-glow";
    /// <summary>Surface tint.</summary>
    public const string Tint = "--flare-liquid-tint";
    /// <summary>Stronger surface tint.</summary>
    public const string TintStrong = "--flare-liquid-tint-strong";
    /// <summary>Content-layer tint.</summary>
    public const string ContentTint = "--flare-liquid-content-tint";
    /// <summary>Content-layer sheen.</summary>
    public const string ContentSheen = "--flare-liquid-content-sheen";
    /// <summary>Edge rim highlight.</summary>
    public const string Rim = "--flare-liquid-rim";
    /// <summary>Low-intensity rim highlight.</summary>
    public const string RimLow = "--flare-liquid-rim-low";
    /// <summary>Edge highlight.</summary>
    public const string Edge = "--flare-liquid-edge";
    /// <summary>Surface sheen.</summary>
    public const string Sheen = "--flare-liquid-sheen";
    /// <summary>Drop shadow.</summary>
    public const string Shadow = "--flare-liquid-shadow";
}
