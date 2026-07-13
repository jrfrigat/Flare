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
    };

    // Fluent 2 motion: the durationUltraFast..durationUltraSlow ramp and the named curves
    // (curveDecelerateMax/Min, curveAccelerateMax, curveEasyEase).
    internal static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "50ms",   // durationUltraFast
        DurationShort2 = "100ms",  // durationFaster
        DurationShort3 = "150ms",
        DurationShort4 = "200ms",
        DurationMedium1 = "150ms", // durationFast
        DurationMedium2 = "200ms", // durationNormal
        DurationLong1 = "300ms",   // durationSlow
        DurationLong2 = "500ms",   // durationUltraSlow
        EasingStandard = "cubic-bezier(0.1, 0.9, 0.2, 1)",   // curveDecelerateMax
        EasingDecelerate = "cubic-bezier(0, 0, 0, 1)",       // curveDecelerateMid
        EasingAccelerate = "cubic-bezier(0.9, 0.1, 1, 0.2)", // curveAccelerateMax
        EasingEmphasized = "cubic-bezier(0.33, 0, 0.67, 1)", // curveEasyEase
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
    };

    internal static readonly BadgeTokens Badge = new()
    {
        // Fluent UI 2: square-ish badge with extra-small radius (not full pill)
        Radius = "var(--flare-shape-extra-small)",
        MinWidth = "1rem",
        Height = "1rem",
        DotSize = "0.375rem",
        PaddingX = "0.25rem",
        Offset = "0.375rem",
        DotOffset = "0",
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
        ContainerRadius = "var(--flare-shape-full)",
        TextPaddingInline = "0.75rem",
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

        // Double focus-ring behavior (inner + outer) and flat shadows
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color, #000000)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer, #FFFFFF)",
        FilledHoverShadow = "none",

        // Compact Fluent icons (no MD3 gigantism at L/XL)
        IconSizeXs = "1rem",     // 16px
        IconSizeSm = "1rem",     // 16px
        IconSizeMd = "1.25rem",  // 20px
        IconSizeLg = "1.25rem",  // 20px
        IconSizeXl = "1.5rem",   // 24px

        // Label typography: compact Fluent scale (text barely grows)
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.TitleMedium,
        LabelXl = Typography.TitleLarge,
    };

    // Connected button group (Fluent's segmented look): touching segments, 1px overlap collapsing the
    // shared border, small rounded ends, flat interior corners.
    internal static readonly ButtonGroupTokens ButtonGroup = new()
    {
        Gap = "0",
        Overlap = "-1px",
        OuterRadius = "var(--flare-shape-small)",
        InnerRadius = "0",
        ZActive = "1",
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "1px", // Thin 1px Microsoft seam between the two parts

        // Fluent: fixed-width trigger 24dp (not square), no inline padding needed
        TriggerWidth = "1.5rem", // 24dp

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
    internal static readonly ToggleButtonTokens ToggleButton = new()
    {
        HeightXs = "1.75rem",
        HeightXl = "3.5rem",
        PaddingXs = "0.5rem",
        PaddingXl = "2rem",
        HeightSm = "2rem",
        HeightMd = "2.5rem",
        HeightLg = "3rem",
        PaddingSm = "0.75rem",
        PaddingMd = "1rem",
        PaddingLg = "1.5rem",
        Gap = "0.375rem",
        // Fluent segmented control: subtle rounded rectangle, and the selected state changes colour
        // only (no MD3 Expressive pill->squircle shape morph, so RadiusSelected* == Radius).
        Radius = "var(--flare-shape-small)",
        RadiusSelectedSm = "var(--flare-shape-small)",
        RadiusSelectedMd = "var(--flare-shape-small)",
        RadiusSelectedLg = "var(--flare-shape-small)",
        RestBg = "var(--flare-color-surface-container-highest)",
        RestColor = "var(--flare-color-on-surface-variant)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedColor = "var(--flare-color-on-secondary-container)",
        GroupBorder = "1px solid var(--flare-color-outline)",
        GroupRadius = "var(--flare-shape-small)",
        GroupRadiusVertical = "var(--flare-shape-small)",
        GroupDivider = "var(--flare-color-outline)",
    };

    // FAB: flatter Fluent rounding (4-8dp).
    internal static readonly FabTokens Fab = new()
    {
        RadiusSm = "var(--flare-shape-small)",
        RadiusMd = "var(--flare-shape-medium)",
        RadiusLg = "var(--flare-shape-large)",
        PaddingSm = "0.5rem",
        PaddingMd = "1rem",
        PaddingLg = "1.75rem",
        Gap = "0.75rem",
        Shadow = "var(--flare-elevation-3)",
        HoverShadow = "var(--flare-elevation-4)",
        AnchorOffset = "1.5rem",
    };

    // Menu: neutral baseline geometry, but the panel appears via fade/slide without scale (Fluent motion).
    // All members are now set explicitly, since the core record carries no default values.
    internal static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        PanelMinWidth = "10rem",
        EnterAnimation = "flare-menu-in-fade",
        PanelRadius = "var(--flare-popover-radius, var(--flare-shape-small))",
        PanelBg = "var(--flare-color-surface-container)",
        PanelShadow = "var(--flare-elevation-2)",
        PanelPaddingBlock = "0.25rem",
        PanelPaddingInline = "0.25rem",
        ItemHeight = "0",
        ItemPaddingBlock = "0.625rem",
        ItemPaddingInline = "1rem",
        ItemGap = "0.75rem",
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
        ItemFocusRingColor = "var(--flare-color-secondary)",
        ItemFocusRingThickness = "3px",
        ItemFocusRingOffset = "-3px",
    };

    // Checkbox - Fluent: 1px border, 4dp corner, no MD3 halo, double focus ring.
    internal static readonly CheckboxTokens Checkbox = new()
    {
        Size = "1.125rem",
        BorderWidth = "1px",
        Radius = "var(--flare-shape-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer)",
    };

    // Radio - Fluent: no MD3 state-layer halo.
    internal static readonly RadioTokens Radio = new()
    {
        Size = "1.25rem",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
    };

    // Chip - Fluent: the same values (8dp/32dp).
    internal static readonly ChipTokens Chip = new()
    {
        Radius = "var(--flare-shape-small)",
        Height = "2rem",
    };
    internal static readonly TabsTokens Tabs = new()
    {
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        LabelFont = "var(--flare-typescale-label-large-font)",
        LabelSize = "var(--flare-typescale-label-large-size)",
        LabelWeight = "var(--flare-typescale-label-large-weight)",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "3px",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-on-surface)",
        DividerColor = "var(--flare-color-surface-variant)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedFg = "var(--flare-color-on-secondary-container)",
        FilledBg = "var(--flare-color-primary)",
        FilledFg = "var(--flare-color-on-primary)",
        TrackBg = "transparent",
        PillRadius = "var(--flare-shape-full)",
    };

    // On-this-page - Fluent: rail style (vertical line + brand bar on the left of the active item),
    // without the MD3 pill.
    internal static readonly Flare.Abstractions.Tokens.Components.TableOfContentsTokens TableOfContents = new()
    {
        ActiveWeight = "600",
        HoverBgOpacity = "40%",
        LineHeight = "1.4",
        TitleTracking = "0.05em",
        TitleWeight = "600",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        TitleColor = "var(--flare-color-on-surface)",
        RailColor = "var(--flare-color-outline-variant)",
        RailWidth = "1px",
        ActiveBg = "transparent",
        ActiveRadius = "0",
        MarkerWidth = "3px",
        // Gap between the rail/marker and the link text.
        LinkPadX = "0.625rem",
        Indent = "0.75rem",
    };

    // Slider - Fluent: thin 4px rail, round white thumb (20px) with a 2px brand ring,
    // active = brand, inactive = neutralStroke; without the MD3 Expressive bar/notch.
    // Geometry is set via plain constant tokens (no dependency on size).
    internal static readonly SliderTokens Slider = new()
    {
        TrackHeight = "4px",
        TrackRadius = "var(--flare-shape-full, 9999px)",
        GapRadius = "0px",
        Gap = "0px",
        HandleHeight = "20px",
        HandleWidth = "20px",
        HandlePressedWidth = "20px",
        HandleRadius = "var(--flare-shape-full, 9999px)",
        HandleBorderWidth = "2px",                   // brand ring (color = --_active)
        HandleFill = "var(--flare-color-surface)",   // white thumb
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-outline-variant)",
        StateLayerSize = "28px",
        StateHoverOpacity = "0.06",
        StatePressedOpacity = "0.08",
        StopColor = "var(--flare-color-outline)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "2px",
        HandleClipPath = "initial",
        ValueBg = "var(--flare-color-inverse-surface)",
        ValueColor = "var(--flare-color-inverse-on-surface)",
    };

    // Input - Fluent outlined style (full 1px border, neutral focus box + 2px brand bottom accent).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface)",
        OutlinedBorder = "1px solid var(--flare-color-outline)",
        OutlinedRadius = "var(--flare-shape-small)",
        FilledBorderBottom = "1px solid var(--flare-color-outline)",
        FocusBorder = "1px solid var(--flare-color-outline)",
        FocusBorderBottom = "2px solid var(--flare-color-primary)",
        // Fluent focus = a 2px brand accent under the field, as a layout-neutral inset shadow (no jump).
        FocusRing = "inset 0 -2px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        HoverBorderBottom = "var(--flare-input-border-bottom, 1px solid var(--flare-color-on-surface-variant))",
        HoverStateLayer = "none",
        Padding = "0.75rem 1rem",
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
    };

    // Progress - Fluent: thin 2px rail, squared corners, no stop dot, 3px butt-cap ring. Wavy is left
    // off (WavyEnabled stays at the default 0) so FlareProgress renders a plain bar/ring.
    internal static readonly ProgressTokens Progress = new()
    {
        LinearHeight = "2px",
        TrackRadius = "var(--flare-shape-extra-small)",
        Gap = "0px",
        StopSize = "0px",
        StopInset = "0px",
        StopColor = "var(--fc-main, var(--flare-color-primary))",
        BufferOpacity = "30%",
        CircularSize = "40px",
        CircularWidth = "3px",
        CircularCap = "butt",
        CircularGap = "0px",
        WavyEnabled = "0",
        WavyHeight = "10px",
        WaveLength = "40px",
        WaveAmplitude = "3px",
        WaveSpeed = "1s",
        RingWaves = "8",
        RingWaveAmplitude = "1.6",
    };

    // Nav - no pill in Fluent; a left accent bar marks the active item.
    internal static readonly NavTokens Nav = new()
    {
        ActiveWeight = "600",
        BadgeWeight = "600",
        RailLabelLineHeight = "1.15",
        ItemRadius = "var(--flare-shape-extra-small)",
        IndicatorRadius = "0",
        ActiveIndicator = "none",
        ActiveLeftBar = "3px solid var(--flare-color-primary)",
    };

    // Switch - Fluent's compact track + thin focus offset map to the typed record; the rest of the
    // Fluent switch visual (white thumb sizing/offsets, double focus ring) has no base-token home and
    // stays in Extended below.
    internal static readonly SwitchTokens Switch = new()
    {
        TrackWidth = "2.5rem",
        TrackHeight = "1.25rem",
        TrackOffBg = "var(--flare-color-surface-container-highest)",
        TrackOnBg = "var(--fc-main, var(--flare-color-primary))",
        TrackBorder = "2px solid var(--flare-color-outline)",
        TrackHoverBorderColor = "var(--flare-color-outline)",
        ThumbOffSize = "1rem",
        ThumbOnSize = "1.5rem",
        ThumbPressedOffSize = "1.75rem",
        ThumbPressedOnSize = "1.75rem",
        ThumbOffLeft = "0.25rem",
        ThumbOnLeft = "calc(100% - 1.75rem)",
        ThumbOffColor = "var(--flare-color-outline)",
        ThumbOnColor = "var(--flare-color-on-primary)",
        ThumbStateOffColor = "var(--flare-color-on-surface-variant)",
        ThumbStateOnColor = "var(--flare-color-primary-container)",
        IconSize = "1rem",
        IconOffColor = "var(--flare-color-surface-container-highest)",
        IconOnColor = "var(--flare-color-primary)",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "1px",
        FocusShadow = "none",
        TrackHoverOffBg = "var(--flare-switch-track-off-bg)",
        TrackHoverOnBg = "var(--flare-switch-track-on-bg)",
        HoverShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        HoverShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        DisabledTrackBg = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DisabledTrackBorder = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DisabledHandleBg = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
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

        // Switch - Fluent UI 2: 1px border, white thumb, hover track fill, double focus ring.
        ["--flare-switch-track-border"] = "1px solid var(--flare-color-secondary)",
        ["--flare-switch-track-off-bg"] = "transparent",
        ["--flare-switch-thumb-off-size"] = "0.625rem",
        ["--flare-switch-thumb-on-size"] = "0.875rem",
        // Fluent does not resize the handle on press -> keep the normal sizes.
        ["--flare-switch-thumb-pressed-off-size"] = "0.625rem",
        ["--flare-switch-thumb-pressed-on-size"] = "0.875rem",
        ["--flare-switch-thumb-off-left"] = "0.1875rem",
        ["--flare-switch-thumb-on-left"] = "calc(100% - 1.0625rem)",
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

    // Rating: same neutral baseline (Size defers to the size classes; filled = primary via --fc-main).
    internal static readonly RatingTokens Rating = new()
    {
        Size = "initial",
        EmptyColor = "var(--flare-color-outline-variant)",
        FilledColor = "var(--flare-color-primary)",
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
        TodayBg = "var(--flare-color-primary)",
        TodayColor = "var(--flare-color-on-primary)",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, var(--flare-color-surface))",
        OtherMonthOpacity = "0.3",
    };

    // Tree: same neutral baseline (24dp indent/handle, 20dp icons, primary-tinted selection).
    internal static readonly TreeTokens Tree = new()
    {
        ToggleHoverBg = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DropInsideBg = "color-mix(in srgb, var(--flare-color-primary) 12%, transparent)",
        Indent = "var(--flare-spacing-12)",
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        SelectedColor = "var(--flare-color-primary)",
        DropIndicatorColor = "var(--flare-color-primary)",
    };

    // Stepper: same neutral baseline (32dp circle, 2px connector, primary when complete).
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

    // Timeline: same neutral baseline (20dp surface dot, 2px accent ring, 2px outline-variant line).
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

    // Pagination: same neutral baseline (Size defers to the size classes; primary active fill).
    internal static readonly PaginationTokens Pagination = new()
    {
        Size = "initial",
        Radius = "var(--flare-shape-extra-small)",
        BorderColor = "var(--flare-color-outline-variant)",
        ActiveColor = "var(--flare-color-primary)",
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
        RecordDividerWidth = "2px",
        AggregateDividerWidth = "2px",
        FilterGroupRail = "3px",
        ActiveCellOutline = "2px solid var(--flare-color-primary)",
        ColumnPickerMinWidth = "160px",
        RowSelectedHoverPct = "18%",
        RowEditingPct = "6%",
        LoadingVeilPct = "55%",
        LoadingDim = "0.6",
    };

    // Spacing scale pinned to the Fluent UI 2 spacing ramp (spacingHorizontal*/spacingVertical*),
    // documented per Fluent token so it stays correct even if the shared default scale changes:
    //   None 0 | XXS 2 | XS 4 | SNudge 6 | S 8 | MNudge 10 | M 12 | L 16 | XL 20 | XXL 24 | XXXL 32 (px).
    // S24/S32 (48/64px) extend past Fluent's named ramp for large layout gaps.
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

    // BottomNav / ColorPicker - Fluent uses the same neutral baseline as Material for these.
    internal static readonly BottomNavTokens BottomNav = new()
    {
        BarHeight = "5rem",
        BarBg = "var(--flare-color-surface-container)",
        BorderColor = "var(--flare-color-surface-variant)",
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
    };

    internal static readonly ColorPickerTokens ColorPicker = new()
    {
        CheckerColor = "var(--flare-color-outline-variant)",
        ThumbBg = "var(--flare-color-surface)",
        ThumbBorderColor = "var(--flare-color-outline)",
    };

    // Tooltip / Avatar - Fluent uses the same neutral baseline as Material.
    internal static readonly TooltipTokens Tooltip = new()
    {
        MaxWidth = "18rem",
        Offset = "0.5rem",
    };

    internal static readonly PopoverTokens Popover = new()
    {
        Radius = "var(--flare-shape-small)",
    };

    internal static readonly AvatarTokens Avatar = new()
    {
        GroupSpacing = "-0.75rem",
        GroupBorderWidth = "2px",
        GroupBorderColor = "var(--flare-color-surface)",
        OverflowBg = "var(--flare-color-surface-container-highest)",
        OverflowColor = "var(--flare-color-on-surface-variant)",
    };

    // Drawer / Snackbar - Fluent uses the Material baseline geometry (only the Snackbar radius differs).
    internal static readonly DrawerTokens Drawer = new()
    {
        Width = "360px",
        MiniWidth = "72px",
    };

    internal static readonly SnackbarTokens Snackbar = new()
    {
        Radius = "var(--flare-shape-small)",
        MinHeight = "3rem",
        PaddingBlock = "0.875rem",
        ProviderInset = "1.5rem",
        CloseOpacity = "0.75",
    };

    // Dialog - Fluent: large radius; the rest is the shared Material baseline.
    internal static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-large)",
        IconSize = "1.5rem",
    };

    internal static readonly AppBarTokens AppBar = new()
    {
        Gap = "0.25rem",
        Height = "4rem",
        HeightDense = "3rem",
        PaddingX = "0.25rem",
        TitlePaddingX = "0.75rem",
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

    internal static readonly DropzoneTokens Dropzone = new()
    {
        BorderWidth = "2px",
        HoverBg = "color-mix(in srgb, var(--flare-color-primary) 5%, var(--flare-color-surface))",
        DraggingBg = "color-mix(in srgb, var(--flare-color-primary) 10%, var(--flare-color-surface))",
        DraggingRingWidth = "2px",
        IconSize = "2.5rem",
    };

    internal static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    internal static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        ContentPadding = "1.5rem 2rem",
        ContentPaddingMobile = "1rem",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "260px",
    };

    internal static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
    };

    internal static readonly OtpTokens Otp = new()
    {
        BorderWidth = "2px",
        CellHeight = "3rem",
        CellWidth = "2.75rem",
        FocusRingWidth = "2px",
        FontSize = "1.25rem",
        FontWeight = "600",
    };

    internal static readonly PickerTokens Picker = new()
    {
        OutsideOpacity = "0.4",
        DisabledOpacity = "0.3",
        WeekNumberOpacity = "0.7",
    };

    internal static readonly ScrimTokens Scrim = new()
    {
        Opacity = "0.32",
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

    internal static readonly TableTokens Table = new()
    {
        CellPaddingH = "1rem",
        CellPaddingV = "0.75rem",
        StripeOpacity = "4%",
    };

    internal static readonly TimePickerTokens TimePicker = new()
    {
        ColumnsSepSize = "1.5rem",
        DisplaySize = "2.75rem",
        HeadlineTracking = "0.05em",
        PanelRadius = "var(--flare-shape-extra-large)",
        TimeSepSize = "2.5rem",
    };

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete Fluent UI 2 design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = new()
    {
        AppBar = AppBar,
        Breadcrumb = Breadcrumb,
        DateTimePicker = DateTimePicker,
        Dropzone = Dropzone,
        Form = Form,
        Layout = Layout,
        Link = Link,
        Otp = Otp,
        Picker = Picker,
        Scrim = Scrim,
        ScrollTop = ScrollTop,
        Skeleton = Skeleton,
        Table = Table,
        TimePicker = TimePicker,
        Dialog = Dialog,
        Drawer = Drawer,
        Snackbar = Snackbar,
        BottomNav = BottomNav,
        ColorPicker = ColorPicker,
        Tooltip = Tooltip,
        Avatar = Avatar,
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Spacing = Spacing,
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
            FilledBg = "var(--flare-color-surface)",
            OutlinedBg = "var(--flare-color-surface)",
            OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
            TonalBg = "var(--flare-color-surface-container-high)",
            TonalColor = "var(--flare-color-on-surface)",
            Radius = "var(--flare-shape-medium)",
            PaddingTop = "12px",
            PaddingRight = "12px",
            PaddingBottom = "12px",
            PaddingLeft = "12px",
            // Neutral baseline members (formerly the record defaults), carried explicitly.
            FilledBorder = "none",
            TextColor = "var(--flare-color-on-surface)",
            SelectedBorder = "2px solid var(--flare-color-primary)",
            SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
            StateLayer = "var(--flare-color-on-surface)",
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
            TransitionDuration = "var(--flare-motion-duration-short2)",
            TransitionEasing = "var(--flare-motion-easing-standard)",
        },
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Switch = Switch,
        DataGrid = DataGrid,
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
        SurfaceContainerLow = "#F5F5F5",
        SurfaceContainerHigh = "#E0E0E0",
        SurfaceContainerHighest = "#D6D6D6",
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
        SurfaceContainerLow = "#222222",
        SurfaceContainerHigh = "#333333",
        SurfaceContainerHighest = "#3D3D3D",
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
        };
        return dict;
    }

    private static TypeStyle T(string font, string weight, string size, string height, string spacing) =>
        new() { FontFamily = font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = spacing };
}
