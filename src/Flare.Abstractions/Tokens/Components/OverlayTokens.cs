using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme measurements for surfaces that have to fit the screen. One family rather than a number
/// per stylesheet: a dialog, a data-grid filter menu and a shortcuts panel are all answering "how much
/// of the screen may I take, and how much air stays at the edge", and a dense corporate theme and an
/// airy one should be able to answer it differently.
///
/// It was six literals in five rules before, and they spread precisely BECAUSE they were literals -
/// the 0.31.0 dialog fix took its <c>3rem</c> from the rule next to it, since copying a number is
/// easier than introducing a token.
/// </summary>
public sealed record OverlayTokens
{
    /// <summary>Air left between a full-screen surface and the viewport edge.</summary>
    [CssVar(Overlay.ViewportInset)] public required string ViewportInset { get; init; }

    /// <summary>The same air on a narrow screen, where there is less of it to spare.</summary>
    [CssVar(Overlay.ViewportInsetCompact)] public required string ViewportInsetCompact { get; init; }

    /// <summary>How much of the viewport a floating panel may grow to.</summary>
    [CssVar(Overlay.PanelMaxBlockSize)] public required string PanelMaxBlockSize { get; init; }
}
