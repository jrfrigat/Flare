using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.Aero;

/// <summary>
/// Aero palette generator: keeps the cool blue-gray Aero surfaces and derives the brand accent
/// (primary/info) plus a muted tertiary from the seed, so any accent color still reads as
/// "Windows 7 / Office 2010".
/// </summary>
public sealed class AeroRampGenerator : IPaletteGenerator
{
    public static readonly AeroRampGenerator Instance = new();

    public Palette Generate(string id, string name, PaletteSeed seed, string? source = null)
    {
        var main = seed.Main;
        var tertiary = ColorMath.RotateHue(main, -45);

        return new Palette
        {
            Id = id,
            Name = name,
            Source = source,
            Light = AeroTokens.LightColors with
            {
                Primary = main,
                OnPrimary = ColorMath.OnColor(main),
                PrimaryContainer = ColorMath.Lighten(main, 0.80),
                OnPrimaryContainer = ColorMath.Darken(main, 0.50),
                Tertiary = ColorMath.WithLightness(tertiary, 0.42),
                OnTertiary = "#FFFFFF",
                TertiaryContainer = ColorMath.Lighten(tertiary, 0.82),
                OnTertiaryContainer = ColorMath.Darken(tertiary, 0.50),
                Info = main,
                OnInfo = ColorMath.OnColor(main),
                InfoContainer = ColorMath.Lighten(main, 0.80),
                OnInfoContainer = ColorMath.Darken(main, 0.50),
                InversePrimary = ColorMath.Lighten(main, 0.45),
            },
            Dark = AeroTokens.DarkColors with
            {
                Primary = ColorMath.Lighten(main, 0.30),
                OnPrimary = ColorMath.Darken(main, 0.55),
                PrimaryContainer = ColorMath.Darken(main, 0.30),
                OnPrimaryContainer = ColorMath.Lighten(main, 0.82),
                Tertiary = ColorMath.Lighten(tertiary, 0.45),
                OnTertiary = "#11161C",
                TertiaryContainer = ColorMath.Darken(tertiary, 0.30),
                OnTertiaryContainer = ColorMath.Lighten(tertiary, 0.82),
                Info = ColorMath.Lighten(main, 0.30),
                OnInfo = ColorMath.Darken(main, 0.55),
                InfoContainer = ColorMath.Darken(main, 0.30),
                OnInfoContainer = ColorMath.Lighten(main, 0.82),
                InversePrimary = main,
            },
        };
    }
}
