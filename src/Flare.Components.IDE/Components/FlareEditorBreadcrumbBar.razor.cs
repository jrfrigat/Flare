using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Flare.Components.IDE;

/// <summary>
/// The editor breadcrumb/toolbar row shown under the document tabs: a theme-sized strip with a
/// leading breadcrumb slot and a trailing action area. The host supplies the segments and commands;
/// the row owns only the chrome. Use <see cref="FlareEditorBreadcrumbSegment"/> items for the
/// breadcrumb path, and any Flare controls for the actions.
/// </summary>
public partial class FlareEditorBreadcrumbBar : FlareLocalizedComponentBase
{
    private IStringLocalizer<IdeStrings> IdeText => Localizer<IdeStrings>();

    /// <summary>Leading content of the row - typically one or more
    /// <see cref="FlareEditorBreadcrumbSegment"/> items. The host supplies the path; the row adds no
    /// items of its own.</summary>
    [Parameter] public RenderFragment? BreadcrumbContent { get; set; }

    /// <summary>Trailing action area of the row - the host's editor commands, an error count or a
    /// split/layout control. Rendered in a labelled <c>toolbar</c>; the row adds no commands.</summary>
    [Parameter] public RenderFragment? ToolbarContent { get; set; }

    /// <summary>Accessible name of the trailing action area. Null uses the package's localized
    /// "Editor actions" text.</summary>
    [Parameter] public string? ActionsLabel { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Ide.EditorBreadcrumbBar.Root;
}
