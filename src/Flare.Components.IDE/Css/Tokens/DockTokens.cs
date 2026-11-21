using Flare.Css;
using static Flare.Css.Tokens.Vars;

namespace Flare.Components.IDE;

/// <summary>Optional dock overrides. Defaults follow the base theme; map with <c>CssVarMap.FlattenTokens</c>.</summary>
public sealed record DockTokens
{
    /// <summary>Design override for <c>--flare-dock-header-height</c>.</summary>
    [CssVar("--flare-dock-header-height")] public string HeaderHeight { get; init; } = Var(Css.Tokens.Spacing.S8);

    /// <summary>Design override for <c>--flare-dock-min-block-size</c>.</summary>
    [CssVar("--flare-dock-min-block-size")] public string MinBlockSize { get; init; } = Var(Css.Tokens.Spacing.S32);

    /// <summary>Design override for <c>--flare-dock-actions-max-size</c>.</summary>
    [CssVar("--flare-dock-actions-max-size")] public string ActionsMaxSize { get; init; } = Var(Css.Tokens.Spacing.S32);

    /// <summary>Gap between split branches. The geometry reads the same name as a theme custom token in pixels.</summary>
    [CssVar("--flare-dock-gutter")] public string Gutter { get; init; } = Var(Css.Tokens.Spacing.S3);

    /// <summary>Minimum size of a group. The geometry reads the same name as a theme custom token in pixels.</summary>
    [CssVar("--flare-dock-group-min")] public string GroupMin { get; init; } = Var(Css.Tokens.Spacing.S24);
}
