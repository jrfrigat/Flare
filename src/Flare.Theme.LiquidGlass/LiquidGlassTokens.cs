using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Theme.LiquidGlass;

/// <summary>
/// Design tokens for the "Liquid Glass" theme - Apple's "liquid glass" look: translucent tinted
/// surfaces, the SF Pro type stack, a #007AFF accent, generous continuous corner radii (capsule
/// buttons), the green iOS switch and soft diffuse shadows. The glass material (tints, specular
/// sheen, lensing rim) lives in the scoped component CSS (wwwroot/css), driven by these tokens.
/// It deliberately avoids backdrop-filter for performance.
/// </summary>
internal class LiquidGlassTokens
{
    private const string Font = "-apple-system, BlinkMacSystemFont, 'SF Pro Text', 'SF Pro Display', 'Helvetica Neue', Arial, sans-serif";

    internal static readonly TypographyTokens Typography = new()
    {
        DisplayLarge = T("700", "2.5rem", "3rem"),
        DisplayMedium = T("700", "2.125rem", "2.5rem"),
        DisplaySmall = T("700", "1.75rem", "2.125rem"),
        HeadlineLarge = T("700", "1.375rem", "1.75rem"),
        HeadlineMedium = T("600", "1.25rem", "1.625rem"),
        HeadlineSmall = T("600", "1.0625rem", "1.375rem"),
        TitleLarge = T("600", "1rem", "1.3125rem"),
        TitleMedium = T("600", "0.9375rem", "1.25rem"),
        TitleSmall = T("600", "0.8125rem", "1.0625rem"),
        BodyLarge = T("400", "0.9375rem", "1.3125rem"),
        BodyMedium = T("400", "0.875rem", "1.1875rem"),
        BodySmall = T("400", "0.8125rem", "1.0625rem"),
        LabelLarge = T("600", "0.9375rem", "1.25rem"),
        LabelMedium = T("600", "0.8125rem", "1.0625rem"),
        LabelSmall = T("500", "0.75rem", "1rem"),
    };

    // Continuous, generously rounded corners - the iOS look.
    internal static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "6px",
        Small = "10px",
        Medium = "14px",
        Large = "20px",
        ExtraLarge = "28px",
        Full = "9999px",
    };

    // Smooth iOS easing with a touch of spring on emphasized motion.
    internal static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "100ms",
        DurationShort2 = "150ms",
        DurationMedium1 = "250ms",
        DurationMedium2 = "350ms",
        DurationLong1 = "450ms",
        DurationLong2 = "600ms",
        EasingStandard = "cubic-bezier(0.25, 0.1, 0.25, 1)",
        EasingDecelerate = "cubic-bezier(0.16, 1, 0.3, 1)",
        EasingAccelerate = "cubic-bezier(0.4, 0, 1, 1)",
        EasingEmphasized = "cubic-bezier(0.34, 1.4, 0.5, 1)",
    };

    internal static readonly StateTokens State = new()
    {
        HoverOpacity = "0.06",
        FocusOpacity = "0.10",
        PressedOpacity = "0.12",
        DraggedOpacity = "0.10",
        DisabledOpacity = "0.35",
        DisabledContainerOpacity = "0.10",
    };

    internal static readonly BadgeTokens Badge = new()
    {
        Radius = "var(--flare-shape-full)",
        MinWidth = "1.125rem",
        Height = "1.125rem",
        DotSize = "0.5rem",
        PaddingX = "0.3125rem",
        Offset = "0.25rem",
        DotOffset = "0",
    };

    internal static readonly AlertTokens Alert = new()
    {
        Radius = "var(--flare-shape-large)",
        BorderWidth = "0px",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        GapXs = "0.25rem",
        GapSm = "0.3125rem",
        GapMd = "0.4375rem",
        GapLg = "0.5rem",
        GapXl = "0.5rem",

        // iOS 44pt touch target around Md.
        HeightXs = "1.75rem",
        HeightSm = "2.25rem",
        HeightMd = "2.75rem",   // 44px
        HeightLg = "3.25rem",
        HeightXl = "3.75rem",

        PaddingInlineXs = "0.75rem",
        PaddingInlineSm = "1rem",
        PaddingInlineMd = "1.25rem",
        PaddingInlineLg = "1.5rem",
        PaddingInlineXl = "1.75rem",

        // Capsule buttons (the iOS prominent-action shape).
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-full)"),

        FocusOutline = "none",
        FocusOutlineOffset = "0px",
        FocusShadow = "0 0 0 4px var(--flare-liquid-glow, rgba(0,122,255,0.35))",
        FilledHoverShadow = "none",

        IconSizeXs = "1rem",
        IconSizeSm = "1.125rem",
        IconSizeMd = "1.25rem",
        IconSizeLg = "1.375rem",
        IconSizeXl = "1.5rem",

        LabelXs = Typography.LabelMedium,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.TitleLarge,
        LabelXl = Typography.HeadlineSmall,
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "2px",
        TriggerWidth = "2rem",
        TriggerPaddingXs = "0px",
        TriggerPaddingSm = "0px",
        TriggerPaddingMd = "0px",
        TriggerPaddingLg = "0px",
        TriggerPaddingXl = "0px",
        CaretSizeXs = "0.875rem",
        CaretSizeSm = "0.875rem",
        CaretSizeMd = "1rem",
        CaretSizeLg = "1rem",
        CaretSizeXl = "1rem",
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

    // FAB - fully round / pill.
    internal static readonly FabTokens Fab = MaterialDesignTokens.Design.Fab with
    {
        RadiusSm = "var(--flare-shape-large)",
        RadiusMd = "var(--flare-shape-large)",
        RadiusLg = "var(--flare-shape-extra-large)",
    };

    internal static readonly MenuTokens Menu = new();

    internal static readonly CheckboxTokens Checkbox = MaterialDesignTokens.Design.Checkbox with
    {
        BorderWidth = "1.5px",
        Radius = "var(--flare-shape-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
    };

    internal static readonly RadioTokens Radio = new()
    {
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
    };

    internal static readonly ChipTokens Chip = new() { Radius = "var(--flare-shape-small)", Height = "2rem" };
    internal static readonly TabsTokens Tabs = MaterialDesignTokens.Design.Tabs;

    // The iconic green iOS switch: gray off track, green on track, white circular thumb.
    internal static readonly SwitchTokens Switch = new()
    {
        TrackWidth = "51px",
        TrackHeight = "31px",
        TrackRadius = "var(--flare-shape-full)",
        TrackColor = "var(--flare-color-surface-container-highest)",
        TrackBorderColor = "transparent",
        TrackBorderWidth = "0px",
        TrackColorSelected = "var(--flare-color-success)",
        TrackBorderColorSelected = "var(--flare-color-success)",
        ThumbSize = "27px",
        ThumbColor = "#FFFFFF",
        ThumbColorSelected = "#FFFFFF",
        ThumbShadow = "0 1px 3px rgba(0,0,0,0.25), 0 2px 8px rgba(0,0,0,0.15)",
        FocusOutlineColor = "var(--flare-color-primary)",
    };

    internal static readonly SliderTokens Slider = new()
    {
        TrackHeight = "4px",
        TrackRadius = "var(--flare-shape-full, 9999px)",
        GapRadius = "0px",
        Gap = "0px",
        HandleHeight = "28px",
        HandleWidth = "28px",
        HandlePressedWidth = "28px",
        HandleRadius = "var(--flare-shape-full, 9999px)",
        HandleBorderWidth = "0px",
        HandleFill = "#FFFFFF",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-outline-variant)",
        StateLayerSize = "36px",
        StateHoverOpacity = "0.05",
        StatePressedOpacity = "0.07",
        StopColor = "var(--flare-color-outline)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "2px",
    };

    // Soft, diffuse iOS shadows; color comes from the active ColorScheme shadow vars.
    internal static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0 2px 8px var(--flare-shadow-umbra), 0 1px 2px var(--flare-shadow-penumbra)",
        Level2 = "0 6px 18px var(--flare-shadow-umbra), 0 2px 4px var(--flare-shadow-penumbra)",
        Level3 = "0 12px 32px var(--flare-shadow-umbra), 0 4px 8px var(--flare-shadow-penumbra)",
        Level4 = "0 20px 48px var(--flare-shadow-umbra), 0 6px 12px var(--flare-shadow-penumbra)",
        Level5 = "0 32px 72px var(--flare-shadow-umbra), 0 8px 16px var(--flare-shadow-penumbra)",
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

    /// <summary>Theme-specific extras: translucent fills + blur hooks consumed by the scoped glass CSS.</summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        // Accent focus glow shared by the scoped CSS (the Liquid Glass "material").
        ["--flare-liquid-glow"] = "rgba(0,122,255,0.35)",
        // Lightly translucent fills (no backdrop blur, for performance) - opaque
        // enough to stay clean over the soft backdrop, tinted enough to read as glass.
        ["--flare-liquid-tint"] = "rgba(255,255,255,0.74)",
        ["--flare-liquid-tint-strong"] = "rgba(255,255,255,0.88)",
        // Content surfaces (menus, dropdowns, dialogs, drawers, popovers): nearly opaque
        // so text stays readable - the colourful backdrop no longer bleeds through. The
        // glass illusion is preserved by the rim, edge and float shadow around the surface.
        ["--flare-liquid-content-tint"] = "rgba(255,255,255,0.96)",
        // Gentler sheen for content surfaces so the top row of text isn't washed out.
        ["--flare-liquid-content-sheen"] = "rgba(255,255,255,0.22)",
        // Lensing rim: a bright light edge bent around the glass (top + bottom + hairline ring).
        ["--flare-liquid-rim"] = "rgba(255,255,255,0.75)",
        ["--flare-liquid-rim-low"] = "rgba(255,255,255,0.30)",
        ["--flare-liquid-edge"] = "rgba(255,255,255,0.50)",
        // Specular sheen layered over the top of every glass surface.
        ["--flare-liquid-sheen"] = "rgba(255,255,255,0.55)",
        // Soft floating drop shadow.
        ["--flare-liquid-shadow"] = "0 8px 30px rgba(0,0,0,0.12)",

        // Card + input backgrounds are translucent glass tints that differ light/dark (see the
        // DarkExtended overrides), so they stay mode-specific here rather than on the mode-agnostic
        // typed Card/Input records. Geometry (radius/border) moves to the typed records below.
        ["--flare-card-elevated-bg"] = "rgba(255,255,255,0.80)",
        ["--flare-card-filled-bg"] = "rgba(255,255,255,0.80)",
        ["--flare-card-outlined-bg"] = "rgba(255,255,255,0.70)",
        ["--flare-card-outlined-border"] = "1px solid rgba(255,255,255,0.55)",
        ["--flare-card-tonal-bg"] = "rgba(255,255,255,0.70)",
        ["--flare-card-elevation"] = "0 8px 30px rgba(0,0,0,0.10)",
        // Input = iOS translucent gray fill (the borderless/rounded geometry is on InputTokens).
        ["--flare-input-bg"] = "rgba(120,120,128,0.12)",
    };

    // Input = iOS translucent field: borderless, rounded; blue glow on focus (scoped CSS). The
    // translucent fill itself is mode-specific and stays in Extended (--flare-input-bg).
    internal static readonly InputTokens Input = new()
    {
        OutlinedRadius = "var(--flare-shape-medium)",
        OutlinedBorder = "1px solid transparent",
        FilledBorderBottom = "1px solid transparent",
        FocusBorder = "1px solid var(--flare-color-primary)",
        FocusBorderBottom = "1px solid var(--flare-color-primary)",
    };

    // Progress - rounded thin bar.
    internal static readonly ProgressTokens Progress = new()
    {
        TrackRadius = "var(--flare-shape-full)",
        LinearHeight = "0.375rem",
        Gap = "0px",
        StopSize = "0px",
    };

    // Nav - pill indicator (iOS tab/segmented look).
    internal static readonly NavTokens Nav = new()
    {
        ItemRadius = "var(--flare-shape-medium)",
        IndicatorRadius = "var(--flare-shape-full)",
    };

    /// <summary>The complete Liquid Glass design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = MaterialDesignTokens.Design with
    {
        FocusRing = "0 0 0 4px var(--flare-liquid-glow, rgba(0,122,255,0.35))",
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
        Switch = Switch,
        Slider = Slider,
        // Card geometry is typed; the translucent variant fills stay mode-specific in Extended.
        Card = new() { Radius = "var(--flare-shape-large)", PaddingTop = "16px", PaddingRight = "16px", PaddingBottom = "16px", PaddingLeft = "16px" },
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Dialog = new() { Radius = "var(--flare-shape-extra-large)" },
        Popover = new() { Radius = "var(--flare-shape-large)" },
        Snackbar = new() { Radius = "var(--flare-shape-large)" },
        Extended = Extended,
    };

    // iOS light system colors over a soft tinted backdrop.
    internal static readonly ColorScheme LightColors = new()
    {
        Primary = "#007AFF",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#D6E9FF",
        OnPrimaryContainer = "#003E80",
        Secondary = "#8E8E93",
        OnSecondary = "#FFFFFF",
        SecondaryContainer = "#E9E9EB",
        OnSecondaryContainer = "#1C1C1E",
        Tertiary = "#AF52DE",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#F2E4FB",
        OnTertiaryContainer = "#4A1A6B",
        Error = "#FF3B30",
        OnError = "#FFFFFF",
        ErrorContainer = "#FFE2E0",
        OnErrorContainer = "#7A0B04",
        Success = "#34C759",
        OnSuccess = "#FFFFFF",
        SuccessContainer = "#DCF6E3",
        OnSuccessContainer = "#0A3D1B",
        Warning = "#FF9500",
        OnWarning = "#FFFFFF",
        WarningContainer = "#FFEBCC",
        OnWarningContainer = "#5C3600",
        Info = "#5AC8FA",
        OnInfo = "#00344A",
        InfoContainer = "#D4F0FB",
        OnInfoContainer = "#00344A",
        Surface = "#FFFFFF",
        OnSurface = "#1C1C1E",
        SurfaceVariant = "#F2F2F7",
        OnSurfaceVariant = "#6C6C70",
        SurfaceContainerLow = "#FBFBFD",
        SurfaceContainer = "#F2F2F7",
        SurfaceContainerHigh = "#E5E5EA",
        SurfaceContainerHighest = "#D1D1D6",
        Background = "#EEF1F6",
        OnBackground = "#1C1C1E",
        Outline = "#C6C6C8",
        OutlineVariant = "#E5E5EA",
        InverseSurface = "#2C2C2E",
        InverseOnSurface = "#F2F2F7",
        InversePrimary = "#9FCBFF",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.12)",
        ShadowPenumbra = "rgba(0,0,0,0.08)",
    };

    // iOS dark - near-black backdrop with frosted dark-gray glass.
    internal static readonly ColorScheme DarkColors = new()
    {
        Primary = "#0A84FF",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#0A3866",
        OnPrimaryContainer = "#D6E9FF",
        Secondary = "#98989D",
        OnSecondary = "#1C1C1E",
        SecondaryContainer = "#2C2C2E",
        OnSecondaryContainer = "#F2F2F7",
        Tertiary = "#BF5AF2",
        OnTertiary = "#2A0A40",
        TertiaryContainer = "#43205F",
        OnTertiaryContainer = "#F2E4FB",
        Error = "#FF453A",
        OnError = "#4A0700",
        ErrorContainer = "#6E0E06",
        OnErrorContainer = "#FFE2E0",
        Success = "#30D158",
        OnSuccess = "#06330F",
        SuccessContainer = "#0C5121",
        OnSuccessContainer = "#DCF6E3",
        Warning = "#FF9F0A",
        OnWarning = "#3D2600",
        WarningContainer = "#5C3600",
        OnWarningContainer = "#FFEBCC",
        Info = "#64D2FF",
        OnInfo = "#00344A",
        InfoContainer = "#00496B",
        OnInfoContainer = "#D4F0FB",
        Surface = "#1C1C1E",
        OnSurface = "#FFFFFF",
        SurfaceVariant = "#2C2C2E",
        OnSurfaceVariant = "#AEAEB2",
        SurfaceContainerLow = "#161618",
        SurfaceContainer = "#1C1C1E",
        SurfaceContainerHigh = "#2C2C2E",
        SurfaceContainerHighest = "#3A3A3C",
        Background = "#000000",
        OnBackground = "#FFFFFF",
        Outline = "#48484A",
        OutlineVariant = "#38383A",
        InverseSurface = "#F2F2F7",
        InverseOnSurface = "#1C1C1E",
        InversePrimary = "#007AFF",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.50)",
        ShadowPenumbra = "rgba(0,0,0,0.35)",
    };

    /// <summary>Dark-mode overrides: dark frosted fills + glow.</summary>
    internal static readonly IReadOnlyDictionary<string, string> DarkExtended = BuildDarkExtended();

    private static IReadOnlyDictionary<string, string> BuildDarkExtended()
    {
        return new Dictionary<string, string>(Extended)
        {
            ["--flare-liquid-glow"] = "rgba(10,132,255,0.40)",
            // Dark frosted glass: darker tint, dimmer rim/sheen, deeper shadow.
            ["--flare-liquid-tint"] = "rgba(44,44,46,0.74)",
            ["--flare-liquid-tint-strong"] = "rgba(58,58,60,0.88)",
            // Nearly opaque dark frosted glass for text-bearing surfaces (dark bleed is
            // worse for contrast, so push opacity high); gentle sheen so labels stay crisp.
            ["--flare-liquid-content-tint"] = "rgba(40,40,42,0.97)",
            ["--flare-liquid-content-sheen"] = "rgba(255,255,255,0.07)",
            ["--flare-liquid-rim"] = "rgba(255,255,255,0.28)",
            ["--flare-liquid-rim-low"] = "rgba(255,255,255,0.10)",
            ["--flare-liquid-edge"] = "rgba(255,255,255,0.14)",
            ["--flare-liquid-sheen"] = "rgba(255,255,255,0.16)",
            ["--flare-liquid-shadow"] = "0 8px 30px rgba(0,0,0,0.45)",
            ["--flare-card-elevated-bg"] = "rgba(44,44,46,0.78)",
            ["--flare-card-filled-bg"] = "rgba(44,44,46,0.78)",
            ["--flare-card-outlined-bg"] = "rgba(44,44,46,0.70)",
            ["--flare-card-outlined-border"] = "1px solid rgba(255,255,255,0.12)",
            ["--flare-card-tonal-bg"] = "rgba(58,58,60,0.78)",
            ["--flare-card-elevation"] = "0 8px 30px rgba(0,0,0,0.45)",
            ["--flare-input-bg"] = "rgba(118,118,128,0.24)",
        };
    }

    private static TypeStyle T(string weight, string size, string height) =>
        new() { FontFamily = Font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = "0em" };
}
