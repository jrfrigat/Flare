namespace Flare.Components;

/// <summary>Cascading state shared by a <c>FlareChipGroup</c> with its child chips.</summary>
/// <param name="SelectedValues">Values of the currently selected chips.</param>
/// <param name="MultiSelect">Whether more than one chip can be selected at once.</param>
/// <param name="Toggle">Callback that toggles selection of the chip with the given value.</param>
public sealed record FlareChipGroupContext(
    IReadOnlySet<string> SelectedValues,
    bool MultiSelect,
    Func<string, Task> Toggle);
