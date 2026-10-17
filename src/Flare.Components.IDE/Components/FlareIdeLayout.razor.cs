using Flare.Components.IDE.Services;
using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

/// <summary>
/// Root layout component for an IDE-style interface. Arranges ribbon, menu bar,
/// tool panels, document area, bottom panel, and status bar in a Visual Studio-style layout.
/// All styling is composed from base Flare CSS classes and design tokens - no custom tokens.
/// The layout owns each region's collapsed state so a collapsed panel frees its docked space
/// (shrinking to a thin rail), and supports per-region auto-hide (flyout) panels.
/// </summary>
public partial class FlareIdeLayout : FlareComponentBase
{
    /// <summary>Top ribbon area rendered above the menu bar.</summary>
    [Parameter] public RenderFragment? Ribbon { get; set; }

    /// <summary>Menu bar rendered below the ribbon.</summary>
    [Parameter] public RenderFragment? MenuBar { get; set; }

    /// <summary>Left tool panel (e.g. Solution Explorer).</summary>
    [Parameter] public RenderFragment? LeftToolPanel { get; set; }

    /// <summary>Right tool panel (e.g. Properties).</summary>
    [Parameter] public RenderFragment? RightToolPanel { get; set; }

    /// <summary>Main document/content area.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Bottom panel (e.g. Output, Terminal).</summary>
    [Parameter] public RenderFragment? BottomPanel { get; set; }

    /// <summary>Status bar at the bottom of the IDE.</summary>
    [Parameter] public RenderFragment? StatusBar { get; set; }

    /// <summary>Width of the left tool panel. Default 300px.</summary>
    [Parameter] public string LeftPanelWidth { get; set; } = "300px";

    /// <summary>Width of the right tool panel. Default 300px.</summary>
    [Parameter] public string RightPanelWidth { get; set; } = "300px";

    /// <summary>Height of the bottom panel. Default 200px.</summary>
    [Parameter] public string BottomPanelHeight { get; set; } = "200px";

    /// <summary>Whether the left region is collapsed (only a thin rail is shown). Two-way bindable.</summary>
    [Parameter] public bool LeftCollapsed { get; set; }
    /// <summary>Raised when <see cref="LeftCollapsed"/> changes.</summary>
    [Parameter] public EventCallback<bool> LeftCollapsedChanged { get; set; }

    /// <summary>Whether the right region is collapsed (only a thin rail is shown). Two-way bindable.</summary>
    [Parameter] public bool RightCollapsed { get; set; }
    /// <summary>Raised when <see cref="RightCollapsed"/> changes.</summary>
    [Parameter] public EventCallback<bool> RightCollapsedChanged { get; set; }

    /// <summary>Whether the bottom region is collapsed (only a thin rail is shown). Two-way bindable.</summary>
    [Parameter] public bool BottomCollapsed { get; set; }
    /// <summary>Raised when <see cref="BottomCollapsed"/> changes.</summary>
    [Parameter] public EventCallback<bool> BottomCollapsedChanged { get; set; }

    /// <summary>Width of the thin rail shown for a collapsed or auto-hidden panel. Default 32px.</summary>
    [Parameter] public string RailSize { get; set; } = "32px";

    /// <summary>Whether the left region can be drag-resized (a divider sits between it and the document).</summary>
    [Parameter] public bool LeftResizable { get; set; } = true;
    /// <summary>Whether the right region can be drag-resized.</summary>
    [Parameter] public bool RightResizable { get; set; } = true;
    /// <summary>Whether the bottom region can be drag-resized.</summary>
    [Parameter] public bool BottomResizable { get; set; } = true;

    /// <summary>Smallest size (px) a region can be dragged to. Default 120.</summary>
    [Parameter] public int MinPanelSize { get; set; } = 120;

    [Inject] private IIdeJsService IdeJs { get; set; } = default!;

    protected override string ComponentCssClass => Css.Classes.Ide.Layout.Root;

    private ElementReference _rootRef;
    private DotNetObjectReference<FlareIdeLayout>? _selfRef;

    // Live drag-resized sizes (px); null until the user drags, then they override the *Width/*Height params.
    private int? _leftPx;
    private int? _rightPx;
    private int? _bottomPx;

    private string EffectiveLeftWidth => _leftPx is { } v ? $"{v}px" : LeftPanelWidth;
    private string EffectiveRightWidth => _rightPx is { } v ? $"{v}px" : RightPanelWidth;
    private string EffectiveBottomHeight => _bottomPx is { } v ? $"{v}px" : BottomPanelHeight;

    private bool ShowResizer(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Left => LeftToolPanel is not null && LeftResizable && !LeftCollapsed && !IsFloating(ToolPanelPosition.Left),
        ToolPanelPosition.Right => RightToolPanel is not null && RightResizable && !RightCollapsed && !IsFloating(ToolPanelPosition.Right),
        ToolPanelPosition.Bottom => BottomPanel is not null && BottomResizable && !BottomCollapsed && !IsFloating(ToolPanelPosition.Bottom),
        _ => false,
    };

    private async Task StartResize(ToolPanelPosition position)
    {
        try
        {
            _selfRef ??= DotNetObjectReference.Create(this);
            await IdeJs.StartPanelResizeAsync(_selfRef, _rootRef, position.ToString(), MinPanelSize);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    /// <summary>Invoked from JS during a divider drag with the new region size in pixels.</summary>
    /// <param name="position">The region being resized.</param>
    /// <param name="sizePx">The new size in pixels.</param>
    [JSInvokable]
    public void ResizePanel(string position, int sizePx)
    {
        sizePx = Math.Max(MinPanelSize, sizePx);
        switch (position)
        {
            case nameof(ToolPanelPosition.Right): _rightPx = sizePx; break;
            case nameof(ToolPanelPosition.Bottom): _bottomPx = sizePx; break;
            default: _leftPx = sizePx; break;
        }
        StateHasChanged();
    }

    // Keyboard resize on a focused divider (arrow keys; Shift = larger step).
    private void ResizeStep(ToolPanelPosition position, KeyboardEventArgs e)
    {
        var step = e.ShiftKey ? 24 : 8;
        int Cur(int? px, string fallback) => px ?? ParsePx(fallback, 300);
        switch (position)
        {
            case ToolPanelPosition.Left when e.Key is "ArrowLeft" or "ArrowRight":
                _leftPx = Math.Max(MinPanelSize, Cur(_leftPx, LeftPanelWidth) + (e.Key == "ArrowRight" ? step : -step));
                break;
            case ToolPanelPosition.Right when e.Key is "ArrowLeft" or "ArrowRight":
                _rightPx = Math.Max(MinPanelSize, Cur(_rightPx, RightPanelWidth) + (e.Key == "ArrowLeft" ? step : -step));
                break;
            case ToolPanelPosition.Bottom when e.Key is "ArrowUp" or "ArrowDown":
                _bottomPx = Math.Max(MinPanelSize, Cur(_bottomPx, BottomPanelHeight) + (e.Key == "ArrowUp" ? step : -step));
                break;
        }
    }

    private static int ParsePx(string value, int fallback)
    {
        var s = value?.Replace("px", "").Trim();
        return int.TryParse(s, out var v) ? v : fallback;
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        // The JS service is shared (DI-owned); only release this layout's callback reference.
        _disposed = true;
        _selfRef?.Dispose();
        return ValueTask.CompletedTask;
    }

    // Panels mounted in the layout, in first-registration (markup) order. A region may host several.
    private readonly List<(FlareToolPanel Panel, ToolPanelPosition Position, string? Title, IdePanelMode Mode)> _panels = new();

    // Rail titles and mode of each region, derived from all of its mounted panels. Kept while a collapsed
    // region has no panel mounted, so the rail keeps its labels after the panels unmount.
    private readonly Dictionary<ToolPanelPosition, (string[] Titles, IdePanelMode Mode)> _regions = new();

    private bool _disposed;

    /// <summary>Called by a hosted <see cref="FlareToolPanel"/> to report its region, title and mode.</summary>
    internal void RegisterPanel(FlareToolPanel panel, ToolPanelPosition position, string? title, IdePanelMode mode)
    {
        var index = _panels.FindIndex(p => ReferenceEquals(p.Panel, panel));
        ToolPanelPosition? previous = null;
        if (index >= 0)
        {
            var entry = _panels[index];
            if (entry.Position == position && entry.Title == title && entry.Mode == mode)
                return;
            previous = entry.Position;
            _panels[index] = (panel, position, title, mode);
        }
        else
        {
            _panels.Add((panel, position, title, mode));
        }

        var changed = UpdateRegion(position);
        if (previous is { } old && old != position)
            changed |= UpdateRegion(old);
        if (changed)
            StateHasChanged();
    }

    /// <summary>Called by a hosted <see cref="FlareToolPanel"/> when it is removed from the layout.</summary>
    internal void UnregisterPanel(FlareToolPanel panel)
    {
        var index = _panels.FindIndex(p => ReferenceEquals(p.Panel, panel));
        if (index < 0)
            return;
        var position = _panels[index].Position;
        _panels.RemoveAt(index);
        // A collapsing region unmounts its panels one by one; its rail keeps what they reported while expanded.
        if (!IsCollapsed(position) && UpdateRegion(position) && !_disposed)
            StateHasChanged();
    }

    // Recomputes a region from its mounted panels; returns true when its titles or mode changed. The result
    // depends only on the set of panels, not on the order their parameters were set within a render, so
    // re-rendering settles instead of alternating between panels.
    private bool UpdateRegion(ToolPanelPosition position)
    {
        var titles = new List<string>();
        var mounted = false;
        var allAutoHide = true;
        foreach (var p in _panels)
        {
            if (p.Position != position)
                continue;
            mounted = true;
            if (!string.IsNullOrEmpty(p.Title))
                titles.Add(p.Title);
            if (p.Mode != IdePanelMode.AutoHide)
                allAutoHide = false;
        }

        if (!mounted)
            return _regions.Remove(position);

        var mode = allAutoHide ? IdePanelMode.AutoHide : IdePanelMode.Docked;
        if (_regions.TryGetValue(position, out var current) && current.Mode == mode && current.Titles.SequenceEqual(titles))
            return false;
        _regions[position] = (titles.ToArray(), mode);
        return true;
    }

    /// <summary>Collapses the region at <paramref name="position"/> to its rail.</summary>
    internal Task CollapseAsync(ToolPanelPosition position) => SetCollapsedAsync(position, true);

    private async Task SetCollapsedAsync(ToolPanelPosition position, bool collapsed)
    {
        switch (position)
        {
            case ToolPanelPosition.Right:
                RightCollapsed = collapsed;
                await RightCollapsedChanged.InvokeAsync(collapsed);
                break;
            case ToolPanelPosition.Bottom:
                BottomCollapsed = collapsed;
                await BottomCollapsedChanged.InvokeAsync(collapsed);
                break;
            default:
                LeftCollapsed = collapsed;
                await LeftCollapsedChanged.InvokeAsync(collapsed);
                break;
        }
        StateHasChanged();
    }

    private bool IsCollapsed(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Right => RightCollapsed,
        ToolPanelPosition.Bottom => BottomCollapsed,
        _ => LeftCollapsed,
    };

    // A region floats only when every panel in it asks for auto-hide; a mixed region stays docked.
    private IdePanelMode ModeOf(ToolPanelPosition position) =>
        _regions.TryGetValue(position, out var i) ? i.Mode : IdePanelMode.Docked;

    private string[] TitlesOf(ToolPanelPosition position) =>
        _regions.TryGetValue(position, out var i) ? i.Titles : [];

    // A region floats (overlay) when expanded in auto-hide mode.
    private bool IsFloating(ToolPanelPosition position) =>
        !IsCollapsed(position) && ModeOf(position) == IdePanelMode.AutoHide;

    private string _rootStyle =>
        $"{Css.Tokens.Ide.Layout.LeftWidth}:{EffectiveLeftWidth};{Css.Tokens.Ide.Layout.RightWidth}:{EffectiveRightWidth};" +
        $"{Css.Tokens.Ide.Layout.BottomHeight}:{EffectiveBottomHeight};{Css.Tokens.Ide.Layout.Rail}:{RailSize};{Style}";

    private string PanelClass(ToolPanelPosition position, string sideClass)
    {
        var cls = $"{Css.Classes.Ide.Layout.Panel} {sideClass}";
        if (IsCollapsed(position)) cls += $" {Css.Classes.Ide.Layout.PanelCollapsed}";
        if (IsFloating(position)) cls += $" {Css.Classes.Ide.Layout.PanelFloating}";
        return cls;
    }

    private FlareIcon RailIcon(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Right => FlareIcons.ChevronLeft,
        ToolPanelPosition.Bottom => FlareIcons.ExpandLess,
        _ => FlareIcons.ChevronRight,
    };
}
