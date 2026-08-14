using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Theme.MaterialDesign3Expressive;

/// <summary>Material Design 3 Expressive theme (design tokens). Light/dark is a mode; colors come from a palette.</summary>
public sealed class MaterialDesign3ExpressiveTheme : ITheme
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
            // Standard: separate pills, and the space between them TIGHTENS as the buttons grow -
            // `md.comp.button-group.standard.<size>.between-space` reads 18/12/8/8/8dp, which is not the
            // ramp anyone would guess. The gap is also what a press spends: the spec's grow takes its
            // 15% from the neighbours, and a small button needs more room around it to do that without
            // disturbing the layout, which is why the ramp runs the way it does.
            StandardGapXs = "1.125rem", // 18dp
            StandardGapSm = "0.75rem",  // 12dp
            StandardGapMd = "0.5rem",   // 8dp
            StandardGapLg = "0.5rem",   // 8dp
            StandardGapXl = "0.5rem",   // 8dp
            // Connected: the spec's 2dp seam at every size, capsule ends, and interior corners that
            // ramp with the size. A selected segment goes fully round - the spec's "selected inner
            // corner size = 50%" - which is the same capsule arithmetic the ends use.
            ConnectedGap = "0.125rem", // 2dp
            ConnectedOverlap = "0",
            ConnectedOuterRadius = "calc(var(--_flare-btn-height, var(--flare-btn-height-md, 3rem)) / 2)",
            ConnectedSelectedRadius = "calc(var(--_flare-btn-height, var(--flare-btn-height-md, 3rem)) / 2)",
            ConnectedInnerRadiusXs = "0.5rem",  // 8dp
            ConnectedInnerRadiusSm = "0.5rem",  // 8dp
            ConnectedInnerRadiusMd = "0.5rem",  // 8dp
            ConnectedInnerRadiusLg = "1rem",    // 16dp
            ConnectedInnerRadiusXl = "1.25rem", // 20dp
            // A pressed segment tightens further, and by the GROUP's own numbers rather than the lone
            // button's: `connected.<size>.pressed.inner-corner` is 4/4/4/12/16dp where a button on its
            // own presses to 8/8/12/16/16dp. Being part of a control changes what a press looks like.
            ConnectedPressedRadiusXs = "0.25rem", // 4dp
            ConnectedPressedRadiusSm = "0.25rem", // 4dp
            ConnectedPressedRadiusMd = "0.25rem", // 4dp
            ConnectedPressedRadiusLg = "0.75rem", // 12dp
            ConnectedPressedRadiusXl = "1rem",    // 16dp
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
