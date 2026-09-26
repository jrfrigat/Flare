using Flare.Theming;

namespace Flare.Abstractions.Tokens;

/// <summary>
/// Builds a branded <see cref="Palette"/> by overlaying a single brand seed onto a base
/// color scheme: the primary/info roles take the brand color (with derived containers and
/// on-colors), while neutrals/surfaces and the other accents stay from the base so the
/// palette fits its design system. One seed yields both light and dark schemes.
/// </summary>
public static class PaletteFactory
{
    /// <summary>
    /// Generates a full palette (light + dark) from one or two colors using the core generator.
    /// <paramref name="main"/> drives the brand roles; optional <paramref name="background"/>
    /// tints the surfaces/neutrals.
    /// </summary>
    public static Palette FromColors(string id, string name, string main, string? background = null, string? source = null) =>
        DefaultPaletteGenerator.Instance.Generate(id, name, new PaletteSeed(main, background), source);

    /// <summary>
    /// Re-derives the primary fixed roles of both schemes from the palette's own primary roles. A palette
    /// that takes a base scheme and overrides only its primary roles would otherwise keep the base
    /// palette's primary in its fixed roles. The fixed fill becomes the light primary container, the dim
    /// step the dark primary, and the text on both the light on-primary-container.
    /// </summary>
    public static Palette WithPrimaryFixedFromPrimary(this Palette palette)
    {
        var light = palette.Light;
        var fixedFill = light.PrimaryContainer;
        var fixedDim = palette.Dark.Primary;
        var onFixed = light.OnPrimaryContainer;
        return palette with
        {
            Light = light with
            {
                PrimaryFixed = fixedFill,
                PrimaryFixedDim = fixedDim,
                OnPrimaryFixed = onFixed,
                OnPrimaryFixedVariant = onFixed,
            },
            Dark = palette.Dark with
            {
                PrimaryFixed = fixedFill,
                PrimaryFixedDim = fixedDim,
                OnPrimaryFixed = onFixed,
                OnPrimaryFixedVariant = onFixed,
            },
        };
    }

    /// <summary>
    /// Re-derives the tertiary fixed roles of both schemes from the palette's own tertiary roles, the
    /// tertiary counterpart of <see cref="WithPrimaryFixedFromPrimary"/> for a palette that overrides the
    /// tertiary roles of its base scheme.
    /// </summary>
    public static Palette WithTertiaryFixedFromTertiary(this Palette palette)
    {
        var light = palette.Light;
        var fixedFill = light.TertiaryContainer;
        var fixedDim = palette.Dark.Tertiary;
        var onFixed = light.OnTertiaryContainer;
        return palette with
        {
            Light = light with
            {
                TertiaryFixed = fixedFill,
                TertiaryFixedDim = fixedDim,
                OnTertiaryFixed = onFixed,
                OnTertiaryFixedVariant = onFixed,
            },
            Dark = palette.Dark with
            {
                TertiaryFixed = fixedFill,
                TertiaryFixedDim = fixedDim,
                OnTertiaryFixed = onFixed,
                OnTertiaryFixedVariant = onFixed,
            },
        };
    }

    /// <summary>Brand.</summary>
    public static Palette Brand(string id, string name, ColorScheme baseLight, ColorScheme baseDark, string seed, string? source = null)
    {
        var darkSeed = ColorMath.Lighten(seed, 0.35); // brighter brand on dark surfaces
        return WithPrimaryFixedFromPrimary(new Palette
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
        });
    }
}
