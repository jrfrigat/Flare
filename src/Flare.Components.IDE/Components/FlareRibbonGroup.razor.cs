using Flare.Core.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// A labeled group of commands within a <see cref="FlareRibbonTab"/>.
/// Renders a content area with an optional label at the bottom.
/// </summary>
public partial class FlareRibbonGroup : FlareComponentBase
{
    /// <summary>Label displayed below the group content.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Command items rendered inside the group.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.Ribbon.Group;
}
