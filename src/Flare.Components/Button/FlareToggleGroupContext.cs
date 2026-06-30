namespace Flare.Components;

/// <summary>Cascading context published by FlareToggleGroup to its child FlareToggleButton components.</summary>
/// <param name="SelectedValues">The values currently selected in the group.</param>
/// <param name="MultiSelect">Whether more than one button can be selected at once.</param>
/// <param name="Toggle">Toggles the selection of the button with the given value.</param>
/// <param name="Size">Size cascaded to every button (null = each button keeps its own <c>Size</c>).</param>
/// <param name="Color">Color cascaded to every button (null = each button keeps its own <c>Color</c>).</param>
/// <param name="Disabled">When true, every button in the group is disabled.</param>
public sealed record FlareToggleGroupContext(
    IReadOnlySet<object?> SelectedValues,
    bool MultiSelect,
    Func<object?, Task> Toggle,
    ButtonSize? Size = null,
    FlareColor? Color = null,
    bool Disabled = false
);
