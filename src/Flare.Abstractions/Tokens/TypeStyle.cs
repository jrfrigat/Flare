namespace Flare.Abstractions.Tokens;

/// <summary>One type-scale style: font family, size, weight, line height and letter spacing.</summary>
public sealed record TypeStyle
{
    /// <summary>Font family token.</summary>
    public required string FontFamily { get; init; }
    /// <summary>Font weight token.</summary>
    public required string FontWeight { get; init; }
    /// <summary>Font size token.</summary>
    public required string FontSize { get; init; }
    /// <summary>Line height token.</summary>
    public required string LineHeight { get; init; }
    /// <summary>Letter spacing token.</summary>
    public required string LetterSpacing { get; init; }
}
