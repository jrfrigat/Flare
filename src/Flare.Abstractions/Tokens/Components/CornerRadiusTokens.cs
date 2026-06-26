namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-corner border-radius values (as CSS lengths) for a single component shape, used as a nested
/// field of component token records such as <c>ButtonTokens</c> and <c>SplitButtonTokens</c>.
/// </summary>
public sealed record CornerRadiusTokens
{
    /// <summary>Top-left corner radius as a CSS length (e.g. "1rem"). Defaults to "0px".</summary>
    public string TopLeft { get; init; } = "0px";
    /// <summary>Top-right corner radius as a CSS length (e.g. "1rem"). Defaults to "0px".</summary>
    public string TopRight { get; init; } = "0px";
    /// <summary>Bottom-right corner radius as a CSS length (e.g. "1rem"). Defaults to "0px".</summary>
    public string BottomRight { get; init; } = "0px";
    /// <summary>Bottom-left corner radius as a CSS length (e.g. "1rem"). Defaults to "0px".</summary>
    public string BottomLeft { get; init; } = "0px";

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
