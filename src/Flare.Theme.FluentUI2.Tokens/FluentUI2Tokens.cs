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
        FocusOpacity = "0.1",
        PressedOpacity = "0.12", // F2: pressed - самый тёмный (тёмнее hover 0.10)
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
        // Fluent UI 2: small radius, border visible by default
        Radius = "var(--flare-shape-small)",
        BorderWidth = "1px",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    internal static readonly ButtonTokens Button = new()
    {
        // Компактные зазоры (Gap) между текстом и иконкой Microsoft
        GapXs = "0.125rem",        // 2px
        GapSm = "0.25rem",         // 4px
        GapMd = "0.375rem",        // 6px (Стандарт по гайдлайнам)
        GapLg = "0.5rem",          // 8px
        GapXl = "0.5rem",          // 8px

        // Высоты контейнеров по спецификации Fluent UI 2
        HeightXs = "1.25rem",   // 20px
        HeightSm = "1.5rem",     // 24px
        HeightMd = "2rem",      // 32px (Стандартная кнопка)
        HeightLg = "2.5rem",     // 40px
        HeightXl = "3rem",      // 48px

        // Строгие боковые паддинги (внутренние отступы)
        PaddingInlineXs = "0.375rem", // 6px
        PaddingInlineSm = "0.5rem",    // 8px
        PaddingInlineMd = "0.75rem",   // 12px
        PaddingInlineLg = "1rem",      // 16px
        PaddingInlineXl = "1.25rem",  // 20px

        // Поугловые радиусы пропорционально габаритам (строгие углы Windows 11)
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-extra-small)"), // 2px
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),       // 4px
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-small)"),       // 4px
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),      // 6px
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-large)"),       // 8px

        // Поведение двойного фокусного кольца (внутреннее + внешнее) и плоские тени
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color, #000000)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer, #FFFFFF)",
        FilledHoverShadow = "none",

        // Компактные иконки Fluent (без MD3-гигантизма на L/XL)
        IconSizeXs = "1rem",     // 16px
        IconSizeSm = "1rem",     // 16px
        IconSizeMd = "1.25rem",  // 20px
        IconSizeLg = "1.25rem",  // 20px
        IconSizeXl = "1.5rem",   // 24px

        // Типографика метки: компактная шкала Fluent (текст почти не растёт)
        LabelXs = Typography.LabelLarge,
        LabelSm = Typography.LabelLarge,
        LabelMd = Typography.LabelLarge,
        LabelLg = Typography.TitleMedium,
        LabelXl = Typography.TitleLarge,
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "1px", // Тонкий стыковой шов Microsoft в 1px

        // Fluent: триггер фиксированной ширины 24dp (не квадрат), боковые паддинги не нужны
        TriggerWidth = "1.5rem", // 24dp
        TriggerPaddingXs = "0px",
        TriggerPaddingSm = "0px",
        TriggerPaddingMd = "0px",
        TriggerPaddingLg = "0px",
        TriggerPaddingXl = "0px",

        // Fluent: шеврон фиксированный 12dp на всех размерах
        CaretSizeXs = "0.75rem",
        CaretSizeSm = "0.75rem",
        CaretSizeMd = "0.75rem",
        CaretSizeLg = "0.75rem",
        CaretSizeXl = "0.75rem",

        // Main: внешние ЛЕВЫЕ края = радиус Button (проброс), внутренние стыки - острый 0 (Fluent)
        MainRadiusXs = new() { TopLeft = "var(--flare-btn-radius-xs-top-left)", BottomLeft = "var(--flare-btn-radius-xs-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusSm = new() { TopLeft = "var(--flare-btn-radius-sm-top-left)", BottomLeft = "var(--flare-btn-radius-sm-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusMd = new() { TopLeft = "var(--flare-btn-radius-md-top-left)", BottomLeft = "var(--flare-btn-radius-md-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusLg = new() { TopLeft = "var(--flare-btn-radius-lg-top-left)", BottomLeft = "var(--flare-btn-radius-lg-bottom-left)", TopRight = "0px", BottomRight = "0px" },
        MainRadiusXl = new() { TopLeft = "var(--flare-btn-radius-xl-top-left)", BottomLeft = "var(--flare-btn-radius-xl-bottom-left)", TopRight = "0px", BottomRight = "0px" },

        // Trigger: внутренние стыки - острый 0, внешние ПРАВЫЕ края = радиус Button (проброс)
        TriggerRadiusXs = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-xs-top-right)", BottomRight = "var(--flare-btn-radius-xs-bottom-right)" },
        TriggerRadiusSm = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-sm-top-right)", BottomRight = "var(--flare-btn-radius-sm-bottom-right)" },
        TriggerRadiusMd = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-md-top-right)", BottomRight = "var(--flare-btn-radius-md-bottom-right)" },
        TriggerRadiusLg = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-lg-top-right)", BottomRight = "var(--flare-btn-radius-lg-bottom-right)" },
        TriggerRadiusXl = new() { TopLeft = "0px", BottomLeft = "0px", TopRight = "var(--flare-btn-radius-xl-top-right)", BottomRight = "var(--flare-btn-radius-xl-bottom-right)" },
    };

    // Toggle: пока = дефолты MD3 (Fluent-специфичные размеры/форма - открытое решение в спеке).
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

    // FAB: более плоское скругление Fluent (4-8dp).
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

    // Menu: дефолты MD3, но появление панели - fade/slide без scale (Fluent motion).
    internal static readonly MenuTokens Menu = new()
    {
        EnterAnimation = "flare-menu-in-fluent",
    };

    // Checkbox - Fluent: 1px рамка, 4dp угол, без MD3 halo, двойное фокусное кольцо.
    internal static readonly CheckboxTokens Checkbox = new()
    {
        BorderWidth = "1px",
        Radius = "var(--flare-shape-small)",
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
        FocusOutline = "2px solid var(--flare-fluent-focus-stroke-color)",
        FocusOutlineOffset = "1px",
        FocusShadow = "0 0 0 5px var(--flare-fluent-focus-stroke-outer)",
    };

    // Radio - Fluent: без MD3 state-layer halo.
    internal static readonly RadioTokens Radio = new()
    {
        StateLayerHover = "transparent",
        StateLayerHoverChecked = "transparent",
    };

    // Chip - Fluent: те же значения (8dp/32dp).
    internal static readonly ChipTokens Chip = new()
    {
        Radius = "var(--flare-shape-small)",
        Height = "2rem",
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

    // On-this-page - Fluent: рейл-стиль (вертикальная линия + brand-полоса слева у активного),
    // без MD3-пилюли.
    internal static readonly Flare.Abstractions.Tokens.Components.TableOfContentsTokens TableOfContents = new()
    {
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

    // Slider - Fluent: тонкий рельс 4px, круглый белый thumb (20px) с brand-обводкой 2px,
    // active = brand, inactive = neutralStroke; без MD3 Expressive-планки/выреза.
    // Геометрия задаётся обычными токенами-константами (без зависимости от размера).
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
    };

    // Input - Fluent outlined style (full 1px border, neutral focus box + 2px brand bottom accent).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface)",
        OutlinedRadius = "var(--flare-shape-small)",
        OutlinedBorder = "1px solid var(--flare-color-outline)",
        FilledBorderBottom = "1px solid var(--flare-color-outline)",
        FocusBorder = "1px solid var(--flare-color-outline)",
        FocusBorderBottom = "2px solid var(--flare-color-primary)",
    };

    // Progress - Fluent: thin 2px rail, squared corners, no stop dot, 3px butt-cap ring. Wavy is left
    // off (WavyEnabled stays at the default 0) so FlareProgress renders a plain bar/ring.
    internal static readonly ProgressTokens Progress = new()
    {
        TrackRadius = "var(--flare-shape-extra-small)",
        LinearHeight = "2px",
        Gap = "0px",
        StopSize = "0px",
        CircularWidth = "3px",
        CircularCap = "butt",
        CircularGap = "0px",
    };

    // Nav - no pill in Fluent; a left accent bar marks the active item.
    internal static readonly NavTokens Nav = new()
    {
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
        FocusOutlineOffset = "1px",
    };

    /// <summary>
    /// Theme-specific extras with no typed home: Fluent stroke/focus tokens and the Fluent-specific
    /// switch visual the base SwitchTokens does not model.
    /// </summary>
    public static readonly Dictionary<string, string> Extended = new()
    {
        ["--flare-fluent-stroke-width-thin"] = "1px",
        ["--flare-fluent-stroke-width-thick"] = "2px",
        ["--flare-fluent-focus-stroke-width"] = "2px",
        ["--flare-fluent-focus-stroke-color"] = "#000000",
        ["--flare-fluent-focus-stroke-outer"] = "#FFFFFF",

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
        Radius = "var(--flare-shape-small)",
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

    // Drawer / Snackbar - Fluent uses the Material baseline geometry (only the Snackbar radius differs).
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
        Radius = "var(--flare-shape-small)",
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

    // ----- v2 composition: one DesignTokens (mode-agnostic) + per-mode ColorScheme -----

    /// <summary>The complete Fluent UI 2 design tokens. Use this as the base for custom themes.</summary>
    public static readonly DesignTokens Design = new()
    {
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
        },
        Input = Input,
        Progress = Progress,
        Nav = Nav,
        Switch = Switch,
        Dialog = new() { Radius = "var(--flare-shape-large)" },
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
            ["--flare-fluent-focus-stroke-color"] = "#FFFFFF",
            ["--flare-fluent-focus-stroke-outer"] = "#000000",
            ["--flare-switch-track-hover-off-bg"] = "var(--flare-color-surface-container)",
            ["--flare-switch-track-hover-on-bg"] = "#2886D4",
        };
        return dict;
    }

    private static TypeStyle T(string font, string weight, string size, string height, string spacing) =>
        new() { FontFamily = font, FontWeight = weight, FontSize = size, LineHeight = height, LetterSpacing = spacing };
}
