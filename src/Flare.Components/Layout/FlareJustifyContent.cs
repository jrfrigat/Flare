namespace Flare.Components;

/// <summary>CSS <c>justify-content</c> values for flex containers.</summary>
public enum FlareJustifyContent
{
    /// <summary>No modifier class - inherits the component's default justification.</summary>
    Default,
    /// <summary>Pack items toward the start of the main axis (CSS: <c>flex-start</c>).</summary>
    Start,
    /// <summary>Pack items toward the end of the main axis (CSS: <c>flex-end</c>).</summary>
    End,
    /// <summary>Centre items along the main axis (CSS: <c>center</c>).</summary>
    Center,
    /// <summary>Distribute items with equal space between them (CSS: <c>space-between</c>).</summary>
    SpaceBetween,
    /// <summary>Distribute items with equal space around them (CSS: <c>space-around</c>).</summary>
    SpaceAround,
    /// <summary>Distribute items with equal space between and around them (CSS: <c>space-evenly</c>).</summary>
    SpaceEvenly,
}
