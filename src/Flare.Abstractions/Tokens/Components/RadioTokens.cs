using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareRadio</c> (ring + state-layer).</summary>
public sealed record RadioTokens
{
    /// <summary>Diameter of the radio control itself, excluding the state-layer halo around it.</summary>
    [CssVar(Radio.Size)] public required string Size { get; init; }
    /// <summary>Background of the state-layer halo on hover while unselected.</summary>
    [CssVar(Radio.StateLayerHover)] public required string StateLayerHover { get; init; }

    /// <summary>State-layer background on hover when selected (accent tint).</summary>
    [CssVar(Radio.StateLayerHoverChecked)] public required string StateLayerHoverChecked { get; init; }
}
