using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Theme.MaterialDesign3Expressive;

/// <summary>Material Design 3 Expressive theme (design tokens). Light/dark is a mode; colors come from a palette.</summary>
public sealed class Md3Theme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>

    public const string ThemeId = "md3-expressive";

    /// <inheritdoc />

    public string Id => ThemeId;
    public string DisplayName => "Material Design 3 Expressive";
    public DesignTokens Design => MaterialDesignTokens.Design with
    {
        // SEPARATED button group (Expressive): a real 2dp gap, no overlap, rounded interior corners and
        // full-capsule ends. Purely a token bundle - the base buttongroup.css is untouched (no override).
        ButtonGroup = new ButtonGroupTokens
        {
            Gap = "0.125rem",
            Overlap = "0",
            OuterRadius = "calc(var(--_flare-btn-height, var(--flare-btn-height-md, 3rem)) / 2)",
            InnerRadius = "var(--flare-shape-small)",
            ZActive = "1",
        },
    };
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
        Light = MaterialDesignTokens.LightColors,
        Dark = MaterialDesignTokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, MaterialDesignTokens.LightColors, MaterialDesignTokens.DarkColors, seed, SourceName);

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
