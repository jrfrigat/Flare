using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Theme.MaterialDesign2;

/// <summary>
/// Material Design 2 design tokens (typography, shape, motion, state, elevation geometry and
/// component tokens) plus the MD2 light/dark color schemes. Fully self-contained: MD2 predates the
/// MD3 tonal system, so it ships its own palette (purple primary / teal secondary), its own type
/// scale (H1..Overline), 4dp default shape, the canonical dp box-shadow elevation set and uppercase
/// contained buttons.
/// </summary>
internal static class MaterialDesign2Tokens
{
    // ---- Typography: the Material Design 2 type scale (Roboto) mapped onto Flare's 15 roles. ----
    internal static readonly TypographyTokens Typography = new()
    {
        // Display = H1..H3
        DisplayLarge = T("Roboto", "300", "6rem", "7rem", "-0.015625em"),       // H1 96/112 -1.5px
        DisplayMedium = T("Roboto", "300", "3.75rem", "4.5rem", "-0.008333em"), // H2 60/72 -0.5px
        DisplaySmall = T("Roboto", "400", "3rem", "3.5rem", "0em"),             // H3 48/56 0
        // Headline = H4..H6
        HeadlineLarge = T("Roboto", "400", "2.125rem", "2.625rem", "0.007353em"), // H4 34/42 0.25px
        HeadlineMedium = T("Roboto", "400", "1.5rem", "2rem", "0em"),             // H5 24/32 0
        HeadlineSmall = T("Roboto", "500", "1.25rem", "2rem", "0.0075em"),        // H6 20/32 0.15px
        // Title = Subtitle 1/2
        TitleLarge = T("Roboto", "400", "1rem", "1.75rem", "0.009375em"),   // Subtitle1 16/28 0.15px
        TitleMedium = T("Roboto", "500", "0.875rem", "1.5rem", "0.007143em"), // Subtitle2 14/24 0.1px
        TitleSmall = T("Roboto", "500", "0.875rem", "1.375rem", "0.007143em"),
        // Body = Body 1/2 + Caption
        BodyLarge = T("Roboto", "400", "1rem", "1.5rem", "0.03125em"),       // Body1 16/24 0.5px
        BodyMedium = T("Roboto", "400", "0.875rem", "1.25rem", "0.017857em"), // Body2 14/20 0.25px
        BodySmall = T("Roboto", "400", "0.75rem", "1.25rem", "0.033333em"),  // Caption 12/20 0.4px
        // Label = Button / smalls / Overline
        LabelLarge = T("Roboto", "500", "0.875rem", "1.25rem", "0.089286em"), // Button 14 1.25px (UPPERCASE via CSS)
        LabelMedium = T("Roboto", "500", "0.75rem", "1rem", "0.03125em"),
        LabelSmall = T("Roboto", "400", "0.625rem", "1rem", "0.15em"),       // Overline 10 1.5px (UPPERCASE via CSS)
    };

    // ---- Shape: MD2 uses a small, mostly-rectangular 4dp scale (no large pill containers). ----
    internal static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "4px",
        Small = "4px",
        Medium = "4px",
        Large = "8px",
        ExtraLarge = "8px",
        Full = "9999px",
    };

    // ---- Elevation: the canonical Material Design 2 dp box-shadows (fixed black alphas). ----
    internal static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0 2px 1px -1px rgba(0,0,0,0.2), 0 1px 1px 0 rgba(0,0,0,0.14), 0 1px 3px 0 rgba(0,0,0,0.12)",   // 1dp
        Level2 = "0 3px 1px -2px rgba(0,0,0,0.2), 0 2px 2px 0 rgba(0,0,0,0.14), 0 1px 5px 0 rgba(0,0,0,0.12)",   // 2dp
        Level3 = "0 2px 4px -1px rgba(0,0,0,0.2), 0 4px 5px 0 rgba(0,0,0,0.14), 0 1px 10px 0 rgba(0,0,0,0.12)",  // 4dp
        Level4 = "0 5px 5px -3px rgba(0,0,0,0.2), 0 8px 10px 1px rgba(0,0,0,0.14), 0 3px 14px 2px rgba(0,0,0,0.12)", // 8dp
        Level5 = "0 7px 8px -4px rgba(0,0,0,0.2), 0 12px 17px 2px rgba(0,0,0,0.14), 0 5px 22px 4px rgba(0,0,0,0.12)", // 12dp
    };

    // ---- Motion: the Material Design 2 standard / decelerate / accelerate / sharp curves. ----
    internal static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "100ms",
        DurationShort2 = "150ms",
        DurationShort3 = "150ms",
        DurationShort4 = "200ms",
        DurationMedium1 = "200ms",
        DurationMedium2 = "250ms",
        DurationLong1 = "300ms",
        DurationLong2 = "375ms",
        EasingStandard = "cubic-bezier(0.4, 0, 0.2, 1)",
        EasingDecelerate = "cubic-bezier(0, 0, 0.2, 1)",
        EasingAccelerate = "cubic-bezier(0.4, 0, 1, 1)",
        EasingEmphasized = "cubic-bezier(0.4, 0, 0.6, 1)", // MD2 "sharp"
    };

    // ---- State layers: Material Design 2 overlay opacities. ----
    internal static readonly StateTokens State = new()
    {
        HoverOpacity = "0.04",
        SelectedOpacity = "0.12",
        FocusOpacity = "0.12",
        PressedOpacity = "0.10",
        DraggedOpacity = "0.08",
        DisabledOpacity = "0.38",
        DisabledContainerOpacity = "0.12",
        HoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        FocusLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        PressedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-pressed-opacity) * 100%), transparent)",
        DraggedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
    };

    // ---- Components: only the tokens that differ from the defaults to read as MD2. ----

    // MD2 buttons: rectangular 4dp corners, 36dp default height, uppercase Button label.
    internal static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        ContainerRadius = "var(--flare-shape-full)",
        TextPaddingInline = "0.75rem",
        HeightXs = "1.75rem",  // 28dp
        HeightSm = "2rem",     // 32dp
        HeightMd = "2.25rem",  // 36dp (classic MD2 contained button)
        HeightLg = "2.75rem",  // 44dp
        HeightXl = "3.25rem",  // 52dp

        PaddingInlineXs = "0.5rem",
        PaddingInlineSm = "0.75rem",
        PaddingInlineMd = "1rem",
        PaddingInlineLg = "1.25rem",
        PaddingInlineXl = "1.5rem",

        RadiusXs = CornerRadiusTokens.All("4px"),
        RadiusSm = CornerRadiusTokens.All("4px"),
        RadiusMd = CornerRadiusTokens.All("4px"),
        RadiusLg = CornerRadiusTokens.All("4px"),
        RadiusXl = CornerRadiusTokens.All("4px"),

        IconSizeXs = "1.125rem", // 18dp
        IconSizeSm = "1.125rem",
        IconSizeMd = "1.125rem",
        IconSizeLg = "1.25rem",
        IconSizeXl = "1.5rem",

        GapXs = "0.5rem",
        GapSm = "0.5rem",
        GapMd = "0.5rem",
        GapLg = "0.5rem",
        GapXl = "0.5rem",

        // MD2 contained buttons sit at 2dp and rise to 8dp on hover (applied in md2-base.css too).
        FilledHoverShadow = "var(--flare-elevation-4)",
        FocusOutline = "2px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",

        // MD2 "Button" type style for every size (uppercased in CSS).
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.LabelLarge,
        LabelXl = Typography.LabelLarge,
    };

    // MD2 chips are fully rounded ("stadium") at 32dp.
    internal static readonly ChipTokens Chip = new() { Radius = "9999px", Height = "2rem" };

    // MD2 tabs: 2dp active indicator, uppercase labels (uppercased in CSS).
    internal static readonly TabsTokens Tabs = MaterialDesignTokens.Design.Tabs with { IndicatorThickness = "2px" };

    // MD2 FABs are circular; resting 6dp, pressed 12dp.
    internal static readonly FabTokens Fab = MaterialDesignTokens.Design.Fab with
    {
        RadiusSm = "9999px",
        RadiusMd = "9999px",
        RadiusLg = "9999px",
        Shadow = "var(--flare-elevation-4)",
        HoverShadow = "var(--flare-elevation-5)",
    };

    // MD2 slider: thin 4px rail with a round 20dp thumb (no MD3 Expressive bar handle).
    internal static readonly SliderTokens Slider = MaterialDesignTokens.Design.Slider with
    {
        // One fixed geometry at every size: a 4px pill rail with a 20px round thumb.
        TrackHeightXs = "4px", TrackHeightSm = "4px", TrackHeightMd = "4px",
        TrackHeightLg = "4px", TrackHeightXl = "4px",
        TrackRadiusXs = "9999px", TrackRadiusSm = "9999px", TrackRadiusMd = "9999px",
        TrackRadiusLg = "9999px", TrackRadiusXl = "9999px",
        HandleHeightXs = "20px", HandleHeightSm = "20px", HandleHeightMd = "20px",
        HandleHeightLg = "20px", HandleHeightXl = "20px",
        // "0px", not "0": the component builds the rail with calc(100% - <pct> + var(--flare-slider-gap)),
        // and inside a calc a unitless zero is a <number>, not a <length> - the whole declaration would be
        // dropped and the rail would vanish. MD2 wants no gap, but it has to say so in a length.
        Gap = "0px",
        GapRadius = "0px",
        HandleWidth = "20px",
        HandlePressedWidth = "20px",
        HandleRadius = "9999px",
        HandleBorderWidth = "0",
        HandleFill = "var(--flare-color-primary)",
        InactiveColor = "color-mix(in srgb, var(--flare-color-primary) 24%, transparent)",
        StopColor = "var(--flare-color-primary)",
        StopColorSelected = "var(--flare-color-on-primary)",
    };

    // MD2 dialogs use a 4dp corner.
    internal static readonly DialogTokens Dialog = MaterialDesignTokens.Design.Dialog with { Radius = "var(--flare-shape-extra-small)" };

    // Classic flat 4dp menu (no MD3 Expressive 16dp panel or island grouping). The default MenuTokens
    // is already flat (square items, transparent groups); MD2 only pins the panel to the 4dp shape.
    internal static readonly MenuTokens Menu = MaterialDesignTokens.Design.Menu with { PanelRadius = "var(--flare-shape-extra-small)" };

    /// <summary>The complete Material Design 2 design tokens, derived from the shared Material baseline.</summary>
    public static readonly DesignTokens Design = MaterialDesignTokens.Design with
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Button = Button,
        ButtonGroup = MaterialDesignTokens.Design.ButtonGroup,
        SplitButton = MaterialDesignTokens.Design.SplitButton,
        Fab = Fab,
        Menu = Menu,
        Chip = Chip,
        Tabs = Tabs,
        Slider = Slider,
        Dialog = Dialog,
        // No theme-specific extras: every MD2 token has a typed home (Extended defaults to empty).
    };

    // ---- Color schemes: the Material Design 2 baseline palette. ----
    internal static readonly ColorScheme LightColors = new()
    {
        Primary = "#6200EE",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#3700B3",        // MD2 primaryVariant
        OnPrimaryContainer = "#FFFFFF",
        Secondary = "#03DAC6",
        OnSecondary = "#000000",
        SecondaryContainer = "#018786",      // MD2 secondaryVariant
        OnSecondaryContainer = "#FFFFFF",
        Tertiary = "#018786",
        OnTertiary = "#FFFFFF",
        TertiaryContainer = "#B2DFDB",
        OnTertiaryContainer = "#00201E",
        Error = "#B00020",
        OnError = "#FFFFFF",
        ErrorContainer = "#FDE7E9",
        OnErrorContainer = "#5F0014",
        Success = "#2E7D32",
        OnSuccess = "#FFFFFF",
        SuccessContainer = "#C8E6C9",
        OnSuccessContainer = "#0A3D0C",
        Warning = "#ED6C02",
        OnWarning = "#FFFFFF",
        WarningContainer = "#FFE0B2",
        OnWarningContainer = "#4A2700",
        Info = "#0288D1",
        OnInfo = "#FFFFFF",
        InfoContainer = "#B3E5FC",
        OnInfoContainer = "#013654",
        Surface = "#FFFFFF",
        OnSurface = "#212121",
        SurfaceVariant = "#F5F5F5",
        OnSurfaceVariant = "#616161",
        OnSurfaceVariant2 = "#6B6B6B",
        SurfaceContainerLow = "#FAFAFA",
        SurfaceContainer = "#F5F5F5",
        SurfaceContainerHigh = "#EEEEEE",
        SurfaceContainerHighest = "#E0E0E0",
        Background = "#FAFAFA",
        OnBackground = "#212121",
        Outline = "#757575",
        OutlineVariant = "#E0E0E0",
        InverseSurface = "#323232",
        InverseOnSurface = "#FFFFFF",
        InversePrimary = "#BB86FC",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.2)",
        ShadowPenumbra = "rgba(0,0,0,0.14)",
    };

    internal static readonly ColorScheme DarkColors = new()
    {
        Primary = "#BB86FC",
        OnPrimary = "#000000",
        PrimaryContainer = "#3700B3",
        OnPrimaryContainer = "#FFFFFF",
        Secondary = "#03DAC6",
        OnSecondary = "#000000",
        SecondaryContainer = "#00504D",
        OnSecondaryContainer = "#B2FFF5",
        Tertiary = "#03DAC6",
        OnTertiary = "#000000",
        TertiaryContainer = "#00504D",
        OnTertiaryContainer = "#B2FFF5",
        Error = "#CF6679",
        OnError = "#000000",
        ErrorContainer = "#8C1D2A",
        OnErrorContainer = "#FFDAD6",
        Success = "#81C784",
        OnSuccess = "#00390A",
        SuccessContainer = "#1B5E20",
        OnSuccessContainer = "#C8E6C9",
        Warning = "#FFB74D",
        OnWarning = "#3E2500",
        WarningContainer = "#6D3B00",
        OnWarningContainer = "#FFE0B2",
        Info = "#4FC3F7",
        OnInfo = "#00344D",
        InfoContainer = "#014A6B",
        OnInfoContainer = "#B3E5FC",
        Surface = "#121212",
        OnSurface = "#E6E1E5",
        SurfaceVariant = "#2C2C2C",
        OnSurfaceVariant = "#BDBDBD",
        OnSurfaceVariant2 = "#A3A3A3",
        SurfaceContainerLow = "#1E1E1E",
        SurfaceContainer = "#242424",
        SurfaceContainerHigh = "#2C2C2C",
        SurfaceContainerHighest = "#333333",
        Background = "#121212",
        OnBackground = "#E6E1E5",
        Outline = "#8A8A8A",
        OutlineVariant = "#3A3A3A",
        InverseSurface = "#E6E1E5",
        InverseOnSurface = "#121212",
        InversePrimary = "#6200EE",
        Scrim = "#000000",
        Shadow = "#000000",
        ShadowUmbra = "rgba(0,0,0,0.2)",
        ShadowPenumbra = "rgba(0,0,0,0.14)",
    };

    private static TypeStyle T(string font, string weight, string size, string height, string spacing) =>
        new() { FontFamily = font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = spacing };
}
