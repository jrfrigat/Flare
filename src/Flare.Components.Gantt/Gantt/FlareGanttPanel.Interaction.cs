using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

public partial class FlareGanttPanel<TRow, TItem>
{
    /// <summary>Lets the user move, resize and re-row items and create links. Off by default and in a compact chart.</summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>Whether moving an item pushes its successors.</summary>
    [Parameter] public GanttScheduleMode ScheduleMode { get; set; } = GanttScheduleMode.Push;

    /// <summary>Whether an item may be moved to another row.</summary>
    [Parameter] public bool AllowRowChange { get; set; } = true;

    /// <summary>
    /// The span an item must keep covering, such as the work of a parent's children: resizing stops at its edges.
    /// Null, or a null result, leaves the item free.
    /// </summary>
    [Parameter] public Func<TItem, GanttSpan?>? ItemCover { get; set; }

    /// <summary>Type of links the user creates.</summary>
    [Parameter] public GanttLinkType NewLinkType { get; set; } = GanttLinkType.FinishToStart;

    /// <summary>Raised with the proposed positions when the user moves or resizes an item.</summary>
    [Parameter] public EventCallback<GanttChangeEventArgs> OnChange { get; set; }

    /// <summary>Raised when the user creates a link that passed the cycle check.</summary>
    [Parameter] public EventCallback<GanttLinkEventArgs> OnLinkCreate { get; set; }

    /// <summary>Raised when a link the user asked for was refused.</summary>
    [Parameter] public EventCallback<GanttLinkRejectedEventArgs> OnLinkRejected { get; set; }

    /// <summary>
    /// Drag group of an outside <c>FlareDragContext</c>. When set, every row and axis unit becomes a drop zone whose
    /// target is <see cref="GanttDropTarget.Format"/>.
    /// </summary>
    [Parameter] public string? DropGroup { get; set; }

    /// <summary>Which payloads the row and unit zones of <see cref="DropGroup"/> take; null takes every payload of the group.</summary>
    [Parameter] public Func<object?, bool>? DropAccepts { get; set; }

    /// <summary>
    /// Drag group of an outside <c>FlareDragContext</c> whose payloads can be dropped onto an item itself. When set, every
    /// bar that is neither a summary nor a suggestion becomes a drop zone whose target is <see cref="GanttItemDropTarget.Format"/>.
    /// While a drag is on, whatever in a track does not take the payload lets the pointer through to the zones beneath it,
    /// so a payload only the units take still lands on a unit covered by a bar.
    /// </summary>
    [Parameter] public string? ItemDropGroup { get; set; }

    /// <summary>Which payloads the bar zones of <see cref="ItemDropGroup"/> take; null takes every payload of the group.</summary>
    [Parameter] public Func<object?, bool>? ItemDropAccepts { get; set; }

    private bool _suppressClick;
    private string? _refocus;
    private bool _skipRender;
    private string? _linkFrom;
    private string? _announcement;

    private bool CanEdit => Editable && Gantt is { Compact: false };

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        if (!_skipRender) return true;
        _skipRender = false;
        return false;
    }

    // A gesture the browser tracked (gantt.js): the bar moved or was resized by whole axis units, possibly into
    // another row. One call per gesture; the same rules as the keyboard apply.
    async Task IGanttPanel.DraggedAsync(string key, GanttEditAction action, int units, string? rowKey)
    {
        if (!CanEdit || !_bars.TryGetValue(key, out var bar) || bar.IsSummary) return;
        var toRow = action == GanttEditAction.Move && AllowRowChange && rowKey is not null && _rows.Any(r => r.Key == rowKey) ? rowKey : bar.RowKey;
        _suppressClick = true;
        await Commit(key, action, Apply(bar.Span, action, units, CoverOf(key)), bar.RowKey, toRow);
        StateHasChanged();
    }

    // A link the user drew from one bar to another in the browser.
    async Task IGanttPanel.LinkedAsync(string fromKey, string toKey)
    {
        if (!CanEdit || !_bars.TryGetValue(fromKey, out var from) || from.IsSummary) return;
        if (!_bars.TryGetValue(toKey, out var to) || to.IsSummary) return;
        _suppressClick = true;
        await CreateLink(new GanttLink(fromKey, toKey, NewLinkType));
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // A bar moved to another row from the keyboard is a new element there: give it the focus back.
        if (_refocus is { } key)
        {
            _refocus = null;
            await Gantt!.FocusItemAsync(key);
        }
    }

    private async Task KeyDown(KeyboardEventArgs e, GanttBarLayout<TItem> bar)
    {
        if (bar.IsSuggested) return;
        switch (e.Key)
        {
            case "Enter" or " ":
                await Activate(bar);
                return;
            case "Escape":
                if (_linkFrom is null) { _skipRender = true; return; }
                _linkFrom = null;
                return;
            case "l" or "L" or "\u0434" or "\u0414" when CanEdit && !e.CtrlKey && !bar.IsSummary:
                _linkFrom = bar.Key;
                _announcement = string.Format(Gantt!.Axis.Culture, Gantt.EffectiveTexts.LinkStarted!, bar.Label ?? bar.Key);
                return;
        }

        if (!CanEdit || bar.IsSummary || !e.CtrlKey) { _skipRender = true; return; }
        switch (e.Key)
        {
            case "ArrowLeft" or "ArrowRight":
                var step = e.Key == "ArrowRight" ? 1 : -1;
                var action = e.ShiftKey ? GanttEditAction.ResizeEnd : GanttEditAction.Move;
                await Commit(bar.Key, action, Apply(bar.Span, action, step, CoverOf(bar.Key)), bar.RowKey, bar.RowKey);
                return;
            case "ArrowUp" or "ArrowDown" when AllowRowChange:
                var index = _rows.FindIndex(r => r.Key == bar.RowKey) + (e.Key == "ArrowDown" ? 1 : -1);
                if (index < 0 || index >= _rows.Count) { _skipRender = true; return; }
                _refocus = bar.Key;
                await Commit(bar.Key, GanttEditAction.Move, bar.Span, bar.RowKey, _rows[index].Key);
                return;
            default:
                _skipRender = true;
                return;
        }
    }

    private async Task Activate(GanttBarLayout<TItem> bar)
    {
        if (_suppressClick) { _suppressClick = false; return; }
        if (_linkFrom is { } from && CanEdit && from != bar.Key && !bar.IsSummary)
        {
            _linkFrom = null;
            await CreateLink(new GanttLink(from, bar.Key, NewLinkType));
            return;
        }
        await Click(bar);
    }

    private async Task CreateLink(GanttLink link)
    {
        var validation = GanttGraph.Validate(Links ?? [], link);
        if (!validation.IsValid)
        {
            var message = Gantt!.EffectiveTexts.Describe(validation, key => _bars.TryGetValue(key, out var bar) ? bar.Label ?? key : key, Gantt.Axis.Culture) ?? "";
            _announcement = message;
            await OnLinkRejected.InvokeAsync(new GanttLinkRejectedEventArgs(link, validation, message));
            return;
        }
        await OnLinkCreate.InvokeAsync(new GanttLinkEventArgs(link));
    }

    // The new span after moving or resizing by whole axis units; a resize never inverts the item and never leaves
    // the span it must cover.
    private GanttSpan Apply(GanttSpan span, GanttEditAction action, int units, GanttSpan? cover = null)
    {
        var axis = Gantt!.Axis;
        DateTime Step(DateTime moment) => units == 0 ? moment : axis.At(axis.Offset(moment) + units);
        return action switch
        {
            GanttEditAction.ResizeStart => new GanttSpan(Min(Min(Step(span.Start), span.End), cover?.Start ?? DateTime.MaxValue), span.End),
            GanttEditAction.ResizeEnd => new GanttSpan(span.Start, Max(Max(Step(span.End), span.Start), cover?.End ?? DateTime.MinValue)),
            _ => new GanttSpan(Step(span.Start), Step(span.End))
        };
    }

    private GanttSpan? CoverOf(string key)
    {
        if (ItemCover is null || ItemKey is null) return null;
        foreach (var item in Items ?? [])
            if (ItemKey(item) == key) return ItemCover(item);
        return null;
    }

    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;


    private async Task Commit(string key, GanttEditAction action, GanttSpan span, string fromRow, string? toRow)
    {
        var rowChanged = toRow is not null && toRow != fromRow;
        var spans = new Dictionary<string, GanttSpan>();
        foreach (var item in Items ?? [])
            if (ItemKey is not null && ItemSpan?.Invoke(item) is { } s) spans.TryAdd(ItemKey(item), s);
        var original = spans.TryGetValue(key, out var before) ? before : span;
        if (span == original && !rowChanged) return;

        var cascade = span == original
            ? new Dictionary<string, GanttSpan>()
            : GanttGraph.Cascade(spans, Links ?? [], key, span, ScheduleMode);
        var changes = new List<GanttChange> { new(key, span, rowChanged ? toRow : null) };
        changes.AddRange(cascade.Where(c => c.Key != key).Select(c => new GanttChange(c.Key, c.Value)));

        var args = new GanttChangeEventArgs(key, action, changes);
        await OnChange.InvokeAsync(args);
        if (!args.Cancel)
        {
            var label = _bars.TryGetValue(key, out var bar) ? bar.Label ?? key : key;
            _announcement = string.Format(Gantt!.Axis.Culture, Gantt.EffectiveTexts.Moved!, label, Describe(span));
        }
    }

    private string Describe(GanttSpan span)
    {
        var culture = Gantt!.Axis.Culture;
        var end = span.End.TimeOfDay == TimeSpan.Zero && span.End > span.Start ? span.End.AddDays(-1) : span.End;
        return Gantt.Scale is GanttScale.Hour
            ? $"{span.Start.ToString("g", culture)} \u2013 {span.End.ToString("t", culture)}"
            : $"{span.Start.ToString("d", culture)} \u2013 {end.ToString("d", culture)}";
    }
}
