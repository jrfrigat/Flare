using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>Corner-radius values for the shape scale (none, extra-small .. full).</summary>
public sealed record ShapeTokens
{
    /// <summary>None token.</summary>
    [CssVar(Shape.None)] public required string None { get; init; }
    /// <summary>Extra small token.</summary>
    [CssVar(Shape.ExtraSmall)] public required string ExtraSmall { get; init; }
    /// <summary>Small token.</summary>
    [CssVar(Shape.Small)] public required string Small { get; init; }
    /// <summary>Medium token.</summary>
    [CssVar(Shape.Medium)] public required string Medium { get; init; }
    /// <summary>Large token.</summary>
    [CssVar(Shape.Large)] public required string Large { get; init; }
    /// <summary>Extra large token.</summary>
    [CssVar(Shape.ExtraLarge)] public required string ExtraLarge { get; init; }
    /// <summary>Full token.</summary>
    [CssVar(Shape.Full)] public required string Full { get; init; }
}
