using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlarePagination</c> (page buttons, ellipsis and rows-per-page select).
/// Following the <c>SliderTokens</c> model, <see cref="Size"/> defaults (in the reference themes) to
/// <c>initial</c> so the component's size classes (Xs..Xl) drive the button square; a theme may pin
/// one size. Gap, typography and padding reuse the shared spacing/typescale tokens, and the hover
/// overlay reuses the shared <c>--flare-state-hover-opacity</c> rather than re-baking a percentage.
/// </summary>
public sealed record PaginationTokens
{
    /// <summary>Page-button square size (min-width and height). <c>initial</c> = size-class driven.</summary>
    [CssVar(Pagination.Size)] public required string Size { get; init; }

    /// <summary>Page-button / rows-select corner radius.</summary>
    [CssVar(Pagination.Radius)] public required string Radius { get; init; }

    /// <summary>Page-button / rows-select border color.</summary>
    [CssVar(Pagination.BorderColor)] public required string BorderColor { get; init; }

    /// <summary>Default active-page fill (the per-instance <c>Color</c> parameter overrides it).</summary>
    [CssVar(Pagination.ActiveColor)] public required string ActiveColor { get; init; }
}
