using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Gantt timeline (the Flare.Components.Gantt package): frame, axis, rows, bars, links, layers and
/// legend, read by gantt.css. A consumer restyles one kind of bar, segment, region, marker, cell or link by setting
/// the same variables on its <c>flare-gantt--kind-{kind}</c> class.
/// </summary>
public sealed record GanttTokens
{
    /// <summary>Background of the whole chart.</summary>
    [CssVar(GanttField.Surface)] public required string Surface { get; init; }

    /// <summary>Text color of the chart.</summary>
    [CssVar(GanttField.OnSurface)] public required string OnSurface { get; init; }

    /// <summary>Corner radius of the chart frame.</summary>
    [CssVar(GanttField.Radius)] public required string Radius { get; init; }

    /// <summary>Font size of the axis, row headers and bars.</summary>
    [CssVar(GanttField.FontSize)] public required string FontSize { get; init; }

    /// <summary>Color of the chart frame and dividing lines.</summary>
    [CssVar(GanttField.Line)] public required string Line { get; init; }

    /// <summary>Width of the chart frame and dividing lines.</summary>
    [CssVar(GanttField.LineWidth)] public required string LineWidth { get; init; }

    /// <summary>Tallest the chart scroll area gets before it scrolls; none lets it grow.</summary>
    [CssVar(GanttField.MaxHeight)] public required string MaxHeight { get; init; }

    /// <summary>Background of the axis header.</summary>
    [CssVar(GanttField.HeaderBg)] public required string HeaderBg { get; init; }

    /// <summary>Text color of the axis header.</summary>
    [CssVar(GanttField.HeaderFg)] public required string HeaderFg { get; init; }

    /// <summary>Height of the row of axis units.</summary>
    [CssVar(GanttField.ScaleHeight)] public required string ScaleHeight { get; init; }

    /// <summary>Height of the row of group captions above the units (months above days).</summary>
    [CssVar(GanttField.ScaleMajorHeight)] public required string ScaleMajorHeight { get; init; }

    /// <summary>Opacity of the secondary caption under an axis unit, such as the day of week.</summary>
    [CssVar(GanttField.CaptionOpacity)] public required string CaptionOpacity { get; init; }

    /// <summary>Background of the row-header column.</summary>
    [CssVar(GanttField.SideBg)] public required string SideBg { get; init; }

    /// <summary>Size of the expand and collapse toggle of a tree row.</summary>
    [CssVar(GanttField.ToggleSize)] public required string ToggleSize { get; init; }

    /// <summary>Indentation of each tree level in the row-header column.</summary>
    [CssVar(GanttField.Indent)] public required string Indent { get; init; }

    /// <summary>Background of a row under the pointer.</summary>
    [CssVar(GanttField.HoverBg)] public required string HoverBg { get; init; }

    /// <summary>Color of the line between rows.</summary>
    [CssVar(GanttField.RowLine)] public required string RowLine { get; init; }

    /// <summary>Color of the vertical line at every axis unit.</summary>
    [CssVar(GanttField.GridLine)] public required string GridLine { get; init; }

    /// <summary>Opacity of the vertical unit lines.</summary>
    [CssVar(GanttField.GridOpacity)] public required string GridOpacity { get; init; }

    /// <summary>Background of non-working time, such as weekends.</summary>
    [CssVar(GanttField.NonWorkingBg)] public required string NonWorkingBg { get; init; }

    /// <summary>Space between two panels on one axis.</summary>
    [CssVar(GanttField.PanelGap)] public required string PanelGap { get; init; }

    /// <summary>Color of the space between two panels.</summary>
    [CssVar(GanttField.PanelGapColor)] public required string PanelGapColor { get; init; }

    /// <summary>Background of an item bar.</summary>
    [CssVar(GanttField.BarBg)] public required string BarBg { get; init; }

    /// <summary>Text color inside a bar.</summary>
    [CssVar(GanttField.BarFg)] public required string BarFg { get; init; }

    /// <summary>Border color of a bar.</summary>
    [CssVar(GanttField.BarBorder)] public required string BarBorder { get; init; }

    /// <summary>Corner radius of a bar.</summary>
    [CssVar(GanttField.BarRadius)] public required string BarRadius { get; init; }

    /// <summary>Fill that shows how much of an item is done.</summary>
    [CssVar(GanttField.ProgressBg)] public required string ProgressBg { get; init; }

    /// <summary>Background of a summary bar over the descendants of a parent row.</summary>
    [CssVar(GanttField.SummaryBg)] public required string SummaryBg { get; init; }

    /// <summary>Text color inside a summary bar.</summary>
    [CssVar(GanttField.SummaryFg)] public required string SummaryFg { get; init; }

    /// <summary>Corner radius of a summary bar.</summary>
    [CssVar(GanttField.SummaryRadius)] public required string SummaryRadius { get; init; }

    /// <summary>Color of the ring around the selected bar.</summary>
    [CssVar(GanttField.Selected)] public required string Selected { get; init; }

    /// <summary>Width of the ring around the selected bar.</summary>
    [CssVar(GanttField.SelectedWidth)] public required string SelectedWidth { get; init; }

    /// <summary>Color of bars and links on the critical path.</summary>
    [CssVar(GanttField.Critical)] public required string Critical { get; init; }

    /// <summary>Border width of a bar on the critical path.</summary>
    [CssVar(GanttField.CriticalWidth)] public required string CriticalWidth { get; init; }

    /// <summary>Dashed border color of a suggested (ghost) bar.</summary>
    [CssVar(GanttField.Suggested)] public required string Suggested { get; init; }

    /// <summary>Text color of a suggested bar.</summary>
    [CssVar(GanttField.SuggestedFg)] public required string SuggestedFg { get; init; }

    /// <summary>Dashed border width of a suggested bar.</summary>
    [CssVar(GanttField.SuggestedWidth)] public required string SuggestedWidth { get; init; }

    /// <summary>Opacity of a bar while it is being dragged.</summary>
    [CssVar(GanttField.PreviewOpacity)] public required string PreviewOpacity { get; init; }

    /// <summary>Width of the grips at the start and end of a bar that resize it.</summary>
    [CssVar(GanttField.HandleWidth)] public required string HandleWidth { get; init; }

    /// <summary>Size of the grip a link is drawn from.</summary>
    [CssVar(GanttField.LinkHandleSize)] public required string LinkHandleSize { get; init; }

    /// <summary>Color of the keyboard focus ring on a bar.</summary>
    [CssVar(GanttField.Focus)] public required string Focus { get; init; }

    /// <summary>Width of the keyboard focus ring on a bar.</summary>
    [CssVar(GanttField.FocusWidth)] public required string FocusWidth { get; init; }

    /// <summary>Distance of the focus ring from the bar.</summary>
    [CssVar(GanttField.FocusOffset)] public required string FocusOffset { get; init; }

    /// <summary>Background of a segment of a bar, such as waiting time.</summary>
    [CssVar(GanttField.SegmentBg)] public required string SegmentBg { get; init; }

    /// <summary>Text color of the label of a segment.</summary>
    [CssVar(GanttField.SegmentFg)] public required string SegmentFg { get; init; }

    /// <summary>Border color of a segment.</summary>
    [CssVar(GanttField.SegmentBorder)] public required string SegmentBorder { get; init; }

    /// <summary>Border style of a segment, such as dashed.</summary>
    [CssVar(GanttField.SegmentBorderStyle)] public required string SegmentBorderStyle { get; init; }

    /// <summary>Background of a highlighted range, such as a sprint.</summary>
    [CssVar(GanttField.RegionBg)] public required string RegionBg { get; init; }

    /// <summary>Color of the edges of a highlighted range.</summary>
    [CssVar(GanttField.RegionBorder)] public required string RegionBorder { get; init; }

    /// <summary>Text color of the caption of a highlighted range.</summary>
    [CssVar(GanttField.RegionFg)] public required string RegionFg { get; init; }

    /// <summary>Color of a marker line, such as today or a deadline.</summary>
    [CssVar(GanttField.Marker)] public required string Marker { get; init; }

    /// <summary>Text color of the caption of a marker.</summary>
    [CssVar(GanttField.MarkerFg)] public required string MarkerFg { get; init; }

    /// <summary>Width of a marker line.</summary>
    [CssVar(GanttField.MarkerWidth)] public required string MarkerWidth { get; init; }

    /// <summary>Line style of a marker, such as dashed.</summary>
    [CssVar(GanttField.MarkerStyle)] public required string MarkerStyle { get; init; }

    /// <summary>Background of a value cell above the lanes of a row.</summary>
    [CssVar(GanttField.CellBg)] public required string CellBg { get; init; }

    /// <summary>Text color of a value cell.</summary>
    [CssVar(GanttField.CellFg)] public required string CellFg { get; init; }

    /// <summary>Color of a dependency connector and its arrow.</summary>
    [CssVar(GanttField.Link)] public required string Link { get; init; }

    /// <summary>Width of a dependency connector.</summary>
    [CssVar(GanttField.LinkWidth)] public required string LinkWidth { get; init; }

    /// <summary>Dash pattern of a dependency connector; none draws it solid.</summary>
    [CssVar(GanttField.LinkDash)] public required string LinkDash { get; init; }

    /// <summary>Tint of every row that can take a dragged payload.</summary>
    [CssVar(GanttField.DropCandidate)] public required string DropCandidate { get; init; }

    /// <summary>Tint of the row under a dragged payload.</summary>
    [CssVar(GanttField.DropRow)] public required string DropRow { get; init; }

    /// <summary>Tint of the axis unit under a dragged payload.</summary>
    [CssVar(GanttField.DropOver)] public required string DropOver { get; init; }

    /// <summary>Outline of a bar that can take a dragged payload.</summary>
    [CssVar(GanttField.DropOutline)] public required string DropOutline { get; init; }

    /// <summary>Text color of the legend.</summary>
    [CssVar(GanttField.LegendFg)] public required string LegendFg { get; init; }

    /// <summary>Font size of the legend.</summary>
    [CssVar(GanttField.LegendFontSize)] public required string LegendFontSize { get; init; }

    /// <summary>Space between legend entries.</summary>
    [CssVar(GanttField.LegendGap)] public required string LegendGap { get; init; }

    /// <summary>Width of a legend sample.</summary>
    [CssVar(GanttField.LegendSampleWidth)] public required string LegendSampleWidth { get; init; }

    /// <summary>Height of a legend sample.</summary>
    [CssVar(GanttField.LegendSampleHeight)] public required string LegendSampleHeight { get; init; }

    /// <summary>Border color of a legend sample.</summary>
    [CssVar(GanttField.LegendSampleBorder)] public required string LegendSampleBorder { get; init; }
}
