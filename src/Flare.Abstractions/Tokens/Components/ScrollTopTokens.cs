using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for scroll - component-specific geometry read by scroll.css.</summary>
public sealed record ScrollTopTokens
{
    /// <summary>Top Inset.</summary>
    [CssVar(ScrollTopField.TopInset)] public required string TopInset { get; init; }

    /// <summary>Top Size.</summary>
    [CssVar(ScrollTopField.TopSize)] public required string TopSize { get; init; }
}
