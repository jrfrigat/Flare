using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

/// <summary>
/// Root layout component for an IDE-style interface. Arranges ribbon, menu bar,
/// tool panels, document area, bottom panel, and status bar in a Visual Studio-style layout.
/// All styling is composed from base Flare CSS classes and design tokens - no custom tokens.
/// The layout owns each region's collapsed state so a collapsed panel frees its docked space
/// (shrinking to a thin rail), and supports per-region auto-hide (flyout) panels. The bottom region
/// can draw a tool-window header with pin and collapse controls: pinned keeps it docked, unpinned
/// turns it into an auto-hide flyout.
/// </summary>
public partial class FlareIdeLayout : FlareLocalizedComponentBase
{
    /// <summary>Top ribbon area rendered above the menu bar.</summary>
    [Parameter] public RenderFragment? Ribbon { get; set; }

    /// <summary>Height of the layout frame as a CSS length (e.g. <c>"520px"</c> or <c>"100%"</c>).
    /// Null keeps the layout filling the viewport (<c>100vh</c>). Use it when the IDE is one panel of
    /// a larger page instead of the whole screen.</summary>
    [Parameter] public string? Height { get; set; }

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

    /// <summary>Whether the bottom region is pinned. The default is true: a pinned bottom panel stays
    /// docked and open. Set it to false for an auto-hide bottom panel - while expanded it overlays the
    /// content above the rail, a pointer press outside it or Escape collapses it, and a rail click
    /// reopens it. Bind it with <see cref="BottomPinnedChanged"/> to persist the state. The pin is
    /// authoritative for the bottom region, whose content is often a host surface rather than a
    /// <see cref="FlareToolPanel"/>; the side regions keep using <see cref="FlareToolPanel.Mode"/>.
    /// </summary>
    [Parameter] public bool BottomPinned { get; set; } = true;

    /// <summary>Reports the new bottom pin state when the built-in pin control, or a host command
    /// through the layout, changes it. Bind it to <see cref="BottomPinned"/>.</summary>
    [Parameter] public EventCallback<bool> BottomPinnedChanged { get; set; }

    /// <summary>Whether the expanded bottom region draws the built-in tool-window header above its
    /// content. The default is false, so existing layouts keep their chrome. The header renders
    /// <see cref="BottomHeaderContent"/>, the typed pin control (see <see cref="ShowBottomPin"/>), the
    /// collapse control (see <see cref="ShowBottomCollapse"/>), and keeps the panel content, resizer,
    /// height and scrolling unchanged. Nothing is drawn while the region is collapsed or when
    /// <see cref="BottomPanel"/> is null.</summary>
    [Parameter] public bool ShowBottomHeader { get; set; }

    /// <summary>Host content rendered at the inline start of the bottom header - a title, tabs or the
    /// host's own commands. The typed pin and collapse controls follow it at the inline end; hide
    /// either with <see cref="ShowBottomPin"/> or <see cref="ShowBottomCollapse"/> and draw a custom
    /// replacement here.</summary>
    [Parameter] public RenderFragment? BottomHeaderContent { get; set; }

    /// <summary>Whether the bottom header draws the built-in pin control. The default is true; it has
    /// no effect unless <see cref="ShowBottomHeader"/> is true. The control reflects
    /// <see cref="BottomPinned"/> and raises <see cref="BottomPinnedChanged"/>.</summary>
    [Parameter] public bool ShowBottomPin { get; set; } = true;

    /// <summary>Whether the bottom header draws the built-in collapse control. The default is true; it
    /// has no effect unless <see cref="ShowBottomHeader"/> is true. The control collapses the region
    /// through <see cref="BottomCollapsed"/> and the rail reopens it.</summary>
    [Parameter] public bool ShowBottomCollapse { get; set; } = true;

    /// <summary>How a collapsed or auto-hidden side region lists its tools.
    /// <see cref="IdeToolRailMode.Region"/> (the default) keeps one button for the whole region.
    /// <see cref="IdeToolRailMode.PerTool"/> gives an auto-hide side region one rail entry per hosted
    /// tool group tab, with the tool's icon and label: selecting an entry opens that tool in the
    /// flyout, the active entry is marked, and selecting it again closes the flyout. Docked regions,
    /// the bottom region and regions without a hosted tool group keep the region button.</summary>
    [Parameter] public IdeToolRailMode ToolRailMode { get; set; } = IdeToolRailMode.Region;

    /// <summary>Controlled key of the tool selected in the left rail in
    /// <see cref="IdeToolRailMode.PerTool"/> mode. The key is the hosted tool's effective key
    /// (<see cref="FlareToolPanel.Key"/>, or its title). Bind the same value to the hosted group's
    /// <see cref="FlareToolPanelGroup.ActiveKey"/> so the flyout shows the tool the rail selected; with
    /// it unset the rail marks the group's own selection. Bind it with
    /// <see cref="LeftActiveToolKeyChanged"/> to persist the selected tool in a host layout state.</summary>
    [Parameter] public string? LeftActiveToolKey { get; set; }

    /// <summary>Reports the tool the user selected on the left rail. Bind it to
    /// <see cref="LeftActiveToolKey"/> and feed the value to the hosted group's
    /// <see cref="FlareToolPanelGroup.ActiveKey"/>.</summary>
    [Parameter] public EventCallback<string?> LeftActiveToolKeyChanged { get; set; }

    /// <summary>Controlled key of the tool selected in the right rail in
    /// <see cref="IdeToolRailMode.PerTool"/> mode; the counterpart of <see cref="LeftActiveToolKey"/>.</summary>
    [Parameter] public string? RightActiveToolKey { get; set; }

    /// <summary>Reports the tool the user selected on the right rail. Bind it to
    /// <see cref="RightActiveToolKey"/> and feed the value to the hosted group's
    /// <see cref="FlareToolPanelGroup.ActiveKey"/>.</summary>
    [Parameter] public EventCallback<string?> RightActiveToolKeyChanged { get; set; }

    /// <summary>Width of the thin rail shown for a collapsed or auto-hidden panel. Default 32px.</summary>
    [Parameter] public string RailSize { get; set; } = "32px";

    /// <summary>Whether the left region is rendered. When false, the layout draws no left panel, dock
    /// strip, documents or resizer, and its documents stay in <see cref="Documents"/> unmounted; the
    /// document area takes the freed space. Unlike <see cref="LeftCollapsed"/>, a hidden region keeps no
    /// rail, so it represents a View command that closes the dock. Default true.</summary>
    [Parameter] public bool LeftRegionVisible { get; set; } = true;

    /// <summary>Whether the right region is rendered. When false, the layout draws no right panel, dock
    /// strip, documents or resizer, and its documents stay in <see cref="Documents"/> unmounted; the
    /// document area takes the freed space. Unlike <see cref="RightCollapsed"/>, a hidden region keeps no
    /// rail, so it represents a View command that closes the dock. Default true.</summary>
    [Parameter] public bool RightRegionVisible { get; set; } = true;

    /// <summary>Whether the left region can be drag-resized (a divider sits between it and the document).</summary>
    [Parameter] public bool LeftResizable { get; set; } = true;
    /// <summary>Whether the right region can be drag-resized.</summary>
    [Parameter] public bool RightResizable { get; set; } = true;
    /// <summary>Whether the bottom region can be drag-resized.</summary>
    [Parameter] public bool BottomResizable { get; set; } = true;

    /// <summary>Smallest size (px) a region can be dragged to. Default 120.</summary>
    [Parameter] public int MinPanelSize { get; set; } = 120;


    protected override string ComponentCssClass => Css.Classes.Ide.Layout.Root;


    // Live drag-resized sizes (px); null until the user drags, then they override the *Width/*Height params.
    private int? _leftPx;
    private int? _rightPx;
    private int? _bottomPx;

    private string EffectiveLeftWidth => _leftPx is { } v ? $"{v}px" : LeftPanelWidth;
    private string EffectiveRightWidth => _rightPx is { } v ? $"{v}px" : RightPanelWidth;
    private string EffectiveBottomHeight => _bottomPx is { } v ? $"{v}px" : BottomPanelHeight;

    private bool ShowResizer(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Left => LeftRegionVisible && (LeftToolPanel is not null || Docking) && LeftResizable && !LeftCollapsed && !IsFloating(ToolPanelPosition.Left),
        ToolPanelPosition.Right => RightRegionVisible && (RightToolPanel is not null || Docking) && RightResizable && !RightCollapsed && !IsFloating(ToolPanelPosition.Right),
        ToolPanelPosition.Bottom => BottomPanel is not null && BottomResizable && !BottomCollapsed && !IsFloating(ToolPanelPosition.Bottom),
        _ => false,
    };

    /// <summary>Sets a docked region size in pixels. Pointer and keyboard interaction are handled by FlareSplitter.</summary>
    /// <param name="position">The region being resized.</param>
    /// <param name="sizePx">The new size in pixels.</param>
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

    // The region's size in pixels for the divider's aria-valuenow: the dragged size, else a "...px" parameter.
    // A size in other units (rem, %) has no pixel value here, and the divider then announces no value.
    private int? SizePx(ToolPanelPosition position)
    {
        var (dragged, param) = position switch
        {
            ToolPanelPosition.Right => (_rightPx, RightPanelWidth),
            ToolPanelPosition.Bottom => (_bottomPx, BottomPanelHeight),
            _ => (_leftPx, LeftPanelWidth),
        };
        if (dragged is { } px) return px;
        var s = param?.Trim();
        return s is not null && s.EndsWith("px", StringComparison.Ordinal)
            && int.TryParse(s[..^2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static int ParsePx(string value, int fallback)
    {
        var s = value?.Replace("px", "").Trim();
        return int.TryParse(s, out var v) ? v : fallback;
    }

    private static string SizeVariable(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Right => Css.Tokens.Ide.Layout.RightWidth,
        ToolPanelPosition.Bottom => Css.Tokens.Ide.Layout.BottomHeight,
        _ => Css.Tokens.Ide.Layout.LeftWidth,
    };

    private static string SizeTarget(ToolPanelPosition position) => position == ToolPanelPosition.Bottom
        ? $":scope > .{Css.Classes.Ide.Layout.Bottom}"
        : $":scope > .{Css.Classes.Ide.Layout.Body} > ." + (position switch
    {
        ToolPanelPosition.Right => Css.Classes.Ide.Layout.PanelRight,
        _ => Css.Classes.Ide.Layout.PanelLeft,
    });

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _disposed = true;
        await DisposeFlyoutAsync();
        if (_ideModule is not null)
        {
            try { await _ideModule.DisposeAsync(); }
            catch (JSDisconnectedException) { }
            catch (JSException) { }
        }
        await base.DisposeAsync();
    }

    // Panels mounted in the layout, in first-registration (markup) order. A region may host several.
    private readonly List<PanelRegistration> _panels = new();

    // Tools, titles and mode of each region, derived from all of its mounted panels. Kept while a
    // collapsed region has no panel mounted, so the rail keeps its labels after the panels unmount.
    private readonly Dictionary<ToolPanelPosition, RegionState> _regions = new();

    // Last effective key each hosted group reported. The per-tool rail marks it while the host does
    // not control the side's active tool key.
    private readonly Dictionary<ToolPanelPosition, string?> _reportedActiveTool = new();

    // Enter/leave depth of a document drag over a rail: moving across the per-tool buttons enters and
    // leaves nested children, which must not clear the highlight until the pointer leaves the rail.
    private ToolPanelPosition? _railDragPosition;
    private int _railDragDepth;

    private bool _disposed;

    private sealed record PanelRegistration(FlareToolPanel Panel, ToolPanelPosition Position,
        string? Title, IdePanelMode Mode, string? Key, FlareIcon? Icon);

    private sealed record RegionTool(string? Key, string? Title, FlareIcon? Icon);

    private sealed record RegionState(RegionTool[] Tools, IdePanelMode Mode);

    /// <summary>Called by a hosted <see cref="FlareToolPanel"/> to report its region, title, mode,
    /// key and icon.</summary>
    internal void RegisterPanel(FlareToolPanel panel, ToolPanelPosition position, string? title,
        IdePanelMode mode, string? key, FlareIcon? icon)
    {
        var index = _panels.FindIndex(p => ReferenceEquals(p.Panel, panel));
        ToolPanelPosition? previous = null;
        if (index >= 0)
        {
            var entry = _panels[index];
            if (entry.Position == position && entry.Title == title && entry.Mode == mode &&
                entry.Key == key && Equals(entry.Icon, icon))
                return;
            previous = entry.Position;
            _panels[index] = new(panel, position, title, mode, key, icon);
        }
        else
        {
            _panels.Add(new(panel, position, title, mode, key, icon));
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
        _railEntries.Remove(panel);
        // A collapsing region unmounts its panels one by one; its rail keeps what they reported while
        // expanded. In per-tool mode the body stays mounted while collapsed, so the rail must follow
        // even then.
        if ((!IsCollapsed(position) || IsPerToolRailMode(position)) &&
            UpdateRegion(position) && !_disposed)
            StateHasChanged();
    }

    // Recomputes a region from its mounted panels; returns true when its tools or mode changed. The result
    // depends only on the set of panels, not on the order their parameters were set within a render, so
    // re-rendering settles instead of alternating between panels.
    private bool UpdateRegion(ToolPanelPosition position)
    {
        var tools = new List<RegionTool>();
        var mounted = false;
        var allAutoHide = true;
        foreach (var p in _panels)
        {
            if (p.Position != position)
                continue;
            mounted = true;
            tools.Add(new(p.Key, p.Title, p.Icon));
            if (p.Mode != IdePanelMode.AutoHide)
                allAutoHide = false;
        }

        if (!mounted)
            return _regions.Remove(position);

        var mode = allAutoHide ? IdePanelMode.AutoHide : IdePanelMode.Docked;
        if (_regions.TryGetValue(position, out var current) && current.Mode == mode &&
            current.Tools.SequenceEqual(tools))
            return false;
        _regions[position] = new(tools.ToArray(), mode);
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

    // A rail is the control of the region it fronts: it expands a collapsed region and closes an
    // auto-hide flyout that is already open.
    private Task ToggleRailAsync(ToolPanelPosition position) =>
        SetCollapsedAsync(position, !IsCollapsed(position));

    private async Task SetBottomPinnedAsync(bool pinned)
    {
        BottomPinned = pinned;
        await BottomPinnedChanged.InvokeAsync(pinned);
        StateHasChanged();
    }

    private Task ToggleBottomPinAsync() => SetBottomPinnedAsync(!BottomPinned);

    private bool HasBottomHeader => ShowBottomHeader && BottomPanel is not null &&
        (BottomHeaderContent is not null || ShowBottomPin || ShowBottomCollapse);

    // The header keeps its place while the content scrolls; without a header the body keeps the
    // historical single scrolling box.
    private string BottomBodyClass() => HasBottomHeader
        ? $"{Css.Classes.Ide.Layout.PanelBody} {Css.Classes.Ide.Layout.BottomBody}"
        : Css.Classes.Ide.Layout.PanelBody;

    private bool IsCollapsed(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Right => RightCollapsed,
        ToolPanelPosition.Bottom => BottomCollapsed,
        _ => LeftCollapsed,
    };

    // A region floats only when every panel in it asks for auto-hide; a mixed region stays docked.
    private IdePanelMode ModeOf(ToolPanelPosition position) =>
        _regions.TryGetValue(position, out var i) ? i.Mode : IdePanelMode.Docked;

    // Non-empty titles of a region's tools, in registration order, for the region rail button.
    private string[] TitlesOf(ToolPanelPosition position) => _regions.TryGetValue(position, out var i)
        ? i.Tools.Where(tool => !string.IsNullOrEmpty(tool.Title)).Select(tool => tool.Title!).ToArray()
        : [];

    // A region floats (overlay) when expanded in auto-hide mode. The bottom region takes its mode
    // from the pin instead of the hosted panels: its content is often a host surface with no
    // FlareToolPanel to report a Mode.
    internal bool IsFloating(ToolPanelPosition position) =>
        !IsCollapsed(position) && (position == ToolPanelPosition.Bottom
            ? BottomPanel is not null && !BottomPinned
            : ModeOf(position) == IdePanelMode.AutoHide && !DocumentFillsRegion(position));

    // The per-tool rail is an opt-in for auto-hide side regions that host tool group tabs. Docked
    // regions, the bottom region and regions hosting standalone panels keep the region button.
    private bool IsPerToolRailMode(ToolPanelPosition position) =>
        ToolRailMode == IdeToolRailMode.PerTool && position != ToolPanelPosition.Bottom &&
        ModeOf(position) == IdePanelMode.AutoHide;

    private bool HasGroupedTools(ToolPanelPosition position) =>
        _panels.Any(p => p.Position == position && p.Panel.Group is not null);

    // In per-tool mode the side region always renders its rail slot and its body, so the panel
    // content stays mounted while the region is collapsed and the rail is populated from the first
    // registration. The body is hidden then; see the layout markup and CSS.
    private bool UsesPerToolBody(ToolPanelPosition position) =>
        ToolRailMode == IdeToolRailMode.PerTool && position != ToolPanelPosition.Bottom;

    private bool RendersToolRail(ToolPanelPosition position) =>
        IsPerToolRailMode(position) && HasGroupedTools(position);

    // The rail slot shows something only while the region occupies rail width: collapsed or
    // floating. In a docked expanded region it stays empty and hidden.
    private bool ShowsRail(ToolPanelPosition position) => IsCollapsed(position) || IsFloating(position);

    private bool RegionModeKnown(ToolPanelPosition position) => _regions.ContainsKey(position);

    // Tools listed on the per-tool rail, in registration order.
    private IEnumerable<PanelRegistration> RailToolsOf(ToolPanelPosition position) =>
        _panels.Where(p => p.Position == position && p.Panel.Group is not null);

    /// <summary>Called by a hosted <see cref="FlareToolPanelGroup"/> to report the effective key of
    /// its active tool. The per-tool rail marks it while the host does not control the side's
    /// active tool key.</summary>
    internal void ReportActiveTool(ToolPanelPosition position, string? key)
    {
        _reportedActiveTool.TryGetValue(position, out var current);
        if (string.Equals(current, key, StringComparison.Ordinal)) return;
        _reportedActiveTool[position] = key;
        if (RendersToolRail(position)) StateHasChanged();
    }

    private string? ActiveToolKey(ToolPanelPosition position)
    {
        // The bottom rail lists no per-tool entries, so it has no active tool to mark.
        if (position == ToolPanelPosition.Bottom) return null;
        var controlled = position == ToolPanelPosition.Right ? RightActiveToolKey : LeftActiveToolKey;
        if (!string.IsNullOrEmpty(controlled)) return controlled;
        return _reportedActiveTool.TryGetValue(position, out var reported) ? reported : null;
    }

    private bool IsActiveRailTool(ToolPanelPosition position, PanelRegistration tool) =>
        tool.Key is not null &&
        string.Equals(ActiveToolKey(position), tool.Key, StringComparison.Ordinal);

    // Selecting a rail entry opens its tool. The layout follows the same controlled contract as
    // every other region state: the local parameter moves for uncontrolled use and the callback lets
    // the host feed the value to the group's ActiveKey.
    private async Task SelectRailToolAsync(ToolPanelPosition position, PanelRegistration tool)
    {
        if (!RendersToolRail(position)) return;
        if (!IsCollapsed(position))
        {
            if (IsActiveRailTool(position, tool))
            {
                await SetCollapsedAsync(position, true);
                return;
            }
            await SetActiveToolAsync(position, tool.Key);
            return;
        }
        await SetActiveToolAsync(position, tool.Key);
        await SetCollapsedAsync(position, false);
    }

    private async Task SetActiveToolAsync(ToolPanelPosition position, string? key)
    {
        var current = position == ToolPanelPosition.Right ? RightActiveToolKey : LeftActiveToolKey;
        if (!string.Equals(current, key, StringComparison.Ordinal))
        {
            if (position == ToolPanelPosition.Right)
            {
                RightActiveToolKey = key;
                await RightActiveToolKeyChanged.InvokeAsync(key);
            }
            else
            {
                LeftActiveToolKey = key;
                await LeftActiveToolKeyChanged.InvokeAsync(key);
            }
        }
        StateHasChanged();
    }

    private string ToolRailClass(ToolPanelPosition position) =>
        _railDropTarget == position
            ? $"{Css.Classes.Ide.Layout.ToolRail} {Css.Classes.Ide.Layout.RailDropTarget}"
            : Css.Classes.Ide.Layout.ToolRail;

    private string ToolRailEntryClass(ToolPanelPosition position, PanelRegistration tool) =>
        IsActiveRailTool(position, tool)
            ? $"{Css.Classes.Ide.Layout.ToolRailEntry} {Css.Classes.Ide.Layout.ToolRailEntryActive}"
            : Css.Classes.Ide.Layout.ToolRailEntry;

    private string ToolRailLabel(PanelRegistration tool) =>
        string.IsNullOrWhiteSpace(tool.Title) ? IdeText["ToolPanelGroup_Untitled"].Value : tool.Title!;

    private string _rootStyle =>
        $"{Css.Tokens.Ide.Layout.LeftWidth}:{EffectiveLeftWidth};{Css.Tokens.Ide.Layout.RightWidth}:{EffectiveRightWidth};" +
        $"{Css.Tokens.Ide.Layout.BottomHeight}:{EffectiveBottomHeight};{Css.Tokens.Ide.Layout.Rail}:{RailSize};" +
        (string.IsNullOrWhiteSpace(Height) ? string.Empty : $"height:{Height};") + Style;

    private string BodyClass()
    {
        var cls = Css.Classes.Ide.Layout.Body;
        if (!LeftRegionVisible) cls += " " + Css.Classes.Ide.Layout.BodyHideLeft;
        if (!RightRegionVisible) cls += " " + Css.Classes.Ide.Layout.BodyHideRight;
        return cls;
    }

    private string PanelClass(ToolPanelPosition position, string sideClass)
    {
        var cls = $"{Css.Classes.Ide.Layout.Panel} {sideClass}";
        if (IsCollapsed(position)) cls += $" {Css.Classes.Ide.Layout.PanelCollapsed}";
        if (IsFloating(position)) cls += $" {Css.Classes.Ide.Layout.PanelFloating}";
        if (RendersToolRail(position)) cls += $" {Css.Classes.Ide.Layout.PanelToolRail}";
        return cls;
    }

    // A rail's icon names what its click does: opening a collapsed region, or closing the open
    // bottom flyout it fronts.
    private FlareIcon RailIcon(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Right => FlareIcons.ChevronLeft,
        ToolPanelPosition.Bottom => IsCollapsed(position) ? FlareIcons.ExpandLess : FlareIcons.ExpandMore,
        _ => FlareIcons.ChevronRight,
    };
}
