using Flare.Core.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// Ribbon component with tabbed command groups. Each <see cref="FlareRibbonTab"/> child
/// defines a tab containing <see cref="FlareRibbonGroup"/> groups of commands.
/// Composed entirely from base Flare CSS classes - no custom tokens.
/// </summary>
public partial class FlareRibbon : FlareComponentBase
{
    /// <summary>Ribbon tabs rendered inside the ribbon bar.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Current ribbon size mode - full (default) shows commands, collapsed shows only tabs.</summary>
    [Parameter] public RibbonSizeMode SizeMode { get; set; } = RibbonSizeMode.Full;

    /// <summary>Callback fired when the active tab changes. The parameter is the tab title.</summary>
    [Parameter] public EventCallback<string?> OnTabChanged { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.Ribbon.Root;

    private readonly List<FlareRibbonTab> _tabs = [];
    private FlareRibbonTab? _activeTab;

    private string? _collapsedClass => SizeMode == RibbonSizeMode.Collapsed ? Css.Classes.Ide.Ribbon.Collapsed : null;

    /// <summary>Registers a ribbon tab. Called by <see cref="FlareRibbonTab"/> on initialization.</summary>
    internal void RegisterTab(FlareRibbonTab tab)
    {
        if (_tabs.Contains(tab)) return;
        _tabs.Add(tab);
        if (_activeTab is null)
        {
            _activeTab = tab;
            StateHasChanged();
        }
    }

    /// <summary>Unregisters a ribbon tab on dispose.</summary>
    internal void UnregisterTab(FlareRibbonTab tab)
    {
        _tabs.Remove(tab);
        if (_activeTab == tab)
            _activeTab = _tabs.FirstOrDefault();
        StateHasChanged();
    }

    /// <summary>Activates the specified tab and raises <see cref="OnTabChanged"/>.</summary>
    internal async Task ActivateTab(FlareRibbonTab tab)
    {
        if (tab.Disabled) return;
        _activeTab = tab;
        StateHasChanged();
        await OnTabChanged.InvokeAsync(tab.Title);
    }

    internal bool IsTabActive(FlareRibbonTab tab) => tab == _activeTab;
}
