namespace Flare.Theme.Aero;

/// <summary>
/// Aero theme-private CSS custom-property names. These are the theme's OWN tokens (not part of the Flare
/// core contract), so they live here in the theme project - the core stays theme-agnostic. The scoped Aero
/// CSS reads them and <see cref="AeroTokens"/> assigns their values per mode.
/// </summary>
public static class AeroCssVars
{
    /// <summary>The Aero focus-glow color, consumed by the scoped button/input CSS.</summary>
    public const string Glow = "--flare-aero-glow";
}
