using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// A collapsible tool panel. Wraps content with a header bar containing title, optional toolbar and
/// a collapse button. Composed from base Flare components - no custom tokens. Standalone, or as a tab
/// inside <see cref="FlareToolPanelGroup"/>.
/// </summary>
public partial class FlareToolPanel : FlareLocalizedComponentBase
{
    private Microsoft.Extensions.Localization.IStringLocalizer<IdeStrings> IdeText => Localizer<IdeStrings>();

    /// <summary>The enclosing tabbed group, when this panel is one of its tabs.</summary>
    [CascadingParameter] internal FlareToolPanelGroup? Group { get; set; }

    /// <summary>Panel title displayed in the header.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Icon that represents the tool. Inside a <see cref="FlareToolPanelGroup"/> it is shown in the
    /// tool's tab, and it is the only visible content of that tab once the tab is too narrow for text;
    /// with <see cref="FlareToolPanelGroup.TabStripPlacement"/> set to
    /// <see cref="Flare.Components.IDE.TabStripPlacement.Bottom"/> it is also shown beside the active tool's
    /// title in the header. Null renders no icon.</summary>
    [Parameter] public FlareIcon? Icon { get; set; }

    /// <summary>Stable key that <see cref="FlareToolPanelGroup.ActiveKey"/> uses to select this panel.
    /// When unset the panel is addressed by <see cref="Title"/>, which is enough for a fixed, untranslated
    /// title; set an explicit key when titles are localized, repeat or change at run time.</summary>
    [Parameter] public string? Key { get; set; }

    /// <summary>Content rendered inside the panel body.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional toolbar content in the header (e.g. filter, settings buttons).</summary>
    [Parameter] public RenderFragment? ToolbarContent { get; set; }

    /// <summary>Whether to show the toolbar area.</summary>
    [Parameter] public bool ShowToolbar { get; set; }

    /// <summary>Position of the panel in the IDE chrome.</summary>
    [Parameter] public ToolPanelPosition Position { get; set; } = ToolPanelPosition.Left;

    /// <summary>Whether the panel is currently collapsed. Ignored while the panel is one tab of a
    /// <see cref="FlareToolPanelGroup"/>.</summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>Callback when the collapsed state changes.</summary>
    [Parameter] public EventCallback<bool> CollapsedChanged { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.ToolPanel.Root;

    private readonly string _tabSuffix = Guid.NewGuid().ToString("N")[..8];
    internal string TabId => $"flare-toolpanel-tab-{_tabSuffix}";
    internal string PanelId => $"flare-toolpanel-panel-{_tabSuffix}";
    internal ElementReference TabElement { get; set; }

    /// <summary>Identity of the panel inside its group: <see cref="Key"/> when set, otherwise
    /// <see cref="Title"/>; null when the panel has neither.</summary>
    internal string? EffectiveKey =>
        !string.IsNullOrEmpty(Key) ? Key : string.IsNullOrEmpty(Title) ? null : Title;

    private FlareToolPanelGroup? _registeredGroup;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_registeredGroup, Group))
        {
            _registeredGroup?.Unregister(this);
            _registeredGroup = Group;
        }
        _registeredGroup?.Register(this);
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _registeredGroup?.Unregister(this);
        return base.DisposeAsync();
    }

    private string? _positionClass => Position switch
    {
        ToolPanelPosition.Right => Css.Classes.Ide.ToolPanel.Right,
        ToolPanelPosition.Bottom => Css.Classes.Ide.ToolPanel.Bottom,
        _ => Css.Classes.Ide.ToolPanel.Left
    };

    // Standalone collapse: the panel narrows to its header rail.
    private string? _collapsedClass => Collapsed ? Css.Classes.Ide.ToolPanel.Collapsed : null;

    private string? _rootStyle => Collapsed
        ? $"width:40px;min-width:40px;{Style}"
        : Style;

    private async Task ToggleCollapsed()
    {
        Collapsed = !Collapsed;
        await CollapsedChanged.InvokeAsync(Collapsed);
    }
}
