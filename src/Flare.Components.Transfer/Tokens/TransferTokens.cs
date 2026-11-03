using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Transfer.Vars;

namespace Flare.Components.Transfer;

/// <summary>Optional Transfer design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record TransferTokens
{
    /// <summary>Design override for <c>--flare-transfer-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-transfer-border-width</c>.</summary>
    [CssVar(Names.BorderWidth)] public string BorderWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-transfer-border-style</c>.</summary>
    [CssVar(Names.BorderStyle)] public string BorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-transfer-outline-variant</c>.</summary>
    [CssVar(Names.OutlineVariant)] public string OutlineVariant { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-transfer-shape-medium</c>.</summary>
    [CssVar(Names.ShapeMedium)] public string ShapeMedium { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Design override for <c>--flare-transfer-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-transfer-spacing-6</c>.</summary>
    [CssVar(Names.Spacing6)] public string Spacing6 { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-transfer-surface-container-low</c>.</summary>
    [CssVar(Names.SurfaceContainerLow)] public string SurfaceContainerLow { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);
    /// <summary>Design override for <c>--flare-transfer-spacing-32</c>.</summary>
    [CssVar(Names.Spacing32)] public string Spacing32 { get; init; } = Var(Css.Tokens.Spacing.S32);
    /// <summary>Design override for <c>--flare-transfer-typescale-body-medium-size</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumSize)] public string TypescaleBodyMediumSize { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Size);
    /// <summary>Design override for <c>--flare-transfer-on-surface</c>.</summary>
    [CssVar(Names.OnSurface)] public string OnSurface { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-transfer-motion-duration-state-change</c>.</summary>
    [CssVar(Names.MotionDurationStateChange)] public string MotionDurationStateChange { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);
    /// <summary>Design override for <c>--flare-transfer-typescale-body-medium-font</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumFont)] public string TypescaleBodyMediumFont { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Font);
    /// <summary>Design override for <c>--flare-transfer-typescale-body-medium-weight</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumWeight)] public string TypescaleBodyMediumWeight { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Weight);
    /// <summary>Design override for <c>--flare-transfer-typescale-body-medium-height</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumHeight)] public string TypescaleBodyMediumHeight { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Height);
    /// <summary>Design override for <c>--flare-transfer-typescale-body-medium-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumSpacing)] public string TypescaleBodyMediumSpacing { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Spacing);
    /// <summary>Design override for <c>--flare-transfer-focus-ring</c>.</summary>
    [CssVar(Names.FocusRing)] public string FocusRing { get; init; } = Var(Css.Tokens.Vars.FocusRing);
    /// <summary>Design override for <c>--flare-transfer-primary</c>.</summary>
    [CssVar(Names.Primary)] public string Primary { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-transfer-spacing-3</c>.</summary>
    [CssVar(Names.Spacing3)] public string Spacing3 { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Design override for <c>--flare-transfer-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-transfer-touch-target-min</c>.</summary>
    [CssVar(Names.TouchTargetMin)] public string TouchTargetMin { get; init; } = Var(Css.Tokens.TouchField.TargetMin);
}
