namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-corner border-radius values (as CSS lengths) for a single component shape, used as a nested
/// field of component token records such as <c>ButtonTokens</c> and <c>SplitButtonTokens</c>.
/// </summary>
public sealed record CornerRadiusTokens
{
    /// <summary>Top-left corner radius as a CSS length (e.g. "1rem").</summary>
    public required string TopLeft { get; init; }
    /// <summary>Top-right corner radius as a CSS length (e.g. "1rem").</summary>
    public required string TopRight { get; init; }
    /// <summary>Bottom-right corner radius as a CSS length (e.g. "1rem").</summary>
    public required string BottomRight { get; init; }
    /// <summary>Bottom-left corner radius as a CSS length (e.g. "1rem").</summary>
    public required string BottomLeft { get; init; }

    /// <summary>Creates a <see cref="CornerRadiusTokens"/> with all four corners set to the same value.</summary>
    /// <param name="value">The CSS length applied to every corner.</param>
    /// <returns>A symmetric <see cref="CornerRadiusTokens"/>.</returns>
    public static CornerRadiusTokens All(string value) => new()
    {
        TopLeft = value,
        TopRight = value,
        BottomRight = value,
        BottomLeft = value
    };
}
