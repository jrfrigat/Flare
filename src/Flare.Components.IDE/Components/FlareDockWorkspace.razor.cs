using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

/// <summary>Renders and coordinates dynamic tab groups and nested splits within one Blazor renderer.</summary>
public partial class FlareDockWorkspace : FlareLocalizedComponentBase
{
    private static readonly string[] BodyZones = ["into", "start", "end", "top", "bottom"];

    [Inject] private IJSRuntime Js { get; set; } = default!;

    /// <summary>The accepted topology, supplied by the application each time it changes.</summary>
    [Parameter, EditorRequired] public DockNode Layout { get; set; } = new DockGroup("root", []);

    /// <summary>Presentation entries for the keys placed in the workspace.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<DockItem> Items { get; set; } = [];

    /// <summary>Renders content for a mounted item. Keep the type of the root component stable across moves.</summary>
    [Parameter, EditorRequired] public RenderFragment<DockItem>? ContentTemplate { get; set; }

    /// <summary>Optionally renders the non-interactive contents of a tab.</summary>
    [Parameter] public RenderFragment<DockItem>? TabTemplate { get; set; }

    /// <summary>Truncates tab labels and then switches to compact tabs as each group narrows.
    /// The compact view uses <see cref="DockItem.Icon"/> or a short <see cref="DockItem.Title"/>
    /// for items without an icon. The default is false, which keeps full labels and scrolls.</summary>
    [Parameter] public bool AdaptiveTabCompression { get; set; }

    /// <summary>Decides whether each group uses adaptive tab compression.
    /// When supplied, its result overrides <see cref="AdaptiveTabCompression"/> for that group.</summary>
    [Parameter] public Func<DockGroup, bool>? AdaptiveTabCompressionForGroup { get; set; }

    /// <summary>Optionally renders non-interactive markers inside a tab after its content - a pin, a
    /// dirty dot, a preview state. The fragment is drawn inside the tab button, so it may contain only
    /// phrasing content; put controls in <see cref="TabActionsTemplate"/> instead.</summary>
    [Parameter] public RenderFragment<DockItem>? TabMarkerTemplate { get; set; }

    /// <summary>Optionally renders actions for a tab beside the tab button, outside it. A host command
    /// belongs here so it can be a real button; the strip starts a drag only from the grip, so an action
    /// never begins one.</summary>
    [Parameter] public RenderFragment<DockItem>? TabActionsTemplate { get; set; }

    /// <summary>Provides a localized accessible label for a group.</summary>
    [Parameter] public Func<DockGroup, string>? GroupLabel { get; set; }

    /// <summary>
    /// Receives a proposed transition. Apply <see cref="DockLayoutChange.Proposed"/> to <see cref="Layout"/>
    /// to accept it. After asynchronous approval, check that <see cref="DockLayoutChange.Previous"/>
    /// is still current before applying the proposal.
    /// </summary>
    [Parameter] public EventCallback<DockLayoutChange> OnChangeRequested { get; set; }

    /// <summary>Declares which operations the application currently offers. Actual approval belongs to the callback.</summary>
    [Parameter] public Func<DockAction, string?, string, bool>? CanRequest { get; set; }

    /// <summary>Which body regions the workspace offers as drop targets. The tab row stays available for reordering.</summary>
    [Parameter] public DockDropZones DropZones { get; set; } = DockDropZones.All;

    /// <summary>Whether the base guide is painted while a tab is dragged over a drop zone. False keeps
    /// the zones active and the move possible but draws no candidate or over highlight, so a host can
    /// present its own guide without overriding the workspace's internal classes.</summary>
    [Parameter] public bool ShowDropGuide { get; set; } = true;

    protected override string ComponentCssClass => DockCss.Root;

    private string? _guideClass => ShowDropGuide ? null : DockCss.NoGuide;

    private readonly string _canvasId = $"dock-{Guid.NewGuid():N}";
    private readonly Dictionary<string, string> _ids = new(StringComparer.Ordinal);
    private readonly HashSet<string> _activated = new(StringComparer.Ordinal);
    private readonly Dictionary<string, (string Group, string Zone)> _targets = new(StringComparer.Ordinal);
    private DockNode _accepted = new DockGroup("root", []);
    private Dictionary<string, DockItem> _catalog = new(StringComparer.Ordinal);
    private DockLayoutIndex _index = new();
    private DockGeometry _geometry = new();
    private double _width;
    private double _height;
    private int _gutter = DockGeometry.Gutter;
    private int _groupMin = DockGeometry.GroupMin;
    private int _nextId;
    private string? _openActions;
    private bool _focusMenu;
    private string? _focusActionsTrigger;
    private string? _status;
    private string? _clearPreview;
    private string? _focusKey;
    private string? _focusGroup;
    private bool _restoreFocus;
    private bool _pending;
    private bool _disposed;
    private bool _rtl;
    private DockNode? _dragLayout;
    private DotNetObjectReference<ViewportObserver>? _self;
    private IJSObjectReference? _module;
    private ElementReference _canvas;

    protected override void OnParametersSet()
    {
        var catalog = new Dictionary<string, DockItem>(StringComparer.Ordinal);
        foreach (var item in Items)
        {
            if (item is null || string.IsNullOrWhiteSpace(item.Key) || !catalog.TryAdd(item.Key, item))
            {
                _status = Text("Dock_Invalid");
                return;
            }
        }

        if (DockLayoutIndex.Validate(Layout, catalog) is not { } index)
        {
            _status = Text("Dock_Invalid");
            return;
        }

        _accepted = Layout;
        _catalog = catalog;
        _index = index;
        _activated.RemoveWhere(key => !_index.Owners.ContainsKey(key));
        foreach (var group in _index.Groups.Values)
            if (group.ActiveKey is { } active) _activated.Add(active);
        foreach (var key in _ids.Keys.ToArray())
            if (!_index.Groups.ContainsKey(key) && !_index.Splits.ContainsKey(key) && !_index.Owners.ContainsKey(key))
                _ids.Remove(key);
        _status = null;
        if (_openActions is { } open && !_index.Groups.ContainsKey(open))
        {
            _openActions = null;
            _focusMenu = false;
        }
        Rebuild();
    }

    private void Rebuild()
    {
        _geometry = DockGeometry.Build(_accepted, Id, _width, _height, _gutter, _groupMin);
        _targets.Clear();
        foreach (var group in _index.Groups.Values)
        {
            var row = ZoneId(group.Key, "row");
            _targets.Add(row, (group.Key, "row"));
            foreach (var zone in BodyZones.Where(AllowsZone))
                _targets.Add(ZoneId(group.Key, zone), (group.Key, zone));
        }
    }

    private string Id(string key)
    {
        if (_ids.TryGetValue(key, out var id)) return id;
        id = $"d{++_nextId}";
        _ids.Add(key, id);
        return id;
    }

    private string TabId(string key) => $"{_canvasId}-tab-{Id(key)}";
    private string PanelId(string key) => $"{_canvasId}-panel-{Id(key)}";
    private string HeaderId(string key) => $"{_canvasId}-group-{Id(key)}";
    private string DragId(string key) => $"{_canvasId}-drag-{Id(key)}";
    private string ZoneId(string key, string zone) => $"{_canvasId}-{Id(key)}-{zone}";
    private string ActionsId(string key) => $"{_canvasId}-actions-{Id(key)}";
    private string ActionsToggleId(string key) => $"{_canvasId}-actions-toggle-{Id(key)}";
    private string Text(string key) => Localizer<IdeStrings>()[key].Value;
    private string TabListLabel(DockGroup group) => GroupLabel?.Invoke(group) ??
        (group.Items.Length > 0 ? _catalog[group.Items[0]].Title : Text("Dock_Empty"));
    private static string TabIndex(DockGroup group, string key) =>
        key == (group.ActiveKey ?? group.Items[0]) ? "0" : "-1";
    private bool HasZone(DockDropZones zones) => (DropZones & zones) != 0;
    private bool AllowsZone(string zone) => HasZone(zone switch
    {
        "into" => DockDropZones.Into,
        "start" => DockDropZones.Start,
        "end" => DockDropZones.End,
        "top" => DockDropZones.Top,
        "bottom" => DockDropZones.Bottom,
        _ => DockDropZones.None
    });
    private static string Pixels(double size) => $"{DockGeometry.CssNumber(size)}px";
    private bool MayRequest(DockAction action, string? key, string target) =>
        OnChangeRequested.HasDelegate && (CanRequest?.Invoke(action, key, target) ?? true);
    private static string BodyStyle(string id)
    {
        var header = $"var({DockGeometry.HeaderVar(id)}, var(--flare-spacing-16))";
        return $"inset-inline-start:var(--flare-dock-{id}-x);inset-block-start:calc(var(--flare-dock-{id}-y) + {header});" +
               $"inline-size:var(--flare-dock-{id}-w);block-size:calc(var(--flare-dock-{id}-h) - {header});";
    }

    private void ToggleActions(string group)
    {
        if (_openActions == group)
        {
            _openActions = null;
            _focusMenu = false;
            return;
        }
        _openActions = group;
        _focusMenu = true;
    }

    private void ActionsKeyAsync(KeyboardEventArgs args)
    {
        if (args.Key != "Escape" || _openActions is not { } open) return;
        _openActions = null;
        _focusMenu = false;
        _focusActionsTrigger = open;
    }

    private Task CloseActionsAsync() => InvokeAsync(() =>
    {
        if (_disposed || _openActions is null) return;
        _openActions = null;
        _focusMenu = false;
        StateHasChanged();
    });

    private Task ActivateAsync(DockGroup group, string key) =>
        ProposeAsync(DockAction.Activate, key, group.Key, DockLayoutOperations.Activate(_accepted, group, key));

    private Task CloseAsync(DockGroup group, string key) =>
        ProposeAsync(DockAction.Close, key, group.Key, DockLayoutOperations.Close(_accepted, group, key));

    private Task MoveAsync(string key, DockGroup target, int index) => _index.Owners.TryGetValue(key, out var source)
        ? ProposeAsync(DockAction.Move, key, target.Key, DockLayoutOperations.Move(_accepted, source, target, key, index))
        : Task.CompletedTask;

    private Task SplitAsync(string key, DockGroup target, DockAxis axis, bool first) =>
        _index.Owners.TryGetValue(key, out var source)
            ? ProposeAsync(DockAction.Split, key, target.Key, DockLayoutOperations.Split(_accepted, source, target, key,
                axis, first, Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")))
            : Task.CompletedTask;

    private async Task ResizeAsync(DockGeometry.Entry entry, string value)
    {
        _clearPreview = DockGeometry.Preview(entry.Id);
        if (entry.Split is not { } split || !_index.Splits.TryGetValue(split.Key, out var current) ||
            !ReferenceEquals(split, current) || entry.Available <= 0 ||
            !double.TryParse(value.EndsWith("px", StringComparison.Ordinal) ? value.AsSpan(0, value.Length - 2) : value.AsSpan(), NumberStyles.Float, CultureInfo.InvariantCulture, out var width))
            return;

        var fraction = Math.Clamp(width, entry.Min, entry.Max) / entry.Available;
        if (fraction > 0 && fraction < 1 && Math.Abs(width - entry.Size) >= 0.5)
            await ProposeAsync(DockAction.Resize, null, split.Key, DockLayoutOperations.Resize(_accepted, split, fraction));
    }

    private Task BeginDrag(string key)
    {
        _dragLayout = _accepted;
        return Task.CompletedTask;
    }

    private Task EndDrag()
    {
        _dragLayout = null;
        return Task.CompletedTask;
    }

    private bool AllowsDrop(string key, string zone)
    {
        if (!_targets.TryGetValue(zone, out var target) || !_index.Owners.TryGetValue(key, out var owner)) return false;
        var action = target.Zone is "row" or "into" ? DockAction.Move : DockAction.Split;
        return (action == DockAction.Move || _index.Groups[target.Group].Items.Length > (owner.Key == target.Group ? 1 : 0))
            && MayRequest(action, key, target.Group);
    }

    private Task HandleDropAsync(FlareDropEventArgs<string> args)
    {
        if (!ReferenceEquals(_dragLayout, _accepted) || !AllowsDrop(args.Payload, args.TargetId) ||
            !_targets.TryGetValue(args.TargetId, out var target)) return Task.CompletedTask;
        var group = _index.Groups[target.Group];
        return target.Zone switch
        {
            "start" => SplitAsync(args.Payload, group, DockAxis.Horizontal, true),
            "end" => SplitAsync(args.Payload, group, DockAxis.Horizontal, false),
            "top" => SplitAsync(args.Payload, group, DockAxis.Vertical, true),
            "bottom" => SplitAsync(args.Payload, group, DockAxis.Vertical, false),
            _ => MoveAsync(args.Payload, group, DropIndex(args, group))
        };
    }

    private int DropIndex(FlareDropEventArgs<string> args, DockGroup group)
    {
        var items = group.Items.Remove(args.Payload);
        if (args.Index < 0) return items.Length;
        if (!_rtl || !args.HasOverPayload || args.OverPayload is not { } key) return args.Index;
        var over = items.IndexOf(key);
        return over < 0 ? items.Length : over + (args.Edge == DropEdge.Before ? 1 : 0);
    }

    private async Task TabKeyAsync(KeyboardEventArgs args, DockGroup group, string key)
    {
        if (args.Key == "Delete" && _catalog[key].CanClose)
        {
            await CloseAsync(group, key);
            return;
        }

        var position = group.Items.IndexOf(key);
        var next = args.Key switch
        {
            "ArrowRight" => (position + (_rtl ? -1 : 1) + group.Items.Length) % group.Items.Length,
            "ArrowLeft" => (position + (_rtl ? 1 : -1) + group.Items.Length) % group.Items.Length,
            "Home" => 0,
            "End" => group.Items.Length - 1,
            _ => -1
        };
        if (next < 0) return;
        await CallJsAsync("focusTab", _canvas, TabId(group.Items[next]));
    }

    private async Task ProposeAsync(DockAction action, string? item, string target, DockNode? next)
    {
        if (_pending || next is null || ReferenceEquals(_accepted, next) || !MayRequest(action, item, target)) return;
        var original = _accepted;
        _pending = true;
        try
        {
            _openActions = null;
            _focusMenu = false;
            // Activating a tab leaves the focus on the tab itself; only actions that can move or
            // dispose the focused pane pay for the capture and restore round trip.
            var restoresFocus = action is not DockAction.Resize and not DockAction.Activate;
            _focusKey = restoresFocus ? item : null;
            _focusGroup = restoresFocus ? target : null;
            if (_focusKey is not null)
            {
                await CallJsAsync("captureFocus", _canvas);
                _restoreFocus = true;
            }
            if (ReferenceEquals(_accepted, original))
            {
                await OnChangeRequested.InvokeAsync(new(action, item, target, original, next));
                _status = null;
            }
        }
        finally
        {
            _pending = false;
        }
    }

    private Task UpdateViewportAsync(double width, double height, bool rtl, double gutter, double groupMin) => InvokeAsync(() =>
    {
        var nextGutter = Math.Max(1, (int)Math.Round(gutter));
        var nextGroupMin = Math.Max(1, (int)Math.Round(groupMin));
        if (_disposed || !double.IsFinite(width) || !double.IsFinite(height) ||
            _width == width && _height == height && _rtl == rtl &&
            _gutter == nextGutter && _groupMin == nextGroupMin) return;
        (_width, _height, _rtl, _gutter, _groupMin) = (width, height, rtl, nextGutter, nextGroupMin);
        Rebuild();
        StateHasChanged();
    });

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _module = await Js.InvokeAsync<IJSObjectReference>("import",
                    "./_content/Flare.Components.IDE/js/flare-dock.js");
                _self = DotNetObjectReference.Create(new ViewportObserver(this));
                await _module.InvokeVoidAsync("register", _canvas, _self);
            }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }

        if (_clearPreview is { } preview)
        {
            _clearPreview = null;
            await CallJsAsync("clearResize", _canvas, preview);
        }
        if (_restoreFocus && !_pending)
        {
            _restoreFocus = false;
            var group = _focusKey is { } item && _index.Owners.TryGetValue(item, out var owner) ? owner :
                _focusGroup is { } target && _index.Groups.TryGetValue(target, out var destination) ? destination :
                _index.Groups.Values.FirstOrDefault();
            var fallback = group?.ActiveKey is { } active ? TabId(active) :
                group is null ? null : HeaderId(group.Key);
            await CallJsAsync("restoreFocus", _canvas, fallback);
        }
        if (_focusMenu && _openActions is not null)
        {
            _focusMenu = false;
            await CallJsAsync("focusActions", _canvas);
        }
        if (_focusActionsTrigger is { } trigger)
        {
            _focusActionsTrigger = null;
            await CallJsAsync("focusElement", _canvas, ActionsToggleId(trigger));
        }
    }

    private async ValueTask CallJsAsync(string method, params object?[] arguments)
    {
        if (_module is null) return;
        try { await _module.InvokeVoidAsync(method, arguments); }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _disposed = true;
        await CallJsAsync("unregister", _canvas);
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        _self?.Dispose();
        await base.DisposeAsync();
    }

    private sealed class ViewportObserver(FlareDockWorkspace owner)
    {
        [JSInvokable]
        public Task UpdateDockViewport(double width, double height, bool rtl, double gutter, double groupMin) =>
            owner.UpdateViewportAsync(width, height, rtl, gutter, groupMin);

        [JSInvokable]
        public Task CloseDockActions() => owner.CloseActionsAsync();
    }
}
