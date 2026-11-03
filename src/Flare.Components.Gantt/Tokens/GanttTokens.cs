using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Gantt.Vars;

namespace Flare.Components.Gantt;

/// <summary>Optional Gantt design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record GanttTokens
{
    /// <summary>Design override for <c>--flare-gantt-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-gantt-on-surface</c>.</summary>
    [CssVar(Names.OnSurface)] public string OnSurface { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-gantt-radius</c>.</summary>
    [CssVar(Names.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design override for <c>--flare-gantt-font-size</c>.</summary>
    [CssVar(Names.FontSize)] public string FontSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-gantt-line</c>.</summary>
    [CssVar(Names.Line)] public string Line { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-gantt-line-width</c>.</summary>
    [CssVar(Names.LineWidth)] public string LineWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-gantt-max-height</c>.</summary>
    [CssVar(Names.MaxHeight)] public string MaxHeight { get; init; } = "none";
    /// <summary>Design override for <c>--flare-gantt-header-bg</c>.</summary>
    [CssVar(Names.HeaderBg)] public string HeaderBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design override for <c>--flare-gantt-header-fg</c>.</summary>
    [CssVar(Names.HeaderFg)] public string HeaderFg { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-gantt-scale-height</c>.</summary>
    [CssVar(Names.ScaleHeight)] public string ScaleHeight { get; init; } = $"calc(var({Css.Tokens.Spacing.S16}) + var({Css.Tokens.Spacing.S2}))";
    /// <summary>Design override for <c>--flare-gantt-scale-major-height</c>.</summary>
    [CssVar(Names.ScaleMajorHeight)] public string ScaleMajorHeight { get; init; } = Var(Css.Tokens.Spacing.S12);
    /// <summary>Design override for <c>--flare-gantt-caption-opacity</c>.</summary>
    [CssVar(Names.CaptionOpacity)] public string CaptionOpacity { get; init; } = "0.75";
    /// <summary>Design override for <c>--flare-gantt-side-bg</c>.</summary>
    [CssVar(Names.SideBg)] public string SideBg { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-gantt-toggle-size</c>.</summary>
    [CssVar(Names.ToggleSize)] public string ToggleSize { get; init; } = Var(Css.Tokens.Spacing.S10);
    /// <summary>Design override for <c>--flare-gantt-indent</c>.</summary>
    [CssVar(Names.Indent)] public string Indent { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-gantt-hover-bg</c>.</summary>
    [CssVar(Names.HoverBg)] public string HoverBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-gantt-row-line</c>.</summary>
    [CssVar(Names.RowLine)] public string RowLine { get; init; } = Var(Names.Line);
    /// <summary>Design override for <c>--flare-gantt-grid-line</c>.</summary>
    [CssVar(Names.GridLine)] public string GridLine { get; init; } = Var(Names.Line);
    /// <summary>Design override for <c>--flare-gantt-grid-opacity</c>.</summary>
    [CssVar(Names.GridOpacity)] public string GridOpacity { get; init; } = "0.6";
    /// <summary>Design override for <c>--flare-gantt-nonworking-bg</c>.</summary>
    [CssVar(Names.NonworkingBg)] public string NonworkingBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-gantt-panel-gap</c>.</summary>
    [CssVar(Names.PanelGap)] public string PanelGap { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-gantt-panel-gap-color</c>.</summary>
    [CssVar(Names.PanelGapColor)] public string PanelGapColor { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-gantt-bar-bg</c>.</summary>
    [CssVar(Names.BarBg)] public string BarBg { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-gantt-bar-fg</c>.</summary>
    [CssVar(Names.BarFg)] public string BarFg { get; init; } = Var(Css.Tokens.Color.OnPrimary);
    /// <summary>Design override for <c>--flare-gantt-bar-border</c>.</summary>
    [CssVar(Names.BarBorder)] public string BarBorder { get; init; } = "transparent";
    /// <summary>Design override for <c>--flare-gantt-bar-radius</c>.</summary>
    [CssVar(Names.BarRadius)] public string BarRadius { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design override for <c>--flare-gantt-progress-bg</c>.</summary>
    [CssVar(Names.ProgressBg)] public string ProgressBg { get; init; } = $"color-mix(in srgb, var({Css.Tokens.Color.OnPrimary}) 22%, transparent)";
    /// <summary>Design override for <c>--flare-gantt-summary-bg</c>.</summary>
    [CssVar(Names.SummaryBg)] public string SummaryBg { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-gantt-summary-fg</c>.</summary>
    [CssVar(Names.SummaryFg)] public string SummaryFg { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-gantt-summary-radius</c>.</summary>
    [CssVar(Names.SummaryRadius)] public string SummaryRadius { get; init; } = Var(Css.Tokens.Shape.ExtraSmall);
    /// <summary>Design override for <c>--flare-gantt-selected</c>.</summary>
    [CssVar(Names.Selected)] public string Selected { get; init; } = Var(Css.Tokens.Color.Tertiary);
    /// <summary>Design override for <c>--flare-gantt-selected-width</c>.</summary>
    [CssVar(Names.SelectedWidth)] public string SelectedWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);
    /// <summary>Design override for <c>--flare-gantt-critical</c>.</summary>
    [CssVar(Names.Critical)] public string Critical { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Design override for <c>--flare-gantt-critical-width</c>.</summary>
    [CssVar(Names.CriticalWidth)] public string CriticalWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);
    /// <summary>Design override for <c>--flare-gantt-suggested</c>.</summary>
    [CssVar(Names.Suggested)] public string Suggested { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-gantt-suggested-fg</c>.</summary>
    [CssVar(Names.SuggestedFg)] public string SuggestedFg { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-gantt-suggested-width</c>.</summary>
    [CssVar(Names.SuggestedWidth)] public string SuggestedWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);
    /// <summary>Design override for <c>--flare-gantt-preview-opacity</c>.</summary>
    [CssVar(Names.PreviewOpacity)] public string PreviewOpacity { get; init; } = "0.7";
    /// <summary>Design override for <c>--flare-gantt-handle-width</c>.</summary>
    [CssVar(Names.HandleWidth)] public string HandleWidth { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Design override for <c>--flare-gantt-link-handle-size</c>.</summary>
    [CssVar(Names.LinkHandleSize)] public string LinkHandleSize { get; init; } = Var(Css.Tokens.Spacing.S5);
    /// <summary>Design override for <c>--flare-gantt-segment-bg</c>.</summary>
    [CssVar(Names.SegmentBg)] public string SegmentBg { get; init; } = "transparent";
    /// <summary>Design override for <c>--flare-gantt-segment-fg</c>.</summary>
    [CssVar(Names.SegmentFg)] public string SegmentFg { get; init; } = "inherit";
    /// <summary>Design override for <c>--flare-gantt-segment-border</c>.</summary>
    [CssVar(Names.SegmentBorder)] public string SegmentBorder { get; init; } = "transparent";
    /// <summary>Design override for <c>--flare-gantt-segment-border-style</c>.</summary>
    [CssVar(Names.SegmentBorderStyle)] public string SegmentBorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-gantt-region-bg</c>.</summary>
    [CssVar(Names.RegionBg)] public string RegionBg { get; init; } = $"color-mix(in srgb, var({Css.Tokens.Color.Primary}) 8%, transparent)";
    /// <summary>Design override for <c>--flare-gantt-region-border</c>.</summary>
    [CssVar(Names.RegionBorder)] public string RegionBorder { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-gantt-region-fg</c>.</summary>
    [CssVar(Names.RegionFg)] public string RegionFg { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-gantt-marker</c>.</summary>
    [CssVar(Names.Marker)] public string Marker { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Design override for <c>--flare-gantt-marker-fg</c>.</summary>
    [CssVar(Names.MarkerFg)] public string MarkerFg { get; init; } = Var(Css.Tokens.Color.OnError);
    /// <summary>Design override for <c>--flare-gantt-marker-width</c>.</summary>
    [CssVar(Names.MarkerWidth)] public string MarkerWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);
    /// <summary>Design override for <c>--flare-gantt-marker-style</c>.</summary>
    [CssVar(Names.MarkerStyle)] public string MarkerStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-gantt-cell-bg</c>.</summary>
    [CssVar(Names.CellBg)] public string CellBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design override for <c>--flare-gantt-cell-fg</c>.</summary>
    [CssVar(Names.CellFg)] public string CellFg { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-gantt-link</c>.</summary>
    [CssVar(Names.Link)] public string Link { get; init; } = Var(Css.Tokens.Color.Outline);
    /// <summary>Design override for <c>--flare-gantt-link-width</c>.</summary>
    [CssVar(Names.LinkWidth)] public string LinkWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-gantt-link-dash</c>.</summary>
    [CssVar(Names.LinkDash)] public string LinkDash { get; init; } = "none";
    /// <summary>Design override for <c>--flare-gantt-drop-candidate</c>.</summary>
    [CssVar(Names.DropCandidate)] public string DropCandidate { get; init; } = $"color-mix(in srgb, var({Css.Tokens.Color.Primary}) 6%, transparent)";
    /// <summary>Design override for <c>--flare-gantt-drop-row</c>.</summary>
    [CssVar(Names.DropRow)] public string DropRow { get; init; } = $"color-mix(in srgb, var({Css.Tokens.Color.Primary}) 10%, transparent)";
    /// <summary>Design override for <c>--flare-gantt-drop-over</c>.</summary>
    [CssVar(Names.DropOver)] public string DropOver { get; init; } = $"color-mix(in srgb, var({Css.Tokens.Color.Primary}) 22%, transparent)";
    /// <summary>Design override for <c>--flare-gantt-drop-outline</c>.</summary>
    [CssVar(Names.DropOutline)] public string DropOutline { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-gantt-legend-fg</c>.</summary>
    [CssVar(Names.LegendFg)] public string LegendFg { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-gantt-legend-font-size</c>.</summary>
    [CssVar(Names.LegendFontSize)] public string LegendFontSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-gantt-legend-gap</c>.</summary>
    [CssVar(Names.LegendGap)] public string LegendGap { get; init; } = Var(Css.Tokens.Spacing.S5);
    /// <summary>Design override for <c>--flare-gantt-legend-sample-width</c>.</summary>
    [CssVar(Names.LegendSampleWidth)] public string LegendSampleWidth { get; init; } = Var(Css.Tokens.Spacing.S10);
    /// <summary>Design override for <c>--flare-gantt-legend-sample-height</c>.</summary>
    [CssVar(Names.LegendSampleHeight)] public string LegendSampleHeight { get; init; } = Var(Css.Tokens.Spacing.S5);
    /// <summary>Design override for <c>--flare-gantt-legend-sample-border</c>.</summary>
    [CssVar(Names.LegendSampleBorder)] public string LegendSampleBorder { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-gantt-font</c>.</summary>
    [CssVar(Names.Font)] public string Font { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Design override for <c>--flare-gantt-font-weight</c>.</summary>
    [CssVar(Names.FontWeight)] public string FontWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Design override for <c>--flare-gantt-line-height</c>.</summary>
    [CssVar(Names.LineHeight)] public string LineHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Design override for <c>--flare-gantt-letter-spacing</c>.</summary>
    [CssVar(Names.LetterSpacing)] public string LetterSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
}
