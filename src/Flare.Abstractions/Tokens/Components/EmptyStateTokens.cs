using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareEmptyState</c>.</summary>
public sealed record EmptyStateTokens
{
    /// <summary>Opacity of the illustration icon. Neither theme family has a shared opacity scale, so the
    /// value stays a literal each theme states.</summary>
    [CssVar(EmptyState.IconOpacity)] public required string IconOpacity { get; init; }

    /// <summary>Upper bound for the description's measure. No base length scale measures a reading line, so
    /// the value stays a literal each theme states.</summary>
    [CssVar(EmptyState.DescriptionMaxWidth)] public required string DescriptionMaxWidth { get; init; }
}
