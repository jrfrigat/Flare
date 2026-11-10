using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Scheduler;

namespace Flare.Components.Scheduler;

/// <summary>Scheduler design values with semantic base-token defaults. Map with CssVarMap.FlattenTokens for
/// DesignTokens.Extended or the theme service; components inherit overrides through the normal theme pipeline.</summary>
public sealed record SchedulerTokens
{
    /// <summary>Maximum height of the resource timeline scroll area.</summary>
    [CssVar(Names.TimelineHeight)] public string TimelineHeight { get; init; } = $"calc({Var(Css.Tokens.Spacing.S32)} * 6)";
    /// <summary>Font family of scheduling captions.</summary>
    [CssVar(Names.Font)] public string Font { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Weight of scheduling captions.</summary>
    [CssVar(Names.FontWeight)] public string FontWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Line height of scheduling captions.</summary>
    [CssVar(Names.LineHeight)] public string LineHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Letter spacing of scheduling captions.</summary>
    [CssVar(Names.LetterSpacing)] public string LetterSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
    /// <summary>Design value for Surface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design value for Color; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.Color)] public string Color { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design value for Border; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.Border)] public string Border { get; init; } = Var(Css.Tokens.Border.Divider);
    /// <summary>Design value for Radius; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design value for Padding; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.Padding)] public string Padding { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design value for SlotHeight; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.SlotHeight)] public string SlotHeight { get; init; } = Var(Css.Tokens.Spacing.S16);
    /// <summary>Design value for DayMinWidth; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.DayMinWidth)] public string DayMinWidth { get; init; } = Var(Css.Tokens.Spacing.S32);
    /// <summary>Design value for SlotLine; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.SlotLine)] public string SlotLine { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design value for MajorLine; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.MajorLine)] public string MajorLine { get; init; } = Var(Css.Tokens.Color.Outline);
    /// <summary>Design value for HeaderSurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.HeaderSurface)] public string HeaderSurface { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design value for AllDaySurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.AllDaySurface)] public string AllDaySurface { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Minimum height of one all-day collision lane; content can grow at larger text sizes.</summary>
    [CssVar(Names.AllDayRowHeight)] public string AllDayRowHeight { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Design value for EventSurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventSurface)] public string EventSurface { get; init; } = Var(Css.Tokens.Color.PrimaryContainer);
    /// <summary>Design value for EventColor; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventColor)] public string EventColor { get; init; } = Var(Css.Tokens.Color.OnPrimaryContainer);
    /// <summary>Design value for EventBorder; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventBorder)] public string EventBorder { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design value for EventRadius; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventRadius)] public string EventRadius { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design value for EventPadding; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventPadding)] public string EventPadding { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design value for EventGap; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventGap)] public string EventGap { get; init; } = Var(Css.Tokens.Spacing.S1);
    /// <summary>Design value for EventHoverSurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.EventHoverSurface)] public string EventHoverSurface { get; init; } = Var(Css.Tokens.Color.SecondaryContainer);
    /// <summary>Design value for OutsideHoursSurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.OutsideHoursSurface)] public string OutsideHoursSurface { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design value for WeekendSurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.WeekendSurface)] public string WeekendSurface { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design value for TodaySurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.TodaySurface)] public string TodaySurface { get; init; } = Var(Css.Tokens.Color.SecondaryContainer);
    /// <summary>Design value for SelectedOutline; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.SelectedOutline)] public string SelectedOutline { get; init; } = Var(Css.Tokens.Vars.FocusRing);
    /// <summary>Design value for NowColor; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.NowColor)] public string NowColor { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Design value for NowWidth; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.NowWidth)] public string NowWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);
    /// <summary>Design value for FontSize; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.FontSize)] public string FontSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design value for ResourceHeaderSurface; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.ResourceHeaderSurface)] public string ResourceHeaderSurface { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design value for ResizeWidth; defaults to the corresponding base theme role.</summary>
    [CssVar(Names.ResizeWidth)] public string ResizeWidth { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Maximum height of one day cell in the month projection; content beyond it scrolls.</summary>
    [CssVar(Names.MonthCellMaxHeight)] public string MonthCellMaxHeight { get; init; } = $"calc({Var(Css.Tokens.Spacing.S32)} * 2)";
}

