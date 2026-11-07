using Flare.Css;
using static Flare.Css.Tokens.Vars;
using Names = Flare.Css.Tokens.Barcode.Vars;

namespace Flare.Components.Barcode;

/// <summary>Optional Barcode design overrides. Defaults follow the base theme; map with CssVarMap.FlattenTokens.</summary>
public sealed record BarcodeTokens
{
    /// <summary>Design override for <c>--flare-barcode-spacing-2</c>.</summary>
    [CssVar(Names.Spacing2)] public string Spacing2 { get; init; } = Var(Css.Tokens.Spacing.S2);
    /// <summary>Design override for <c>--flare-barcode-spacing-4</c>.</summary>
    [CssVar(Names.Spacing4)] public string Spacing4 { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Design override for <c>--flare-barcode-shape-small</c>.</summary>
    [CssVar(Names.ShapeSmall)] public string ShapeSmall { get; init; } = Var(Css.Tokens.Shape.Small);
    /// <summary>Design override for <c>--flare-barcode-typescale-label-medium-font</c>.</summary>
    [CssVar(Names.TypescaleLabelMediumFont)] public string TypescaleLabelMediumFont { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Font);
    /// <summary>Design override for <c>--flare-barcode-typescale-label-medium-size</c>.</summary>
    [CssVar(Names.TypescaleLabelMediumSize)] public string TypescaleLabelMediumSize { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Size);
    /// <summary>Design override for <c>--flare-barcode-typescale-label-medium-weight</c>.</summary>
    [CssVar(Names.TypescaleLabelMediumWeight)] public string TypescaleLabelMediumWeight { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Weight);
    /// <summary>Design override for <c>--flare-barcode-typescale-label-medium-height</c>.</summary>
    [CssVar(Names.TypescaleLabelMediumHeight)] public string TypescaleLabelMediumHeight { get; init; } = Var(Css.Tokens.Typography.LabelMedium.Height);
    /// <summary>Design override for <c>--flare-barcode-surface-container</c>.</summary>
    [CssVar(Names.SurfaceContainer)] public string SurfaceContainer { get; init; } = Var(Css.Tokens.Color.SurfaceContainer);
    /// <summary>Design override for <c>--flare-barcode-typescale-body-small-font</c>.</summary>
    [CssVar(Names.TypescaleBodySmallFont)] public string TypescaleBodySmallFont { get; init; } = Var(Css.Tokens.Typography.BodySmall.Font);
    /// <summary>Design override for <c>--flare-barcode-typescale-body-small-size</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSize)] public string TypescaleBodySmallSize { get; init; } = Var(Css.Tokens.Typography.BodySmall.Size);
    /// <summary>Design override for <c>--flare-barcode-on-surface-variant</c>.</summary>
    [CssVar(Names.OnSurfaceVariant)] public string OnSurfaceVariant { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Design override for <c>--flare-barcode-typescale-body-small-weight</c>.</summary>
    [CssVar(Names.TypescaleBodySmallWeight)] public string TypescaleBodySmallWeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Weight);
    /// <summary>Design override for <c>--flare-barcode-typescale-body-small-height</c>.</summary>
    [CssVar(Names.TypescaleBodySmallHeight)] public string TypescaleBodySmallHeight { get; init; } = Var(Css.Tokens.Typography.BodySmall.Height);
    /// <summary>Design override for <c>--flare-barcode-typescale-body-small-spacing</c>.</summary>
    [CssVar(Names.TypescaleBodySmallSpacing)] public string TypescaleBodySmallSpacing { get; init; } = Var(Css.Tokens.Typography.BodySmall.Spacing);
    /// <summary>Scan-safe bar color, independent of light or dark display mode.</summary>
    [CssVar(Css.Tokens.Barcode.Surface.Bar)] public string Bar { get; init; } = "#000";
    /// <summary>Scan-safe light ground behind the encoded bars.</summary>
    [CssVar(Css.Tokens.Barcode.Surface.Ground)] public string Ground { get; init; } = "#fff";
}
