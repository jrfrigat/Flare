using Flare.Css;
using static Flare.Css.Tokens.Vars;

namespace Flare.Components.IDE;

/// <summary>Optional dock overrides. Defaults follow the base theme; map with <c>CssVarMap.FlattenTokens</c>.</summary>
public sealed record DockTokens
{
    /// <summary>Design override for <c>--flare-dock-header-height</c>. The <c>em</c> default follows the dock text size.</summary>
    [CssVar(DockTokenNames.HeaderHeight)] public string HeaderHeight { get; init; } = "2.5em";

    /// <summary>Design override for <c>--flare-dock-min-block-size</c>.</summary>
    [CssVar(DockTokenNames.MinBlockSize)] public string MinBlockSize { get; init; } = Var(Css.Tokens.Spacing.S32);

    /// <summary>Design override for <c>--flare-dock-actions-max-size</c>.</summary>
    [CssVar(DockTokenNames.ActionsMaxSize)] public string ActionsMaxSize { get; init; } = Var(Css.Tokens.Spacing.S32);

    /// <summary>Gap between split branches. Follows the shared splitter gutter unless the application overrides it;
    /// the geometry resolves it to pixels at runtime.</summary>
    [CssVar(DockTokenNames.Gutter)] public string Gutter { get; init; } = Var(Css.Tokens.Splitter.GutterSize);

    /// <summary>Minimum size of a group. The geometry resolves it to pixels at runtime.</summary>
    [CssVar(DockTokenNames.GroupMin)] public string GroupMin { get; init; } = Var(Css.Tokens.Spacing.S24);
}

// The package's token names, in the shape Flare.CssAudit reads: a constants class under Css/Tokens.
internal static class DockTokenNames
{
    internal const string HeaderHeight = "--flare-dock-header-height";
    internal const string MinBlockSize = "--flare-dock-min-block-size";
    internal const string ActionsMaxSize = "--flare-dock-actions-max-size";
    internal const string Gutter = "--flare-dock-gutter";
    internal const string GroupMin = "--flare-dock-group-min";
}
