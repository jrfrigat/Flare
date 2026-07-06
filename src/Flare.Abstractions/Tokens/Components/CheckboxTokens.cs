using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCheckbox</c> (box, state-layer, focus ring).</summary>
public sealed record CheckboxTokens
{
    /// <summary>Size.</summary>
    [CssVar(Checkbox.Size)] public required string Size { get; init; }
    /// <summary>Border (outline) thickness of the box.</summary>
    [CssVar(Checkbox.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Corner radius of the box.</summary>
    [CssVar(Checkbox.Radius)] public required string Radius { get; init; }

    /// <summary>State-layer background on hover (unchecked). Set transparent to suppress the halo.</summary>
    [CssVar(Checkbox.StateLayerHover)] public required string StateLayerHover { get; init; }

    /// <summary>State-layer background on hover when checked (accent tint). Set transparent to suppress the halo.</summary>
    [CssVar(Checkbox.StateLayerHoverChecked)] public required string StateLayerHoverChecked { get; init; }

    /// <summary>Focus-visible outline shorthand.</summary>
    [CssVar(Checkbox.FocusOutline)] public required string FocusOutline { get; init; }

    /// <summary>Focus outline offset.</summary>
    [CssVar(Checkbox.FocusOutlineOffset)] public required string FocusOutlineOffset { get; init; }

    /// <summary>Extra focus shadow (e.g. an outer focus ring); set to none for no shadow.</summary>
    [CssVar(Checkbox.FocusShadow)] public required string FocusShadow { get; init; }
}
