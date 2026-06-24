using Flare.Core.Services;

namespace Flare.Core.Tokens;

/// <summary>
/// Builds a branded <see cref="Palette"/> by overlaying a single brand seed onto a base
/// color scheme: the primary/info roles take the brand color (with derived containers and
/// on-colors), while neutrals/surfaces and the other accents stay from the base so the
/// palette fits its design system. One seed yields both light and dark schemes.
/// </summary>
public static class PaletteFactory
{
    /// <summary>Brand.</summary>
    public static Palette Brand(string id, string name, ColorScheme baseLight, ColorScheme baseDark, string seed, string? source = null)
    {
        var darkSeed = ColorMath.Lighten(seed, 0.35); // brighter brand on dark surfaces
        return new Palette
        {
            Id = id,
            Name = name,
            Source = source,
            Light = baseLight with
            {
                Primary = seed,
                OnPrimary = ColorMath.OnColor(seed),
                PrimaryContainer = ColorMath.Lighten(seed, 0.82),
                OnPrimaryContainer = ColorMath.Darken(seed, 0.55),
                Info = seed,
                OnInfo = ColorMath.OnColor(seed),
                InfoContainer = ColorMath.Lighten(seed, 0.82),
                OnInfoContainer = ColorMath.Darken(seed, 0.55),
            },
            Dark = baseDark with
            {
                Primary = darkSeed,
                OnPrimary = ColorMath.OnColor(darkSeed),
                PrimaryContainer = ColorMath.Darken(seed, 0.30),
                OnPrimaryContainer = ColorMath.Lighten(seed, 0.82),
                Info = darkSeed,
                OnInfo = ColorMath.OnColor(darkSeed),
                InfoContainer = ColorMath.Darken(seed, 0.30),
                OnInfoContainer = ColorMath.Lighten(seed, 0.82),
            },
        };
    }
}
