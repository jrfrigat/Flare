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

    /// <summary>Container behind a filled chip. A language that draws the chip as a grey pill states
    /// the wash here; one that treats it as a surface points this at a surface role.</summary>
    [CssVar(Chip.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Container behind an elevated chip - the one that carries a shadow instead of a border.</summary>
    [CssVar(Chip.ElevatedBg)] public required string ElevatedBg { get; init; }
}
