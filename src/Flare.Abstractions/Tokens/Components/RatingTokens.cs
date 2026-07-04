using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareRating</c> (star glyph size, filled/empty colors, hover feedback).
/// Following the <c>SliderTokens</c> model, <see cref="Size"/> defaults (in the reference themes) to
/// the keyword <c>initial</c> so the component's per-size classes (Xs..Xl) drive the glyph size; a
/// theme can override it to a single uniform size. Star-to-star gap, label typography and padding
/// reuse the shared spacing/typescale tokens rather than inventing per-component names.
/// </summary>
public sealed record RatingTokens
{
    /// <summary>Star glyph size. <c>initial</c> = size-class driven (1..2.5rem); a theme may pin one size.</summary>
    [CssVar(Rating.Size)] public required string Size { get; init; }

    /// <summary>Color of an unfilled (empty) star.</summary>
    [CssVar(Rating.EmptyColor)] public required string EmptyColor { get; init; }

    /// <summary>Default color of a filled star (the per-instance <c>Color</c> parameter overrides it).</summary>
    [CssVar(Rating.FilledColor)] public required string FilledColor { get; init; }

    /// <summary>Scale applied to a star on hover (interactive feedback).</summary>
    [CssVar(Rating.HoverScale)] public required string HoverScale { get; init; }
}
