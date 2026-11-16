using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

/// <summary>
/// Presents several <see cref="FlareToolPanel"/> children as one tabbed tool area: a tab strip, a single
/// visible panel and the panel bodies kept mounted while hidden. Inside a <see cref="FlareIdeLayout"/>
/// region the hosted panels still report their titles and modes, so the region rail and auto-hide
/// behavior are unchanged; the group draws the region's collapse button because the individual panel
/// headers are replaced by the tabs.
/// </summary>
public partial class FlareToolPanelGroup : FlareLocalizedComponentBase
{
    [Inject] private IUiJsService Ui { get; set; } = default!;

    /// <summary>The enclosing IDE layout, when the group is used in one of its regions.</summary>
    [CascadingParameter] internal FlareIdeLayout? Layout { get; set; }

    /// <summary>Tool panels shown as tabs. Every child is a <see cref="FlareToolPanel"/>; its content
    /// stays mounted while another tab is active.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Region that hosts the group inside a <see cref="FlareIdeLayout"/>. The hosted panels use
    /// this position for rail labels, auto-hide and collapse instead of their own
    /// <see cref="FlareToolPanel.Position"/>.</summary>
    [Parameter] public ToolPanelPosition Position { get; set; } = ToolPanelPosition.Left;

    /// <summary>Effective key (<see cref="FlareToolPanel.Key"/>, or the title when that is unset) of the
    /// panel to show. Null or empty keeps the group's own selection. A key that matches no panel keeps
    /// the current one, so a stale or not-yet-registered key never leaves the body empty. Bind it with
    /// <see cref="ActiveKeyChanged"/> and set it from a host command to select a tool.</summary>
    [Parameter] public string? ActiveKey { get; set; }

    /// <summary>Reports the effective key of the panel the user activated. Bind it to
    /// <see cref="ActiveKey"/> to accept the choice; in controlled mode the group moves only after the
    /// parent sets the new key.</summary>
    [Parameter] public EventCallback<string?> ActiveKeyChanged { get; set; }

    /// <summary>Reports the title of the panel the user activated, for a status line or command state.
    /// Not raised for parameter-driven changes or when the removal of a panel falls back to a neighbour.</summary>
    [Parameter] public EventCallback<string?> OnActiveChanged { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.ToolPanelGroup.Root;

    private IStringLocalizer<IdeStrings> IdeText => Localizer<IdeStrings>();

    internal IReadOnlyList<FlareToolPanel> Panels => _panels;

    internal bool IsActive(FlareToolPanel panel) => ReferenceEquals(_active, panel);

    private readonly List<FlareToolPanel> _panels = [];
    private readonly Dictionary<FlareToolPanel, string?> _titles = [];
    private FlareToolPanel? _active;
    private ElementReference _bar;
    private DotNetObjectReference<FlareToolPanelGroup>? _reference;
    private bool _keyboardRegistered;
    private bool _disposed;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (string.IsNullOrEmpty(ActiveKey)) return;
        if (FindByKey(ActiveKey) is { } requested)
            _active = requested;
        else if (_active is null && _panels.Count > 0)
            _active = _panels[0];
    }

    /// <summary>Called by a hosted panel when it mounts or its title or key changes.</summary>
    internal void Register(FlareToolPanel panel)
    {
        var isNew = !_panels.Contains(panel);
        if (isNew) _panels.Add(panel);
        _titles.TryGetValue(panel, out var previousTitle);
        var changed = isNew || previousTitle != panel.Title;
        _titles[panel] = panel.Title;

        // Children register after the parent's OnParametersSet, so a controlled key that names a panel
        // which is still mounting first resolves here.
        if (!string.IsNullOrEmpty(ActiveKey))
        {
            var requested = FindByKey(ActiveKey);
            if (requested is not null && !ReferenceEquals(_active, requested))
            {
                _active = requested;
                changed = true;
            }
            else if (_active is null && _panels.Count > 0)
            {
                _active = _panels[0];
                changed = true;
            }
        }
        else if (isNew && _active is null)
        {
            _active = panel;
        }

        if (changed && !_disposed) StateHasChanged();
    }

    /// <summary>Called by a hosted panel when it is removed from the group.</summary>
    internal void Unregister(FlareToolPanel panel)
    {
        var index = _panels.IndexOf(panel);
        if (index < 0) return;
        var wasActive = ReferenceEquals(_active, panel);
        _panels.RemoveAt(index);
        _titles.Remove(panel);
        if (wasActive)
            _active = _panels.Count == 0 ? null : _panels[Math.Min(index, _panels.Count - 1)];
        if (!_disposed) StateHasChanged();
    }

    private string TabClass(FlareToolPanel panel) => IsActive(panel)
        ? $"{Css.Classes.Ide.ToolPanelGroup.Tab} {Css.Classes.Ide.ToolPanelGroup.TabActive}"
        : Css.Classes.Ide.ToolPanelGroup.Tab;

    private string TabLabel(FlareToolPanel panel) => string.IsNullOrWhiteSpace(panel.Title)
        ? IdeText["ToolPanelGroup_Untitled"].Value
        : panel.Title!;

    private FlareToolPanel? FindByKey(string key) =>
        _panels.Find(panel => string.Equals(panel.EffectiveKey, key, StringComparison.Ordinal));

    private async Task SelectAsync(FlareToolPanel panel, bool focus)
    {
        if (!_panels.Contains(panel)) return;
        if (!ReferenceEquals(_active, panel))
        {
            // Uncontrolled selection moves here; a controlled one moves when the parent sets ActiveKey.
            if (string.IsNullOrEmpty(ActiveKey))
            {
                _active = panel;
                StateHasChanged();
            }
            await ActiveKeyChanged.InvokeAsync(panel.EffectiveKey);
            await OnActiveChanged.InvokeAsync(panel.Title);
        }
        if (!focus) return;

        try { await panel.TabElement.FocusAsync(preventScroll: true); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
        try { await Ui.RevealFocusedTabAsync(_bar); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    // The C# path stays bound while the module is registered because an event attribute cannot take a
    // nullable handler; the flag keeps a live module at one step per key, as the other tab strips do.
    private Task HandleFallbackKeyDownAsync(KeyboardEventArgs e)
    {
        if (_keyboardRegistered) return Task.CompletedTask;
        return e.Key is "ArrowLeft" or "ArrowRight" or "Home" or "End"
            ? OnTabNavigation(e.Key, ThemeService?.IsRtl == true)
            : Task.CompletedTask;
    }

    /// <summary>Activates and focuses the tab addressed by an orientation-aware navigation key.</summary>
    /// <param name="key">The navigation key whose native scroll behavior was prevented.</param>
    /// <param name="rtl">Whether the tablist resolves to right-to-left in the browser.</param>
    [JSInvokable]
    public Task OnTabNavigation(string key, bool rtl) => InvokeAsync(async () =>
    {
        if (_disposed || _panels.Count == 0) return;
        var index = _active is null ? -1 : _panels.IndexOf(_active);
        var forward = rtl ? "ArrowLeft" : "ArrowRight";
        var backward = rtl ? "ArrowRight" : "ArrowLeft";
        FlareToolPanel? target = key switch
        {
            var k when k == forward => _panels[(index + 1 + _panels.Count) % _panels.Count],
            var k when k == backward => _panels[index <= 0 ? _panels.Count - 1 : index - 1],
            "Home" => _panels[0],
            "End" => _panels[^1],
            _ => null,
        };
        if (target is not null) await SelectAsync(target, focus: true);
    });

    private Task CollapseAsync() => Layout?.CollapseAsync(Position) ?? Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _reference = DotNetObjectReference.Create(this);
        try
        {
            await Ui.RegisterTabKeyboardAsync(_bar, _reference);
            _keyboardRegistered = true;
        }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_reference is not null)
        {
            try { await Ui.RemoveTabKeyboardAsync(_bar); }
            catch (InvalidOperationException) { }
            catch (JSException) { }
            _reference.Dispose();
        }
        await base.DisposeAsync();
    }
}
