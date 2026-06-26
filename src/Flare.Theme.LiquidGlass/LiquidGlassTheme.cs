using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.LiquidGlass;

/// <summary>Liquid Glass theme (frosted "liquid glass"). Light/dark is a mode; colors come from a palette.</summary>
public sealed class LiquidGlassTheme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>

    public const string ThemeId = "liquid-glass";

    /// <inheritdoc />

    public string Id => ThemeId;
    public string DisplayName => "Liquid Glass";
    public DesignTokens Design => LiquidGlassTokens.Design;
    public string DefaultPaletteId => LiquidGlassPalettes.Blue.Id;
    public IReadOnlyList<Palette> Palettes => LiquidGlassPalettes.All;
    public IPaletteGenerator? PaletteGenerator => LiquidGlassRampGenerator.Instance;

    public IReadOnlyList<string> StyleAssets =>
    [
        "_content/Flare.Theme.LiquidGlass/css/liquid-glass-base.css",
        "_content/Flare.Theme.LiquidGlass/css/components/button.css",
        "_content/Flare.Theme.LiquidGlass/css/components/input.css",
        "_content/Flare.Theme.LiquidGlass/css/components/controls.css",
        "_content/Flare.Theme.LiquidGlass/css/components/surfaces.css",
        "_content/Flare.Theme.LiquidGlass/css/components/chrome.css",
        "_content/Flare.Theme.LiquidGlass/css/components/pickers.css",
    ];

    // Dark mode flips the frosted fills and glow to their dark variants.
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => LiquidGlassTokens.DarkExtended;
}

/// <summary>Built-in Liquid Glass palettes (iOS system accent colors).</summary>
public static class LiquidGlassPalettes
{
    /// <summary>Palette id for <c>Blue</c> (<c>liquid-blue</c>); switch palettes without a magic string.</summary>
    public const string BlueId = "liquid-blue";
    /// <summary>Palette id for <c>Purple</c> (<c>liquid-purple</c>); switch palettes without a magic string.</summary>
    public const string PurpleId = "liquid-purple";
    /// <summary>Palette id for <c>Pink</c> (<c>liquid-pink</c>); switch palettes without a magic string.</summary>
    public const string PinkId = "liquid-pink";
    /// <summary>Palette id for <c>Green</c> (<c>liquid-green</c>); switch palettes without a magic string.</summary>
    public const string GreenId = "liquid-green";
    /// <summary>Palette id for <c>Orange</c> (<c>liquid-orange</c>); switch palettes without a magic string.</summary>
    public const string OrangeId = "liquid-orange";
    /// <summary>Palette id for <c>Teal</c> (<c>liquid-teal</c>); switch palettes without a magic string.</summary>
    public const string TealId = "liquid-teal";

    /// <summary>Source label for grouping Liquid Glass palettes in pickers.</summary>
    public const string SourceName = "Liquid Glass";

    /// <summary>The default iOS blue palette -- light + dark.</summary>
    public static readonly Palette Blue = new()
    {
        Id = BlueId,
        Name = "Blue",
        Source = SourceName,
        Light = LiquidGlassTokens.LightColors,
        Dark = LiquidGlassTokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, LiquidGlassTokens.LightColors, LiquidGlassTokens.DarkColors, seed, SourceName);

    /// <summary>iOS purple.</summary>
    public static readonly Palette Purple = Brand(PurpleId, "Purple", "#AF52DE");
    /// <summary>iOS pink.</summary>
    public static readonly Palette Pink = Brand(PinkId, "Pink", "#FF2D55");
    /// <summary>iOS green.</summary>
    public static readonly Palette Green = Brand(GreenId, "Green", "#34C759");
    /// <summary>iOS orange.</summary>
    public static readonly Palette Orange = Brand(OrangeId, "Orange", "#FF9500");
    /// <summary>iOS teal.</summary>
    public static readonly Palette Teal = Brand(TealId, "Teal", "#5AC8FA");

    /// <summary>All built-in Liquid Glass palettes.</summary>
    public static IReadOnlyList<Palette> All => [Blue, Purple, Pink, Green, Teal, Orange];
}
