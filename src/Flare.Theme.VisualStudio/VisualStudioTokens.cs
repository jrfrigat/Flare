using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;

namespace Flare.Theme.VisualStudio;

/// <summary>
/// Design tokens for the "Visual Studio 2026" theme - the modern VS shell look: neutral gray
/// surfaces layered editor/tool-window/tab-strip, Segoe UI type, tight 4px geometry, the VS blue
/// accent and flat shadows. The signature document-tab gap and active-tab selection (top accent
/// line over the editor surface) live in the theme-scoped CSS (wwwroot/css/components/tabs.css),
/// driven by these tokens.
/// </summary>
internal class VisualStudioTokens
{
    private const string Font = "'Segoe UI', 'Segoe UI Variable', -apple-system, system-ui, 'Helvetica Neue', Arial, sans-serif";

    internal static readonly TypographyTokens Typography = new()
    {
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
    };

    // Snappy IDE transitions.
    internal static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "40ms",
        DurationShort2 = "80ms",
        DurationMedium1 = "120ms",
        DurationMedium2 = "180ms",
        DurationLong1 = "260ms",
        DurationLong2 = "360ms",
        EasingStandard = "cubic-bezier(0.4, 0, 0.2, 1)",
        EasingDecelerate = "cubic-bezier(0, 0, 0, 1)",
        EasingAccelerate = "cubic-bezier(0.4, 0, 1, 1)",
        EasingEmphasized = "cubic-bezier(0.4, 0, 0.2, 1)",
    };

    internal static readonly StateTokens State = new()
    {
        HoverOpacity = "0.08",
        FocusOpacity = "0.10",
        PressedOpacity = "0.14",
        DraggedOpacity = "0.10",
        DisabledOpacity = "0.4",
        DisabledContainerOpacity = "0.12",
    };

    internal static readonly BadgeTokens Badge = new()
    {
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
        Radius = "var(--flare-shape-small)",
        BorderWidth = "1px",
        Padding = "0.625rem 0.875rem",
        Gap = "0.625rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        GapXs = "0.1875rem",
        GapSm = "0.25rem",
        GapMd = "0.375rem",
        GapLg = "0.5rem",
        GapXl = "0.5rem",

        // Compact VS command/toolbar button heights.
        HeightXs = "1.25rem",
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
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-extra-small)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-large)"),

        FocusOutline = "1px solid var(--flare-color-primary)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 2px var(--flare-vs-focus, rgba(0,120,212,0.4))",
        FilledHoverShadow = "none",

        IconSizeXs = "0.875rem",
        IconSizeSm = "1rem",
        IconSizeMd = "1rem",
        IconSizeLg = "1.125rem",
        IconSizeXl = "1.25rem",

        LabelXs = Typography.LabelMedium,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.TitleMedium,
        LabelXl = Typography.TitleLarge,
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "1px",
        TriggerWidth = "1.25rem",
        TriggerPaddingXs = "0px",
        TriggerPaddingSm = "0px",
        TriggerPaddingMd = "0px",
        TriggerPaddingLg = "0px",
        TriggerPaddingXl = "0px",
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

    internal static readonly ToggleButtonTokens ToggleButton = new();

    internal static readonly FabTokens Fab = new()
    {
        RadiusSm = "var(--flare-shape-small)",
        RadiusMd = "var(--flare-shape-medium)",
        RadiusLg = "var(--flare-shape-large)",
    };

    internal static readonly MenuTokens Menu = new();

    // Checkbox/Radio - VS: 1px border, tight corner, no MD3 halo.
    internal static readonly CheckboxTokens Checkbox = new()
    {
        BorderWidth = "1px",
        Radius = "var(--flare-shape-extra-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
    };

    internal static readonly RadioTokens Radio = new()
    {
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
    };

    internal static readonly ChipTokens Chip = new();

    // VS 2026 document tabs: thin top accent line, neutral strip, the active tab fills with the
    // editor surface. The full document-tab look (gap, top accent, hover) is finished in the
    // theme-scoped tabs.css using these tokens.
    internal static readonly TabsTokens Tabs = new()
    {
        IndicatorThickness = "2px",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        DividerColor = "var(--flare-color-outline-variant)",
        SelectedBg = "var(--flare-color-surface)",
        SelectedFg = "var(--flare-color-on-surface)",
        FilledBg = "var(--flare-color-primary)",
        FilledFg = "var(--flare-color-on-primary)",
        TrackBg = "var(--flare-color-surface-container)",
        PillRadius = "var(--flare-shape-small)",
    };

    internal static readonly SliderTokens Slider = new()
    {
        TrackHeight = "4px",
        TrackRadius = "var(--flare-shape-full, 9999px)",
        GapRadius = "0px",
        Gap = "0px",
        HandleHeight = "16px",
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

    // Card - flat VS panel with a 1px border, no shadow (all variants share the flat look).
    internal static readonly CardTokens Card = new()
    {
        ElevatedBg = "var(--flare-color-surface)",
        FilledBg = "var(--flare-color-surface)",
        OutlinedBg = "var(--flare-color-surface)",
        OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
        TonalBg = "var(--flare-color-surface-container-high)",
        Radius = "var(--flare-shape-small)",
        Elevation = "none",
        PaddingTop = "8px",
        PaddingRight = "8px",
        PaddingBottom = "8px",
        PaddingLeft = "8px",
    };

    // Input - flat field with a 1px border; blue focus finished in scoped CSS.
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface)",
        OutlinedRadius = "var(--flare-shape-extra-small)",
        OutlinedBorder = "1px solid var(--flare-color-outline)",
        FilledBorderBottom = "1px solid var(--flare-color-outline)",
        FocusBorder = "1px solid var(--flare-color-primary)",
        FocusBorderBottom = "1px solid var(--flare-color-primary)",
    };

    // Progress - thin VS bar.
    internal static readonly ProgressTokens Progress = new()
    {
        TrackRadius = "var(--flare-shape-extra-small)",
        LinearHeight = "0.375rem",
        Gap = "0px",
        StopSize = "0px",
    };

    // Nav - left accent bar (VS solution-explorer style), no pill.
    internal static readonly NavTokens Nav = new()
    {
        ItemRadius = "var(--flare-shape-extra-small)",
        IndicatorRadius = "0",
        ActiveIndicator = "none",
        ActiveLeftBar = "2px solid var(--flare-color-primary)",
    };

    /// <summary>Theme-specific extras (VS chrome hooks consumed by the scoped CSS).</summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        // VS focus ring color used by the scoped button/input CSS.
        ["--flare-vs-focus"] = "rgba(0,120,212,0.45)",

        // Tabs - VS 2026 "flowing" document tabs, consumed by the theme-scoped tabs.css.
        ["--flare-vs-tab-gap"] = "2px",
        ["--flare-vs-tab-strip-bg"] = "var(--flare-color-surface-container)",
        // Rounded top corners so the strip flows around the active/hovered tab ("обтекание").
        ["--flare-vs-tab-radius"] = "var(--flare-shape-large)",
        // Active tab fill: in light it is the white editor surface; the dark override lifts it to a
        // lighter selection gray so the rounded tab still reads as "floating" above the strip.
        ["--flare-vs-tab-active-bg"] = "var(--flare-color-surface)",
    };

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete Visual Studio 2026 design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = new()
    {
        FocusRing = "1px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Spacing = Spacing,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Badge = Badge,
        Alert = Alert,
        Button = Button,
        SplitButton = SplitButton,
        ToggleButton = ToggleButton,
        Fab = Fab,
        Menu = Menu,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        Slider = Slider,
        Card = Card,
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Dialog = new() { Radius = "var(--flare-shape-large)" },
        Popover = new() { Radius = "var(--flare-shape-small)" },
        Snackbar = new() { Radius = "var(--flare-shape-small)" },
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
        SurfaceContainerLow = "#F5F5F7",
        SurfaceContainer = "#EEEEF2",
        SurfaceContainerHigh = "#E4E4E8",
        SurfaceContainerHighest = "#DADADE",
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

    // Visual Studio 2026 dark scheme: the layered #1E1E1E/#252526/#2D2D30 shell + bright blue accent.
    internal static readonly ColorScheme DarkColors = new()
    {
        Primary = "#3794FF",
        OnPrimary = "#002747",
        PrimaryContainer = "#004781",
        OnPrimaryContainer = "#CFE4FB",
        Secondary = "#A8A8AC",
        OnSecondary = "#1E1E1E",
        SecondaryContainer = "#3F3F46",
        OnSecondaryContainer = "#F1F1F1",
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
        Info = "#3794FF",
        OnInfo = "#002747",
        InfoContainer = "#004781",
        OnInfoContainer = "#CFE4FB",
        // Editor surface = #1E1E1E; tool window #252526; tab strip / titlebar #2D2D30.
        Surface = "#1E1E1E",
        OnSurface = "#F1F1F1",
        SurfaceVariant = "#2D2D30",
        OnSurfaceVariant = "#CCCCCC",
        SurfaceContainerLow = "#252526",
        SurfaceContainer = "#2D2D30",
        SurfaceContainerHigh = "#333337",
        SurfaceContainerHighest = "#3F3F46",
        Background = "#1F1F1F",
        OnBackground = "#F1F1F1",
        Outline = "#434346",
        OutlineVariant = "#3F3F46",
        InverseSurface = "#F1F1F1",
        InverseOnSurface = "#1E1E1E",
        InversePrimary = "#005FB8",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.45)",
        ShadowPenumbra = "rgba(0,0,0,0.30)",
    };

    /// <summary>Dark-mode override of the focus ring color.</summary>
    internal static readonly IReadOnlyDictionary<string, string> DarkExtended = BuildDarkExtended();

    private static IReadOnlyDictionary<string, string> BuildDarkExtended()
    {
        return new Dictionary<string, string>(Extended)
        {
            ["--flare-vs-focus"] = "rgba(55,148,255,0.5)",
            // Dark: the editor (#1E1E1E) is darker than the strip, so the active tab uses a lighter
            // selection gray to keep the rounded VS 2026 tab visibly floating above the strip.
            ["--flare-vs-tab-active-bg"] = "var(--flare-color-surface-container-highest)",
        };
    }

    private static TypeStyle T(string weight, string size, string height) =>
        new() { FontFamily = Font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = "0em" };
}
