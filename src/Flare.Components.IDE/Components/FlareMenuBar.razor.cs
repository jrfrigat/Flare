using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// Horizontal menu bar rendered below the ribbon. Typically contains top-level
/// menus (File, Edit, View, etc.) using <see cref="FlareMenu"/> components.
/// Composed from base Flare CSS classes - no custom tokens.
/// </summary>
public partial class FlareMenuBar : FlareComponentBase
{
    /// <summary>Menu items rendered inside the bar.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.MenuBar.Root;
}
