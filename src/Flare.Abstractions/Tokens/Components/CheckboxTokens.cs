using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCheckbox</c> (box, state-layer, focus ring).</summary>
public sealed record CheckboxTokens
{
    /// <summary>Border (outline) thickness of the box. MD3 = 2dp, Fluent = 1px.</summary>
    [CssVar(Checkbox.BorderWidth)] public string BorderWidth { get; init; } = "2px";

    /// <summary>Corner radius of the box. MD3 = 2dp, Fluent = 4dp.</summary>
    [CssVar(Checkbox.Radius)] public string Radius { get; init; } = "2px";

    /// <summary>State-layer background on hover (unchecked). Fluent suppresses (transparent).</summary>
    [CssVar(Checkbox.StateLayerHover)] public string StateLayerHover { get; init; } =
        "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)";

    /// <summary>State-layer background on hover when checked (primary tint). Fluent suppresses.</summary>
    [CssVar(Checkbox.StateLayerHoverChecked)] public string StateLayerHoverChecked { get; init; } =
        "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)";

    /// <summary>Focus-visible outline. MD3 = secondary 3dp (focus.indicator); Fluent = inner ring.</summary>
    [CssVar(Checkbox.FocusOutline)] public string FocusOutline { get; init; } = "3px solid var(--flare-color-secondary)";

    /// <summary>Focus outline offset.</summary>
    [CssVar(Checkbox.FocusOutlineOffset)] public string FocusOutlineOffset { get; init; } = "2px";

    /// <summary>Extra focus shadow (Fluent outer ring; MD3 = none).</summary>
    [CssVar(Checkbox.FocusShadow)] public string FocusShadow { get; init; } = "none";
}
