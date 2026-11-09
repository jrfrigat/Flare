using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// One projection on the shared axis of a <see cref="FlareGantt"/>: rows (a list or a tree) and the items, cells,
/// regions, markers and links drawn on them. The panel knows nothing about what a row or an item is - the
/// consumer supplies its own objects and selectors, and decides what the projection means.
/// </summary>
/// <typeparam name="TRow">The consumer's row type.</typeparam>
/// <typeparam name="TItem">The consumer's item type.</typeparam>
public partial class FlareGanttPanel<TRow, TItem> : IGanttPanel
{
    /// <summary>The axis container; a panel must sit inside <see cref="FlareGantt"/>.</summary>
    [CascadingParameter] public FlareGantt? Gantt { get; set; }

    /// <summary>Top-level rows.</summary>
    [Parameter, EditorRequired] public IEnumerable<TRow>? Rows { get; set; }

    /// <summary>Unique key of a row.</summary>
    [Parameter, EditorRequired] public Func<TRow, string> RowKey { get; set; } = default!;

    /// <summary>Child rows; null or empty for a leaf. Turns the panel into a tree.</summary>
    [Parameter] public Func<TRow, IEnumerable<TRow>?>? RowChildren { get; set; }

    /// <summary>Content of the row-header cell; the row key when null.</summary>
    [Parameter] public RenderFragment<TRow>? RowHeader { get; set; }

    /// <summary>Extra CSS class for a row.</summary>
    [Parameter] public Func<TRow, string?>? RowClass { get; set; }

    /// <summary>Whether tree rows start expanded.</summary>
    [Parameter] public bool ExpandedByDefault { get; set; } = true;

    /// <summary>Items drawn as bars.</summary>
    [Parameter] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>Unique key of an item; shared across panels so that selection links projections.</summary>
    [Parameter] public Func<TItem, string>? ItemKey { get; set; }

    /// <summary>Key of the row an item is drawn on.</summary>
    [Parameter] public Func<TItem, string>? ItemRow { get; set; }

    /// <summary>Span of an item; items without a span are not drawn.</summary>
    [Parameter] public Func<TItem, GanttSpan?>? ItemSpan { get; set; }

    /// <summary>Kind key of an item, rendered as a CSS modifier and <c>data-kind</c>.</summary>
    [Parameter] public Func<TItem, string?>? ItemKind { get; set; }

    /// <summary>Extra CSS class for a bar, appended next to the kind and structural classes. It is called
    /// for item-backed bars, including suggested ghosts that carry a source item; a summary bar has no item
    /// and is not called. Null or empty leaves the class list unchanged.</summary>
    [Parameter] public Func<TItem, string?>? ItemClass { get; set; }

    /// <summary>Parts of an item's bar, such as work and waiting.</summary>
    [Parameter] public Func<TItem, IEnumerable<GanttSegment>?>? ItemSegments { get; set; }

    /// <summary>Completion of an item from 0 to 1, drawn as a fill; null for none.</summary>
    [Parameter] public Func<TItem, double?>? ItemProgress { get; set; }

    /// <summary>Accessible name and default text of an item; the key when null.</summary>
    [Parameter] public Func<TItem, string?>? ItemLabel { get; set; }

    /// <summary>Content inside a bar; the label when null.</summary>
    [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
    /// <summary>Replaces item interaction content inside the geometry and gesture boundary. Own activation or use its standard commands; the item bar becomes a group. Mark secondary actions with data-gantt-no-drag to exclude them from pointer gestures.</summary>
    [Parameter] public RenderFragment<GanttItemContext<TItem>>? ItemInteractionTemplate { get; set; }

    /// <summary>Rich tooltip of a bar, shown in a popover on hover and focus.</summary>
    [Parameter] public RenderFragment<TItem>? ItemTooltip { get; set; }

    /// <summary>
    /// Plain hover text of an item, such as its dates and why it is at risk: the bar's <c>title</c>, so it costs nothing on a
    /// chart of hundreds of bars. The label when null; ignored when <see cref="ItemTooltip"/> is set.
    /// </summary>
    [Parameter] public Func<TItem, string?>? ItemTitle { get; set; }

    /// <summary>Raised when a bar is clicked.</summary>
    [Parameter] public EventCallback<TItem> OnItemClick { get; set; }

    /// <summary>Draws a summary bar over the descendants' items on tree rows that have no items of their own.</summary>
    [Parameter] public bool RollupParents { get; set; }

    /// <summary>Kind key of summary bars.</summary>
    [Parameter] public string SummaryKind { get; set; } = "summary";

    /// <summary>Dependencies between items.</summary>
    [Parameter] public IEnumerable<GanttLink>? Links { get; set; }

    /// <summary>Link types to draw; all when null.</summary>
    [Parameter] public IReadOnlyCollection<GanttLinkType>? LinkTypes { get; set; }

    /// <summary>Highlights the items and links with no slack in the current schedule.</summary>
    [Parameter] public bool ShowCriticalPath { get; set; }

    /// <summary>Per-row values along the axis, such as hours per day with a status; drawn in a strip above the lanes.</summary>
    [Parameter] public IEnumerable<GanttCell>? Cells { get; set; }

    /// <summary>Content of a cell; its text when null.</summary>
    [Parameter] public RenderFragment<GanttCell>? CellTemplate { get; set; }

    /// <summary>Extra CSS class for a cell, appended next to the kind and structural classes.</summary>
    [Parameter] public Func<GanttCell, string?>? CellClass { get; set; }

    /// <summary>Regions of this panel; those without a row key span the whole panel.</summary>
    [Parameter] public IEnumerable<GanttRegion>? Regions { get; set; }

    /// <summary>Extra CSS class for a region, appended next to the kind and structural classes. It also
    /// applies to chart-level regions that span every row, because the panel renders them.</summary>
    [Parameter] public Func<GanttRegion, string?>? RegionClass { get; set; }

    /// <summary>Markers of this panel; those without a row key span the whole panel.</summary>
    [Parameter] public IEnumerable<GanttMarker>? Markers { get; set; }

    /// <summary>Extra CSS class for a marker, appended next to the kind and structural classes. It also
    /// applies to chart-level markers that span every row, because the panel renders them.</summary>
    [Parameter] public Func<GanttMarker, string?>? MarkerClass { get; set; }

    /// <summary>Proposed changes drawn as ghosts, without being applied.</summary>
    [Parameter] public IEnumerable<GanttChange>? Suggested { get; set; }

    /// <summary>Title row of the panel.</summary>
    [Parameter] public RenderFragment? Header { get; set; }

    /// <summary>Accessible name of the panel's grid.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Gantt.Panel;

    private readonly Dictionary<string, bool> _expanded = [];
    private bool? _expandAll;
    private readonly string _id = "g" + Guid.NewGuid().ToString("N")[..8];

    private List<GanttRowLayout<TRow, TItem>> _rows = [];
    private List<GanttRowLayout<TRow, TItem>> _drawnRows = [];
    private readonly List<GanttRowLayout<TRow, TItem>> _windowRows = [];
    private readonly HashSet<string> _windowKeys = [];
    private bool _viewportOnly;
    private bool _hasLayout;
    private readonly List<GanttLinkLayout> _links = [];
    private List<GanttRegion> _panelRegions = [];
    private List<GanttMarker> _panelMarkers = [];
    private readonly List<GanttLink> _layoutLinks = [];
    private readonly List<GanttLink> _criticalLinks = [];
    private readonly Dictionary<string, GanttSpan> _criticalSpans = [];
    private static readonly IReadOnlySet<string> EmptyCritical = new HashSet<string>();
    private IReadOnlySet<string> _critical = EmptyCritical;
    private readonly Dictionary<string, GanttBarLayout<TItem>> _bars = [];
    private double _height;
    private bool _isTree;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Gantt is null) throw new InvalidOperationException($"{nameof(FlareGanttPanel<TRow, TItem>)} must be placed inside {nameof(FlareGantt)}.");
        Gantt.Register(this);
    }

    /// <inheritdoc />
    protected override void OnParametersSet() => _viewportOnly = false;

    /// <summary>Expands every tree row.</summary>
    public void ExpandAll() { _expanded.Clear(); _expandAll = true; RenderLayout(); }

    /// <summary>Collapses every tree row.</summary>
    public void CollapseAll() { _expanded.Clear(); _expandAll = false; RenderLayout(); }

    void IGanttPanel.Refresh() => InvokeAsync(RenderLayout);

    void IGanttPanel.RefreshViewport() => InvokeAsync(() =>
    {
        _viewportOnly = _hasLayout;
        StateHasChanged();
    });

    private void RenderLayout()
    {
        _viewportOnly = false;
        StateHasChanged();
    }

    string IGanttPanel.Id => _id;

    bool IGanttPanel.Uncovered(double from, double to) => Gantt!.Virtualize && (from < _keepFrom || to > _keepTo);

    // Before the browser has told what is visible, a virtualized panel draws its first screen.
    private const double FirstScreen = 1200;
    private double _spacer;
    private double _keepFrom = double.MinValue;
    private double _keepTo = double.MaxValue;

    // The rows to draw: all of them, or with Virtualize those within a screen above and below the visible part; the
    // rows skipped above are one spacer. Redrawn once the visible part has moved half a screen from where it was.
    private List<GanttRowLayout<TRow, TItem>> DrawnRows()
    {
        _spacer = 0;
        _windowRows.Clear();
        _windowKeys.Clear();
        if (!Gantt!.Virtualize) return _rows;
        var (from, to) = Gantt.VisibleOf(_id) ?? (0, FirstScreen);
        var margin = Math.Max(600, to - from);
        _keepFrom = from - margin / 2;
        _keepTo = to + margin / 2;
        foreach (var row in _rows)
        {
            if (!(row.Top + row.Height > from - margin && row.Top < to + margin)) continue;
            _windowRows.Add(row);
            _windowKeys.Add(row.Key);
        }
        _spacer = _windowRows.Count > 0 ? _windowRows[0].Top : 0;
        return _windowRows;
    }

    private bool IsExpanded(string key) => _expanded.TryGetValue(key, out var value) ? value : _expandAll ?? ExpandedByDefault;

    private void Toggle(string key)
    {
        _viewportOnly = false;
        _expanded[key] = !IsExpanded(key);
    }

    private void Layout()
    {
        if (!_viewportOnly) BuildLayout();
        _viewportOnly = false;
        _hasLayout = true;
        ProjectViewport();
    }

    private void BuildLayout()
    {
        var gantt = Gantt!;
        var rows = new List<GanttRowLayout<TRow, TItem>>();
        Dictionary<string, List<string>>? descendants = null;
        _isTree = false;
        if (RowChildren is { } children)
        {
            // Only summary bars need a walk through collapsed descendants.
            if (RollupParents) descendants = [];
            var visible = TreeFlattener.Flatten(Rows ?? [], children, row => IsExpanded(RowKey(row)),
                descendants is null ? null : (row, ancestors) =>
                {
                    var key = RowKey(row);
                    descendants[key] = [];
                    foreach (var ancestor in ancestors) descendants[RowKey(ancestor)].Add(key);
                });
            rows.EnsureCapacity(visible.Count);
            foreach (var node in visible)
            {
                _isTree |= node.HasChildren;
                rows.Add(new()
                {
                    Index = rows.Count, Key = RowKey(node.Item), Row = node.Item, Depth = node.Depth,
                    HasChildren = node.HasChildren, Expanded = node.HasChildren && node.IsExpanded,
                });
            }
        }
        else
        {
            var roots = Rows ?? [];
            if (roots.TryGetNonEnumeratedCount(out var count)) rows.EnsureCapacity(count);
            foreach (var row in roots)
                rows.Add(new() { Index = rows.Count, Key = RowKey(row), Row = row });
        }

        var byKey = rows.ToDictionary(r => r.Key);
        var suggested = Suggested;
        List<TItem>? suggestedItems = suggested is null ? null : [];
        Dictionary<string, List<(TItem Item, GanttSpan Span)>>? summaryItems = descendants is null ? null : [];
        if (ItemKey is { } keyOf && ItemRow is { } rowOf && ItemSpan is { } spanOf)
        {
            foreach (var item in Items ?? [])
            {
                if (spanOf(item) is not { } span) continue;
                var rowKey = rowOf(item);
                suggestedItems?.Add(item);
                if (summaryItems is not null)
                {
                    if (!summaryItems.TryGetValue(rowKey, out var group))
                        summaryItems.Add(rowKey, group = []);
                    group.Add((item, span));
                }
                if (byKey.TryGetValue(rowKey, out var row))
                {
                    var key = keyOf(item);
                    row.Bars.Add(new GanttBarLayout<TItem>
                    {
                        Key = key, RowKey = row.Key, Item = item, Span = span,
                        Kind = ItemKind?.Invoke(item), Label = ItemLabel?.Invoke(item) ?? key,
                        Segments = ItemSegments?.Invoke(item)?.ToList() ?? [], Progress = ItemProgress?.Invoke(item)
                    });
                }
            }
        }
        if (summaryItems is not null)
        {
            foreach (var row in rows)
            {
                if (!row.HasChildren || row.Bars.Count > 0) continue;
                var under = descendants![row.Key].SelectMany(k => summaryItems.TryGetValue(k, out var group) ? group : []).ToList();
                if (GanttRollup.Span(under.Select(i => i.Span)) is { } span)
                    row.Bars.Add(new GanttBarLayout<TItem>
                    {
                        Key = row.Key, RowKey = row.Key, Span = span, Kind = SummaryKind, IsSummary = true, Label = null,
                        Progress = ItemProgress is null ? null : GanttRollup.Progress(under.Select(i => (i.Span, ItemProgress(i.Item) ?? 0)))
                    });
            }
        }

        foreach (var change in suggested ?? [])
        {
            var source = suggestedItems!.FirstOrDefault(i => ItemKey!(i) == change.ItemKey);
            var rowKey = change.RowKey ?? (source is null ? null : ItemRow!(source));
            if (rowKey is null || !byKey.TryGetValue(rowKey, out var target)) continue;
            target.Bars.Add(new GanttBarLayout<TItem>
            {
                Key = change.ItemKey, RowKey = rowKey, Item = source, Span = change.Span, IsSuggested = true,
                Kind = source is null ? null : ItemKind?.Invoke(source), Label = source is null ? change.ItemKey : ItemLabel?.Invoke(source) ?? change.ItemKey
            });
        }

        foreach (var cell in Cells ?? []) if (byKey.TryGetValue(cell.RowKey, out var row)) row.Cells.Add(cell);
        _panelRegions = [.. (gantt.Regions ?? []).Concat(Regions ?? []).Where(r => r.RowKey is null)];
        foreach (var region in Regions ?? []) if (region.RowKey is { } k && byKey.TryGetValue(k, out var row)) row.Regions.Add(region);
        _panelMarkers = [.. (gantt.Markers ?? []).Concat(Markers ?? []).Where(m => m.RowKey is null)];
        foreach (var marker in Markers ?? []) if (marker.RowKey is { } k && byKey.TryGetValue(k, out var row)) row.Markers.Add(marker);

        // Geometry: lanes per row, then cumulative tops.
        var top = 0d;
        var lane = gantt.RowHeight;
        var inset = Math.Min(gantt.BarInset, lane / 2 - 1);
        _bars.Clear();
        foreach (var row in rows)
        {
            var layout = row.Bars.Count > 1 ? GanttLanes.Assign(row.Bars.Select((b, i) => (i, b.Span))) : null;
            row.LanesTop = row.Cells.Count > 0 ? lane : 0;
            row.Top = top;
            row.Height = row.LanesTop + (layout?.Count ?? 1) * lane;
            for (var i = 0; i < row.Bars.Count; i++)
            {
                var bar = row.Bars[i];
                bar.Lane = layout is null ? 0 : layout.Lanes[i];
                var x = gantt.X(bar.Span.Start);
                var width = Math.Max(2, gantt.X(bar.Span.End) - x);
                var y = row.LanesTop + bar.Lane * lane + inset;
                bar.Local = new GanttBox(x, y, width, lane - 2 * inset);
                bar.Box = bar.Local with { Y = top + y };
                if (!bar.IsSuggested) _bars.TryAdd(bar.Key, bar);
            }
            top += row.Height;
        }
        _height = top;
        _rows = rows;

        _layoutLinks.Clear();
        if (ShowCriticalPath && Links is { } links)
            _layoutLinks.AddRange(links);
        UpdateCriticalPath();
    }

    private void ProjectViewport()
    {
        var gantt = Gantt!;
        _drawnRows = DrawnRows();
        if (_rowFocusKey is null && !_drawnRows.Any(row => row.Key == _activeRowKey))
            _activeRowKey = _drawnRows.FirstOrDefault()?.Key;
        _links.Clear();
        foreach (var link in ShowCriticalPath ? _layoutLinks : Links ?? [])
        {
            if (LinkTypes is not null && !LinkTypes.Contains(link.Type)) continue;
            if (!_bars.TryGetValue(link.FromKey, out var from) || !_bars.TryGetValue(link.ToKey, out var to)) continue;
            if (gantt.Virtualize && (!_windowKeys.Contains(from.RowKey) || !_windowKeys.Contains(to.RowKey))) continue;
            _links.Add(new(link, GanttRouting.Path(from.Box, to.Box, link.Type, gantt.LinkGap),
                ShowCriticalPath && _critical.Contains(link.FromKey) && _critical.Contains(link.ToKey)));
        }
    }

    private void UpdateCriticalPath()
    {
        if (!ShowCriticalPath)
        {
            _criticalSpans.Clear();
            _criticalLinks.Clear();
            _critical = EmptyCritical;
            return;
        }

        // Compare the projected spans and edge values. The application can mutate its sources
        // in place, while a scroll or cosmetic render leaves this graph unchanged.
        var changed = !_layoutLinks.SequenceEqual(_criticalLinks);
        var count = 0;
        foreach (var bar in _bars.Values)
        {
            if (bar.IsSummary) continue;
            count++;
            if (!_criticalSpans.TryGetValue(bar.Key, out var span) || span != bar.Span)
                changed = true;
        }
        if (!changed && count == _criticalSpans.Count) return;

        _criticalSpans.Clear();
        foreach (var bar in _bars.Values)
            if (!bar.IsSummary)
                _criticalSpans.Add(bar.Key, bar.Span);
        _criticalLinks.Clear();
        _criticalLinks.AddRange(_layoutLinks);
        _critical = GanttGraph.CriticalPath(_criticalSpans, _criticalLinks);
    }

    private string BarClass(GanttBarLayout<TItem> bar)
    {
        var classes = new List<string> { Css.Classes.Gantt.Bar, Css.Classes.Gantt.Kind(bar.Kind) };
        if (bar.IsSummary) classes.Add(Css.Classes.Gantt.BarSummary);
        if (bar.IsSuggested) classes.Add(Css.Classes.Gantt.BarSuggested);
        if (_linkFrom == bar.Key) classes.Add(Css.Classes.Gantt.BarLinkSource);
        if (!bar.IsSuggested && Gantt!.Selected == bar.Key) classes.Add(Css.Classes.Gantt.BarSelected);
        if (ShowCriticalPath && _critical.Contains(bar.Key)) classes.Add(Css.Classes.Gantt.BarCritical);
        if (!bar.IsSummary && bar.Item is { } item && ItemClass?.Invoke(item) is { Length: > 0 } itemClass)
            classes.Add(itemClass);
        return string.Join(' ', classes.Where(c => c.Length > 0));
    }

    private string LinkClass(GanttLinkLayout link) => string.Join(' ', new[]
    {
        Css.Classes.Gantt.Link, Css.Classes.Gantt.LinkType(link.Link.Type), Css.Classes.Gantt.Kind(link.Link.Kind),
        link.Critical ? Css.Classes.Gantt.LinkCritical : ""
    }.Where(c => c.Length > 0));

    private string SpanStyle(GanttSpan span)
    {
        var x = Gantt!.X(span.Start);
        return $"left:{GanttFormat.Px(x)};width:{GanttFormat.Px(Math.Max(1, Gantt.X(span.End) - x))}";
    }

    // A bar far off the drawn columns is skipped - unless it is the one selected or being dragged.
    private bool Drawn(GanttBarLayout<TItem> bar) =>
        Gantt!.InView(bar.Local.X, bar.Local.X + bar.Local.Width) || bar.Key == Gantt.Selected;

    private string BarStyle(GanttBarLayout<TItem> bar) =>
        $"left:{GanttFormat.Px(bar.Local.X)};top:{GanttFormat.Px(bar.Local.Y)};width:{GanttFormat.Px(bar.Local.Width)};height:{GanttFormat.Px(bar.Local.Height)}";

    private string SegmentStyle(GanttBarLayout<TItem> bar, GanttSegment segment)
    {
        var x = Gantt!.X(segment.Span.Start) - bar.Local.X;
        return $"left:{GanttFormat.Px(x)};width:{GanttFormat.Px(Math.Max(1, Gantt.X(segment.Span.End) - Gantt.X(segment.Span.Start)))}";
    }

    private string CellStyle(GanttCell cell) => $"{SpanStyle(cell.Span)};height:{GanttFormat.Px(Gantt!.RowHeight)}";

    private async Task Click(GanttBarLayout<TItem> bar)
    {
        if (bar.IsSuggested) return;
        await Gantt!.Select(bar.Key);
        if (bar.Item is { } item && !bar.IsSummary) await OnItemClick.InvokeAsync(item);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        Gantt?.Unregister(this);
        await DisposeRowKeyboardAsync();
        await base.DisposeAsync();
    }
}
