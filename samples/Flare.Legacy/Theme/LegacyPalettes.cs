using Flare.Core.Tokens;
using Flare.Theme.FluentUI2;

namespace Flare.Legacy;

/// <summary>The gray "Windows / 1C" enterprise palette used by <see cref="LegacyTheme"/>.</summary>
public static class LegacyPalettes
{
    /// <summary>The single Legacy palette. The look stays light in both modes.</summary>
    public static readonly Palette Legacy = new()
    {
        Id = "legacy-gray",
        Name = "Legacy",
        Source = "Legacy",
        Light = Gray(Fluent2Palettes.Blue.Light),
        Dark = Gray(Fluent2Palettes.Blue.Light),
    };

    // Medium-gray window, white panels with dark borders, light-gray (tonal) buttons, and saturated
    // blue/purple/green accents. Applied to both modes - the legacy look stays light either way.
    private static ColorScheme Gray(ColorScheme c) => c with
    {
        Primary = "#2F6CB0",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#CFE0F2",
        OnPrimaryContainer = "#06325A",
        // Tonal buttons read from the secondary container -> light raised gray with dark text.
        Secondary = "#5A5A5A",
        OnSecondary = "#FFFFFF",
        SecondaryContainer = "#E2E2E2",
        OnSecondaryContainer = "#1E1E1E",
        Tertiary = "#8A56B0",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#E7DAF2",
        OnTertiaryContainer = "#2E1247",
        Surface = "#FFFFFF",
        OnSurface = "#000000",
        SurfaceVariant = "#E3E3E3",
        OnSurfaceVariant = "#333333",
        SurfaceContainerLow = "#F4F4F4",
        SurfaceContainer = "#E4E4E4",
        SurfaceContainerHigh = "#D6D6D6",
        SurfaceContainerHighest = "#CACACA",
        Background = "#C9C9C9",
        OnBackground = "#000000",
        Outline = "#5C5C5C",
        OutlineVariant = "#9A9A9A",
        Error = "#C00000",
        OnError = "#FFFFFF",
        Success = "#2E9E4F",
        OnSuccess = "#FFFFFF",
        Warning = "#B36B00",
    };
}
