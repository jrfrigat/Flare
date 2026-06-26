namespace Flare.Abstractions.Tokens;

/// <summary>State-layer opacities for hover, focus, pressed, dragged and disabled states.</summary>
public sealed record StateTokens
{
    /// <summary>Hover opacity token.</summary>
    public required string HoverOpacity { get; init; }
    /// <summary>Focus opacity token.</summary>
    public required string FocusOpacity { get; init; }
    /// <summary>Pressed opacity token.</summary>
    public required string PressedOpacity { get; init; }
    /// <summary>Dragged opacity token.</summary>
    public required string DraggedOpacity { get; init; }
    /// <summary>Disabled opacity token.</summary>
    public required string DisabledOpacity { get; init; }
    /// <summary>Disabled container opacity token.</summary>
    public required string DisabledContainerOpacity { get; init; }
}
