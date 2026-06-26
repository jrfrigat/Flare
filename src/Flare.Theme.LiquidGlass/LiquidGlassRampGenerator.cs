using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.LiquidGlass;

/// <summary>
/// Liquid Glass palette generator: keeps the iOS system surfaces/frosted fills and derives the accent
/// (primary/info) plus a muted tertiary from the seed, so any accent still reads as "iOS".
/// </summary>
public sealed class LiquidGlassRampGenerator : IPaletteGenerator
{
    public static readonly LiquidGlassRampGenerator Instance = new();

    public Palette Generate(string id, string name, PaletteSeed seed, string? source = null)
    {
        var main = seed.Main;
        var tertiary = ColorMath.RotateHue(main, 40);

        return new Palette
        {
            Id = id,
            Name = name,
            Source = source,
            Light = LiquidGlassTokens.LightColors with
            {
                Primary = main,
                OnPrimary = ColorMath.OnColor(main),
                PrimaryContainer = ColorMath.Lighten(main, 0.82),
                OnPrimaryContainer = ColorMath.Darken(main, 0.50),
                Tertiary = ColorMath.WithLightness(tertiary, 0.55),
                OnTertiary = "#FFFFFF",
                TertiaryContainer = ColorMath.Lighten(tertiary, 0.84),
                OnTertiaryContainer = ColorMath.Darken(tertiary, 0.50),
                Info = main,
                OnInfo = ColorMath.OnColor(main),
                InfoContainer = ColorMath.Lighten(main, 0.82),
                OnInfoContainer = ColorMath.Darken(main, 0.50),
                InversePrimary = ColorMath.Lighten(main, 0.45),
            },
            Dark = LiquidGlassTokens.DarkColors with
            {
                Primary = ColorMath.Lighten(main, 0.12),
                OnPrimary = "#FFFFFF",
                PrimaryContainer = ColorMath.Darken(main, 0.35),
                OnPrimaryContainer = ColorMath.Lighten(main, 0.82),
                Tertiary = ColorMath.Lighten(tertiary, 0.18),
                OnTertiary = ColorMath.Darken(tertiary, 0.55),
                TertiaryContainer = ColorMath.Darken(tertiary, 0.35),
                OnTertiaryContainer = ColorMath.Lighten(tertiary, 0.82),
                Info = ColorMath.Lighten(main, 0.12),
                OnInfo = "#FFFFFF",
                InfoContainer = ColorMath.Darken(main, 0.35),
                OnInfoContainer = ColorMath.Lighten(main, 0.82),
                InversePrimary = main,
            },
        };
    }
}
