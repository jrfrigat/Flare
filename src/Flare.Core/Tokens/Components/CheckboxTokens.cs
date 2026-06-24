namespace Flare.Core.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCheckbox</c> (box, state-layer, focus ring).</summary>
public sealed record CheckboxTokens
{
    /// <summary>Border (outline) thickness of the box. MD3 = 2dp, Fluent = 1px.</summary>
    public string BorderWidth { get; init; } = "2px";

    /// <summary>Corner radius of the box. MD3 = 2dp, Fluent = 4dp.</summary>
    public string Radius { get; init; } = "2px";

    /// <summary>State-layer background on hover (unchecked). Fluent suppresses (transparent).</summary>
    public string StateLayerHover { get; init; } =
        "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)";

    /// <summary>State-layer background on hover when checked (primary tint). Fluent suppresses.</summary>
    public string StateLayerHoverChecked { get; init; } =
        "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)";

    /// <summary>Focus-visible outline. MD3 = secondary 3dp (focus.indicator); Fluent = inner ring.</summary>
    public string FocusOutline { get; init; } = "3px solid var(--flare-color-secondary)";

    /// <summary>Focus outline offset.</summary>
    public string FocusOutlineOffset { get; init; } = "2px";

    /// <summary>Extra focus shadow (Fluent outer ring; MD3 = none).</summary>
    public string FocusShadow { get; init; } = "none";
}
