using Flare.Abstractions;
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
        new Dictionary<string, string>(MaterialDesign3Tokens.Extended)
        {
            [Css.Tokens.Md3e.Progress.Length] = "40px",
            [Css.Tokens.Md3e.Progress.IndeterminateLength] = "20px",
            [Css.Tokens.Md3e.Progress.Amplitude] = "3px",
            [Css.Tokens.Md3e.Progress.Speed] = "1s",
        };


    /// <inheritdoc />

    public string Id => ThemeId;
    public string DisplayName => "Material Design 3 Expressive";
    public DesignTokens Design => MaterialDesign3Tokens.Design with
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
        Button = MaterialDesign3Tokens.Design.Button with
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

            // The label ramps with the container instead of staying one button type style at every
            // size: label-large -> title-medium -> headline-small -> headline-large. Baseline M3
            // keeps label-large throughout, so this belongs here rather than in the reference.
            LabelMd = MaterialDesign3Tokens.Design.Typography.TitleMedium,
            LabelLg = MaterialDesign3Tokens.Design.Typography.HeadlineSmall,
            LabelXl = MaterialDesign3Tokens.Design.Typography.HeadlineLarge,

            TextPaddingInlineLg = "1rem",
            TextPaddingInlineXl = "1.25rem",

            // `md.comp.button.<size>.icon.size` beside a label, `md.comp.icon-button.<size>.icon.size` for a
            // lone glyph: the two part only at small, where an icon button draws 24dp against 20dp.
            IconSizeXs = "1.25rem", // 20dp
            IconSizeSm = "1.25rem", // 20dp
            IconSizeMd = "1.5rem",  // 24dp
            IconSizeLg = "2rem",    // 32dp
            IconSizeXl = "2.5rem",  // 40dp
            IconOnlyIconSizeXs = "1.25rem", // 20dp
            IconOnlyIconSizeSm = "1.5rem",  // 24dp
            IconOnlyIconSizeMd = "1.5rem",  // 24dp
            IconOnlyIconSizeLg = "2rem",    // 32dp
            IconOnlyIconSizeXl = "2.5rem",  // 40dp
            // The size tables state one leading and one trailing space with no with-icon variant, so the
            // icon side is not tucked.
            IconInset = "0",
            TextIconInset = "0",
            // md.comp.button.outlined: Expressive draws the outlined button neutral, on a quieter stroke.
            OutlinedColor = "var(--flare-color-on-surface-variant)",
            OutlinedBorderColor = "var(--flare-color-outline-variant)",

            // Selected: `md.comp.button.<size>.selected.container.shape.round` - 12/12/16/28/28dp. These are
            // the SQUARE shape values, which is the whole idea: a round button that gets selected takes the
            // square shape and a square one takes the capsule, so selection reads as a change of kind rather
            // than a change of degree.
            SelectedRadiusXs = "0.75rem",  // 12dp
            SelectedRadiusSm = "0.75rem",  // 12dp
            SelectedRadiusMd = "1rem",     // 16dp
            SelectedRadiusLg = "1.75rem",  // 28dp
            SelectedRadiusXl = "1.75rem",  // 28dp
            SelectedRadiusSquare = "calc(var(--_flare-btn-height, var(--flare-btn-height-md)) / 2)",

            // The Expressive toggle table (md.comp.button.<variant>.selected/unselected): tonal steps down
            // from the container to the tone itself when selected, and a filled toggle at rest is a neutral
            // container rather than the primary fill.
            TonalSelectedBg = "var(--flare-color-secondary)",
            TonalSelectedColor = "var(--flare-color-on-secondary)",
            TonalUnselectedBg = "var(--flare-color-secondary-container)",
            TonalUnselectedColor = "var(--flare-color-on-secondary-container)",
            FilledUnselectedBg = "var(--flare-color-surface-container)",
            FilledUnselectedColor = "var(--flare-color-on-surface-variant)",
        },
        // Expressive menus: a 16dp panel with rounded items, and group sections drawn as "islands" -
        // each its own rounded, elevated surface on a transparent backing panel, so adjacent sections
        // read as two cards. Baseline M3 is one classic 4dp panel with square items.
        Menu = MaterialDesign3Tokens.Design.Menu with
        {
            ItemIconSize = "1.25rem", // md.comp.menus.menu-item.*-icon.size 20dp
            PanelRadius = "var(--flare-shape-large)",         // 16dp
            ItemGapBetween = "0.125rem",                      // 2dp
            // md.comp.menus.group-padding: the panel wraps its items at 2dp all round, where the
            // baseline menu leaves 8dp above and below and none at the sides.
            PanelPaddingInline = "0.125rem",
            PanelPaddingBlock = "0.125rem",
            ItemRadius = "var(--flare-shape-extra-small)",    // 4dp  md.comp.menus.menu-item.shape
            // The ends of the list mirror the panel: md.comp.menus.menu-item.first-child.shape and
            // .last-child.shape both read 12dp, against the 4dp every interior item takes.
            ItemRadiusEnd = "var(--flare-shape-medium)",      // 12dp
            // md.comp.menus.menu-item.height. Baseline M3 keeps the 48dp list-item row
            // (md.comp.menu.list-item.container.height); Expressive tightens it.
            ItemHeight = "2.75rem",                           // 44dp
            GroupRadius = "var(--flare-shape-small)",         // 8dp
            GroupPadding = "0.125rem",                        // 2dp
            GroupBg = "var(--flare-color-surface-container-high)",
            GroupGap = "0.5rem",
            GroupShadow = "var(--flare-elevation-3)",
            GroupedPanelBg = "transparent",
            GroupedPanelShadow = "none",
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
        // The split button exists only in Expressive, so its trigger follows `md.comp.split-button.<size>`
        // here rather than the baseline square. The trigger is its caret plus the trailing-button spaces:
        // 13+22+13 = 48dp at xsmall and small, wider than the 32 and 40dp buttons, and square from medium
        // up (15+26+15 = 56, 29+38+29 = 96, 43+50+43 = 136).
        SplitButton = MaterialDesign3Tokens.Design.SplitButton with
        {
            TriggerWidthXs = "3rem",   // 48dp
            TriggerWidthSm = "3rem",   // 48dp
            TriggerWidthMd = "auto",   // 56dp, square
            TriggerWidthLg = "auto",   // 96dp, square
            TriggerWidthXl = "auto",   // 136dp, square
            CaretSizeXs = "1.375rem",  // 22dp
            CaretSizeSm = "1.375rem",  // 22dp
            CaretSizeMd = "1.625rem",  // 26dp
            CaretSizeLg = "2.375rem",  // 38dp
            CaretSizeXl = "3.125rem",  // 50dp
        },
        // Expressive's flexible navigation bar (md.comp.nav-bar) is shorter than the baseline one and its
        // vertical item's indicator narrower: 64dp and 56dp against 80dp and 64dp.
        BottomNav = MaterialDesign3Tokens.Design.BottomNav with
        {
            BarHeight = "4rem",        // 64dp
            IndicatorWidth = "3.5rem", // 56dp
        },
        Progress = MaterialDesign3Tokens.Design.Progress with
        {
            LinearIndeterminateDuration = "1750ms",
            LinearIndeterminateEasing = "cubic-bezier(0.3, 0, 0.8, 0.15)",
            CircularIndeterminateRotationDuration = "1500ms",
            CircularIndeterminateProgressDuration = "6000ms",
        },

        // Charts get the Expressive treatment for the same reason the buttons do: shape is the axis this
        // era pushes. A heavier stroke and a visibly rounded bar end read as Expressive at a glance, and
        // baseline M3 keeps the quieter geometry from the shared Material bundle.
        Chart = MaterialDesign3Tokens.Design.Chart with
        {
            LineWidth = "3",
            PointRadius = "3.5",
            BarRadius = "6",
            CellRadius = "4",
            AreaOpacity = "0.4",
            LegendDotSize = "0.75rem",
        },

        // Expressive motion scheme: its spatial springs overshoot where the standard ones settle - fast =
        // stiffness 800 / damping 0.6 (a pronounced 9% overshoot on small, quick moves), default and slow =
        // stiffness 380 and 200 at damping 0.8 (a restrained 1.5%). Sampled from a damped harmonic oscillator
        // into linear(), each over its own duration below, so the two must move together.
        Motion = MaterialDesign3Tokens.Design.Motion with
        {
            EasingSpringFast = "linear(0, 0.0315, 0.1124, 0.2244, 0.3524, 0.4842, 0.611, 0.7265, 0.8269, 0.9103, 0.9764, 1.0258, 1.0603, 1.0817, 1.0924, 1.0947, 1.0906, 1.0823, 1.0713, 1.059, 1.0465, 1.0347, 1.024, 1.0148, 1.0072, 1.0012, 0.9968, 0.9938, 0.992, 0.9911, 0.9911, 0.9915, 1)",
            EasingSpring = "linear(0, 0.0203, 0.0723, 0.1449, 0.2293, 0.3189, 0.4087, 0.4952, 0.5761, 0.6499, 0.7158, 0.7736, 0.8233, 0.8655, 0.9006, 0.9294, 0.9526, 0.9709, 0.985, 0.9956, 1.0033, 1.0087, 1.0122, 1.0142, 1.0151, 1.0151, 1.0146, 1.0136, 1.0124, 1.0111, 1.0097, 1.0084, 1)",
            EasingSpringSlow = "linear(0, 0.0217, 0.077, 0.1536, 0.2421, 0.3353, 0.428, 0.5165, 0.5984, 0.6724, 0.7378, 0.7944, 0.8426, 0.8829, 0.916, 0.9427, 0.9638, 0.98, 0.9923, 1.0012, 1.0074, 1.0115, 1.0139, 1.015, 1.0151, 1.0146, 1.0137, 1.0124, 1.0111, 1.0096, 1.0083, 1.0069, 1)",
            DurationSpringFast = "300ms",
            DurationSpring = "350ms",
            DurationSpringSlow = "500ms",
        },
        Extended = Extended,
    };
    public string DefaultPaletteId => Md3Palettes.Violet.Id;
    public IReadOnlyList<Palette> Palettes => Md3Palettes.All;
    public IPaletteGenerator? PaletteGenerator => Md3TonalGenerator.Instance;

    public IReadOnlyList<string> StyleAssets =>
    [
        "https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap",
        "_content/Flare.Theme.MaterialDesign3Expressive/css/components.css",
    ];
}
