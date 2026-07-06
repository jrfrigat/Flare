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
    /// <summary>CSS custom-property name for the surface color token.</summary>
    public const string SurfaceColor = "--flare-popover-surface-color";
    /// <summary>CSS custom-property name for the radius token.</summary>
    public const string Radius = "--flare-popover-radius";
    /// <summary>CSS custom-property name for the elevation token.</summary>
    public const string Elevation = "--flare-popover-elevation";
    /// <summary>CSS custom-property name for the padding token.</summary>
    public const string Padding = "--flare-popover-padding";
    /// <summary>CSS custom-property name for the min width token.</summary>
    public const string MinWidth = "--flare-popover-min-width";
    /// <summary>CSS custom-property name for the max width token.</summary>
    public const string MaxWidth = "--flare-popover-max-width";
    /// <summary>CSS custom-property name for the max height token.</summary>
    public const string MaxHeight = "--flare-popover-max-height";
    /// <summary>CSS custom-property name for the offset token.</summary>
    public const string Offset = "--flare-popover-offset";
    /// <summary>CSS custom-property name for the arrow size token.</summary>
    public const string ArrowSize = "--flare-popover-arrow-size";
    /// <summary>CSS custom-property name for the scrim color token.</summary>
    public const string ScrimColor = "--flare-popover-scrim-color";
    /// <summary>CSS custom-property name for the transition duration token.</summary>
    public const string TransitionDuration = "--flare-popover-transition-duration";
    /// <summary>CSS custom-property name for the transition easing token.</summary>
    public const string TransitionEasing = "--flare-popover-transition-easing";
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
    /// <summary>CSS custom-property name for the track color token.</summary>
    public const string TrackColor = "--flare-progress-track-color";
    /// <summary>CSS custom-property name for the indicator color token.</summary>
    public const string IndicatorColor = "--flare-progress-indicator-color";
    /// <summary>CSS custom-property name for the circular color token.</summary>
    public const string CircularColor = "--flare-progress-circular-color";
    /// <summary>CSS custom-property name for the circular track color token.</summary>
    public const string CircularTrackColor = "--flare-progress-circular-track-color";
    /// <summary>CSS custom-property name for the linear height token.</summary>
    public const string LinearHeight = "--flare-progress-linear-height";
    /// <summary>CSS custom-property name for the linear height sm token.</summary>
    public const string LinearHeightSm = "--flare-progress-linear-height-sm";
    /// <summary>CSS custom-property name for the linear height lg token.</summary>
    public const string LinearHeightLg = "--flare-progress-linear-height-lg";
    /// <summary>CSS custom-property name for the linear radius token.</summary>
    public const string LinearRadius = "--flare-progress-linear-radius";
    /// <summary>CSS custom-property name for the circular size token.</summary>
    public const string CircularSize = "--flare-progress-circular-size";
    /// <summary>CSS custom-property name for the circular size sm token.</summary>
    public const string CircularSizeSm = "--flare-progress-circular-size-sm";
    /// <summary>CSS custom-property name for the circular size lg token.</summary>
    public const string CircularSizeLg = "--flare-progress-circular-size-lg";
    /// <summary>CSS custom-property name for the circular stroke width token.</summary>
    public const string CircularStrokeWidth = "--flare-progress-circular-stroke-width";
    /// <summary>CSS custom-property name for the circular stroke width sm token.</summary>
    public const string CircularStrokeWidthSm = "--flare-progress-circular-stroke-width-sm";
    /// <summary>CSS custom-property name for the circular stroke width lg token.</summary>
    public const string CircularStrokeWidthLg = "--flare-progress-circular-stroke-width-lg";
    /// <summary>CSS custom-property name for the indeterminate duration token.</summary>
    public const string IndeterminateDuration = "--flare-progress-indeterminate-duration";
    /// <summary>CSS custom-property name for the indeterminate easing token.</summary>
    public const string IndeterminateEasing = "--flare-progress-indeterminate-easing";
    /// <summary>CSS custom-property name for the buffer color token.</summary>
    public const string BufferColor = "--flare-progress-buffer-color";
    /// <summary>CSS custom-property name for the wavy duration token.</summary>
    public const string WavyDuration = "--flare-progress-wavy-duration";

    // Linear/circular progress geometry the component actually reads (track/stop/wave),
    // distinct from the legacy linear-radius/circular-stroke-width names above.
    /// <summary>CSS custom-property name for the linear track radius token.</summary>
    public const string TrackRadius = "--flare-progress-track-radius";
    /// <summary>CSS custom-property name for the gap between track and indicator token.</summary>
    public const string Gap = "--flare-progress-gap";
    /// <summary>CSS custom-property name for the trailing stop-indicator size token.</summary>
    public const string StopSize = "--flare-progress-stop-size";
    /// <summary>CSS custom-property name for the trailing stop-indicator inset token.</summary>
    public const string StopInset = "--flare-progress-stop-inset";
    /// <summary>CSS custom-property name for the circular indicator stroke width token.</summary>
    public const string CircularWidth = "--flare-progress-circular-width";
    /// <summary>CSS custom-property name for the circular indicator line cap token.</summary>
    public const string CircularCap = "--flare-progress-circular-cap";
    /// <summary>CSS custom-property name for the circular indicator/track gap token.</summary>
    public const string CircularGap = "--flare-progress-circular-gap";
    /// <summary>CSS custom-property name for the wavy-progress enable flag token (1 = on).</summary>
    public const string WavyEnabled = "--flare-progress-wavy-enabled";
    /// <summary>CSS custom-property name for the wavy linear-track height token.</summary>
    public const string WavyHeight = "--flare-progress-wavy-height";
    /// <summary>CSS custom-property name for the wave length token.</summary>
    public const string WaveLength = "--flare-progress-wave-length";
    /// <summary>CSS custom-property name for the wave amplitude token.</summary>
    public const string WaveAmplitude = "--flare-progress-wave-amplitude";
    /// <summary>CSS custom-property name for the wave animation speed token.</summary>
    public const string WaveSpeed = "--flare-progress-wave-speed";
    /// <summary>CSS custom-property name for the circular wavy ring wave count token.</summary>
    public const string RingWaves = "--flare-progress-ring-waves";
    /// <summary>CSS custom-property name for the circular wavy ring wave amplitude token.</summary>
    public const string RingWaveAmplitude = "--flare-progress-ring-wave-amplitude";
}

/// <summary>CSS variable tokens for navigation (drawer/rail nav items and the active indicator).</summary>
public static class NavField
{
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
    /// <summary>CSS custom-property name for the track width token.</summary>
    public const string TrackWidth = "--flare-switch-track-width";
    /// <summary>CSS custom-property name for the track height token.</summary>
    public const string TrackHeight = "--flare-switch-track-height";
    /// <summary>CSS custom-property name for the track width sm token.</summary>
    public const string TrackWidthSm = "--flare-switch-track-width-sm";
    /// <summary>CSS custom-property name for the track height sm token.</summary>
    public const string TrackHeightSm = "--flare-switch-track-height-sm";
    /// <summary>CSS custom-property name for the track width lg token.</summary>
    public const string TrackWidthLg = "--flare-switch-track-width-lg";
    /// <summary>CSS custom-property name for the track height lg token.</summary>
    public const string TrackHeightLg = "--flare-switch-track-height-lg";
    /// <summary>CSS custom-property name for the track radius token.</summary>
    public const string TrackRadius = "--flare-switch-track-radius";
    /// <summary>CSS custom-property name for the track color token.</summary>
    public const string TrackColor = "--flare-switch-track-color";
    /// <summary>CSS custom-property name for the track border color token.</summary>
    public const string TrackBorderColor = "--flare-switch-track-border-color";
    /// <summary>CSS custom-property name for the track border width token.</summary>
    public const string TrackBorderWidth = "--flare-switch-track-border-width";
    /// <summary>CSS custom-property name for the track color selected token.</summary>
    public const string TrackColorSelected = "--flare-switch-track-color-selected";
    /// <summary>CSS custom-property name for the track border color selected token.</summary>
    public const string TrackBorderColorSelected = "--flare-switch-track-border-color-selected";
    /// <summary>CSS custom-property name for the thumb size token.</summary>
    public const string ThumbSize = "--flare-switch-thumb-size";
    /// <summary>CSS custom-property name for the thumb size sm token.</summary>
    public const string ThumbSizeSm = "--flare-switch-thumb-size-sm";
    /// <summary>CSS custom-property name for the thumb size lg token.</summary>
    public const string ThumbSizeLg = "--flare-switch-thumb-size-lg";
    /// <summary>CSS custom-property name for the thumb color token.</summary>
    public const string ThumbColor = "--flare-switch-thumb-color";
    /// <summary>CSS custom-property name for the thumb color selected token.</summary>
    public const string ThumbColorSelected = "--flare-switch-thumb-color-selected";
    /// <summary>CSS custom-property name for the thumb icon color token.</summary>
    public const string ThumbIconColor = "--flare-switch-thumb-icon-color";
    /// <summary>CSS custom-property name for the thumb icon color selected token.</summary>
    public const string ThumbIconColorSelected = "--flare-switch-thumb-icon-color-selected";
    /// <summary>CSS custom-property name for the thumb shadow token.</summary>
    public const string ThumbShadow = "--flare-switch-thumb-shadow";
    /// <summary>CSS custom-property name for the focus outline width token.</summary>
    public const string FocusOutlineWidth = "--flare-switch-focus-outline-width";
    /// <summary>CSS custom-property name for the focus outline color token.</summary>
    public const string FocusOutlineColor = "--flare-switch-focus-outline-color";
    /// <summary>CSS custom-property name for the focus outline offset token.</summary>
    public const string FocusOutlineOffset = "--flare-switch-focus-outline-offset";
    /// <summary>CSS custom-property name for the transition duration token.</summary>
    public const string TransitionDuration = "--flare-switch-transition-duration";
    /// <summary>CSS custom-property name for the transition easing token.</summary>
    public const string TransitionEasing = "--flare-switch-transition-easing";
    /// <summary>CSS custom-property name for the pressed layer color token.</summary>
    public const string PressedLayerColor = "--flare-switch-pressed-layer-color";
    /// <summary>CSS custom-property name for the pressed layer opacity token.</summary>
    public const string PressedLayerOpacity = "--flare-switch-pressed-layer-opacity";
    /// <summary>CSS custom-property name for the disabled opacity token.</summary>
    public const string DisabledOpacity = "--flare-switch-disabled-opacity";
}
