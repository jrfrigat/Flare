namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareChip</c>.</summary>
public sealed record ChipTokens
{
    /// <summary>Corner radius. MD3 = 8dp (shape-small).</summary>
    public string Radius { get; init; } = "var(--flare-shape-small)";

    /// <summary>Minimum height. MD3 = 32dp.</summary>
    public string Height { get; init; } = "2rem";
}
