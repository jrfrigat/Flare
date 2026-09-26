using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.FluentUI2;

/// <summary>
/// Fluent UI 2 palette generator: a flatter brand "ramp" with neutral (un-tinted) grays for
/// surfaces, the brand as primary/info, and a gray secondary -- matching Fluent's look rather
/// than MD3's tonal surfaces.
/// </summary>
public sealed class Fluent2RampGenerator : IPaletteGenerator
{
    public static readonly Fluent2RampGenerator Instance = new();

    private static string Gray(double l) => ColorMath.FromHsl(0, 0, l);

    public Palette Generate(string id, string name, PaletteSeed seed, string? source = null)
    {
        var main = seed.Main;
        var tertiary = ColorMath.RotateHue(main, -40);
        // Fixed roles do not change with the mode: the light container pair, and the dark accent as the
        // dim step.
        var primaryFixed = ColorMath.Lighten(main, 0.85);
        var primaryFixedDim = ColorMath.Lighten(main, 0.30);
        var onPrimaryFixed = ColorMath.Darken(main, 0.50);
        var tertiaryFixed = ColorMath.Lighten(tertiary, 0.85);
        var tertiaryFixedDim = ColorMath.Lighten(tertiary, 0.45);
        var onTertiaryFixed = ColorMath.Darken(tertiary, 0.50);

        return new Palette
        {
            Id = id,
            Name = name,
            Source = source,
            Light = new ColorScheme
            {
                Primary = main,
                OnPrimary = ColorMath.OnColor(main),
                PrimaryContainer = ColorMath.Lighten(main, 0.85),
                OnPrimaryContainer = ColorMath.Darken(main, 0.50),
                Secondary = Gray(0.38),
                OnSecondary = "#FFFFFF",
                SecondaryContainer = Gray(0.92),
                OnSecondaryContainer = Gray(0.14),
                Tertiary = ColorMath.WithLightness(tertiary, 0.42),
                OnTertiary = "#FFFFFF",
                TertiaryContainer = ColorMath.Lighten(tertiary, 0.85),
                OnTertiaryContainer = ColorMath.Darken(tertiary, 0.50),
                Error = "#B10E1C",
                OnError = "#FFFFFF",
                ErrorContainer = "#FDE7E9",
                OnErrorContainer = "#6E0811",
                Success = "#0E700E",
                OnSuccess = "#FFFFFF",
                SuccessContainer = "#DFF6DD",
                OnSuccessContainer = "#052505",
                Warning = "#835B00",
                OnWarning = "#FFFFFF",
                WarningContainer = "#FFF4CE",
                OnWarningContainer = "#3D2C00",
                Info = main,
                OnInfo = ColorMath.OnColor(main),
                InfoContainer = ColorMath.Lighten(main, 0.85),
                OnInfoContainer = ColorMath.Darken(main, 0.50),
                Surface = "#FFFFFF",
                OnSurface = "#242424",
                SurfaceVariant = "#F5F5F5",
                OnSurfaceVariant = "#616161",
                OnSurfaceVariant2 = "#8A8A8A",
                SurfaceContainerLowest = "#FFFFFF",
                SurfaceContainerLow = "#F5F5F5",
                SurfaceContainer = "#EBEBEB",
                SurfaceContainerHigh = "#E0E0E0",
                SurfaceContainerHighest = "#D6D6D6",
                SurfaceBright = "#FFFFFF",
                SurfaceDim = "#D6D6D6",
                PrimaryFixed = primaryFixed,
                PrimaryFixedDim = primaryFixedDim,
                OnPrimaryFixed = onPrimaryFixed,
                OnPrimaryFixedVariant = onPrimaryFixed,
                SecondaryFixed = Gray(0.92),
                SecondaryFixedDim = Gray(0.68),
                OnSecondaryFixed = Gray(0.14),
                OnSecondaryFixedVariant = Gray(0.14),
                TertiaryFixed = tertiaryFixed,
                TertiaryFixedDim = tertiaryFixedDim,
                OnTertiaryFixed = onTertiaryFixed,
                OnTertiaryFixedVariant = onTertiaryFixed,
                Background = "#F5F5F5",
                OnBackground = "#242424",
                Outline = "#D1D1D1",
                OutlineVariant = "#E0E0E0",
                InverseSurface = "#292929",
                InverseOnSurface = "#F5F5F5",
                InversePrimary = ColorMath.Lighten(main, 0.45),
                Scrim = "#000000",
                Shadow = "#000000",
                ShadowUmbra = "rgba(0,0,0,0.14)",
                ShadowPenumbra = "rgba(0,0,0,0.12)",
            },
            Dark = new ColorScheme
            {
                Primary = ColorMath.Lighten(main, 0.30),
                OnPrimary = ColorMath.Darken(main, 0.55),
                PrimaryContainer = ColorMath.Darken(main, 0.30),
                OnPrimaryContainer = ColorMath.Lighten(main, 0.85),
                Secondary = Gray(0.68),
                OnSecondary = Gray(0.10),
                SecondaryContainer = Gray(0.24),
                OnSecondaryContainer = Gray(0.94),
                Tertiary = ColorMath.Lighten(tertiary, 0.45),
                OnTertiary = "#11161C",
                TertiaryContainer = ColorMath.Darken(tertiary, 0.30),
                OnTertiaryContainer = ColorMath.Lighten(tertiary, 0.85),
                Error = "#F1707B",
                OnError = "#4A0007",
                ErrorContainer = "#750014",
                OnErrorContainer = "#FDE7E9",
                Success = "#5EC75E",
                OnSuccess = "#0B2E0B",
                SuccessContainer = "#0E5814",
                OnSuccessContainer = "#C9F4C9",
                Warning = "#F2C811",
                OnWarning = "#3D3000",
                WarningContainer = "#6B5300",
                OnWarningContainer = "#FFF1B3",
                Info = ColorMath.Lighten(main, 0.30),
                OnInfo = ColorMath.Darken(main, 0.55),
                InfoContainer = ColorMath.Darken(main, 0.30),
                OnInfoContainer = ColorMath.Lighten(main, 0.85),
                Surface = "#1A1A1A",
                OnSurface = "#F0F0F0",
                SurfaceVariant = "#2A2A2A",
                OnSurfaceVariant = "#ADADAD",
                OnSurfaceVariant2 = "#7A7A7A",
                SurfaceContainerLowest = "#141414",
                SurfaceContainerLow = "#222222",
                SurfaceContainer = "#2A2A2A",
                SurfaceContainerHigh = "#333333",
                SurfaceContainerHighest = "#3D3D3D",
                SurfaceBright = "#3D3D3D",
                SurfaceDim = "#1A1A1A",
                PrimaryFixed = primaryFixed,
                PrimaryFixedDim = primaryFixedDim,
                OnPrimaryFixed = onPrimaryFixed,
                OnPrimaryFixedVariant = onPrimaryFixed,
                SecondaryFixed = Gray(0.92),
                SecondaryFixedDim = Gray(0.68),
                OnSecondaryFixed = Gray(0.14),
                OnSecondaryFixedVariant = Gray(0.14),
                TertiaryFixed = tertiaryFixed,
                TertiaryFixedDim = tertiaryFixedDim,
                OnTertiaryFixed = onTertiaryFixed,
                OnTertiaryFixedVariant = onTertiaryFixed,
                Background = "#141414",
                OnBackground = "#F0F0F0",
                Outline = "#4A4A4A",
                OutlineVariant = "#3D3D3D",
                InverseSurface = "#F0F0F0",
                InverseOnSurface = "#1A1A1A",
                InversePrimary = main,
                Scrim = "#000000",
                Shadow = "#000000",
                ShadowUmbra = "rgba(0,0,0,0.3)",
                ShadowPenumbra = "rgba(0,0,0,0.25)",
            },
        };
    }
}
