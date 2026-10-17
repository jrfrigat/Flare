using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Avatar GROUP (the overlapping stack and its "+N" overflow badge). Avatar-proper
/// color/size/radius/font are intentionally not tokens here - they come from the shared color-role, shape,
/// typescale and size-utility systems (see <see cref="AvatarField"/>).
/// </summary>
public sealed record AvatarTokens
{
    /// <summary>Overlap between stacked avatars (a negative inline margin).</summary>
    [CssVar(AvatarField.GroupSpacing)] public required string GroupSpacing { get; init; }

    /// <summary>Width of the ring drawn around each stacked avatar to separate it from its neighbour.</summary>
    [CssVar(AvatarField.GroupBorderWidth)] public string GroupBorderWidth { get; init; } = "var(--flare-border-width-emphasis)";

    /// <summary>Color of the ring drawn around each stacked avatar.</summary>
    [CssVar(AvatarField.GroupBorderColor)] public string GroupBorderColor { get; init; } = "var(--flare-color-surface)";

    /// <summary>Background color of the "+N" overflow badge.</summary>
    [CssVar(AvatarField.OverflowBg)] public string OverflowBg { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Text color of the "+N" overflow badge.</summary>
    [CssVar(AvatarField.OverflowColor)] public string OverflowColor { get; init; } = "var(--flare-color-on-surface-variant)";
}
