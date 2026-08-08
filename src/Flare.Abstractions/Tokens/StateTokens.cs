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

    /// <summary>
    /// State layer painted while an element is hovered <i>and</i> focused at once - the one pairing
    /// of states the design languages genuinely disagree about, so each theme answers it outright
    /// instead of the core imposing an order.
    /// </summary>
    /// <remarks>
    /// A language whose focus is a fill wants focus to win here, matching its documented state
    /// precedence. A language whose focus is a stroke wants the hover fill to survive, because its
    /// focus indicator lives on a different channel and the two are meant to coexist. No other pair
    /// is contested: pressed outranks both everywhere.
    /// </remarks>
    [CssVar(State.FocusHoverLayer)] public required string FocusHoverLayer { get; init; }
}
