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
        "_content/Flare.Theme.MaterialDesign2/css/components.css",
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

    // Every built-in palette is written out from the Material 2 colour swatches rather than derived from a
    // seed. Light: primary 500, container 700 (primaryVariant), secondary A200 of a second swatch, its
    // container and tertiary 700, tertiary container 100, inverse primary 200. Dark: primary and secondary
    // 200, primary container 700, secondary and tertiary containers 900 over 100, inverse primary 900.
    // Each on-colour is white when it reaches 4.5:1, otherwise the higher-contrast of black and white.
    /// <summary>Material indigo 500 with a pink accent.</summary>
    public static readonly Palette Indigo = new()
    {
        Id = IndigoId,
        Name = "Indigo",
        Source = SourceName,
        Light = MaterialDesign2Tokens.LightColors with
        {
            Primary = "#3F51B5",
            OnPrimary = "#FFFFFF",
            PrimaryContainer = "#303F9F",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#FF4081",
            OnSecondary = "#000000",
            SecondaryContainer = "#C2185B",
            OnSecondaryContainer = "#FFFFFF",
            Tertiary = "#C2185B",
            OnTertiary = "#FFFFFF",
            TertiaryContainer = "#F8BBD0",
            OnTertiaryContainer = "#880E4F",
            InversePrimary = "#9FA8DA",
        },
        Dark = MaterialDesign2Tokens.DarkColors with
        {
            Primary = "#9FA8DA",
            OnPrimary = "#000000",
            PrimaryContainer = "#303F9F",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#F48FB1",
            OnSecondary = "#000000",
            SecondaryContainer = "#880E4F",
            OnSecondaryContainer = "#F8BBD0",
            Tertiary = "#F48FB1",
            OnTertiary = "#000000",
            TertiaryContainer = "#880E4F",
            OnTertiaryContainer = "#F8BBD0",
            InversePrimary = "#1A237E",
        },
    };

    /// <summary>Material teal 500 with a deep purple accent.</summary>
    public static readonly Palette Teal = new()
    {
        Id = TealId,
        Name = "Teal",
        Source = SourceName,
        Light = MaterialDesign2Tokens.LightColors with
        {
            Primary = "#009688",
            OnPrimary = "#000000",
            PrimaryContainer = "#00796B",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#7C4DFF",
            OnSecondary = "#FFFFFF",
            SecondaryContainer = "#512DA8",
            OnSecondaryContainer = "#FFFFFF",
            Tertiary = "#512DA8",
            OnTertiary = "#FFFFFF",
            TertiaryContainer = "#D1C4E9",
            OnTertiaryContainer = "#311B92",
            InversePrimary = "#80CBC4",
        },
        Dark = MaterialDesign2Tokens.DarkColors with
        {
            Primary = "#80CBC4",
            OnPrimary = "#000000",
            PrimaryContainer = "#00796B",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#B39DDB",
            OnSecondary = "#000000",
            SecondaryContainer = "#311B92",
            OnSecondaryContainer = "#D1C4E9",
            Tertiary = "#B39DDB",
            OnTertiary = "#000000",
            TertiaryContainer = "#311B92",
            OnTertiaryContainer = "#D1C4E9",
            InversePrimary = "#004D40",
        },
    };

    /// <summary>Material blue 500 with a purple accent.</summary>
    public static readonly Palette Blue = new()
    {
        Id = BlueId,
        Name = "Blue",
        Source = SourceName,
        Light = MaterialDesign2Tokens.LightColors with
        {
            Primary = "#2196F3",
            OnPrimary = "#000000",
            PrimaryContainer = "#1976D2",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#E040FB",
            OnSecondary = "#000000",
            SecondaryContainer = "#7B1FA2",
            OnSecondaryContainer = "#FFFFFF",
            Tertiary = "#7B1FA2",
            OnTertiary = "#FFFFFF",
            TertiaryContainer = "#E1BEE7",
            OnTertiaryContainer = "#4A148C",
            InversePrimary = "#90CAF9",
        },
        Dark = MaterialDesign2Tokens.DarkColors with
        {
            Primary = "#90CAF9",
            OnPrimary = "#000000",
            PrimaryContainer = "#1976D2",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#CE93D8",
            OnSecondary = "#000000",
            SecondaryContainer = "#4A148C",
            OnSecondaryContainer = "#E1BEE7",
            Tertiary = "#CE93D8",
            OnTertiary = "#000000",
            TertiaryContainer = "#4A148C",
            OnTertiaryContainer = "#E1BEE7",
            InversePrimary = "#0D47A1",
        },
    };

    /// <summary>Material pink 500 with an indigo accent.</summary>
    public static readonly Palette Pink = new()
    {
        Id = PinkId,
        Name = "Pink",
        Source = SourceName,
        Light = MaterialDesign2Tokens.LightColors with
        {
            Primary = "#E91E63",
            OnPrimary = "#000000",
            PrimaryContainer = "#C2185B",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#536DFE",
            OnSecondary = "#000000",
            SecondaryContainer = "#303F9F",
            OnSecondaryContainer = "#FFFFFF",
            Tertiary = "#303F9F",
            OnTertiary = "#FFFFFF",
            TertiaryContainer = "#C5CAE9",
            OnTertiaryContainer = "#1A237E",
            InversePrimary = "#F48FB1",
        },
        Dark = MaterialDesign2Tokens.DarkColors with
        {
            Primary = "#F48FB1",
            OnPrimary = "#000000",
            PrimaryContainer = "#C2185B",
            OnPrimaryContainer = "#FFFFFF",
            Secondary = "#9FA8DA",
            OnSecondary = "#000000",
            SecondaryContainer = "#1A237E",
            OnSecondaryContainer = "#C5CAE9",
            Tertiary = "#9FA8DA",
            OnTertiary = "#000000",
            TertiaryContainer = "#1A237E",
            OnTertiaryContainer = "#C5CAE9",
            InversePrimary = "#880E4F",
        },
    };

    /// <summary>Material green 500 with a pink accent.</summary>
    public static readonly Palette Green = new()
    {
        Id = GreenId,
        Name = "Green",
        Source = SourceName,
        Light = MaterialDesign2Tokens.LightColors with
        {
            Primary = "#4CAF50",
            OnPrimary = "#000000",
            PrimaryContainer = "#388E3C",
            OnPrimaryContainer = "#000000",
            Secondary = "#FF4081",
            OnSecondary = "#000000",
            SecondaryContainer = "#C2185B",
            OnSecondaryContainer = "#FFFFFF",
            Tertiary = "#C2185B",
            OnTertiary = "#FFFFFF",
            TertiaryContainer = "#F8BBD0",
            OnTertiaryContainer = "#880E4F",
            InversePrimary = "#A5D6A7",
        },
        Dark = MaterialDesign2Tokens.DarkColors with
        {
            Primary = "#A5D6A7",
            OnPrimary = "#000000",
            PrimaryContainer = "#388E3C",
            OnPrimaryContainer = "#000000",
            Secondary = "#F48FB1",
            OnSecondary = "#000000",
            SecondaryContainer = "#880E4F",
            OnSecondaryContainer = "#F8BBD0",
            Tertiary = "#F48FB1",
            OnTertiary = "#000000",
            TertiaryContainer = "#880E4F",
            OnTertiaryContainer = "#F8BBD0",
            InversePrimary = "#1B5E20",
        },
    };

    /// <summary>All built-in MD2 palettes.</summary>
    public static IReadOnlyList<Palette> All => [Purple, Indigo, Teal, Blue, Pink, Green];
}
