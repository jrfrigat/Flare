namespace Flare.Components;

/// <summary>
/// Whether a <c>FlareChip</c> is something to press or something to read.
/// </summary>
/// <remarks>
/// A chip used as a tag - an identifier, a status, a version - is not a control, and rendering it as one
/// puts a <c>role="button"</c> and a tab stop on every label on the screen. A chip used as a filter is a
/// control and needs both. The component can tell which it is from whether anything is wired to it, so
/// <see cref="Auto"/> is the default and the two explicit values exist for the cases it cannot see.
/// </remarks>
public enum ChipInteraction
{
    /// <summary>
    /// Decide from what the chip is wired to: a chip with <c>OnClick</c> or <c>SelectedChanged</c> bound,
    /// or one taking part in a <c>FlareChipGroup</c>, is a control; anything else is a tag. A close
    /// button is not counted, because it is its own control and carries its own focus.
    /// </summary>
    Auto,

    /// <summary>
    /// Always a control. Needed when the handler reaches the chip by attribute splatting rather than
    /// through a parameter, which <see cref="Auto"/> cannot see.
    /// </summary>
    Button,

    /// <summary>
    /// Always a tag: no role, no tab stop, no hover or pressed layer, and its text stays selectable so it
    /// can be copied. Use it for a chip that shows state it does not let you change.
    /// </summary>
    Static,
}
