namespace Flare.Components;

internal sealed record FlareRadioGroupContext(
    object? SelectedValue,
    bool GroupDisabled,
    Func<object?, Task> Select,
    string GroupName,
    FlareSize GroupSize,
    string? ErrorId);
