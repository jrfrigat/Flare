namespace Flare.Abstractions.Tokens;

/// <summary>Corner-radius values for the shape scale (none, extra-small .. full).</summary>
public sealed record ShapeTokens
{
    /// <summary>None token.</summary>
    public required string None { get; init; }
    /// <summary>Extra small token.</summary>
    public required string ExtraSmall { get; init; }
    /// <summary>Small token.</summary>
    public required string Small { get; init; }
    /// <summary>Medium token.</summary>
    public required string Medium { get; init; }
    /// <summary>Large token.</summary>
    public required string Large { get; init; }
    /// <summary>Extra large token.</summary>
    public required string ExtraLarge { get; init; }
    /// <summary>Full token.</summary>
    public required string Full { get; init; }
}
