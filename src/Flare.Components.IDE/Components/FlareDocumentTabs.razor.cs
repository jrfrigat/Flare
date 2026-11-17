using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

/// <summary>Displays document tabs with optional controlled selection, ordering and retained panels.</summary>
public partial class FlareDocumentTabs
{
    [Inject] private IUiJsService Ui { get; set; } = default!;
    [Inject] private IJSRuntime Js { get; set; } = default!;

    /// <summary>Declares the document tabs, using a Blazor key for each dynamically rendered document.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>How to navigate when the strip overflows.</summary>
    [Parameter] public TabOverflowBehavior OverflowBehavior { get; set; } = TabOverflowBehavior.Scroll;

    /// <summary>Active document key. Null starts with the first document and permits internal selection;
    /// with <see cref="ControlledSelection"/> set, null means no active tab and the value is authoritative.</summary>
    [Parameter] public string? ActiveKey { get; set; }

    /// <summary>Whether <see cref="ActiveKey"/> is the only source of the selection. When true, null means
    /// no document is active and a click or navigation key only reports the key through
    /// <see cref="ActiveKeyChanged"/> - the host selects it by setting <see cref="ActiveKey"/>. A key that
    /// matches no declared document also leaves every tab inactive. The default false keeps the
    /// uncontrolled behavior: null starts with the first document and clicks select internally.</summary>
    [Parameter] public bool ControlledSelection { get; set; }

    /// <summary>Reports the selected document's key.</summary>
    [Parameter] public EventCallback<string> ActiveKeyChanged { get; set; }

    /// <summary>Reports a request to close the document by key. The host removes its declaration.</summary>
    [Parameter] public EventCallback<string> OnCloseRequested { get; set; }

    /// <summary>Reports the title of the document requested to close.</summary>
    [Parameter] public EventCallback<string> OnTabClose { get; set; }

    /// <summary>Reports the title of the selected document.</summary>
    [Parameter] public EventCallback<string> OnTabChanged { get; set; }

    /// <summary>Whether documents can be moved within the strip using drag or Alt+Arrow.</summary>
    [Parameter] public bool Reorderable { get; set; }

    /// <summary>Makes every tab a drag source for <see cref="FlareIdeLayout"/> docking even when
    /// <see cref="Reorderable"/> is off. Each dockable tab needs an explicit <see cref="FlareDocumentTab.Key"/>;
    /// the key is what the layout receives through <see cref="FlareIdeLayout.OnDocumentMove"/>. The host
    /// must declare the same key in <see cref="FlareIdeLayout.Documents"/>. A drop is accepted only by
    /// the layout that contains the strip, so two layouts may use the same key without moving each
    /// other's document; a strip outside every layout cannot dock into one.</summary>
    [Parameter] public bool Dockable { get; set; }

    /// <summary>Destinations offered in each tab's move menu. Set <see cref="OnDockRequested"/> too;
    /// without it no menu is rendered. The strip does not move the document itself - the host updates
    /// its document list and layout selection.</summary>
    [Parameter] public IReadOnlyList<FlareDocumentDockTarget>? DockTargets { get; set; }

    /// <summary>Reports the document key and the destination the user chose in a tab's move menu.</summary>
    [Parameter] public EventCallback<FlareDocumentDockRequest> OnDockRequested { get; set; }

    /// <summary>Controlled document key order. Unlisted documents follow in declaration order.</summary>
    [Parameter] public IReadOnlyList<string>? TabOrder { get; set; }

    /// <summary>Reports a requested order containing the keys of the current documents.</summary>
    [Parameter] public EventCallback<IReadOnlyList<string>> TabOrderChanged { get; set; }

    /// <summary>Context menu entries rendered for the given document key.</summary>
    [Parameter] public RenderFragment<string>? ContextMenuContent { get; set; }

    /// <summary>Keeps previously activated panels mounted while hidden.</summary>
    [Parameter] public bool KeepAlive { get; set; }

    /// <summary>Renders only the tab strip and no panel region. The <see cref="FlareDocumentTab"/> children
    /// still declare the documents (key, title, icon, closeable) and stay mounted, but neither their content
    /// nor a tabpanel element is rendered - the host renders the document itself, so <see cref="KeepAlive"/>
    /// and each tab's content are ignored in this mode.
    /// <para>Use it as <see cref="FlareIdeLayout.CenterDockHeader"/>: the layout then takes the Center
    /// selection only from <see cref="FlareIdeLayout.ActiveDocuments"/>, so write the key reported by
    /// <see cref="ActiveKeyChanged"/> into that selection, and return to the main content by setting the
    /// Center entry to null.</para></summary>
    [Parameter] public bool StripOnly { get; set; }

    /// <summary>The active document's title, or null when there are no documents.</summary>
    public string? ActiveTabTitle => _active?.Title;

    protected override string ComponentCssClass => Css.Classes.Ide.DocTabs.Root;
    private IStringLocalizer<IdeStrings> Text => Localizer<IdeStrings>();
    private readonly List<FlareDocumentTab> _tabs = [];
    private IReadOnlyList<FlareDocumentTab> _ordered = [];
    private IReadOnlyList<string>? _internalOrder;
    private FlareDocumentTab? _active;
    private FlareDocumentTab? _dragged;
    private FlareDocumentTab? _requestedClose;
    private FlareDocumentTab? _focusTarget;
    private ElementReference _bar;
    private DotNetObjectReference<FlareDocumentTabs>? _reference;
    private IJSObjectReference? _ideModule;
    private bool _dragSourceRequested;
    private bool _registeredKeyboard;
    private bool _overflowing;
    private bool _atStart = true;
    private bool _atEnd = true;
    private bool _disposed;
    private IReadOnlyList<FlareDocumentTab> OrderedTabs => _ordered;

    protected override void OnParametersSet() => RefreshTabs();

    internal void Register(FlareDocumentTab tab)
    {
        if (!_tabs.Contains(tab)) _tabs.Add(tab);
        RefreshTabs();
        StateHasChanged();
    }

    internal void Unregister(FlareDocumentTab tab)
    {
        if (!_tabs.Remove(tab) || _disposed) return;
        var index = _ordered.ToList().IndexOf(tab);
        var wasActive = ReferenceEquals(_active, tab);
        var restoreFocus = ReferenceEquals(_requestedClose, tab);
        if (restoreFocus) _requestedClose = null;
        RefreshTabs(wasActive ? Math.Max(index, 0) : 0);
        if (restoreFocus) _focusTarget = _active;
        StateHasChanged();
    }

    private void RefreshTabs(int fallbackIndex = 0)
    {
        _ordered = IdeItemOrder.Apply(_tabs, TabOrder ?? _internalOrder, static tab => tab.EffectiveKey);
        if (ControlledSelection)
        {
            _active = ActiveKey is null ? null : _tabs.Find(tab => tab.EffectiveKey == ActiveKey);
            return;
        }
        var requested = ActiveKey is null ? _active : _tabs.Find(tab => tab.EffectiveKey == ActiveKey);
        _active = requested is not null && _tabs.Contains(requested)
            ? requested
            : _ordered.Count == 0 ? null : _ordered[Math.Min(fallbackIndex, _ordered.Count - 1)];
    }

    internal bool IsTabActive(FlareDocumentTab tab) => ReferenceEquals(tab, _active);

    // With no active tab the first one is the entry point for the roving tab stop, so the strip stays
    // keyboard reachable while aria-selected stays false on every tab.
    private int TabIndex(FlareDocumentTab tab) =>
        IsTabActive(tab) || (_active is null && _ordered.Count > 0 && ReferenceEquals(tab, _ordered[0])) ? 0 : -1;

    private string TabClass(FlareDocumentTab tab) => IsTabActive(tab)
        ? $"{Css.Classes.Ide.DocTabs.Tab} {Css.Classes.Ide.DocTabs.TabActive}"
        : Css.Classes.Ide.DocTabs.Tab;

    private async Task ActivateAsync(FlareDocumentTab tab, bool focus)
    {
        if (focus) _focusTarget = tab;
        if (IsTabActive(tab)) return;
        if (ActiveKey is null && !ControlledSelection) _active = tab;
        await ActiveKeyChanged.InvokeAsync(tab.EffectiveKey);
        await OnTabChanged.InvokeAsync(tab.Title);
    }

    private async Task CloseAsync(FlareDocumentTab tab)
    {
        if (!tab.Closeable) return;
        _requestedClose = tab;
        await OnCloseRequested.InvokeAsync(tab.EffectiveKey);
        await OnTabClose.InvokeAsync(tab.Title);
    }

    private Task RequestDockAsync(FlareDocumentTab tab, FlareDocumentDockTarget target) =>
        OnDockRequested.InvokeAsync(new(tab.EffectiveKey, target));

    private void BeginDrag(FlareDocumentTab tab)
    {
        if (Reorderable) _dragged = tab;
    }

    private void EndDrag() => _dragged = null;

    private Task DropAsync(FlareDocumentTab target)
    {
        var source = _dragged;
        EndDrag();
        return source is null ? Task.CompletedTask : MoveAsync(source, _ordered.ToList().IndexOf(target));
    }

    private async Task MoveAsync(FlareDocumentTab tab, int index)
    {
        if (!Reorderable) return;
        var order = IdeItemOrder.Move(_ordered, tab, index, static item => item.EffectiveKey);
        if (order is null) return;
        if (TabOrder is null)
        {
            _internalOrder = order;
            RefreshTabs();
        }

        _focusTarget = tab;
        await TabOrderChanged.InvokeAsync(order);
    }

    private Task HandleFallbackAsync(FlareDocumentTab tab, KeyboardEventArgs args)
    {
#if NET9_0_OR_GREATER
        if (args.IsComposing) return Task.CompletedTask;
#endif
        if (args.CtrlKey || args.MetaKey) return Task.CompletedTask;
        if (args.Key is " " or "Enter") return ActivateAsync(tab, focus: true);
        if (_registeredKeyboard) return Task.CompletedTask;
        var rtl = ThemeService?.IsRtl == true;
        return args.AltKey && args.Key is "ArrowLeft" or "ArrowRight"
            ? OnTabReorder(args.Key, rtl)
            : !args.AltKey ? OnTabNavigation(args.Key, rtl) : Task.CompletedTask;
    }

    /// <summary>Handles the orientation-aware keys reported by the tab strip.</summary>
    [JSInvokable]
    public Task OnTabNavigation(string key, bool rtl) => InvokeAsync(async () =>
    {
        if (_disposed || _ordered.Count == 0) return;
        var index = _active is null ? -1 : _ordered.ToList().IndexOf(_active);
        var forward = rtl ? "ArrowLeft" : "ArrowRight";
        var backward = rtl ? "ArrowRight" : "ArrowLeft";
        if (key == "Delete")
        {
            if (_active is not null) await CloseAsync(_active);
        }
        else if (key == "Home" || key == "End" || key == forward || key == backward)
        {
            var count = _ordered.Count;
            var next = key == "Home" ? 0 : key == "End" ? count - 1
                : key == forward ? (index + 1 + count) % count
                : index <= 0 ? count - 1 : index - 1;
            await ActivateAsync(_ordered[next], focus: true);
        }
        StateHasChanged();
    });

    /// <summary>Moves the active tab by one position for an orientation-aware Alt+Arrow.</summary>
    [JSInvokable]
    public Task OnTabReorder(string key, bool rtl) => InvokeAsync(async () =>
    {
        if (_disposed || _active is null || key is not ("ArrowRight" or "ArrowLeft")) return;
        var step = (key == "ArrowRight") != rtl ? 1 : -1;
        await MoveAsync(_active, _ordered.ToList().IndexOf(_active) + step);
        StateHasChanged();
    });

    /// <summary>Receives changes to the measured scroll extent and its edges.</summary>
    [JSInvokable]
    public Task OnTabScrollState(bool overflowing, bool atStart, bool atEnd) => InvokeAsync(() =>
    {
        if (_disposed || (_overflowing, _atStart, _atEnd) == (overflowing, atStart, atEnd)) return;
        (_overflowing, _atStart, _atEnd) = (overflowing, atStart, atEnd);
        StateHasChanged();
    });

    private async Task ScrollAsync(int direction) => await Ui.ScrollTabsAsync(_bar, direction);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _reference = DotNetObjectReference.Create(this);
            try
            {
                await Ui.RegisterTabScrollerAsync(_bar, _reference);
                await Ui.RegisterTabKeyboardAsync(_bar, _reference);
                _registeredKeyboard = true;
            }
            catch (InvalidOperationException) { }
            catch (JSException) { }
        }

        if (Dockable && _ideModule is null && !_dragSourceRequested)
            await RegisterDragSourceAsync();

        if (_focusTarget is not { } tab || !_tabs.Contains(tab)) return;
        _focusTarget = null;
        try
        {
            await tab.Header.FocusAsync(preventScroll: true);
            await Ui.RevealFocusedTabAsync(_bar);
        }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    // The module's delegated dragstart reads data-flare-doc-key and writes both the DOM data transfer
    // and the module's own state; the layout reads that state back on drop.
    private async Task RegisterDragSourceAsync()
    {
        _dragSourceRequested = true;
        try
        {
            _ideModule = await Js.InvokeAsync<IJSObjectReference>("import", IdeInterop.ModulePath);
            await _ideModule.InvokeVoidAsync("registerDocumentDragSource");
        }
        catch (InvalidOperationException) { _ideModule = null; }
        catch (JSException) { _ideModule = null; }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_reference is not null)
        {
            try
            {
                await Ui.RemoveTabKeyboardAsync(_bar);
                await Ui.RemoveTabScrollerAsync(_bar);
            }
            catch (InvalidOperationException) { }
            catch (JSException) { }
            _reference.Dispose();
        }
        if (_ideModule is not null)
        {
            try { await _ideModule.DisposeAsync(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        await base.DisposeAsync();
    }
}
