using Flare.Abstractions.Tokens;
using Flare.Theming;

namespace Flare.Theme.MaterialDesign3.Tokens;

/// <summary>Built-in Material Design 3 palettes.</summary>
public static class Md3Palettes
{
    /// <summary>Palette id for <c>Violet</c> (<c>md3-violet</c>); switch palettes without a magic string.</summary>
    public const string VioletId = "md3-violet";
    /// <summary>Palette id for <c>Blue</c> (<c>md3-blue</c>); switch palettes without a magic string.</summary>
    public const string BlueId = "md3-blue";
    /// <summary>Palette id for <c>Green</c> (<c>md3-green</c>); switch palettes without a magic string.</summary>
    public const string GreenId = "md3-green";
    /// <summary>Palette id for <c>Teal</c> (<c>md3-teal</c>); switch palettes without a magic string.</summary>
    public const string TealId = "md3-teal";
    /// <summary>Palette id for <c>Orange</c> (<c>md3-orange</c>); switch palettes without a magic string.</summary>
    public const string OrangeId = "md3-orange";

    /// <summary>Source label for grouping Material palettes in pickers.</summary>
    public const string SourceName = "Material Design 3";

    /// <summary>The MD3 baseline (violet) palette -- light + dark.</summary>
    public static readonly Palette Violet = new()
    {
        Id = VioletId,
        Name = "Violet",
        Source = SourceName,
        Light = MaterialDesign3Tokens.LightColors,
        Dark = MaterialDesign3Tokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, MaterialDesign3Tokens.LightColors, MaterialDesign3Tokens.DarkColors, seed, SourceName);

    /// <summary>Material blue.</summary>
    public static readonly Palette Blue = Brand(BlueId, "Blue", "#0B57D0");
    /// <summary>Material green.</summary>
    public static readonly Palette Green = Brand(GreenId, "Green", "#1E8E3E");
    /// <summary>Material teal.</summary>
    public static readonly Palette Teal = Brand(TealId, "Teal", "#00897B");
    /// <summary>Material orange.</summary>
    public static readonly Palette Orange = Brand(OrangeId, "Orange", "#C2410C");

    /// <summary>All built-in MD3 palettes.</summary>
    public static IReadOnlyList<Palette> All => [Violet, Blue, Green, Teal, Orange];
}
