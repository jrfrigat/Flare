namespace Flare.Components;

internal sealed record FlareChoiceGroupContext(
    object? SelectedValue,
    bool GroupDisabled,
    Func<object?, Task> Select,
    string GroupName);
