namespace Flare.Components;

/// <summary>CSS <c>align-items</c> values for flex containers.</summary>
public enum FlareAlignItems
{
    /// <summary>No modifier class - inherits the component's default alignment.</summary>
    Default,
    /// <summary>Stretch items to fill the cross axis (CSS: <c>stretch</c>).</summary>
    Stretch,
    /// <summary>Centre items on the cross axis (CSS: <c>center</c>).</summary>
    Center,
    /// <summary>Align items to the start of the cross axis (CSS: <c>flex-start</c>).</summary>
    Start,
    /// <summary>Align items to the end of the cross axis (CSS: <c>flex-end</c>).</summary>
    End,
    /// <summary>Align items along their text baseline (CSS: <c>baseline</c>).</summary>
    Baseline,
}
