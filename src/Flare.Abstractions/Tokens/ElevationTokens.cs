namespace Flare.Abstractions.Tokens;

/// <summary>Box-shadow values for the elevation scale (levels 0-5).</summary>
public sealed record ElevationTokens
{
    /// <summary>Level 0 token.</summary>
    public required string Level0 { get; init; }
    /// <summary>Level 1 token.</summary>
    public required string Level1 { get; init; }
    /// <summary>Level 2 token.</summary>
    public required string Level2 { get; init; }
    /// <summary>Level 3 token.</summary>
    public required string Level3 { get; init; }
    /// <summary>Level 4 token.</summary>
    public required string Level4 { get; init; }
    /// <summary>Level 5 token.</summary>
    public required string Level5 { get; init; }
}
