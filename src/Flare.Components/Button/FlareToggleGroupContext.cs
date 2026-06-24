namespace Flare.Components;

/// <summary>Cascading context published by FlareToggleGroup to its child FlareToggleButton components.</summary>
public sealed record FlareToggleGroupContext(
    IReadOnlySet<object?> SelectedValues,
    bool MultiSelect,
    Func<object?, Task> Toggle
);
