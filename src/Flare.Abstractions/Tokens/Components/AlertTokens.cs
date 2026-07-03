using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareAlert</c>.</summary>
public sealed record AlertTokens
{
    /// <summary>Border radius of the alert container. Defaults to the shared small shape token so the
    /// alert follows the theme's shape scale instead of a hardcoded radius.</summary>
    [CssVar(Alert.Radius)] public string Radius { get; init; } = Vars.Var(Shape.Small);

    /// <summary>Border width for outlined and text variants (filled uses 0).</summary>
    [CssVar(Alert.BorderWidth)] public string BorderWidth { get; init; } = "1px";

    /// <summary>Internal padding of the alert container.</summary>
    [CssVar(Alert.Padding)] public string Padding { get; init; } = "0.875rem 1rem";

    /// <summary>Gap between icon and content.</summary>
    [CssVar(Alert.Gap)] public string Gap { get; init; } = "0.75rem";
}
