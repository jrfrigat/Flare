using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareRating</c> (star glyph size, filled/empty colors, hover feedback).
/// Following the <c>SliderTokens</c> model, the glyph size is ONE TOKEN PER SIZE: the theme emits all five
/// and the component's size class reads the matching one, so the ramp lives in the theme and the component
/// CSS carries no size of its own. A theme that wants one uniform size sets the same value five times.
/// Star-to-star gap, label typography and padding reuse the shared spacing/typescale tokens rather than
/// inventing per-component names.
/// </summary>
public sealed record RatingTokens
{
    /// <summary>Star glyph size at the xs size.</summary>
    [CssVar(Rating.Size.Xs)] public required string SizeXs { get; init; }
    /// <summary>Star glyph size at the sm size.</summary>
    [CssVar(Rating.Size.Sm)] public required string SizeSm { get; init; }
    /// <summary>Star glyph size at the md (default) size.</summary>
    [CssVar(Rating.Size.Md)] public required string SizeMd { get; init; }
    /// <summary>Star glyph size at the lg size.</summary>
    [CssVar(Rating.Size.Lg)] public required string SizeLg { get; init; }
    /// <summary>Star glyph size at the xl size.</summary>
    [CssVar(Rating.Size.Xl)] public required string SizeXl { get; init; }

    /// <summary>Color of an unfilled (empty) star.</summary>
    [CssVar(Rating.EmptyColor)] public required string EmptyColor { get; init; }

    /// <summary>Default color of a filled star (the per-instance <c>Color</c> parameter overrides it).</summary>
    [CssVar(Rating.FilledColor)] public required string FilledColor { get; init; }

    /// <summary>Scale applied to a star on hover (interactive feedback).</summary>
    [CssVar(Rating.HoverScale)] public required string HoverScale { get; init; }
}
