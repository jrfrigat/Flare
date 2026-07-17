using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Css;
using Flare.Css.Tokens;

namespace Flare.Theme.MaterialDesign3.Tokens;

/// <summary>
/// Material Design 3 baseline design-token values: the reference <see cref="DesignTokens"/> plus the
/// light/dark <c>ColorScheme</c>s. This is the shared source of truth the Material-lineage themes
/// (MD3 Expressive, MD3, MD2, and any custom Material theme) build from via <c>with</c>.
/// </summary>
public class MaterialDesignTokens
{
    internal static readonly TypographyTokens Typography = new()
    {
        DisplayLarge = T("Roboto", "400", "3.5625rem", "4rem", "-0.015625em"),
        DisplayMedium = T("Roboto", "400", "2.8125rem", "3.25rem", "0em"),
        DisplaySmall = T("Roboto", "400", "2.25rem", "2.75rem", "0em"),
        HeadlineLarge = T("Roboto", "400", "2rem", "2.5rem", "0em"),
        HeadlineMedium = T("Roboto", "400", "1.75rem", "2.25rem", "0em"),
        HeadlineSmall = T("Roboto", "400", "1.5rem", "2rem", "0em"),
        TitleLarge = T("Roboto", "400", "1.375rem", "1.75rem", "0em"),
        TitleMedium = T("Roboto", "500", "1rem", "1.5rem", "0.009375em"),
        TitleSmall = T("Roboto", "500", "0.875rem", "1.25rem", "0.00625em"),
        BodyLarge = T("Roboto", "400", "1rem", "1.5rem", "0.03125em"),
        BodyMedium = T("Roboto", "400", "0.875rem", "1.25rem", "0.015625em"),
        BodySmall = T("Roboto", "400", "0.75rem", "1rem", "0.025em"),
        LabelLarge = T("Roboto", "500", "0.875rem", "1.25rem", "0.00625em"),
        LabelMedium = T("Roboto", "500", "0.75rem", "1rem", "0.03125em"),
        LabelSmall = T("Roboto", "500", "0.6875rem", "1rem", "0.03125em"),
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

    internal static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "50ms",
        DurationShort2 = "100ms",
        DurationShort3 = "150ms",
        DurationShort4 = "200ms",
        DurationMedium1 = "200ms",
        DurationMedium2 = "300ms",
        DurationLong1 = "450ms",
        DurationLong2 = "600ms",
        EasingStandard = "cubic-bezier(0.2, 0, 0, 1)",
        EasingDecelerate = "cubic-bezier(0, 0, 0, 1)",
        EasingAccelerate = "cubic-bezier(0.3, 0, 1, 1)",
        EasingEmphasized = "cubic-bezier(0.2, 0, 0, 1)",
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
    };

    internal static readonly BadgeTokens Badge = new()
    {
        // MD3 Expressive: pill shape (full radius), compact sizing
        Radius = "var(--flare-shape-full)",
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
        // MD3 Expressive: extra-small radius, no border on filled variant
        Radius = "var(--flare-shape-extra-small)",
        BorderWidth = "0",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        ContainerRadius = "var(--flare-shape-full)",
        TextPaddingInline = "0.75rem",
        // 5 gap sizes (Gap) between text and icon
        // XS/S - height matches the MD3 spec -> gap = spec (8dp);
        // M=8 matches; L/XL - adapted to the reduced height.
        GapXs = "0.5rem",  // 8dp (spec)
        GapSm = "0.5rem",  // 8dp (spec)
        GapMd = "0.5rem",  // 8dp (= spec)
        GapLg = "0.5rem",  // adapted (spec 12dp at height 96)
        GapXl = "0.75rem", // adapted (spec 16dp at height 136)

        // 5 container heights
        HeightXs = "2rem",       // 32dp
        HeightSm = "2.5rem",      // 40dp
        HeightMd = "3rem",       // 48dp
        HeightLg = "3.5rem",      // 56dp
        HeightXl = "4rem",       // 64dp

        // 5 inline paddings
        PaddingInlineXs = "0.75rem",
        PaddingInlineSm = "1rem",
        PaddingInlineMd = "1.5rem",
        PaddingInlineLg = "2rem",
        PaddingInlineXl = "2.5rem",

        // Per-corner radii: a fully rounded capsule at all 5 sizes, so we reference
        // the Shape.Full scale (like the other components) rather than a hardcoded half-height rem.
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-full)"),

        // Focus outline and shadow settings
        FocusOutline = "3px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FilledHoverShadow = "var(--flare-elevation-1)",

        // Icon size per the MD3 Expressive spec
        IconSizeXs = "1.25rem", // 20dp
        IconSizeSm = "1.25rem", // 20dp
        IconSizeMd = "1.5rem",  // 24dp
        IconSizeLg = "2rem",    // 32dp
        IconSizeXl = "2.5rem",  // 40dp

        // Label typography: label-large -> title-medium -> headline-small -> headline-large
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.TitleMedium,
        LabelLg = Typography.HeadlineSmall,
        LabelXl = Typography.HeadlineLarge,
    };

    // CONNECTED button group: no gap, a 1px negative overlap that collapses adjacent borders into one
    // seam, rounded group ends, flat interior corners. MD3 Expressive overrides this to a separated look.
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
        Gap = "0.125rem", // 2dp (between space)

        // Trigger is square: width = Button height (forwarded), no inline padding needed
        TriggerWidth = "var(--_flare-btn-height, var(--flare-btn-height-md, 3rem))",

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
        Radius = "var(--flare-shape-full)",
        RadiusSelectedSm = "var(--flare-shape-medium)",
        RadiusSelectedMd = "var(--flare-shape-medium)",
        RadiusSelectedLg = "1rem",
        RestBg = "var(--flare-color-surface-container-highest)",
        RestColor = "var(--flare-color-on-surface-variant)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedColor = "var(--flare-color-on-secondary-container)",
        GroupBorder = "1px solid var(--flare-color-outline)",
        GroupRadius = "var(--flare-shape-full)",
        GroupRadiusVertical = "var(--flare-shape-medium)",
        GroupDivider = "var(--flare-color-outline)",
    };

    // FAB: padding-based sizing, large/medium/extra-large rounding.
    internal static readonly FabTokens Fab = new()
    {
        PaddingSm = "0.5rem",
        PaddingMd = "1rem",
        PaddingLg = "1.75rem",
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
        Size = "1.125rem",
        BorderWidth = "2px",
        Radius = "2px",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
    };
    internal static readonly RadioTokens Radio = new()
    {
        Size = "1.25rem",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
    };
    internal static readonly ChipTokens Chip = new()
    {
        Radius = "var(--flare-shape-small)",  // MD3 = 8dp
        Height = "2rem",                       // MD3 = 32dp
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

    // Menu (MD3 Expressive "Menus"): container 16dp (shape-large), elevation 3,
    // item 4dp (end items 12dp), label-large text, vertical 8dp, gap 2dp, focus-ring secondary.
    internal static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        PanelRadius = "var(--flare-shape-large)",     // 16dp
        PanelMinWidth = "7rem",                       // 112dp
        PanelShadow = "var(--flare-elevation-3)",     // elevation 3
        PanelPaddingInline = "0.125rem",              // group padding 2dp
        PanelPaddingBlock = "0.125rem",               // 2dp
        ItemHeight = "3rem",                          // item height 48dp (MD3 list-item)
        ItemPaddingBlock = "0.5rem",                  // top/bottom 8dp
        ItemGapBetween = "0.125rem",                  // gap between items 2dp
        ItemRadius = "var(--flare-shape-extra-small)",// 4dp
        ItemRadiusEnd = "var(--flare-shape-extra-small)", // 4dp
        GroupRadius = "var(--flare-shape-small)",     // group 8dp
        GroupPadding = "0.125rem",                    // group padding 2dp
        // Expressive "island" group sections: each group is a separate rounded surface tone with its
        // own elevation, on a transparent backing panel, so adjacent sections read as two cards.
        GroupBg = "var(--flare-color-surface-container-high)",
        GroupGap = "0.5rem",
        GroupShadow = "var(--flare-elevation-3)",
        GroupedPanelBg = "transparent",
        GroupedPanelShadow = "none",
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
        ItemIconSize = "1.25rem",
        ItemFocusRingColor = "var(--flare-color-secondary)",
        ItemFocusRingThickness = "3px",
        ItemFocusRingOffset = "-3px",
    };

    // Input - filled style (md.comp.filled-text-field): surface-container-highest container, 56dp
    // height (1rem block padding + 24px line), 1dp on-surface-variant active indicator, hover ->
    // on-surface indicator + 8% state layer, focus -> 3dp primary (Expressive).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface-container-highest)",
        OutlinedBorder = "none",
        OutlinedRadius = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0",
        FilledBorderBottom = "1px solid var(--flare-color-on-surface-variant)",
        // Focus = a 2px primary active indicator drawn as a layout-neutral inset shadow (no jump).
        FocusRing = "inset 0 -3px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        HoverBorderBottom = "1px solid var(--flare-color-on-surface)",
        HoverStateLayer = "linear-gradient(color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent), color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent))",
        Padding = "1rem 1rem",
        IconSize = "1.5rem",                          // leading/trailing icon 24dp
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
    };

    // Progress - MD3 Expressive: rounded full track, 4px thick, trailing stop-indicator dot, round
    // circular caps, and the opt-in wavy determinate track (with-wave 10dp, amplitude 3dp, wavelength 40dp).
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
        TrackRadius = "var(--flare-shape-full)",
        Gap = "4px",
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
        CircularWidthLg = "4px",  // spec: same 4dp on the thick ring
        CircularWidthXl = "5px",
        CircularCap = "round",
        CircularGap = "4px",
        WavyEnabled = "1",
        WavyHeight = "10px",
        WaveLength = "40px",
        WaveAmplitude = "3px",
        WaveSpeed = "1s",
        RingWaves = "8",
        RingWaveAmplitude = "1.6",
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
        BadgeWeight = "600",
        RailLabelLineHeight = "1.15",
        ItemRadius = "var(--flare-shape-extra-small)",
        IndicatorRadius = "var(--flare-shape-full)",
        ActiveIndicator = "var(--flare-color-secondary-container)",
        ActiveLeftBar = "none",
    };

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

    internal static readonly TableOfContentsTokens TableOfContents = new()
    {
        ActiveWeight = "600",
        HoverBgOpacity = "40%",
        LineHeight = "1.4",
        TitleTracking = "0.05em",
        TitleWeight = "600",
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
    };

    internal static readonly PopoverTokens Popover = new()
    {
        Radius = "var(--flare-shape-medium)",
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
    };

    internal static readonly SnackbarTokens Snackbar = new()
    {
        Radius = "var(--flare-shape-extra-small)",
        MinHeight = "3rem",
        PaddingBlock = "0.875rem",
        ProviderInset = "1.5rem",
        CloseOpacity = "0.75",
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
        HandleHeightXs = "2.75rem",
        HandleHeightSm = "2.75rem",
        HandleHeightMd = "3.25rem",
        HandleHeightLg = "4.25rem",
        HandleHeightXl = "6.75rem",
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
        StateHoverOpacity = "0.08",
        StatePressedOpacity = "0.10",
        StopColor = "var(--flare-color-on-secondary-container)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "4px",
        ValueBg = "var(--flare-color-inverse-surface)",
        ValueColor = "var(--flare-color-inverse-on-surface)",
    };

    internal static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-extra-large)",
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
        OtherMonthOpacity = "0.3",
    };

    // Tree: 24dp indent per level, 24dp expander/handle, 20dp icons, primary-tinted selection.
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
    };

    // Switch (MD3 baseline geometry: 52x32 track, 24 thumb, elevation-1 lift). Carried explicitly
    // now that the core record ships no defaults.
    internal static readonly SwitchTokens Switch = new()
    {
        TrackWidth = "52px",
        TrackHeight = "32px",
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
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
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

    /// <summary>The complete MD3 Expressive design tokens. Use this as the base for custom themes.</summary>
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
        Spacing = Spacing,
        Switch = Switch,
        DataGrid = DataGrid,
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
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        // MD3 cards: elevated hover = level 3 (spec), outlined border = full outline role (spec),
        // 16px inner padding on raw-content cards. Variant bg roles come from the base CardTokens.
        Card = new()
        {
            ElevationHover = "var(--flare-elevation-3)",
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
        SurfaceContainerLow = "#F7F2FA",
        SurfaceContainerHigh = "#ECE6F0",
        SurfaceContainerHighest = "#E6E0E9",
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
        SurfaceContainerLow = "#1D1B20",
        SurfaceContainerHigh = "#2B2930",
        SurfaceContainerHighest = "#36343B",
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
