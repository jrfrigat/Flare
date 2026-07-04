using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCheckbox</c> (box, state-layer, focus ring).</summary>
public sealed record CheckboxTokens
{
    /// <summary>Border (outline) thickness of the box. MD3 = 2dp, Fluent = 1px.</summary>
    [CssVar(Checkbox.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Corner radius of the box. MD3 = 2dp, Fluent = 4dp.</summary>
    [CssVar(Checkbox.Radius)] public required string Radius { get; init; }

    /// <summary>State-layer background on hover (unchecked). Fluent suppresses (transparent).</summary>
    [CssVar(Checkbox.StateLayerHover)] public required string StateLayerHover { get; init; }

    /// <summary>State-layer background on hover when checked (primary tint). Fluent suppresses.</summary>
    [CssVar(Checkbox.StateLayerHoverChecked)] public required string StateLayerHoverChecked { get; init; }

    /// <summary>Focus-visible outline. MD3 = secondary 3dp (focus.indicator); Fluent = inner ring.</summary>
    [CssVar(Checkbox.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Focus outline offset.</summary>
    [CssVar(Checkbox.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Extra focus shadow (Fluent outer ring; MD3 = none).</summary>
    [CssVar(Checkbox.FocusShadow)] public required string FocusShadow { get; init; }
}
