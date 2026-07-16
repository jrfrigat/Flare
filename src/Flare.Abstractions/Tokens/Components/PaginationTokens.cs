using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlarePagination</c> (page buttons, ellipsis and rows-per-page select).
/// Following the <c>SliderTokens</c> model, the button square is ONE TOKEN PER SIZE: the theme emits all
/// five and the component's size class reads the matching one, so the ramp lives in the theme and the
/// component CSS carries no size of its own. A theme that wants one uniform size sets the same value five
/// times. Gap, typography and padding reuse the shared spacing/typescale tokens, and the hover overlay
/// reuses the shared <c>--flare-state-hover-opacity</c> rather than re-baking a percentage.
/// </summary>
public sealed record PaginationTokens
{
    /// <summary>Page-button square size (min-width and height) at the xs size.</summary>
    [CssVar(Pagination.Size.Xs)] public required string SizeXs { get; init; }
    /// <summary>Page-button square size at the sm size.</summary>
    [CssVar(Pagination.Size.Sm)] public required string SizeSm { get; init; }
    /// <summary>Page-button square size at the md (default) size.</summary>
    [CssVar(Pagination.Size.Md)] public required string SizeMd { get; init; }
    /// <summary>Page-button square size at the lg size.</summary>
    [CssVar(Pagination.Size.Lg)] public required string SizeLg { get; init; }
    /// <summary>Page-button square size at the xl size.</summary>
    [CssVar(Pagination.Size.Xl)] public required string SizeXl { get; init; }

    /// <summary>Page-button / rows-select corner radius.</summary>
    [CssVar(Pagination.Radius)] public required string Radius { get; init; }

    /// <summary>Page-button / rows-select border color.</summary>
    [CssVar(Pagination.BorderColor)] public required string BorderColor { get; init; }

    /// <summary>Default active-page fill (the per-instance <c>Color</c> parameter overrides it).</summary>
    [CssVar(Pagination.ActiveColor)] public required string ActiveColor { get; init; }
}
