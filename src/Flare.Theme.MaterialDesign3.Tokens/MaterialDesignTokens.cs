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

        // Поугловые радиусы: полностью скруглённая капсула на всех 5 размерах, поэтому ссылаемся
        // на шкалу Shape.Full (как и остальные компоненты), а не на захардкоженный half-height rem.
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-full)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-full)"),

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

        // Триггер квадратный: ширина = высоте Button (проброс), боковые паддинги не нужны
        TriggerWidth = "var(--_flare-btn-height, var(--flare-btn-height-md, 3rem))",
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
    internal static readonly ToggleButtonTokens ToggleButton = new()
    {
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

    // FAB: padding-based габарит, скругление large/medium/extra-large.
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
        OutlinedRadius = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0",
        Padding = "1rem 1rem",
        OutlinedBorder = "none",
        FilledBorderBottom = "1px solid var(--flare-color-on-surface-variant)",
        HoverBorderBottom = "1px solid var(--flare-color-on-surface)",
        HoverStateLayer = "linear-gradient(color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent), color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent))",
        FocusBorder = "none",
        FocusBorderBottom = "3px solid var(--flare-color-primary)",
        // Neutral baseline members (formerly the record defaults), carried explicitly.
        FilledRadius = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0",
        FontFamily = "var(--flare-typescale-body-large-font)",
        FontSize = "var(--flare-typescale-body-large-size)",
        TextColor = "var(--flare-color-on-surface)",
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        CaretColor = "var(--flare-color-primary)",
        ErrorBorder = "var(--flare-color-error)",
        ErrorColor = "var(--flare-color-error)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        HelperFontSize = "var(--flare-typescale-body-small-size, 0.75rem)",
        HelperColor = "var(--flare-color-on-surface-variant)",
        LabelFontFamily = "var(--flare-typescale-label-medium-font, var(--flare-typescale-label-large-font))",
        LabelFontSize = "var(--flare-typescale-label-medium-size, 0.75rem)",
        LabelFontWeight = "var(--flare-typescale-label-medium-weight, 400)",
        LabelColor = "var(--flare-color-on-surface-variant)",
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
        // Neutral baseline members (formerly the record defaults), carried explicitly.
        TrackColor = "var(--flare-color-surface-container-highest)",
        IndicatorColor = "var(--flare-color-primary)",
        CircularColor = "var(--flare-color-primary)",
        CircularTrackColor = "var(--flare-color-surface-container-highest)",
        LinearHeightSm = "2px",
        LinearHeightLg = "8px",
        LinearRadius = "var(--flare-shape-full)",
        CircularSize = "40px",
        CircularSizeSm = "24px",
        CircularSizeLg = "56px",
        CircularStrokeWidth = "4px",
        CircularStrokeWidthSm = "3px",
        CircularStrokeWidthLg = "5px",
        IndeterminateDuration = "2s",
        IndeterminateEasing = "var(--flare-motion-easing-standard)",
        BufferColor = "color-mix(in srgb, var(--flare-color-primary) 32%, var(--flare-color-surface-container-highest))",
        WavyDuration = "1.5s",
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
        ActiveColor = "var(--flare-color-on-secondary-container)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        TitleColor = "var(--flare-color-on-surface-variant)",
        RailColor = "transparent",
        RailWidth = "0",
        ActiveBg = "var(--flare-color-secondary-container)",
        ActiveRadius = "var(--flare-shape-full)",
        MarkerWidth = "0",
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
        SurfaceColor = "var(--flare-color-inverse-surface)",
        TextColor = "var(--flare-color-inverse-on-surface)",
        Radius = "var(--flare-shape-extra-small)",
        Padding = "6px 8px",
        MaxWidth = "300px",
        FontFamily = "var(--flare-typescale-body-small-font)",
        FontSize = "var(--flare-typescale-body-small-size)",
        FontWeight = "var(--flare-typescale-body-small-weight, 400)",
        LineHeight = "var(--flare-typescale-body-small-height)",
        Offset = "8px",
        ArrowSize = "8px",
        TransitionDuration = "var(--flare-motion-duration-short1)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
        ShowDelay = 100,
        HideDelay = 0,
    };

    internal static readonly PopoverTokens Popover = new()
    {
        SurfaceColor = "var(--flare-color-surface-container)",
        Radius = "var(--flare-shape-medium)",
        Elevation = "var(--flare-elevation-2)",
        Padding = "8px 0",
        MinWidth = "112px",
        MaxWidth = "calc(100vw - 32px)",
        MaxHeight = "calc(100vh - 32px)",
        Offset = "4px",
        ArrowSize = "12px",
        ScrimColor = "transparent",
        TransitionDuration = "var(--flare-motion-duration-short2)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
    };

    internal static readonly AvatarTokens Avatar = new()
    {
        SurfaceColor = "var(--flare-color-primary-container)",
        TextColor = "var(--flare-color-on-primary-container)",
        IconColor = "var(--flare-color-on-primary-container)",
        RoundedRadius = "var(--flare-shape-full)",
        SquareRadius = "var(--flare-shape-small)",
        SizeXs = "24px",
        SizeSm = "32px",
        SizeMd = "40px",
        SizeLg = "48px",
        SizeXl = "64px",
        FontFamily = "var(--flare-typescale-label-large-font)",
        FontSize = "var(--flare-typescale-label-large-size)",
        FontWeight = "var(--flare-typescale-label-large-weight, 500)",
        GroupBorderWidth = "2px",
        GroupBorderColor = "var(--flare-color-surface)",
        GroupOverflowBg = "var(--flare-color-surface-container-highest)",
        GroupOverflowColor = "var(--flare-color-on-surface-variant)",
    };

    internal static readonly DrawerTokens Drawer = new()
    {
        SurfaceColor = "var(--flare-color-surface-container-low)",
        Width = "360px",
        MiniWidth = "72px",
        BreakpointSmWidth = "256px",
        BreakpointMdWidth = "256px",
        BreakpointLgWidth = "360px",
        BreakpointXlWidth = "360px",
        Elevation = "var(--flare-elevation-1)",
        Radius = "var(--flare-shape-extra-large)",
        ScrimColor = "var(--flare-color-scrim)",
        ScrimOpacity = "0.32",
        TransitionDuration = "var(--flare-motion-duration-medium2)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
        HeaderPadding = "16px",
        ContentPadding = "8px 0",
        TitleColor = "var(--flare-color-on-surface)",
        TitleFontFamily = "var(--flare-typescale-title-large-font)",
        TitleFontSize = "var(--flare-typescale-title-large-size)",
    };

    internal static readonly SnackbarTokens Snackbar = new()
    {
        SurfaceColor = "var(--flare-color-inverse-surface)",
        TextColor = "var(--flare-color-inverse-on-surface)",
        ActionColor = "var(--flare-color-inverse-primary)",
        Radius = "var(--flare-shape-extra-small)",
        MinWidth = "344px",
        MaxWidth = "560px",
        Height = "48px",
        HeightMultiLine = "68px",
        Padding = "0 16px",
        Gap = "8px",
        Elevation = "var(--flare-elevation-3)",
        FontFamily = "var(--flare-typescale-body-medium-font)",
        FontSize = "var(--flare-typescale-body-medium-size)",
        ActionFontWeight = "var(--flare-typescale-label-large-weight, 500)",
        ActionFontSize = "var(--flare-typescale-label-large-size)",
        TransitionDuration = "var(--flare-motion-duration-short2)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
        AutoHideDelay = 5000,
        BottomOffset = "16px",
        LeftOffset = "16px",
        RightOffset = "16px",
        StackGap = "8px",
    };

    internal static readonly SliderTokens Slider = new()
    {
        TrackHeight = "initial",
        TrackRadius = "initial",
        GapRadius = "initial",
        Gap = "initial",
        HandleHeight = "initial",
        HandleWidth = "initial",
        HandlePressedWidth = "initial",
        HandleRadius = "initial",
        HandleClipPath = "initial",
        HandleBorderWidth = "initial",
        HandleFill = "initial",
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
        SurfaceColor = "var(--flare-color-surface-container-high)",
        Radius = "var(--flare-shape-extra-large)",
        MaxWidth = "560px",
        MinWidth = "280px",
        Padding = "24px",
        HeaderPadding = "24px 24px 0 24px",
        ActionsPadding = "0 24px 24px 24px",
        ActionsGap = "8px",
        ScrimColor = "var(--flare-color-scrim)",
        ScrimOpacity = "0.32",
        Elevation = "var(--flare-elevation-3)",
        TitleColor = "var(--flare-color-on-surface)",
        TitleFontFamily = "var(--flare-typescale-headline-small-font)",
        TitleFontSize = "var(--flare-typescale-headline-small-size)",
        TitleFontWeight = "var(--flare-typescale-headline-small-weight)",
        ContentColor = "var(--flare-color-on-surface-variant)",
        ContentFontFamily = "var(--flare-typescale-body-medium-font)",
        ContentFontSize = "var(--flare-typescale-body-medium-size)",
        TransitionDuration = "var(--flare-motion-duration-medium2)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
        SizeXsWidth = "320px",
        SizeSmWidth = "400px",
        SizeMdWidth = "560px",
        SizeLgWidth = "720px",
        SizeXlWidth = "880px",
        SizeFullWidth = "100%",
    };

    // DataGrid baseline geometry/colors. No theme customizes the grid, so both references carry the
    // same neutral baseline explicitly now that the core record ships no defaults.
    internal static readonly DataGridTokens DataGrid = new()
    {
        SurfaceColor = "var(--flare-color-surface)",
        HeaderBg = "var(--flare-color-surface-container)",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderFontFamily = "var(--flare-typescale-label-medium-font)",
        HeaderFontSize = "var(--flare-typescale-label-medium-size)",
        HeaderFontWeight = "var(--flare-typescale-label-medium-weight, 500)",
        HeaderHeight = "56px",
        HeaderPadding = "0 16px",
        RowHeight = "52px",
        RowHeightDense = "40px",
        CellPadding = "0 16px",
        CellColor = "var(--flare-color-on-surface)",
        CellFontFamily = "var(--flare-typescale-body-medium-font)",
        CellFontSize = "var(--flare-typescale-body-medium-size)",
        SelectedRowBg = "var(--flare-color-primary-container)",
        SelectedRowColor = "var(--flare-color-on-primary-container)",
        HoverRowBg = "color-mix(in srgb, var(--flare-color-on-surface) var(--flare-state-hover-opacity, 8%), transparent)",
        SortIconColor = "var(--flare-color-on-surface-variant)",
        SortIconActiveColor = "var(--flare-color-primary)",
        BorderColor = "var(--flare-color-outline-variant)",
        BorderWidth = "1px",
        FilterRowBg = "var(--flare-color-surface-container-low)",
        GroupHeaderBg = "var(--flare-color-surface-container)",
        GroupHeaderColor = "var(--flare-color-on-surface)",
        ToolbarBg = "var(--flare-color-surface)",
        ToolbarHeight = "56px",
        ToolbarPadding = "0 16px",
        EmptyStateBg = "transparent",
        EmptyStateColor = "var(--flare-color-on-surface-variant)",
        ResizeHandleWidth = "4px",
        ResizeHandleColor = "var(--flare-color-outline)",
        ColumnPickerBg = "var(--flare-color-surface-container)",
        ColumnPickerElevation = "var(--flare-elevation-2)",
    };

    // Rating: Size = initial defers to the component size classes; empty star = outline-variant,
    // filled default = primary (the Color parameter overrides via --fc-main), hover scale 1.15.
    internal static readonly RatingTokens Rating = new()
    {
        Size = "initial",
        EmptyColor = "var(--flare-color-outline-variant)",
        FilledColor = "var(--flare-color-primary)",
        HoverScale = "1.15",
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
        Size = "initial",
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
        TrackWidthSm = "40px",
        TrackHeightSm = "24px",
        TrackWidthLg = "64px",
        TrackHeightLg = "40px",
        TrackRadius = "var(--flare-shape-full)",
        TrackColor = "var(--flare-color-surface-container-highest)",
        TrackBorderColor = "var(--flare-color-outline)",
        TrackBorderWidth = "2px",
        TrackColorSelected = "var(--flare-color-primary)",
        TrackBorderColorSelected = "var(--flare-color-primary)",
        ThumbSize = "24px",
        ThumbSizeSm = "16px",
        ThumbSizeLg = "32px",
        ThumbColor = "var(--flare-color-outline)",
        ThumbColorSelected = "var(--flare-color-on-primary)",
        ThumbIconColor = "var(--flare-color-surface-container-highest)",
        ThumbIconColorSelected = "var(--flare-color-primary)",
        ThumbShadow = "var(--flare-elevation-1)",
        FocusOutlineWidth = "2px",
        FocusOutlineColor = "var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        TransitionDuration = "var(--flare-motion-duration-short2)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
        PressedLayerColor = "var(--flare-color-primary)",
        PressedLayerOpacity = "var(--flare-state-pressed-opacity)",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
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

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete MD3 Expressive design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = new()
    {
        Spacing = Spacing,
        Switch = Switch,
        DataGrid = DataGrid,
        Rating = Rating,
        Pagination = Pagination,
        Timeline = Timeline,
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
