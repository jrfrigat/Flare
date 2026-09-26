using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Theme.Aero;

/// <summary>
/// Design tokens for the "Aero" theme - the glossy gradient look of Windows 7 Aero, Office 2010 and
/// 1C: cool blue-gray surfaces, Segoe UI / Tahoma type, square-ish 3px geometry, an Aero blue accent
/// and soft drop shadows. The signature gradient/gloss/glow effects live in the scoped component CSS
/// (wwwroot/css), driven by these tokens.
/// </summary>
internal class AeroTokens
{
    private const string Font = "'Segoe UI', Tahoma, 'Helvetica Neue', Arial, sans-serif";

    internal static readonly TypographyTokens Typography = new()
    {
        // No design language names a code face, so this is the generic - twice, because a generic
        // family standing alone makes several engines use their own "monospace default size".
        MonoFont = "monospace, monospace",
        DisplayLarge = T("700", "2.25rem", "2.875rem"),
        DisplayMedium = T("700", "1.75rem", "2.25rem"),
        DisplaySmall = T("700", "1.375rem", "1.75rem"),
        HeadlineLarge = T("600", "1.1875rem", "1.625rem"),
        HeadlineMedium = T("600", "1.0625rem", "1.5rem"),
        HeadlineSmall = T("600", "0.9375rem", "1.3125rem"),
        TitleLarge = T("600", "0.875rem", "1.1875rem"),
        TitleMedium = T("600", "0.8125rem", "1.0625rem"),
        TitleSmall = T("600", "0.75rem", "1rem"),
        BodyLarge = T("400", "0.8125rem", "1.1875rem"),
        BodyMedium = T("400", "0.75rem", "1.0625rem"),
        BodySmall = T("400", "0.6875rem", "0.9375rem"),
        LabelLarge = T("600", "0.8125rem", "1.125rem"),
        LabelMedium = T("400", "0.75rem", "1rem"),
        LabelSmall = T("400", "0.6875rem", "0.9375rem"),
    };

    // Square-ish "chrome" geometry - the gentle 2-3px rounding of Aero/Office controls.
    internal static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "2px",
        Small = "3px",
        Medium = "3px",
        Large = "4px",
        ExtraLarge = "6px",
        Full = "9999px",

        // Aero reacts with glow and gloss; its containers keep one shape throughout.
        MorphDuration = "0s",
        MorphEasing = "cubic-bezier(0, 0, 0, 1)",
    };

    // Snappy, short transitions - the quick hover fades of the Aero era. The era has one decelerating and
    // one accelerating curve, so every level of expression shares them.
    private const string CurveStandard = "cubic-bezier(0.4, 0, 0.2, 1)";
    private const string CurveDecelerate = "cubic-bezier(0, 0, 0, 1)";
    private const string CurveAccelerate = "cubic-bezier(0.4, 0, 1, 1)";

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

    // Menus of the era fade in briefly and vanish at once.
    private static readonly SurfaceMotionTokens MenuMotion = new()
    {
        Enter = Still with { Opacity = "0", FadeDuration = "130ms", FadeEasing = CurveDecelerate },
        Exit = Still with { Opacity = "0" },
    };

    private static readonly SurfaceMotionTokens EdgeMotion = new()
    {
        Enter = Still with { Duration = "280ms", Easing = CurveDecelerate, Offset = "100%" },
        Exit = Still with { Duration = "200ms", Easing = CurveAccelerate, Offset = "100%" },
    };

    internal static readonly MotionTokens Motion = new()
    {
        EasingStandard = CurveStandard,
        EasingDecelerate = CurveDecelerate,
        EasingAccelerate = CurveAccelerate,
        EasingEmphasized = CurveStandard,
        EasingEmphasizedDecelerate = CurveDecelerate,
        EasingEmphasizedAccelerate = CurveAccelerate,
        EasingSubtle = CurveStandard,
        EasingSubtleDecelerate = CurveDecelerate,
        EasingSubtleAccelerate = CurveAccelerate,
        EasingLinear = "linear",

        DurationStateChange = "90ms",
        DurationSmallMove = "150ms",
        DurationEnterSmall = "130ms",
        DurationEnterMedium = "200ms",
        DurationEnterLarge = "280ms",
        DurationExitSmall = "90ms",
        DurationExitMedium = "130ms",
        DurationExitLarge = "200ms",
        DurationCycle = "1000ms",
        DurationCycleLong = "2000ms",

        // Aero's motion is glide and glow, not bounce, so the springs resolve to the same
        // decelerating curve as everything else and only the settling times differ.
        EasingSpringFast = CurveDecelerate,
        EasingSpring = CurveDecelerate,
        EasingSpringSlow = CurveDecelerate,
        DurationSpringFast = "150ms",
        DurationSpring = "200ms",
        DurationSpringSlow = "400ms",

        // A window of the era opens with a slight zoom and a fade, and closes the same way, faster.
        Dialog = new()
        {
            Enter = Still with { Duration = "200ms", Easing = CurveDecelerate, Scale = "0.9", Opacity = "0", FadeDuration = "200ms", FadeEasing = CurveDecelerate },
            Exit = Still with { Duration = "130ms", Easing = CurveAccelerate, Scale = "0.9", Opacity = "0", FadeDuration = "130ms", FadeEasing = CurveAccelerate },
        },
        Sheet = EdgeMotion,
        Menu = MenuMotion,
        Popover = MenuMotion,
        Tooltip = new()
        {
            Enter = Still with { Delay = "500ms", Opacity = "0", FadeDelay = "500ms", FadeDuration = "130ms", FadeEasing = CurveDecelerate },
            Exit = Still with { Opacity = "0", FadeDuration = "90ms", FadeEasing = CurveAccelerate },
        },
        // A notification balloon slides up from its edge and fades away.
        Snackbar = new()
        {
            Enter = Still with { Duration = "280ms", Easing = CurveDecelerate, Offset = "100%", Opacity = "0", FadeDuration = "200ms", FadeEasing = CurveDecelerate },
            Exit = Still with { Opacity = "0", FadeDuration = "200ms", FadeEasing = CurveAccelerate },
        },
        Drawer = EdgeMotion,
    };

    internal static readonly StateTokens State = new()
    {
        HoverOpacity = "0.08",
        SelectedOpacity = "0.12",
        FocusOpacity = "0.10",
        PressedOpacity = "0.14",
        DraggedOpacity = "0.10",
        DisabledOpacity = "0.4",
        DisabledContainerOpacity = "0.12",
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
        Radius = "var(--flare-shape-extra-small)",
        // Per-size ramp; the four non-default steps used to be literals in badge.css. Values unchanged.
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
        Radius = "var(--flare-shape-small)",
        BorderWidth = "1px",
        Padding = "0.625rem 0.875rem",
        Gap = "0.625rem",
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
        // Tighter than the contained ladder at every size - the text button has no glass to balance.
        TextPaddingInlineXs = "0.375rem",
        TextPaddingInlineSm = "0.5rem",
        TextPaddingInlineMd = "0.75rem",
        TextPaddingInlineLg = "1rem",
        TextPaddingInlineXl = "1.25rem",
        GapXs = "0.1875rem",
        GapSm = "0.25rem",
        GapMd = "0.375rem",
        GapLg = "0.5rem",
        GapXl = "0.5rem",

        // Compact Win7/Office control heights.
        HeightXs = "1.25rem",
        HeightSm = "1.5rem",
        HeightMd = "1.75rem",   // 28px - classic toolbar button
        HeightLg = "2.25rem",
        HeightXl = "2.75rem",

        PaddingInlineXs = "0.5rem",
        PaddingInlineSm = "0.625rem",
        PaddingInlineMd = "0.875rem",
        PaddingInlineLg = "1.125rem",
        PaddingInlineXl = "1.375rem",

        // Gentle 3px corners on every size.
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-extra-small)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-large)"),

        // Aero says "on" with its gloss and its tint, not by reshaping - the corners stay where the
        // rest state left them at each size.
        SelectedRadiusXs = "var(--flare-shape-extra-small)",
        SelectedRadiusSm = "var(--flare-shape-small)",
        SelectedRadiusMd = "var(--flare-shape-small)",
        SelectedRadiusLg = "var(--flare-shape-medium)",
        SelectedRadiusXl = "var(--flare-shape-large)",
        SelectedRadiusSquare = "var(--flare-shape-none)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedColor = "var(--flare-color-on-secondary-container)",
        // Selection is the accent here, whatever the variant: this language does not send each variant
        // somewhere different the way Material does, it just says "on" in the accent colour. The one
        // thing it cannot do is repeat a variant s own resting colour - a selected tonal button painted
        // secondary-container would be indistinguishable from an unselected one, which is exactly what
        // happened when these four pairs were first filled in - so every variant travels to the accent,
        // and the filled toggle starts from a neutral container rather than from the accent it ends on.
        ElevatedSelectedBg = "var(--flare-color-primary)",
        ElevatedSelectedColor = "var(--flare-color-on-primary)",
        FilledSelectedBg = "var(--flare-color-primary)",
        FilledSelectedColor = "var(--flare-color-on-primary)",
        TonalSelectedBg = "var(--flare-color-primary)",
        TonalSelectedColor = "var(--flare-color-on-primary)",
        OutlinedSelectedBg = "var(--flare-color-primary)",
        OutlinedSelectedColor = "var(--flare-color-on-primary)",
        FilledUnselectedBg = "var(--flare-color-surface-container)",
        FilledUnselectedColor = "var(--flare-color-on-surface-variant)",
        // The tonal toggle keeps its variant's container until selected.
        TonalUnselectedBg = "var(--flare-color-secondary-container)",
        TonalUnselectedColor = "var(--flare-color-on-secondary-container)",

        FocusOutline = "1px dotted var(--flare-color-on-surface)",
        FocusOutlineOffset = "-3px",
        FocusShadow = "0 0 0 2px var(--flare-aero-glow, rgba(60,127,177,0.45))",
        FilledHoverShadow = "none",
        // Fades the whole control; no repaint, so the disabled colours are never mixed in.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        DisabledRepaint = "0%",
        DisabledContainer = "transparent",
        DisabledContent = "transparent",
        DisabledStroke = "transparent",

        IconSizeXs = "0.875rem",
        IconSizeSm = "1rem",
        IconSizeMd = "1rem",
        IconSizeLg = "1.125rem",
        IconSizeXl = "1.25rem",
        // A lone glyph takes the labelled size, and the icon side is tucked by one small step.
        IconOnlyIconSizeXs = "var(--flare-btn-icon-size-xs)",
        IconOnlyIconSizeSm = "var(--flare-btn-icon-size-sm)",
        IconOnlyIconSizeMd = "var(--flare-btn-icon-size-md)",
        IconOnlyIconSizeLg = "var(--flare-btn-icon-size-lg)",
        IconOnlyIconSizeXl = "var(--flare-btn-icon-size-xl)",
        IconInset = "var(--flare-spacing-2)",
        TextIconInset = "var(--flare-spacing-2)",
        OutlinedColor = "var(--flare-color-on-surface-variant)",
        OutlinedBorderColor = "var(--flare-color-outline-variant)",

        LabelXs = Typography.LabelMedium,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.TitleMedium,
        LabelXl = Typography.TitleLarge,
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "1px",
        TriggerWidthXs = "1.25rem",
        TriggerWidthSm = "1.25rem",
        TriggerWidthMd = "1.25rem",
        TriggerWidthLg = "1.25rem",
        TriggerWidthXl = "1.25rem",
        CaretSizeXs = "0.75rem",
        CaretSizeSm = "0.75rem",
        CaretSizeMd = "0.75rem",
        CaretSizeLg = "0.75rem",
        CaretSizeXl = "0.75rem",
        MainRadiusXs = new() { TopLeft = "var(--flare-btn-radius-xs-top-left)", BottomLeft = "var(--flare-btn-radius-xs-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusSm = new() { TopLeft = "var(--flare-btn-radius-sm-top-left)", BottomLeft = "var(--flare-btn-radius-sm-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusMd = new() { TopLeft = "var(--flare-btn-radius-md-top-left)", BottomLeft = "var(--flare-btn-radius-md-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusLg = new() { TopLeft = "var(--flare-btn-radius-lg-top-left)", BottomLeft = "var(--flare-btn-radius-lg-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusXl = new() { TopLeft = "var(--flare-btn-radius-xl-top-left)", BottomLeft = "var(--flare-btn-radius-xl-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        TriggerRadiusXs = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-xs-top-right)", BottomRight = "var(--flare-btn-radius-xs-bottom-right)" },
        TriggerRadiusSm = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-sm-top-right)", BottomRight = "var(--flare-btn-radius-sm-bottom-right)" },
        TriggerRadiusMd = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-md-top-right)", BottomRight = "var(--flare-btn-radius-md-bottom-right)" },
        TriggerRadiusLg = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-lg-top-right)", BottomRight = "var(--flare-btn-radius-lg-bottom-right)" },
        TriggerRadiusXl = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-xl-top-right)", BottomRight = "var(--flare-btn-radius-xl-bottom-right)" },
    };

    internal static readonly ToggleButtonTokens ToggleButton = MaterialDesign3Tokens.Design.ToggleButton;

    internal static readonly FabTokens Fab = MaterialDesign3Tokens.Design.Fab with
    {
        RadiusSm = "var(--flare-shape-small)",
        RadiusMd = "var(--flare-shape-medium)",
        RadiusLg = "var(--flare-shape-large)",
    };

    internal static readonly MenuTokens Menu = MaterialDesign3Tokens.Design.Menu with
    {
        // This panel keeps its own lift and a 2dp inset; the Material record states the baseline menu's.
        PanelShadow = "var(--flare-elevation-3)",
        PanelPaddingInline = "0.125rem",
        PanelPaddingBlock = "0.125rem",
    };

    // Checkbox/Radio - Aero: 1px border, gentle corner, no MD3 halo.
    internal static readonly CheckboxTokens Checkbox = MaterialDesign3Tokens.Design.Checkbox with
    {
        BorderWidth = "1px",
        Radius = "var(--flare-shape-extra-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
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
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        // Matches the checkbox this theme derives from Material, so the family stays consistent.
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        // Dims, like its Material lineage.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    internal static readonly ChipTokens Chip = new() { BorderColor = "var(--flare-color-outline)", LabelColor = "var(--flare-color-on-surface-variant)", LabelFont = "var(--flare-typescale-label-large-font)", LabelWeight = "inherit", LabelSpacing = "normal", LabelSizeXs = "var(--flare-typescale-label-small-size)", LabelSizeSm = "var(--flare-typescale-label-small-size)", LabelSizeMd = "var(--flare-typescale-label-large-size)", LabelSizeLg = "var(--flare-typescale-title-small-size)", LabelSizeXl = "var(--flare-typescale-title-medium-size)", Radius = "var(--flare-shape-small)", Height = "2rem", FilledBg = "var(--flare-color-surface-container-high)", ElevatedBg = "var(--flare-color-surface-container-low)", IconSizeXs = "0.875rem", IconSizeSm = "1rem", IconSizeMd = "1.125rem", IconSizeLg = "1.25rem", IconSizeXl = "1.5rem", AvatarSizeXs = "1rem", AvatarSizeSm = "1.125rem", AvatarSizeMd = "1.5rem", AvatarSizeLg = "1.875rem", AvatarSizeXl = "2.25rem", PaddingInlineXs = "var(--flare-spacing-4)", PaddingInlineSm = "var(--flare-spacing-5)", PaddingInlineMd = "var(--flare-spacing-8)", PaddingInlineLg = "var(--flare-spacing-10)", PaddingInlineXl = "var(--flare-spacing-12)" };
    internal static readonly TabsTokens Tabs = MaterialDesign3Tokens.Design.Tabs with
    {
        // Sized by their content, as these tabs always were; the Material record states its 48dp tab.
        TabHeight = "auto",
        TabPaddingInline = "var(--flare-spacing-10)",
    };

    internal static readonly SliderTokens Slider = MaterialDesign3Tokens.Design.Slider with
    {
        // One fixed geometry at every size: a 4px pill rail with a 16px round thumb.
        TrackHeightXs = "4px", TrackHeightSm = "4px", TrackHeightMd = "4px",
        TrackHeightLg = "4px", TrackHeightXl = "4px",
        TrackRadiusXs = "var(--flare-shape-full, 9999px)", TrackRadiusSm = "var(--flare-shape-full, 9999px)",
        TrackRadiusMd = "var(--flare-shape-full, 9999px)", TrackRadiusLg = "var(--flare-shape-full, 9999px)",
        TrackRadiusXl = "var(--flare-shape-full, 9999px)",
        HandleHeightXs = "16px", HandleHeightSm = "16px", HandleHeightMd = "16px",
        HandleHeightLg = "16px", HandleHeightXl = "16px",
        GapRadius = "0px",
        Gap = "0px",
        HandleWidth = "16px",
        HandlePressedWidth = "16px",
        HandleRadius = "var(--flare-shape-full, 9999px)",
        HandleBorderWidth = "1px",
        HandleFill = "var(--flare-color-surface)",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-outline-variant)",
        StateLayerSize = "24px",
        StateHoverOpacity = "0.06",
        StatePressedOpacity = "0.08",
        StopColor = "var(--flare-color-outline)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "2px",
    };

    // Soft Aero drop shadows; color comes from the active ColorScheme shadow vars.
    internal static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0 1px 2px var(--flare-shadow-umbra), 0 1px 1px var(--flare-shadow-penumbra)",
        Level2 = "0 2px 5px var(--flare-shadow-umbra), 0 1px 2px var(--flare-shadow-penumbra)",
        Level3 = "0 5px 12px var(--flare-shadow-umbra), 0 2px 4px var(--flare-shadow-penumbra)",
        Level4 = "0 10px 22px var(--flare-shadow-umbra), 0 3px 6px var(--flare-shadow-penumbra)",
        Level5 = "0 18px 38px var(--flare-shadow-umbra), 0 4px 8px var(--flare-shadow-penumbra)",
    };

    internal static readonly SpacingTokens Spacing = MaterialDesign3Tokens.Design.Spacing;

    // Input - sunken white field with a 1px border; blue focus is finished in scoped CSS.
    internal static readonly InputTokens Input = MaterialDesign3Tokens.Design.Input with
    {
        // Stated here rather than taken from the Material record, whose focus indicator follows its own spec.
        FocusRing = "inset 0 -3px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusBorderBottomColor = "var(--flare-input-border-bottom-color)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
        FilledBg = "var(--flare-color-surface)",
        OutlinedRadius = "var(--flare-shape-extra-small)",
        BorderColor = "var(--flare-color-outline)",
        BorderBottomColor = "var(--flare-color-outline)",
        // Focus and error keep the look this theme has always had: the border stays as it rests, and an error
        // outlines the whole well with a 1px error ring on focus. The Material record states its own spec.
        FocusBorderColor = "var(--flare-color-outline)",
        ErrorBorderColor = "var(--flare-color-error)",
        ErrorBorderBottomColor = "var(--flare-color-error)",
        ErrorFocusRing = "inset 0 0 0 1px var(--flare-color-error)",
        // Explicit variants keep the fields this theme drew before they became theme tokens.
        FilledVariantFocusBorderBottomColor = "var(--flare-color-on-surface-variant)",
        FilledVariantErrorBorderColor = "var(--flare-color-error)",
        FilledVariantErrorFocusRing = "inset 0 0 0 1px var(--flare-color-error)",
        OutlinedVariantFocusRing = "inset 0 0 0 1px var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantFocusBorderColor = "var(--flare-color-outline)",
        OutlinedVariantFocusBorderBottomColor = "var(--flare-color-outline)",
        OutlinedVariantErrorFocusRing = "inset 0 0 0 1px var(--flare-color-error)",
    };

    // Progress - thin classic bar; flat (no MD3 Expressive wavy/round-cap indicator).
    internal static readonly ProgressTokens Progress = MaterialDesign3Tokens.Design.Progress with
    {
        // The large ring keeps the 4px stroke of the smaller ones; the Material record thickens it.
        CircularWidthLg = "4px",
        // Centred content: the ramp follows the indicator sizes, so a label dropped into the
        // smallest ring still fits it.
        ContentColor = "var(--flare-color-on-surface)",
        ContentSizeXs = "0.5rem",
        ContentSizeSm = "0.625rem",
        ContentSizeMd = "0.75rem",
        ContentSizeLg = "1rem",
        ContentSizeXl = "1.25rem",
        TrackRadius = "var(--flare-shape-extra-small)",
        // Aero's glossy capsule bar is chunky by design, so the whole ramp sits above MD3's.
        // Md is the thickness Aero has always drawn.
        LinearHeightXs = "0.375rem",
        LinearHeightSm = "0.5rem",
        LinearHeightMd = "0.75rem",
        LinearHeightLg = "1rem",
        LinearHeightXl = "1.25rem",
        Gap = "0px",
        StopSize = "0px",
        CircularCap = "butt",
        CircularGap = "0",
    };

    // Nav - left accent bar (Office side-nav), no pill.
    internal static readonly NavTokens Nav = new()
    {
        ActiveWeight = "600",
        BadgeWeight = "600",
        RailLabelLineHeight = "1.15",
        ItemRadius = "var(--flare-shape-extra-small)",
        IndicatorRadius = "0",
        ActiveIndicator = "none",
        // The selected item is marked with a left accent bar and no pill (ActiveIndicator is none), so
        // the label is read against the panel surface. It was painted on-secondary-container, a
        // container role with no container under it.
        ActiveColor = "var(--flare-color-on-surface)",
        ItemColor = "var(--flare-color-on-surface-variant)",
        ItemHoverColor = "var(--flare-color-on-surface)",
        GroupColor = "var(--flare-color-on-surface-variant)",
        MetaColor = "var(--flare-color-on-surface-variant)",
        ActiveLeftBar = "3px solid var(--flare-color-primary)",
        LinkDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconSize = "1.25rem",
        ItemHeight = "2.5rem",
    };

    /// <summary>Theme-specific extras (geometry/gloss hooks consumed by the scoped CSS).</summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        // Aero focus glow color used by the scoped button/input CSS.
        [AeroCssVars.Glow] = "rgba(60,127,177,0.55)",
    };

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete Aero design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = MaterialDesign3Tokens.Design with
    {
        FocusRing = "1px dotted var(--flare-color-on-surface)",
        Typography = Typography,
        Shape = Shape,
        Spacing = Spacing,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Badge = Badge,
        Alert = Alert,
        Button = Button,
        ButtonGroup = MaterialDesign3Tokens.Design.ButtonGroup,
        SplitButton = SplitButton,
        ToggleButton = ToggleButton,
        Fab = Fab,
        Menu = Menu,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        // Weights, label sizes and fades this theme set before the Material record moved onto its type scale.
        TableOfContents = MaterialDesign3Tokens.Design.TableOfContents with { ActiveWeight = "600", TitleWeight = "600" },
        Otp = MaterialDesign3Tokens.Design.Otp with { FontWeight = "600" },
        Chart = MaterialDesign3Tokens.Design.Chart with { LabelSize = "9px" },
        Calendar = MaterialDesign3Tokens.Design.Calendar with { OtherMonthOpacity = "0.3" },
        // The date and time pickers keep their own fades and headline spacing; the Material record states the spec's.
        Picker = MaterialDesign3Tokens.Design.Picker with { OutsideOpacity = "0.4", DisabledOpacity = "0.3" },
        TimePicker = MaterialDesign3Tokens.Design.TimePicker with { HeadlineTracking = "0.05em" },
        // A hairline above the bar, which the Material record leaves out.
        BottomNav = MaterialDesign3Tokens.Design.BottomNav with { BorderColor = "var(--flare-color-surface-variant)" },
        Slider = Slider,
        // Aero cards: flat glass panels with a 1px border, small radius, no drop shadow.
        Card = MaterialDesign3Tokens.Design.Card with
        {
            ElevatedBg = "var(--flare-color-surface)",
            Elevation = "none",
            ElevationHover = "none",
            FilledBg = "var(--flare-color-surface)",
            OutlinedBg = "var(--flare-color-surface)",
            OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
            TonalBg = "var(--flare-color-surface-container-high)",
            TonalColor = "var(--flare-color-on-surface)",
            Radius = "var(--flare-shape-small)",
            PaddingTop = "8px",
            PaddingRight = "8px",
            PaddingBottom = "8px",
            PaddingLeft = "8px",
            // Flat panels stay flat on hover, whatever the Material card steps to.
            FilledElevationHover = "none",
            OutlinedElevationHover = "none",
        },
        // The rich tooltip keeps its on-surface text and level 3 shadow; the Material record states the spec's.
        Tooltip = MaterialDesign3Tokens.Design.Tooltip with { RichColor = "var(--flare-color-on-surface)", RichShadow = "var(--flare-elevation-3)" },
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Dialog = MaterialDesign3Tokens.Design.Dialog with { Radius = "var(--flare-shape-large)" },
        Popover = MaterialDesign3Tokens.Design.Popover with { Radius = "var(--flare-shape-small)", Shadow = "var(--flare-elevation-3)" },
        Snackbar = MaterialDesign3Tokens.Design.Snackbar with { Radius = "var(--flare-shape-small)" },
        Extended = Extended,
    };

    // Aero / Office 2010 / Win7 light scheme: cool blue-gray surfaces + an Aero blue accent.
    internal static readonly ColorScheme LightColors = new()
    {
        Primary = "#2A72C9",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#CFE3F7",
        OnPrimaryContainer = "#0A3D70",
        Secondary = "#5A5A5A",
        OnSecondary = "#FFFFFF",
        SecondaryContainer = "#EAEAEA",
        OnSecondaryContainer = "#1C1C1C",
        Tertiary = "#7A5BA6",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#E9DFF5",
        OnTertiaryContainer = "#33155F",
        Error = "#C42B1C",
        OnError = "#FFFFFF",
        ErrorContainer = "#FDE7E4",
        OnErrorContainer = "#5C0A05",
        Success = "#107C10",
        OnSuccess = "#FFFFFF",
        SuccessContainer = "#DFF6DD",
        OnSuccessContainer = "#052505",
        Warning = "#9D5D00",
        OnWarning = "#FFFFFF",
        WarningContainer = "#FFF4CE",
        OnWarningContainer = "#3D2C00",
        Info = "#2A72C9",
        OnInfo = "#FFFFFF",
        InfoContainer = "#D6E8FB",
        OnInfoContainer = "#0A3D70",
        Surface = "#FFFFFF",
        OnSurface = "#1F1F1F",
        SurfaceVariant = "#E6ECF2",
        OnSurfaceVariant = "#44505C",
        OnSurfaceVariant2 = "#66727E",
        SurfaceContainerLowest = "#FFFFFF",
        SurfaceContainerLow = "#F4F7FB",
        SurfaceContainer = "#E9EFF5",
        SurfaceContainerHigh = "#DDE5EE",
        SurfaceContainerHighest = "#D0DAE6",
        SurfaceBright = "#FFFFFF",
        SurfaceDim = "#D0DAE6",
        PrimaryFixed = "#CFE3F7",
        PrimaryFixedDim = "#5B9BE0",
        OnPrimaryFixed = "#0A3D70",
        OnPrimaryFixedVariant = "#0A3D70",
        SecondaryFixed = "#EAEAEA",
        SecondaryFixedDim = "#A8B2BC",
        OnSecondaryFixed = "#1C1C1C",
        OnSecondaryFixedVariant = "#1C1C1C",
        TertiaryFixed = "#E9DFF5",
        TertiaryFixedDim = "#BBA3DC",
        OnTertiaryFixed = "#33155F",
        OnTertiaryFixedVariant = "#33155F",
        Background = "#E7EDF4",
        OnBackground = "#1F1F1F",
        Outline = "#8A97A4",
        OutlineVariant = "#BCC7D2",
        InverseSurface = "#2A2F35",
        InverseOnSurface = "#F0F3F7",
        InversePrimary = "#9FC6F0",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.18)",
        ShadowPenumbra = "rgba(0,0,0,0.12)",
    };

    // Dark "glass" variant - slate blue-gray (Aero never shipped a true dark mode; this is a tasteful one).
    internal static readonly ColorScheme DarkColors = new()
    {
        Primary = "#5B9BE0",
        OnPrimary = "#06243F",
        PrimaryContainer = "#0E3A66",
        OnPrimaryContainer = "#CFE3F7",
        Secondary = "#A8B2BC",
        OnSecondary = "#1A1F24",
        SecondaryContainer = "#39414A",
        OnSecondaryContainer = "#E8EDF2",
        Tertiary = "#BBA3DC",
        OnTertiary = "#2A1248",
        TertiaryContainer = "#42306A",
        OnTertiaryContainer = "#ECE3FA",
        Error = "#E8897C",
        OnError = "#4A0A03",
        ErrorContainer = "#6E1208",
        OnErrorContainer = "#FDE7E4",
        Success = "#5EC75E",
        OnSuccess = "#0B2E0B",
        SuccessContainer = "#0E5814",
        OnSuccessContainer = "#C9F4C9",
        Warning = "#E0B33C",
        OnWarning = "#3D2C00",
        WarningContainer = "#6B5300",
        OnWarningContainer = "#FFF1B3",
        Info = "#5B9BE0",
        OnInfo = "#06243F",
        InfoContainer = "#0E3A66",
        OnInfoContainer = "#CFE3F7",
        Surface = "#1E242B",
        OnSurface = "#E6EBF0",
        SurfaceVariant = "#2A323B",
        OnSurfaceVariant = "#AEB9C4",
        OnSurfaceVariant2 = "#828D98",
        SurfaceContainerLowest = "#171C22",
        SurfaceContainerLow = "#222A31",
        SurfaceContainer = "#28313A",
        SurfaceContainerHigh = "#313B45",
        SurfaceContainerHighest = "#3A4550",
        SurfaceBright = "#3A4550",
        SurfaceDim = "#1E242B",
        PrimaryFixed = "#CFE3F7",
        PrimaryFixedDim = "#5B9BE0",
        OnPrimaryFixed = "#0A3D70",
        OnPrimaryFixedVariant = "#0A3D70",
        SecondaryFixed = "#EAEAEA",
        SecondaryFixedDim = "#A8B2BC",
        OnSecondaryFixed = "#1C1C1C",
        OnSecondaryFixedVariant = "#1C1C1C",
        TertiaryFixed = "#E9DFF5",
        TertiaryFixedDim = "#BBA3DC",
        OnTertiaryFixed = "#33155F",
        OnTertiaryFixedVariant = "#33155F",
        Background = "#171C22",
        OnBackground = "#E6EBF0",
        Outline = "#56616C",
        OutlineVariant = "#3A434D",
        InverseSurface = "#E6EBF0",
        InverseOnSurface = "#1E242B",
        InversePrimary = "#2A72C9",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.45)",
        ShadowPenumbra = "rgba(0,0,0,0.30)",
    };

    /// <summary>Dark-mode override of the focus glow.</summary>
    internal static readonly IReadOnlyDictionary<string, string> DarkExtended = BuildDarkExtended();

    private static IReadOnlyDictionary<string, string> BuildDarkExtended()
    {
        return new Dictionary<string, string>(Extended)
        {
            [AeroCssVars.Glow] = "rgba(91,155,224,0.55)",
        };
    }

    private static TypeStyle T(string weight, string size, string height) =>
        new() { FontFamily = Font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = "0em" };
}
