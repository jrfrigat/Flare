using Flare.Abstractions;
using Flare.Abstractions.Tokens;

namespace Flare.Theming;

// PaletteSeed and IPaletteGenerator (the input DTO and the port) now live in Flare.Abstractions;
// only the default engine implementation remains here.

/// <summary>
/// Core, design-system-agnostic generator: derives every role from the seed using HSL/contrast
/// math (brand-tinted neutrals, derived containers/on-colors, fixed semantic accents). A theme
/// may supply a richer generator (e.g. a tonal or ramp-based scheme) in a later sprint; the contract is the same.
/// </summary>
public sealed class DefaultPaletteGenerator : IPaletteGenerator
{
    /// <summary>Shared singleton instance.</summary>
    public static readonly DefaultPaletteGenerator Instance = new();

    /// <summary>Generates a full light+dark <see cref="Palette"/> from the given seed.</summary>
    public Palette Generate(string id, string name, PaletteSeed seed, string? source = null) => new()
    {
        Id = id,
        Name = name,
        Source = source,
        Light = Build(seed, dark: false),
        Dark = Build(seed, dark: true),
    };

    private static ColorScheme Build(PaletteSeed seed, bool dark)
    {
        var main = seed.Main;
        var nHue = ColorMath.ToHsl(seed.Background ?? main).H;
        string Neutral(double l, double s = 0.06) => ColorMath.FromHsl(nHue, s, l);

        // Accent triad derived from the brand hue.
        var secBase = seed.Secondary ?? ColorMath.WithSaturation(main, Math.Min(0.30, ColorMath.ToHsl(main).S));
        var terBase = ColorMath.RotateHue(main, 60);

        if (!dark)
        {
            var surface = seed.Background ?? Neutral(0.99);
            return new ColorScheme
            {
                Primary = main,
                OnPrimary = ColorMath.OnColor(main),
                PrimaryContainer = ColorMath.Lighten(main, 0.82),
                OnPrimaryContainer = ColorMath.Darken(main, 0.55),
                Secondary = ColorMath.WithLightness(secBase, 0.42),
                OnSecondary = "#FFFFFF",
                SecondaryContainer = ColorMath.Lighten(secBase, 0.82),
                OnSecondaryContainer = ColorMath.Darken(secBase, 0.55),
                Tertiary = ColorMath.WithLightness(terBase, 0.42),
                OnTertiary = "#FFFFFF",
                TertiaryContainer = ColorMath.Lighten(terBase, 0.82),
                OnTertiaryContainer = ColorMath.Darken(terBase, 0.55),
                Error = "#B3261E",
                OnError = "#FFFFFF",
                ErrorContainer = "#F9DEDC",
                OnErrorContainer = "#410E0B",
                Success = "#2E6C47",
                OnSuccess = "#FFFFFF",
                SuccessContainer = "#B2F1C5",
                OnSuccessContainer = "#002111",
                Warning = "#7D5700",
                OnWarning = "#FFFFFF",
                WarningContainer = "#FFDEA6",
                OnWarningContainer = "#271900",
                Info = main,
                OnInfo = ColorMath.OnColor(main),
                InfoContainer = ColorMath.Lighten(main, 0.82),
                OnInfoContainer = ColorMath.Darken(main, 0.55),
                Surface = surface,
                OnSurface = Neutral(0.12, 0.12),
                SurfaceVariant = Neutral(0.90),
                OnSurfaceVariant = Neutral(0.30, 0.10),
                SurfaceContainerLow = Neutral(0.96),
                SurfaceContainer = Neutral(0.94),
                SurfaceContainerHigh = Neutral(0.92),
                SurfaceContainerHighest = Neutral(0.90),
                Background = surface,
                OnBackground = Neutral(0.12, 0.12),
                Outline = Neutral(0.50, 0.08),
                OutlineVariant = Neutral(0.80, 0.08),
                InverseSurface = Neutral(0.20, 0.08),
                InverseOnSurface = Neutral(0.95),
                InversePrimary = ColorMath.Lighten(main, 0.40),
                Scrim = "#000000",
                Shadow = "#000000",
                ShadowUmbra = "rgba(0,0,0,0.3)",
                ShadowPenumbra = "rgba(0,0,0,0.15)",
            };
        }

        var dMain = ColorMath.Lighten(main, 0.35);
        var dSurface = seed.Background ?? Neutral(0.10);
        return new ColorScheme
        {
            Primary = dMain,
            OnPrimary = ColorMath.OnColor(dMain),
            PrimaryContainer = ColorMath.Darken(main, 0.30),
            OnPrimaryContainer = ColorMath.Lighten(main, 0.82),
            Secondary = ColorMath.Lighten(secBase, 0.55),
            OnSecondary = "#11161C",
            SecondaryContainer = ColorMath.Darken(secBase, 0.30),
            OnSecondaryContainer = ColorMath.Lighten(secBase, 0.82),
            Tertiary = ColorMath.Lighten(terBase, 0.55),
            OnTertiary = "#11161C",
            TertiaryContainer = ColorMath.Darken(terBase, 0.30),
            OnTertiaryContainer = ColorMath.Lighten(terBase, 0.82),
            Error = "#F2B8B5",
            OnError = "#601410",
            ErrorContainer = "#8C1D18",
            OnErrorContainer = "#F9DEDC",
            Success = "#97D5A9",
            OnSuccess = "#00391E",
            SuccessContainer = "#0F5130",
            OnSuccessContainer = "#B2F1C5",
            Warning = "#F5BC49",
            OnWarning = "#412D00",
            WarningContainer = "#5E4200",
            OnWarningContainer = "#FFDEA6",
            Info = dMain,
            OnInfo = ColorMath.OnColor(dMain),
            InfoContainer = ColorMath.Darken(main, 0.30),
            OnInfoContainer = ColorMath.Lighten(main, 0.82),
            Surface = dSurface,
            OnSurface = Neutral(0.90),
            SurfaceVariant = Neutral(0.25),
            OnSurfaceVariant = Neutral(0.75, 0.08),
            SurfaceContainerLow = Neutral(0.12),
            SurfaceContainer = Neutral(0.14),
            SurfaceContainerHigh = Neutral(0.17),
            SurfaceContainerHighest = Neutral(0.20),
            Background = Neutral(0.08),
            OnBackground = Neutral(0.90),
            Outline = Neutral(0.55, 0.06),
            OutlineVariant = Neutral(0.30, 0.06),
            InverseSurface = Neutral(0.90),
            InverseOnSurface = Neutral(0.15),
            InversePrimary = main,
            Scrim = "#000000",
            Shadow = "#000000",
            ShadowUmbra = "rgba(0,0,0,0.3)",
            ShadowPenumbra = "rgba(0,0,0,0.25)",
        };
    }
}
