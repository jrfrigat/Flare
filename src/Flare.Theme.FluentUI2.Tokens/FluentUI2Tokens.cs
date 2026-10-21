using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Css;
using Flare.Css.Tokens;

namespace Flare.Theme.FluentUI2.Tokens;

/// <summary>
/// Fluent UI 2 baseline design-token values: the reference <see cref="DesignTokens"/> plus the
/// light/dark <c>ColorScheme</c>s and dark-mode Extended overrides. This is the shared source of
/// truth the Fluent-lineage themes build from via <c>with</c>. Fluent diverges sharply from Material,
/// so it carries its own complete baseline rather than deriving from the Material tokens.
/// </summary>
public class FluentUI2Tokens
{
    // Fluent 2 type ramp (Segoe UI): every size/line-height/weight is a real Fluent token
    // (fontSizeBase/Hero + lineHeightBase/Hero, Semibold 600 headings, Regular 400 body). The MD-style
    // role names map onto Fluent's named text styles largeTitle..caption2.
    internal static readonly TypographyTokens Typography = new()
    {
        // No design language names a code face, so this is the generic - twice, because a generic
        // family standing alone makes several engines use their own "monospace default size".
        MonoFont = "monospace, monospace",
        WeightBold = "700",
        DisplayLarge = T("Segoe UI", "600", "2.5rem", "3.25rem", "0em"),     // largeTitle 40/52
        DisplayMedium = T("Segoe UI", "600", "2rem", "2.5rem", "0em"),       // title1 32/40
        DisplaySmall = T("Segoe UI", "600", "1.75rem", "2.25rem", "0em"),    // title2 28/36
        HeadlineLarge = T("Segoe UI", "600", "1.5rem", "2rem", "0em"),       // title3 24/32
        HeadlineMedium = T("Segoe UI", "600", "1.25rem", "1.75rem", "0em"),  // subtitle1 20/28
        HeadlineSmall = T("Segoe UI", "600", "1rem", "1.375rem", "0em"),     // subtitle2 16/22
        TitleLarge = T("Segoe UI", "600", "1rem", "1.375rem", "0em"),        // subtitle2 16/22
        TitleMedium = T("Segoe UI", "600", "0.875rem", "1.25rem", "0em"),    // body1Strong 14/20
        TitleSmall = T("Segoe UI", "600", "0.75rem", "1rem", "0em"),         // caption1Strong 12/16
        BodyLarge = T("Segoe UI", "400", "1rem", "1.375rem", "0em"),         // body2 16/22
        BodyMedium = T("Segoe UI", "400", "0.875rem", "1.25rem", "0em"),     // body1 14/20 (default)
        BodySmall = T("Segoe UI", "400", "0.75rem", "1rem", "0em"),          // caption1 12/16
        LabelLarge = T("Segoe UI", "600", "0.875rem", "1.25rem", "0em"),     // body1Strong 14/20
        LabelMedium = T("Segoe UI", "600", "0.75rem", "1rem", "0em"),        // caption1Strong 12/16
        LabelSmall = T("Segoe UI", "400", "0.625rem", "0.875rem", "0em"),    // caption2 10/14
    };

    internal static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "2px",
        Small = "4px",
        Medium = "6px",
        Large = "8px",
        ExtraLarge = "12px",
        Full = "9999px",

        // Fluent 2 holds a constant corner in every state - the fill changes, the shape does not.
        MorphDuration = "0s",
        MorphEasing = "cubic-bezier(0.1, 0.9, 0.2, 1)",
    };

    // Fluent 2 motion: the nine named curves and the durationUltraFast..durationUltraSlow ramp of
    // @fluentui/tokens, placed by role.
    private const string CurveDecelerateMid = "cubic-bezier(0, 0, 0, 1)";
    private const string CurveAccelerateMin = "cubic-bezier(0.8, 0, 0.78, 1)";
    private const string CurveEasyEase = "cubic-bezier(0.33, 0, 0.67, 1)";

    // A surface phase that neither moves nor fades; each family states only what it changes.
    private static readonly MotionPhaseTokens Still = new()
    {
        Delay = "0ms",
        Duration = "0ms",
        Easing = "linear",
        Offset = "0px",
        Scale = "1",
        Opacity = "1",
        Reveal = "100%",
        Blur = "0px",
        FadeDelay = "0ms",
        FadeDuration = "0ms",
        FadeEasing = "linear",
    };

    // PopoverSurfaceMotion / MenuSurfaceMotion: slide 10px from the anchor and fade in over durationSlower on
    // curveDecelerateMid. They have no exit - the surface unmounts at once.
    private static readonly SurfaceMotionTokens AnchoredMotion = new()
    {
        Enter = Still with { Duration = "400ms", Easing = CurveDecelerateMid, Offset = "10px", Opacity = "0", FadeDuration = "400ms", FadeEasing = CurveDecelerateMid },
        Exit = Still with { Opacity = "0" },
    };

    // drawerMotions: slide in the drawer's own size and fade, curveDecelerateMid in and curveAccelerateMin out,
    // durationSlow for the default (medium) size. A bottom sheet is the same drawer at position="bottom".
    private static readonly SurfaceMotionTokens EdgeMotion = new()
    {
        Enter = Still with { Duration = "300ms", Easing = CurveDecelerateMid, Offset = "100%", Opacity = "0", FadeDuration = "300ms", FadeEasing = CurveDecelerateMid },
        Exit = Still with { Duration = "300ms", Easing = CurveAccelerateMin, Offset = "100%", Opacity = "0", FadeDuration = "300ms", FadeEasing = CurveAccelerateMin },
    };

    internal static readonly MotionTokens Motion = new()
    {
        EasingStandard = CurveEasyEase,                               // curveEasyEase
        EasingDecelerate = CurveDecelerateMid,                        // curveDecelerateMid
        EasingAccelerate = "cubic-bezier(1, 0, 1, 1)",                // curveAccelerateMid
        EasingEmphasized = "cubic-bezier(0.8, 0, 0.2, 1)",            // curveEasyEaseMax
        EasingEmphasizedDecelerate = "cubic-bezier(0.1, 0.9, 0.2, 1)", // curveDecelerateMax
        EasingEmphasizedAccelerate = "cubic-bezier(0.9, 0.1, 1, 0.2)", // curveAccelerateMax
        EasingSubtle = CurveEasyEase,                                 // no gentler on-screen curve in Fluent
        EasingSubtleDecelerate = "cubic-bezier(0.33, 0, 0.1, 1)",     // curveDecelerateMin
        EasingSubtleAccelerate = CurveAccelerateMin,                  // curveAccelerateMin
        EasingLinear = "linear",                                      // curveLinear

        DurationStateChange = "100ms", // durationFaster
        DurationSmallMove = "200ms",   // durationNormal
        DurationEnterSmall = "250ms",  // durationGentle
        DurationEnterMedium = "300ms", // durationSlow
        DurationEnterLarge = "400ms",  // durationSlower
        DurationExitSmall = "250ms",   // Fluent leaves at the pace it arrives
        DurationExitMedium = "300ms",
        DurationExitLarge = "400ms",
        DurationCycle = "1500ms",      // Spinner rotation
        DurationCycleLong = "3000ms",  // ProgressBar indeterminate pass

        // Fluent 2 has no spring in its motion language - it moves on decelerating curves and never
        // overshoots - so the spring easings resolve to curveDecelerateMax and only the durations
        // distinguish them. A theme is entitled to answer "this design does not bounce".
        EasingSpringFast = "cubic-bezier(0.1, 0.9, 0.2, 1)",
        EasingSpring = "cubic-bezier(0.1, 0.9, 0.2, 1)",
        EasingSpringSlow = "cubic-bezier(0.1, 0.9, 0.2, 1)",
        DurationSpringFast = "150ms",  // durationFast
        DurationSpring = "200ms",      // durationNormal
        DurationSpringSlow = "300ms",  // durationSlow

        // DialogSurfaceMotion: scale from 0.85 and fade, durationGentle both ways, curveDecelerateMid in and
        // curveAccelerateMin out.
        Dialog = new()
        {
            Enter = Still with { Duration = "250ms", Easing = CurveDecelerateMid, Scale = "0.85", Opacity = "0", FadeDuration = "250ms", FadeEasing = CurveDecelerateMid },
            Exit = Still with { Duration = "250ms", Easing = CurveAccelerateMin, Scale = "0.85", Opacity = "0", FadeDuration = "250ms", FadeEasing = CurveAccelerateMin },
        },
        Sheet = EdgeMotion,
        Menu = AnchoredMotion,
        Popover = AnchoredMotion,
        // Tooltip: no motion of its own - it waits showDelay (250ms) to appear and hideDelay (250ms) to go.
        Tooltip = new()
        {
            Enter = Still with { Delay = "250ms", Opacity = "0", FadeDelay = "250ms" },
            Exit = Still with { Delay = "250ms", Opacity = "0", FadeDelay = "250ms" },
        },
        // Toast (CollapseDelayed): the height opens over durationNormal and the content fades in over
        // durationSlower after a durationNormal stagger; leaving, it fades first and collapses after
        // durationSlower. curveEasyEase throughout.
        Snackbar = new()
        {
            Enter = Still with { Duration = "200ms", Easing = CurveEasyEase, Opacity = "0", Reveal = "0%", FadeDelay = "200ms", FadeDuration = "400ms", FadeEasing = CurveEasyEase },
            Exit = Still with { Delay = "400ms", Duration = "200ms", Easing = CurveEasyEase, Opacity = "0", Reveal = "0%", FadeDuration = "400ms", FadeEasing = CurveEasyEase },
        },
        Drawer = EdgeMotion,
    };

    internal static readonly StateTokens State = new()
    {
        HoverOpacity = "0.1",
        SelectedOpacity = "0.12",
        FocusOpacity = "0.1",
        PressedOpacity = "0.12", // F2: pressed - the darkest (darker than hover 0.10)
        DraggedOpacity = "0.1",
        DisabledOpacity = "0.4",
        DisabledContainerOpacity = "0.1",
        // Foundation: currentColor wash at each state's opacity (identical to today). A later pass switches
        // these to Fluent's discrete per-state fills so the button/menu/list override CSS can be dropped.
        HoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        FocusLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        PressedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-pressed-opacity) * 100%), transparent)",
        DraggedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
        // Fluent signals focus with a stroke, not a fill, so the hover fill must survive the pairing.
        FocusHoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        SelectedLayer = "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-selected-opacity) * 100%), transparent)",
        SelectedHoverLayer = "color-mix(in srgb, var(--flare-color-primary) 18%, transparent)",
    };

    internal static readonly BadgeTokens Badge = new()
    {
        PaddingXXs = "0.1875rem",
        PaddingXSm = "0.1875rem",
        LabelSizeXs = "0.5625rem",
        // Fluent UI 2: square-ish badge with extra-small radius (not full pill)
        Radius = "var(--flare-shape-extra-small)",
        // The ramp, one token per size so every size is the theme's to name.
        MinWidthXs = "0.75rem",   MinWidthSm = "0.875rem",   MinWidthMd = "1rem",   MinWidthLg = "1.25rem",  MinWidthXl = "1.5rem",
        HeightXs   = "0.75rem",   HeightSm   = "0.875rem",   HeightMd   = "1rem",   HeightLg   = "1.25rem",  HeightXl   = "1.5rem",
        DotSizeXs  = "0.25rem",   DotSizeSm  = "0.3125rem",  DotSizeMd  = "0.375rem", DotSizeLg = "0.5rem",  DotSizeXl  = "0.625rem",
        Offset = "0.375rem",
    };

    internal static readonly AlertTokens Alert = new()
    {
        BodyOpacity = "0.9",
        CloseOpacity = "0.7",
        // Fluent UI 2: small radius, border visible by default
        Radius = "var(--flare-shape-small)",
        BorderWidth = "1px",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        // Fluent does not tighten a subtle button: the ladder matches the contained one.
        TextPaddingInlineXs = "0.375rem",
        TextPaddingInlineSm = "0.5rem",
        TextPaddingInlineMd = "0.75rem",
        TextPaddingInlineLg = "1rem",
        TextPaddingInlineXl = "1.25rem",
        // Compact gaps (Gap) between text and the Microsoft icon
        GapXs = "0.125rem",        // 2px
        GapSm = "0.25rem",         // 4px
        GapMd = "0.375rem",        // 6px (guideline standard)
        GapLg = "0.5rem",          // 8px
        GapXl = "0.5rem",          // 8px

        // Container heights per the Fluent UI 2 spec
        HeightXs = "1.25rem",   // 20px
        HeightSm = "1.5rem",     // 24px
        HeightMd = "2rem",      // 32px (standard button)
        HeightLg = "2.5rem",     // 40px
        HeightXl = "3rem",      // 48px

        // Strict inline padding (inner spacing)
        PaddingInlineXs = "0.375rem", // 6px
        PaddingInlineSm = "0.5rem",    // 8px
        PaddingInlineMd = "0.75rem",   // 12px
        PaddingInlineLg = "1rem",      // 16px
        PaddingInlineXl = "1.25rem",  // 20px

        // Corner radii proportional to size (strict Windows 11 corners)
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-extra-small)"), // 2px
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),       // 4px
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-small)"),       // 4px
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),      // 6px
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-large)"),       // 8px

        // Selection is a repaint here, not a reshape: Windows keeps its corners where they are and says
        // "on" with colour. Each selected radius therefore repeats the rest radius at that size rather
        // than travelling anywhere.
        SelectedRadiusXs = "var(--flare-shape-extra-small)",
        SelectedRadiusSm = "var(--flare-shape-small)",
        SelectedRadiusMd = "var(--flare-shape-small)",
        SelectedRadiusLg = "var(--flare-shape-medium)",
        SelectedRadiusXl = "var(--flare-shape-large)",
        // Selection is the accent here, whatever the variant: this language does not send each variant
        // somewhere different the way Material does, it just says "on" in the accent colour. The one
        // thing it cannot do is repeat a variant s own resting colour - a selected tonal button painted
        // secondary-container would be indistinguishable from an unselected one, which is exactly what
        // happened when these four pairs were first filled in - so every variant travels to the accent,
        // and the filled toggle starts from a neutral container rather than from the accent it ends on.
        TonalSelectedBg = "var(--flare-color-primary)",
        TonalSelectedColor = "var(--flare-color-on-primary)",
        OutlinedSelectedBg = "var(--flare-color-primary)",
        OutlinedSelectedColor = "var(--flare-color-on-primary)",
        FilledUnselectedBg = "var(--flare-color-surface-container)",
        FilledUnselectedColor = "var(--flare-color-on-surface-variant)",
        // The tonal toggle keeps its variant's container until selected.
        TonalUnselectedBg = "var(--flare-color-secondary-container)",
        TonalUnselectedColor = "var(--flare-color-on-secondary-container)",

        // Double focus-ring behavior (inner + outer) and flat shadows
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color, #000000)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer, #FFFFFF)",
        FilledHoverShadow = "none",
        // Fluent repaints disabled controls in a flat palette rather than fading them: the solid variants take
        // the disabled fill, every variant the disabled foreground, the outlined one the disabled stroke.
        DisabledOpacity = "1",
        DisabledRepaint = "100%",
        DisabledContainer = "var(--flare-fluent-disabled-bg, #F0F0F0)",
        DisabledContent = "var(--flare-fluent-disabled-fg, #BDBDBD)",
        DisabledStroke = "var(--flare-fluent-disabled-border, #E0E0E0)",

        // Compact Fluent icons (no MD3 gigantism at L/XL)
        IconSizeXs = "1rem",     // 16px
        IconSizeSm = "1rem",     // 16px
        IconSizeMd = "1.25rem",  // 20px
        IconSizeLg = "1.25rem",  // 20px
        IconSizeXl = "1.5rem",   // 24px
        // A lone glyph takes the labelled size, and the icon side is tucked by one small step.
        IconOnlyIconSizeXs = "var(--flare-btn-icon-size-xs)",
        IconOnlyIconSizeSm = "var(--flare-btn-icon-size-sm)",
        IconOnlyIconSizeMd = "var(--flare-btn-icon-size-md)",
        IconOnlyIconSizeLg = "var(--flare-btn-icon-size-lg)",
        IconOnlyIconSizeXl = "var(--flare-btn-icon-size-xl)",
        IconOnlySizeXs = "var(--flare-btn-height-xs)",
        IconOnlySizeSm = "var(--flare-btn-height-sm)",
        IconOnlySizeMd = "var(--flare-btn-height-md)",
        IconOnlySizeLg = "var(--flare-btn-height-lg)",
        IconOnlySizeXl = "var(--flare-btn-height-xl)",
        // An icon-only button rounds like a labelled one of the same size.
        IconOnlyRadiusXs = "var(--flare-btn-radius-xs-top-left)",
        IconOnlyRadiusSm = "var(--flare-btn-radius-sm-top-left)",
        IconOnlyRadiusMd = "var(--flare-btn-radius-md-top-left)",
        IconOnlyRadiusLg = "var(--flare-btn-radius-lg-top-left)",
        IconOnlyRadiusXl = "var(--flare-btn-radius-xl-top-left)",

        IconInset = "var(--flare-spacing-2)",
        TextIconInset = "var(--flare-spacing-2)",
        OutlinedColor = "var(--flare-color-on-surface-variant)",
        OutlinedBorderColor = "var(--flare-color-outline-variant)",

        // Label typography: compact Fluent scale (text barely grows)
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.TitleMedium,
        LabelXl = Typography.TitleLarge,
    };

    // Connected (Fluent's segmented look): touching segments, 1px overlap collapsing the shared border,
    // small rounded ends, flat interior corners. Standard is Fluent's toolbar shape - separate buttons a
    // spacing step apart, each keeping the 4px corner the button already has, which is why the group
    // says nothing about corners here.
    // Fluent groups its buttons at one rhythm regardless of size, and its connected group is a single
    // rounded rectangle whose segments share a hairline seam: square inside, softened only at the two
    // ends. Neither a press nor a selection reshapes anything - Fluent signals both by repainting - so
    // the pressed and selected corners are pointed back at the shape the segment already has.
    internal static readonly ButtonGroupTokens ButtonGroup = new()
    {
        ConnectedGap = "0",
        ConnectedOverlap = "-1px",
        ConnectedSelectedRadius = "0",
        ConnectedInnerRadiusXs = "0",
        ConnectedInnerRadiusSm = "0",
        ConnectedInnerRadiusMd = "0",
        ConnectedInnerRadiusLg = "0",
        ConnectedInnerRadiusXl = "0",
        ConnectedPressedRadiusXs = "0",
        ConnectedPressedRadiusSm = "0",
        ConnectedPressedRadiusMd = "0",
        ConnectedPressedRadiusLg = "0",
        ConnectedPressedRadiusXl = "0",
        ZActive = "1",
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "1px", // Thin 1px Microsoft seam between the two parts

        // Fluent: fixed-width trigger 24dp (not square), no inline padding needed
        TriggerWidthXs = "1.5rem",
        TriggerWidthSm = "1.5rem",
        TriggerWidthMd = "1.5rem",
        TriggerWidthLg = "1.5rem",
        TriggerWidthXl = "1.5rem",

        // Fluent: fixed 12dp chevron at every size
        CaretSizeXs = "0.75rem",
        CaretSizeSm = "0.75rem",
        CaretSizeMd = "0.75rem",
        CaretSizeLg = "0.75rem",
        CaretSizeXl = "0.75rem",

        // Main: outer LEFT edges = Button radius (passthrough), inner joints - sharp 0 (Fluent)
        MainRadiusXs = new() { TopLeft = "var(--flare-btn-radius-xs-top-left)", BottomLeft = "var(--flare-btn-radius-xs-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusSm = new() { TopLeft = "var(--flare-btn-radius-sm-top-left)", BottomLeft = "var(--flare-btn-radius-sm-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusMd = new() { TopLeft = "var(--flare-btn-radius-md-top-left)", BottomLeft = "var(--flare-btn-radius-md-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusLg = new() { TopLeft = "var(--flare-btn-radius-lg-top-left)", BottomLeft = "var(--flare-btn-radius-lg-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusXl = new() { TopLeft = "var(--flare-btn-radius-xl-top-left)", BottomLeft = "var(--flare-btn-radius-xl-bottom-left)", TopRight = "0px", BottomRight = "0px" },

        // Trigger: inner joints - sharp 0, outer RIGHT edges = Button radius (passthrough)
        TriggerRadiusXs = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-xs-top-right)", BottomRight = "var(--flare-btn-radius-xs-bottom-right)" },
        TriggerRadiusSm = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-sm-top-right)", BottomRight = "var(--flare-btn-radius-sm-bottom-right)" },
        TriggerRadiusMd = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-md-top-right)", BottomRight = "var(--flare-btn-radius-md-bottom-right)" },
        TriggerRadiusLg = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-lg-top-right)", BottomRight = "var(--flare-btn-radius-lg-bottom-right)" },
        TriggerRadiusXl = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-xl-top-right)", BottomRight = "var(--flare-btn-radius-xl-bottom-right)" },
    };

    // Toggle: currently = MD3 defaults (Fluent-specific sizes/shape - an open decision in the spec).
    // Fluent's segmented control: a subtle rounded rectangle around segments that are ordinary buttons.
    internal static readonly ToggleButtonTokens ToggleButton = new()
    {
        GroupRadius = "var(--flare-shape-small)",
        GroupRadiusVertical = "var(--flare-shape-small)",
    };

    // FAB: flatter Fluent rounding (4-8dp).
    internal static readonly FabTokens Fab = new()
    {
        // Fluent circular button icon: 20px small and medium, 24px large.
        IconSizeSm = "1.25rem",
        IconSizeMd = "1.25rem",
        IconSizeLg = "1.5rem",
        RadiusSm = "var(--flare-shape-small)",
        RadiusMd = "var(--flare-shape-medium)",
        RadiusLg = "var(--flare-shape-large)",
        PaddingLg = "1.75rem",
    };

    // Menu: neutral baseline geometry, but the panel appears via fade/slide without scale (Fluent motion).
    // All members are now set explicitly, since the core record carries no default values.
    /// <summary>List container and row tokens.</summary>
    internal static readonly ListTokens List = new()
    {
        ItemHeight = "3.5rem",
        ItemHeightTwoLine = "4.5rem",
        ItemHeightDense = "3rem",
        ItemHeightTwoLineDense = "3.5rem",
        ItemRadius = "0",
        ItemDisabledOpacity = "1",
        ItemIconSize = "1.25rem", // Fluent's regular icon size; the List spec names none
    };

    /// <summary>Accordion container, header and body tokens.</summary>
    internal static readonly AccordionTokens Accordion = new()
    {
        HeaderDisabledOpacity = "1",
        IconSize = "0.875rem",
    };

    /// <summary>Standalone collapse header tokens.</summary>
    internal static readonly CollapseTokens Collapse = new()
    {
        HeaderBg = "transparent",
        HeaderDisabledOpacity = "1",
    };

    internal static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        // No authoritative Fluent 2 figure for the flyout gap; keeps the 4px the menu has always used.
        PanelOffset = "0.25rem",
        PanelMinWidth = "10rem",
        EnterAnimation = "flare-menu-in-fade",
        PanelRadius = "var(--flare-popover-radius, var(--flare-shape-small))",
        PanelPaddingBlock = "0.25rem",
        PanelPaddingInline = "0.25rem",
        ItemHeight = "0",
        ItemPaddingBlock = "0.625rem",
        ItemGapBetween = "0rem",
        ItemIconSize = "1.25rem",
        ItemRadius = "0",
        ItemRadiusEnd = "0",
        GroupRadius = "0",
        GroupPadding = "0",
        GroupBg = "transparent",
        GroupGap = "0.5rem",
        GroupShadow = "none",
        GroupedPanelBg = "var(--flare-menu-panel-bg, var(--flare-color-surface-container))",
        GroupedPanelShadow = "var(--flare-menu-panel-shadow, var(--flare-elevation-2))",
        ItemLabelFont = "var(--flare-typescale-body-large-font)",
        ItemLabelWeight = "var(--flare-typescale-body-large-weight)",
        ItemLabelSize = "var(--flare-typescale-body-large-size)",
        ItemLabelHeight = "var(--flare-typescale-body-large-height)",
        ItemLabelSpacing = "var(--flare-typescale-body-large-spacing)",
        // createFocusOutlineStyle: a strokeWidthThick stroke in colorStrokeFocus2, black in light and white in dark.
        ItemFocusRingColor = "var(--flare-fluent-focus-stroke-color)",
        ItemFocusRingThickness = "2px",
        ItemFocusRingOffset = "-2px",
        // Fluent repaints a disabled item rather than fading it.
        ItemDisabledOpacity = "1",
    };

    // Checkbox - Fluent: 1px border, 4dp corner, no MD3 halo, double focus ring.
    internal static readonly CheckboxTokens Checkbox = new()
    {
        // The size ramp, one token per size so a theme can move any of the five steps.
        SizeXs = "0.875rem",
        SizeSm = "1rem",
        SizeMd = "1.125rem",
        SizeLg = "1.375rem",
        SizeXl = "1.625rem",
        StateLayerSize = "2.5rem",
        BorderWidth = "1px",
        Radius = "var(--flare-shape-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer)",
        // Fluent repaints a disabled control in a flat palette rather than fading it.
        DisabledOpacity = "1",
    };

    // Radio - Fluent: no MD3 state-layer halo.
    internal static readonly RadioTokens Radio = new()
    {
        // The size ramp, one token per size so a theme can move any of the five steps.
        SizeXs = "1rem",
        SizeSm = "1.125rem",
        SizeMd = "1.25rem",
        SizeLg = "1.5rem",
        SizeXl = "1.75rem",
        StateLayerSize = "2.5rem",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        // Fluent's two-stroke focus indicator, as stated for the checkbox above.
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer)",
        // Fluent repaints a disabled control in a flat palette rather than fading it.
        DisabledOpacity = "1",
    };

    // Chip - Fluent: the same values (8dp/32dp).
    internal static readonly ChipTokens Chip = new()
    {
        BorderColor = "var(--flare-color-outline)",
        // Weight and spacing are `inherit`/`normal`: the chip declared neither and took them from around it.
        LabelWeight = "inherit",
        LabelSpacing = "normal",
        // a chip glyph sizes with its label, same ramp.
        IconSizeXs = "0.875rem",
        IconSizeSm = "1rem",
        IconSizeMd = "1.125rem",
        IconSizeLg = "1.25rem",
        IconSizeXl = "1.5rem",
        AvatarSizeXs = "1rem",
        AvatarSizeSm = "1.125rem",
        AvatarSizeMd = "1.5rem",
        AvatarSizeLg = "1.875rem",
        AvatarSizeXl = "2.25rem",
        Height = "2rem",
    };
    internal static readonly TabsTokens Tabs = new()
    {
        // Fluent tab icon: 20px at the medium tab.
        IconSize = "1.25rem",
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "3px",
        // Fluent has no nested-tab level of its own, so the subordinate strip is expressed the way
        // this language expresses subordination anyway: a thinner rail and a neutral active label.
        SecondaryIndicatorThickness = "2px",
        InactiveColor = "var(--flare-color-on-surface)",
        TrackBg = "transparent",
        TabDisabledOpacity = "1",
        ScrollDisabledOpacity = "1",
        TabHeight = "auto",
        TabMinWidth = "0",
        TabPaddingInline = "var(--flare-spacing-10)",
    };

    // On-this-page - Fluent: rail style (vertical line + brand bar on the left of the active item),
    // without the MD3 pill.
    internal static readonly Flare.Abstractions.Tokens.Components.TableOfContentsTokens TableOfContents = new()
    {
        TitleTracking = "0.05em",
        ActiveWeight = "600",
        HoverBgOpacity = "40%",
        LineHeight = "1.4",
        ActiveColor = "var(--flare-color-primary)",
        TitleColor = "var(--flare-color-on-surface)",
        RailColor = "var(--flare-color-outline-variant)",
        RailWidth = "1px",
        ActiveBg = "transparent",
        ActiveRadius = "0",
        MarkerWidth = "3px",
        // Gap between the rail/marker and the link text.
        LinkPadX = "0.625rem",
    };

    // Splitter: an 8dp gutter with a 2dp centred grip. The colours are semantic roles, so they already
    // resolve to Fluent's palette; the geometry matches MD3's because Fluent has no separate opinion
    // about a resize gutter. Stated here rather than inherited so a divergence stays a one-line edit.
    internal static readonly SplitterTokens Splitter = new()
    {
        GutterSize = "0.5rem",
        GripThickness = "2px",
        GripLength = "1.75rem",
        Color = "var(--flare-color-surface-variant)",
        IconSize = "1.125rem",
        // colorNeutralStroke1 at rest (the outline role), colorNeutralStroke1Pressed while held.
        GripPressedColor = "var(--flare-fluent-stroke-pressed)",
        GripPressedThickness = "var(--flare-splitter-grip-thickness)",
        GripPressedLength = "var(--flare-splitter-grip-length)",
        GripPressedRadius = "var(--flare-splitter-grip-radius)",
    };

    // Slider - Fluent: thin 4px rail, round white thumb (20px) with a 2px brand ring,
    // active = brand, inactive = neutralStroke; without the MD3 Expressive bar/notch.
    // Geometry is set via plain constant tokens (no dependency on size).
    internal static readonly SliderTokens Slider = new()
    {
        // Fluent's slider is one fixed geometry at every size: a 4px rail with a 20px round thumb.
        TrackHeightXs = "4px",
        TrackHeightSm = "4px",
        TrackHeightMd = "4px",
        TrackHeightLg = "4px",
        TrackHeightXl = "4px",
        TrackRadiusXs = "var(--flare-shape-full, 9999px)",
        TrackRadiusSm = "var(--flare-shape-full, 9999px)",
        TrackRadiusMd = "var(--flare-shape-full, 9999px)",
        TrackRadiusLg = "var(--flare-shape-full, 9999px)",
        TrackRadiusXl = "var(--flare-shape-full, 9999px)",
        HandleHeightXs = "20px",
        HandleHeightSm = "20px",
        HandleHeightMd = "20px",
        HandleHeightLg = "20px",
        HandleHeightXl = "20px",
        // Flanking StartIcon/EndIcon ramp.
        IconSizeXs = "20px",
        IconSizeSm = "22px",
        IconSizeMd = "24px",
        IconSizeLg = "24px",
        IconSizeXl = "32px",
        Length = "12rem",
        GapRadius = "0px",
        Gap = "0px",
        HandleWidth = "20px",
        HandlePressedWidth = "20px",
        HandleRadius = "var(--flare-shape-full, 9999px)",
        HandleBorderWidth = "2px",                   // brand ring (color = --_active)
        HandleFill = "var(--flare-color-surface)",   // white thumb
        InactiveColor = "var(--flare-color-outline-variant)",
        // Fluent's thumb has no halo: it shows focus with the neutral stroke round the control instead.
        StateLayerSize = "0px",
        StateHoverOpacity = "0",
        StatePressedOpacity = "0",
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color)",
        FocusOutlineOffset = "2px",
        StopColor = "var(--flare-color-outline)",
        StopSize = "2px",
        HandleClipPath = "none",
        DisabledOpacity = "1",
        // Flat disabled palette rather than a fade - the same two values the theme's own slider.css
        // used to force onto the track with !important.
        DisabledActiveColor = "var(--flare-fluent-disabled-fg, #BDBDBD)",
        DisabledInactiveColor = "var(--flare-fluent-disabled-border, #E0E0E0)",
    };

    // Input - Fluent outlined style (full 1px border, neutral focus box + 2px brand bottom accent).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface)",
        BorderColor = "var(--flare-color-outline)",
        OutlinedRadius = "var(--flare-shape-small)",
        BorderBottomColor = "var(--flare-color-outline)",
        // Fluent focus = a 2px brand accent under the field, as a layout-neutral inset shadow (no jump).
        FocusRing = "inset 0 -2px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        HoverBorderBottomColor = "var(--flare-fluent-stroke-hover, #C7C7C7)",
        FocusBorderBottomColor = "var(--flare-input-border-bottom-color)",
        HoverStateLayer = "none",
        PaddingXs = "0.1875rem 0.5rem",
        PaddingSm = "0.375rem 0.625rem",
        PaddingMd = "0.75rem 1rem",
        PaddingLg = "0.875rem 1.125rem",
        PaddingXl = "1.125rem 1.25rem",
        // Height ramp, measured off the padding ramp above: each step is the tallest control of that size, so`r`n        // nothing shrinks.
        HeightXs = "1.75rem",   // 28px
        HeightSm = "2.125rem", // 34px
        HeightMd = "2.875rem", // 46px
        HeightLg = "3.125rem", // 50px
        HeightXl = "3.625rem", // 58px
        IconSize = "1.25rem",                         // medium input icon 20px
        // Fluent flat disabled: grey field + disabled stroke (was MD3 on-surface opacity mixes).
        DisabledBg = "var(--flare-fluent-disabled-bg, #F0F0F0)",
        DisabledIndicator = "var(--flare-fluent-disabled-border, #E0E0E0)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
        DisabledOpacity = "1",
        // The stroke stays as it rests on focus; the brand bar underneath carries it. An invalid field is
        // stroked in the error colour.
        FocusBorderColor = "var(--flare-color-outline)",
        ErrorBorderColor = "var(--flare-color-error)",
        ErrorFocusRing = "inset 0 0 0 1px var(--flare-color-error)",
        // Filled = Fluent's filled-darker appearance: a neutral fill with no stroke, the same brand bar on focus.
        FilledVariantBg = "var(--flare-color-surface-container-low)",
        FilledVariantBorderColor = "transparent",
        FilledVariantBorderBottomColor = "transparent",
        FilledVariantRadius = "var(--flare-shape-small)",
        FilledVariantFocusRing = "inset 0 -2px 0 0 var(--fc-main, var(--flare-color-primary))",
        FilledVariantFocusBorderColor = "transparent",
        FilledVariantFocusBorderBottomColor = "transparent",
        FilledVariantErrorBorderColor = "var(--flare-color-error)",
        FilledVariantErrorFocusRing = "inset 0 0 0 1px var(--flare-color-error)",
        // Outlined = Fluent's own outline appearance, the field this theme draws by default.
        OutlinedVariantBg = "var(--flare-color-surface)",
        OutlinedVariantRadius = "var(--flare-shape-small)",
        OutlinedVariantFocusRing = "inset 0 -2px 0 0 var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantFocusBorderColor = "var(--flare-color-outline)",
        OutlinedVariantFocusBorderBottomColor = "var(--flare-color-outline)",
        OutlinedVariantErrorFocusRing = "inset 0 0 0 1px var(--flare-color-error)",
        DropdownMaxHeight = "16rem",
    };

    // Chart - Fluent: flat marks, square legend swatches, hairline solid grid, no frame around the plot.
    // The palette is built from the theme's own hues so a brand swap carries into the data, with the same
    // three-hue / four-treatment construction the Material base uses; the error role stays out of it.
    // A product with the official Fluent data-visualization ramp overrides these twelve values.
    internal static readonly GaugeTokens Gauge = new()
    {
        TrackColor = "var(--flare-color-surface-container-high)",
        TrackWidth = "10",
        TrackCap = "butt",

        FillWidth = "10",

        NeedleWidth = "2",
        NeedleLength = "0.82",
        PivotRadius = "3",

        TickWidth = "1",
        TickLength = "6",
        TickMinorWidth = "1",
        TickMinorLength = "3",
        TickGap = "4",

        LabelSize = "8.5",
        ValueWeight = "600",

        BandOpacity = "0.45",
        TargetWidth = "2",
    };

    internal static readonly ChartTokens Chart = new()
    {
        Series7 = "color-mix(in srgb, var(--flare-color-primary) 70%, var(--flare-color-on-surface))",
        Series8 = "color-mix(in srgb, var(--flare-color-tertiary) 70%, var(--flare-color-on-surface))",
        Series9 = "color-mix(in srgb, var(--flare-color-secondary) 70%, var(--flare-color-on-surface))",
        Series10 = "color-mix(in srgb, var(--flare-color-primary) 60%, var(--flare-color-surface))",
        Series11 = "color-mix(in srgb, var(--flare-color-tertiary) 60%, var(--flare-color-surface))",
        Series12 = "color-mix(in srgb, var(--flare-color-secondary) 60%, var(--flare-color-surface))",

        // Fluent draws thinner and squarer than Material: butt caps, near-square bars, smaller points.
        LineWidth = "1.5",
        LineCap = "butt",
        PointRadius = "2",
        PointOpacity = "0.7",
        BubbleMinRadius = "3",
        BubbleMaxRadius = "22",
        BarRadius = "1",
        AreaOpacity = "0.25",
        RadarFillOpacity = "0.15",
        WedgeOpacity = "0.75",
        SliceStrokeWidth = "1",

        RampMinOpacity = "0.1",
        RampMaxOpacity = "1",
        CellRadius = "0",
        CellGap = "2",
        CellHoverOpacity = "0.85",

        GridWidth = "0.5",
        GridDash = "none",
        GridMinorWidth = "0.5",
        LabelSize = "9px",
        ValueSize = "8px",
        AxisTitleSize = "10px",

        // Fluent charts sit in the page, not in a card: no border, tighter padding.
        Surface = "transparent",
        Radius = "var(--flare-shape-extra-small)",
        BorderWidth = "0",
        BorderColor = "transparent",
        Padding = "var(--flare-spacing-4)",
        Gap = "var(--flare-spacing-3)",

        LegendGap = "var(--flare-spacing-5)",
        LegendItemGap = "var(--flare-spacing-2)",
        LegendDotSize = "0.625rem",
        LegendDotRadius = "0",
        LegendOffOpacity = "0.4",

        TrendWidth = "1",
        TrendDash = "4 3",
        TrendOpacity = "0.8",
        AnnotationWidth = "1",
        AnnotationDash = "3 3",
        AnnotationBandOpacity = "0.1",
        AnnotationArrowSize = "6",
        AnnotationPointRadius = "3",

        // Fluent's line cap is butt, so a dotted stroke needs a real (short) dash rather than a 0-length one.
        LineDashDashed = "5 3",
        LineDashDotted = "1 3",
        LineDashDashDot = "6 3 1 3",

        ZoomSelectionFill = "color-mix(in srgb, var(--flare-color-primary) 12%, transparent)",
    };

    // Progress - Fluent: thin 2px rail, squared corners, no stop dot and a 3px butt-cap ring.
    internal static readonly ProgressTokens Progress = new()
    {
        // Size ramp, anchored at Md (the default) and running both ways. Fluent names two bar thicknesses -
        // 2px (medium, the default) and 4px (large) - which pin Md and Xl.
        LinearHeightXs = "1px",
        LinearHeightSm = "1.5px",
        LinearHeightMd = "2px",   // spec: bar thickness (medium, the default)
        LinearHeightLg = "3px",
        LinearHeightXl = "4px",   // spec: bar thickness (large)
        // Centred content: the ramp follows the indicator sizes, so a label dropped into the
        // smallest ring still fits it.
        ContentSizeXs = "0.5rem",
        ContentSizeSm = "0.625rem",
        ContentSizeMd = "0.75rem",
        ContentSizeLg = "1rem",
        ContentSizeXl = "1.25rem",
        TrackRadius = "var(--flare-shape-extra-small)",
        Gap = "0px",
        LinearIndeterminateDuration = "1500ms",
        StopSize = "0px",
        StopInset = "0px",
        StopColor = "var(--fc-main, var(--flare-color-primary))",
        BufferOpacity = "30%",
        // Fluent's spinner scale runs 16..44px in eight steps, so four of these five land on it exactly.
        // Md stays 40px - the value this theme has always declared - rather than dropping to Fluent's own
        // 32px "medium" default, which would silently resize every existing spinner. That this theme sits a
        // step above its spec default is a pre-existing fidelity question, tracked separately; it is not
        // something to settle under a sizing refactor.
        CircularSizeXs = "24px",  // spec: spinner (extra-small)
        CircularSizeSm = "32px",  // spec: spinner (medium)
        CircularSizeMd = "40px",  // spec: spinner (extra-large)
        CircularSizeLg = "44px",  // spec: spinner (huge)
        CircularSizeXl = "56px",
        // Spec: 2px stroke up to the small spinner, 3px from medium up.
        CircularWidthXs = "2px",  // spec: strokeWidthThick
        CircularWidthSm = "3px",  // spec: strokeWidthThicker
        CircularWidthMd = "3px",
        CircularWidthLg = "3px",
        CircularWidthXl = "4px",
        CircularCap = "butt",
        CircularGap = "0px",
        CircularIndeterminateRotationDuration = "1400ms",
        CircularIndeterminateProgressDuration = "1400ms",
    };

    // Nav - no pill in Fluent; a left accent bar marks the active item.
    internal static readonly NavTokens Nav = new()
    {
        ActiveWeight = "600",
        BadgeWeight = "600",
        RailLabelLineHeight = "1.15",
        IndicatorRadius = "0",
        ActiveIndicator = "none",
        // Fluent marks the selected item with a left accent bar and no pill (ActiveIndicator is none),
        // so the label is read against the panel surface.
        ActiveColor = "var(--flare-color-on-surface)",
        ActiveLeftBar = "3px solid var(--flare-color-primary)",
        LinkDisabledOpacity = "1",
        // NavItem: 20px icon, 10px block padding around a 20px line.
        IconSize = "1.25rem",
        ItemHeight = "2.5rem",
    };

    // Switch - Fluent's compact track + thin focus offset map to the typed record; the rest of the
    // Fluent switch visual (white thumb sizing/offsets, double focus ring) has no base-token home and
    // stays in Extended below.
    internal static readonly SwitchTokens Switch = new()
    {
        // Fluent's own compact ramp (2:1 track, small thumb), scaled monotonically around its 40x20 md.
        // It does NOT borrow the Material ramp for xs/sm/lg/xl - doing so made md smaller than sm and gave
        // lg/xl a big Material thumb, so the sizes read out of order.
        TrackWidthXs = "2rem",    TrackWidthSm = "2.25rem",   TrackWidthMd = "2.5rem",  TrackWidthLg = "3rem",    TrackWidthXl = "3.5rem",
        TrackHeightXs = "1rem",   TrackHeightSm = "1.125rem", TrackHeightMd = "1.25rem", TrackHeightLg = "1.5rem", TrackHeightXl = "1.75rem",
        TrackOffBg = "var(--flare-color-surface-container-highest)",
        TrackOnBg = "var(--fc-main, var(--flare-color-primary))",
        TrackBorder = "2px solid var(--flare-color-outline)",
        TrackHoverBorderColor = "var(--flare-color-outline)",
        // Small thumb, ~70% of track height, growing a little on check; md matches Fluent's 10/14px exactly.
        ThumbOffSizeXs = "0.5rem",  ThumbOffSizeSm = "0.5625rem", ThumbOffSizeMd = "0.625rem", ThumbOffSizeLg = "0.75rem",   ThumbOffSizeXl = "0.875rem",
        ThumbOnSizeXs = "0.625rem", ThumbOnSizeSm = "0.75rem",    ThumbOnSizeMd = "0.875rem",  ThumbOnSizeLg = "1.0625rem",  ThumbOnSizeXl = "1.25rem",
        // Fluent does not grow the handle on press -> pressed == the normal off/on sizes.
        ThumbPressedOffSizeXs = "0.5rem",  ThumbPressedOffSizeSm = "0.5625rem", ThumbPressedOffSizeMd = "0.625rem", ThumbPressedOffSizeLg = "0.75rem",   ThumbPressedOffSizeXl = "0.875rem",
        ThumbPressedOnSizeXs = "0.625rem", ThumbPressedOnSizeSm = "0.75rem",    ThumbPressedOnSizeMd = "0.875rem",  ThumbPressedOnSizeLg = "1.0625rem",  ThumbPressedOnSizeXl = "1.25rem",
        // Tight 3px inset both ends; on-left = 100% - (thumb-on + 3px), so the thumb never overflows.
        ThumbOffLeftXs = "0.1875rem", ThumbOffLeftSm = "0.1875rem", ThumbOffLeftMd = "0.1875rem", ThumbOffLeftLg = "0.1875rem", ThumbOffLeftXl = "0.1875rem",
        ThumbOnLeftXs = "calc(100% - 0.8125rem)", ThumbOnLeftSm = "calc(100% - 0.9375rem)", ThumbOnLeftMd = "calc(100% - 1.0625rem)", ThumbOnLeftLg = "calc(100% - 1.25rem)", ThumbOnLeftXl = "calc(100% - 1.4375rem)",
        ThumbOffColor = "var(--flare-color-outline)",
        ThumbStateOffColor = "var(--flare-color-on-surface-variant)",
        ThumbStateOnColor = "var(--flare-color-primary-container)",
        IconSize = "1rem",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "1px",
        FocusShadow = "none",
        FocusShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        FocusShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        TrackHoverOffBg = "var(--flare-switch-track-off-bg)",
        TrackHoverOnBg = "var(--flare-switch-track-on-bg)",
        HoverShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        HoverShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        // Fluent flat disabled palette (was MD3-style on-surface opacity mixes).
        DisabledTrackBg = "var(--flare-fluent-disabled-bg, #F0F0F0)",
        DisabledTrackBorder = "var(--flare-fluent-disabled-border, #E0E0E0)",
        DisabledHandleBg = "var(--flare-fluent-disabled-fg, #BDBDBD)",
    };

    /// <summary>
    /// Theme-specific extras with no typed home: Fluent stroke/focus tokens and the Fluent-specific
    /// switch visual the base SwitchTokens does not model.
    /// </summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        [FluentCssVars.StrokeWidthThin] = "1px",
        [FluentCssVars.StrokeWidthThick] = "2px",
        [FluentCssVars.FocusStrokeWidth] = "2px",
        [FluentCssVars.FocusStrokeColor] = "#000000",
        [FluentCssVars.FocusStrokeOuter] = "#FFFFFF",

        // Fluent disabled palette (colorNeutralBackgroundDisabled / ...ForegroundDisabled /
        // ...StrokeDisabled). Fluent uses a flat discrete disabled look, not element opacity.
        // No semantic role maps to these exact greys, so they live here (dark values in DarkExtended).
        [FluentCssVars.DisabledBg] = "#F0F0F0",
        [FluentCssVars.DisabledFg] = "#BDBDBD",
        [FluentCssVars.DisabledBorder] = "#E0E0E0",
        // Subtle button/item hover+pressed fill (colorSubtleBackgroundHover/Pressed). Explicit per-mode
        // because the surface-container role scale inverts between light and dark.
        [FluentCssVars.SubtleHover] = "#F5F5F5",
        [FluentCssVars.SubtlePressed] = "#E0E0E0",
        // Neutral stroke hover/pressed (colorNeutralStroke1Hover/Pressed) - subtle border darken.
        [FluentCssVars.StrokeHover] = "#C7C7C7",
        [FluentCssVars.StrokePressed] = "#B3B3B3",

        // Switch - Fluent UI 2: 1px border, white thumb, hover track fill, double focus ring.
        // The geometry (its compact ramp) lives in SwitchTokens now; only the non-geometry look stays here.
        ["--flare-switch-track-border"] = "1px solid var(--flare-color-secondary)",
        ["--flare-switch-track-off-bg"] = "transparent",
        ["--flare-switch-thumb-off-color"] = "var(--flare-color-secondary)",
        // Hover: subtle fill on track instead of MD3 state-layer bubble
        ["--flare-switch-hover-shadow-off"] = "none",
        ["--flare-switch-hover-shadow-on"] = "none",
        // Fluent keeps its handle color on hover/focus/pressed and has no MD3 focus state layer.
        ["--flare-switch-thumb-state-off-color"] = "var(--flare-color-secondary)",
        ["--flare-switch-thumb-state-on-color"] = "var(--flare-color-on-primary)",
        ["--flare-switch-focus-shadow-off"] = "none",
        ["--flare-switch-focus-shadow-on"] = "none",
        ["--flare-switch-track-hover-off-bg"] = "var(--flare-color-surface-container-low)",
        ["--flare-switch-track-hover-border-color"] = "var(--flare-color-on-surface-variant)",
        ["--flare-switch-track-hover-on-bg"] = "#115EA3",
        // Focus: Fluent double-ring (inner dark + outer light)
        ["--flare-switch-focus-outline"] = "2px solid var(--flare-fluent-focus-stroke-color)",
        ["--flare-switch-focus-shadow"] = "0 0 0 5px var(--flare-fluent-focus-stroke-outer)",
    };


    // Geometry only; shadow color comes from the active ColorScheme via
    // --flare-shadow-umbra / --flare-shadow-penumbra (light 0.14/0.12, dark 0.30/0.25).
    internal static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0 1px 2px var(--flare-shadow-umbra), 0 0px 2px var(--flare-shadow-penumbra)",
        Level2 = "0 2px 4px var(--flare-shadow-umbra), 0 0px 2px var(--flare-shadow-penumbra)",
        Level3 = "0 4px 8px var(--flare-shadow-umbra), 0 0px 2px var(--flare-shadow-penumbra)",
        Level4 = "0 8px 16px var(--flare-shadow-umbra), 0 2px 4px var(--flare-shadow-penumbra)",
        Level5 = "0 16px 32px var(--flare-shadow-umbra), 0 2px 4px var(--flare-shadow-penumbra)",
    };

    // Overlay: Fluent runs denser than MD3 - a dialog sits closer to the edge, and a panel is allowed
    // a little more of the screen because it carries less padding of its own.
    internal static readonly OverlayTokens Overlay = new()
    {
        ViewportInset = "2rem",
        ViewportInsetCompact = "1.5rem",
        PanelMaxBlockSize = "75dvh",
        ScrimLayer = "1200",
        ScrimOpacity = "0.5",
    };

    // Drag: Fluent lifts a dragged object on shadow 16 (elevation 4 here) and shows it at reduced
    // opacity rather than dimming what it left behind as hard as MD3 does; the drop target is a subtle
    // brand tint with a brand outline.
    internal static readonly DragTokens Drag = new()
    {
        SourceOpacity = "0.5",
        PreviewOpacity = "0.85",
        ZoneActiveBackground =
            "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
    };

    // Rating: same neutral baseline (Size defers to the size classes; filled = primary via --fc-main).
    internal static readonly RatingTokens Rating = new()
    {
        // Star ramp: 1 / 1.25 / 1.5 / 2 / 2.5rem.
        SizeXs = "1rem",
        SizeSm = "1.25rem",
        SizeMd = "1.5rem",
        SizeLg = "2rem",
        SizeXl = "2.5rem",
        HoverScale = "1.15",
    };

    // Calendar: same neutral baseline (400px cap, 32dp nav, 48dp cells, primary today/selection).
    internal static readonly CalendarTokens Calendar = new()
    {
        EventPadY = "0.0625rem",
        MaxWidth = "400px",
        MonthMinWidth = "16rem",
        NavBtnSize = "2rem",
        CellMinHeight = "3rem",
        DayNumSize = "1.5rem",
        OtherMonthOpacity = "0.3",
    };

    // Tree: same neutral baseline (24dp indent/handle, 20dp icons, primary-tinted selection).
    internal static readonly TreeTokens Tree = new()
    {
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
    };

    // Stepper: same neutral baseline (32dp circle, 2px connector, primary when complete).
    internal static readonly StepperTokens Stepper = new()
    {
        FocusRingThickness = "2px",
        CircleSize = "2rem",
        CircleIconSize = "1.125rem",
        ConnectorThickness = "2px",
        ConnectorMinLength = "1.5rem",
        StepMinWidth = "5rem",
    };

    // Timeline: same neutral baseline (20dp surface dot, 2px accent ring, 2px outline-variant line).
    internal static readonly TimelineTokens Timeline = new()
    {
        DotSize = "1.25rem",
        DotIconSize = "0.75rem",
        ConnectorWidth = "2rem",
    };

    // Pagination: same neutral baseline (Size defers to the size classes; primary active fill).
    internal static readonly PaginationTokens Pagination = new()
    {
        // Button square ramp: 1.5 / 1.75 / 2.25 / 2.75 / 3.25rem.
        SizeXs = "1.5rem",
        SizeSm = "1.75rem",
        SizeMd = "2.25rem",
        SizeLg = "2.75rem",
        SizeXl = "3.25rem",
        BtnDisabledOpacity = "1",
    };

    // DataGrid baseline geometry/colors. Fluent does not customize the grid, so it carries the same
    // neutral baseline explicitly (self-contained, independent of the MD3 reference).
    internal static readonly DataGridTokens DataGrid = new()
    {
        SortIconSize = "1.125rem",
        SortPrioritySize = "0.625rem",
        FilterIconSize = "1.125rem",
        BoolIconSize = "1.25rem",
        BtnIconSize = "1rem",
        CloseIconSize = "1.125rem",
        ChevronSize = "1.25rem",
        DetailIconSize = "1.25rem",
        TreeToggleSize = "1.25rem",
        CompositeLabelSize = "0.6875rem",
        ResizeHandleWidth = "4px",
        FilterGroupRail = "3px",
        ColumnPickerMinWidth = "160px",
        RowEditingPct = "6%",
        LoadingVeilPct = "55%",
        LoadingDim = "0.6",
    };

    // Spacing scale pinned to the Fluent UI 2 spacing ramp (spacingHorizontal*/spacingVertical*),
    // documented per Fluent token so it stays correct even if the shared default scale changes:
    //   None 0 | XXS 2 | XS 4 | SNudge 6 | S 8 | MNudge 10 | M 12 | L 16 | XL 20 | XXL 24 | XXXL 32 (px).
    // S24/S32 (48/64px) extend past Fluent's named ramp for large layout gaps.
    // Fluent's strokeWidthThin / strokeWidthThick, which is what its surfaces and selection states use.
    internal static readonly BorderTokens Border = new()
    {
        Width = "1px",
        WidthEmphasis = "2px",
        Style = "solid",
        Divider = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline-variant)",
        Outline = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline)",
    };

    // Fluent's own scrollbars are a neutral thumb on a transparent track, slightly firmer at rest than
    // M3's.
    internal static readonly ScrollbarTokens Scrollbar = new()
    {
        Width = "thin",
        Size = "8px",
        Thumb = "color-mix(in srgb, var(--flare-color-on-surface) 36%, transparent)",
        ThumbHover = "color-mix(in srgb, var(--flare-color-on-surface) 52%, transparent)",
        Track = "transparent",
        Radius = "var(--flare-shape-full)",
    };

    internal static readonly SpacingTokens Spacing = new()
    {
        S0 = "0",          // spacingHorizontalNone
        S1 = "0.125rem",   // XXS  (2px)
        S2 = "0.25rem",    // XS   (4px)
        S3 = "0.375rem",   // SNudge (6px)
        S4 = "0.5rem",     // S    (8px)
        S5 = "0.625rem",   // MNudge (10px)
        S6 = "0.75rem",    // M    (12px)
        S8 = "1rem",       // L    (16px)
        S10 = "1.25rem",   // XL   (20px)
        S12 = "1.5rem",    // XXL  (24px)
        S16 = "2rem",      // XXXL (32px)
        S24 = "3rem",      // extension (48px)
        S32 = "4rem",      // extension (64px)
    };

    // The stacking ladder. The order is universal - every design language agrees a dialog covers a
    // navigation bar - so what this states is where the ladder sits and how far apart the rungs are.
    // The gaps leave a component room to lift one of its own parts with calc(var(--rung) + 1), such as
    // a drawer panel over its own scrim, without reaching the rung above.
    internal static readonly LayerTokens Layer = new()
    {
        Chrome = "100",
        Drawer = "200",
        Dropdown = "300",
        Modal = "400",
        Toast = "500",
        Tooltip = "600",
        Drag = "700",
    };

    // BottomNav / ColorPicker - Fluent uses the same neutral baseline as Material for these.
    internal static readonly BottomNavTokens BottomNav = new()
    {
        LabelFontWeight = "500",
        BarHeight = "5rem",
        BorderColor = "var(--flare-color-surface-variant)",
        SafeAreaPadding = "env(safe-area-inset-bottom, 0px)",
        IconSize = "1.5rem",
        IndicatorSize = "2rem",
        IndicatorWidth = "0",
        ItemGap = "var(--flare-spacing-1)",
        ItemDisabledOpacity = "1",
    };

    internal static readonly ColorPickerTokens ColorPicker = new()
    {
    };

    // Tooltip / Avatar - Fluent uses the same neutral baseline as Material.
    internal static readonly TooltipTokens Tooltip = new()
    {
        MaxWidth = "18rem",
        // Fluent tooltip (normal appearance): colorNeutralBackground1 / colorNeutralForeground1 with its drop-shadow (shadow8 geometry). No rich tooltip in Fluent: the rich one is a popover surface at shadow16.
        Bg = "var(--flare-color-surface)",
        Color = "var(--flare-color-on-surface)",
        RichBg = "var(--flare-color-surface)",
        RichColor = "var(--flare-color-on-surface)",
        RichShadow = "0 0 2px var(--flare-shadow-penumbra), 0 8px 16px var(--flare-shadow-umbra)",
        Shadow = "0 0 2px var(--flare-shadow-penumbra), 0 4px 8px var(--flare-shadow-umbra)",
    };

    internal static readonly PopoverTokens Popover = new()
    {
        Radius = "var(--flare-shape-small)",
        // Fluent popover surface: colorNeutralBackground1 / colorNeutralForeground1 at shadow16.
        Bg = "var(--flare-color-surface)",
        Shadow = "0 0 2px var(--flare-shadow-penumbra), 0 8px 16px var(--flare-shadow-umbra)",
    };

    internal static readonly AvatarTokens Avatar = new()
    {
        GroupSpacing = "-0.75rem",
        SizeXs = "1.5rem",  SizeSm = "2rem",   SizeMd = "2.5rem", SizeLg = "3rem",   SizeXl = "4rem",
        FontXs = "0.625rem", FontSm = "0.75rem", FontMd = "1rem", FontLg = "1.125rem", FontXl = "1.5rem",
    };

    // Drawer / Snackbar - Fluent uses the Material baseline geometry (only the Snackbar radius differs).
    internal static readonly DrawerTokens Drawer = new()
    {
        Width = "360px",
        MiniWidth = "72px",
        Border = "1px solid var(--flare-color-outline-variant)",
        SectionBorder = "1px solid var(--flare-color-outline-variant)",
    };

    internal static readonly SnackbarTokens Snackbar = new()
    {
        Radius = "var(--flare-shape-small)",
        MinHeight = "3rem",
        PaddingBlock = "0.875rem",
        CloseOpacity = "0.75",
        MinWidth = "20rem",
        MaxWidth = "36rem",
        ProviderOffset = "0px",
        // Fluent toast: colorNeutralBackground1 / colorNeutralForeground1, the action in colorBrandForeground1, shadow8. Intents use the theme's status containers rather than Material's accent containers.
        Bg = "var(--flare-color-surface)",
        Color = "var(--flare-color-on-surface)",
        ActionColor = "var(--flare-color-primary)",
        Shadow = "0 0 2px var(--flare-shadow-penumbra), 0 4px 8px var(--flare-shadow-umbra)",
        // Severity surfaces, as the snackbar has always drawn them: error on the error role, the others on
        // the secondary, tertiary and primary containers.
        ErrorBg = "var(--flare-color-error-container)",
        ErrorColor = "var(--flare-color-on-error-container)",
        SuccessBg = "var(--flare-color-success-container)",
        SuccessColor = "var(--flare-color-on-success-container)",
        WarningBg = "var(--flare-color-warning-container)",
        WarningColor = "var(--flare-color-on-warning-container)",
        InfoBg = "var(--flare-color-info-container)",
        InfoColor = "var(--flare-color-on-info-container)",
    };

    // Dialog - Fluent: large radius; the rest is the shared Material baseline.
    internal static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-large)",
        IconSize = "1.5rem",
        // Fluent dialog surface: colorNeutralBackground1 / colorNeutralForeground1 at shadow64.
        Bg = "var(--flare-color-surface)",
        Shadow = "0 0 8px var(--flare-shadow-penumbra), 0 32px 64px var(--flare-shadow-umbra)",
        // Fluent's dialog is 600px wide at its default size, with a 20px/28px title.
        MinWidth = "17.5rem",
        MaxWidthXs = "20rem",
        MaxWidthSm = "28rem",
        MaxWidthMd = "37.5rem",
        MaxWidthLg = "50rem",
        MaxWidthXl = "70rem",
        CloseSize = "2rem",
        TitleSize = "1.25rem",
        TitleLineHeight = "1.75rem",
        TitleWeight = "600",
    };

    internal static readonly AppBarTokens AppBar = new()
    {
        Height = "4rem",
        HeightDense = "3rem",
        // Fluent separates chrome surfaces with a thin stroke rather than with elevation, so the bar keeps
        // its rule where M3 drops it.
        Border = "1px solid var(--flare-color-outline-variant)",
    };

    internal static readonly BreadcrumbTokens Breadcrumb = new()
    {
        LinkHoverOpacity = "0.8",
        SeparatorOpacity = "0.5",
    };

    internal static readonly DateTimePickerTokens DateTimePicker = new()
    {
        PanelMinWidth = "280px",
        DialPanelMinWidth = "320px",
        SplitPanelMinWidth = "36rem",
        PaneBasis = "14rem",
        DialPaneWidth = "16.5rem",
        TimeInputWidth = "3rem",
        PanelGap = "1rem",
    };

    internal static readonly FileUploadTokens FileUpload = new()
    {
        DraggingRingWidth = "2px",
        IconSize = "2.5rem",
        ZoneMinHeight = "10rem",
        FileIconSize = "1.125rem",
        RowRadius = "var(--flare-shape-small)",
        RowActiveBg = "color-mix(in srgb, var(--flare-color-primary) 6%, transparent)",
        RowSuccessBg = "color-mix(in srgb, var(--flare-color-tertiary) 6%, transparent)",
        RowErrorBg = "color-mix(in srgb, var(--flare-color-error) 6%, transparent)",
    };

    internal static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    internal static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        // The dense bar's own height; Fluent's spec has no compact app bar to read a number from, so a Fluent-native height is a separate decision.
        AppBarHeightDense = "3rem",
        ContentPadding = "1.5rem 2rem",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "260px",
        AppBarBorder = "1px solid var(--flare-color-outline-variant)",
        DrawerBorder = "1px solid var(--flare-color-outline-variant)",
        AppBarShadow = "none",
        // The shell's three planes, named by the theme rather than picked in core CSS. These values
        // reproduce what core chose before, so nothing moves until a theme says otherwise.
        ContentBg = "transparent",
        // No in-box language separates the drawer from the canvas by depth; they use DrawerBorder.
        DrawerShadowOffset = "0px",
        DrawerShadowBlur = "0px",
        DrawerShadowColor = "transparent",
        // Fluent's Hamburger is an icon-only medium Button - 32px square - around Navigation20Regular,
        // whose three bars are 16px wide, 1px thick and 5px apart.
        AppBarToggleSize = "32px",
        AppBarToggleGlyphWidth = "16px",
        AppBarToggleGlyphHeight = "11px",
        AppBarToggleBarThickness = "1px",
        // The Button's borderRadiusMedium (4px), not a circle.
        AppBarToggleRadius = "var(--flare-shape-small)",
    };

    internal static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
        DisabledOpacity = "1",
    };

    internal static readonly OtpTokens Otp = new()
    {
        CellHeight = "3rem",
        CellWidth = "2.75rem",
        FocusRingWidth = "2px",
        FontSize = "1.25rem",
    };

    internal static readonly PickerTokens Picker = new()
    {
        CalendarPanelPadding = "var(--flare-spacing-6)",
        HeaderGap = "var(--flare-spacing-4)",
        ActionHeight = "var(--flare-btn-height-md)",
        ActionPadding = "var(--flare-spacing-4) 0 0",
        ActionMargin = "var(--flare-spacing-4)",
        ActionBorder = "var(--flare-border-divider)",
        TodayBorder = "none",
        TodayWeight = "700",
        WeekNumberWidth = "1.5rem",
        RangeFieldBasis = "12rem",
        RangeCalendarMaxWidth = "20rem",
        RangeSeparatorSize = "1.25rem",
        OutsideOpacity = "0.8",
        DisabledOpacity = "0.3",
        WeekNumberOpacity = "0.7",
        PanelMinWidth = "18rem",
        PanelRadius = "var(--flare-popover-radius)",
        HeaderHeight = "1.75rem",
        NavIconSize = "var(--flare-btn-icon-size-sm)",
        WeekdayHeight = "1.25rem",
        WeekdayFontSize = "0.75rem",
        DaySize = "2.375rem",
        DayLayerSize = "2.25rem",
        DayFontSize = "0.875rem",
    };

    internal static readonly ScrimTokens Scrim = new()
    {
        Opacity = "0.32",
    };

    // Fluent's touch guidance is 44px, not Material's 48dp. The control keeps its drawn size and the
    // core widens the hit area to this under a coarse pointer.
    internal static readonly TouchTokens Touch = new()
    {
        TargetMin = "44px",
    };

    internal static readonly ScrollTopTokens ScrollTop = new()
    {
        TopSize = "2.75rem",
    };

    internal static readonly PageHeaderTokens PageHeader = new()
    {
        Background = "transparent",
        BorderBottom = "none",
        EyebrowTextTransform = "none",
    };

    internal static readonly SkeletonTokens Skeleton = new()
    {
        PulseMinOpacity = "0.4",
        WaveOpacity = "12%",
    };

    // Fluent's motion is a short decelerated fade, not a spring, and the language has no icon overshoot in
    // it: the glyph crosses over in place, so the geometry axes stay parked and the swap is opacity only.
    internal static readonly IconTokens Icon = new()
    {
        MorphDuration = "var(--flare-motion-duration-state-change)",
        MorphEasing = "var(--flare-motion-easing-decelerate)",
        MorphScale = "1",
        MorphRotate = "0deg",
    };

    internal static readonly StripeTokens Stripe = new()
    {
    };

    internal static readonly TableTokens Table = new()
    {
    };

    internal static readonly TimePickerTokens TimePicker = new()
    {
        PanelMinWidth = "17rem",
        DialPanelMinWidth = "19rem",
        ColumnMaxHeight = "12rem",
        CellMinWidth = "3rem",
        ColumnsSepSize = "1.5rem",
        DialCenterSize = "0.5rem",
        DialHandleSize = "2.25rem",
        DialSize = "16rem",
        DialTrackWidth = "0.125rem",
        PeriodHeight = "auto",
        PeriodWidth = "auto",
        TimeFieldHeight = "auto",
        TimeFieldWidth = "5rem",
        DisplaySize = "2.75rem",
        HeadlineTracking = "0.05em",
        TimeSepSize = "2.5rem",
    };


    /// <summary>The complete Fluent UI 2 design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = new()
    {
        AppBar = AppBar,
        Breadcrumb = Breadcrumb,
        DateTimePicker = DateTimePicker,
        FileUpload = FileUpload,
        Form = Form,
        Layout = Layout,
        Link = Link,
        Otp = Otp,
        Picker = Picker,
        Scrim = Scrim,
        Touch = Touch,
        ScrollTop = ScrollTop,
        PageHeader = PageHeader,
        Skeleton = Skeleton,
        Icon = Icon,
        Stripe = Stripe,
        Splitter = Splitter,
        Table = Table,
        TimePicker = TimePicker,
        Dialog = Dialog,
        Drawer = Drawer,
        Snackbar = Snackbar,
        BottomNav = BottomNav,
        Layer = Layer,
        ColorPicker = ColorPicker,
        Tooltip = Tooltip,
        Avatar = Avatar,
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Spacing = Spacing,
        Border = Border,
        Scrollbar = Scrollbar,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Badge = Badge,
        Alert = Alert,
        Button = Button,
        ButtonGroup = ButtonGroup,
        SplitButton = SplitButton,
        ToggleButton = ToggleButton,
        Fab = Fab,
        Menu = Menu,
        List = List,
        Accordion = Accordion,
        Collapse = Collapse,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        TableOfContents = TableOfContents,
        Slider = Slider,
        // Fluent UI 2 cards: neutral surface backgrounds, a subtle shadow on the elevated variant,
        // a 1px stroke on outline, and a neutral (not brand) "filled-alternative" for tonal.
        Card = new()
        {
            ElevatedBg = "var(--flare-color-surface)",
            Elevation = "var(--flare-elevation-2)",
            ElevationHover = "var(--flare-elevation-3)",
            // Filled cards cast the shadow; the outline and subtle appearances do not.
            FilledElevation = "var(--flare-elevation-2)",
            TonalElevation = "var(--flare-elevation-2)",
            // Only the elevated card lifts on hover; the others keep their resting shadow.
            FilledElevationHover = "var(--flare-elevation-2)",
            OutlinedElevationHover = "none",
            TonalElevationHover = "var(--flare-elevation-2)",
            FocusRing = "var(--flare-focus-ring)",
            FilledBg = "var(--flare-color-surface)",
            OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
            TonalBg = "var(--flare-color-surface-container-high)",
            TonalColor = "var(--flare-color-on-surface)",
            PaddingTop = "12px",
            PaddingRight = "12px",
            PaddingBottom = "12px",
            PaddingLeft = "12px",
            // Neutral baseline members, carried explicitly.
            FilledBorder = "none",
            ContentPadding = "16px",
            HeaderPadding = "16px 16px 0 16px",
            FooterPadding = "8px 16px 16px 16px",
            ActionsPadding = "8px 16px 16px 16px",
            ActionsGap = "8px",
            MediaRadius = "0",
        },
        Input = Input,
        Chart = Chart,
        Gauge = Gauge,
        Progress = Progress,
        Nav = Nav,
        Switch = Switch,
        DataGrid = DataGrid,
        Overlay = Overlay,
        Drag = Drag,
        Rating = Rating,
        Pagination = Pagination,
        Timeline = Timeline,
        Stepper = Stepper,
        Tree = Tree,
        Calendar = Calendar,
        Popover = Popover,
        Extended = Extended,
    };

    public static readonly ColorScheme LightColors = new()
    {
        Primary = "#0F6CBD",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#CFE4F7",
        OnPrimaryContainer = "#003966",
        Secondary = "#616161",
        OnSecondary = "#FFFFFF",
        SecondaryContainer = "#EBEBEB",
        OnSecondaryContainer = "#242424",
        Tertiary = "#8764B8",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#F5F0FF",
        OnTertiaryContainer = "#3B0077",
        Error = "#B10E1C",
        OnError = "#FFFFFF",
        ErrorContainer = "#FDE7E9",
        OnErrorContainer = "#6E0811",
        Success = "#0E700E",
        OnSuccess = "#FFFFFF",
        SuccessContainer = "#DFF6DD",
        OnSuccessContainer = "#052505",
        Warning = "#835B00",
        OnWarning = "#FFFFFF",
        WarningContainer = "#FFF4CE",
        OnWarningContainer = "#3D2C00",
        Info = "#0F6CBD",
        OnInfo = "#FFFFFF",
        InfoContainer = "#EBF3FC",
        OnInfoContainer = "#003966",
        Surface = "#FFFFFF",
        OnSurface = "#242424",
        SurfaceVariant = "#F5F5F5",
        OnSurfaceVariant = "#616161",
        OnSurfaceVariant2 = "#8A8A8A",
        SurfaceContainer = "#EBEBEB",
        SurfaceContainerLowest = "#FFFFFF",
        SurfaceContainerLow = "#F5F5F5",
        SurfaceContainerHigh = "#E0E0E0",
        SurfaceContainerHighest = "#D6D6D6",
        SurfaceBright = "#FFFFFF",
        SurfaceDim = "#D6D6D6",
        PrimaryFixed = "#CFE4F7",
        PrimaryFixedDim = "#479EF5",
        OnPrimaryFixed = "#003966",
        OnPrimaryFixedVariant = "#003966",
        SecondaryFixed = "#EBEBEB",
        SecondaryFixedDim = "#ADADAD",
        OnSecondaryFixed = "#242424",
        OnSecondaryFixedVariant = "#242424",
        TertiaryFixed = "#F5F0FF",
        TertiaryFixedDim = "#C1A3E0",
        OnTertiaryFixed = "#3B0077",
        OnTertiaryFixedVariant = "#3B0077",
        Background = "#F5F5F5",
        OnBackground = "#242424",
        Outline = "#D1D1D1",
        OutlineVariant = "#E0E0E0",
        InverseSurface = "#292929",
        InverseOnSurface = "#F5F5F5",
        InversePrimary = "#82CFFF",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.14)",
        ShadowPenumbra = "rgba(0,0,0,0.12)",
    };

    public static readonly ColorScheme DarkColors = new()
    {
        Primary = "#479EF5",
        OnPrimary = "#00224A",
        PrimaryContainer = "#004578",
        OnPrimaryContainer = "#CFE4F7",
        Secondary = "#ADADAD",
        OnSecondary = "#1A1A1A",
        SecondaryContainer = "#3D3D3D",
        OnSecondaryContainer = "#F0F0F0",
        Tertiary = "#C1A3E0",
        OnTertiary = "#1A0033",
        TertiaryContainer = "#4B2E72",
        OnTertiaryContainer = "#F0E5FF",
        Error = "#F1707B",
        OnError = "#4A0007",
        ErrorContainer = "#750014",
        OnErrorContainer = "#FDE7E9",
        Success = "#5EC75E",
        OnSuccess = "#0B2E0B",
        SuccessContainer = "#0E5814",
        OnSuccessContainer = "#C9F4C9",
        Warning = "#F2C811",
        OnWarning = "#3D3000",
        WarningContainer = "#6B5300",
        OnWarningContainer = "#FFF1B3",
        Info = "#479EF5",
        OnInfo = "#002744",
        InfoContainer = "#004578",
        OnInfoContainer = "#CFE4FA",
        Surface = "#1A1A1A",
        OnSurface = "#F0F0F0",
        SurfaceVariant = "#2A2A2A",
        OnSurfaceVariant = "#ADADAD",
        OnSurfaceVariant2 = "#7A7A7A",
        SurfaceContainer = "#2A2A2A",
        SurfaceContainerLowest = "#141414",
        SurfaceContainerLow = "#222222",
        SurfaceContainerHigh = "#333333",
        SurfaceContainerHighest = "#3D3D3D",
        SurfaceBright = "#3D3D3D",
        SurfaceDim = "#1A1A1A",
        PrimaryFixed = "#CFE4F7",
        PrimaryFixedDim = "#479EF5",
        OnPrimaryFixed = "#003966",
        OnPrimaryFixedVariant = "#003966",
        SecondaryFixed = "#EBEBEB",
        SecondaryFixedDim = "#ADADAD",
        OnSecondaryFixed = "#242424",
        OnSecondaryFixedVariant = "#242424",
        TertiaryFixed = "#F5F0FF",
        TertiaryFixedDim = "#C1A3E0",
        OnTertiaryFixed = "#3B0077",
        OnTertiaryFixedVariant = "#3B0077",
        Background = "#141414",
        OnBackground = "#F0F0F0",
        Outline = "#4A4A4A",
        OutlineVariant = "#3D3D3D",
        InverseSurface = "#F0F0F0",
        InverseOnSurface = "#1A1A1A",
        InversePrimary = "#0F6CBD",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.3)",
        ShadowPenumbra = "rgba(0,0,0,0.25)",
    };

    /// <summary>Dark-mode overrides of the 4 mode-specific Extended keys (focus stroke, switch hover).</summary>
    public static readonly IReadOnlyDictionary<string, string> DarkExtended = BuildDarkExtended();

    private static IReadOnlyDictionary<string, string> BuildDarkExtended()
    {
        var dict = new Dictionary<string, string>(Extended)
        {
            [FluentCssVars.FocusStrokeColor] = "#FFFFFF",
            [FluentCssVars.FocusStrokeOuter] = "#000000",
            ["--flare-switch-track-hover-off-bg"] = "var(--flare-color-surface-container)",
            ["--flare-switch-track-hover-on-bg"] = "#2886D4",
            // Fluent dark disabled palette.
            [FluentCssVars.DisabledBg] = "#141414",
            [FluentCssVars.DisabledFg] = "#5C5C5C",
            [FluentCssVars.DisabledBorder] = "#424242",
            // Fluent dark subtle hover/pressed.
            [FluentCssVars.SubtleHover] = "#383838",
            [FluentCssVars.SubtlePressed] = "#2E2E2E",
            // Fluent dark neutral stroke hover/pressed.
            [FluentCssVars.StrokeHover] = "#757575",
            [FluentCssVars.StrokePressed] = "#6B6B6B",
        };
        return dict;
    }

    private static TypeStyle T(string font, string weight, string size, string height, string spacing) =>
        new() { FontFamily = font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = spacing };
}
