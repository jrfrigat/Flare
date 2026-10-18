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
/// inherit a language it does not speak.
/// </para>
/// </summary>
public class MaterialDesign3Tokens
{
    internal static readonly TypographyTokens Typography = new()
    {
        // No design language names a code face, so this is the generic - twice, because a generic
        // family standing alone makes several engines use their own "monospace default size".
        MonoFont = "monospace, monospace",
        WeightBold = "700",
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
        PaddingXXs = "0.1875rem",
        PaddingXSm = "0.1875rem",
        LabelSizeXs = "0.5625rem",
        LabelSizeSm = "0.625rem",
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
        BorderWidth = "0",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        // Baseline M3 does not reshape a selected button - swapping round and square on selection is an
        // Expressive behaviour, stated in that theme - so a selected button keeps the corners it stands in.
        SelectedRadiusXs = "calc(var(--flare-btn-height-xs) / 2)",
        SelectedRadiusSm = "calc(var(--flare-btn-height-sm) / 2)",
        SelectedRadiusMd = "calc(var(--flare-btn-height-md) / 2)",
        SelectedRadiusLg = "calc(var(--flare-btn-height-lg) / 2)",
        SelectedRadiusXl = "calc(var(--flare-btn-height-xl) / 2)",
        LoadingOpacity = "0.8",
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
        // 56dp.
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
        // md.comp.*-button.disabled: the container on-surface at 12%, the label and icon on-surface at 38%,
        // the outline on-surface at 12% - a repaint, not a fade of the whole control.
        DisabledOpacity = "1",
        DisabledRepaint = "100%",

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

        // The icon side of a contained button is 16dp against 24dp on the label side (with-leading-icon
        // leading-space). A text button keeps its 12dp on the icon side, so it tucks nothing.
        IconInset = "0.5rem",
        TextIconInset = "0",

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
        ConnectedGap = "0",
        ConnectedOverlap = "-1px",
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
    };

    // FAB: padding-based sizing, large/medium/extra-large rounding.
    internal static readonly FabTokens Fab = new()
    {
        PaddingLg = "1.875rem",
        // A FAB sizes itself as glyph plus padding on both sides, so the icon and the padding together
        // make the container. Lg is the large FAB - its 28dp corner says so - which is
        // md.comp.fab.large.*: a 36dp glyph in a 96dp container, hence 30dp of padding. Md is the
        // baseline FAB (24dp glyph, 56dp container) and Sm the small one (24dp, 40dp).
        IconSizeSm = "1.5rem",    // 24dp -> 40dp container
        IconSizeMd = "1.5rem",    // 24dp -> 56dp container
        IconSizeLg = "2.25rem",   // 36dp -> 96dp container
    };

    internal static readonly CheckboxTokens Checkbox = new()
    {
        // The size ramp, one token per size so a theme can move any of the five steps.
        SizeXs = "0.875rem",
        SizeSm = "1rem",
        SizeMd = "1.125rem",
        SizeLg = "1.375rem",
        SizeXl = "1.625rem",
        StateLayerSize = "2.5rem",
        BorderWidth = "2px",
        Radius = "2px",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
    };
    internal static readonly RadioTokens Radio = new()
    {
        // The size ramp, one token per size so a theme can move any of the five steps.
        SizeXs = "1rem",
        SizeSm = "1.125rem",
        SizeMd = "1.25rem",
        SizeLg = "1.5rem",
        SizeXl = "1.75rem",
        StateLayerSize = "2.5rem",
        // The focus this theme already states for the checkbox and the switch - the selection
        // controls answer as one family rather than each drawing its own ring.
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
    };
    internal static readonly ChipTokens Chip = new()
    {
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
        Height = "2rem",                       // MD3 = 32dp
    };
    internal static readonly TabsTokens Tabs = new()
    {
        // md.comp.primary-navigation-tab.with-icon.icon.size: 24dp.
        IconSize = "1.5rem",
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "3px",
        // Secondary navigation tab: 2dp indicator, active label on-surface (spec md.comp
        // .secondary-navigation-tab.active-indicator.height / .label-text.color).
        SecondaryIndicatorThickness = "2px",
        TrackBg = "transparent",
        // The label and its padding decide the height here; no minimum width.
        TabHeight = "3rem",                          // md.comp.primary-navigation-tab.container.height 48dp
        TabMinWidth = "0",
    };

    // Menu (MD3 Expressive "Menus"): container 16dp (shape-large), elevation 3,
    // item 4dp (end items 12dp), label-large text, vertical 8dp, gap 2dp, focus-ring secondary.
    /// <summary>List container and row tokens.</summary>
    internal static readonly ListTokens List = new()
    {
        ItemHeight = "3.5rem",
        ItemHeightTwoLine = "4.5rem",
        ItemHeightDense = "3rem",
        ItemHeightTwoLineDense = "3.5rem",
        ItemRadius = "0",
        ItemIconSize = "1.5rem", // md.comp.list.list-item.leading-icon.size / trailing-icon.size 24dp
    };

    /// <summary>Accordion container, header and body tokens.</summary>
    internal static readonly AccordionTokens Accordion = new()
    {
        IconSize = "0.875rem",
    };

    /// <summary>Standalone collapse header tokens.</summary>
    internal static readonly CollapseTokens Collapse = new()
    {
        HeaderBg = "transparent",
    };

    internal static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        // Flush against the anchor, as material-web md-menu places it (x/y offset 0).
        PanelOffset = "0px",
        // Baseline M3 is one classic surface: a 4dp panel with square items and no group islands.
        // The 16dp panel, the rounded items and the floating group sections are Expressive, and the
        // Expressive theme states them.
        PanelMinWidth = "7rem",                       // 112dp
        PanelPaddingInline = "0",                     // md-menu: items run edge to edge
        PanelPaddingBlock = "0.5rem",                 // md-menu top/bottom space 8dp
        ItemHeight = "3rem",                          // item height 48dp (MD3 list-item)
        ItemPaddingBlock = "0.5rem",                  // top/bottom 8dp
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
        // Neutral baseline members, carried explicitly because the core record ships no defaults.
        EnterAnimation = "flare-menu-in",
        ItemIconSize = "1.5rem", // md.comp.menu.list-item.*-icon.size 24dp
        ItemFocusRingThickness = "3px",
        ItemFocusRingOffset = "-3px",
    };

    // Input - filled style (md.comp.filled-text-field): surface-container-highest container, 56dp
    // height (1rem block padding + 24px line), 1dp on-surface-variant active indicator, hover ->
    // on-surface indicator + 8% state layer, focus -> 3dp primary (Expressive).
    internal static readonly InputTokens Input = new()
    {
        BorderColor = "transparent",                  // filled default: no side border, colour only
        // Focus = the 2dp primary active indicator (md.comp.filled-text-field.focus.active-indicator.height):
        // the 1dp bottom border turns primary and a 1dp inset shadow adds the second pixel without a jump.
        FocusRing = "inset 0 -1px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusBorderBottomColor = "var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        PaddingXs = "0.1875rem 0.5rem",
        PaddingSm = "0.375rem 0.625rem",
        PaddingMd = "1rem 1rem",                      // the M3 56dp field
        // Large and extra-large are measured up from the 56dp medium, so neither is shorter than medium
        // under this theme.
        PaddingLg = "1.25rem 1.125rem",
        PaddingXl = "1.5rem 1.25rem",
        // Height ramp. M3 puts the text field AND the select at 56dp, so Md is the spec number and the
        // rest are measured off the padding ramp above - each step is the tallest control of that size, so
        // the family converges upward and no control shrinks.
        HeightXs = "1.875rem", // 30px
        HeightSm = "2.25rem",  // 36px
        HeightMd = "3.5rem",   // 56px - the M3 field height
        HeightLg = "4rem",     // 64px
        HeightXl = "4.75rem",  // 76px
        IconSize = "1.5rem",                          // leading/trailing icon 24dp
        FocusBorderColor = "transparent",
        // The default field is filled, so an error is the active indicator alone
        // (md.comp.filled-text-field.error-active-indicator.color), 2dp while focused.
        ErrorBorderColor = "transparent",
        ErrorFocusRing = "inset 0 -1px 0 0 var(--flare-color-error)",
        // md.comp.filled-text-field: container surface-container-highest, corner-extra-small-top, a 1dp
        // on-surface-variant indicator that turns primary and 2dp on focus, error-coloured on error.
        FilledVariantBorderColor = "transparent",
        FilledVariantFocusRing = "inset 0 -1px 0 0 var(--fc-main, var(--flare-color-primary))",
        FilledVariantFocusBorderColor = "transparent",
        FilledVariantFocusBorderBottomColor = "var(--fc-main, var(--flare-color-primary))",
        FilledVariantErrorBorderColor = "transparent",
        FilledVariantErrorFocusRing = "inset 0 -1px 0 0 var(--flare-color-error)",
        // md.comp.outlined-text-field: a 1dp outline, corner-extra-small, and a 3dp primary outline on focus -
        // the border turns primary and a 2dp inset ring adds the rest without a layout shift.
        OutlinedVariantBg = "transparent",
        OutlinedVariantFocusRing = "inset 0 0 0 2px var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantFocusBorderColor = "var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantFocusBorderBottomColor = "var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantErrorFocusRing = "inset 0 0 0 2px var(--flare-color-error)",
        DropdownMaxHeight = "16rem",
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
        ValueWeight = "500",
        TrackWidth = "14",
        TrackCap = "round",

        FillWidth = "14",

        NeedleWidth = "3",
        NeedleLength = "0.78",
        PivotRadius = "5",

        TickWidth = "1.5",
        TickLength = "7",
        TickMinorWidth = "1",
        TickMinorLength = "4",
        TickGap = "5",

        LabelSize = "9",

        BandOpacity = "0.55",
        TargetWidth = "2.5",
    };

    internal static readonly ChartTokens Chart = new()
    {

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
        SliceStrokeWidth = "1.5",

        // Sequential ramp: one hue, intensity carries the value. The floor keeps a zero cell visible as a
        // cell rather than as a hole in the grid.
        RampMinOpacity = "0.12",
        RampMaxOpacity = "1",
        CellRadius = "2",
        CellGap = "2",
        CellHoverOpacity = "0.85",

        // Axes. A grid line is thinner than an outline: it sits behind the data and must not compete.
        GridWidth = "0.5",
        GridDash = "none",
        GridMinorWidth = "0.5",
        ValueSize = "8px",
        AxisTitleSize = "10px",

        BorderWidth = "1px",

        LegendDotSize = "0.625rem",
        LegendOffOpacity = "0.4",

        // Overlays. The annotation default IS the error role - a threshold line means what red means,
        // which is exactly why a data series must not borrow it.
        TrendWidth = "1.5",
        TrendDash = "5 4",
        TrendOpacity = "0.7",
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
        ContentSizeXs = "0.5rem",
        ContentSizeSm = "0.625rem",
        ContentSizeMd = "0.75rem",
        ContentSizeLg = "1rem",
        ContentSizeXl = "1.25rem",
        Gap = "4px",
        LinearIndeterminateDuration = "1500ms",
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
        RailLabelLineHeight = "1.15",
        ActiveLeftBar = "none",
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
        // The tonal container is the edge (md.comp.navigation-bar has no divider); the 1px stays reserved.
        BorderColor = "transparent",
        SafeAreaPadding = "env(safe-area-inset-bottom, 0px)",
        IconSize = "1.5rem",
        IndicatorSize = "2rem",
        IndicatorWidth = "4rem",           // md.comp.navigation-bar.active-indicator.width 64dp
    };

    internal static readonly TableOfContentsTokens TableOfContents = new()
    {
        TitleTracking = "0.05em",
        ActiveWeight = "700", // weight-prominent, as the active navigation item
        HoverBgOpacity = "40%",
        LineHeight = "1.4",
        RailColor = "transparent",
        RailWidth = "0",
        // "0px", not "0": the marker is also pulled over the rail with calc(-1 * var(--flare-toc-marker-width)),
        // and a unitless value there yields a <number>, which margin rejects. At zero the dropped declaration
        // happens to compute the same margin, so nothing shows - but the trap is real the moment it is read
        // as anything other than a length, and the sibling themes already write the unit.
        MarkerWidth = "0px",
        LinkPadX = "0.75rem",
    };

    internal static readonly ColorPickerTokens ColorPicker = new()
    {
    };

    internal static readonly TooltipTokens Tooltip = new()
    {
        MaxWidth = "18rem",
        // md.comp.plain-tooltip has no elevation: the inverse-surface chip stands out on its own.
        Shadow = "none",
    };

    internal static readonly PopoverTokens Popover = new()
    {
    };

    internal static readonly AvatarTokens Avatar = new()
    {
        GroupSpacing = "-0.75rem",
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
        MinHeight = "3rem",
        PaddingBlock = "0.875rem",
        CloseOpacity = "0.75",
        MinWidth = "20rem",
        MaxWidth = "36rem",
    };

    // Splitter: an 8dp gutter - comfortable to grab without reading as a divider - carrying a 2dp grip
    // mark, tinted with the primary wash on hover the way the rest of the theme signals an active target.
    // Geometry and colours of the pane drag handle (androidx DragHandleTokens): a 24dp container around a
    // 4 x 48dp grip that widens to 12 x 52dp with a medium corner while pressed or dragged.
    internal static readonly SplitterTokens Splitter = new()
    {
        GutterSize = "24px",
        GripThickness = "4px",
        GripLength = "48px",
        // The handle has no container colour; the gap shows the panes' shared background.
        Color = "transparent",
        IconSize = "1.125rem",
        GripPressedThickness = "12px",
        GripPressedLength = "52px",
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
        HandleClipPath = "none",
        HandleBorderWidth = "0px",
        // Follow the per-instance Color: --fc-main is the local accent, falling back to the role.
        HandleFill = "var(--fc-main, var(--flare-color-primary))",
        StateLayerSize = "40px",
        FocusOutline = "none",
        FocusOutlineOffset = "0px",
        StopSize = "4px",
    };

    internal static readonly DialogTokens Dialog = new()
    {
        IconSize = "1.5rem",
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
        ResizeHandleWidth = "4px",
        FilterGroupRail = "3px",
        ColumnPickerMinWidth = "160px",
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
        PreviewOpacity = "1",
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
        OtherMonthOpacity = "0.38", // as the date picker's days outside the month
    };

    // Tree: 24dp indent per level, 24dp expander/handle, 20dp icons, primary-tinted selection.
    internal static readonly TreeTokens Tree = new()
    {
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
    };

    // Stepper: 32dp indicator circle (2px border, 18dp icon), 2px connector (outline-variant, primary
    // when complete) at least 24dp long, 80dp per-step min width.
    internal static readonly StepperTokens Stepper = new()
    {
        FocusRingThickness = "2px",
        CircleSize = "2rem",
        CircleIconSize = "1.125rem",
        ConnectorThickness = "2px",
        ConnectorMinLength = "1.5rem",
        StepMinWidth = "5rem",
    };

    // Timeline: 20dp surface dot with a 2px accent ring, 2px outline-variant connector line in a 2rem column.
    internal static readonly TimelineTokens Timeline = new()
    {
        DotSize = "1.25rem",
        DotIconSize = "0.75rem",
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
    };

    // Switch (MD3 baseline geometry: 52x32 track, 24 thumb, elevation-1 lift). Carried explicitly
    // now that the core record ships no defaults.
    internal static readonly SwitchTokens Switch = new()
    {
        // Per-size ramp; the xs/sm/lg/xl steps were literals in switch.css, md is this theme's value.
        TrackWidthXs = "2.125rem", TrackWidthSm = "2.5rem", TrackWidthMd = "52px",  TrackWidthLg = "4rem",   TrackWidthXl = "4.75rem",
        TrackHeightXs = "1.25rem", TrackHeightSm = "1.5rem", TrackHeightMd = "32px", TrackHeightLg = "2.5rem", TrackHeightXl = "3rem",
        TrackOnBg = "var(--fc-main, var(--flare-color-primary))",
        TrackBorder = "2px solid var(--flare-color-outline)",
        ThumbOffSizeXs = "0.5rem",   ThumbOffSizeSm = "0.625rem", ThumbOffSizeMd = "1rem",   ThumbOffSizeLg = "1.25rem",  ThumbOffSizeXl = "1.5rem",
        ThumbOnSizeXs = "0.9375rem", ThumbOnSizeSm = "1.125rem",  ThumbOnSizeMd = "1.5rem",  ThumbOnSizeLg = "1.875rem",  ThumbOnSizeXl = "2.25rem",
        ThumbPressedOffSizeXs = "1.0625rem", ThumbPressedOffSizeSm = "1.25rem", ThumbPressedOffSizeMd = "1.75rem", ThumbPressedOffSizeLg = "2.1875rem", ThumbPressedOffSizeXl = "2.625rem",
        ThumbPressedOnSizeXs = "1.0625rem",  ThumbPressedOnSizeSm = "1.25rem",  ThumbPressedOnSizeMd = "1.75rem",  ThumbPressedOnSizeLg = "2.1875rem",  ThumbPressedOnSizeXl = "2.625rem",
        // The handle is centred in the track height at both ends - md.comp.switch puts the 16dp handle 8dp
        // from the edge and the 24dp one 4dp - and `left` is measured inside the 2dp track outline, so
        // each offset is (track height - handle) / 2 minus that outline.
        ThumbOffLeftXs = "0.25rem", ThumbOffLeftSm = "0.3125rem", ThumbOffLeftMd = "0.375rem", ThumbOffLeftLg = "0.5rem", ThumbOffLeftXl = "0.625rem",
        ThumbOnLeftXs = "calc(100% - 0.96875rem)", ThumbOnLeftSm = "calc(100% - 1.1875rem)", ThumbOnLeftMd = "calc(100% - 1.625rem)", ThumbOnLeftLg = "calc(100% - 2.0625rem)", ThumbOnLeftXl = "calc(100% - 2.5rem)",
        IconSize = "1rem",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FocusShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        FocusShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        TrackHoverOffBg = "var(--flare-switch-track-off-bg)",
        TrackHoverOnBg = "var(--flare-switch-track-on-bg)",
        HoverShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        HoverShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
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
        Height = "4rem",
        HeightDense = "3rem",
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
    };

    internal static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    internal static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        AppBarHeightDense = "3rem",              // MD3 dense top app bar = 48dp
        ContentPadding = "1.5rem 2rem",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "260px",
        AppBarBorder = "none",
        DrawerBorder = "none",
        AppBarShadow = "none",
        // The shell's three planes, named by the theme rather than picked in core CSS. These values
        // reproduce what core chose before, so nothing moves until a theme says otherwise.
        ContentBg = "transparent",
        // No in-box language separates the drawer from the canvas by depth; they use DrawerBorder.
        DrawerShadowOffset = "0px",
        DrawerShadowBlur = "0px",
        DrawerShadowColor = "transparent",
        // The navigation icon of a top app bar is a standard icon button: a 40dp container around the
        // 24dp `menu` symbol, whose bars are 18dp wide, 2dp thick and 5dp apart (y = 6, 11, 16).
        AppBarToggleSize = "40px",
        AppBarToggleGlyphWidth = "18px",
        AppBarToggleGlyphHeight = "12px",
        AppBarToggleBarThickness = "2px",
    };

    internal static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
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
        HeaderGap = "0",
        ActionHeight = "2.5rem",
        ActionPadding = "0.5rem 0",
        ActionMargin = "0",
        ActionBorder = "none",
        TodayBorder = "1px solid var(--flare-color-primary)",
        TodayWeight = "400",
        WeekNumberWidth = "1.5rem",
        RangeFieldBasis = "12rem",
        RangeCalendarMaxWidth = "20rem",
        RangeSeparatorSize = "1.25rem",
        OutsideOpacity = "0.8", // Available adjacent-month dates must remain readable.
        WeekNumberOpacity = "0.7",
        PanelMinWidth = "22.5rem",
        HeaderHeight = "4rem",
        NavIconSize = "1.125rem",
        WeekdayHeight = "2.75rem",
        DaySize = "3rem",
        DayLayerSize = "2.5rem",
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

    // A glyph swap is Material's "icon transition": the outgoing symbol leaves while the incoming one
    // arrives, rather than one repainting over the other. It rides the fast spring because the travel is
    // spatial and the element is small - the same pairing the switch thumb and the checkbox tick use.
    internal static readonly IconTokens Icon = new()
    {
        MorphScale = "0.6",
        MorphRotate = "90deg",
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
        DialHandleSize = "3rem",
        DialSize = "16rem",
        DialTrackWidth = "0.125rem",
        PeriodHeight = "5rem",
        PeriodWidth = "3.25rem",
        TimeFieldHeight = "5rem",
        TimeFieldWidth = "6rem",
        DisplaySize = "3.5625rem",
        TimeSepSize = "3.5625rem",
    };


    /// <summary>The complete baseline MD3 design tokens. Use this as the base for custom themes.</summary>
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
            OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
            PaddingTop = "16px",
            PaddingRight = "16px",
            PaddingBottom = "16px",
            PaddingLeft = "16px",
            // Neutral baseline members, carried explicitly.
            FilledBorder = "none",
            // Only the elevated card rests on a shadow; filled and outlined sit at level 0 (md.comp.*-card.container.elevation).
            FilledElevation = "none",
            TonalElevation = "none",
            // Tonal is not an M3 variant and stays flat on hover.
            TonalElevationHover = "none",
            // md.sys.state.focus-indicator: 3dp, secondary.
            FocusRing = "3px solid var(--flare-color-secondary)",
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
