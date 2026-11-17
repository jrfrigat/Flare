using Flare.Components;
using Flare.Icons;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// One segment of a <see cref="FlareEditorBreadcrumbBar"/>: an optional 16px icon, a label and, when
/// <see cref="MenuContent"/> is set, a chevron and a dropdown fed by the host. The segment never
/// decides what the menu contains; a namespace, a type and its members are host data.
/// </summary>
public partial class FlareEditorBreadcrumbSegment : FlareComponentBase
{
    /// <summary>The visible segment label, also the dropdown's accessible name when
    /// <see cref="AriaLabel"/> is unset.</summary>
    [Parameter] public string? Text { get; set; }

    /// <summary>Leading icon of the segment, rendered at the row's icon size (16px by default).</summary>
    [Parameter] public FlareIcon? Icon { get; set; }

    /// <summary>Whether this is the active (current) segment. The active segment takes the editor
    /// surface and the full foreground colour instead of the muted row colour.</summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>Dropdown content of the segment, typically <see cref="FlareMenuItem"/> items the host
    /// builds for its namespace/type path. Null renders a static icon-and-text segment with no menu
    /// and no chevron.</summary>
    [Parameter] public RenderFragment? MenuContent { get; set; }

    /// <summary>Accessible name of a dropdown segment's button and menu panel. Null uses
    /// <see cref="Text"/>; set it when the visible label is absent or too terse. A static segment
    /// has no button and takes its name from <see cref="Text"/>.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Ide.EditorBreadcrumbBar.Segment;
}
