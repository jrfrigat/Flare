using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareChip</c>.</summary>
public sealed record ChipTokens
{
    /// <summary>Corner radius of the chip container.</summary>
    [CssVar(Chip.Radius)] public required string Radius { get; init; }

    /// <summary>Minimum height of the chip.</summary>
    [CssVar(Chip.Height)] public required string Height { get; init; }
}
