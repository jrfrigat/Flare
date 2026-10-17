namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for the Gantt timeline (the Flare.Components.Gantt package).</summary>
public static class GanttField
{
    /// <summary>CSS custom-property name for the token: Background of the whole chart.</summary>
    public const string Surface = "--flare-gantt-surface";
    /// <summary>CSS custom-property name for the token: Text color of the chart.</summary>
    public const string OnSurface = "--flare-gantt-on-surface";
    /// <summary>CSS custom-property name for the token: Corner radius of the chart frame.</summary>
    public const string Radius = "--flare-gantt-radius";
    /// <summary>CSS custom-property name for the token: Font size of the axis, row headers and bars.</summary>
    public const string FontSize = "--flare-gantt-font-size";
    /// <summary>CSS custom-property name for the token: Color of the chart frame and dividing lines.</summary>
    public const string Line = "--flare-gantt-line";
    /// <summary>CSS custom-property name for the token: Width of the chart frame and dividing lines.</summary>
    public const string LineWidth = "--flare-gantt-line-width";
    /// <summary>CSS custom-property name for the token: Tallest the chart scroll area gets before it scrolls; none lets it grow.</summary>
    public const string MaxHeight = "--flare-gantt-max-height";
    /// <summary>CSS custom-property name for the token: Background of the axis header.</summary>
    public const string HeaderBg = "--flare-gantt-header-bg";
    /// <summary>CSS custom-property name for the token: Text color of the axis header.</summary>
    public const string HeaderFg = "--flare-gantt-header-fg";
    /// <summary>CSS custom-property name for the token: Height of the row of axis units.</summary>
    public const string ScaleHeight = "--flare-gantt-scale-height";
    /// <summary>CSS custom-property name for the token: Height of the row of group captions above the units (months above days).</summary>
    public const string ScaleMajorHeight = "--flare-gantt-scale-major-height";
    /// <summary>CSS custom-property name for the token: Opacity of the secondary caption under an axis unit, such as the day of week.</summary>
    public const string CaptionOpacity = "--flare-gantt-caption-opacity";
    /// <summary>CSS custom-property name for the token: Background of the row-header column.</summary>
    public const string SideBg = "--flare-gantt-side-bg";
    /// <summary>CSS custom-property name for the token: Size of the expand and collapse toggle of a tree row.</summary>
    public const string ToggleSize = "--flare-gantt-toggle-size";
    /// <summary>CSS custom-property name for the token: Indentation of each tree level in the row-header column.</summary>
    public const string Indent = "--flare-gantt-indent";
    /// <summary>CSS custom-property name for the token: Background of a row under the pointer.</summary>
    public const string HoverBg = "--flare-gantt-hover-bg";
    /// <summary>CSS custom-property name for the token: Color of the line between rows.</summary>
    public const string RowLine = "--flare-gantt-row-line";
    /// <summary>CSS custom-property name for the token: Color of the vertical line at every axis unit.</summary>
    public const string GridLine = "--flare-gantt-grid-line";
    /// <summary>CSS custom-property name for the token: Opacity of the vertical unit lines.</summary>
    public const string GridOpacity = "--flare-gantt-grid-opacity";
    /// <summary>CSS custom-property name for the token: Background of non-working time, such as weekends.</summary>
    public const string NonWorkingBg = "--flare-gantt-nonworking-bg";
    /// <summary>CSS custom-property name for the token: Space between two panels on one axis.</summary>
    public const string PanelGap = "--flare-gantt-panel-gap";
    /// <summary>CSS custom-property name for the token: Color of the space between two panels.</summary>
    public const string PanelGapColor = "--flare-gantt-panel-gap-color";
    /// <summary>CSS custom-property name for the token: Background of an item bar.</summary>
    public const string BarBg = "--flare-gantt-bar-bg";
    /// <summary>CSS custom-property name for the token: Text color inside a bar.</summary>
    public const string BarFg = "--flare-gantt-bar-fg";
    /// <summary>CSS custom-property name for the token: Border color of a bar.</summary>
    public const string BarBorder = "--flare-gantt-bar-border";
    /// <summary>CSS custom-property name for the token: Corner radius of a bar.</summary>
    public const string BarRadius = "--flare-gantt-bar-radius";
    /// <summary>CSS custom-property name for the token: Fill that shows how much of an item is done.</summary>
    public const string ProgressBg = "--flare-gantt-progress-bg";
    /// <summary>CSS custom-property name for the token: Background of a summary bar over the descendants of a parent row.</summary>
    public const string SummaryBg = "--flare-gantt-summary-bg";
    /// <summary>CSS custom-property name for the token: Text color inside a summary bar.</summary>
    public const string SummaryFg = "--flare-gantt-summary-fg";
    /// <summary>CSS custom-property name for the token: Corner radius of a summary bar.</summary>
    public const string SummaryRadius = "--flare-gantt-summary-radius";
    /// <summary>CSS custom-property name for the token: Color of the ring around the selected bar.</summary>
    public const string Selected = "--flare-gantt-selected";
    /// <summary>CSS custom-property name for the token: Width of the ring around the selected bar.</summary>
    public const string SelectedWidth = "--flare-gantt-selected-width";
    /// <summary>CSS custom-property name for the token: Color of bars and links on the critical path.</summary>
    public const string Critical = "--flare-gantt-critical";
    /// <summary>CSS custom-property name for the token: Border width of a bar on the critical path.</summary>
    public const string CriticalWidth = "--flare-gantt-critical-width";
    /// <summary>CSS custom-property name for the token: Dashed border color of a suggested (ghost) bar.</summary>
    public const string Suggested = "--flare-gantt-suggested";
    /// <summary>CSS custom-property name for the token: Text color of a suggested bar.</summary>
    public const string SuggestedFg = "--flare-gantt-suggested-fg";
    /// <summary>CSS custom-property name for the token: Dashed border width of a suggested bar.</summary>
    public const string SuggestedWidth = "--flare-gantt-suggested-width";
    /// <summary>CSS custom-property name for the token: Opacity of a bar while it is being dragged.</summary>
    public const string PreviewOpacity = "--flare-gantt-preview-opacity";
    /// <summary>CSS custom-property name for the token: Width of the grips at the start and end of a bar that resize it.</summary>
    public const string HandleWidth = "--flare-gantt-handle-width";
    /// <summary>CSS custom-property name for the token: Size of the grip a link is drawn from.</summary>
    public const string LinkHandleSize = "--flare-gantt-link-handle-size";
    /// <summary>CSS custom-property name for the token: Color of the keyboard focus ring on a bar.</summary>
    public const string Focus = "--flare-gantt-focus";
    /// <summary>CSS custom-property name for the token: Width of the keyboard focus ring on a bar.</summary>
    public const string FocusWidth = "--flare-gantt-focus-width";
    /// <summary>CSS custom-property name for the token: Distance of the focus ring from the bar.</summary>
    public const string FocusOffset = "--flare-gantt-focus-offset";
    /// <summary>CSS custom-property name for the token: Background of a segment of a bar, such as waiting time.</summary>
    public const string SegmentBg = "--flare-gantt-segment-bg";
    /// <summary>CSS custom-property name for the token: Text color of the label of a segment.</summary>
    public const string SegmentFg = "--flare-gantt-segment-fg";
    /// <summary>CSS custom-property name for the token: Border color of a segment.</summary>
    public const string SegmentBorder = "--flare-gantt-segment-border";
    /// <summary>CSS custom-property name for the token: Border style of a segment, such as dashed.</summary>
    public const string SegmentBorderStyle = "--flare-gantt-segment-border-style";
    /// <summary>CSS custom-property name for the token: Background of a highlighted range, such as a sprint.</summary>
    public const string RegionBg = "--flare-gantt-region-bg";
    /// <summary>CSS custom-property name for the token: Color of the edges of a highlighted range.</summary>
    public const string RegionBorder = "--flare-gantt-region-border";
    /// <summary>CSS custom-property name for the token: Text color of the caption of a highlighted range.</summary>
    public const string RegionFg = "--flare-gantt-region-fg";
    /// <summary>CSS custom-property name for the token: Color of a marker line, such as today or a deadline.</summary>
    public const string Marker = "--flare-gantt-marker";
    /// <summary>CSS custom-property name for the token: Text color of the caption of a marker.</summary>
    public const string MarkerFg = "--flare-gantt-marker-fg";
    /// <summary>CSS custom-property name for the token: Width of a marker line.</summary>
    public const string MarkerWidth = "--flare-gantt-marker-width";
    /// <summary>CSS custom-property name for the token: Line style of a marker, such as dashed.</summary>
    public const string MarkerStyle = "--flare-gantt-marker-style";
    /// <summary>CSS custom-property name for the token: Background of a value cell above the lanes of a row.</summary>
    public const string CellBg = "--flare-gantt-cell-bg";
    /// <summary>CSS custom-property name for the token: Text color of a value cell.</summary>
    public const string CellFg = "--flare-gantt-cell-fg";
    /// <summary>CSS custom-property name for the token: Color of a dependency connector and its arrow.</summary>
    public const string Link = "--flare-gantt-link";
    /// <summary>CSS custom-property name for the token: Width of a dependency connector.</summary>
    public const string LinkWidth = "--flare-gantt-link-width";
    /// <summary>CSS custom-property name for the token: Dash pattern of a dependency connector; none draws it solid.</summary>
    public const string LinkDash = "--flare-gantt-link-dash";
    /// <summary>CSS custom-property name for the token: Tint of every row that can take a dragged payload.</summary>
    public const string DropCandidate = "--flare-gantt-drop-candidate";
    /// <summary>CSS custom-property name for the token: Tint of the row under a dragged payload.</summary>
    public const string DropRow = "--flare-gantt-drop-row";
    /// <summary>CSS custom-property name for the token: Tint of the axis unit under a dragged payload.</summary>
    public const string DropOver = "--flare-gantt-drop-over";
    /// <summary>CSS custom-property name for the token: Outline of a bar that can take a dragged payload.</summary>
    public const string DropOutline = "--flare-gantt-drop-outline";
    /// <summary>CSS custom-property name for the token: Text color of the legend.</summary>
    public const string LegendFg = "--flare-gantt-legend-fg";
    /// <summary>CSS custom-property name for the token: Font size of the legend.</summary>
    public const string LegendFontSize = "--flare-gantt-legend-font-size";
    /// <summary>CSS custom-property name for the token: Space between legend entries.</summary>
    public const string LegendGap = "--flare-gantt-legend-gap";
    /// <summary>CSS custom-property name for the token: Width of a legend sample.</summary>
    public const string LegendSampleWidth = "--flare-gantt-legend-sample-width";
    /// <summary>CSS custom-property name for the token: Height of a legend sample.</summary>
    public const string LegendSampleHeight = "--flare-gantt-legend-sample-height";
    /// <summary>CSS custom-property name for the token: Border color of a legend sample.</summary>
    public const string LegendSampleBorder = "--flare-gantt-legend-sample-border";
}
