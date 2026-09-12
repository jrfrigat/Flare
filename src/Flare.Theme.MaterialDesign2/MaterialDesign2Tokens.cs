using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;

namespace Flare.Theme.MaterialDesign2;

/// <summary>
/// Every Material Design 2 design token: typography, shape, motion, state, elevation and each
/// component family, plus the light and dark Material 2 color schemes.
/// <para>
/// Self-contained on purpose. This theme used to be a set of overrides on the Material 3 token
/// package, which meant everything nobody thought to override arrived as Material 3 - and an audit
/// against the published Material 2 specification found exactly that: Material 3 colors, shapes,
/// elevations and control geometry reaching a Material 2 application. Material 2 predates Material 3
/// and shares none of its opinions, so it states its own values, the way the Fluent and Material 3
/// themes state theirs.
/// </para>
/// <para>
/// A value carrying a comment with a spec reference has been checked against the generated
/// <c>docs/spec/*/md2-spec.md</c>; a value without one has not been checked yet.
/// </para>
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
        // The baseline shape scheme, verbatim: small 4dp, medium 4dp, large 0dp - a nav drawer, a
        // side sheet and a bottom sheet all meet the screen edge square. See
        // docs/spec/_foundation/md2-spec.md, "Baseline shape values".
        None = "0px",
        ExtraSmall = "4px",
        Small = "4px",
        Medium = "4px",
        Large = "0px",
        ExtraLarge = "0px",
        Full = "9999px",

        // Shape morphing arrived with Material 3 Expressive; Material 2 containers keep their corners.
        MorphDuration = "0s",
        MorphEasing = "cubic-bezier(0.4, 0, 0.2, 1)",
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

        // Springs arrived with Material 3 Expressive; Material 2 predates them, so these resolve to
        // the standard curve and keep this theme reading as its own era.
        EasingSpringFast = "cubic-bezier(0.4, 0, 0.2, 1)",
        EasingSpring = "cubic-bezier(0.4, 0, 0.2, 1)",
        EasingSpringSlow = "cubic-bezier(0.4, 0, 0.2, 1)",
        DurationSpringFast = "150ms",
        DurationSpring = "250ms",
        DurationSpringSlow = "375ms",
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
        // Focus outranks hover in this language, so the pair resolves to the focus wash.
        FocusHoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        SelectedLayer = "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-selected-opacity) * 100%), transparent)",
        SelectedHoverLayer = "color-mix(in srgb, var(--flare-color-primary) 18%, transparent)",
    };

    internal static readonly BadgeTokens Badge = new()
    {
        // MD3 Expressive: pill shape (full radius), compact sizing
        Radius = "var(--flare-shape-full)",
        // The ramp core used to hardcode for xs/sm/lg/xl, now the theme's to name. Values unchanged.
        MinWidthXs = "0.75rem",   MinWidthSm = "0.875rem",   MinWidthMd = "1rem",   MinWidthLg = "1.25rem",  MinWidthXl = "1.5rem",
        HeightXs   = "0.75rem",   HeightSm   = "0.875rem",   HeightMd   = "1rem",   HeightLg   = "1.25rem",  HeightXl   = "1.5rem",
        DotSizeXs  = "0.25rem",   DotSizeSm  = "0.3125rem",  DotSizeMd  = "0.375rem", DotSizeLg = "0.5rem",  DotSizeXl  = "0.625rem",
        PaddingXXs = "0.1875rem", PaddingXSm = "0.1875rem",  PaddingXMd = "0.25rem", PaddingXLg = "0.375rem", PaddingXXl = "0.5rem",
        LabelSizeXs = "0.5625rem",
        LabelSizeSm = "0.625rem",
        LabelSizeMd = "var(--flare-typescale-label-small-size)",
        LabelSizeLg = "var(--flare-typescale-label-medium-size)",
        LabelSizeXl = "var(--flare-typescale-label-large-size)",
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

    // MD2 buttons: rectangular 4dp corners, 36dp default height, uppercase Button label.
    internal static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        // A hairline at every size; the Expressive theme is the one that thickens it as the button grows.
        OutlineWidthXs = "1px",
        OutlineWidthSm = "1px",
        OutlineWidthMd = "1px",
        OutlineWidthLg = "1px",
        OutlineWidthXl = "1px",
        // The fallback radius for the buttons that do not take a per-size one (the confirm dialog's
        // and the message box's). A pill here made those two the only capsule buttons in a theme
        // whose every other button is a 4dp rectangle.
        ContainerRadius = "var(--flare-shape-extra-small)",
        // 8dp, not the 16dp the container variants take: the text button trades its container for
        // tighter padding (spec redline "Text button").
        TextPaddingInline = "0.5rem",
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

        // M2 keeps its one 4dp corner in every state - the shape swap on selection arrived with M3
        // Expressive and would be an anachronism here.
        SelectedRadiusXs = "4px",
        SelectedRadiusSm = "4px",
        SelectedRadiusMd = "4px",
        SelectedRadiusLg = "4px",
        SelectedRadiusXl = "4px",
        SelectedRadiusSquare = "var(--flare-shape-none)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedColor = "var(--flare-color-on-secondary-container)",
        // Selection is the accent here, whatever the variant: this language does not send each variant
        // somewhere different the way Material does, it just says "on" in the accent colour. The one
        // thing it cannot do is repeat a variant s own resting colour - a selected tonal button painted
        // secondary-container would be indistinguishable from an unselected one, which is exactly what
        // happened when these four pairs were first filled in - so every variant travels to the accent,
        // and the filled toggle starts from a neutral container rather than from the accent it ends on.
        ElevatedSelectedBg = "var(--flare-color-primary)",
        ElevatedSelectedColor = "var(--flare-color-on-primary)",
        FilledSelectedBg = "var(--flare-color-primary)",
        FilledSelectedColor = "var(--flare-color-on-primary)",
        TonalSelectedBg = "var(--flare-color-primary)",
        TonalSelectedColor = "var(--flare-color-on-primary)",
        OutlinedSelectedBg = "var(--flare-color-primary)",
        OutlinedSelectedColor = "var(--flare-color-on-primary)",
        FilledUnselectedBg = "var(--flare-color-surface-container)",
        FilledUnselectedColor = "var(--flare-color-on-surface-variant)",

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
        // Material fades the whole control, so the repaint layer stays out of the way.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        DisabledLayer = "transparent",
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

    // Both group models. Connected: no gap, a 1px negative overlap that collapses adjacent borders into
    // one seam, rounded control ends, flat interior corners. Standard: separate buttons a spacing step
    // apart, each keeping its own corner - MD3 Expressive keeps this and reshapes only the connected
    // half, since a standard group's segments are already the buttons the theme drew.
    // Baseline M3 has no button-group component of its own - the group is an Expressive addition - so
    // this states the plain, unanimated reading: one gap at every size, segments sharing a seam, and a
    // selection that is a repaint rather than a reshape. The Expressive theme replaces the whole record
    // with the spec's per-size ramps.
    internal static readonly ButtonGroupTokens ButtonGroup = new()
    {
        StandardGapXs = "var(--flare-spacing-4)",
        StandardGapSm = "var(--flare-spacing-4)",
        StandardGapMd = "var(--flare-spacing-4)",
        StandardGapLg = "var(--flare-spacing-4)",
        StandardGapXl = "var(--flare-spacing-4)",
        ConnectedGap = "0",
        ConnectedOverlap = "-1px",
        ConnectedOuterRadius = "var(--flare-shape-small)",
        ConnectedSelectedRadius = "var(--flare-shape-small)",
        ConnectedInnerRadiusXs = "0",
        ConnectedInnerRadiusSm = "0",
        ConnectedInnerRadiusMd = "0",
        ConnectedInnerRadiusLg = "0",
        ConnectedInnerRadiusXl = "0",
        ConnectedPressedRadiusXs = "0",
        ConnectedPressedRadiusSm = "0",
        ConnectedPressedRadiusMd = "0",
        ConnectedPressedRadiusLg = "0",
        ConnectedPressedRadiusXl = "0",
        ZActive = "1",
    };

    internal static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "0.125rem", // 2dp (between space)

        // Trigger is square at every size. `auto` hands the width to the stylesheet's aspect-ratio, which
        // reads the button's real height; a length here (as the other themes use) would win instead.
        // It cannot forward --_flare-btn-height: this record is emitted on :root, where that per-size
        // variable does not exist, so every size would end up the md fallback width.
        TriggerWidth = "auto",

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
    // Only the segmented container's own chrome: the buttons inside it are buttons and read the button
    // family for everything else.
    internal static readonly ToggleButtonTokens ToggleButton = new()
    {
        GroupBorder = "1px solid var(--flare-color-outline)",
        GroupRadius = "var(--flare-shape-full)",
        GroupRadiusVertical = "var(--flare-shape-medium)",
        GroupDivider = "var(--flare-color-outline)",
    };

    // MD2 FABs are circular; resting 6dp, pressed 12dp. 6dp has no level in the shared six-step
    // scale (it runs 0/1/2/4/8/12), so the resting shadow is written out from the same umbra /
    // penumbra / ambient family the levels above are built from.
    // FAB: padding-based sizing, large/medium/extra-large rounding.
    internal static readonly FabTokens Fab = new()
    {
        PaddingSm = "0.5rem",
        PaddingMd = "1rem",
        PaddingLg = "1.75rem",
        RadiusSm = "9999px",
        RadiusMd = "9999px",
        RadiusLg = "9999px",
        Gap = "0.75rem",
        Shadow = Dp6,
        HoverShadow = "var(--flare-elevation-5)",
        AnchorOffset = "1.5rem",
    };

    // The box is 24dp here, against Material 3's 18dp - and the difference is the whole control,
    // since a checkbox IS its box (spec redline "Checkboxes").
    internal static readonly CheckboxTokens Checkbox = new()
    {
        Size = "1.5rem",
        BorderWidth = "2px",
        Radius = "2px",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        // Material dims the whole control; the shared state value is the one it means.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };
    internal static readonly RadioTokens Radio = new()
    {
        Size = "1.25rem",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        // The focus this theme already states for the checkbox and the switch - the selection
        // controls answer as one family rather than each drawing its own ring.
        FocusOutline = "3px solid var(--flare-color-secondary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        // Material dims the whole control; the shared state value is the one it means.
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };
    // MD2 chips are fully rounded ("stadium") at 32dp.
    internal static readonly ChipTokens Chip = new() { Radius = "9999px", Height = "2rem" };
    // MD2 tabs: 2dp active indicator, uppercase labels (uppercased in CSS).
    internal static readonly TabsTokens Tabs = new()
    {
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        LabelFont = "var(--flare-typescale-label-large-font)",
        LabelSize = "var(--flare-typescale-label-large-size)",
        LabelWeight = "var(--flare-typescale-label-large-weight)",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "2px",
        ActiveColor = "var(--flare-color-primary)",
        // Secondary navigation tab: 2dp indicator, active label on-surface (spec md.comp
        // .secondary-navigation-tab.active-indicator.height / .label-text.color).
        SecondaryIndicatorThickness = "2px",
        SecondaryActiveColor = "var(--flare-color-on-surface)",
        // Spec is on-surface-VARIANT at rest (#49454F) for both tab levels, and on-surface only once
        // the tab is hovered or focused. This was on-surface, so an inactive tab read as dark as the
        // active one - invisible on a primary strip, where the accent carries the distinction, but
        // plainly wrong on a secondary strip, where the label colour is all there is besides a 2dp
        // rail. The hover darkening is not modelled yet; the state layer signals hover meanwhile.
        InactiveColor = "var(--flare-color-on-surface-variant)",
        DividerColor = "var(--flare-color-surface-variant)",
        SelectedBg = "var(--flare-color-secondary-container)",
        SelectedFg = "var(--flare-color-on-secondary-container)",
        FilledBg = "var(--flare-color-primary)",
        FilledFg = "var(--flare-color-on-primary)",
        TrackBg = "transparent",
        PillRadius = "var(--flare-shape-full)",
        TabDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ScrollDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // Menu (MD3 Expressive "Menus"): container 16dp (shape-large), elevation 3,
    // item 4dp (end items 12dp), label-large text, vertical 8dp, gap 2dp, focus-ring secondary.
    /// <summary>List container and row tokens.</summary>
    internal static readonly ListTokens List = new()
    {
        Bg = "var(--flare-color-surface)",
        Radius = "var(--flare-shape-medium)",
        Divider = "1px solid var(--flare-color-outline-variant)",
        ItemHeight = "3.5rem",
        ItemHeightTwoLine = "4.5rem",
        ItemHeightDense = "3rem",
        ItemHeightTwoLineDense = "3.5rem",
        ItemPaddingBlock = "var(--flare-spacing-6)",
        ItemPaddingBlockDense = "var(--flare-spacing-3)",
        ItemPaddingInline = "var(--flare-spacing-8)",
        ItemGap = "var(--flare-spacing-8)",
        ItemContentGap = "var(--flare-spacing-1)",
        ItemRadius = "0",
        ItemLabelFont = "var(--flare-typescale-body-large-font)",
        ItemLabelSize = "var(--flare-typescale-body-large-size)",
        ItemColor = "var(--flare-color-on-surface)",
        ItemTrailingColor = "var(--flare-color-on-surface-variant)",
        ItemSelectedBg = "var(--flare-color-secondary-container)",
        ItemSelectedColor = "var(--flare-color-on-secondary-container)",
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    /// <summary>Accordion container, header and body tokens.</summary>
    internal static readonly AccordionTokens Accordion = new()
    {
        Border = "1px solid var(--flare-color-outline-variant)",
        Radius = "var(--flare-shape-medium)",
        PanelDivider = "1px solid var(--flare-color-outline-variant)",
        HeaderBg = "var(--flare-color-surface)",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderPaddingBlock = "var(--flare-spacing-8)",
        HeaderPaddingInline = "var(--flare-spacing-12)",
        HeaderGap = "var(--flare-spacing-4)",
        HeaderLabelFont = "var(--flare-typescale-title-small-font)",
        HeaderLabelSize = "var(--flare-typescale-title-small-size)",
        HeaderLabelWeight = "var(--flare-typescale-title-small-weight)",
        HeaderDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconSize = "0.875rem",
        BodyPaddingBlock = "var(--flare-spacing-8)",
        BodyPaddingInline = "var(--flare-spacing-12)",
        BodyColor = "var(--flare-color-on-surface-variant)",
    };

    /// <summary>Standalone collapse header tokens.</summary>
    internal static readonly CollapseTokens Collapse = new()
    {
        HeaderBg = "transparent",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderRadius = "var(--flare-shape-small)",
        HeaderPaddingBlock = "var(--flare-spacing-6)",
        HeaderPaddingInline = "var(--flare-spacing-8)",
        HeaderGap = "var(--flare-spacing-4)",
        HeaderLabelFont = "var(--flare-typescale-title-small-font)",
        HeaderLabelSize = "var(--flare-typescale-title-small-size)",
        HeaderLabelWeight = "var(--flare-typescale-title-small-weight)",
        HeaderDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconColor = "var(--flare-color-on-surface-variant)",
    };

    // Classic flat 4dp menu (no MD3 Expressive 16dp panel or island grouping). The default MenuTokens
    // is already flat (square items, transparent groups); MD2 only pins the panel to the 4dp shape.
    // Menus sit at 8dp in the default elevation table, which is level 4 here.
    internal static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        PanelRadius = "var(--flare-shape-extra-small)", // 16dp,
        PanelMinWidth = "7rem",                       // 112dp
        PanelShadow = "var(--flare-elevation-4)", // elevation 3,
        PanelPaddingInline = "0.125rem",              // group padding 2dp
        PanelPaddingBlock = "0.125rem",               // 2dp
        ItemHeight = "3rem",                          // item height 48dp (MD3 list-item)
        ItemPaddingBlock = "0.5rem",                  // top/bottom 8dp
        // Dense: the value core used to hardcode in menuitem.css, so a theme could style a normal menu
        // but never a dense one. Unchanged.
        ItemPaddingBlockDense = "0.375rem",
        ItemGapDense = "0.5rem",
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
        // Material dims a disabled item.
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // Input - filled style (md.comp.filled-text-field): surface-container-highest container, 56dp
    // height (1rem block padding + 24px line), 1dp on-surface-variant active indicator, hover ->
    // on-surface indicator + 8% state layer, focus -> 3dp primary (Expressive).
    internal static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface-container-highest)",
        BorderColor = "transparent",                  // filled default: no side border, colour only
        OutlinedRadius = "var(--flare-shape-extra-small) var(--flare-shape-extra-small) 0 0",
        BorderBottomColor = "var(--flare-color-on-surface-variant)",
        // Focus = a 2px primary active indicator drawn as a layout-neutral inset shadow (no jump).
        FocusRing = "inset 0 -3px 0 0 var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        HoverBorderBottomColor = "var(--flare-color-on-surface)",
        HoverStateLayer = "linear-gradient(color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent), color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent))",
        PaddingXs = "0.1875rem 0.5rem",
        PaddingSm = "0.375rem 0.625rem",
        PaddingMd = "1rem 1rem",                      // the M3 56dp field
        // Large and extra-large are measured up from the 56dp medium. They used to be 14px and 18px of
        // block padding, authored in core against a shorter medium, which left Large SHORTER than Medium
        // under this theme.
        PaddingLg = "1.25rem 1.125rem",
        PaddingXl = "1.5rem 1.25rem",
        // Height ramp. M3 puts the text field AND the select at 56dp, so Md is the spec number and the
        // rest are measured off the padding ramp above - each step is the tallest control that step used
        // to produce, so the family converges upward and no control shrinks. Before these existed the
        // steps disagreed by 2px (the trigger was taller) and by 5px at Xl (the text field was).
        HeightXs = "1.875rem", // 30px
        HeightSm = "2.25rem",  // 36px
        HeightMd = "3.5rem",   // 56px - the M3 field height
        HeightLg = "4rem",     // 64px
        HeightXl = "4.75rem",  // 76px
        IconSize = "1.5rem",                          // leading/trailing icon 24dp
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // Chart. The categorical palette is built from the theme's OWN hues rather than from fixed ink, so a
    // dynamic palette and a mode switch both carry through to the plot: three source hues (primary,
    // tertiary, secondary) crossed with four tonal treatments - pure, deepened toward on-surface, lightened
    // toward surface, and blended with a neighbour. The error role is deliberately ABSENT: red carries a
    // meaning here that a fourth series does not have. A theme that wants twelve genuinely distinct hues
    // overrides these twelve values; that is what the tokens are for.
    // Sizes without a unit are viewBox units: the gauge is 200 wide and scales to its container, so
    // these are proportions of the gauge rather than pixels. Band colors are absent by design - a band is
    // a FlareZone carrying its own FlareColor, and which part of a scale is "bad" is the application's
    // statement about its data, not the theme's.
    internal static readonly GaugeTokens Gauge = new()
    {
        TrackColor = "var(--flare-color-surface-container-highest)",
        TrackWidth = "14",
        TrackCap = "round",

        FillColor = "var(--flare-color-primary)",
        FillWidth = "14",

        NeedleColor = "var(--flare-color-on-surface)",
        NeedleWidth = "3",
        NeedleLength = "0.78",
        PivotColor = "var(--flare-color-on-surface)",
        PivotRadius = "5",

        TickColor = "var(--flare-color-outline)",
        TickWidth = "1.5",
        TickLength = "7",
        TickMinorColor = "var(--flare-color-outline-variant)",
        TickMinorWidth = "1",
        TickMinorLength = "4",
        TickGap = "5",

        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSize = "9",
        ValueColor = "var(--flare-color-on-surface)",
        ValueSize = "var(--flare-typescale-headline-small-size)",
        ValueWeight = "500",

        BandOpacity = "0.55",
        TargetColor = "var(--flare-color-on-surface)",
        TargetWidth = "2.5",
    };

    internal static readonly ChartTokens Chart = new()
    {
        Series1 = "var(--flare-color-primary)",
        Series2 = "var(--flare-color-tertiary)",
        Series3 = "var(--flare-color-secondary)",
        Series4 = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-tertiary))",
        Series5 = "color-mix(in srgb, var(--flare-color-secondary) 50%, var(--flare-color-tertiary))",
        Series6 = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-secondary))",
        Series7 = "color-mix(in srgb, var(--flare-color-primary) 65%, var(--flare-color-on-surface))",
        Series8 = "color-mix(in srgb, var(--flare-color-tertiary) 65%, var(--flare-color-on-surface))",
        Series9 = "color-mix(in srgb, var(--flare-color-secondary) 65%, var(--flare-color-on-surface))",
        Series10 = "color-mix(in srgb, var(--flare-color-primary) 55%, var(--flare-color-surface))",
        Series11 = "color-mix(in srgb, var(--flare-color-tertiary) 55%, var(--flare-color-surface))",
        Series12 = "color-mix(in srgb, var(--flare-color-secondary) 55%, var(--flare-color-surface))",

        // Marks. Sizes without a unit are viewBox units: the plot is 400 wide and scales to its container,
        // so these are proportions of the chart, not pixels.
        LineWidth = "2",
        LineCap = "round",
        PointRadius = "2.5",
        PointOpacity = "0.6",
        BubbleMinRadius = "4",
        BubbleMaxRadius = "24",
        BarRadius = "2",
        AreaOpacity = "0.35",
        RadarFillOpacity = "0.2",
        WedgeOpacity = "0.7",
        SliceStrokeColor = "var(--flare-color-surface)",
        SliceStrokeWidth = "1.5",

        // Sequential ramp: one hue, intensity carries the value. The floor keeps a zero cell visible as a
        // cell rather than as a hole in the grid.
        RampColor = "var(--flare-color-primary)",
        RampMinOpacity = "0.12",
        RampMaxOpacity = "1",
        CellRadius = "2",
        CellGap = "2",
        CellHoverOpacity = "0.85",

        // Axes. A grid line is lighter than an outline: it sits behind the data and must not compete.
        GridColor = "var(--flare-color-outline-variant)",
        GridWidth = "0.5",
        GridDash = "none",
        GridMinorColor = "color-mix(in srgb, var(--flare-color-outline-variant) 45%, transparent)",
        GridMinorWidth = "0.5",
        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSize = "9px",
        ValueColor = "var(--flare-color-on-surface-variant)",
        ValueOnFillColor = "var(--flare-color-surface)",
        ValueSize = "8px",
        AxisTitleColor = "var(--flare-color-on-surface-variant)",
        AxisTitleSize = "10px",

        Surface = "var(--flare-color-surface)",
        Radius = "var(--flare-shape-medium)",
        BorderWidth = "1px",
        BorderColor = "var(--flare-color-outline-variant)",
        Padding = "var(--flare-spacing-8)",
        Gap = "var(--flare-spacing-4)",

        LegendGap = "var(--flare-spacing-6)",
        LegendItemGap = "var(--flare-spacing-3)",
        LegendDotSize = "0.625rem",
        LegendDotRadius = "var(--flare-shape-full)",
        LegendSize = "var(--flare-typescale-label-small-size)",
        LegendColor = "var(--flare-color-on-surface-variant)",
        LegendOffOpacity = "0.4",

        // Overlays. The annotation default IS the error role - a threshold line means what red means,
        // which is exactly why a data series must not borrow it.
        TrendWidth = "1.5",
        TrendDash = "5 4",
        TrendOpacity = "0.7",
        AnnotationColor = "var(--flare-color-error)",
        AnnotationWidth = "1.5",
        AnnotationDash = "4 3",
        AnnotationBandOpacity = "0.12",
        AnnotationArrowSize = "7",
        AnnotationPointRadius = "3.5",

        // Dash rhythm is measured in stroke widths, not absolute units, because a round cap adds half a
        // stroke width to BOTH ends of every dash: the painted dash grows by one width and the painted gap
        // shrinks by one. Absolute arrays therefore only hold for the width they were authored against -
        // at Expressive's 3 the old "6 4" painted 9-long dashes separated by 1, i.e. a solid line.
        // Authored 2w/3w paints as 3w of ink and 2w of air at any width, and a dash shorter than the cap
        // paints as a round dot exactly one stroke wide, which is what Dotted has to be.
        LineDashDashed = "calc(var(--flare-chart-line-width) * 2) calc(var(--flare-chart-line-width) * 3)",
        LineDashDotted = "0.1 calc(var(--flare-chart-line-width) * 3)",
        LineDashDashDot = "calc(var(--flare-chart-line-width) * 2) calc(var(--flare-chart-line-width) * 3)" +
                          " 0.1 calc(var(--flare-chart-line-width) * 3)",

        ZoomSelectionFill = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        ZoomSelectionStroke = "var(--flare-color-primary)",
    };

    // Progress - shared Material geometry. Optional Expressive wave behavior is supplied by the
    // Expressive theme package, so this baseline token bundle stays reusable by flat themes.
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
        LinearIndeterminateDuration = "1500ms",
        LinearIndeterminateEasing = "var(--flare-motion-easing-standard)",
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
        CircularIndeterminateRotationDuration = "1400ms",
        CircularIndeterminateProgressDuration = "1400ms",
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
        LinkDisabledOpacity = "var(--flare-state-disabled-opacity)",
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
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ZIndex = "1100",
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

    // The navigation drawer is 256dp wide here (spec redline "Standard navigation drawer"), against
    // the 360dp Material 3 widened it to.
    internal static readonly DrawerTokens Drawer = new()
    {
        Width = "256px",
        MiniWidth = "72px",
        // The M3 navigation drawer is a surface-container-low panel against a surface background; the tone
        // step is the separation, and the spec draws no divider on either edge.
        Border = "none",
        SectionBorder = "none",
    };

    internal static readonly SnackbarTokens Snackbar = new()
    {
        Radius = "var(--flare-shape-extra-small)",
        MinHeight = "3rem",
        PaddingBlock = "0.875rem",
        ProviderInset = "1.5rem",
        CloseOpacity = "0.75",
    };

    // Splitter: an 8dp gutter - comfortable to grab without reading as a divider - carrying a 2dp grip
    // mark, tinted with the primary wash on hover the way the rest of the theme signals an active target.
    internal static readonly SplitterTokens Splitter = new()
    {
        GutterSize = "0.5rem",
        GripThickness = "2px",
        GripLength = "1.75rem",
        Color = "var(--flare-color-surface-variant)",
        HoverColor = "color-mix(in srgb, var(--flare-color-primary) 24%, transparent)",
        IconSize = "1.125rem",
        IconColor = "var(--flare-color-on-surface-variant)",
    };

    // MD2 slider: thin 4px rail with a round 20dp thumb (no MD3 Expressive bar handle).
    internal static readonly SliderTokens Slider = new()
    {
        // Size ramp: track 16/24/40/56/96dp, handle 44/44/52/68/108dp, track shape 8/8/12/16/28dp.
        TrackHeightXs = "4px",
        TrackHeightSm = "4px",
        TrackHeightMd = "4px",
        TrackHeightLg = "4px",
        TrackHeightXl = "4px",
        TrackRadiusXs = "9999px",
        TrackRadiusSm = "9999px",
        TrackRadiusMd = "9999px",
        TrackRadiusLg = "9999px",
        TrackRadiusXl = "9999px",
        HandleHeightXs = "20px",
        HandleHeightSm = "20px",
        HandleHeightMd = "20px",
        HandleHeightLg = "20px",
        HandleHeightXl = "20px",
        // Flanking StartIcon/EndIcon ramp.
        IconSizeXs = "20px",
        IconSizeSm = "22px",
        IconSizeMd = "24px",
        IconSizeLg = "24px",
        IconSizeXl = "32px",
        Length = "12rem",
        GapRadius = "0px",
        Gap = "0px",
        HandleWidth = "20px",
        HandlePressedWidth = "20px",
        HandleRadius = "9999px",
        HandleClipPath = "none",
        HandleBorderWidth = "0",
        // Follow the per-instance Color: --fc-main is the local accent, falling back to the role.
        HandleFill = "var(--flare-color-primary)",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "color-mix(in srgb, var(--flare-color-primary) 24%, transparent)",
        StateLayerSize = "40px",
        StateHoverOpacity = "0.08",
        StatePressedOpacity = "0.10",
        StopColor = "var(--flare-color-primary)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "4px",
        ValueBg = "var(--flare-color-inverse-surface)",
        ValueColor = "var(--flare-color-inverse-on-surface)",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        // Disabled is a fade of the content colour, and the two halves fade by different amounts so
        // the filled portion is still readable against the rest.
        DisabledActiveColor = "color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-disabled-opacity) * 100%), transparent)",
        DisabledInactiveColor = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
    };

    // MD2 dialogs use a 4dp corner.
    internal static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-extra-small)",
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
        RangeLayer = "color-mix(in srgb, var(--flare-color-primary) 14%, transparent)",
        RowEditingPct = "6%",
        LoadingVeilPct = "55%",
        LoadingDim = "0.6",
    };

    // Overlay: MD3 is the airier of the two, so a dialog keeps a full 3rem off the edge and drops to
    // 2rem on a phone, where the screen cannot spare it. A floating panel takes at most 70% of it.
    internal static readonly OverlayTokens Overlay = new()
    {
        ViewportInset = "3rem",
        ViewportInsetCompact = "2rem",
        PanelMaxBlockSize = "70dvh",
    };

    // Drag: MD3 gives a dragged object elevation 4 and leaves a dimmed silhouette behind it. The drop
    // target reads the primary state layer at the DRAGGED opacity - the one MD3 already defines for
    // exactly this state - so the target's tint and a card's own dragged state stay in step.
    internal static readonly DragTokens Drag = new()
    {
        SourceOpacity = "0.4",
        PreviewElevation = "var(--flare-elevation-4)",
        PreviewOpacity = "1",
        ZoneActiveBackground =
            "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
        ZoneActiveOutline = "var(--flare-color-primary)",
        IndicatorColor = "var(--flare-color-primary)",
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
        Indent = "var(--flare-spacing-12)",
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        SelectedColor = "var(--flare-color-primary)",
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
        BtnDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    // The Material 2 switch is a thin track with a thumb RIDING OVER it, not a thumb tucked inside a
    // pill: the published redline gives one 20dp thumb and a 36dp overall width, and the thumb is
    // what sets the height. Material 3's 52x32 track with a thumb inside is the shape this theme was
    // wearing. The track is the overall width less the thumb's overhang at each end, and 14dp tall -
    // the two numbers the drawing implies but does not print.
    // Switch (MD3 baseline geometry: 52x32 track, 24 thumb, elevation-1 lift). Carried explicitly
    // now that the core record ships no defaults.
    internal static readonly SwitchTokens Switch = new()
    {
        // Per-size ramp; the xs/sm/lg/xl steps were literals in switch.css, md is this theme's value.
        TrackWidthXs = "1.75rem", TrackWidthSm = "1.9375rem", TrackWidthMd = "34px", TrackWidthLg = "2.625rem", TrackWidthXl = "3.0625rem",
        TrackHeightXs = "0.5rem", TrackHeightSm = "0.6875rem", TrackHeightMd = "14px", TrackHeightLg = "1.0625rem", TrackHeightXl = "1.25rem",
        TrackOffBg = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        TrackOnBg = "color-mix(in srgb, var(--fc-main, var(--flare-color-primary)) 54%, transparent)",
        TrackBorder = "none",
        TrackHoverBorderColor = "var(--flare-color-outline)",
        ThumbOffSizeXs = "1rem", ThumbOffSizeSm = "1.125rem", ThumbOffSizeMd = "20px", ThumbOffSizeLg = "1.5rem", ThumbOffSizeXl = "1.75rem",
        ThumbOnSizeXs = "1rem", ThumbOnSizeSm = "1.125rem", ThumbOnSizeMd = "20px", ThumbOnSizeLg = "1.5rem", ThumbOnSizeXl = "1.75rem",
        ThumbPressedOffSizeXs = "1rem", ThumbPressedOffSizeSm = "1.125rem", ThumbPressedOffSizeMd = "20px", ThumbPressedOffSizeLg = "1.5rem", ThumbPressedOffSizeXl = "1.75rem",
        ThumbPressedOnSizeXs = "1rem", ThumbPressedOnSizeSm = "1.125rem", ThumbPressedOnSizeMd = "20px", ThumbPressedOnSizeLg = "1.5rem", ThumbPressedOnSizeXl = "1.75rem",
        ThumbOffLeftXs = "-1px", ThumbOffLeftSm = "-1px", ThumbOffLeftMd = "-1px", ThumbOffLeftLg = "-1px", ThumbOffLeftXl = "-1px",
        ThumbOnLeftXs = "calc(100% - 15px)", ThumbOnLeftSm = "calc(100% - 17px)", ThumbOnLeftMd = "calc(100% - 19px)", ThumbOnLeftLg = "calc(100% - 23px)", ThumbOnLeftXl = "calc(100% - 27px)",
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
        FocusShadowOff = "0 0 0 0.75rem color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        FocusShadowOn = "0 0 0 0.5rem color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-focus-opacity) * 100%), transparent)",
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
    // M3 draws separators as 1dp outline-variant rules and reserves the heavier weight for selection and
    // drop targets.
    internal static readonly BorderTokens Border = new()
    {
        Width = "1px",
        WidthEmphasis = "2px",
        Style = "solid",
        Divider = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline-variant)",
        Outline = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline)",
    };

    // M3 has no scrollbar component, so this is read off what the language does elsewhere: an
    // interactive element at rest carries the on-surface tone at a low opacity and firms up on hover,
    // and the track stays out of the way so the surface behind it keeps its own tone.
    internal static readonly ScrollbarTokens Scrollbar = new()
    {
        Width = "thin",
        Size = "8px",
        Thumb = "color-mix(in srgb, var(--flare-color-on-surface) 28%, transparent)",
        ThumbHover = "color-mix(in srgb, var(--flare-color-on-surface) 45%, transparent)",
        Track = "transparent",
        Radius = "var(--flare-shape-full)",
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

    internal static readonly AppBarTokens AppBar = new()
    {
        Gap = "0.25rem",
        Height = "4rem",
        HeightDense = "3rem",
        PaddingX = "0.25rem",
        TitlePaddingX = "0.75rem",
        // M3 separates the top app bar from the content by raising its container tone on scroll, and the
        // spec draws no rule under it. Elevation carries the separation here too.
        Border = "none",
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

    internal static readonly FileUploadTokens FileUpload = new()
    {
        BorderWidth = "2px",
        HoverBg = "color-mix(in srgb, var(--flare-color-primary) 5%, var(--flare-color-surface))",
        DraggingBg = "color-mix(in srgb, var(--flare-color-primary) 10%, var(--flare-color-surface))",
        DraggingRingWidth = "2px",
        IconSize = "2.5rem",
        ZoneMinHeight = "10rem",
        ZoneRadius = "var(--flare-card-radius)",
        FileIconSize = "1.125rem",
        RowGap = "var(--flare-spacing-2)",
        RowRadius = "var(--flare-shape-extra-small)",
        RowActiveBg = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        RowSuccessBg = "color-mix(in srgb, var(--flare-color-tertiary) 8%, transparent)",
        RowErrorBg = "color-mix(in srgb, var(--flare-color-error) 8%, transparent)",
        RowErrorColor = "var(--flare-color-error)",
    };

    internal static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    internal static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        AppBarHeightDense = "3rem",              // MD3 dense top app bar = 48dp
        AppBarBg = "var(--flare-color-surface)",
        ContentPadding = "1.5rem 2rem",
        ContentPaddingMobile = "1rem",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "260px",
        AppBarBorder = "none",
        DrawerBorder = "none",
    };

    internal static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
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

    // Material asks for a 48dp minimum target; the control keeps its drawn size and the core widens
    // the hit area to this under a coarse pointer.
    internal static readonly TouchTokens Touch = new()
    {
        TargetMin = "48px",
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

    // A glyph swap is Material's "icon transition": the outgoing symbol leaves while the incoming one
    // arrives, rather than one repainting over the other. It rides the fast spring because the travel is
    // spatial and the element is small - the same pairing the switch thumb and the checkbox tick use.
    internal static readonly IconTokens Icon = new()
    {
        MorphDuration = "var(--flare-motion-duration-spring-fast)",
        MorphEasing = "var(--flare-motion-easing-spring-fast)",
        MorphScale = "0.6",
        MorphRotate = "90deg",
    };

    internal static readonly StripeTokens Stripe = new()
    {
        // A wash of the content colour, which is what a striped table has always been here. The data
        // grid used to step to surface-container-low instead; one language, one answer.
        Background = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
    };

    internal static readonly TableTokens Table = new()
    {
        CellPaddingH = "1rem",
        CellPaddingV = "0.75rem",
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
        FileUpload = FileUpload,
        Form = Form,
        Layout = Layout,
        Link = Link,
        Otp = Otp,
        Picker = Picker,
        Scrim = Scrim,
        Touch = Touch,
        ScrollTop = ScrollTop,
        Skeleton = Skeleton,
        Icon = Icon,
        Stripe = Stripe,
        Splitter = Splitter,
        Table = Table,
        TimePicker = TimePicker,
        Spacing = Spacing,
        Border = Border,
        Scrollbar = Scrollbar,
        Switch = Switch,
        DataGrid = DataGrid,
        Overlay = Overlay,
        Drag = Drag,
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
        // A 2px ring, not the 3px Material 3 widened it to.
        FocusRing = "2px solid var(--flare-color-primary)",
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
        List = List,
        Accordion = Accordion,
        Collapse = Collapse,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        // MD3 cards: elevated hover = level 3 (spec), outlined border = full outline role (spec),
        // 16px inner padding on raw-content cards. Variant bg roles come from the base CardTokens.
        Card = new()
        {
            // A card rests at 1dp and rises to 8dp when it is picked up, not to the 4dp the Material 3
            // baseline steps to (spec "Default elevation values").
            ElevationHover = "var(--flare-elevation-4)",
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
            StateLayer = "var(--flare-state-hover-layer)",
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
        Chart = Chart,
        Gauge = Gauge,
        Progress = Progress,
        // Popover / dropdown panels use the medium shape (Snackbar/Dialog/Nav already match defaults).
        Popover = Popover,
        Tooltip = Tooltip,
        Avatar = Avatar,
        Extended = Extended,
    };

    // The Material dp shadows the six-level scale has no room for. Same three-layer recipe as the
    private const string Dp6 = "0 3px 5px -1px rgba(0,0,0,0.2), 0 6px 10px 0 rgba(0,0,0,0.14), 0 1px 18px 0 rgba(0,0,0,0.12)";
    private const string Dp16 = "0 8px 10px -5px rgba(0,0,0,0.2), 0 16px 24px 2px rgba(0,0,0,0.14), 0 6px 30px 5px rgba(0,0,0,0.12)";
    private const string Dp24 = "0 11px 15px -7px rgba(0,0,0,0.2), 0 24px 38px 3px rgba(0,0,0,0.14), 0 9px 46px 8px rgba(0,0,0,0.12)";

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
        // The baseline background AND surface are both #FFFFFF: in this language a card is told apart
        // from the page it sits on by its shadow, not by a different grey.
        Background = "#FFFFFF",
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
        // 87% white over #121212 - the dark high-emphasis level, mirroring #212121 on the light side.
        // #E6E1E5 is Material 3's dark on-surface and had been carried in from the reference package.
        OnSurface = "#E0E0E0",
        SurfaceVariant = "#2C2C2C",
        OnSurfaceVariant = "#BDBDBD",
        OnSurfaceVariant2 = "#A3A3A3",
        SurfaceContainerLow = "#1E1E1E",
        SurfaceContainer = "#242424",
        SurfaceContainerHigh = "#2C2C2C",
        SurfaceContainerHighest = "#333333",
        Background = "#121212",
        OnBackground = "#E0E0E0",
        Outline = "#8A8A8A",
        OutlineVariant = "#3A3A3A",
        InverseSurface = "#E0E0E0",
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
