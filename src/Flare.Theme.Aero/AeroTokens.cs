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
    };

    // Snappy, short transitions - the quick hover fades of the Aero era.
    internal static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "50ms",
        DurationShort2 = "90ms",
        DurationMedium1 = "130ms",
        DurationMedium2 = "200ms",
        DurationLong1 = "280ms",
        DurationLong2 = "400ms",
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

        FocusOutline = "1px dotted var(--flare-color-on-surface)",
        FocusOutlineOffset = "-3px",
        FocusShadow = "0 0 0 2px var(--flare-aero-glow, rgba(60,127,177,0.45))",
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

    internal static readonly ToggleButtonTokens ToggleButton = MaterialDesignTokens.Design.ToggleButton;

    internal static readonly FabTokens Fab = MaterialDesignTokens.Design.Fab with
    {
        RadiusSm = "var(--flare-shape-small)",
        RadiusMd = "var(--flare-shape-medium)",
        RadiusLg = "var(--flare-shape-large)",
    };

    internal static readonly MenuTokens Menu = MaterialDesignTokens.Design.Menu;

    // Checkbox/Radio - Aero: 1px border, gentle corner, no MD3 halo.
    internal static readonly CheckboxTokens Checkbox = MaterialDesignTokens.Design.Checkbox with
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

    internal static readonly ChipTokens Chip = new() { Radius = "var(--flare-shape-small)", Height = "2rem" };
    internal static readonly TabsTokens Tabs = MaterialDesignTokens.Design.Tabs;

    internal static readonly SliderTokens Slider = MaterialDesignTokens.Design.Slider with
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

    internal static readonly SpacingTokens Spacing = MaterialDesignTokens.Design.Spacing;

    // Input - sunken white field with a 1px border; blue focus is finished in scoped CSS.
    internal static readonly InputTokens Input = MaterialDesignTokens.Design.Input with
    {
        FilledBg = "var(--flare-color-surface)",
        OutlinedRadius = "var(--flare-shape-extra-small)",
        OutlinedBorder = "1px solid var(--flare-color-outline)",
        FilledBorderBottom = "1px solid var(--flare-color-outline)",
        FocusBorder = "1px solid var(--flare-color-primary)",
        FocusBorderBottom = "1px solid var(--flare-color-primary)",
    };

    // Progress - thin classic bar; flat (no MD3 Expressive wavy/round-cap indicator).
    internal static readonly ProgressTokens Progress = MaterialDesignTokens.Design.Progress with
    {
        TrackRadius = "var(--flare-shape-extra-small)",
        LinearHeight = "0.75rem",
        Gap = "0px",
        StopSize = "0px",
        CircularCap = "butt",
        CircularGap = "0",
        WavyEnabled = "0",
    };

    // Nav - left accent bar (Office side-nav), no pill.
    internal static readonly NavTokens Nav = new()
    {
        ItemRadius = "var(--flare-shape-extra-small)",
        IndicatorRadius = "0",
        ActiveIndicator = "none",
        ActiveLeftBar = "3px solid var(--flare-color-primary)",
    };

    /// <summary>Theme-specific extras (geometry/gloss hooks consumed by the scoped CSS).</summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        // Aero focus glow color used by the scoped button/input CSS.
        ["--flare-aero-glow"] = "rgba(60,127,177,0.55)",
    };

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete Aero design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = MaterialDesignTokens.Design with
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
        SplitButton = SplitButton,
        ToggleButton = ToggleButton,
        Fab = Fab,
        Menu = Menu,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        Slider = Slider,
        // Aero cards: flat glass panels with a 1px border, small radius, no drop shadow.
        Card = MaterialDesignTokens.Design.Card with
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
        },
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Dialog = MaterialDesignTokens.Design.Dialog with { Radius = "var(--flare-shape-large)" },
        Popover = MaterialDesignTokens.Design.Popover with { Radius = "var(--flare-shape-small)" },
        Snackbar = MaterialDesignTokens.Design.Snackbar with { Radius = "var(--flare-shape-small)" },
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
        SurfaceContainerLow = "#F4F7FB",
        SurfaceContainer = "#E9EFF5",
        SurfaceContainerHigh = "#DDE5EE",
        SurfaceContainerHighest = "#D0DAE6",
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
        SurfaceContainerLow = "#222A31",
        SurfaceContainer = "#28313A",
        SurfaceContainerHigh = "#313B45",
        SurfaceContainerHighest = "#3A4550",
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
            ["--flare-aero-glow"] = "rgba(91,155,224,0.55)",
        };
    }

    private static TypeStyle T(string weight, string size, string height) =>
        new() { FontFamily = Font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = "0em" };
}
