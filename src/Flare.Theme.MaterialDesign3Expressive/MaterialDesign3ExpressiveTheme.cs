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

    private static readonly IReadOnlyDictionary<string, string> Extended =
        new Dictionary<string, string>(MaterialDesignTokens.Extended)
        {
            [Css.Tokens.Md3e.Progress.Length] = "40px",
            [Css.Tokens.Md3e.Progress.IndeterminateLength] = "20px",
            [Css.Tokens.Md3e.Progress.Amplitude] = "3px",
            [Css.Tokens.Md3e.Progress.Speed] = "1s",
            [Css.Tokens.Md3e.Progress.RingMask] = RingWaveMask,
        };

    // Seven waves of amplitude 4 around a mean radius of 41, stroked 10 wide, in a 100-unit box.
    // progress.css sets the ring's stroke to 18% = 2 * (4 + 10 / 2), which is what makes the core's
    // radius, (100% - 18%) / 2, come out at the 41% this path is drawn on.
    private const string RingWaveMask =
        "url(\"data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'%3E"
        + "%3Cpath d='M91,50L92.2,51.9L93.2,53.9L93.8,55.9L94.1,58L93.9,60L93.2,61.9L92.1,63.7L90.6,65.2"
        + "L88.8,66.6L86.9,67.8L85,68.8L83.2,69.8L81.5,70.8L80.1,71.9L78.9,73.1L78,74.5L77.3,76.1L76.7,77.9"
        + "L76.2,79.9L75.6,82.1L74.8,84.2L73.9,86.2L72.7,88L71.2,89.5L69.5,90.5L67.6,91.2L65.5,91.4L63.4,91.2"
        + "L61.2,90.7L59.1,90L57.1,89.1L55.2,88.3L53.4,87.6L51.7,87.2L50,87L48.3,87.2L46.6,87.6L44.8,88.3"
        + "L42.9,89.1L40.9,90L38.8,90.7L36.6,91.2L34.5,91.4L32.4,91.2L30.5,90.5L28.8,89.5L27.3,88L26.1,86.2"
        + "L25.2,84.2L24.4,82.1L23.8,79.9L23.3,77.9L22.7,76.1L22,74.5L21.1,73.1L19.9,71.9L18.5,70.8L16.8,69.8"
        + "L15,68.8L13.1,67.8L11.2,66.6L9.4,65.2L7.9,63.7L6.8,61.9L6.1,60L5.9,58L6.2,55.9L6.8,53.9L7.8,51.9"
        + "L9,50L10.3,48.2L11.5,46.5L12.6,44.9L13.4,43.4L13.9,41.8L14.1,40.1L14.1,38.3L13.8,36.4L13.4,34.4"
        + "L13.1,32.2L12.8,30L12.8,27.8L13.1,25.6L13.8,23.7L14.8,21.9L16.3,20.5L18,19.4L20,18.7L22.2,18.2"
        + "L24.4,17.9L26.6,17.8L28.7,17.7L30.6,17.6L32.4,17.2L33.9,16.7L35.4,15.8L36.7,14.6L38.1,13.2"
        + "L39.4,11.7L40.9,10L42.5,8.4L44.2,7L46,5.9L48,5.2L50,5L52,5.2L54,5.9L55.8,7L57.5,8.4L59.1,10"
        + "L60.6,11.7L61.9,13.2L63.3,14.6L64.6,15.8L66.1,16.7L67.6,17.2L69.4,17.6L71.3,17.7L73.4,17.8"
        + "L75.6,17.9L77.8,18.2L80,18.7L82,19.4L83.7,20.5L85.2,21.9L86.2,23.7L86.9,25.6L87.2,27.8L87.2,30"
        + "L86.9,32.2L86.6,34.4L86.2,36.4L85.9,38.3L85.9,40.1L86.1,41.8L86.6,43.4L87.4,44.9L88.5,46.5"
        + "L89.7,48.2Z' fill='none' stroke='%23fff' stroke-width='10'/%3E%3C/svg%3E\")";

    /// <inheritdoc />

    public string Id => ThemeId;
    public string DisplayName => "Material Design 3 Expressive";
    public DesignTokens Design => MaterialDesignTokens.Design with
    {
        // THE EXPRESSIVE SIZE RAMP. Baseline M3 has one button - "small", 40dp tall - and the XS/M/L/XL
        // steps arrive with Expressive, which is why they are stated here rather than in the shared
        // Material bundle: the two eras genuinely disagree about how big a button is, and the baseline
        // theme keeps its own gentler ramp.
        //
        // The numbers are `md.comp.button.<size>.*` read straight off the table, and the top of the
        // ramp is meant to look startling: an extra-large Expressive button is 136dp tall with a 32pt
        // label, a display-scale control for one hero action per screen rather than a slightly bigger
        // button. Flare used to compress the whole ramp into 32-64dp, which made large and extra-large
        // near-duplicates of medium and quietly threw away the size axis Expressive exists to offer.
        Button = MaterialDesignTokens.Design.Button with
        {
            HeightXs = "2rem",     // 32dp
            HeightSm = "2.5rem",   // 40dp
            HeightMd = "3.5rem",   // 56dp
            HeightLg = "6rem",     // 96dp
            HeightXl = "8.5rem",   // 136dp

            // Leading and trailing space, which the spec ramps far harder than the heights do.
            PaddingInlineXs = "0.75rem", // 12dp
            PaddingInlineSm = "1rem",    // 16dp
            PaddingInlineMd = "1.5rem",  // 24dp
            PaddingInlineLg = "3rem",    // 48dp
            PaddingInlineXl = "4rem",    // 64dp

            // Space between icon and label.
            GapXs = "0.5rem",  // 8dp
            GapSm = "0.5rem",  // 8dp
            GapMd = "0.5rem",  // 8dp
            GapLg = "0.75rem", // 12dp
            GapXl = "1rem",    // 16dp

            // `md.comp.button.<size>.outlined.outline.width` - the stroke thickens with the container.
            OutlineWidthXs = "1px",
            OutlineWidthSm = "1px",
            OutlineWidthMd = "1px",
            OutlineWidthLg = "2px",
            OutlineWidthXl = "3px",
        },
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
        Progress = MaterialDesignTokens.Design.Progress with
        {
            LinearIndeterminateDuration = "1750ms",
            LinearIndeterminateEasing = "cubic-bezier(0.3, 0, 0.8, 0.15)",
            CircularIndeterminateRotationDuration = "1500ms",
            CircularIndeterminateProgressDuration = "6000ms",
        },

        // Charts get the Expressive treatment for the same reason the buttons do: shape is the axis this
        // era pushes. A heavier stroke and a visibly rounded bar end read as Expressive at a glance, and
        // baseline M3 keeps the quieter geometry from the shared Material bundle.
        Chart = MaterialDesignTokens.Design.Chart with
        {
            LineWidth = "3",
            PointRadius = "3.5",
            BarRadius = "6",
            CellRadius = "4",
            AreaOpacity = "0.4",
            LegendDotSize = "0.75rem",
        },
        Extended = Extended,
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
        "_content/Flare.Theme.MaterialDesign3Expressive/css/components/progress.css",
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
