using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

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
        // The first tab that can be opened starts active; a disabled one is skipped.
        if (!tab.Disabled && (_activeTab is null || _activeTab.Disabled))
        {
            _activeTab = tab;
            StateHasChanged();
        }
    }

    /// <summary>Picks the first enabled tab when the active one became disabled or went away.</summary>
    internal void RefreshActive()
    {
        if (_activeTab is { Disabled: false }) return;
        _activeTab = _tabs.FirstOrDefault(tab => !tab.Disabled);
        StateHasChanged();
    }

    private ElementReference _tablistRef;
    private DotNetObjectReference<FlareRibbon>? _dotNetRef;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _dotNetRef = DotNetObjectReference.Create(this);
        try { await Ui.RegisterTabKeyboardAsync(_tablistRef, _dotNetRef); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    /// <summary>
    /// Moves to the next or previous enabled tab (Left/Right, mirrored in right-to-left text), or the first or last
    /// one (Home/End), opens it and focuses its tab. Called by the shared tab-keyboard script, which has already
    /// kept the browser from scrolling.
    /// </summary>
    /// <param name="key">The navigation key.</param>
    /// <param name="rtl">Whether the tab strip is laid out right to left.</param>
    [JSInvokable]
    public async Task OnTabNavigation(string key, bool rtl)
    {
        if (_tabs.Count == 0) return;
        var direction = key == "ArrowRight" ? 1 : key == "ArrowLeft" ? -1 : 0;
        if (rtl) direction = -direction;
        var index = _activeTab is null ? -1 : _tabs.IndexOf(_activeTab);
        if (key is "Home" or "End")
        {
            direction = key == "Home" ? 1 : -1;
            index = direction == 1 ? -1 : _tabs.Count;
        }
        else if (direction == 0) return;

        for (var i = 0; i < _tabs.Count; i++)
        {
            index = (index + direction + _tabs.Count) % _tabs.Count;
            var tab = _tabs[index];
            if (tab.Disabled) continue;
            await ActivateTab(tab);
            try
            {
                await tab.ButtonRef.FocusAsync(preventScroll: true);
                await Ui.RevealFocusedTabAsync(_tablistRef);
            }
            catch (InvalidOperationException) { }
            catch (JSException) { }
            return;
        }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        try { await Ui.RemoveTabKeyboardAsync(_tablistRef); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
        _dotNetRef?.Dispose();
        await base.DisposeAsync();
    }

    /// <summary>Unregisters a ribbon tab on dispose.</summary>
    internal void UnregisterTab(FlareRibbonTab tab)
    {
        _tabs.Remove(tab);
        if (_activeTab == tab)
            _activeTab = _tabs.FirstOrDefault(candidate => !candidate.Disabled);
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
