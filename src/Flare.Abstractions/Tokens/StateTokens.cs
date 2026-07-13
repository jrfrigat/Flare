using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>State-layer opacities for hover, focus, pressed, dragged, selected and disabled states.</summary>
public sealed record StateTokens
{
    /// <summary>Hover opacity token.</summary>
    [CssVar(State.HoverOpacity)] public required string HoverOpacity { get; init; }
    /// <summary>Selected/active-container tint opacity token.</summary>
    [CssVar(State.SelectedOpacity)] public required string SelectedOpacity { get; init; }
    /// <summary>Focus opacity token.</summary>
    [CssVar(State.FocusOpacity)] public required string FocusOpacity { get; init; }
    /// <summary>Pressed opacity token.</summary>
    [CssVar(State.PressedOpacity)] public required string PressedOpacity { get; init; }
    /// <summary>Dragged opacity token.</summary>
    [CssVar(State.DraggedOpacity)] public required string DraggedOpacity { get; init; }
    /// <summary>Disabled opacity token.</summary>
    [CssVar(State.DisabledOpacity)] public required string DisabledOpacity { get; init; }
    /// <summary>Disabled container opacity token.</summary>
    [CssVar(State.DisabledContainerOpacity)] public required string DisabledContainerOpacity { get; init; }

    /// <summary>Hover state-layer paint (full background value incl. alpha). Lets a theme pick the state
    /// model - translucent currentColor wash or a discrete fill - instead of the core baking one.</summary>
    [CssVar(State.HoverLayer)] public required string HoverLayer { get; init; }
    /// <summary>Focus state-layer paint (full background value incl. alpha).</summary>
    [CssVar(State.FocusLayer)] public required string FocusLayer { get; init; }
    /// <summary>Pressed state-layer paint (full background value incl. alpha).</summary>
    [CssVar(State.PressedLayer)] public required string PressedLayer { get; init; }
    /// <summary>Dragged state-layer paint (full background value incl. alpha).</summary>
    [CssVar(State.DraggedLayer)] public required string DraggedLayer { get; init; }
}
