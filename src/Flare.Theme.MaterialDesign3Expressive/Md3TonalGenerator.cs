using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.MaterialDesign3Expressive;

/// <summary>
/// Material Design 3 palette generator: derives roles from tonal palettes (key color -> tones),
/// approximating HCT with HSL. Primary/secondary/tertiary come from the brand hue at fixed tones;
/// surfaces use a low-chroma neutral palette tinted by the brand hue. Light maps to mid tones,
/// dark to high tones, matching the MD3 scheme.
/// </summary>
public sealed class Md3TonalGenerator : IPaletteGenerator
{
    public static readonly Md3TonalGenerator Instance = new();

    public Palette Generate(string id, string name, PaletteSeed seed, string? source = null)
    {
        var (h, s, _) = ColorMath.ToHsl(seed.Main);
        double cP = Math.Clamp(Math.Max(s, 0.55), 0, 1);     // vivid primary
        double cS = cP * 0.40;                               // secondary: muted
        double cT = cP * 0.65;                               // tertiary: hue + 60
        double nHue = ColorMath.ToHsl(seed.Background ?? seed.Main).H;

        string P(double t) => ColorMath.FromHsl(h, cP, t / 100.0);
        string Sc(double t) => ColorMath.FromHsl(h, cS, t / 100.0);
        string T(double t) => ColorMath.FromHsl(h + 60, cT, t / 100.0);
        string N(double t) => ColorMath.FromHsl(nHue, 0.04, t / 100.0);
        string NV(double t) => ColorMath.FromHsl(nHue, 0.08, t / 100.0);

        return new Palette
        {
            Id = id,
            Name = name,
            Source = source,
            Light = new ColorScheme
            {
                Primary = P(40),
                OnPrimary = P(100),
                PrimaryContainer = P(90),
                // Updated MD3 (Expressive) color spec: light on-*-container roles use tone 30 (was tone 10).
                OnPrimaryContainer = P(30),
                Secondary = Sc(40),
                OnSecondary = Sc(100),
                SecondaryContainer = Sc(90),
                OnSecondaryContainer = Sc(30),
                Tertiary = T(40),
                OnTertiary = T(100),
                TertiaryContainer = T(90),
                OnTertiaryContainer = T(30),
                Error = "#B3261E",
                OnError = "#FFFFFF",
                ErrorContainer = "#F9DEDC",
                OnErrorContainer = "#8C1D18",
                Success = "#2E6C47",
                OnSuccess = "#FFFFFF",
                SuccessContainer = "#B2F1C5",
                OnSuccessContainer = "#002111",
                Warning = "#7D5700",
                OnWarning = "#FFFFFF",
                WarningContainer = "#FFDEA6",
                OnWarningContainer = "#271900",
                Info = P(40),
                OnInfo = P(100),
                InfoContainer = P(90),
                OnInfoContainer = P(30),
                Surface = N(98),
                OnSurface = N(10),
                SurfaceVariant = NV(90),
                OnSurfaceVariant = NV(30),
                OnSurfaceVariant2 = NV(40),
                SurfaceContainerLow = N(96),
                SurfaceContainer = N(94),
                SurfaceContainerHigh = N(92),
                SurfaceContainerHighest = N(90),
                Background = N(98),
                OnBackground = N(10),
                Outline = NV(50),
                OutlineVariant = NV(80),
                InverseSurface = N(20),
                InverseOnSurface = N(95),
                InversePrimary = P(80),
                Scrim = "#000000",
                Shadow = "#000000",
                ShadowUmbra = "rgba(0,0,0,0.3)",
                ShadowPenumbra = "rgba(0,0,0,0.15)",
            },
            Dark = new ColorScheme
            {
                Primary = P(80),
                OnPrimary = P(20),
                PrimaryContainer = P(30),
                OnPrimaryContainer = P(90),
                Secondary = Sc(80),
                OnSecondary = Sc(20),
                SecondaryContainer = Sc(30),
                OnSecondaryContainer = Sc(90),
                Tertiary = T(80),
                OnTertiary = T(20),
                TertiaryContainer = T(30),
                OnTertiaryContainer = T(90),
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
                Info = P(80),
                OnInfo = P(20),
                InfoContainer = P(30),
                OnInfoContainer = P(90),
                Surface = N(6),
                OnSurface = N(90),
                SurfaceVariant = NV(30),
                OnSurfaceVariant = NV(80),
                OnSurfaceVariant2 = NV(70),
                SurfaceContainerLow = N(10),
                SurfaceContainer = N(12),
                SurfaceContainerHigh = N(17),
                SurfaceContainerHighest = N(22),
                Background = N(6),
                OnBackground = N(90),
                Outline = NV(60),
                OutlineVariant = NV(30),
                InverseSurface = N(90),
                InverseOnSurface = N(20),
                InversePrimary = P(40),
                Scrim = "#000000",
                Shadow = "#000000",
                ShadowUmbra = "rgba(0,0,0,0.3)",
                ShadowPenumbra = "rgba(0,0,0,0.15)",
            },
        };
    }
}
