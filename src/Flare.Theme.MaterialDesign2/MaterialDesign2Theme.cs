using Flare.Abstractions;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.MaterialDesign2;

/// <summary>
/// Material Design 2 theme (design tokens). The classic pre-Material-You system: a purple/teal
/// baseline palette, the MD2 type scale, 4dp rectangular shapes, dp box-shadow elevation and
/// uppercase contained buttons. Light/dark is a mode; colors come from a palette.
/// </summary>
public sealed class MaterialDesign2Theme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>
    public const string ThemeId = "md2";

    /// <inheritdoc />
    public string Id => ThemeId;

    /// <inheritdoc />
    public string DisplayName => "Material Design 2";

    /// <inheritdoc />
    public DesignTokens Design => MaterialDesign2Tokens.Design;

    /// <inheritdoc />
    public string DefaultPaletteId => Md2Palettes.PurpleId;

    /// <inheritdoc />
    public IReadOnlyList<Palette> Palettes => Md2Palettes.All;

    /// <inheritdoc />
    public IPaletteGenerator? PaletteGenerator => MaterialDesign2RampGenerator.Instance;

    /// <inheritdoc />
    public IReadOnlyList<string> StyleAssets =>
    [
        "https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap",
        "_content/Flare.Theme.MaterialDesign2/css/md2-base.css",
    ];
}

/// <summary>Built-in Material Design 2 palettes (classic Material color swatches).</summary>
public static class Md2Palettes
{
    /// <summary>Palette id for <c>Purple</c> (<c>md2-purple</c>); the MD2 baseline.</summary>
    public const string PurpleId = "md2-purple";
    /// <summary>Palette id for <c>Indigo</c> (<c>md2-indigo</c>).</summary>
    public const string IndigoId = "md2-indigo";
    /// <summary>Palette id for <c>Teal</c> (<c>md2-teal</c>).</summary>
    public const string TealId = "md2-teal";
    /// <summary>Palette id for <c>Blue</c> (<c>md2-blue</c>).</summary>
    public const string BlueId = "md2-blue";
    /// <summary>Palette id for <c>Pink</c> (<c>md2-pink</c>).</summary>
    public const string PinkId = "md2-pink";
    /// <summary>Palette id for <c>Green</c> (<c>md2-green</c>).</summary>
    public const string GreenId = "md2-green";

    /// <summary>Source label for grouping Material Design 2 palettes in pickers.</summary>
    public const string SourceName = "Material Design 2";

    /// <summary>The MD2 baseline (purple) palette -- light + dark.</summary>
    public static readonly Palette Purple = new()
    {
        Id = PurpleId,
        Name = "Purple",
        Source = SourceName,
        Light = MaterialDesign2Tokens.LightColors,
        Dark = MaterialDesign2Tokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, MaterialDesign2Tokens.LightColors, MaterialDesign2Tokens.DarkColors, seed, SourceName);

    /// <summary>Material indigo.</summary>
    public static readonly Palette Indigo = Brand(IndigoId, "Indigo", "#3F51B5");
    /// <summary>Material teal.</summary>
    public static readonly Palette Teal = Brand(TealId, "Teal", "#009688");
    /// <summary>Material blue.</summary>
    public static readonly Palette Blue = Brand(BlueId, "Blue", "#2196F3");
    /// <summary>Material pink.</summary>
    public static readonly Palette Pink = Brand(PinkId, "Pink", "#E91E63");
    /// <summary>Material green.</summary>
    public static readonly Palette Green = Brand(GreenId, "Green", "#4CAF50");

    /// <summary>All built-in MD2 palettes.</summary>
    public static IReadOnlyList<Palette> All => [Purple, Indigo, Teal, Blue, Pink, Green];
}
