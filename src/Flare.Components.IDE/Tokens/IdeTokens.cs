using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Ide.Vars;

namespace Flare.Components.IDE;

/// <summary>Optional Ide design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record IdeTokens
{
    /// <summary>Design override for <c>--flare-ide-border-width</c>.</summary>
    [CssVar(Names.BorderWidth)] public string BorderWidth { get; init; } = Var(Css.Tokens.Border.Width);
    /// <summary>Design override for <c>--flare-ide-border-style</c>.</summary>
    [CssVar(Names.BorderStyle)] public string BorderStyle { get; init; } = Var(Css.Tokens.Border.Style);
    /// <summary>Design override for <c>--flare-ide-outline-variant</c>.</summary>
    [CssVar(Names.OutlineVariant)] public string OutlineVariant { get; init; } = Var(Css.Tokens.Color.OutlineVariant);
    /// <summary>Design override for <c>--flare-ide-surface</c>.</summary>
    [CssVar(Names.Surface)] public string Surface { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Design override for <c>--flare-ide-elevation-3</c>.</summary>
    [CssVar(Names.Elevation3)] public string Elevation3 { get; init; } = Var(Css.Tokens.Elevation.Level3);
    /// <summary>Design override for <c>--flare-ide-spacing-3</c>.</summary>
    [CssVar(Names.Spacing3)] public string Spacing3 { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Design override for <c>--flare-ide-motion-duration-state-change</c>.</summary>
    [CssVar(Names.MotionDurationStateChange)] public string MotionDurationStateChange { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);
    /// <summary>Design override for <c>--flare-ide-motion-easing-standard</c>.</summary>
    [CssVar(Names.MotionEasingStandard)] public string MotionEasingStandard { get; init; } = Var(Css.Tokens.Motion.EasingStandard);
    /// <summary>Design override for <c>--flare-ide-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-ide-on-surface</c>.</summary>
    [CssVar(Names.OnSurface)] public string OnSurface { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Design override for <c>--flare-ide-state-hover-opacity</c>.</summary>
    [CssVar(Names.StateHoverOpacity)] public string StateHoverOpacity { get; init; } = Var(Css.Tokens.State.HoverOpacity);
    /// <summary>Design override for <c>--flare-ide-typescale-body-small-size</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSize)] public string TypescaleBodySmallSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-ide-on-surface-variant</c>.</summary>
    [CssVar(Names.OnSurfaceVariant)] public string OnSurfaceVariant { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-ide-typescale-body-small-font</c>.</summary>
    [CssVar(Names.TypescaleBodySmallFont)] public string TypescaleBodySmallFont { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Design override for <c>--flare-ide-typescale-body-small-weight</c>.</summary>
    [CssVar(Names.TypescaleBodySmallWeight)] public string TypescaleBodySmallWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Design override for <c>--flare-ide-typescale-body-small-height</c>.</summary>
    [CssVar(Names.TypescaleBodySmallHeight)] public string TypescaleBodySmallHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Design override for <c>--flare-ide-typescale-body-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSpacing)] public string TypescaleBodySmallSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
    /// <summary>Design override for <c>--flare-ide-spacing-1</c>.</summary>
    [CssVar(Names.Spacing1)] public string Spacing1 { get; init; } = Var(Css.Tokens.Spacing.S1);
    /// <summary>Design override for <c>--flare-ide-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-ide-spacing-8</c>.</summary>
    [CssVar(Names.Spacing8)] public string Spacing8 { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Design override for <c>--flare-ide-typescale-body-medium-size</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumSize)] public string TypescaleBodyMediumSize { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Size);
    /// <summary>Design override for <c>--flare-ide-shape-extra-small</c>.</summary>
    [CssVar(Names.ShapeExtraSmall)] public string ShapeExtraSmall { get; init; } = Var(Css.Tokens.Shape.ExtraSmall);
    /// <summary>Design override for <c>--flare-ide-typescale-body-medium-font</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumFont)] public string TypescaleBodyMediumFont { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Font);
    /// <summary>Design override for <c>--flare-ide-typescale-body-medium-weight</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumWeight)] public string TypescaleBodyMediumWeight { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Weight);
    /// <summary>Design override for <c>--flare-ide-typescale-body-medium-height</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumHeight)] public string TypescaleBodyMediumHeight { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Height);
    /// <summary>Design override for <c>--flare-ide-typescale-body-medium-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodyMediumSpacing)] public string TypescaleBodyMediumSpacing { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Spacing);
    /// <summary>Design override for <c>--flare-ide-typescale-label-large-weight</c>.</summary>
    [CssVar(Names.TypescaleLabelLargeWeight)] public string TypescaleLabelLargeWeight { get; init; } = Var(Css.Tokens.Typography.LabelLarge.Weight);
    /// <summary>Design override for <c>--flare-ide-state-disabled-opacity</c>.</summary>
    [CssVar(Names.StateDisabledOpacity)] public string StateDisabledOpacity { get; init; } = Var(Css.Tokens.State.DisabledOpacity);
    /// <summary>Design override for <c>--flare-ide-typescale-label-small-size</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallSize)] public string TypescaleLabelSmallSize { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Size);
    /// <summary>Design override for <c>--flare-ide-typescale-label-small-font</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallFont)] public string TypescaleLabelSmallFont { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Font);
    /// <summary>Design override for <c>--flare-ide-typescale-label-small-weight</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallWeight)] public string TypescaleLabelSmallWeight { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Weight);
    /// <summary>Design override for <c>--flare-ide-typescale-label-small-height</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallHeight)] public string TypescaleLabelSmallHeight { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Height);
    /// <summary>Design override for <c>--flare-ide-typescale-label-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleLabelSmallSpacing)] public string TypescaleLabelSmallSpacing { get; init; } = Var(Css.Tokens.Typography.LabelSmall.Spacing);
    /// <summary>Design override for <c>--flare-ide-surface-variant</c>.</summary>
    [CssVar(Names.SurfaceVariant)] public string SurfaceVariant { get; init; } = Var(Css.Tokens.Color.SurfaceVariant);
    /// <summary>Design override for <c>--flare-ide-spacing-6</c>.</summary>
    [CssVar(Names.Spacing6)] public string Spacing6 { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Design override for <c>--flare-ide-primary</c>.</summary>
    [CssVar(Names.Primary)] public string Primary { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Design override for <c>--flare-ide-motion-duration-enter-small</c>.</summary>
    [CssVar(Names.MotionDurationEnterSmall)] public string MotionDurationEnterSmall { get; init; } = Var(Css.Tokens.Motion.DurationEnterSmall);
    /// <summary>Design override for <c>--flare-ide-state-pressed-opacity</c>.</summary>
    [CssVar(Names.StatePressedOpacity)] public string StatePressedOpacity { get; init; } = Var(Css.Tokens.State.PressedOpacity);
    /// <summary>Design override for <c>--flare-ide-touch-target-min</c>.</summary>
    [CssVar(Names.TouchTargetMin)] public string TouchTargetMin { get; init; } = Var(Css.Tokens.TouchField.TargetMin);
    /// <summary>Design override for <c>--flare-ide-shape-small</c>.</summary>
    [CssVar(Names.ShapeSmall)] public string ShapeSmall { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design override for <c>--flare-ide-elevation-2</c>.</summary>
    [CssVar(Names.Elevation2)] public string Elevation2 { get; init; } = Var(Css.Tokens.Elevation.Level2);
    /// <summary>Design override for <c>--flare-ide-on-primary</c>.</summary>
    [CssVar(Names.OnPrimary)] public string OnPrimary { get; init; } = Var(Css.Tokens.Color.OnPrimary);
    /// <summary>Design override for <c>--flare-ide-spacing-5</c>.</summary>
    [CssVar(Names.Spacing5)] public string Spacing5 { get; init; } = Var(Css.Tokens.Spacing.S5);
    /// <summary>Design override for <c>--flare-ide-spacing-12</c>.</summary>
    [CssVar(Names.Spacing12)] public string Spacing12 { get; init; } = Var(Css.Tokens.Spacing.S12);
    /// <summary>Design override for <c>--flare-ide-spacing-16</c>.</summary>
    [CssVar(Names.Spacing16)] public string Spacing16 { get; init; } = Var(Css.Tokens.Spacing.S16);
}
