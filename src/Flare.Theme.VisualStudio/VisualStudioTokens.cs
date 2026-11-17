using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.FluentUI2.Tokens;

namespace Flare.Theme.VisualStudio;

/// <summary>
/// Design tokens for the "Visual Studio 2026" theme - the modern VS shell look: neutral gray
/// surfaces layered editor/tool-window/tab-strip, Segoe UI type, tight 4px geometry, the measured
/// #9184EE accent in dark and flat shadows. The signature document-tab gap and active-tab selection
/// live in the theme-scoped CSS (wwwroot/css/components/tabs.css), driven by these tokens and the
/// `--flare-vs-chrome-*` roles.
/// </summary>
internal class VisualStudioTokens
{
    private const string Font = "'Segoe UI', 'Segoe UI Variable', -apple-system, system-ui, 'Helvetica Neue', Arial, sans-serif";

    internal static readonly TypographyTokens Typography = new()
    {
        // No design language names a code face, so this is the generic - twice, because a generic
        // family standing alone makes several engines use their own "monospace default size".
        MonoFont = "monospace, monospace",
        WeightBold = "700",
        DisplayLarge = T("600", "2.125rem", "2.75rem"),
        DisplayMedium = T("600", "1.625rem", "2.125rem"),
        DisplaySmall = T("600", "1.3125rem", "1.75rem"),
        HeadlineLarge = T("600", "1.125rem", "1.5rem"),
        HeadlineMedium = T("600", "1.0625rem", "1.4375rem"),
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

    // Modern VS 2026 geometry - tight, lightly rounded 4px chrome (square-ish but not hard 0px).
    internal static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "2px",
        Small = "4px",
        Medium = "4px",
        Large = "6px",
        ExtraLarge = "8px",
        Full = "9999px",

        // IDE chrome does not move under the pointer; a control that reshapes itself would be noise.
        MorphDuration = "0s",
        MorphEasing = "cubic-bezier(0.4, 0, 0.2, 1)",
    };

    // Snappy IDE transitions: surfaces fade rather than travel, and menus close at once.
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

    private static readonly SurfaceMotionTokens MenuMotion = new()
    {
        Enter = Still with { Opacity = "0", FadeDuration = "80ms", FadeEasing = CurveDecelerate },
        Exit = Still with { Opacity = "0" },
    };

    private static readonly SurfaceMotionTokens EdgeMotion = new()
    {
        Enter = Still with { Duration = "180ms", Easing = CurveDecelerate, Offset = "100%" },
        Exit = Still with { Duration = "120ms", Easing = CurveAccelerate, Offset = "100%" },
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
        EasingStepEnd = "steps(1, end)",
        EasingStepStart = "steps(1, start)",

        DurationStateChange = "80ms",
        DurationSmallMove = "120ms",
        DurationEnterSmall = "120ms",
        DurationEnterMedium = "180ms",
        DurationEnterLarge = "260ms",
        DurationExitSmall = "80ms",
        DurationExitMedium = "120ms",
        DurationExitLarge = "180ms",
        DurationCycle = "1000ms",
        DurationCycleLong = "2000ms",

        // An IDE chrome theme wants motion to stay out of the way, so nothing here springs; the
        // durations are the short end of the ramp for the same reason.
        EasingSpringFast = CurveStandard,
        EasingSpring = CurveStandard,
        EasingSpringSlow = CurveStandard,
        DurationSpringFast = "90ms",
        DurationSpring = "180ms",
        DurationSpringSlow = "360ms",

        Dialog = new()
        {
            Enter = Still with { Opacity = "0", FadeDuration = "120ms", FadeEasing = CurveDecelerate },
            Exit = Still with { Opacity = "0", FadeDuration = "80ms", FadeEasing = CurveAccelerate },
        },
        Sheet = EdgeMotion,
        Menu = MenuMotion,
        Popover = MenuMotion,
        Tooltip = new()
        {
            Enter = Still with { Delay = "400ms", Opacity = "0", FadeDelay = "400ms", FadeDuration = "80ms", FadeEasing = CurveDecelerate },
            Exit = Still with { Opacity = "0" },
        },
        // An info bar slides in from its edge and fades out.
        Snackbar = new()
        {
            Enter = Still with { Duration = "180ms", Easing = CurveDecelerate, Offset = "100%", Opacity = "0", FadeDuration = "120ms", FadeEasing = CurveDecelerate },
            Exit = Still with { Opacity = "0", FadeDuration = "120ms", FadeEasing = CurveAccelerate },
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
        PaddingXXs = "0.1875rem",
        PaddingXSm = "0.1875rem",
        LabelSizeXs = "0.5625rem",
        LabelSizeSm = "0.625rem",
        Radius = "var(--flare-shape-extra-small)",
        // Per-size ramp, one token per step.
        MinWidthXs = "0.75rem",   MinWidthSm = "0.875rem",   MinWidthMd = "1rem",   MinWidthLg = "1.25rem",  MinWidthXl = "1.5rem",
        HeightXs   = "0.75rem",   HeightSm   = "0.875rem",   HeightMd   = "1rem",   HeightLg   = "1.25rem",  HeightXl   = "1.5rem",
        DotSizeXs  = "0.25rem",   DotSizeSm  = "0.3125rem",  DotSizeMd  = "0.375rem", DotSizeLg = "0.5rem",  DotSizeXl  = "0.625rem",
        Offset = "0.375rem",
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

    internal static readonly EmptyStateTokens EmptyState = new()
    {
        IconOpacity = "0.5",
        DescriptionMaxWidth = "32rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        // Tighter than the contained ladder at every size - a toolbar button hugs its label.
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

        // Compact VS command/toolbar button heights. The measured VS 2026 command button is a 24px
        // square carrying a 16px glyph, so Xs stops at that size.
        HeightXs = "1.5rem",  // 24px - the measured command button
        HeightSm = "1.5rem",
        HeightMd = "1.75rem",   // 28px - classic VS command button
        HeightLg = "2.25rem",
        HeightXl = "2.75rem",

        PaddingInlineXs = "0.5rem",
        PaddingInlineSm = "0.625rem",
        PaddingInlineMd = "0.875rem",
        PaddingInlineLg = "1.125rem",
        PaddingInlineXl = "1.375rem",

        // Tight 4px corners on every size.
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-large)"),

        // A toggled tool-window button in Visual Studio changes colour and stays exactly where it is;
        // reshaping on selection would read as a different control appearing.
        SelectedRadiusXs = "var(--flare-shape-small)",
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

        FocusOutline = "1px solid var(--flare-color-primary)",
        FocusOutlineOffset = "1px",
        // The measured VS 2026 focus is a single 1px accent line - no outer glow ring.
        FocusShadow = "none",
        FilledHoverShadow = "none",
        // Fluent repaints disabled controls in a flat palette rather than fading them: the solid variants take
        // the disabled fill, every variant the disabled foreground, the outlined one the disabled stroke.
        DisabledOpacity = "1",
        DisabledRepaint = "100%",
        DisabledContainer = "var(--flare-fluent-disabled-bg, #F0F0F0)",
        DisabledContent = "var(--flare-fluent-disabled-fg, #BDBDBD)",
        DisabledStroke = "var(--flare-fluent-disabled-border, #E0E0E0)",

        IconSizeXs = "1rem",
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

    internal static readonly ToggleButtonTokens ToggleButton = FluentUI2Tokens.Design.ToggleButton;

    // VS 2026 scrollbars: a 2px square #9D9D9D thumb on the panel surface. The size is painted by the
    // WebKit path; vs-base.css keeps the standard properties at auto so Chromium (WebView2) uses it.
    internal static readonly ScrollbarTokens Scrollbar = new()
    {
        Width = "auto",
        Size = "2px",
        Thumb = "#9D9D9D",
        ThumbHover = "#BDBDBD",
        Track = "var(--flare-vs-chrome-scrollbar-track)",
        Radius = "0px",
    };

    internal static readonly FabTokens Fab = FluentUI2Tokens.Design.Fab with
    {
        RadiusSm = "var(--flare-shape-small)",
        RadiusMd = "var(--flare-shape-medium)",
        RadiusLg = "var(--flare-shape-large)",
    };

    internal static readonly MenuTokens Menu = FluentUI2Tokens.Design.Menu with
    {
        // VS 2026 dropdowns: a #2C2C2C panel with 6px corners and a 2px inset, 22px items on a 24px
        // pitch, 4px hover pills, a 16px icon/check gutter and the measured shortcut/check/rule
        // colours. The hover fill is a scoped state-layer re-point in components/ide.css.
        PanelBg = "var(--flare-vs-chrome-menu)",
        PanelRadius = "var(--flare-shape-large)",
        PanelShadow = "var(--flare-elevation-3)",
        PanelPaddingBlock = "0.0625rem",
        PanelPaddingInline = "0.0625rem",
        ItemHeight = "1.375rem",
        ItemPaddingBlock = "0",
        ItemPaddingInline = "0.75rem",
        ItemGapBetween = "0.125rem",
        ItemGap = "0.125rem",
        ItemIconSize = "1rem",
        ItemRadius = "var(--flare-shape-small)",
        ItemRadiusEnd = "var(--flare-shape-small)",
        ItemShortcutColor = "var(--flare-vs-chrome-text-disabled)",
        ItemCheckColor = "var(--flare-vs-chrome-accent)",
        ItemDisabledOpacity = "1",
        SeparatorRule = "1px solid var(--flare-vs-chrome-divider)",
        SeparatorSpacing = "0.3125rem",
    };

    // Checkbox/Radio - VS: 1px border, 4px corner, no MD3 halo, a single 1px accent focus line.
    internal static readonly CheckboxTokens Checkbox = FluentUI2Tokens.Design.Checkbox with
    {
        BorderWidth = "1px",
        Radius = "var(--flare-shape-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        FocusOutline = "1px solid var(--flare-color-primary)",
        FocusOutlineOffset = "0",
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
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        // Matches the checkbox this theme derives from Fluent: one 1px accent line, no outer glow.
        FocusOutline = "1px solid var(--flare-color-primary)",
        FocusOutlineOffset = "0",
        FocusShadow = "none",
        // Repaints, like its Fluent lineage.
        DisabledOpacity = "1",
    };

    internal static readonly ChipTokens Chip = new() { BorderColor = "var(--flare-color-outline)", LabelWeight = "inherit", LabelSpacing = "normal", Height = "2rem", IconSizeXs = "0.875rem", IconSizeSm = "1rem", IconSizeMd = "1.125rem", IconSizeLg = "1.25rem", IconSizeXl = "1.5rem", AvatarSizeXs = "1rem", AvatarSizeSm = "1.125rem", AvatarSizeMd = "1.5rem", AvatarSizeLg = "1.875rem", AvatarSizeXl = "2.25rem" };

    // VS 2026 document tabs: thin top accent line, neutral strip, the active tab fills with the
    // editor surface. The full document-tab look (gap, top accent, hover) is finished in the
    // theme-scoped tabs.css using these tokens.
    internal static readonly TabsTokens Tabs = new()
    {
        // Follows Fluent: 20px tab icon.
        IconSize = "1.25rem",
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "2px",
        // The IDE's own tab rail is already 2dp, so a subordinate strip separates itself by dropping
        // the accent label rather than by getting thinner still.
        SecondaryIndicatorThickness = "2px",
        DividerColor = "var(--flare-color-outline-variant)",
        SelectedBg = "var(--flare-color-surface)",
        SelectedFg = "var(--flare-color-on-surface)",
        TrackBg = "var(--flare-color-surface-container)",
        // The track belongs to the segmented variants; the folder tabs keep the page behind them.
        OutlinedStripBg = "transparent",
        PillRadius = "var(--flare-shape-small)",
        TabDisabledOpacity = "1",
        ScrollDisabledOpacity = "1",
        TabHeight = "auto",
        TabMinWidth = "0",
        TabPaddingInline = "var(--flare-spacing-10)",
        // Flare's own part sizes, which no Fluent table states.
        ScrollIconSize = "1.25rem",
        CloseSize = "1.25rem",
        CloseIconSize = "1rem",
        BadgeSize = "1.25rem",
    };

    internal static readonly SliderTokens Slider = FluentUI2Tokens.Design.Slider with
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
        InactiveColor = "var(--flare-color-outline-variant)",
        StateLayerSize = "24px",
        StateHoverOpacity = "0.06",
        StatePressedOpacity = "0.08",
        FocusOutline = "none",
        FocusOutlineOffset = "0px",
        StopColor = "var(--flare-color-outline)",
        StopSize = "2px",
    };

    // Switch - the Fluent-derived record carries the MD3 grow-on-check thumb (16 -> 24px) that only
    // the Fluent Extended dict corrects; VS supplies its own Extended, so that correction was lost and
    // the 24px "on" thumb overflowed the compact 40x20 (2.5rem x 1.25rem) Fluent rail. Set the authentic
    // Visual Studio 2026 / Fluent v9 toggle here: a 14px thumb, the same size checked and unchecked, that
    // fits the 20px rail (a subtle grow to 16px on press, still inside the rail).
    internal static readonly SwitchTokens Switch = FluentUI2Tokens.Design.Switch with
    {
        // md step only; xs/sm/lg/xl inherited from the Fluent ramp.
        ThumbOffSizeMd = "0.875rem",              // 14px
        ThumbOnSizeMd = "0.875rem",               // 14px - does not grow on check
        ThumbPressedOffSizeMd = "1rem",           // 16px press grow, still < 20px rail
        ThumbPressedOnSizeMd = "1rem",
        ThumbOffLeftMd = "0.1875rem",             // 3px from the leading edge
        ThumbOnLeftMd = "calc(100% - 1.0625rem)", // 14px thumb + 3px trailing margin
    };

    // Flat VS shadows; color comes from the active ColorScheme shadow vars.
    internal static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0 1px 2px var(--flare-shadow-umbra), 0 0px 1px var(--flare-shadow-penumbra)",
        Level2 = "0 2px 4px var(--flare-shadow-umbra), 0 1px 2px var(--flare-shadow-penumbra)",
        Level3 = "0 4px 10px var(--flare-shadow-umbra), 0 1px 3px var(--flare-shadow-penumbra)",
        Level4 = "0 8px 18px var(--flare-shadow-umbra), 0 2px 5px var(--flare-shadow-penumbra)",
        Level5 = "0 14px 30px var(--flare-shadow-umbra), 0 4px 8px var(--flare-shadow-penumbra)",
    };

    internal static readonly SpacingTokens Spacing = FluentUI2Tokens.Design.Spacing;

    // Card - flat VS panel with a 1px border, no shadow (all variants share the flat look).
    internal static readonly CardTokens Card = FluentUI2Tokens.Design.Card with
    {
        ElevatedBg = "var(--flare-color-surface)",
        FilledBg = "var(--flare-color-surface)",
        OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
        TonalBg = "var(--flare-color-surface-container-high)",
        Radius = "var(--flare-shape-small)",
        Elevation = "none",
        FilledElevation = "none",
        TonalElevation = "none",
        // Flat panels stay flat on hover; the Fluent card lifts its shadowed variants.
        FilledElevationHover = "none",
        TonalElevationHover = "none",
        PaddingTop = "8px",
        PaddingRight = "8px",
        PaddingBottom = "8px",
        PaddingLeft = "8px",
        // Keep the neutral baseline for the two members the Fluent card overrides.
        ElevationHover = "var(--flare-elevation-2)",
        TonalColor = "var(--flare-color-on-secondary-container)",
    };

    // Input - the measured VS 2026 search/input well: a 23px box with an 8px text inset, a #353535
    // fill, a 1px #454545 stroke and 4px corners. Focus is a single 1px accent outline, no glow.
    internal static readonly InputTokens Input = FluentUI2Tokens.Design.Input with
    {
        FilledBg = "var(--flare-vs-chrome-field)",
        OutlinedRadius = "var(--flare-shape-small)",
        BorderColor = "var(--flare-color-outline)",
        BorderBottomColor = "var(--flare-color-outline)",
        // VS/Windows focuses with a rectangle around the control - a real 1px outline, not a shadow.
        FocusRing = "none",
        FocusOutline = "1px solid var(--fc-main, var(--flare-color-primary))",
        FocusOutlineOffset = "0",
        // The resting stroke stays neutral on the default variant; the outline is the focus mark.
        FocusBorderColor = "var(--flare-color-outline)",
        FocusBorderBottomColor = "var(--flare-color-outline)",
        // Height ramp measured from the 23px default: 20/22/23/26/30px. Block padding only places
        // the line inside that definite height, so each step leaves room for text and stays ordered.
        HeightXs = "1.25rem",   // 20px
        HeightSm = "1.375rem",  // 22px
        HeightMd = "1.4375rem", // 23px - the measured input/search/combobox well
        HeightLg = "1.625rem",  // 26px
        HeightXl = "1.875rem",  // 30px
        PaddingXs = "0.0625rem 0.375rem",
        PaddingSm = "0.09375rem 0.4375rem",
        PaddingMd = "0.125rem 0.5rem",
        PaddingLg = "0.1875rem 0.625rem",
        PaddingXl = "0.25rem 0.75rem",
        IconSize = "1rem",
        // An explicit variant draws its focus with the ring, so it drops the ring and recolors the
        // 1px stroke instead: the same single accent line as the default variant.
        FilledVariantBg = "var(--flare-vs-chrome-field)",
        FilledVariantBorderColor = "var(--flare-color-outline)",
        FilledVariantBorderBottomColor = "var(--flare-color-outline)",
        FilledVariantRadius = "var(--flare-shape-small)",
        FilledVariantFocusRing = "none",
        FilledVariantFocusBorderColor = "var(--fc-main, var(--flare-color-primary))",
        FilledVariantFocusBorderBottomColor = "var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantBg = "var(--flare-vs-chrome-field)",
        OutlinedVariantBorderColor = "var(--flare-color-outline)",
        OutlinedVariantBorderBottomColor = "var(--flare-color-outline)",
        OutlinedVariantRadius = "var(--flare-shape-small)",
        OutlinedVariantFocusRing = "none",
        OutlinedVariantFocusBorderColor = "var(--fc-main, var(--flare-color-primary))",
        OutlinedVariantFocusBorderBottomColor = "var(--fc-main, var(--flare-color-primary))",
    };

    // Progress - thin VS bar (derives Fluent's already-flat progress).
    internal static readonly ProgressTokens Progress = FluentUI2Tokens.Design.Progress with
    {
        // Centred content: the ramp follows the indicator sizes, so a label dropped into the
        // smallest ring still fits it.
        ContentSizeXs = "0.5rem",
        ContentSizeSm = "0.625rem",
        ContentSizeMd = "0.75rem",
        ContentSizeLg = "1rem",
        ContentSizeXl = "1.25rem",
        TrackRadius = "var(--flare-shape-extra-small)",
        // Md is the thickness Visual Studio has always drawn; the ramp runs both ways from it.
        LinearHeightXs = "0.25rem",
        LinearHeightSm = "0.3125rem",
        LinearHeightMd = "0.375rem",
        LinearHeightLg = "0.5rem",
        LinearHeightXl = "0.75rem",
        Gap = "0px",
        StopSize = "0px",
        CircularWidthXs = "3px",
        CircularWidthSm = "3px",
        CircularWidthMd = "4px",
        CircularWidthLg = "4px",
        CircularWidthXl = "5px",
    };

    // Nav - left accent bar (VS solution-explorer style), no pill.
    internal static readonly NavTokens Nav = new()
    {
        ActiveWeight = "600",
        BadgeWeight = "600",
        RailLabelLineHeight = "1.15",
        IndicatorRadius = "0",
        ActiveIndicator = "none",
        // The selected item is marked with a left accent bar and no pill (ActiveIndicator is none), so
        // the label is read against the panel surface.
        ActiveColor = "var(--flare-color-on-surface)",
        ActiveLeftBar = "2px solid var(--flare-color-primary)",
        LinkDisabledOpacity = "1",
        IconSize = "1.25rem",
        ItemHeight = "2.5rem",
    };

    // Tree - VS Solution Explorer: the measured 16px glyph inside a 24px expander target. Only the
    // glyph shrinks; the toggle keeps its size so the pointer target does not fall below 24px.
    internal static readonly TreeTokens Tree = new()
    {
        ToggleSize = "1.5rem",
        IconSize = "1rem",
    };

    /// <summary>Theme-specific extras (VS chrome hooks consumed by the scoped CSS and by hosts).</summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        // VS focus ring color used by the scoped button/input CSS.
        [VisualStudioCssVars.Focus] = "var(--flare-color-primary)",

        // The checkbox, radio and menu focus rings inherited from Fluent 2 name these, and this map
        // replaces Fluent's rather than extending it.
        [FluentCssVars.FocusStrokeColor] = "#000000",
        [FluentCssVars.FocusStrokeOuter] = "#FFFFFF",

        // Tabs - VS 2026 "flowing" document tabs, consumed by the theme-scoped tabs.css.
        [VisualStudioCssVars.TabGap] = "2px",
        [VisualStudioCssVars.TabStripBg] = "var(--flare-vs-chrome-tab-strip)",
        // Rounded top corners so the strip flows around the active/hovered tab (wrap-around).
        [VisualStudioCssVars.TabRadius] = "var(--flare-shape-large)",
        [VisualStudioCssVars.TabActiveBg] = "var(--flare-vs-chrome-tab-active)",

        // VS 2026 chrome surfaces. Light mode names the existing light scheme roles, so the palette
        // above stays valid; DarkExtended below replaces them with the measured VS 2026 values.
        [VisualStudioCssVars.ChromeTitleBar] = "var(--flare-color-surface-container-low)",
        [VisualStudioCssVars.ChromeCommandBar] = "var(--flare-color-surface-container)",
        [VisualStudioCssVars.ChromeTabStrip] = "var(--flare-color-surface-container)",
        [VisualStudioCssVars.ChromeTabActive] = "var(--flare-color-surface)",
        [VisualStudioCssVars.ChromeEditor] = "var(--flare-color-surface)",
        [VisualStudioCssVars.ChromeBreadcrumb] = "var(--flare-color-surface-container-low)",
        [VisualStudioCssVars.ChromeBreadcrumbEdge] = "var(--flare-color-outline-variant)",
        [VisualStudioCssVars.ChromeToolWindow] = "var(--flare-color-surface)",
        [VisualStudioCssVars.ChromeToolTab] = "var(--flare-color-surface-container)",
        [VisualStudioCssVars.ChromeField] = "var(--flare-color-surface)",
        [VisualStudioCssVars.ChromeBottomPanel] = "var(--flare-color-surface-container)",
        [VisualStudioCssVars.ChromeBottomTabUnderline] = "var(--flare-color-outline-variant)",
        [VisualStudioCssVars.ChromeStatusBar] = "var(--flare-color-surface-container)",
        [VisualStudioCssVars.ChromeBarEdge] = "var(--flare-color-surface-container-high)",
        [VisualStudioCssVars.ChromeDivider] = "var(--flare-color-outline-variant)",
        [VisualStudioCssVars.ChromeOutline] = "var(--flare-color-outline)",
        [VisualStudioCssVars.ChromeMenu] = "var(--flare-color-surface-container)",
        [VisualStudioCssVars.ChromeHover] = "var(--flare-color-surface-container-high)",
        [VisualStudioCssVars.ChromeMenuBarHover] = "var(--flare-color-surface-container-high)",
        [VisualStudioCssVars.ChromeAccent] = "var(--flare-color-primary)",
        [VisualStudioCssVars.ChromeText] = "var(--flare-color-on-surface)",
        [VisualStudioCssVars.ChromeTextSecondary] = "var(--flare-color-on-surface-variant)",
        [VisualStudioCssVars.ChromeTextDisabled] = "var(--flare-color-on-surface-variant2)",
        [VisualStudioCssVars.ChromeScrollbarThumb] = "var(--flare-color-on-surface-variant2)",
        [VisualStudioCssVars.ChromeScrollbarTrack] = "var(--flare-color-surface)",
        [VisualStudioCssVars.ChromeSearchRadius] = "var(--flare-shape-large)",

        // The optional IDE shell reads its public hooks; this theme points them at the chrome roles so
        // a host gets the VS surfaces without overriding a single generic color role.
        [IdeSurface] = "var(--flare-vs-chrome-tool-window)",
        [IdeSurfaceVariant] = "var(--flare-vs-chrome-command-bar)",
        [IdeOutlineVariant] = "var(--flare-vs-chrome-divider)",
        [IdeOnSurface] = "var(--flare-vs-chrome-text)",
        [IdeOnSurfaceVariant] = "var(--flare-vs-chrome-text-secondary)",
        [IdePrimary] = "var(--flare-vs-chrome-accent)",
        [IdeMenuBarBg] = "var(--flare-vs-chrome-title-bar)",
        [IdeCommandBarBg] = "var(--flare-vs-chrome-command-bar)",
        [IdeTabStripBg] = "var(--flare-vs-chrome-tab-strip)",
        [IdeTabActiveBg] = "var(--flare-vs-chrome-tab-active)",
        [IdeEditorBg] = "var(--flare-vs-chrome-editor)",
        [IdeToolHeaderBg] = "var(--flare-vs-chrome-tool-tab)",
        [IdeToolTabActiveBg] = "var(--flare-vs-chrome-tool-tab)",
        [IdeBottomTabStripBg] = "var(--flare-vs-chrome-bottom-panel)",
        [IdeStatusBarBg] = "var(--flare-vs-chrome-status-bar)",
        [IdePanelBg] = "var(--flare-vs-chrome-tool-window)",

        // Command bar - the measured 28px strip: #262626 under the groups, #1C1C1C in the gaps and
        // the rest of the row, with a #0E0E0E hard edge above and below. The group carries its own
        // 2px block padding, so its surface spans the full 28px strip.
        [IdeToolbarSurface] = "var(--flare-vs-chrome-title-bar)",
        [IdeToolbarGroupSurface] = "var(--flare-vs-chrome-command-bar)",
        [IdeToolbarGap] = "0.25rem",
        [IdeToolbarDenseGap] = "0.25rem",
        [IdeToolbarHeight] = "1.75rem",
        [IdeToolbarDenseHeight] = "1.75rem",
        [IdeToolbarPadding] = "0",
        [IdeToolbarDensePadding] = "0",
        [IdeToolbarEdgeColor] = "var(--flare-vs-chrome-bar-edge)",

        // The measured document tab close glyph is 10px inside a 20px target (the target size is in
        // the theme's scoped tabs.css); the default keeps every other theme's 14px glyph.
        [IdeDocTabCloseIcon] = "0.625rem",

        // The measured menu bar is 30px tall including its 1px bottom edge; scoped ide.css paints
        // the edge, the gap and the 24px hover pill.
        [IdeMenubarHeight] = "1.875rem",

        // Status bar - the measured 24px #141414 strip with 8px inline padding, a 12px gap between
        // segments and a 16px icon slot (sized in scoped ide.css).
        [IdeStatusBarSurface] = "var(--flare-vs-chrome-status-bar)",
        [IdeStatusBarColor] = "var(--flare-vs-chrome-text)",
        [IdeStatusBarPadding] = "0 0.5rem",
        [IdeStatusBarGap] = "0.75rem",

        // Bottom-panel sheet tabs - the measured 33px strip on #1C1C1C carrying a 3px #404040
        // underline under every tab, 14px gaps and a 7px start inset. The bar sits 4px above the
        // strip's bottom edge through the bottom padding; selection has no pill, fill or outline; a
        // 1px accent line separates the strip from the editor above it.
        [IdeSheetTabsSurface] = "var(--flare-vs-chrome-bottom-panel)",
        [IdeSheetTabsColor] = "var(--flare-vs-chrome-text-secondary)",
        [IdeSheetTabsActiveBg] = "transparent",
        [IdeSheetTabsActiveBorderColor] = "transparent",
        [IdeSheetTabsActiveColor] = "var(--flare-vs-chrome-text)",
        [IdeSheetTabsUnderlineColor] = "var(--flare-vs-chrome-bottom-tab-underline)",
        [IdeSheetTabsDivider] = "var(--flare-vs-chrome-accent)",
        [IdeSheetTabsHeight] = "2.0625rem",
        [IdeSheetTabsGap] = "0.875rem",
        [IdeSheetTabsPadding] = "0 0.4375rem 0.25rem",
        [IdeSheetTabsUnderlineHeight] = "3px",

        // Editor breadcrumb/toolbar row - the measured 22px #2B2B2B strip under the document tabs,
        // edged in #2E2E2E. The trailing action area is its own #2E2E2E block, the current segment
        // takes the editor surface, and neighbouring segments are separated by the chrome divider.
        [IdeBreadcrumbSurface] = "var(--flare-vs-chrome-breadcrumb)",
        [IdeBreadcrumbColor] = "var(--flare-vs-chrome-text-secondary)",
        [IdeBreadcrumbEdgeColor] = "var(--flare-vs-chrome-breadcrumb-edge)",
        [IdeBreadcrumbSeparatorColor] = "var(--flare-vs-chrome-divider)",
        [IdeBreadcrumbActiveSurface] = "var(--flare-vs-chrome-editor)",
        [IdeBreadcrumbActiveColor] = "var(--flare-vs-chrome-text)",
        [IdeBreadcrumbActionsSurface] = "var(--flare-vs-chrome-breadcrumb-edge)",
        [IdeBreadcrumbBarHeight] = "1.375rem",
        [IdeBreadcrumbBarPadding] = "0.5rem",
        [IdeBreadcrumbSegmentPadding] = "0.5rem",
        [IdeBreadcrumbSegmentGap] = "0.25rem",
        [IdeBreadcrumbIconSize] = "1rem",
        [IdeBreadcrumbChevronSize] = "0.75rem",
        [IdeBreadcrumbActionsPadding] = "0.5rem",
        [IdeBreadcrumbActionsGap] = "0.25rem",

        // The measured chrome text is 12px: the shell's body-small hooks resolve to this theme's
        // BodyMedium role, while menu and tree labels keep BodyLarge (13px).
        [IdeTypescaleBodySmallFont] = "var(--flare-typescale-body-medium-font)",
        [IdeTypescaleBodySmallSize] = "var(--flare-typescale-body-medium-size)",
        [IdeTypescaleBodySmallWeight] = "var(--flare-typescale-body-medium-weight)",
        [IdeTypescaleBodySmallHeight] = "var(--flare-typescale-body-medium-height)",
        [IdeTypescaleBodySmallSpacing] = "var(--flare-typescale-body-medium-spacing)",
        // The tool-window title reads its weight from the label-small hook; VS titles are not bold.
        [IdeTypescaleLabelSmallWeight] = "var(--flare-typescale-body-medium-weight)",
    };

    // The IDE package's public override hooks, named here so the theme can point them at its chrome
    // roles without taking a dependency on the optional components package.
    internal const string IdeSurface = "--flare-ide-surface";
    internal const string IdeSurfaceVariant = "--flare-ide-surface-variant";
    internal const string IdeOutlineVariant = "--flare-ide-outline-variant";
    internal const string IdeOnSurface = "--flare-ide-on-surface";
    internal const string IdeOnSurfaceVariant = "--flare-ide-on-surface-variant";
    internal const string IdePrimary = "--flare-ide-primary";
    internal const string IdeMenuBarBg = "--flare-ide-menubar-bg";
    internal const string IdeCommandBarBg = "--flare-ide-commandbar-bg";
    internal const string IdeTabStripBg = "--flare-ide-tab-strip-bg";
    internal const string IdeTabActiveBg = "--flare-ide-tab-active-bg";
    internal const string IdeEditorBg = "--flare-ide-editor-bg";
    internal const string IdeToolHeaderBg = "--flare-ide-tool-header-bg";
    internal const string IdeToolTabActiveBg = "--flare-ide-tool-tab-active-bg";
    internal const string IdeBottomTabStripBg = "--flare-ide-bottom-tab-strip-bg";
    internal const string IdeStatusBarBg = "--flare-ide-statusbar-bg";
    internal const string IdePanelBg = "--flare-ide-panel-bg";
    internal const string IdeToolbarSurface = "--flare-ide-toolbar-surface";
    internal const string IdeToolbarGroupSurface = "--flare-ide-toolbar-group-surface";
    internal const string IdeToolbarGap = "--flare-ide-toolbar-gap";
    internal const string IdeToolbarDenseGap = "--flare-ide-toolbar-dense-gap";
    internal const string IdeToolbarHeight = "--flare-ide-toolbar-height";
    internal const string IdeToolbarDenseHeight = "--flare-ide-toolbar-dense-height";
    internal const string IdeToolbarPadding = "--flare-ide-toolbar-padding";
    internal const string IdeToolbarDensePadding = "--flare-ide-toolbar-dense-padding";
    internal const string IdeToolbarEdgeColor = "--flare-ide-toolbar-edge-color";
    internal const string IdeDocTabCloseIcon = "--flare-ide-doc-tab-close-icon";
    internal const string IdeMenubarHeight = "--flare-ide-menubar-height";
    internal const string IdeStatusBarSurface = "--flare-ide-statusbar-surface";
    internal const string IdeStatusBarColor = "--flare-ide-statusbar-color";
    internal const string IdeStatusBarPadding = "--flare-ide-statusbar-padding";
    internal const string IdeStatusBarGap = "--flare-ide-statusbar-gap";
    internal const string IdeSheetTabsSurface = "--flare-ide-sheet-tabs-surface";
    internal const string IdeSheetTabsColor = "--flare-ide-sheet-tabs-color";
    internal const string IdeSheetTabsActiveBg = "--flare-ide-sheet-tabs-active-bg";
    internal const string IdeSheetTabsActiveBorderColor = "--flare-ide-sheet-tabs-active-border-color";
    internal const string IdeSheetTabsActiveColor = "--flare-ide-sheet-tabs-active-color";
    internal const string IdeSheetTabsUnderlineColor = "--flare-ide-sheet-tabs-underline-color";
    internal const string IdeSheetTabsDivider = "--flare-ide-sheet-tabs-divider";
    internal const string IdeSheetTabsHeight = "--flare-ide-sheet-tabs-height";
    internal const string IdeSheetTabsGap = "--flare-ide-sheet-tabs-gap";
    internal const string IdeSheetTabsPadding = "--flare-ide-sheet-tabs-padding";
    internal const string IdeSheetTabsUnderlineHeight = "--flare-ide-sheet-tabs-underline-height";
    internal const string IdeBreadcrumbSurface = "--flare-ide-breadcrumb-surface";
    internal const string IdeBreadcrumbColor = "--flare-ide-breadcrumb-color";
    internal const string IdeBreadcrumbEdgeColor = "--flare-ide-breadcrumb-edge-color";
    internal const string IdeBreadcrumbSeparatorColor = "--flare-ide-breadcrumb-separator-color";
    internal const string IdeBreadcrumbActiveSurface = "--flare-ide-breadcrumb-active-surface";
    internal const string IdeBreadcrumbActiveColor = "--flare-ide-breadcrumb-active-color";
    internal const string IdeBreadcrumbActionsSurface = "--flare-ide-breadcrumb-actions-surface";
    internal const string IdeBreadcrumbBarHeight = "--flare-ide-breadcrumb-bar-height";
    internal const string IdeBreadcrumbBarPadding = "--flare-ide-breadcrumb-bar-padding";
    internal const string IdeBreadcrumbSegmentPadding = "--flare-ide-breadcrumb-segment-padding";
    internal const string IdeBreadcrumbSegmentGap = "--flare-ide-breadcrumb-segment-gap";
    internal const string IdeBreadcrumbIconSize = "--flare-ide-breadcrumb-icon-size";
    internal const string IdeBreadcrumbChevronSize = "--flare-ide-breadcrumb-chevron-size";
    internal const string IdeBreadcrumbActionsPadding = "--flare-ide-breadcrumb-actions-padding";
    internal const string IdeBreadcrumbActionsGap = "--flare-ide-breadcrumb-actions-gap";
    internal const string IdeTypescaleBodySmallFont = "--flare-ide-typescale-body-small-font";
    internal const string IdeTypescaleBodySmallSize = "--flare-ide-typescale-body-small-size";
    internal const string IdeTypescaleBodySmallWeight = "--flare-ide-typescale-body-small-weight";
    internal const string IdeTypescaleBodySmallHeight = "--flare-ide-typescale-body-small-height";
    internal const string IdeTypescaleBodySmallSpacing = "--flare-ide-typescale-body-small-spacing";
    internal const string IdeTypescaleLabelSmallWeight = "--flare-ide-typescale-label-small-weight";


    /// <summary>The complete Visual Studio 2026 design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = FluentUI2Tokens.Design with
    {
        FocusRing = "1px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Spacing = Spacing,
        Scrollbar = Scrollbar,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Badge = Badge,
        Alert = Alert,
        EmptyState = EmptyState,
        Button = Button,
        ButtonGroup = FluentUI2Tokens.Design.ButtonGroup,
        SplitButton = SplitButton,
        ToggleButton = ToggleButton,
        Fab = Fab,
        Menu = Menu,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        Slider = Slider,
        Switch = Switch,
        Card = Card,
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Tree = Tree,
        Dialog = FluentUI2Tokens.Design.Dialog with { Radius = "var(--flare-shape-large)" },
        Popover = FluentUI2Tokens.Design.Popover with { Radius = "var(--flare-shape-large)", Shadow = "var(--flare-elevation-3)" },
        Snackbar = FluentUI2Tokens.Design.Snackbar with { Radius = "var(--flare-shape-small)" },
        // The drawer toggle keeps the burger this theme drew before the Fluent record took the Hamburger's size and shape.
        Layout = FluentUI2Tokens.Design.Layout with { AppBarToggleSize = "36px", AppBarToggleGlyphWidth = "18px", AppBarToggleGlyphHeight = "14px", AppBarToggleBarThickness = "2px", AppBarToggleRadius = "var(--flare-shape-full)" },
        // The Fluent record darkens the pressed grip through a Fluent-only variable this theme does not declare.
        Splitter = FluentUI2Tokens.Design.Splitter with { GripPressedColor = "var(--flare-splitter-grip-color)" },
        Extended = Extended,
    };

    // Visual Studio 2026 light scheme: neutral gray shell surfaces + the VS blue accent.
    internal static readonly ColorScheme LightColors = new()
    {
        Primary = "#005FB8",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#CFE4FB",
        OnPrimaryContainer = "#003A70",
        Secondary = "#5A5A5A",
        OnSecondary = "#FFFFFF",
        SecondaryContainer = "#E9E9EC",
        OnSecondaryContainer = "#1E1E1E",
        Tertiary = "#68217A",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#F0E0F4",
        OnTertiaryContainer = "#3B0B47",
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
        Info = "#005FB8",
        OnInfo = "#FFFFFF",
        InfoContainer = "#D6E8FB",
        OnInfoContainer = "#003A70",
        // Editor surface = white; shell surfaces layer up in neutral gray.
        Surface = "#FFFFFF",
        OnSurface = "#1E1E1E",
        SurfaceVariant = "#EEEEF2",
        OnSurfaceVariant = "#6E6E6E",
        OnSurfaceVariant2 = "#9C9DA4",
        SurfaceContainerLowest = "#FFFFFF",
        SurfaceContainerLow = "#F5F5F7",
        SurfaceContainer = "#EEEEF2",
        SurfaceContainerHigh = "#E4E4E8",
        SurfaceContainerHighest = "#DADADE",
        SurfaceBright = "#FFFFFF",
        SurfaceDim = "#DADADE",
        PrimaryFixed = "#CFE4FB",
        PrimaryFixedDim = "#3794FF",
        OnPrimaryFixed = "#003A70",
        OnPrimaryFixedVariant = "#003A70",
        SecondaryFixed = "#E9E9EC",
        SecondaryFixedDim = "#A8A8AC",
        OnSecondaryFixed = "#1E1E1E",
        OnSecondaryFixedVariant = "#1E1E1E",
        TertiaryFixed = "#F0E0F4",
        TertiaryFixedDim = "#C58FD6",
        OnTertiaryFixed = "#3B0B47",
        OnTertiaryFixedVariant = "#3B0B47",
        Background = "#F5F5F7",
        OnBackground = "#1E1E1E",
        Outline = "#CCCEDB",
        OutlineVariant = "#E4E4E8",
        InverseSurface = "#2D2D30",
        InverseOnSurface = "#F1F1F1",
        InversePrimary = "#9CCBF5",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.16)",
        ShadowPenumbra = "rgba(0,0,0,0.10)",
    };

    // Visual Studio 2026 dark scheme: measured chrome surfaces (#1C1C1C/#262626/#282828/#1E1E1E),
    // #3D3D3D separators, #454545 outlines and the #9184EE purple accent.
    internal static readonly ColorScheme DarkColors = new()
    {
        Primary = "#9184EE",
        OnPrimary = "#1C1C1C",
        PrimaryContainer = "#3B3168",
        OnPrimaryContainer = "#E8E2FF",
        Secondary = "#CCCCCC",
        OnSecondary = "#1C1C1C",
        SecondaryContainer = "#3D3D3D",
        OnSecondaryContainer = "#FFFFFF",
        Tertiary = "#C58FD6",
        OnTertiary = "#3B0B47",
        TertiaryContainer = "#522463",
        OnTertiaryContainer = "#F0E0F4",
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
        Info = "#9184EE",
        OnInfo = "#1C1C1C",
        InfoContainer = "#3B3168",
        OnInfoContainer = "#E8E2FF",
        // Editor #1E1E1E; tool window #282828; command bar/tab strip #262626; menu #2C2C2C;
        // status bar #141414; title bar/menu bar #1C1C1C.
        Surface = "#1E1E1E",
        OnSurface = "#FFFFFF",
        SurfaceVariant = "#282828",
        OnSurfaceVariant = "#CCCCCC",
        OnSurfaceVariant2 = "#858585",
        SurfaceContainerLowest = "#141414",
        SurfaceContainerLow = "#262626",
        SurfaceContainer = "#2C2C2C",
        SurfaceContainerHigh = "#383838",
        SurfaceContainerHighest = "#3D3D3D",
        SurfaceBright = "#383838",
        SurfaceDim = "#1E1E1E",
        PrimaryFixed = "#CFE4FB",
        PrimaryFixedDim = "#3794FF",
        OnPrimaryFixed = "#003A70",
        OnPrimaryFixedVariant = "#003A70",
        SecondaryFixed = "#E9E9EC",
        SecondaryFixedDim = "#A8A8AC",
        OnSecondaryFixed = "#1E1E1E",
        OnSecondaryFixedVariant = "#1E1E1E",
        TertiaryFixed = "#F0E0F4",
        TertiaryFixedDim = "#C58FD6",
        OnTertiaryFixed = "#3B0B47",
        OnTertiaryFixedVariant = "#3B0B47",
        Background = "#1C1C1C",
        OnBackground = "#FFFFFF",
        Outline = "#454545",
        OutlineVariant = "#3D3D3D",
        InverseSurface = "#F1F1F1",
        InverseOnSurface = "#1C1C1C",
        InversePrimary = "#005FB8",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.45)",
        ShadowPenumbra = "rgba(0,0,0,0.30)",
    };

    /// <summary>Dark-mode override of the focus ring color and chrome surfaces.</summary>
    internal static readonly IReadOnlyDictionary<string, string> DarkExtended = BuildDarkExtended();

    private static IReadOnlyDictionary<string, string> BuildDarkExtended()
    {
        return new Dictionary<string, string>(Extended)
        {
            [FluentCssVars.FocusStrokeColor] = "#FFFFFF",
            [FluentCssVars.FocusStrokeOuter] = "#000000",
            // Measured VS 2026 dark chrome. The light map above names palette roles; these concrete
            // values are what the screenshot gives, so a host can delete its old #007ACC/#2D2D30
            // overrides and the chrome still renders the measured shell.
            [VisualStudioCssVars.ChromeTitleBar] = "#1C1C1C",
            [VisualStudioCssVars.ChromeCommandBar] = "#262626",
            [VisualStudioCssVars.ChromeTabStrip] = "#262626",
            [VisualStudioCssVars.ChromeTabActive] = "#282828",
            [VisualStudioCssVars.ChromeEditor] = "#1E1E1E",
            [VisualStudioCssVars.ChromeBreadcrumb] = "#2B2B2B",
            [VisualStudioCssVars.ChromeBreadcrumbEdge] = "#2E2E2E",
            [VisualStudioCssVars.ChromeToolWindow] = "#282828",
            [VisualStudioCssVars.ChromeToolTab] = "#282828",
            [VisualStudioCssVars.ChromeField] = "#353535",
            [VisualStudioCssVars.ChromeBottomPanel] = "#1C1C1C",
            [VisualStudioCssVars.ChromeBottomTabUnderline] = "#404040",
            [VisualStudioCssVars.ChromeStatusBar] = "#141414",
            [VisualStudioCssVars.ChromeBarEdge] = "#0E0E0E",
            [VisualStudioCssVars.ChromeDivider] = "#3D3D3D",
            [VisualStudioCssVars.ChromeOutline] = "#454545",
            [VisualStudioCssVars.ChromeMenu] = "#2C2C2C",
            [VisualStudioCssVars.ChromeHover] = "#383838",
            [VisualStudioCssVars.ChromeMenuBarHover] = "#292929",
            [VisualStudioCssVars.ChromeAccent] = "#9184EE",
            [VisualStudioCssVars.ChromeText] = "#FFFFFF",
            [VisualStudioCssVars.ChromeTextSecondary] = "#CCCCCC",
            [VisualStudioCssVars.ChromeTextDisabled] = "#858585",
            [VisualStudioCssVars.ChromeScrollbarThumb] = "#9D9D9D",
            [VisualStudioCssVars.ChromeScrollbarTrack] = "#282828",
        };
    }

    private static TypeStyle T(string weight, string size, string height) =>
        new() { FontFamily = Font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = "0em" };
}
