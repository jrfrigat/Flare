using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>State-layer opacities for hover, focus, pressed, dragged and disabled states.</summary>
public sealed record StateTokens
{
    /// <summary>Hover opacity token.</summary>
    [CssVar(State.HoverOpacity)] public required string HoverOpacity { get; init; }
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
}
