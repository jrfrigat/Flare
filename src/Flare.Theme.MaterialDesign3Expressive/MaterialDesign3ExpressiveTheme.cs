using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.MaterialDesign3Expressive;

/// <summary>Material Design 3 Expressive theme (design tokens). Light/dark is a mode; colors come from a palette.</summary>
public sealed class Md3Theme : ITheme
{
    public string Id => "md3-expressive";
    public string DisplayName => "Material Design 3 Expressive";
    public DesignTokens Design => MaterialDesignTokens.Design;
    public string DefaultPaletteId => Md3Palettes.Violet.Id;
    public IReadOnlyList<Palette> Palettes => Md3Palettes.All;
    public IPaletteGenerator? PaletteGenerator => Md3TonalGenerator.Instance;

    public IReadOnlyList<string> StyleAssets =>
    [
        "https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap",
        "_content/Flare.Theme.MaterialDesign3Expressive/css/md3-base.css",
        "_content/Flare.Theme.MaterialDesign3Expressive/css/components/button.css",
        "_content/Flare.Theme.MaterialDesign3Expressive/css/components/split-button.css",
        "_content/Flare.Theme.MaterialDesign3Expressive/css/components/button-group.css",
    ];
}

/// <summary>Built-in Material Design 3 palettes.</summary>
public static class Md3Palettes
{
    /// <summary>Source label for grouping Material palettes in pickers.</summary>
    public const string SourceName = "Material Design 3";

    /// <summary>The MD3 baseline (violet) palette -- light + dark.</summary>
    public static readonly Palette Violet = new()
    {
        Id = "md3-violet",
        Name = "Violet",
        Source = SourceName,
        Light = MaterialDesignTokens.LightColors,
        Dark = MaterialDesignTokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, MaterialDesignTokens.LightColors, MaterialDesignTokens.DarkColors, seed, SourceName);

    /// <summary>Material blue.</summary>
    public static readonly Palette Blue = Brand("md3-blue", "Blue", "#0B57D0");
    /// <summary>Material green.</summary>
    public static readonly Palette Green = Brand("md3-green", "Green", "#1E8E3E");
    /// <summary>Material teal.</summary>
    public static readonly Palette Teal = Brand("md3-teal", "Teal", "#00897B");
    /// <summary>Material orange.</summary>
    public static readonly Palette Orange = Brand("md3-orange", "Orange", "#C2410C");

    /// <summary>All built-in MD3 palettes.</summary>
    public static IReadOnlyList<Palette> All => [Violet, Blue, Green, Teal, Orange];
}
