using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Query.Vars;

namespace Flare.Components.Query;

/// <summary>Optional Query design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record QueryTokens
{
    /// <summary>Design override for <c>--flare-query-spacing-6</c>.</summary>
    [CssVar(Names.Spacing6)] public string Spacing6 { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-query-on-surface</c>.</summary>
    [CssVar(Names.OnSurface)] public string OnSurface { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-query-typescale-body-medium-font</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumFont)] public string TypescaleBodyMediumFont { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Font);
    /// <summary>Design override for <c>--flare-query-typescale-body-medium-size</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumSize)] public string TypescaleBodyMediumSize { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Size);
    /// <summary>Design override for <c>--flare-query-typescale-body-medium-weight</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumWeight)] public string TypescaleBodyMediumWeight { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Weight);
    /// <summary>Design override for <c>--flare-query-typescale-body-medium-height</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumHeight)] public string TypescaleBodyMediumHeight { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Height);
    /// <summary>Design override for <c>--flare-query-typescale-body-medium-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumSpacing)] public string TypescaleBodyMediumSpacing { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Spacing);
    /// <summary>Design override for <c>--flare-query-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-query-surface-container-low</c>.</summary>
    [CssVar(Names.SurfaceContainerLow)] public string SurfaceContainerLow { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design override for <c>--flare-query-border-width</c>.</summary>
    [CssVar(Names.BorderWidth)] public string BorderWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-query-border-style</c>.</summary>
    [CssVar(Names.BorderStyle)] public string BorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-query-outline-variant</c>.</summary>
    [CssVar(Names.OutlineVariant)] public string OutlineVariant { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-query-shape-medium</c>.</summary>
    [CssVar(Names.ShapeMedium)] public string ShapeMedium { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design override for <c>--flare-query-typescale-title-small-font</c>.</summary>
    [CssVar(Names.TypescaleTitleSmallFont)] public string TypescaleTitleSmallFont { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Font);
    /// <summary>Design override for <c>--flare-query-typescale-title-small-size</c>.</summary>
    [CssVar(Names.TypescaleTitleSmallSize)] public string TypescaleTitleSmallSize { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Size);
    /// <summary>Design override for <c>--flare-query-typescale-title-small-weight</c>.</summary>
    [CssVar(Names.TypescaleTitleSmallWeight)] public string TypescaleTitleSmallWeight { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Weight);
    /// <summary>Design override for <c>--flare-query-on-surface-variant</c>.</summary>
    [CssVar(Names.OnSurfaceVariant)] public string OnSurfaceVariant { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-query-typescale-title-small-height</c>.</summary>
    [CssVar(Names.TypescaleTitleSmallHeight)] public string TypescaleTitleSmallHeight { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Height);
    /// <summary>Design override for <c>--flare-query-typescale-title-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleTitleSmallSpacing)] public string TypescaleTitleSmallSpacing { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Spacing);
    /// <summary>Design override for <c>--flare-query-spacing-3</c>.</summary>
    [CssVar(Names.Spacing3)] public string Spacing3 { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Design override for <c>--flare-query-shape-small</c>.</summary>
    [CssVar(Names.ShapeSmall)] public string ShapeSmall { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design override for <c>--flare-query-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-query-surface-container</c>.</summary>
    [CssVar(Names.SurfaceContainer)] public string SurfaceContainer { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-query-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-query-outline</c>.</summary>
    [CssVar(Names.Outline)] public string Outline { get; init; } = Var(Css.Tokens.Color.Outline);
    /// <summary>Design override for <c>--flare-query-shape-full</c>.</summary>
    [CssVar(Names.ShapeFull)] public string ShapeFull { get; init; } = Var(Css.Tokens.Shape.Full);
    /// <summary>Design override for <c>--flare-query-surface-container-high</c>.</summary>
    [CssVar(Names.SurfaceContainerHigh)] public string SurfaceContainerHigh { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHigh);
    /// <summary>Design override for <c>--flare-query-primary</c>.</summary>
    [CssVar(Names.Primary)] public string Primary { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-query-typescale-label-large-weight</c>.</summary>
    [CssVar(Names.TypescaleLabelLargeWeight)] public string TypescaleLabelLargeWeight { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Weight);
    /// <summary>Design override for <c>--flare-query-motion-duration-state-change</c>.</summary>
    [CssVar(Names.MotionDurationStateChange)] public string MotionDurationStateChange { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);
    /// <summary>Design override for <c>--flare-query-motion-easing-standard</c>.</summary>
    [CssVar(Names.MotionEasingStandard)] public string MotionEasingStandard { get; init; } = Var(Css.Tokens.Motion.EasingStandard);
    /// <summary>Design override for <c>--flare-query-surface-container-highest</c>.</summary>
    [CssVar(Names.SurfaceContainerHighest)] public string SurfaceContainerHighest { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHighest);
    /// <summary>Design override for <c>--flare-query-error</c>.</summary>
    [CssVar(Names.Error)] public string Error { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Design override for <c>--flare-query-spacing-8</c>.</summary>
    [CssVar(Names.Spacing8)] public string Spacing8 { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Design override for <c>--flare-query-spacing-16</c>.</summary>
    [CssVar(Names.Spacing16)] public string Spacing16 { get; init; } = Var(Css.Tokens.Spacing.S16);
    /// <summary>Design override for <c>--flare-query-typescale-body-small-size</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSize)] public string TypescaleBodySmallSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-query-typescale-body-small-font</c>.</summary>
    [CssVar(Names.TypescaleBodySmallFont)] public string TypescaleBodySmallFont { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Design override for <c>--flare-query-typescale-body-small-weight</c>.</summary>
    [CssVar(Names.TypescaleBodySmallWeight)] public string TypescaleBodySmallWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Design override for <c>--flare-query-typescale-body-small-height</c>.</summary>
    [CssVar(Names.TypescaleBodySmallHeight)] public string TypescaleBodySmallHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Design override for <c>--flare-query-typescale-body-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSpacing)] public string TypescaleBodySmallSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
}
