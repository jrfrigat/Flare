using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Kanban.Vars;

namespace Flare.Components.Kanban;

/// <summary>Optional Kanban design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record KanbanTokens
{
    /// <summary>Elevation of a hovered card.</summary>
    [CssVar(Names.Elevation2)] public string Elevation2 { get; init; } = Var(Css.Tokens.Elevation.Level2);
    /// <summary>Keyboard focus outline of a card.</summary>
    [CssVar(Names.FocusRing)] public string FocusRing { get; init; } = Var(Css.Tokens.Vars.FocusRing);
    /// <summary>Design override for <c>--flare-kanban-spacing-6</c>.</summary>
    [CssVar(Names.Spacing6)] public string Spacing6 { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-kanban-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-kanban-spacing-32</c>.</summary>
    [CssVar(Names.Spacing32)] public string Spacing32 { get; init; } = Var(Css.Tokens.Spacing.S32);
    /// <summary>Design override for <c>--flare-kanban-surface-container-low</c>.</summary>
    [CssVar(Names.SurfaceContainerLow)] public string SurfaceContainerLow { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design override for <c>--flare-kanban-shape-medium</c>.</summary>
    [CssVar(Names.ShapeMedium)] public string ShapeMedium { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design override for <c>--flare-kanban-border-width</c>.</summary>
    [CssVar(Names.BorderWidth)] public string BorderWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-kanban-border-style</c>.</summary>
    [CssVar(Names.BorderStyle)] public string BorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-kanban-outline-variant</c>.</summary>
    [CssVar(Names.OutlineVariant)] public string OutlineVariant { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-kanban-spacing-5</c>.</summary>
    [CssVar(Names.Spacing5)] public string Spacing5 { get; init; } = Var(Css.Tokens.Spacing.S5);
    /// <summary>Design override for <c>--flare-kanban-surface-container</c>.</summary>
    [CssVar(Names.SurfaceContainer)] public string SurfaceContainer { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-kanban-spacing-10</c>.</summary>
    [CssVar(Names.Spacing10)] public string Spacing10 { get; init; } = Var(Css.Tokens.Spacing.S10);
    /// <summary>Design override for <c>--flare-kanban-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-kanban-shape-full</c>.</summary>
    [CssVar(Names.ShapeFull)] public string ShapeFull { get; init; } = Var(Css.Tokens.Shape.Full);
    /// <summary>Design override for <c>--flare-kanban-typescale-label-small-size</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallSize)] public string TypescaleLabelSmallSize { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Size);
    /// <summary>Design override for <c>--flare-kanban-on-surface-variant</c>.</summary>
    [CssVar(Names.OnSurfaceVariant)] public string OnSurfaceVariant { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-kanban-typescale-label-small-font</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallFont)] public string TypescaleLabelSmallFont { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Font);
    /// <summary>Design override for <c>--flare-kanban-typescale-label-small-weight</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallWeight)] public string TypescaleLabelSmallWeight { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Weight);
    /// <summary>Design override for <c>--flare-kanban-typescale-label-small-height</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallHeight)] public string TypescaleLabelSmallHeight { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Height);
    /// <summary>Design override for <c>--flare-kanban-typescale-label-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallSpacing)] public string TypescaleLabelSmallSpacing { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Spacing);
    /// <summary>Design override for <c>--flare-kanban-spacing-24</c>.</summary>
    [CssVar(Names.Spacing24)] public string Spacing24 { get; init; } = Var(Css.Tokens.Spacing.S24);
    /// <summary>Design override for <c>--flare-kanban-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-kanban-shape-small</c>.</summary>
    [CssVar(Names.ShapeSmall)] public string ShapeSmall { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design override for <c>--flare-kanban-motion-duration-state-change</c>.</summary>
    [CssVar(Names.MotionDurationStateChange)] public string MotionDurationStateChange { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);
    /// <summary>Design override for <c>--flare-kanban-primary</c>.</summary>
    [CssVar(Names.Primary)] public string Primary { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-kanban-spacing-1</c>.</summary>
    [CssVar(Names.Spacing1)] public string Spacing1 { get; init; } = Var(Css.Tokens.Spacing.S1);
    /// <summary>Design override for <c>--flare-kanban-spacing-3</c>.</summary>
    [CssVar(Names.Spacing3)] public string Spacing3 { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Design override for <c>--flare-kanban-secondary-container</c>.</summary>
    [CssVar(Names.SecondaryContainer)] public string SecondaryContainer { get; init; } = Var(Css.Tokens.Color.SecondaryContainer);
    /// <summary>Design override for <c>--flare-kanban-on-secondary-container</c>.</summary>
    [CssVar(Names.OnSecondaryContainer)] public string OnSecondaryContainer { get; init; } = Var(Css.Tokens.Color.OnSecondaryContainer);
}
