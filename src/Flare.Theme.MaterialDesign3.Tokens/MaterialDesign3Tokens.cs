using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Css;
using Flare.Css.Tokens;

namespace Flare.Theme.MaterialDesign3.Tokens;

/// <summary>
/// Material Design 3 baseline design-token values: the reference <see cref="DesignTokens"/> plus the
/// light/dark <c>ColorScheme</c>s. Themes of the Material 3 lineage (MD3 Expressive and any custom
/// Material 3 theme) build from it via <c>with</c>.
/// <para>
/// It carries Material 3's opinions - the tonal color roles, the pill shapes, the surface-tint
/// elevation - so it is a base only for themes that share them. Material 2 predates all three and
/// ships its own complete set; a theme from another design language should do the same rather than
/// inherit a language it does not speak. It was called <c>MaterialDesignTokens</c> until that name
/// let Material 2 sit on it by accident.
/// </para>
/// </summary>
public class MaterialDesign3Tokens
{
    internal static readonly TypographyTokens Typography = new()
    {
        // No design language names a code face, so this is the generic - twice, because a generic
        // family standing alone makes several engines use their own "monospace default size".
        MonoFont = "monospace, monospace",
        // md.sys.typescale tracking is absolute, in rem: -0.25px on Display Large whatever its size. Material 2
        // states tracking relative to the font size (em); written in em here it grew 3.5x on the display steps.
        DisplayLarge = T("Roboto", "400", "3.5625rem", "4rem", "-0.015625rem"),
        DisplayMedium = T("Roboto", "400", "2.8125rem", "3.25rem", "0rem"),
        DisplaySmall = T("Roboto", "400", "2.25rem", "2.75rem", "0rem"),
        HeadlineLarge = T("Roboto", "400", "2rem", "2.5rem", "0rem"),
        HeadlineMedium = T("Roboto", "400", "1.75rem", "2.25rem", "0rem"),
        HeadlineSmall = T("Roboto", "400", "1.5rem", "2rem", "0rem"),
        TitleLarge = T("Roboto", "400", "1.375rem", "1.75rem", "0rem"),
        TitleMedium = T("Roboto", "500", "1rem", "1.5rem", "0.009375rem"),
        TitleSmall = T("Roboto", "500", "0.875rem", "1.25rem", "0.00625rem"),
        BodyLarge = T("Roboto", "400", "1rem", "1.5rem", "0.03125rem"),
        BodyMedium = T("Roboto", "400", "0.875rem", "1.25rem", "0.015625rem"),
        BodySmall = T("Roboto", "400", "0.75rem", "1rem", "0.025rem"),
        LabelLarge = T("Roboto", "500", "0.875rem", "1.25rem", "0.00625rem"),
        LabelMedium = T("Roboto", "500", "0.75rem", "1rem", "0.03125rem"),
        LabelSmall = T("Roboto", "500", "0.6875rem", "1rem", "0.03125rem"),
    };

    internal static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "4px",
        Small = "8px",
        Medium = "12px",
        Large = "16px",
        ExtraLarge = "28px",
        Full = "9999px",

        // Expressive reshapes its components as you interact with them - a button is a pill at rest,
        // a softer rectangle on hover and a tighter corner when pressed; a toggle button rounds into
        // a squircle on selection. This theme is therefore the one that turns the corner travel on,
        // and it rides the fast spring because a corner change is a spatial move.
        MorphDuration = "var(--flare-motion-duration-spring-fast)",
        MorphEasing = "var(--flare-motion-easing-spring-fast)",
    };

    // Geometry only; shadow color comes from the active ColorScheme via
    // --flare-shadow-umbra (key, alpha 0.3) and --flare-shadow-penumbra (ambient, alpha 0.15).
    internal static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0px 1px 2px var(--flare-shadow-umbra), 0px 1px 3px 1px var(--flare-shadow-penumbra)",
        Level2 = "0px 1px 2px var(--flare-shadow-umbra), 0px 2px 6px 2px var(--flare-shadow-penumbra)",
        Level3 = "0px 4px 8px 3px var(--flare-shadow-penumbra), 0px 1px 3px var(--flare-shadow-umbra)",
        Level4 = "0px 6px 10px 4px var(--flare-shadow-penumbra), 0px 2px 3px var(--flare-shadow-umbra)",
        Level5 = "0px 8px 12px 6px var(--flare-shadow-penumbra), 0px 4px 4px var(--flare-shadow-umbra)",
    };

    // md.sys.motion easings (@material/web tokens). Emphasized is the one exception: the token aliases it to
    // standard because the real curve is a path, and the components themselves play (.3, 0, 0, 1) instead.
    private const string CurveEmphasized = "cubic-bezier(0.3, 0, 0, 1)";
    private const string CurveEmphasizedDecelerate = "cubic-bezier(0.05, 0.7, 0.1, 1)";
    private const string CurveEmphasizedAccelerate = "cubic-bezier(0.3, 0, 0.8, 0.15)";
    private const string CurveStandardDecelerate = "cubic-bezier(0, 0, 0, 1)";
    private const string CurveStandardAccelerate = "cubic-bezier(0.3, 0, 1, 1)";

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

    // md-menu: the surface grows open from nothing over the long2 emphasized travel while the container fades
    // in within the first 50ms; on close it shrinks to 35% and fades in the last 50ms.
    private static readonly SurfaceMotionTokens MenuMotion = new()
    {
        Enter = Still with { Duration = "500ms", Easing = CurveEmphasized, Opacity = "0", Reveal = "0%", FadeDuration = "50ms" },
        Exit = Still with { Duration = "150ms", Easing = CurveEmphasizedAccelerate, Opacity = "0", Reveal = "35%", FadeDelay = "100ms", FadeDuration = "50ms" },
    };

    internal static readonly MotionTokens Motion = new()
    {
        EasingStandard = "cubic-bezier(0.2, 0, 0, 1)",      // standard
        EasingDecelerate = CurveStandardDecelerate,         // standard-decelerate
        EasingAccelerate = CurveStandardAccelerate,         // standard-accelerate
        EasingEmphasized = CurveEmphasized,                 // emphasized (as played by the components)
        EasingEmphasizedDecelerate = CurveEmphasizedDecelerate,
        EasingEmphasizedAccelerate = CurveEmphasizedAccelerate,
        EasingSubtle = "cubic-bezier(0.4, 0, 0.2, 1)",      // legacy
        EasingSubtleDecelerate = "cubic-bezier(0, 0, 0.2, 1)", // legacy-decelerate
        EasingSubtleAccelerate = "cubic-bezier(0.4, 0, 1, 1)", // legacy-accelerate
        EasingLinear = "linear",                            // linear

        DurationStateChange = "100ms", // short2
        DurationSmallMove = "200ms",   // short4
        DurationEnterSmall = "250ms",  // medium1
        DurationEnterMedium = "400ms", // medium4
        DurationEnterLarge = "500ms",  // long2
        DurationExitSmall = "150ms",   // short3
        DurationExitMedium = "200ms",  // short4
        DurationExitLarge = "250ms",   // medium1
        DurationCycle = "1568ms",      // md-circular-progress container rotation
        DurationCycleLong = "2000ms",  // md-linear-progress indeterminate pass

        // Standard-scheme spatial springs (fast 1400 / default 700 / slow 300, damping 0.9), sampled from a
        // damped harmonic oscillator into linear() so the browser plays the real curve. Each is sampled over
        // its own duration below, so the two must move together. The Expressive theme replaces them with its
        // own scheme; the effects springs are critically damped and add nothing a bezier does not.
        EasingSpringFast = "linear(0, 0.0238, 0.0828, 0.1622, 0.2516, 0.3436, 0.4331, 0.517, 0.5936, 0.6619, 0.7218, 0.7734, 0.8174, 0.8544, 0.8852, 0.9104, 0.931, 0.9476, 0.9608, 0.9712, 0.9793, 0.9856, 0.9903, 0.9939, 0.9965, 0.9983, 0.9996, 1.0005, 1.001, 1.0013, 1.0015, 1.0015, 1)",
        EasingSpring = "linear(0, 0.0189, 0.0668, 0.133, 0.2094, 0.2902, 0.3711, 0.4493, 0.5227, 0.5903, 0.6515, 0.706, 0.7541, 0.7959, 0.8321, 0.8629, 0.8891, 0.911, 0.9293, 0.9445, 0.9569, 0.967, 0.9751, 0.9816, 0.9867, 0.9907, 0.9938, 0.9961, 0.9979, 0.9992, 1.0001, 1.0007, 1)",
        EasingSpringSlow = "linear(0, 0.016, 0.0572, 0.1151, 0.1831, 0.2563, 0.3308, 0.4041, 0.4743, 0.5401, 0.6009, 0.6562, 0.7059, 0.7502, 0.7893, 0.8235, 0.8531, 0.8786, 0.9005, 0.919, 0.9347, 0.9478, 0.9587, 0.9677, 0.9751, 0.9811, 0.9859, 0.9898, 0.9928, 0.9952, 0.9971, 0.9985, 1)",
        DurationSpringFast = "200ms",
        DurationSpring = "250ms",
        DurationSpringSlow = "350ms",

        // md-dialog: slides 50px down from above over long2 emphasized while the container grows open from 35%
        // and fades in within 50ms; closes in 150ms emphasized-accelerate, fading in the last 50ms.
        Dialog = new()
        {
            Enter = Still with { Duration = "500ms", Easing = CurveEmphasized, Offset = "50px", Opacity = "0", Reveal = "35%", FadeDuration = "50ms" },
            Exit = Still with { Duration = "150ms", Easing = CurveEmphasizedAccelerate, Offset = "50px", Opacity = "0", Reveal = "35%", FadeDelay = "100ms", FadeDuration = "50ms" },
        },
        // Sheets and the modal drawer enter from their edge on emphasized-decelerate over long2 and leave on
        // emphasized-accelerate over short4 - the M3 transition rule for large surfaces entering and exiting.
        Sheet = new()
        {
            Enter = Still with { Duration = "500ms", Easing = CurveEmphasizedDecelerate, Offset = "100%" },
            Exit = Still with { Duration = "200ms", Easing = CurveEmphasizedAccelerate, Offset = "100%" },
        },
        Menu = MenuMotion,
        // Select, autocomplete and date-picker panels are md-menu surfaces in Material 3.
        Popover = MenuMotion,
        // A plain tooltip waits out a hover, then scales up from 80% and fades in on standard-decelerate.
        Tooltip = new()
        {
            Enter = Still with { Delay = "500ms", Duration = "150ms", Easing = CurveStandardDecelerate, Scale = "0.8", Opacity = "0", FadeDelay = "500ms", FadeDuration = "150ms", FadeEasing = CurveStandardDecelerate },
            Exit = Still with { Opacity = "0", FadeDuration = "100ms", FadeEasing = CurveStandardAccelerate },
        },
        // Snackbar (MDC BaseTransientBottomBar, slide mode): slides its own height over long2 on the emphasized
        // curve both ways, fading in over 150ms and out over medium1.
        Snackbar = new()
        {
            Enter = Still with { Duration = "500ms", Easing = CurveEmphasized, Offset = "100%", Opacity = "0", FadeDuration = "150ms", FadeEasing = CurveEmphasized },
            Exit = Still with { Duration = "500ms", Easing = CurveEmphasized, Offset = "100%", Opacity = "0", FadeDelay = "250ms", FadeDuration = "250ms", FadeEasing = CurveEmphasized },
        },
        Drawer = new()
        {
            Enter = Still with { Duration = "500ms", Easing = CurveEmphasizedDecelerate, Offset = "100%" },
            Exit = Still with { Duration = "200ms", Easing = CurveEmphasizedAccelerate, Offset = "100%" },
        },
    };

    internal static readonly StateTokens State = new()
    {
        HoverOpacity = "0.08",
        SelectedOpacity = "0.12",
        FocusOpacity = "0.10",   // MD3 state layer: focus = 10%
        PressedOpacity = "0.10", // MD3 state layer: pressed = 10%
        DraggedOpacity = "0.16",
        DisabledOpacity = "0.38",
        DisabledContainerOpacity = "0.12",
        // State-layer paint = a translucent currentColor wash at each state's opacity (the Material model).
        HoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        FocusLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        PressedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-pressed-opacity) * 100%), transparent)",
        DraggedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
        // Focus outranks hover in this language, so the pair resolves to the focus wash.
        FocusHoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        SelectedLayer = "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-selected-opacity) * 100%), transparent)",
        SelectedHoverLayer = "color-mix(in srgb, var(--flare-color-primary) 18%, transparent)",
    };

    internal static readonly BadgeTokens Badge = new()
    {
        // MD3 Expressive: pill shape (full radius), compact sizing
        Radius = "var(--flare-shape-full)",
        // The ramp core used to hardcode for xs/sm/lg/xl, now the theme's to name. Values unchanged.
        MinWidthXs = "0.75rem",   MinWidthSm = "0.875rem",   MinWidthMd = "1rem",   MinWidthLg = "1.25rem",  MinWidthXl = "1.5rem",
        HeightXs   = "0.75rem",   HeightSm   = "0.875rem",   HeightMd   = "1rem",   HeightLg   = "1.25rem",  HeightXl   = "1.5rem",
        DotSizeXs  = "0.25rem",   DotSizeSm  = "0.3125rem",  DotSizeMd  = "0.375rem", DotSizeLg = "0.5rem",  DotSizeXl  = "0.625rem",
        PaddingXXs = "0.1875rem", PaddingXSm = "0.1875rem",  PaddingXMd = "0.25rem", PaddingXLg = "0.375rem", PaddingXXl = "0.5rem",
        LabelSizeXs = "0.5625rem",
        LabelSizeSm = "0.625rem",
        LabelSizeMd = "var(--flare-typescale-label-small-size)",
        LabelSizeLg = "var(--flare-typescale-label-medium-size)",
        LabelSizeXl = "var(--flare-typescale-label-large-size)",
        Offset = "0.375rem",
        DotOffset = "0",
    };

    internal static readonly AlertTokens Alert = new()
    {
        BodyOpacity = "0.9",
        CloseOpacity = "0.7",
        // MD3 Expressive: extra-small radius, no border on filled variant
        Radius = "var(--flare-shape-extra-small)",
        BorderWidth = "0",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        // A hairline at every size; the Expressive theme is the one that thickens it as the button grows.
        OutlineWidthXs = "1px",
        OutlineWidthSm = "1px",
        OutlineWidthMd = "1px",
        OutlineWidthLg = "1px",
        OutlineWidthXl = "1px",
        ContainerRadius = "var(--flare-shape-full)",
        // A text button takes half the contained padding: 12dp against 24dp at M (md.comp.text-button),
        // and the same halving across the rest of the ladder.
        TextPaddingInlineXs = "0.375rem",
        TextPaddingInlineSm = "0.5rem",
        TextPaddingInlineMd = "0.75rem",
        TextPaddingInlineLg = "0.875rem",
        TextPaddingInlineXl = "1rem",
        // 8dp between icon and label (md.comp.button.icon-label-space).
        GapXs = "0.5rem",
        GapSm = "0.5rem",
        GapMd = "0.5rem",
        GapLg = "0.5rem",
        GapXl = "0.5rem",

        // Baseline M3 has ONE button, 40dp tall (md.comp.filled-button.container.height), and Md is the
        // size every button takes by default, so Md is that button. The other steps are Flare's ramp
        // around it - the Expressive theme states its own 32/40/56/96/136dp one - with Xl at the FAB's
        // 56dp. Md used to be 48dp, which made every default button, icon button and segmented control
        // 8dp taller than the spec.
        HeightXs = "2rem",       // 32dp
        HeightSm = "2.25rem",    // 36dp
        HeightMd = "2.5rem",     // 40dp (spec)
        HeightLg = "3rem",       // 48dp
        HeightXl = "3.5rem",     // 56dp

        // 24dp leading and trailing space at Md (md-filled-button leading-space).
        PaddingInlineXs = "0.75rem",
        PaddingInlineSm = "1rem",
        PaddingInlineMd = "1.5rem",  // 24dp (spec)
        PaddingInlineLg = "1.75rem",
        PaddingInlineXl = "2rem",

        // Baseline M3 does not reshape a selected button - swapping round and square on selection is an
        // Expressive behaviour, stated in that theme - so a selected button keeps the corners it stands in.
        SelectedRadiusXs = "calc(var(--flare-btn-height-xs) / 2)",
        SelectedRadiusSm = "calc(var(--flare-btn-height-sm) / 2)",
        SelectedRadiusMd = "calc(var(--flare-btn-height-md) / 2)",
        SelectedRadiusLg = "calc(var(--flare-btn-height-lg) / 2)",
        SelectedRadiusXl = "calc(var(--flare-btn-height-xl) / 2)",
        SelectedRadiusSquare = "var(--flare-shape-none)",
        // The fallback pair, which in Material's own table reaches only the text style - the one style it
        // says a toggle should not use. Named anyway so a text toggle reads as on rather than as nothing.
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedColor = "var(--flare-color-on-secondary-container)",

        // The baseline toggle table (md.comp.filled-icon-button / filled-tonal-icon-button /
        // outlined-icon-button): a selected filled or tonal toggle keeps its variant's own container, and
        // it is the UNselected one that moves - to surface-container-highest - so the row does not read as
        // already chosen. Outlined inverts the surface. Baseline has no elevated toggle; the accent fill
        // below is the Expressive reading, kept so an elevated toggle still reads as on.
        ElevatedSelectedBg = "var(--flare-color-primary)",
        ElevatedSelectedColor = "var(--flare-color-on-primary)",
        FilledSelectedBg = "var(--flare-color-primary)",
        FilledSelectedColor = "var(--flare-color-on-primary)",
        TonalSelectedBg = "var(--flare-color-secondary-container)",
        TonalSelectedColor = "var(--flare-color-on-secondary-container)",
        OutlinedSelectedBg = "var(--flare-color-inverse-surface)",
        OutlinedSelectedColor = "var(--flare-color-inverse-on-surface)",
        FilledUnselectedBg = "var(--flare-color-surface-container-highest)",
        FilledUnselectedColor = "var(--flare-color-primary)",
        TonalUnselectedBg = "var(--flare-color-surface-container-highest)",
        TonalUnselectedColor = "var(--flare-color-on-surface-variant)",

        // Per-corner radii: a fully rounded capsule at all 5 sizes, expressed as half the size's own
        // height rather than through the Shape.Full scale.
        //
        // The distinction is invisible at rest and decisive in motion. Shape.Full is 9999px, and the
        // browser clamps a radius that large down to half the height when it paints - so the pill
        // looks right either way. But now that these corners animate toward the hover and pressed
        // radii, the value being interpolated matters: from 9999px nothing changes on screen until
        // the number falls under the clamp, which happens in the last fraction of a percent of the
        // duration and arrives as a snap. Half the height interpolates through values that are
        // actually painted, so the morph is the morph it was specified to be. The split button hit
        // the same trap in its static form.
        RadiusXs = CornerRadiusTokens.All("calc(var(--flare-btn-height-xs) / 2)"),
        RadiusSm = CornerRadiusTokens.All("calc(var(--flare-btn-height-sm) / 2)"),
        RadiusMd = CornerRadiusTokens.All("calc(var(--flare-btn-height-md) / 2)"),
        RadiusLg = CornerRadiusTokens.All("calc(var(--flare-btn-height-lg) / 2)"),
        RadiusXl = CornerRadiusTokens.All("calc(var(--flare-btn-height-xl) / 2)"),

        // Focus outline and shadow settings
        FocusOutline = "3px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FilledHoverShadow = "var(--flare-elevation-1)",
        // Material fades the whole control, so the repaint layer stays out of the way.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        DisabledLayer = "transparent",

        // An icon beside a label is 18dp (md.comp.filled-button.with-icon.icon.size); a lone glyph in an
        // icon button of the same 40dp is 24dp (md.comp.icon-button.icon.size).
        IconSizeXs = "1.125rem", // 18dp
        IconSizeSm = "1.125rem", // 18dp
        IconSizeMd = "1.125rem", // 18dp (spec)
        IconSizeLg = "1.25rem",  // 20dp
        IconSizeXl = "1.5rem",   // 24dp
        IconOnlyIconSizeXs = "1.25rem", // 20dp
        IconOnlyIconSizeSm = "1.5rem",  // 24dp
        IconOnlyIconSizeMd = "1.5rem",  // 24dp (spec)
        IconOnlyIconSizeLg = "1.5rem",  // 24dp
        IconOnlyIconSizeXl = "1.75rem", // 28dp
        // The icon side of a contained button is 16dp against 24dp on the label side (with-leading-icon
        // leading-space). A text button keeps its 12dp on the icon side, so it tucks nothing.
        IconInset = "0.5rem",
        TextIconInset = "0",
        // md.comp.outlined-button: the label is the accent and the stroke the plain outline role.
        OutlinedColor = "var(--flare-color-primary)",
        OutlinedBorderColor = "var(--flare-color-outline)",

        // Baseline M3 uses the one button type style at every size. Ramping the label up to
        // title-medium / headline-small / headline-large is an Expressive behaviour and lives in
        // the Expressive theme, which is what "Expressive overrides only what it needs" means.
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.LabelLarge,
        LabelXl = Typography.LabelLarge,
    };

    // Both group models. Connected: no gap, a 1px negative overlap that collapses adjacent borders into
    // one seam, rounded control ends, flat interior corners. Standard: separate buttons a spacing step
    // apart, each keeping its own corner - MD3 Expressive keeps this and reshapes only the connected
    // half, since a standard group's segments are already the buttons the theme drew.
    // Baseline M3 has no button-group component of its own - the group is an Expressive addition - so
    // this states the plain, unanimated reading: one gap at every size, segments sharing a seam, and a
    // selection that is a repaint rather than a reshape. The Expressive theme replaces the whole record
    // with the spec's per-size ramps.
    internal static readonly ButtonGroupTokens ButtonGroup = new()
    {
        StandardGapXs = "var(--flare-spacing-4)",
        StandardGapSm = "var(--flare-spacing-4)",
        StandardGapMd = "var(--flare-spacing-4)",
        StandardGapLg = "var(--flare-spacing-4)",
        StandardGapXl = "var(--flare-spacing-4)",
        ConnectedGap = "0",
        ConnectedOverlap = "-1px",
        ConnectedOuterRadius = "var(--flare-shape-small)",
        ConnectedSelectedRadius = "var(--flare-shape-small)",
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
        Gap = "0.125rem", // 2dp (between space)

        // Trigger is square at every size. `auto` hands the width to the stylesheet's aspect-ratio, which
        // reads the button's real height; a length here (as the other themes use) would win instead.
        // It cannot forward --_flare-btn-height: this record is emitted on :root, where that per-size
        // variable does not exist, so every size would end up the md fallback width.
        TriggerWidthXs = "auto",
        TriggerWidthSm = "auto",
        TriggerWidthMd = "auto",
        TriggerWidthLg = "auto",
        TriggerWidthXl = "auto",

        // Caret icon = Button icon size at the same size (token forwarded)
        CaretSizeXs = "var(--flare-btn-icon-size-xs)",
        CaretSizeSm = "var(--flare-btn-icon-size-sm)",
        CaretSizeMd = "var(--flare-btn-icon-size-md)",
        CaretSizeLg = "var(--flare-btn-icon-size-lg)",
        CaretSizeXl = "var(--flare-btn-icon-size-xl)",

        // Main: outer LEFT corners = Button radius (forwarded), inner RIGHT = inner corner (spec 4/4/4/8/12dp)
        MainRadiusXs = new() { TopLeft = "var(--flare-btn-radius-xs-top-left)", BottomLeft = "var(--flare-btn-radius-xs-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusSm = new() { TopLeft = "var(--flare-btn-radius-sm-top-left)", BottomLeft = "var(--flare-btn-radius-sm-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusMd = new() { TopLeft = "var(--flare-btn-radius-md-top-left)", BottomLeft = "var(--flare-btn-radius-md-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusLg = new() { TopLeft = "var(--flare-btn-radius-lg-top-left)", BottomLeft = "var(--flare-btn-radius-lg-bottom-left)", TopRight = "0.5rem", BottomRight = "0.5rem" },
        MainRadiusXl = new() { TopLeft = "var(--flare-btn-radius-xl-top-left)", BottomLeft = "var(--flare-btn-radius-xl-bottom-left)", TopRight = "0.75rem", BottomRight = "0.75rem" },

        // Trigger: inner LEFT = inner corner, outer RIGHT = Button radius (forwarded)
        TriggerRadiusXs = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-xs-top-right)", BottomRight = "var(--flare-btn-radius-xs-bottom-right)" },
        TriggerRadiusSm = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-sm-top-right)", BottomRight = "var(--flare-btn-radius-sm-bottom-right)" },
        TriggerRadiusMd = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-md-top-right)", BottomRight = "var(--flare-btn-radius-md-bottom-right)" },
        TriggerRadiusLg = new() { TopLeft = "0.5rem", BottomLeft = "0.5rem", TopRight = "var(--flare-btn-radius-lg-top-right)", BottomRight = "var(--flare-btn-radius-lg-bottom-right)" },
        TriggerRadiusXl = new() { TopLeft = "0.75rem", BottomLeft = "0.75rem", TopRight = "var(--flare-btn-radius-xl-top-right)", BottomRight = "var(--flare-btn-radius-xl-bottom-right)" },
    };

    // MD3: round rest shape, morphs into a squircle on selection (values = record defaults).
    // Only the segmented container's own chrome: the buttons inside it are buttons and read the button
    // family for everything else.
    internal static readonly ToggleButtonTokens ToggleButton = new()
    {
        GroupBorder = "1px solid var(--flare-color-outline)",
        GroupRadius = "var(--flare-shape-full)",
        GroupRadiusVertical = "var(--flare-shape-medium)",
        GroupDivider = "var(--flare-color-outline)",
    };

    // FAB: padding-based sizing, large/medium/extra-large rounding.
    internal static readonly FabTokens Fab = new()
    {
        // A FAB sizes itself as glyph plus padding on both sides, so the icon and the padding together
        // make the container. Lg is the large FAB - its 28dp corner says so - which is
        // md.comp.fab.large.*: a 36dp glyph in a 96dp container, hence 30dp of padding. Md is the
        // baseline FAB (24dp glyph, 56dp container) and Sm the small one (24dp, 40dp).
        IconSizeSm = "1.5rem",    // 24dp -> 40dp container
        IconSizeMd = "1.5rem",    // 24dp -> 56dp container
        IconSizeLg = "2.25rem",   // 36dp -> 96dp container
        PaddingSm = "0.5rem",
        PaddingMd = "1rem",
        PaddingLg = "1.875rem",   // 30dp
        RadiusSm = "var(--flare-shape-medium)",
        RadiusMd = "var(--flare-shape-large)",
        RadiusLg = "var(--flare-shape-extra-large)",
        Gap = "0.75rem",
        Shadow = "var(--flare-elevation-3)",
        HoverShadow = "var(--flare-elevation-4)",
        AnchorOffset = "1.5rem",
    };

    internal static readonly CheckboxTokens Checkbox = new()
    {
        // The size ramp. Core used to hold xs/sm/lg/xl as literals in its own stylesheet, so a theme
        // could move only the middle step; the medium value below is exactly what it was before.
        SizeXs = "0.875rem",
        SizeSm = "1rem",
        SizeMd = "1.125rem",
        SizeLg = "1.375rem",
        SizeXl = "1.625rem",
        StateLayerSize = "2.5rem",
        BorderWidth = "2px",
        Radius = "2px",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        // Material dims the whole control; the shared state value is the one it means.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };
    internal static readonly RadioTokens Radio = new()
    {
        // The size ramp. Core used to hold xs/sm/lg/xl as literals in its own stylesheet, so a theme
        // could move only the middle step; the medium value below is exactly what it was before.
        SizeXs = "1rem",
        SizeSm = "1.125rem",
        SizeMd = "1.25rem",
        SizeLg = "1.5rem",
        SizeXl = "1.75rem",
        StateLayerSize = "2.5rem",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        // The focus this theme already states for the checkbox and the switch - the selection
        // controls answer as one family rather than each drawing its own ring.
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        // Material dims the whole control; the shared state value is the one it means.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };
    internal static readonly ChipTokens Chip = new()
    {
        // md.comp.*-chip.label-text: label-large, including its 500 weight and 0.1px tracking.
        LabelFont = "var(--flare-typescale-label-large-font)",
        LabelWeight = "var(--flare-typescale-label-large-weight)",
        LabelSpacing = "var(--flare-typescale-label-large-spacing)",
        // The outline is the quiet outline-variant role (flat-outline-color); the label is the filter and
        // suggestion chip's on-surface-variant.
        BorderColor = "var(--flare-color-outline-variant)",
        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSizeXs = "var(--flare-typescale-label-small-size)",
        LabelSizeSm = "var(--flare-typescale-label-small-size)",
        LabelSizeMd = "var(--flare-typescale-label-large-size)",
        LabelSizeLg = "var(--flare-typescale-title-small-size)",
        LabelSizeXl = "var(--flare-typescale-title-medium-size)",
        // 18dp at md - md.comp.*-chip.with-icon.icon.size; the ramp follows the label scale.
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
        PaddingInlineXs = "var(--flare-spacing-4)",
        PaddingInlineSm = "var(--flare-spacing-5)",
        PaddingInlineMd = "var(--flare-spacing-8)",
        PaddingInlineLg = "var(--flare-spacing-10)",
        PaddingInlineXl = "var(--flare-spacing-12)",
        FilledBg = "var(--flare-color-surface-container-high)",
        ElevatedBg = "var(--flare-color-surface-container-low)",
        Radius = "var(--flare-shape-small)",  // MD3 = 8dp
        Height = "2rem",                       // MD3 = 32dp
    };
    internal static readonly TabsTokens Tabs = new()
    {
        // md.comp.primary-navigation-tab.with-icon.icon.size: 24dp.
        IconSize = "1.5rem",
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        LabelFont = "var(--flare-typescale-label-large-font)",
        LabelSize = "var(--flare-typescale-label-large-size)",
        LabelSpacing = "var(--flare-typescale-label-large-spacing)",
        LabelWeight = "var(--flare-typescale-label-large-weight)",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "3px",
        ActiveColor = "var(--flare-color-primary)",
        // Secondary navigation tab: 2dp indicator, active label on-surface (spec md.comp
        // .secondary-navigation-tab.active-indicator.height / .label-text.color).
        SecondaryIndicatorThickness = "2px",
        SecondaryActiveColor = "var(--flare-color-on-surface)",
        // Spec is on-surface-VARIANT at rest (#49454F) for both tab levels, and on-surface only once
        // the tab is hovered or focused. This was on-surface, so an inactive tab read as dark as the
        // active one - invisible on a primary strip, where the accent carries the distinction, but
        // plainly wrong on a secondary strip, where the label colour is all there is besides a 2dp
        // rail. The hover darkening is not modelled yet; the state layer signals hover meanwhile.
        InactiveColor = "var(--flare-color-on-surface-variant)",
        DividerColor = "var(--flare-color-surface-variant)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedFg = "var(--flare-color-on-secondary-container)",
        FilledBg = "var(--flare-color-primary)",
        FilledFg = "var(--flare-color-on-primary)",
        TrackBg = "transparent",
        PillRadius = "var(--flare-shape-full)",
        TabDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ScrollDisabledOpacity = "var(--flare-state-disabled-opacity)",
        // The label and its padding decide the height here; no minimum width.
        TabHeight = "3rem",                          // md.comp.primary-navigation-tab.container.height 48dp
        TabMinWidth = "0",
        TabPaddingInline = "var(--flare-spacing-8)", // md-tabs 16dp
    };

    // Menu (MD3 Expressive "Menus"): container 16dp (shape-large), elevation 3,
    // item 4dp (end items 12dp), label-large text, vertical 8dp, gap 2dp, focus-ring secondary.
    /// <summary>List container and row tokens.</summary>
    internal static readonly ListTokens List = new()
    {
        Bg = "var(--flare-color-surface)",
        Radius = "var(--flare-shape-medium)",
        Divider = "1px solid var(--flare-color-outline-variant)",
        ItemHeight = "3.5rem",
        ItemHeightTwoLine = "4.5rem",
        ItemHeightDense = "3rem",
        ItemHeightTwoLineDense = "3.5rem",
        ItemPaddingBlock = "var(--flare-spacing-6)",
        ItemPaddingBlockDense = "var(--flare-spacing-3)",
        ItemPaddingInline = "var(--flare-spacing-8)",
        ItemGap = "var(--flare-spacing-8)",
        ItemContentGap = "var(--flare-spacing-1)",
        ItemRadius = "0",
        ItemLabelFont = "var(--flare-typescale-body-large-font)",
        ItemLabelSize = "var(--flare-typescale-body-large-size)",
        ItemColor = "var(--flare-color-on-surface)",
        ItemTrailingColor = "var(--flare-color-on-surface-variant)",
        ItemSelectedBg = "var(--flare-color-secondary-container)",
        ItemSelectedColor = "var(--flare-color-on-secondary-container)",
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ItemIconSize = "1.5rem", // md.comp.list.list-item.leading-icon.size / trailing-icon.size 24dp
    };

    /// <summary>Accordion container, header and body tokens.</summary>
    internal static readonly AccordionTokens Accordion = new()
    {
        Border = "1px solid var(--flare-color-outline-variant)",
        Radius = "var(--flare-shape-medium)",
        PanelDivider = "1px solid var(--flare-color-outline-variant)",
        HeaderBg = "var(--flare-color-surface)",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderPaddingBlock = "var(--flare-spacing-8)",
        HeaderPaddingInline = "var(--flare-spacing-12)",
        HeaderGap = "var(--flare-spacing-4)",
        HeaderLabelFont = "var(--flare-typescale-title-small-font)",
        HeaderLabelSize = "var(--flare-typescale-title-small-size)",
        HeaderLabelWeight = "var(--flare-typescale-title-small-weight)",
        HeaderDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconSize = "0.875rem",
        BodyPaddingBlock = "var(--flare-spacing-8)",
        BodyPaddingInline = "var(--flare-spacing-12)",
        BodyColor = "var(--flare-color-on-surface-variant)",
    };

    /// <summary>Standalone collapse header tokens.</summary>
    internal static readonly CollapseTokens Collapse = new()
    {
        HeaderBg = "transparent",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderRadius = "var(--flare-shape-small)",
        HeaderPaddingBlock = "var(--flare-spacing-6)",
        HeaderPaddingInline = "var(--flare-spacing-8)",
        HeaderGap = "var(--flare-spacing-4)",
        HeaderLabelFont = "var(--flare-typescale-title-small-font)",
        HeaderLabelSize = "var(--flare-typescale-title-small-size)",
        HeaderLabelWeight = "var(--flare-typescale-title-small-weight)",
        HeaderDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconColor = "var(--flare-color-on-surface-variant)",
    };

    internal static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        // Flush against the anchor, as material-web md-menu places it (x/y offset 0).
        PanelOffset = "0px",
        // Baseline M3 is one classic surface: a 4dp panel with square items and no group islands.
        // The 16dp panel, the rounded items and the floating group sections are Expressive, and the
        // Expressive theme states them.
        PanelRadius = "var(--flare-shape-extra-small)", // 4dp
        PanelMinWidth = "7rem",                       // 112dp
        PanelShadow = "var(--flare-elevation-2)",     // md.comp.menu.container.elevation level2
        PanelPaddingInline = "0",                     // md-menu: items run edge to edge
        PanelPaddingBlock = "0.5rem",                 // md-menu top/bottom space 8dp
        ItemHeight = "3rem",                          // item height 48dp (MD3 list-item)
        ItemPaddingBlock = "0.5rem",                  // top/bottom 8dp
        // Dense: the value core used to hardcode in menuitem.css, so a theme could style a normal menu
        // but never a dense one. Unchanged.
        ItemPaddingBlockDense = "0.375rem",
        ItemGapDense = "0.5rem",
        ItemGapBetween = "0",
        ItemRadius = "0",
        ItemRadiusEnd = "0",
        GroupRadius = "0",
        GroupPadding = "0",
        // Groups are sections of the one panel, marked by spacing alone - not the Expressive
        // "island" model, where each group is its own rounded, elevated surface on a transparent
        // backing panel.
        GroupBg = "transparent",
        GroupGap = "0",
        GroupShadow = "none",
        GroupedPanelBg = "var(--flare-color-surface-container)",
        GroupedPanelShadow = "var(--flare-elevation-2)",
        ItemLabelFont = "var(--flare-typescale-label-large-font)",
        ItemLabelWeight = "var(--flare-typescale-label-large-weight)",
        ItemLabelSize = "var(--flare-typescale-label-large-size)",
        ItemLabelHeight = "var(--flare-typescale-label-large-height)",
        ItemLabelSpacing = "var(--flare-typescale-label-large-spacing)",
        // Neutral baseline members (formerly the record defaults) - carried explicitly now that
        // the core record no longer ships defaults.
        EnterAnimation = "flare-menu-in",
        PanelBg = "var(--flare-color-surface-container)",
        ItemPaddingInline = "1rem",
        ItemGap = "0.75rem",
        ItemIconSize = "1.5rem", // md.comp.menu.list-item.*-icon.size 24dp
        ItemFocusRingColor = "var(--flare-color-secondary)",
        ItemFocusRingThickness = "3px",
        ItemFocusRingOffset = "-3px",
        // Material dims a disabled item.
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // Input - filled style (md.comp.filled-text-field): surface-container-highest container, 56dp
    // height (1rem block padding + 24px line), 1dp on-surface-variant active indicator, hover ->
    // on-surface indicator + 8% state layer, focus -> 3dp primary (Expressive).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface-container-highest)",
        BorderColor = "transparent",                  // filled default: no side border, colour only
        OutlinedRadius = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0",
        BorderBottomColor = "var(--flare-color-on-surface-variant)",
        // Focus = the 2dp primary active indicator (md.comp.filled-text-field.focus.active-indicator.height):
        // the 1dp bottom border turns primary and a 1dp inset shadow adds the second pixel without a jump.
        FocusRing = "inset 0 -1px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusBorderBottomColor = "var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        HoverBorderBottomColor = "var(--flare-color-on-surface)",
        HoverStateLayer = "linear-gradient(color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent), color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent))",
        PaddingXs = "0.1875rem 0.5rem",
        PaddingSm = "0.375rem 0.625rem",
        PaddingMd = "1rem 1rem",                      // the M3 56dp field
        // Large and extra-large are measured up from the 56dp medium. They used to be 14px and 18px of
        // block padding, authored in core against a shorter medium, which left Large SHORTER than Medium
        // under this theme.
        PaddingLg = "1.25rem 1.125rem",
        PaddingXl = "1.5rem 1.25rem",
        // Height ramp. M3 puts the text field AND the select at 56dp, so Md is the spec number and the
        // rest are measured off the padding ramp above - each step is the tallest control that step used
        // to produce, so the family converges upward and no control shrinks. Before these existed the
        // steps disagreed by 2px (the trigger was taller) and by 5px at Xl (the text field was).
        HeightXs = "1.875rem", // 30px
        HeightSm = "2.25rem",  // 36px
        HeightMd = "3.5rem",   // 56px - the M3 field height
        HeightLg = "4rem",     // 64px
        HeightXl = "4.75rem",  // 76px
        IconSize = "1.5rem",                          // leading/trailing icon 24dp
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        ErrorHoverIndicator = "var(--flare-color-on-error-container)", // error.hover.active-indicator.color
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // Chart. The categorical palette is built from the theme's OWN hues rather than from fixed ink, so a
    // dynamic palette and a mode switch both carry through to the plot: three source hues (primary,
    // tertiary, secondary) crossed with four tonal treatments - pure, deepened toward on-surface, lightened
    // toward surface, and blended with a neighbour. The error role is deliberately ABSENT: red carries a
    // meaning here that a fourth series does not have. A theme that wants twelve genuinely distinct hues
    // overrides these twelve values; that is what the tokens are for.
    // Sizes without a unit are viewBox units: the gauge is 200 wide and scales to its container, so
    // these are proportions of the gauge rather than pixels. Band colors are absent by design - a band is
    // a FlareZone carrying its own FlareColor, and which part of a scale is "bad" is the application's
    // statement about its data, not the theme's.
    internal static readonly GaugeTokens Gauge = new()
    {
        TrackColor = "var(--flare-color-surface-container-highest)",
        TrackWidth = "14",
        TrackCap = "round",

        FillColor = "var(--flare-color-primary)",
        FillWidth = "14",

        NeedleColor = "var(--flare-color-on-surface)",
        NeedleWidth = "3",
        NeedleLength = "0.78",
        PivotColor = "var(--flare-color-on-surface)",
        PivotRadius = "5",

        TickColor = "var(--flare-color-outline)",
        TickWidth = "1.5",
        TickLength = "7",
        TickMinorColor = "var(--flare-color-outline-variant)",
        TickMinorWidth = "1",
        TickMinorLength = "4",
        TickGap = "5",

        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSize = "9",
        ValueColor = "var(--flare-color-on-surface)",
        ValueSize = "var(--flare-typescale-headline-small-size)",
        ValueWeight = "500",

        BandOpacity = "0.55",
        TargetColor = "var(--flare-color-on-surface)",
        TargetWidth = "2.5",
    };

    internal static readonly ChartTokens Chart = new()
    {
        Series1 = "var(--flare-color-primary)",
        Series2 = "var(--flare-color-tertiary)",
        Series3 = "var(--flare-color-secondary)",
        Series4 = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-tertiary))",
        Series5 = "color-mix(in srgb, var(--flare-color-secondary) 50%, var(--flare-color-tertiary))",
        Series6 = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-secondary))",
        Series7 = "color-mix(in srgb, var(--flare-color-primary) 65%, var(--flare-color-on-surface))",
        Series8 = "color-mix(in srgb, var(--flare-color-tertiary) 65%, var(--flare-color-on-surface))",
        Series9 = "color-mix(in srgb, var(--flare-color-secondary) 65%, var(--flare-color-on-surface))",
        Series10 = "color-mix(in srgb, var(--flare-color-primary) 55%, var(--flare-color-surface))",
        Series11 = "color-mix(in srgb, var(--flare-color-tertiary) 55%, var(--flare-color-surface))",
        Series12 = "color-mix(in srgb, var(--flare-color-secondary) 55%, var(--flare-color-surface))",

        // Marks. Sizes without a unit are viewBox units: the plot is 400 wide and scales to its container,
        // so these are proportions of the chart, not pixels.
        LineWidth = "2",
        LineCap = "round",
        PointRadius = "2.5",
        PointOpacity = "0.6",
        BubbleMinRadius = "4",
        BubbleMaxRadius = "24",
        BarRadius = "2",
        AreaOpacity = "0.35",
        RadarFillOpacity = "0.2",
        WedgeOpacity = "0.7",
        SliceStrokeColor = "var(--flare-color-surface)",
        SliceStrokeWidth = "1.5",

        // Sequential ramp: one hue, intensity carries the value. The floor keeps a zero cell visible as a
        // cell rather than as a hole in the grid.
        RampColor = "var(--flare-color-primary)",
        RampMinOpacity = "0.12",
        RampMaxOpacity = "1",
        CellRadius = "2",
        CellGap = "2",
        CellHoverOpacity = "0.85",

        // Axes. A grid line is lighter than an outline: it sits behind the data and must not compete.
        GridColor = "var(--flare-color-outline-variant)",
        GridWidth = "0.5",
        GridDash = "none",
        GridMinorColor = "color-mix(in srgb, var(--flare-color-outline-variant) 45%, transparent)",
        GridMinorWidth = "0.5",
        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSize = "var(--flare-typescale-label-small-size)", // the smallest step of the type scale
        ValueColor = "var(--flare-color-on-surface-variant)",
        ValueOnFillColor = "var(--flare-color-surface)",
        ValueSize = "8px",
        AxisTitleColor = "var(--flare-color-on-surface-variant)",
        AxisTitleSize = "10px",

        Surface = "var(--flare-color-surface)",
        Radius = "var(--flare-shape-medium)",
        BorderWidth = "1px",
        BorderColor = "var(--flare-color-outline-variant)",
        Padding = "var(--flare-spacing-8)",
        Gap = "var(--flare-spacing-4)",

        LegendGap = "var(--flare-spacing-6)",
        LegendItemGap = "var(--flare-spacing-3)",
        LegendDotSize = "0.625rem",
        LegendDotRadius = "var(--flare-shape-full)",
        LegendSize = "var(--flare-typescale-label-small-size)",
        LegendColor = "var(--flare-color-on-surface-variant)",
        LegendOffOpacity = "0.4",

        // Overlays. The annotation default IS the error role - a threshold line means what red means,
        // which is exactly why a data series must not borrow it.
        TrendWidth = "1.5",
        TrendDash = "5 4",
        TrendOpacity = "0.7",
        AnnotationColor = "var(--flare-color-error)",
        AnnotationWidth = "1.5",
        AnnotationDash = "4 3",
        AnnotationBandOpacity = "0.12",
        AnnotationArrowSize = "7",
        AnnotationPointRadius = "3.5",

        // Dash rhythm is measured in stroke widths, not absolute units, because a round cap adds half a
        // stroke width to BOTH ends of every dash: the painted dash grows by one width and the painted gap
        // shrinks by one. Absolute arrays therefore only hold for the width they were authored against -
        // at Expressive's 3 the old "6 4" painted 9-long dashes separated by 1, i.e. a solid line.
        // Authored 2w/3w paints as 3w of ink and 2w of air at any width, and a dash shorter than the cap
        // paints as a round dot exactly one stroke wide, which is what Dotted has to be.
        LineDashDashed = "calc(var(--flare-chart-line-width) * 2) calc(var(--flare-chart-line-width) * 3)",
        LineDashDotted = "0.1 calc(var(--flare-chart-line-width) * 3)",
        LineDashDashDot = "calc(var(--flare-chart-line-width) * 2) calc(var(--flare-chart-line-width) * 3)" +
                          " 0.1 calc(var(--flare-chart-line-width) * 3)",

        ZoomSelectionFill = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        ZoomSelectionStroke = "var(--flare-color-primary)",
    };

    // Progress - shared Material geometry. Optional Expressive wave behavior is supplied by the
    // Expressive theme package, so this baseline token bundle stays reusable by flat themes.
    internal static readonly ProgressTokens Progress = new()
    {
        // Size ramp. The spec names two steps of each - linear 4dp with an 8dp "thick", circular 40dp with a
        // 52dp "thick" - and they anchor Md (the default) and the step above it. The ramp runs BOTH ways
        // from Md because a progress indicator's natural default sits mid-scale, not at the bottom: the
        // hairline steps below it are what an inline spinner or a dense table row needs.
        LinearHeightXs = "2px",
        LinearHeightSm = "3px",
        LinearHeightMd = "4px",   // spec: linear height (the default)
        LinearHeightLg = "6px",
        LinearHeightXl = "8px",   // spec: linear thick height
        // Centred content: the ramp follows the indicator sizes, so a label dropped into the
        // smallest ring still fits it.
        ContentColor = "var(--flare-color-on-surface)",
        ContentSizeXs = "0.5rem",
        ContentSizeSm = "0.625rem",
        ContentSizeMd = "0.75rem",
        ContentSizeLg = "1rem",
        ContentSizeXl = "1.25rem",
        TrackRadius = "var(--flare-shape-full)",
        Gap = "4px",
        LinearIndeterminateDuration = "1500ms",
        LinearIndeterminateEasing = "var(--flare-motion-easing-standard)",
        StopSize = "4px",
        StopInset = "0px",
        StopColor = "var(--fc-main, var(--flare-color-primary))",
        BufferOpacity = "30%",
        CircularSizeXs = "24px",
        CircularSizeSm = "32px",
        CircularSizeMd = "40px",  // spec: circular size (the default)
        CircularSizeLg = "52px",  // spec: circular thick size
        CircularSizeXl = "64px",
        // The spec pins the circular stroke at 4dp for both the default and the thick ring, so it holds
        // flat across md..lg and only moves where the spec stops speaking.
        CircularWidthXs = "3px",
        CircularWidthSm = "3px",
        CircularWidthMd = "4px",  // spec: circular active-indicator thickness
        CircularWidthLg = "8px",  // spec: thick-active-indicator-thickness 8dp on the thick ring
        CircularWidthXl = "5px",
        CircularCap = "round",
        CircularGap = "4px",
        CircularIndeterminateRotationDuration = "1400ms",
        CircularIndeterminateProgressDuration = "1400ms",
    };

    /// <summary>
    /// Theme-specific extras with no typed home in the core schema. Everything that maps to a typed
    /// component token (dialog/input/menu/progress/snackbar/popover/nav) now lives on the matching
    /// record above, not here.
    /// </summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        // DateTimePicker panels (Variant.Panels): MD3 spacing between the calendar and time panes.
        ["--flare-datetimepicker-panel-gap"] = "1.5rem",
    };


    internal static readonly NavTokens Nav = new()
    {
        ActiveWeight = "700",
        BadgeWeight = "var(--flare-typescale-label-small-weight)", // the badge label is label-small
        RailLabelLineHeight = "1.15",
        ItemRadius = "var(--flare-shape-extra-small)",
        IndicatorRadius = "var(--flare-shape-full)",
        ActiveIndicator = "var(--flare-color-secondary-container)",
        // The indicator is a tinted pill, so the label is read against that container.
        ActiveColor = "var(--flare-color-on-secondary-container)",
        ItemColor = "var(--flare-color-on-surface-variant)",
        ItemHoverColor = "var(--flare-color-on-surface)",
        GroupColor = "var(--flare-color-on-surface-variant)",
        MetaColor = "var(--flare-color-on-surface-variant)",
        ActiveLeftBar = "none",
        LinkDisabledOpacity = "var(--flare-state-disabled-opacity)",
        // md.comp.navigation-drawer: 24dp icon in a 56dp active indicator.
        IconSize = "1.5rem",
        ItemHeight = "3.5rem",
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

    internal static readonly BottomNavTokens BottomNav = new()
    {
        BarHeight = "5rem",
        BarBg = "var(--flare-color-surface-container)",
        // The tonal container is the edge (md.comp.navigation-bar has no divider); the 1px stays reserved.
        BorderColor = "transparent",
        SafeAreaPadding = "env(safe-area-inset-bottom, 0px)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        ActiveColor = "var(--flare-color-on-secondary-container)",
        IconSize = "1.5rem",
        LabelFontSize = "var(--flare-typescale-label-medium-size)",
        LabelFontWeight = "500",
        LabelFontWeightActive = "700",
        IndicatorBg = "var(--flare-nav-active-indicator)",
        IndicatorRadius = "var(--flare-nav-indicator-radius)",
        IndicatorSize = "2rem",
        IndicatorWidth = "4rem",           // md.comp.navigation-bar.active-indicator.width 64dp
        ItemGap = "var(--flare-spacing-2)", // icon-label space 4dp
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    internal static readonly TableOfContentsTokens TableOfContents = new()
    {
        ActiveWeight = "700", // weight-prominent, as the active navigation item
        HoverBgOpacity = "40%",
        LineHeight = "1.4",
        TitleTracking = "0.05em",
        TitleWeight = "var(--flare-typescale-title-small-weight)",
        ActiveColor = "var(--flare-color-on-secondary-container)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        TitleColor = "var(--flare-color-on-surface-variant)",
        RailColor = "transparent",
        RailWidth = "0",
        ActiveBg = "var(--flare-color-secondary-container)",
        ActiveRadius = "var(--flare-shape-full)",
        // "0px", not "0": the marker is also pulled over the rail with calc(-1 * var(--flare-toc-marker-width)),
        // and a unitless value there yields a <number>, which margin rejects. At zero the dropped declaration
        // happens to compute the same margin, so nothing shows - but the trap is real the moment it is read
        // as anything other than a length, and the sibling themes already write the unit.
        MarkerWidth = "0px",
        LinkPadX = "0.75rem",
        Indent = "0.75rem",
    };

    internal static readonly ColorPickerTokens ColorPicker = new()
    {
        CheckerColor = "var(--flare-color-outline-variant)",
        ThumbBg = "var(--flare-color-surface)",
        ThumbBorderColor = "var(--flare-color-outline)",
    };

    internal static readonly TooltipTokens Tooltip = new()
    {
        MaxWidth = "18rem",
        Offset = "0.5rem",
        Bg = "var(--flare-color-inverse-surface)",
        Color = "var(--flare-color-inverse-on-surface)",
        Padding = "var(--flare-spacing-3) var(--flare-spacing-6)",
        // md.comp.rich-tooltip: surface-container at level 2, supporting text on-surface-variant.
        RichBg = "var(--flare-color-surface-container)",
        RichColor = "var(--flare-color-on-surface-variant)",
        RichShadow = "var(--flare-elevation-2)",
        RichPadding = "var(--flare-spacing-6) var(--flare-spacing-8)",
    };

    internal static readonly PopoverTokens Popover = new()
    {
        Radius = "var(--flare-shape-medium)",
        Offset = "0.5rem",
        Bg = "var(--flare-color-surface-container)",
        Color = "var(--flare-color-on-surface)",
        Shadow = "var(--flare-elevation-2)",
    };

    internal static readonly AvatarTokens Avatar = new()
    {
        GroupSpacing = "-0.75rem",
        GroupBorderWidth = "2px",
        GroupBorderColor = "var(--flare-color-surface)",
        OverflowBg = "var(--flare-color-surface-container-highest)",
        OverflowColor = "var(--flare-color-on-surface-variant)",
    };

    internal static readonly DrawerTokens Drawer = new()
    {
        Width = "360px",
        MiniWidth = "72px",
        // The M3 navigation drawer is a surface-container-low panel against a surface background; the tone
        // step is the separation, and the spec draws no divider on either edge.
        Border = "none",
        SectionBorder = "none",
    };

    internal static readonly SnackbarTokens Snackbar = new()
    {
        Radius = "var(--flare-shape-extra-small)",
        MinHeight = "3rem",
        PaddingBlock = "0.875rem",
        ProviderInset = "1.5rem",
        CloseOpacity = "0.75",
        MinWidth = "20rem",
        MaxWidth = "36rem",
        PaddingInline = "var(--flare-spacing-10)",
        Bg = "var(--flare-color-inverse-surface)",
        Color = "var(--flare-color-inverse-on-surface)",
        ActionColor = "var(--flare-color-inverse-primary)",
        Shadow = "var(--flare-elevation-3)",
    };

    // Splitter: an 8dp gutter - comfortable to grab without reading as a divider - carrying a 2dp grip
    // mark, tinted with the primary wash on hover the way the rest of the theme signals an active target.
    internal static readonly SplitterTokens Splitter = new()
    {
        GutterSize = "0.5rem",
        GripThickness = "2px",
        GripLength = "1.75rem",
        Color = "var(--flare-color-surface-variant)",
        HoverColor = "color-mix(in srgb, var(--flare-color-primary) 24%, transparent)",
        IconSize = "1.125rem",
        IconColor = "var(--flare-color-on-surface-variant)",
    };

    internal static readonly SliderTokens Slider = new()
    {
        // Size ramp: track 16/24/40/56/96dp, handle 44/44/52/68/108dp, track shape 8/8/12/16/28dp.
        TrackHeightXs = "1rem",
        TrackHeightSm = "1.5rem",
        TrackHeightMd = "2.5rem",
        TrackHeightLg = "3.5rem",
        TrackHeightXl = "6rem",
        TrackRadiusXs = "0.5rem",
        TrackRadiusSm = "0.5rem",
        TrackRadiusMd = "0.75rem",
        TrackRadiusLg = "1rem",
        TrackRadiusXl = "1.75rem",
        // md.comp.slider.<size>.active.handle.height reads 44/44/44/68/108dp - the handle stays the
        // same height for the first three steps and only grows for large and extra-large, which is
        // why the ramp looks flat at the bottom. Md was 3.25rem against the table's 44dp.
        HandleHeightXs = "2.75rem",  // 44dp
        HandleHeightSm = "2.75rem",  // 44dp
        HandleHeightMd = "2.75rem",  // 44dp
        HandleHeightLg = "4.25rem",  // 68dp
        HandleHeightXl = "6.75rem",  // 108dp
        // Flanking StartIcon/EndIcon ramp.
        IconSizeXs = "20px",
        IconSizeSm = "22px",
        IconSizeMd = "24px",
        IconSizeLg = "24px",
        IconSizeXl = "32px",
        Length = "12rem",
        GapRadius = "2px",
        Gap = "6px",
        HandleWidth = "4px",
        HandlePressedWidth = "2px",
        HandleRadius = "var(--flare-shape-full)",
        HandleClipPath = "none",
        HandleBorderWidth = "0px",
        // Follow the per-instance Color: --fc-main is the local accent, falling back to the role.
        HandleFill = "var(--fc-main, var(--flare-color-primary))",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-secondary-container)",
        StateLayerSize = "40px",
        StateHoverOpacity = "var(--flare-state-hover-opacity)",
        StatePressedOpacity = "var(--flare-state-pressed-opacity)",
        FocusOutline = "none",
        FocusOutlineOffset = "0px",
        StopColor = "var(--flare-color-on-secondary-container)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "4px",
        ValueBg = "var(--flare-color-inverse-surface)",
        ValueColor = "var(--flare-color-inverse-on-surface)",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        // Disabled is a fade of the content colour, and the two halves fade by different amounts so
        // the filled portion is still readable against the rest.
        DisabledActiveColor = "color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-disabled-opacity) * 100%), transparent)",
        DisabledInactiveColor = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
    };

    internal static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-extra-large)",
        IconSize = "1.5rem",
        Bg = "var(--flare-color-surface-container-high)",
        Color = "var(--flare-color-on-surface)",
        Shadow = "var(--flare-elevation-3)",
    };

    // DataGrid baseline geometry/colors. No theme customizes the grid, so both references carry the
    // same neutral baseline explicitly now that the core record ships no defaults.
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
        CompositeLabelSize = "var(--flare-typescale-label-small-size)",
        ResizeHandleWidth = "4px",
        RecordDividerWidth = "2px",
        AggregateDividerWidth = "2px",
        FilterGroupRail = "3px",
        ActiveCellOutline = "2px solid var(--flare-color-primary)",
        ColumnPickerMinWidth = "160px",
        RangeLayer = "color-mix(in srgb, var(--flare-color-primary) 14%, transparent)",
        RowEditingPct = "6%",
        LoadingVeilPct = "55%",
        LoadingDim = "0.6",
    };

    // Overlay: MD3 is the airier of the two, so a dialog keeps a full 3rem off the edge and drops to
    // 2rem on a phone, where the screen cannot spare it. A floating panel takes at most 70% of it.
    internal static readonly OverlayTokens Overlay = new()
    {
        ViewportInset = "3rem",
        ViewportInsetCompact = "2rem",
        PanelMaxBlockSize = "70dvh",
    };

    // Drag: MD3 gives a dragged object elevation 4 and leaves a dimmed silhouette behind it. The drop
    // target reads the primary state layer at the DRAGGED opacity - the one MD3 already defines for
    // exactly this state - so the target's tint and a card's own dragged state stay in step.
    internal static readonly DragTokens Drag = new()
    {
        SourceOpacity = "0.4",
        PreviewElevation = "var(--flare-elevation-4)",
        PreviewOpacity = "1",
        ZoneActiveBackground =
            "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
        ZoneActiveOutline = "var(--flare-color-primary)",
        IndicatorColor = "var(--flare-color-primary)",
    };

    // Rating: Size = initial defers to the component size classes; empty star = outline-variant,
    // filled default = primary (the Color parameter overrides via --fc-main), hover scale 1.15.
    internal static readonly RatingTokens Rating = new()
    {
        // Star ramp: 1 / 1.25 / 1.5 / 2 / 2.5rem.
        SizeXs = "1rem",
        SizeSm = "1.25rem",
        SizeMd = "1.5rem",
        SizeLg = "2rem",
        SizeXl = "2.5rem",
        EmptyColor = "var(--flare-color-outline-variant)",
        FilledColor = "var(--flare-color-primary)",
        HoverScale = "1.15",
    };

    // Calendar: 400px single-month cap, 16rem month min, 32dp nav buttons, 48dp cells, 24dp day
    // circles, primary "today" marker, primary-tinted selection, 30% dimmed adjacent-month days.
    internal static readonly CalendarTokens Calendar = new()
    {
        EventPadY = "0.0625rem",
        MaxWidth = "400px",
        MonthMinWidth = "16rem",
        NavBtnSize = "2rem",
        CellMinHeight = "3rem",
        DayNumSize = "1.5rem",
        TodayBg = "var(--flare-color-primary)",
        TodayColor = "var(--flare-color-on-primary)",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, var(--flare-color-surface))",
        OtherMonthOpacity = "0.38", // as the date picker's days outside the month
    };

    // Tree: 24dp indent per level, 24dp expander/handle, 20dp icons, primary-tinted selection.
    internal static readonly TreeTokens Tree = new()
    {
        ToggleHoverBg = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        Indent = "var(--flare-spacing-12)",
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        SelectedColor = "var(--flare-color-primary)",
    };

    // Stepper: 32dp indicator circle (2px border, 18dp icon), 2px connector (outline-variant, primary
    // when complete) at least 24dp long, 80dp per-step min width.
    internal static readonly StepperTokens Stepper = new()
    {
        FocusRingThickness = "2px",
        FocusRingColor = "var(--flare-color-primary)",
        CircleSize = "2rem",
        CircleBorderWidth = "2px",
        CircleIconSize = "1.125rem",
        ConnectorThickness = "2px",
        ConnectorMinLength = "1.5rem",
        ConnectorColor = "var(--flare-color-outline-variant)",
        ConnectorActiveColor = "var(--flare-color-primary)",
        StepMinWidth = "5rem",
    };

    // Timeline: 20dp surface dot with a 2px accent ring, 2px outline-variant connector line in a 2rem column.
    internal static readonly TimelineTokens Timeline = new()
    {
        DotSize = "1.25rem",
        DotBg = "var(--flare-color-surface)",
        DotBorderWidth = "2px",
        DotIconSize = "0.75rem",
        LineWidth = "2px",
        LineColor = "var(--flare-color-outline-variant)",
        ConnectorWidth = "2rem",
    };

    // Pagination: Size = initial defers to the size classes; extra-small radius, outline-variant
    // border, primary active fill (the Color parameter overrides via --fc-main).
    internal static readonly PaginationTokens Pagination = new()
    {
        // Button square ramp: 1.5 / 1.75 / 2.25 / 2.75 / 3.25rem.
        SizeXs = "1.5rem",
        SizeSm = "1.75rem",
        SizeMd = "2.25rem",
        SizeLg = "2.75rem",
        SizeXl = "3.25rem",
        Radius = "var(--flare-shape-extra-small)",
        BorderColor = "var(--flare-color-outline-variant)",
        ActiveColor = "var(--flare-color-primary)",
        BtnDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // Switch (MD3 baseline geometry: 52x32 track, 24 thumb, elevation-1 lift). Carried explicitly
    // now that the core record ships no defaults.
    internal static readonly SwitchTokens Switch = new()
    {
        // Per-size ramp; the xs/sm/lg/xl steps were literals in switch.css, md is this theme's value.
        TrackWidthXs = "2.125rem", TrackWidthSm = "2.5rem", TrackWidthMd = "52px",  TrackWidthLg = "4rem",   TrackWidthXl = "4.75rem",
        TrackHeightXs = "1.25rem", TrackHeightSm = "1.5rem", TrackHeightMd = "32px", TrackHeightLg = "2.5rem", TrackHeightXl = "3rem",
        TrackOffBg = "var(--flare-color-surface-container-highest)",
        TrackOnBg = "var(--fc-main, var(--flare-color-primary))",
        TrackBorder = "2px solid var(--flare-color-outline)",
        TrackHoverBorderColor = "var(--flare-color-outline)",
        ThumbOffSizeXs = "0.5rem",   ThumbOffSizeSm = "0.625rem", ThumbOffSizeMd = "1rem",   ThumbOffSizeLg = "1.25rem",  ThumbOffSizeXl = "1.5rem",
        ThumbOnSizeXs = "0.9375rem", ThumbOnSizeSm = "1.125rem",  ThumbOnSizeMd = "1.5rem",  ThumbOnSizeLg = "1.875rem",  ThumbOnSizeXl = "2.25rem",
        ThumbPressedOffSizeXs = "1.0625rem", ThumbPressedOffSizeSm = "1.25rem", ThumbPressedOffSizeMd = "1.75rem", ThumbPressedOffSizeLg = "2.1875rem", ThumbPressedOffSizeXl = "2.625rem",
        ThumbPressedOnSizeXs = "1.0625rem",  ThumbPressedOnSizeSm = "1.25rem",  ThumbPressedOnSizeMd = "1.75rem",  ThumbPressedOnSizeLg = "2.1875rem",  ThumbPressedOnSizeXl = "2.625rem",
        // The handle is centred in the track height at both ends - md.comp.switch puts the 16dp handle 8dp
        // from the edge and the 24dp one 4dp - and `left` is measured inside the 2dp track outline, so
        // each offset is (track height - handle) / 2 minus that outline.
        ThumbOffLeftXs = "0.25rem", ThumbOffLeftSm = "0.3125rem", ThumbOffLeftMd = "0.375rem", ThumbOffLeftLg = "0.5rem", ThumbOffLeftXl = "0.625rem",
        ThumbOnLeftXs = "calc(100% - 0.96875rem)", ThumbOnLeftSm = "calc(100% - 1.1875rem)", ThumbOnLeftMd = "calc(100% - 1.625rem)", ThumbOnLeftLg = "calc(100% - 2.0625rem)", ThumbOnLeftXl = "calc(100% - 2.5rem)",
        ThumbOffColor = "var(--flare-color-outline)",
        ThumbOnColor = "var(--flare-color-on-primary)",
        ThumbStateOffColor = "var(--flare-color-on-surface-variant)",
        ThumbStateOnColor = "var(--flare-color-primary-container)",
        IconSize = "1rem",
        IconOffColor = "var(--flare-color-surface-container-highest)",
        IconOnColor = "var(--flare-color-primary)",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FocusShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        FocusShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        TrackHoverOffBg = "var(--flare-switch-track-off-bg)",
        TrackHoverOnBg = "var(--flare-switch-track-on-bg)",
        HoverShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        HoverShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        DisabledTrackBg = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DisabledTrackBorder = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DisabledHandleBg = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
    };

    // Spacing scale: the 2px-base ramp (0/2/4/6/8/10/12/16/20/24/32/48/64px). Superset of the MD3 4dp
    // grid; carried explicitly now that the core record ships no defaults.
    // M3 draws separators as 1dp outline-variant rules and reserves the heavier weight for selection and
    // drop targets.
    internal static readonly BorderTokens Border = new()
    {
        Width = "1px",
        WidthEmphasis = "2px",
        Style = "solid",
        Divider = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline-variant)",
        Outline = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline)",
    };

    // M3 has no scrollbar component, so this is read off what the language does elsewhere: an
    // interactive element at rest carries the on-surface tone at a low opacity and firms up on hover,
    // and the track stays out of the way so the surface behind it keeps its own tone.
    internal static readonly ScrollbarTokens Scrollbar = new()
    {
        Width = "thin",
        Size = "8px",
        Thumb = "color-mix(in srgb, var(--flare-color-on-surface) 28%, transparent)",
        ThumbHover = "color-mix(in srgb, var(--flare-color-on-surface) 45%, transparent)",
        Track = "transparent",
        Radius = "var(--flare-shape-full)",
    };

    internal static readonly SpacingTokens Spacing = new()
    {
        S0 = "0",
        S1 = "0.125rem",
        S2 = "0.25rem",
        S3 = "0.375rem",
        S4 = "0.5rem",
        S5 = "0.625rem",
        S6 = "0.75rem",
        S8 = "1rem",
        S10 = "1.25rem",
        S12 = "1.5rem",
        S16 = "2rem",
        S24 = "3rem",
        S32 = "4rem",
    };

    internal static readonly AppBarTokens AppBar = new()
    {
        Gap = "0.25rem",
        Height = "4rem",
        HeightDense = "3rem",
        PaddingX = "0.25rem",
        TitlePaddingX = "0.75rem",
        // M3 separates the top app bar from the content by raising its container tone on scroll, and the
        // spec draws no rule under it. Elevation carries the separation here too.
        Border = "none",
    };

    internal static readonly BreadcrumbTokens Breadcrumb = new()
    {
        LinkHoverOpacity = "0.8",
        SeparatorOpacity = "0.5",
    };

    internal static readonly DateTimePickerTokens DateTimePicker = new()
    {
        PanelGap = "1rem",
    };

    internal static readonly FileUploadTokens FileUpload = new()
    {
        BorderWidth = "2px",
        HoverBg = "color-mix(in srgb, var(--flare-color-primary) 5%, var(--flare-color-surface))",
        DraggingBg = "color-mix(in srgb, var(--flare-color-primary) 10%, var(--flare-color-surface))",
        DraggingRingWidth = "2px",
        IconSize = "2.5rem",
        ZoneMinHeight = "10rem",
        ZoneRadius = "var(--flare-card-radius)",
        FileIconSize = "1.125rem",
        RowGap = "var(--flare-spacing-2)",
        RowRadius = "var(--flare-shape-extra-small)",
        RowActiveBg = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        RowSuccessBg = "color-mix(in srgb, var(--flare-color-tertiary) 8%, transparent)",
        RowErrorBg = "color-mix(in srgb, var(--flare-color-error) 8%, transparent)",
        RowErrorColor = "var(--flare-color-error)",
    };

    internal static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    internal static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        AppBarHeightDense = "3rem",              // MD3 dense top app bar = 48dp
        AppBarBg = "var(--flare-color-surface)",
        ContentPadding = "1.5rem 2rem",
        ContentPaddingMobile = "1rem",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "260px",
        AppBarBorder = "none",
        DrawerBorder = "none",
        AppBarShadow = "none",
        // The shell's three planes, named by the theme rather than picked in core CSS. These values
        // reproduce what core chose before, so nothing moves until a theme says otherwise.
        ShellBg = "var(--flare-color-background)",
        DrawerBg = "var(--flare-color-surface-container-low)",
        RailBg = "var(--flare-layout-drawer-bg)",
        ContentBg = "transparent",
        // No in-box language separates the drawer from the canvas by depth; they use DrawerBorder.
        DrawerShadowOffset = "0px",
        DrawerShadowBlur = "0px",
        DrawerShadowColor = "transparent",
    };

    internal static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    internal static readonly OtpTokens Otp = new()
    {
        BorderWidth = "2px",
        CellHeight = "3rem",
        CellWidth = "2.75rem",
        FocusRingWidth = "2px",
        FontSize = "1.25rem",
        FontWeight = "var(--flare-typescale-title-medium-weight)",
    };

    internal static readonly PickerTokens Picker = new()
    {
        OutsideOpacity = "0.38",                           // date-unselected-outside-month-label-text-opacity
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        WeekNumberOpacity = "0.7",
        PanelMinWidth = "22.5rem",
        PanelRadius = "var(--flare-shape-large)",
        HeaderHeight = "4rem",
        NavIconSize = "1.125rem",
        WeekdayHeight = "2.75rem",
        WeekdayFontSize = "var(--flare-typescale-body-large-size)",
        DaySize = "3rem",
        DayLayerSize = "2.5rem",
        DayFontSize = "var(--flare-typescale-body-large-size)",
    };

    internal static readonly ScrimTokens Scrim = new()
    {
        Opacity = "0.32",
    };

    // Material asks for a 48dp minimum target; the control keeps its drawn size and the core widens
    // the hit area to this under a coarse pointer.
    internal static readonly TouchTokens Touch = new()
    {
        TargetMin = "48px",
    };

    internal static readonly ScrollTopTokens ScrollTop = new()
    {
        TopInset = "1.5rem",
        TopSize = "2.75rem",
    };

    internal static readonly SkeletonTokens Skeleton = new()
    {
        PulseMinOpacity = "0.4",
        WaveOpacity = "12%",
    };

    // A glyph swap is Material's "icon transition": the outgoing symbol leaves while the incoming one
    // arrives, rather than one repainting over the other. It rides the fast spring because the travel is
    // spatial and the element is small - the same pairing the switch thumb and the checkbox tick use.
    internal static readonly IconTokens Icon = new()
    {
        MorphDuration = "var(--flare-motion-duration-spring-fast)",
        MorphEasing = "var(--flare-motion-easing-spring-fast)",
        MorphScale = "0.6",
        MorphRotate = "90deg",
    };

    internal static readonly StripeTokens Stripe = new()
    {
        // A wash of the content colour, which is what a striped table has always been here. The data
        // grid used to step to surface-container-low instead; one language, one answer.
        Background = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
    };

    internal static readonly TableTokens Table = new()
    {
        CellPaddingH = "1rem",
        CellPaddingV = "0.75rem",
    };

    internal static readonly TimePickerTokens TimePicker = new()
    {
        ColumnsSepSize = "1.5rem",
        DialCenterSize = "0.5rem",
        DialHandleSize = "3rem",
        DialSize = "16rem",
        DialTrackWidth = "0.125rem",
        PeriodHeight = "5rem",
        PeriodWidth = "3.25rem",
        TimeFieldHeight = "5rem",
        TimeFieldWidth = "6rem",
        DisplaySize = "3.5625rem",
        HeadlineTracking = "var(--flare-typescale-label-medium-spacing)", // headline is label-medium
        PanelRadius = "var(--flare-shape-extra-large)",
        TimeSepSize = "3.5625rem",
    };

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete MD3 Expressive design tokens. Use this as the base for custom themes.</summary>
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
        Skeleton = Skeleton,
        Icon = Icon,
        Stripe = Stripe,
        Splitter = Splitter,
        Table = Table,
        TimePicker = TimePicker,
        Spacing = Spacing,
        Border = Border,
        Scrollbar = Scrollbar,
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
        Slider = Slider,
        Dialog = Dialog,
        Drawer = Drawer,
        Snackbar = Snackbar,
        Nav = Nav,
        BottomNav = BottomNav,
        Layer = Layer,
        TableOfContents = TableOfContents,
        ColorPicker = ColorPicker,
        FocusRing = "3px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
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
        // MD3 cards: elevated hover = level 3 (spec), outlined border = full outline role (spec),
        // 16px inner padding on raw-content cards. Variant bg roles come from the base CardTokens.
        Card = new()
        {
            ElevationHover = "var(--flare-elevation-2)", // md.comp.elevated-card.hover.container.elevation
            OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
            PaddingTop = "16px",
            PaddingRight = "16px",
            PaddingBottom = "16px",
            PaddingLeft = "16px",
            // Neutral baseline members (formerly the record defaults), carried explicitly.
            ElevatedBg = "var(--flare-color-surface-container-low)",
            FilledBg = "var(--flare-color-surface-container-highest)",
            FilledBorder = "none",
            OutlinedBg = "var(--flare-color-surface)",
            TonalBg = "var(--flare-color-secondary-container)",
            TonalColor = "var(--flare-color-on-secondary-container)",
            TextColor = "var(--flare-color-on-surface)",
            Radius = "var(--flare-shape-medium)",
            Elevation = "var(--flare-elevation-1)",
            // Only the elevated card rests on a shadow; filled and outlined sit at level 0 (md.comp.*-card.container.elevation).
            FilledElevation = "none",
            OutlinedElevation = "none",
            TonalElevation = "none",
            TextElevation = "none",
            // Filled and outlined cards rise to level 1 on hover (md.comp.*-card.hover.container.elevation);
            // tonal is not an M3 variant and stays flat.
            FilledElevationHover = "var(--flare-elevation-1)",
            OutlinedElevationHover = "var(--flare-elevation-1)",
            TonalElevationHover = "none",
            // md.sys.state.focus-indicator: 3dp, secondary.
            FocusRing = "3px solid var(--flare-color-secondary)",
            SelectedBorder = "2px solid var(--flare-color-primary)",
            SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
            StateLayer = "var(--flare-state-hover-layer)",
            ContentPadding = "16px",
            HeaderPadding = "16px 16px 0 16px",
            FooterPadding = "8px 16px 16px 16px",
            ActionsPadding = "8px 16px 16px 16px",
            ActionsGap = "8px",
            MediaRadius = "0",
            TitleColor = "var(--flare-color-on-surface)",
            TitleFontFamily = "var(--flare-typescale-title-medium-font)",
            TitleFontSize = "var(--flare-typescale-title-medium-size)",
            SubtitleColor = "var(--flare-color-on-surface-variant)",
            SubtitleFontFamily = "var(--flare-typescale-body-medium-font)",
            SubtitleFontSize = "var(--flare-typescale-body-medium-size)",
            TransitionDuration = "var(--flare-motion-duration-state-change)",
            TransitionEasing = "var(--flare-motion-easing-standard)",
        },
        Input = Input,
        Chart = Chart,
        Gauge = Gauge,
        Progress = Progress,
        // Popover / dropdown panels use the medium shape (Snackbar/Dialog/Nav already match defaults).
        Popover = Popover,
        Tooltip = Tooltip,
        Avatar = Avatar,
        Extended = Extended,
    };

    public static readonly ColorScheme LightColors = new()
    {
        Primary = "#6750A4",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#EADDFF",
        // Updated MD3 (Expressive) color spec: light on-*-container roles use tone 30 (was tone 10).
        OnPrimaryContainer = "#4F378B",
        Secondary = "#625B71",
        OnSecondary = "#FFFFFF",
        SecondaryContainer = "#E8DEF8",
        OnSecondaryContainer = "#4A4458",
        Tertiary = "#7D5260",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#FFD8E4",
        OnTertiaryContainer = "#633B48",
        Error = "#B3261E",
        OnError = "#FFFFFF",
        ErrorContainer = "#F9DEDC",
        OnErrorContainer = "#8C1D18",
        Success = "#2E6C47",
        OnSuccess = "#FFFFFF",
        SuccessContainer = "#B2F1C5",
        OnSuccessContainer = "#002111",
        Warning = "#7D5700",
        OnWarning = "#FFFFFF",
        WarningContainer = "#FFDEA6",
        OnWarningContainer = "#271900",
        Info = "#00639B",
        OnInfo = "#FFFFFF",
        InfoContainer = "#CDE5FF",
        OnInfoContainer = "#001D33",
        Surface = "#FEF7FF",
        OnSurface = "#1D1B20",
        SurfaceVariant = "#E7E0EC",
        OnSurfaceVariant = "#49454F",
        OnSurfaceVariant2 = "#615D67",
        SurfaceContainer = "#F3EDF7",
        SurfaceContainerLowest = "#FFFFFF",
        SurfaceContainerLow = "#F7F2FA",
        SurfaceContainerHigh = "#ECE6F0",
        SurfaceContainerHighest = "#E6E0E9",
        // md.sys.color.surface-bright / -dim: neutral98 / neutral87.
        SurfaceBright = "#FEF7FF",
        SurfaceDim = "#DED8E1",
        // Fixed roles are the same in both modes: tones 90 / 80 / 10 / 30 of each accent palette.
        PrimaryFixed = "#EADDFF",
        PrimaryFixedDim = "#D0BCFF",
        OnPrimaryFixed = "#21005D",
        OnPrimaryFixedVariant = "#4F378B",
        SecondaryFixed = "#E8DEF8",
        SecondaryFixedDim = "#CCC2DC",
        OnSecondaryFixed = "#1D192B",
        OnSecondaryFixedVariant = "#4A4458",
        TertiaryFixed = "#FFD8E4",
        TertiaryFixedDim = "#EFB8C8",
        OnTertiaryFixed = "#31111D",
        OnTertiaryFixedVariant = "#633B48",
        Background = "#FEF7FF",
        OnBackground = "#1D1B20",
        Outline = "#79747E",
        OutlineVariant = "#CAC4D0",
        InverseSurface = "#322F35",
        InverseOnSurface = "#F5EFF7",
        InversePrimary = "#D0BCFF",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.3)",
        ShadowPenumbra = "rgba(0,0,0,0.15)",
    };

    public static readonly ColorScheme DarkColors = new()
    {
        Primary = "#D0BCFF",
        OnPrimary = "#381E72",
        PrimaryContainer = "#4F378B",
        OnPrimaryContainer = "#EADDFF",
        Secondary = "#CCC2DC",
        OnSecondary = "#332D41",
        SecondaryContainer = "#4A4458",
        OnSecondaryContainer = "#E8DEF8",
        Tertiary = "#EFB8C8",
        OnTertiary = "#492532",
        TertiaryContainer = "#633B48",
        OnTertiaryContainer = "#FFD8E4",
        Error = "#F2B8B5",
        OnError = "#601410",
        ErrorContainer = "#8C1D18",
        OnErrorContainer = "#F9DEDC",
        Success = "#97D5A9",
        OnSuccess = "#00391E",
        SuccessContainer = "#0F5130",
        OnSuccessContainer = "#B2F1C5",
        Warning = "#F5BC49",
        OnWarning = "#412D00",
        WarningContainer = "#5E4200",
        OnWarningContainer = "#FFDEA6",
        Info = "#94CCFF",
        OnInfo = "#003353",
        InfoContainer = "#004A77",
        OnInfoContainer = "#CDE5FF",
        Surface = "#141218",
        OnSurface = "#E6E0E9",
        SurfaceVariant = "#49454F",
        OnSurfaceVariant = "#CAC4D0",
        OnSurfaceVariant2 = "#AFAAB6",
        SurfaceContainer = "#211F26",
        SurfaceContainerLowest = "#0F0D13",
        SurfaceContainerLow = "#1D1B20",
        SurfaceContainerHigh = "#2B2930",
        SurfaceContainerHighest = "#36343B",
        // md.sys.color.surface-bright / -dim: neutral24 / neutral6.
        SurfaceBright = "#3B383E",
        SurfaceDim = "#141218",
        // Fixed roles are the same in both modes: tones 90 / 80 / 10 / 30 of each accent palette.
        PrimaryFixed = "#EADDFF",
        PrimaryFixedDim = "#D0BCFF",
        OnPrimaryFixed = "#21005D",
        OnPrimaryFixedVariant = "#4F378B",
        SecondaryFixed = "#E8DEF8",
        SecondaryFixedDim = "#CCC2DC",
        OnSecondaryFixed = "#1D192B",
        OnSecondaryFixedVariant = "#4A4458",
        TertiaryFixed = "#FFD8E4",
        TertiaryFixedDim = "#EFB8C8",
        OnTertiaryFixed = "#31111D",
        OnTertiaryFixedVariant = "#633B48",
        Background = "#141218",
        OnBackground = "#E6E0E9",
        Outline = "#938F99",
        OutlineVariant = "#49454F",
        InverseSurface = "#E6E0E9",
        InverseOnSurface = "#322F35",
        InversePrimary = "#6750A4",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.3)",
        ShadowPenumbra = "rgba(0,0,0,0.15)",
    };

    private static TypeStyle T(string font, string weight, string size, string height, string spacing) =>
        new() { FontFamily = font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = spacing };
}
