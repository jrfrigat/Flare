using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>Box-shadow values for the elevation scale (levels 0-5).</summary>
public sealed record ElevationTokens
{
    /// <summary>Level 0 token.</summary>
    [CssVar(Elevation.Level0)] public required string Level0 { get; init; }
    /// <summary>Level 1 token.</summary>
    [CssVar(Elevation.Level1)] public required string Level1 { get; init; }
    /// <summary>Level 2 token.</summary>
    [CssVar(Elevation.Level2)] public required string Level2 { get; init; }
    /// <summary>Level 3 token.</summary>
    [CssVar(Elevation.Level3)] public required string Level3 { get; init; }
    /// <summary>Level 4 token.</summary>
    [CssVar(Elevation.Level4)] public required string Level4 { get; init; }
    /// <summary>Level 5 token.</summary>
    [CssVar(Elevation.Level5)] public required string Level5 { get; init; }
}
