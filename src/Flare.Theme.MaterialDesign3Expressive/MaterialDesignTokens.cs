using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;

namespace Flare.Theme.MaterialDesign3Expressive;

internal class MaterialDesignTokens
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
        FocusOpacity = "0.10",   // MD3 state layer: focus = 10%
        PressedOpacity = "0.10", // MD3 state layer: pressed = 10%
        DraggedOpacity = "0.16",
        DisabledOpacity = "0.38",
        DisabledContainerOpacity = "0.12",
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
        // MD3 Expressive: extra-small radius, no border on filled variant
        Radius = "var(--flare-shape-extra-small)",
        BorderWidth = "0",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        // 5 Размеров зазоров (Gap) между текстом и иконкой
        // XS/S - высота совпадает со спекой MD3 -> gap = спека (8dp);
        // M=8 совпадает; L/XL - адаптировано под уменьшённую высоту.
        GapXs = "0.5rem",  // 8dp (спека)
        GapSm = "0.5rem",  // 8dp (спека)
        GapMd = "0.5rem",  // 8dp (= спека)
        GapLg = "0.5rem",  // адаптировано (спека 12dp при height 96)
        GapXl = "0.75rem", // адаптировано (спека 16dp при height 136)

        // 5 Высот контейнеров
        HeightXs = "2rem",       // 32dp
        HeightSm = "2.5rem",      // 40dp
        HeightMd = "3rem",       // 48dp
        HeightLg = "3.5rem",      // 56dp
        HeightXl = "4rem",       // 64dp

        // 5 Боковых отступов
        PaddingInlineXs = "0.75rem",
        PaddingInlineSm = "1rem",
        PaddingInlineMd = "1.5rem",
        PaddingInlineLg = "2rem",
        PaddingInlineXl = "2.5rem",

        // Поугловые радиусы (высота контейнера / 2) для создания идеальных капсул
        RadiusXs = CornerRadiusTokens.All("1rem"),
        RadiusSm = CornerRadiusTokens.All("1.25rem"),
        RadiusMd = CornerRadiusTokens.All("1.5rem"),
        RadiusLg = CornerRadiusTokens.All("1.75rem"),
        RadiusXl = CornerRadiusTokens.All("2rem"),

        // Настройки обводки фокуса и теней
        FocusOutline = "3px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FilledHoverShadow = "var(--flare-elevation-1)",

        // Размер иконки по спецификации MD3 Expressive
        IconSizeXs = "1.25rem", // 20dp
        IconSizeSm = "1.25rem", // 20dp
        IconSizeMd = "1.5rem",  // 24dp
        IconSizeLg = "2rem",    // 32dp
        IconSizeXl = "2.5rem",  // 40dp

        // Типографика метки: label-large -> title-medium -> headline-small -> headline-large
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.TitleMedium,
        LabelLg = Typography.HeadlineSmall,
        LabelXl = Typography.HeadlineLarge,
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "0.125rem", // 2dp (between space)

        // Триггер квадратный (ширина = высоте Button), боковые паддинги не нужны
        TriggerPaddingXs = "0px",
        TriggerPaddingSm = "0px",
        TriggerPaddingMd = "0px",
        TriggerPaddingLg = "0px",
        TriggerPaddingXl = "0px",

        // Иконка-стрелка = размер иконки Button того же размера (ПРОБРОС токена)
        CaretSizeXs = "var(--flare-btn-icon-size-xs)",
        CaretSizeSm = "var(--flare-btn-icon-size-sm)",
        CaretSizeMd = "var(--flare-btn-icon-size-md)",
        CaretSizeLg = "var(--flare-btn-icon-size-lg)",
        CaretSizeXl = "var(--flare-btn-icon-size-xl)",

        // Main: внешние ЛЕВЫЕ углы = радиус Button (проброс), внутренние ПРАВЫЕ = inner corner (spec 4/4/4/8/12dp)
        MainRadiusXs = new() { TopLeft = "var(--flare-btn-radius-xs-top-left)", BottomLeft = "var(--flare-btn-radius-xs-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusSm = new() { TopLeft = "var(--flare-btn-radius-sm-top-left)", BottomLeft = "var(--flare-btn-radius-sm-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusMd = new() { TopLeft = "var(--flare-btn-radius-md-top-left)", BottomLeft = "var(--flare-btn-radius-md-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusLg = new() { TopLeft = "var(--flare-btn-radius-lg-top-left)", BottomLeft = "var(--flare-btn-radius-lg-bottom-left)", TopRight = "0.5rem", BottomRight = "0.5rem" },
        MainRadiusXl = new() { TopLeft = "var(--flare-btn-radius-xl-top-left)", BottomLeft = "var(--flare-btn-radius-xl-bottom-left)", TopRight = "0.75rem", BottomRight = "0.75rem" },

        // Trigger: внутренние ЛЕВЫЕ = inner corner, внешние ПРАВЫЕ = радиус Button (проброс)
        TriggerRadiusXs = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-xs-top-right)", BottomRight = "var(--flare-btn-radius-xs-bottom-right)" },
        TriggerRadiusSm = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-sm-top-right)", BottomRight = "var(--flare-btn-radius-sm-bottom-right)" },
        TriggerRadiusMd = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-md-top-right)", BottomRight = "var(--flare-btn-radius-md-bottom-right)" },
        TriggerRadiusLg = new() { TopLeft = "0.5rem", BottomLeft = "0.5rem", TopRight = "var(--flare-btn-radius-lg-top-right)", BottomRight = "var(--flare-btn-radius-lg-bottom-right)" },
        TriggerRadiusXl = new() { TopLeft = "0.75rem", BottomLeft = "0.75rem", TopRight = "var(--flare-btn-radius-xl-top-right)", BottomRight = "var(--flare-btn-radius-xl-bottom-right)" },
    };

    // MD3: круглая форма покоя, морфинг в squircle при выборе (значения = дефолты записи).
    internal static readonly ToggleButtonTokens ToggleButton = new();

    // FAB: padding-based габарит, скругление large/medium/extra-large (значения = дефолты записи).
    internal static readonly FabTokens Fab = new();

    // Checkbox / Radio / Chip: значения MD3 = дефолты записей.
    internal static readonly CheckboxTokens Checkbox = new();
    internal static readonly RadioTokens Radio = new();
    internal static readonly ChipTokens Chip = new();
    internal static readonly TabsTokens Tabs = new();

    // Menu (MD3 Expressive "Menus"): контейнер 16dp (shape-large), elevation 3,
    // пункт 4dp (крайние 12dp), текст label-large, vertical 8dp, gap 2dp, focus-ring secondary.
    internal static readonly MenuTokens Menu = new()
    {
        PanelRadius = "var(--flare-shape-large)",     // 16dp
        PanelMinWidth = "7rem",                       // 112dp
        PanelShadow = "var(--flare-elevation-3)",     // elevation 3
        PanelPaddingInline = "0.125rem",              // group padding 2dp
        PanelPaddingBlock = "0.125rem",               // 2dp
        ItemHeight = "2.75rem",                       // высота пункта 44dp
        ItemPaddingBlock = "0.5rem",                  // top/bottom 8dp
        ItemGapBetween = "0.125rem",                  // gap между пунктами 2dp
        ItemRadius = "var(--flare-shape-extra-small)",// 4dp
        ItemRadiusEnd = "var(--flare-shape-extra-small)", // 4dp
        GroupRadius = "var(--flare-shape-small)",     // группа 8dp
        GroupPadding = "0.125rem",                    // отступ группы 2dp
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
    };

    // Input - filled style (md.comp.filled-text-field): surface-container-highest container, 56dp
    // height (1rem block padding + 24px line), 1dp on-surface-variant active indicator, hover ->
    // on-surface indicator + 8% state layer, focus -> 3dp primary (Expressive).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface-container-highest)",
        OutlinedRadius = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0",
        Padding = "1rem 1rem",
        OutlinedBorder = "none",
        FilledBorderBottom = "1px solid var(--flare-color-on-surface-variant)",
        HoverBorderBottom = "1px solid var(--flare-color-on-surface)",
        HoverStateLayer = "linear-gradient(color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent), color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent))",
        FocusBorder = "none",
        FocusBorderBottom = "3px solid var(--flare-color-primary)",
    };

    // Progress - MD3 Expressive: rounded full track, 4px thick, trailing stop-indicator dot, round
    // circular caps, and the opt-in wavy determinate track (with-wave 10dp, amplitude 3dp, wavelength 40dp).
    internal static readonly ProgressTokens Progress = new()
    {
        TrackRadius = "var(--flare-shape-full)",
        LinearHeight = "4px",
        Gap = "4px",
        StopSize = "4px",
        StopInset = "0px",
        CircularWidth = "4px",
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


    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete MD3 Expressive design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = new()
    {
        FocusRing = "3px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
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
        // MD3 cards: elevated hover = level 3 (spec), outlined border = full outline role (spec),
        // 16px inner padding on raw-content cards. Variant bg roles come from the base CardTokens.
        Card = new()
        {
            ElevationHover = "var(--flare-elevation-3)",
            OutlinedBorder = "1px solid var(--flare-color-outline)",
            PaddingTop = "16px",
            PaddingRight = "16px",
            PaddingBottom = "16px",
            PaddingLeft = "16px",
        },
        Input = Input,
        Progress = Progress,
        // Popover / dropdown panels use the medium shape (Snackbar/Dialog/Nav already match defaults).
        Popover = new() { Radius = "var(--flare-shape-medium)" },
        Extended = Extended,
    };

    internal static readonly ColorScheme LightColors = new()
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

    internal static readonly ColorScheme DarkColors = new()
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
