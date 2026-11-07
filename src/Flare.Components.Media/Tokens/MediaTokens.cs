using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Media.Vars;

namespace Flare.Components.Media;

/// <summary>Optional Media design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record MediaTokens
{
    /// <summary>Design override for <c>--flare-media-inverse-surface</c>.</summary>
    [CssVar(Names.InverseSurface)] public string InverseSurface { get; init; } = Var(Css.Tokens.Color.InverseSurface);
    /// <summary>Design override for <c>--flare-media-shape-medium</c>.</summary>
    [CssVar(Names.ShapeMedium)] public string ShapeMedium { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design override for <c>--flare-media-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-media-spacing-6</c>.</summary>
    [CssVar(Names.Spacing6)] public string Spacing6 { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-media-inverse-on-surface</c>.</summary>
    [CssVar(Names.InverseOnSurface)] public string InverseOnSurface { get; init; } = Var(Css.Tokens.Color.InverseOnSurface);
    /// <summary>Design override for <c>--flare-media-primary</c>.</summary>
    [CssVar(Names.Primary)] public string Primary { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-media-typescale-body-small-size</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSize)] public string TypescaleBodySmallSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-media-typescale-body-small-font</c>.</summary>
    [CssVar(Names.TypescaleBodySmallFont)] public string TypescaleBodySmallFont { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Design override for <c>--flare-media-typescale-body-small-weight</c>.</summary>
    [CssVar(Names.TypescaleBodySmallWeight)] public string TypescaleBodySmallWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Design override for <c>--flare-media-typescale-body-small-height</c>.</summary>
    [CssVar(Names.TypescaleBodySmallHeight)] public string TypescaleBodySmallHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Design override for <c>--flare-media-typescale-body-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSpacing)] public string TypescaleBodySmallSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
    /// <summary>Design override for <c>--flare-media-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-media-shape-extra-small</c>.</summary>
    [CssVar(Names.ShapeExtraSmall)] public string ShapeExtraSmall { get; init; } = Var(Css.Tokens.Shape.ExtraSmall);
    /// <summary>Design override for <c>--flare-media-motion-duration-state-change</c>.</summary>
    [CssVar(Names.MotionDurationStateChange)] public string MotionDurationStateChange { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);
    /// <summary>Design override for <c>--flare-media-surface-container</c>.</summary>
    [CssVar(Names.SurfaceContainer)] public string SurfaceContainer { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-media-on-surface</c>.</summary>
    [CssVar(Names.OnSurface)] public string OnSurface { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-media-border-width</c>.</summary>
    [CssVar(Names.BorderWidth)] public string BorderWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-media-border-style</c>.</summary>
    [CssVar(Names.BorderStyle)] public string BorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-media-outline-variant</c>.</summary>
    [CssVar(Names.OutlineVariant)] public string OutlineVariant { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-media-spacing-1</c>.</summary>
    [CssVar(Names.Spacing1)] public string Spacing1 { get; init; } = Var(Css.Tokens.Spacing.S1);
    /// <summary>Design override for <c>--flare-media-outline</c>.</summary>
    [CssVar(Names.Outline)] public string Outline { get; init; } = Var(Css.Tokens.Color.Outline);
    /// <summary>Design override for <c>--flare-media-shape-small</c>.</summary>
    [CssVar(Names.ShapeSmall)] public string ShapeSmall { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design override for <c>--flare-media-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
}
