namespace Flare.Components;

/// <summary>
/// Cascading context published by <c>FlareButtonGroup</c> so child <c>FlareButton</c> components
/// can inherit the group's visual settings. A <c>null</c> member means "no group-level override" -
/// the button keeps its own parameter value.
/// </summary>
public sealed record FlareButtonGroupContext(
    ButtonSize? Size,
    ButtonVariant? Variant,
    FlareColor? Color
);
