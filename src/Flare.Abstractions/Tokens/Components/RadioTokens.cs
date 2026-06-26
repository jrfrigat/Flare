namespace Flare.Core.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareRadio</c> (ring + state-layer).</summary>
public sealed record RadioTokens
{
    /// <summary>State-layer (40dp halo) background on hover (unselected). Fluent suppresses (transparent).</summary>
    public string StateLayerHover { get; init; } =
        "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)";

    /// <summary>State-layer background on hover when selected (primary tint). Fluent suppresses.</summary>
    public string StateLayerHoverChecked { get; init; } =
        "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)";
}
