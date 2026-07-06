using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for breadcrumb - component-specific geometry read by breadcrumb.css.</summary>
public sealed record BreadcrumbTokens
{
    /// <summary>Link Hover Opacity.</summary>
    [CssVar(BreadcrumbField.LinkHoverOpacity)] public required string LinkHoverOpacity { get; init; }

    /// <summary>Separator Opacity.</summary>
    [CssVar(BreadcrumbField.SeparatorOpacity)] public required string SeparatorOpacity { get; init; }
}
