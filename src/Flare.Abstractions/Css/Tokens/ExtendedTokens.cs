namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for tooltip popup.</summary>
public static class TooltipPopup
{
    // Tooltip color/radius/padding/font/motion are NOT tokenized here: the tooltip reuses the shared
    // inverse-surface role, --flare-shape-*, --flare-spacing-*, --flare-typescale-* and --flare-motion-*
    // directly (per the "reuse shared tokens, do not duplicate" mandate). Only the two tooltip-specific
    // geometry values remain.
    /// <summary>CSS custom-property name for the rich-tooltip max width.</summary>
    public const string MaxWidth = "--flare-tooltip-max-width";
    /// <summary>CSS custom-property name for the tooltip's distance from its anchor.</summary>
    public const string Offset = "--flare-tooltip-offset";
}

/// <summary>CSS variable tokens for popover popup.</summary>
public static class PopoverPopup
{
    // Popover surface/elevation/padding/width/offset/scrim/motion are NOT tokens here - popover.css and
    // menu.css read the shared color/elevation/spacing/motion tokens directly. Only the corner radius is a
    // popover-family token (themes vary it: shape-medium vs shape-small), shared by all popover-like surfaces.
    /// <summary>CSS custom-property name for the popover-family corner radius.</summary>
    public const string Radius = "--flare-popover-radius";
}

/// <summary>CSS variable tokens for data grid field.</summary>
public static class DataGridField
{
    // DataGrid color/font/bg/padding/row-height are NOT tokenized here: the grid reuses the shared color
    // roles, --flare-typescale-*, --flare-spacing-* and --flare-state-* directly (per the "reuse shared
    // tokens, do not duplicate" mandate). Only grid-specific geometry - icon sizes, divider/handle widths,
    // overlay percentages - are tokens.
    /// <summary>CSS custom-property name for the sort-direction icon size.</summary>
    public const string SortIconSize = "--flare-datagrid-sort-icon-size";
    /// <summary>CSS custom-property name for the multi-sort priority badge size.</summary>
    public const string SortPrioritySize = "--flare-datagrid-sort-priority-size";
    /// <summary>CSS custom-property name for the filter-icon size.</summary>
    public const string FilterIconSize = "--flare-datagrid-filter-icon-size";
    /// <summary>CSS custom-property name for the boolean-cell icon size.</summary>
    public const string BoolIconSize = "--flare-datagrid-bool-icon-size";
    /// <summary>CSS custom-property name for the toolbar-button icon size.</summary>
    public const string BtnIconSize = "--flare-datagrid-btn-icon-size";
    /// <summary>CSS custom-property name for the close-icon size.</summary>
    public const string CloseIconSize = "--flare-datagrid-close-icon-size";
    /// <summary>CSS custom-property name for the group/tree chevron size.</summary>
    public const string ChevronSize = "--flare-datagrid-chevron-size";
    /// <summary>CSS custom-property name for the detail-row toggle icon size.</summary>
    public const string DetailIconSize = "--flare-datagrid-detail-icon-size";
    /// <summary>CSS custom-property name for the tree-toggle icon size.</summary>
    public const string TreeToggleSize = "--flare-datagrid-tree-toggle-size";
    /// <summary>CSS custom-property name for the composite-column label size.</summary>
    public const string CompositeLabelSize = "--flare-datagrid-composite-label-size";
    /// <summary>CSS custom-property name for the column resize-handle width.</summary>
    public const string ResizeHandleWidth = "--flare-datagrid-resize-handle-width";
    /// <summary>CSS custom-property name for the record (card) divider width.</summary>
    public const string RecordDividerWidth = "--flare-datagrid-record-divider-width";
    /// <summary>CSS custom-property name for the aggregate-row divider width.</summary>
    public const string AggregateDividerWidth = "--flare-datagrid-aggregate-divider-width";
    /// <summary>CSS custom-property name for the filter-group rail width.</summary>
    public const string FilterGroupRail = "--flare-datagrid-filter-group-rail";
    /// <summary>CSS custom-property name for the active-cell focus outline.</summary>
    public const string ActiveCellOutline = "--flare-datagrid-active-cell-outline";
    /// <summary>CSS custom-property name for the column-picker minimum width.</summary>
    public const string ColumnPickerMinWidth = "--flare-datagrid-column-picker-min-width";
    /// <summary>CSS custom-property name for the selected+hover row mix percentage.</summary>
    public const string RowSelectedHoverPct = "--flare-datagrid-row-selected-hover-pct";
    /// <summary>CSS custom-property name for the row-being-edited tint percentage.</summary>
    public const string RowEditingPct = "--flare-datagrid-row-editing-pct";
    /// <summary>CSS custom-property name for the loading-veil opacity percentage.</summary>
    public const string LoadingVeilPct = "--flare-datagrid-loading-veil-pct";
    /// <summary>CSS custom-property name for the loading content dim opacity.</summary>
    public const string LoadingDim = "--flare-datagrid-loading-dim";
}

/// <summary>CSS variable tokens for card field.</summary>
public static class CardField
{
    /// <summary>CSS custom-property name for the elevated bg token.</summary>
    public const string ElevatedBg = "--flare-card-elevated-bg";
    /// <summary>CSS custom-property name for the filled bg token.</summary>
    public const string FilledBg = "--flare-card-filled-bg";
    /// <summary>CSS custom-property name for the filled border token.</summary>
    public const string FilledBorder = "--flare-card-filled-border";
    /// <summary>CSS custom-property name for the outlined bg token.</summary>
    public const string OutlinedBg = "--flare-card-outlined-bg";
    /// <summary>CSS custom-property name for the outlined border token.</summary>
    public const string OutlinedBorder = "--flare-card-outlined-border";
    /// <summary>CSS custom-property name for the tonal bg token.</summary>
    public const string TonalBg = "--flare-card-tonal-bg";
    /// <summary>CSS custom-property name for the tonal color token.</summary>
    public const string TonalColor = "--flare-card-tonal-color";
    /// <summary>CSS custom-property name for the text color token.</summary>
    public const string TextColor = "--flare-card-text-color";
    /// <summary>CSS custom-property name for the radius token.</summary>
    public const string Radius = "--flare-card-radius";
    /// <summary>CSS custom-property name for the elevation token.</summary>
    public const string Elevation = "--flare-card-elevation";
    /// <summary>CSS custom-property name for the elevation hover token.</summary>
    public const string ElevationHover = "--flare-card-elevation-hover";
    /// <summary>CSS custom-property name for the selected border token.</summary>
    public const string SelectedBorder = "--flare-card-selected-border";
    /// <summary>CSS custom-property name for the selected bg token.</summary>
    public const string SelectedBg = "--flare-card-selected-bg";
    /// <summary>CSS custom-property name for the state layer token.</summary>
    public const string StateLayer = "--flare-card-state-layer";
    /// <summary>CSS custom-property name for the padding top token.</summary>
    public const string PaddingTop = "--flare-card-padding-top";
    /// <summary>CSS custom-property name for the padding right token.</summary>
    public const string PaddingRight = "--flare-card-padding-right";
    /// <summary>CSS custom-property name for the padding bottom token.</summary>
    public const string PaddingBottom = "--flare-card-padding-bottom";
    /// <summary>CSS custom-property name for the padding left token.</summary>
    public const string PaddingLeft = "--flare-card-padding-left";
    /// <summary>CSS custom-property name for the content padding token.</summary>
    public const string ContentPadding = "--flare-card-content-padding";
    /// <summary>CSS custom-property name for the header padding token.</summary>
    public const string HeaderPadding = "--flare-card-header-padding";
    /// <summary>CSS custom-property name for the footer padding token.</summary>
    public const string FooterPadding = "--flare-card-footer-padding";
    /// <summary>CSS custom-property name for the actions padding token.</summary>
    public const string ActionsPadding = "--flare-card-actions-padding";
    /// <summary>CSS custom-property name for the actions gap token.</summary>
    public const string ActionsGap = "--flare-card-actions-gap";
    /// <summary>CSS custom-property name for the media radius token.</summary>
    public const string MediaRadius = "--flare-card-media-radius";
    /// <summary>CSS custom-property name for the title color token.</summary>
    public const string TitleColor = "--flare-card-title-color";
    /// <summary>CSS custom-property name for the title font family token.</summary>
    public const string TitleFontFamily = "--flare-card-title-font-family";
    /// <summary>CSS custom-property name for the title font size token.</summary>
    public const string TitleFontSize = "--flare-card-title-font-size";
    /// <summary>CSS custom-property name for the subtitle color token.</summary>
    public const string SubtitleColor = "--flare-card-subtitle-color";
    /// <summary>CSS custom-property name for the subtitle font family token.</summary>
    public const string SubtitleFontFamily = "--flare-card-subtitle-font-family";
    /// <summary>CSS custom-property name for the subtitle font size token.</summary>
    public const string SubtitleFontSize = "--flare-card-subtitle-font-size";
    /// <summary>CSS custom-property name for the transition duration token.</summary>
    public const string TransitionDuration = "--flare-card-transition-duration";
    /// <summary>CSS custom-property name for the transition easing token.</summary>
    public const string TransitionEasing = "--flare-card-transition-easing";
}

/// <summary>
/// CSS variable tokens for the avatar GROUP (overlapping stack + overflow badge). Avatar-proper color,
/// size, radius and font are NOT tokenized here on purpose: they come from the shared systems - the color
/// role / tonal <c>--fc-*</c> model, <c>--flare-shape-*</c>, <c>--flare-typescale-*</c> and the size
/// utility classes - so the component is themed the same way as every other surface.
/// </summary>
public static class AvatarField
{
    /// <summary>CSS custom-property name for the group overlap spacing (negative inline margin).</summary>
    public const string GroupSpacing = "--flare-avatar-group-spacing";
    /// <summary>CSS custom-property name for the group overlap ring width.</summary>
    public const string GroupBorderWidth = "--flare-avatar-group-border-width";
    /// <summary>CSS custom-property name for the group overlap ring color.</summary>
    public const string GroupBorderColor = "--flare-avatar-group-border-color";
    /// <summary>CSS custom-property name for the overflow badge background.</summary>
    public const string OverflowBg = "--flare-avatar-overflow-bg";
    /// <summary>CSS custom-property name for the overflow badge text color.</summary>
    public const string OverflowColor = "--flare-avatar-overflow-color";
}

/// <summary>CSS variable tokens for progress field.</summary>
public static class ProgressField
{
    // Colors, the sm/lg size+stroke variants, indeterminate timing and buffer-color are gone: progress.css
    // reuses the shared color/motion scales and its own size classes directly. What remains is the geometry
    // the component reads - in CSS, and (for the wavy variant) in C# via ReadToken.
    /// <summary>CSS custom-property name for the linear track/indicator height.</summary>
    public const string LinearHeight = "--flare-progress-linear-height";
    /// <summary>CSS custom-property name for the linear track corner radius.</summary>
    public const string TrackRadius = "--flare-progress-track-radius";
    /// <summary>CSS custom-property name for the gap between indicator and remaining track.</summary>
    public const string Gap = "--flare-progress-gap";
    /// <summary>CSS custom-property name for the trailing stop-indicator size.</summary>
    public const string StopSize = "--flare-progress-stop-size";
    /// <summary>CSS custom-property name for the trailing stop-indicator inset.</summary>
    public const string StopInset = "--flare-progress-stop-inset";
    /// <summary>CSS custom-property name for the trailing stop-indicator color.</summary>
    public const string StopColor = "--flare-progress-stop-color";
    /// <summary>CSS custom-property name for the buffer track opacity.</summary>
    public const string BufferOpacity = "--flare-progress-buffer-opacity";
    /// <summary>CSS custom-property name for the circular variant diameter.</summary>
    public const string CircularSize = "--flare-progress-circular-size";
    /// <summary>CSS custom-property name for the circular indicator stroke width.</summary>
    public const string CircularWidth = "--flare-progress-circular-width";
    /// <summary>CSS custom-property name for the circular indicator line cap.</summary>
    public const string CircularCap = "--flare-progress-circular-cap";
    /// <summary>CSS custom-property name for the circular indicator/track gap.</summary>
    public const string CircularGap = "--flare-progress-circular-gap";
    /// <summary>CSS custom-property name for the wavy-progress enable flag (1 = on).</summary>
    public const string WavyEnabled = "--flare-progress-wavy-enabled";
    /// <summary>CSS custom-property name for the wavy linear-track height.</summary>
    public const string WavyHeight = "--flare-progress-wavy-height";
    /// <summary>CSS custom-property name for the wave length.</summary>
    public const string WaveLength = "--flare-progress-wave-length";
    /// <summary>CSS custom-property name for the wave amplitude.</summary>
    public const string WaveAmplitude = "--flare-progress-wave-amplitude";
    /// <summary>CSS custom-property name for the wave animation speed.</summary>
    public const string WaveSpeed = "--flare-progress-wave-speed";
    /// <summary>CSS custom-property name for the circular wavy ring wave count.</summary>
    public const string RingWaves = "--flare-progress-ring-waves";
    /// <summary>CSS custom-property name for the circular wavy ring wave amplitude.</summary>
    public const string RingWaveAmplitude = "--flare-progress-ring-wave-amplitude";
}

/// <summary>CSS variable tokens for navigation (drawer/rail nav items and the active indicator).</summary>
public static class NavField
{
    /// <summary>CSS custom-property name for the active weight token.</summary>
    public const string ActiveWeight = "--flare-nav-active-weight";
    /// <summary>CSS custom-property name for the badge weight token.</summary>
    public const string BadgeWeight = "--flare-nav-badge-weight";
    /// <summary>CSS custom-property name for the rail label line height token.</summary>
    public const string RailLabelLineHeight = "--flare-nav-rail-label-line-height";
    /// <summary>CSS custom-property name for the nav item hover/focus radius token.</summary>
    public const string ItemRadius = "--flare-nav-item-radius";
    /// <summary>CSS custom-property name for the active-indicator radius token.</summary>
    public const string IndicatorRadius = "--flare-nav-indicator-radius";
    /// <summary>CSS custom-property name for the active-indicator background token.</summary>
    public const string ActiveIndicator = "--flare-nav-active-indicator";
    /// <summary>CSS custom-property name for the active item left-bar token (an optional left accent bar).</summary>
    public const string ActiveLeftBar = "--flare-nav-active-left-bar";
}

/// <summary>CSS variable tokens for switch field.</summary>
public static class SwitchField
{
    // Track radius, transition, disabled/pressed-layer and the sm/lg size variants are gone: switch.css
    // reuses the shared shape/motion/state scales and its own size classes. What remains is the per-state
    // geometry and colors the CSS reads.
    /// <summary>CSS custom-property name for the track width.</summary>
    public const string TrackWidth = "--flare-switch-track-width";
    /// <summary>CSS custom-property name for the track height.</summary>
    public const string TrackHeight = "--flare-switch-track-height";
    /// <summary>CSS custom-property name for the track background when off.</summary>
    public const string TrackOffBg = "--flare-switch-track-off-bg";
    /// <summary>CSS custom-property name for the track background when on.</summary>
    public const string TrackOnBg = "--flare-switch-track-on-bg";
    /// <summary>CSS custom-property name for the track border (shorthand) when off.</summary>
    public const string TrackBorder = "--flare-switch-track-border";
    /// <summary>CSS custom-property name for the track border color on hover.</summary>
    public const string TrackHoverBorderColor = "--flare-switch-track-hover-border-color";
    /// <summary>CSS custom-property name for the thumb diameter when off.</summary>
    public const string ThumbOffSize = "--flare-switch-thumb-off-size";
    /// <summary>CSS custom-property name for the thumb diameter when on.</summary>
    public const string ThumbOnSize = "--flare-switch-thumb-on-size";
    /// <summary>CSS custom-property name for the thumb diameter when pressed and off.</summary>
    public const string ThumbPressedOffSize = "--flare-switch-thumb-pressed-off-size";
    /// <summary>CSS custom-property name for the thumb diameter when pressed and on.</summary>
    public const string ThumbPressedOnSize = "--flare-switch-thumb-pressed-on-size";
    /// <summary>CSS custom-property name for the thumb inline position when off.</summary>
    public const string ThumbOffLeft = "--flare-switch-thumb-off-left";
    /// <summary>CSS custom-property name for the thumb inline position when on.</summary>
    public const string ThumbOnLeft = "--flare-switch-thumb-on-left";
    /// <summary>CSS custom-property name for the thumb color when off.</summary>
    public const string ThumbOffColor = "--flare-switch-thumb-off-color";
    /// <summary>CSS custom-property name for the thumb color when on.</summary>
    public const string ThumbOnColor = "--flare-switch-thumb-on-color";
    /// <summary>CSS custom-property name for the thumb color in the off state layer.</summary>
    public const string ThumbStateOffColor = "--flare-switch-thumb-state-off-color";
    /// <summary>CSS custom-property name for the thumb color in the on state layer.</summary>
    public const string ThumbStateOnColor = "--flare-switch-thumb-state-on-color";
    /// <summary>CSS custom-property name for the thumb icon size.</summary>
    public const string IconSize = "--flare-switch-icon-size";
    /// <summary>CSS custom-property name for the thumb icon color when off.</summary>
    public const string IconOffColor = "--flare-switch-icon-off-color";
    /// <summary>CSS custom-property name for the thumb icon color when on.</summary>
    public const string IconOnColor = "--flare-switch-icon-on-color";
    /// <summary>CSS custom-property name for the focus outline (shorthand).</summary>
    public const string FocusOutline = "--flare-switch-focus-outline";
    /// <summary>CSS custom-property name for the focus outline offset.</summary>
    public const string FocusOutlineOffset = "--flare-switch-focus-outline-offset";
    /// <summary>CSS custom-property name for the focus shadow.</summary>
    public const string FocusShadow = "--flare-switch-focus-shadow";
    /// <summary>CSS custom-property name for the track background on off-hover.</summary>
    public const string TrackHoverOffBg = "--flare-switch-track-hover-off-bg";
    /// <summary>CSS custom-property name for the track background on on-hover.</summary>
    public const string TrackHoverOnBg = "--flare-switch-track-hover-on-bg";
    /// <summary>CSS custom-property name for the off-state hover state-layer shadow.</summary>
    public const string HoverShadowOff = "--flare-switch-hover-shadow-off";
    /// <summary>CSS custom-property name for the on-state hover state-layer shadow.</summary>
    public const string HoverShadowOn = "--flare-switch-hover-shadow-on";
    /// <summary>CSS custom-property name for the disabled track background.</summary>
    public const string DisabledTrackBg = "--flare-switch-disabled-track-bg";
    /// <summary>CSS custom-property name for the disabled track border color.</summary>
    public const string DisabledTrackBorder = "--flare-switch-disabled-track-border";
    /// <summary>CSS custom-property name for the disabled thumb (handle) background.</summary>
    public const string DisabledHandleBg = "--flare-switch-disabled-handle-bg";
}
