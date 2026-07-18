using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.IDE;

/// <summary>
/// A single command button inside a <see cref="FlareRibbonGroup"/>.
/// Supports large (with label) and small (icon-only) display modes.
/// Wraps <see cref="FlareButton"/> with ribbon-specific styling.
/// </summary>
public partial class FlareRibbonButton : FlareComponentBase
{
    /// <summary>Icon displayed in the button - any provider (a bare string is a Material Symbols name).</summary>
    [Parameter] public FlareIcon? Icon { get; set; }

    /// <summary>Label text displayed below the icon in large mode.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Tooltip text shown on hover.</summary>
    [Parameter] public string? Tooltip { get; set; }

    /// <summary>Large buttons show icon + label vertically. Small buttons show icon only.</summary>
    [Parameter] public bool IsLarge { get; set; }

    /// <summary>Whether the button is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Callback when the button is clicked.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.Ribbon.Button;
}
