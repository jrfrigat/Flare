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

    /// <summary>Parts of an item's bar, such as work and waiting.</summary>
    [Parameter] public Func<TItem, IEnumerable<GanttSegment>?>? ItemSegments { get; set; }

    /// <summary>Completion of an item from 0 to 1, drawn as a fill; null for none.</summary>
    [Parameter] public Func<TItem, double?>? ItemProgress { get; set; }

    /// <summary>Accessible name and default text of an item; the key when null.</summary>
    [Parameter] public Func<TItem, string?>? ItemLabel { get; set; }

    /// <summary>Content inside a bar; the label when null.</summary>
    [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }

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

    /// <summary>Regions of this panel; those without a row key span the whole panel.</summary>
    [Parameter] public IEnumerable<GanttRegion>? Regions { get; set; }

    /// <summary>Markers of this panel; those without a row key span the whole panel.</summary>
    [Parameter] public IEnumerable<GanttMarker>? Markers { get; set; }

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
    private List<GanttLinkLayout> _links = [];
    private List<GanttRegion> _panelRegions = [];
    private List<GanttMarker> _panelMarkers = [];
    private HashSet<string> _critical = [];
    private bool _criticalDirty = true;
    private Dictionary<string, GanttBarLayout<TItem>> _bars = [];
    private double _height;
    private bool _isTree;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Gantt is null) throw new InvalidOperationException($"{nameof(FlareGanttPanel<TRow, TItem>)} must be placed inside {nameof(FlareGantt)}.");
        Gantt.Register(this);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Any parameter push can change the graph the critical path is computed from; scrolling cannot.
        _criticalDirty = true;
    }

    /// <summary>Expands every tree row.</summary>
    public void ExpandAll() { _expanded.Clear(); _expandAll = true; _criticalDirty = true; StateHasChanged(); }

    /// <summary>Collapses every tree row.</summary>
    public void CollapseAll() { _expanded.Clear(); _expandAll = false; _criticalDirty = true; StateHasChanged(); }

    void IGanttPanel.Refresh() => InvokeAsync(StateHasChanged);

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
        if (!Gantt!.Virtualize) return _rows;
        var (from, to) = Gantt.VisibleOf(_id) ?? (0, FirstScreen);
        var margin = Math.Max(600, to - from);
        _keepFrom = from - margin / 2;
        _keepTo = to + margin / 2;
        var drawn = _rows.Where(r => r.Top + r.Height > from - margin && r.Top < to + margin).ToList();
        _spacer = drawn.Count > 0 ? drawn[0].Top : 0;
        return drawn;
    }

    private bool IsExpanded(string key) => _expanded.TryGetValue(key, out var value) ? value : _expandAll ?? ExpandedByDefault;

    private void Toggle(string key)
    {
        _expanded[key] = !IsExpanded(key);
        _criticalDirty = true;
    }

    // Everything the markup draws is computed here, from the parameters and the axis, on every render.
    private void Layout()
    {
        var gantt = Gantt!;
        // Every row's descendants, hidden ones included, for the summary bars; the visible rows come from the
        // tree walk every tree-shaped Flare view shares.
        var descendants = new Dictionary<string, List<string>>();
        _isTree = false;
        var visible = TreeFlattener.Flatten(Rows ?? [], row => RowChildren?.Invoke(row), row => IsExpanded(RowKey(row)), (row, ancestors) =>
        {
            var key = RowKey(row);
            descendants[key] = [];
            if (ancestors.Count > 0) _isTree = true;
            foreach (var ancestor in ancestors) descendants[RowKey(ancestor)].Add(key);
        });
        var rows = visible.Select(n => new GanttRowLayout<TRow, TItem>
        {
            Key = RowKey(n.Item), Row = n.Item, Depth = n.Depth, HasChildren = n.HasChildren, Expanded = n.HasChildren && n.IsExpanded,
        }).ToList();

        var byKey = rows.ToDictionary(r => r.Key);
        var items = (Items ?? []).Where(i => ItemKey is not null && ItemRow is not null && ItemSpan?.Invoke(i) is not null).ToList();
        string RowOf(TItem i) => ItemRow!(i);
        GanttSpan SpanOf(TItem i) => ItemSpan!(i)!.Value;
        var itemsByRow = items.GroupBy(RowOf).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var row in rows)
        {
            if (itemsByRow.TryGetValue(row.Key, out var own))
                foreach (var item in own)
                    row.Bars.Add(new GanttBarLayout<TItem>
                    {
                        Key = ItemKey!(item), RowKey = row.Key, Item = item, Span = SpanOf(item),
                        Kind = ItemKind?.Invoke(item), Label = ItemLabel?.Invoke(item) ?? ItemKey!(item),
                        Segments = ItemSegments?.Invoke(item)?.ToList() ?? [], Progress = ItemProgress?.Invoke(item)
                    });
            else if (RollupParents && row.HasChildren)
            {
                var under = descendants[row.Key].SelectMany(k => itemsByRow.TryGetValue(k, out var list) ? list : []).ToList();
                if (GanttRollup.Span(under.Select(i => ItemSpan!(i)!.Value)) is { } span)
                    row.Bars.Add(new GanttBarLayout<TItem>
                    {
                        Key = row.Key, RowKey = row.Key, Span = span, Kind = SummaryKind, IsSummary = true, Label = null,
                        Progress = ItemProgress is null ? null : GanttRollup.Progress(under.Select(i => (ItemSpan!(i)!.Value, ItemProgress(i) ?? 0)))
                    });
            }
        }

        foreach (var change in Suggested ?? [])
        {
            var source = items.FirstOrDefault(i => ItemKey!(i) == change.ItemKey);
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
        _bars = [];
        foreach (var row in rows)
        {
            var layout = GanttLanes.Assign(row.Bars.Select((b, i) => (i, b.Span)));
            row.LanesTop = row.Cells.Count > 0 ? lane : 0;
            row.Top = top;
            row.Height = row.LanesTop + layout.Count * lane;
            for (var i = 0; i < row.Bars.Count; i++)
            {
                var bar = row.Bars[i];
                bar.Lane = layout.Lanes[i];
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

        // The visible window is decided once per render: the markup draws these rows, and only the links whose two
        // ends sit in them are routed. A link to a row outside the window would not be visible anyway.
        _drawnRows = DrawnRows();
        var drawnKeys = gantt.Virtualize ? _drawnRows.Select(r => r.Key).ToHashSet() : null;

        var links = (Links ?? []).Where(l => LinkTypes is null || LinkTypes.Contains(l.Type)).ToList();
        if (ShowCriticalPath)
        {
            if (_criticalDirty)
            {
                _critical = [.. GanttGraph.CriticalPath(_bars.Values.Where(b => !b.IsSummary).ToDictionary(b => b.Key, b => b.Span), Links ?? [])];
                _criticalDirty = false;
            }
        }
        else
        {
            _critical.Clear();
        }

        _links = [.. links
            .Where(l => _bars.ContainsKey(l.FromKey) && _bars.ContainsKey(l.ToKey)
                && (drawnKeys is null
                    || (drawnKeys.Contains(_bars[l.FromKey].RowKey) && drawnKeys.Contains(_bars[l.ToKey].RowKey))))
            .Select(l => new GanttLinkLayout(l, GanttRouting.Path(_bars[l.FromKey].Box, _bars[l.ToKey].Box, l.Type, gantt.LinkGap),
                _critical.Contains(l.FromKey) && _critical.Contains(l.ToKey)))];
    }

    private string BarClass(GanttBarLayout<TItem> bar)
    {
        var classes = new List<string> { Css.Classes.Gantt.Bar, Css.Classes.Gantt.Kind(bar.Kind) };
        if (bar.IsSummary) classes.Add(Css.Classes.Gantt.BarSummary);
        if (bar.IsSuggested) classes.Add(Css.Classes.Gantt.BarSuggested);
        if (_linkFrom == bar.Key) classes.Add(Css.Classes.Gantt.BarLinkSource);
        if (!bar.IsSuggested && Gantt!.Selected == bar.Key) classes.Add(Css.Classes.Gantt.BarSelected);
        if (_critical.Contains(bar.Key)) classes.Add(Css.Classes.Gantt.BarCritical);
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
    public override ValueTask DisposeAsync()
    {
        Gantt?.Unregister(this);
        return base.DisposeAsync();
    }
}
