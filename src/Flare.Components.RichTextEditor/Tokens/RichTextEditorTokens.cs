using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.RichTextEditor.Vars;

namespace Flare.Components.RichTextEditor;

/// <summary>Optional RichTextEditor design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record RichTextEditorTokens
{
    /// <summary>Design override for <c>--flare-rte-border-width</c>.</summary>
    [CssVar(Names.BorderWidth)] public string BorderWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-rte-border-style</c>.</summary>
    [CssVar(Names.BorderStyle)] public string BorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-rte-outline-variant</c>.</summary>
    [CssVar(Names.OutlineVariant)] public string OutlineVariant { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-rte-shape-medium</c>.</summary>
    [CssVar(Names.ShapeMedium)] public string ShapeMedium { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design override for <c>--flare-rte-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-rte-motion-duration-state-change</c>.</summary>
    [CssVar(Names.MotionDurationStateChange)] public string MotionDurationStateChange { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);
    /// <summary>Design override for <c>--flare-rte-primary</c>.</summary>
    [CssVar(Names.Primary)] public string Primary { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-rte-focus-ring</c>.</summary>
    [CssVar(Names.FocusRing)] public string FocusRing { get; init; } = Var(Css.Tokens.Vars.FocusRing);
    /// <summary>Design override for <c>--flare-rte-spacing-1</c>.</summary>
    [CssVar(Names.Spacing1)] public string Spacing1 { get; init; } = Var(Css.Tokens.Spacing.S1);
    /// <summary>Design override for <c>--flare-rte-spacing-3</c>.</summary>
    [CssVar(Names.Spacing3)] public string Spacing3 { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Design override for <c>--flare-rte-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-rte-surface-container-low</c>.</summary>
    [CssVar(Names.SurfaceContainerLow)] public string SurfaceContainerLow { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design override for <c>--flare-rte-spacing-16</c>.</summary>
    [CssVar(Names.Spacing16)] public string Spacing16 { get; init; } = Var(Css.Tokens.Spacing.S16);
    /// <summary>Design override for <c>--flare-rte-spacing-8</c>.</summary>
    [CssVar(Names.Spacing8)] public string Spacing8 { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Design override for <c>--flare-rte-shape-extra-small</c>.</summary>
    [CssVar(Names.ShapeExtraSmall)] public string ShapeExtraSmall { get; init; } = Var(Css.Tokens.Shape.ExtraSmall);
    /// <summary>Design override for <c>--flare-rte-on-surface</c>.</summary>
    [CssVar(Names.OnSurface)] public string OnSurface { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-rte-spacing-10</c>.</summary>
    [CssVar(Names.Spacing10)] public string Spacing10 { get; init; } = Var(Css.Tokens.Spacing.S10);
    /// <summary>Design override for <c>--flare-rte-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-rte-spacing-6</c>.</summary>
    [CssVar(Names.Spacing6)] public string Spacing6 { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-rte-typescale-body-large-font</c>.</summary>
    [CssVar(Names.TypescaleBodyLargeFont)] public string TypescaleBodyLargeFont { get; init; } = Var(Css.Tokens.Typography.BodyLarge.Font);
    /// <summary>Design override for <c>--flare-rte-typescale-body-large-size</c>.</summary>
    [CssVar(Names.TypescaleBodyLargeSize)] public string TypescaleBodyLargeSize { get; init; } = Var(Css.Tokens.Typography.BodyLarge.Size);
    /// <summary>Design override for <c>--flare-rte-typescale-body-large-height</c>.</summary>
    [CssVar(Names.TypescaleBodyLargeHeight)] public string TypescaleBodyLargeHeight { get; init; } = Var(Css.Tokens.Typography.BodyLarge.Height);
    /// <summary>Design override for <c>--flare-rte-typescale-body-large-weight</c>.</summary>
    [CssVar(Names.TypescaleBodyLargeWeight)] public string TypescaleBodyLargeWeight { get; init; } = Var(Css.Tokens.Typography.BodyLarge.Weight);
    /// <summary>Design override for <c>--flare-rte-typescale-body-large-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodyLargeSpacing)] public string TypescaleBodyLargeSpacing { get; init; } = Var(Css.Tokens.Typography.BodyLarge.Spacing);
    /// <summary>Design override for <c>--flare-rte-on-surface-variant</c>.</summary>
    [CssVar(Names.OnSurfaceVariant)] public string OnSurfaceVariant { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-rte-outline</c>.</summary>
    [CssVar(Names.Outline)] public string Outline { get; init; } = Var(Css.Tokens.Color.Outline);
    /// <summary>Design override for <c>--flare-rte-typescale-body-small-size</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSize)] public string TypescaleBodySmallSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-rte-typescale-body-small-font</c>.</summary>
    [CssVar(Names.TypescaleBodySmallFont)] public string TypescaleBodySmallFont { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Design override for <c>--flare-rte-typescale-body-small-weight</c>.</summary>
    [CssVar(Names.TypescaleBodySmallWeight)] public string TypescaleBodySmallWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Design override for <c>--flare-rte-typescale-body-small-height</c>.</summary>
    [CssVar(Names.TypescaleBodySmallHeight)] public string TypescaleBodySmallHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Design override for <c>--flare-rte-typescale-body-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSpacing)] public string TypescaleBodySmallSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
}
