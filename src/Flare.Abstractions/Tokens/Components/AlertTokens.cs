using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareAlert</c>.</summary>
public sealed record AlertTokens
{
    /// <summary>Body opacity.</summary>
    [CssVar(Alert.BodyOpacity)] public required string BodyOpacity { get; init; }
    /// <summary>Close opacity.</summary>
    [CssVar(Alert.CloseOpacity)] public required string CloseOpacity { get; init; }
    /// <summary>Border radius of the alert container (a theme typically maps this to its shape scale).</summary>
    [CssVar(Alert.Radius)] public required string Radius { get; init; }

    /// <summary>Border width for outlined and text variants (filled uses 0).</summary>
    [CssVar(Alert.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Internal padding of the alert container.</summary>
    [CssVar(Alert.Padding)] public required string Padding { get; init; }

    /// <summary>Gap between icon and content.</summary>
    [CssVar(Alert.Gap)] public required string Gap { get; init; }
}
