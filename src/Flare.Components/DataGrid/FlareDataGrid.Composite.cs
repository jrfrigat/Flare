using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Banded multi-row composite columns (DevExpress AdvBandedView style): each record spans several
// table rows; composite fields align across the whole table. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // -- Banded multi-row composite (DevExpress-style) ------------------------

    private IEnumerable<GridColumn<TItem>> _visibleSlots =>
        _gridColumns.Where(c => !_hiddenColumns.Contains(c.Key));

    private bool _hasBandedComposite =>
        _gridColumns.Any(c => c.IsComposite && c.CompositeMode == CompositeMode.Banded && !_hiddenColumns.Contains(c.Key));

    // Number of table rows each record spans (max composite-host row count).
    private int _recordRows => Math.Max(1, _visibleSlots
        .Where(c => c.IsComposite && c.CompositeMode == CompositeMode.Banded)
        .Select(c => c.CompositeRows!.Count).DefaultIfEmpty(1).Max());

    // Virtualize item size for a banded record = its row count * the per-row height estimate.
    private float _recordItemSize => _recordRows * _effectiveVirtualItemSize;

    // Total leaf columns across the banded table (composite hosts contribute their own column count).
    private int _bandedColumnCount =>
        _visibleSlots.Sum(c => c.IsComposite && c.CompositeMode == CompositeMode.Banded ? CompositeHostColumns(c) : 1)
        + (SelectionMode == SelectionMode.Multiple ? 1 : 0) + (RowDetailTemplate != null ? 1 : 0);

    // A composite host's own sub-column count = widest row (by summed field colspans).
    private int CompositeHostColumns(GridColumn<TItem> host) => host.CompositeRows!
        .Select(r => r.Fields.Sum(f => Math.Max(1, ((FlareColumn<TItem>)f).ColSpan))).DefaultIfEmpty(1).Max();

    // Fields of a host's row with effective colspans (the last field fills the host's width).
    private List<(FlareColumn<TItem> Field, int ColSpan)> CompositeRowCells(GridColumn<TItem> host, int rowIndex)
    {
        var rows = host.CompositeRows!;
        if (rowIndex >= rows.Count) return [];
        var width = CompositeHostColumns(host);
        var fields = rows[rowIndex].Fields.Cast<FlareColumn<TItem>>().ToList();
        var spans = fields.Select(f => Math.Max(1, f.ColSpan)).ToList();
        var total = spans.Sum();
        if (spans.Count > 0 && total < width) spans[^1] += width - total;
        return fields.Zip(spans, (f, s) => (f, s)).ToList();
    }

    // A stable, namespaced sort key for a composite field (its Title is not a property name).
    private static string CompositeFieldKey(GridColumn<TItem> host, FlareColumn<TItem> field)
        => $"{host.Title}/{field.SortKey ?? field.Title}";

    // Selector map so sortable composite fields sort by their Field accessor.
    private Dictionary<string, Func<TItem, object?>> CompositeSortSelectors()
    {
        var map = new Dictionary<string, Func<TItem, object?>>();
        foreach (var host in _gridColumns.Where(c => c.IsComposite))
            foreach (var row in host.CompositeRows!)
                foreach (var field in row.Fields.Cast<FlareColumn<TItem>>())
                    if (field.Sortable && field.Field is not null)
                        map[CompositeFieldKey(host, field)] = field.Field;
        return map;
    }

    // Selector map so filterable composite fields filter by their Field accessor.
    private Dictionary<string, Func<TItem, object?>> CompositeFilterSelectors()
    {
        var map = new Dictionary<string, Func<TItem, object?>>();
        foreach (var host in _gridColumns.Where(c => c.IsComposite))
            foreach (var row in host.CompositeRows!)
                foreach (var field in row.Fields.Cast<FlareColumn<TItem>>())
                    if (field.Filterable && field.Field is not null)
                        map[CompositeFieldKey(host, field)] = field.Field;
        return map;
    }

    // Find a composite field by its namespaced filter/sort key ("Host/Field").
    private FlareColumn<TItem>? CompositeFieldByKey(string key)
    {
        if (!key.Contains('/')) return null;
        foreach (var host in _gridColumns.Where(c => c.IsComposite))
            foreach (var row in host.CompositeRows!)
                foreach (var field in row.Fields.Cast<FlareColumn<TItem>>())
                    if (CompositeFieldKey(host, field) == key)
                        return field;
        return null;
    }

    // True when the banded composite has at least one filterable field (drives the inline header filter).
    private bool _hasBandedCompositeFilter => _gridColumns
        .Where(c => c.IsComposite && c.CompositeMode == CompositeMode.Banded)
        .SelectMany(c => c.CompositeRows!)
        .SelectMany(r => r.Fields.Cast<FlareColumn<TItem>>())
        .Any(f => f.Filterable);

    private int CompositeFieldSortIndex(GridColumn<TItem> host, FlareColumn<TItem> field)
        => _sortStack.FindIndex(s => s.Column.Key == CompositeFieldKey(host, field));

    // Sort by a composite field via a synthetic column, reusing the normal sort toggle.
    private async Task OnCompositeFieldHeaderClick(GridColumn<TItem> host, FlareColumn<TItem> field, MouseEventArgs e)
    {
        if (!field.Sortable) return;
        var col = new GridColumn<TItem> { Title = field.Title, Value = field.Field, SortKey = CompositeFieldKey(host, field), Sortable = true };
        await OnHeaderClick(col, e);
    }

    // Symmetric multi-row header: composite hosts emit their field titles per row; plain/card
    // columns and prefix cells span the full record height.
    private RenderFragment RenderBandedCompositeHead() => builder =>
    {
        var slots = _visibleSlots.ToList();
        var rows = _recordRows;
        var seq = 0;
        for (var r = 0; r < rows; r++)
        {
            builder.OpenElement(seq++, "tr");
            builder.AddAttribute(seq++, "role", "row");
            builder.AddAttribute(seq++, "aria-rowindex", (r + 1).ToString());

            if (r == 0)
            {
                if (RowDetailTemplate != null) AddSpacerTh(builder, ref seq, rows);
                if (SelectionMode == SelectionMode.Multiple)
                {
                    builder.OpenElement(seq++, "th");
                    builder.AddAttribute(seq++, "class", $"{Css.Classes.DataGrid.Th} {Css.Classes.DataGrid.ThSelect}");
                    if (rows > 1) builder.AddAttribute(seq++, "rowspan", rows.ToString());
                    builder.OpenComponent<FlareCheckbox>(seq++);
                    builder.AddAttribute(seq++, "Value", _selectAllState);
                    builder.AddAttribute(seq++, "ValueChanged", EventCallback.Factory.Create<bool>(this, _ => ToggleSelectAll()));
                    builder.CloseComponent();
                    builder.CloseElement();
                }
            }

            foreach (var slot in slots)
            {
                if (slot.IsComposite && slot.CompositeMode == CompositeMode.Banded)
                {
                    var cells = CompositeRowCells(slot, r);
                    if (cells.Count == 0)
                    {
                        builder.OpenElement(seq++, "th");
                        builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.Th);
                        var width = CompositeHostColumns(slot);
                        if (width > 1) builder.AddAttribute(seq++, "colspan", width.ToString());
                        builder.CloseElement();
                    }
                    else
                    {
                        foreach (var (field, span) in cells)
                        {
                            var host = slot;
                            var f = field;
                            var sortIdx = f.Sortable ? CompositeFieldSortIndex(host, f) : -1;
                            builder.OpenElement(seq++, "th");
                            builder.AddAttribute(seq++, "role", "columnheader");
                            builder.AddAttribute(seq++, "class", $"{Css.Classes.DataGrid.Th} {Css.Classes.DataGrid.ThComposite}{(f.Sortable ? " " + Css.Classes.DataGrid.ThSortable : "")}");
                            if (span > 1) builder.AddAttribute(seq++, "colspan", span.ToString());
                            if (f.Sortable)
                            {
                                builder.AddAttribute(seq++, "aria-sort", sortIdx >= 0 ? (_sortStack[sortIdx].Direction == SortDirection.Ascending ? "ascending" : "descending") : "none");
                                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ev => OnCompositeFieldHeaderClick(host, f, ev)));
                            }
                            builder.AddContent(seq++, f.Title);
                            if (sortIdx >= 0)
                            {
                                builder.OpenElement(seq++, "span");
                                builder.AddAttribute(seq++, "class", $"material-symbols-rounded {Css.Classes.DataGrid.SortIcon}");
                                builder.AddContent(seq++, _sortStack[sortIdx].Direction == SortDirection.Ascending ? "arrow_upward" : "arrow_downward");
                                builder.CloseElement();
                            }
                            // Per-field inline filter: lives in the field sub-header so it stays aligned
                            // across the record's rows (a single filter row cannot span them).
                            if (f.Filterable && FilterMode != DataGridFilterMode.Menu)
                            {
                                builder.OpenElement(seq++, "div");
                                builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.CompositeFilter);
                                builder.AddEventStopPropagationAttribute(seq++, "onclick", true);
                                builder.AddContent(seq++, RenderFilterEditor(new(CompositeFieldKey(host, f), ResolveFilterType(CompositeFieldKey(host, f), f.FilterType, f.Type), f.FilterOptions)));
                                builder.CloseElement();
                            }
                            builder.CloseElement();
                        }
                    }
                }
                else if (r == 0)
                {
                    builder.AddContent(seq++, RenderColumnHeaderTh(slot, rows));
                }
            }
            builder.CloseElement();
        }
    };

    // One record rendered as N table rows; composite fields fill their cells, plain/card columns
    // span the full record height.
    private RenderFragment RenderBandedCompositeRecord(TItem item) => builder =>
    {
        var slots = _visibleSlots.ToList();
        var rows = _recordRows;
        var selected = _selection.Contains(item);
        var seq = 0;
        for (var r = 0; r < rows; r++)
        {
            builder.OpenElement(seq++, "tr");
            builder.SetKey((item!, r));
            var cls = $"{Css.Classes.DataGrid.Row} {Css.Classes.DataGrid.RecordRow}"
                + (selected ? " " + Css.Classes.DataGrid.RowSelected : "")
                + (r == 0 ? " " + Css.Classes.DataGrid.RecordFirst : "")
                + (RowClassFunc?.Invoke(item) is { Length: > 0 } rc ? " " + rc : "");
            builder.AddAttribute(seq++, "class", cls);
            if (RowStyleFunc?.Invoke(item) is { Length: > 0 } rs) builder.AddAttribute(seq++, "style", rs);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => HandleRowClickAsync(item)));

            if (r == 0)
            {
                if (RowDetailTemplate != null)
                {
                    builder.OpenElement(seq++, "td");
                    builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.Td);
                    if (rows > 1) builder.AddAttribute(seq++, "rowspan", rows.ToString());
                    builder.CloseElement();
                }
                if (SelectionMode == SelectionMode.Multiple)
                {
                    builder.OpenElement(seq++, "td");
                    builder.AddAttribute(seq++, "class", $"{Css.Classes.DataGrid.Td} {Css.Classes.DataGrid.TdSelect}");
                    if (rows > 1) builder.AddAttribute(seq++, "rowspan", rows.ToString());
                    builder.AddEventStopPropagationAttribute(seq++, "onclick", true);
                    builder.OpenComponent<FlareCheckbox>(seq++);
                    builder.AddAttribute(seq++, "Value", selected);
                    builder.AddAttribute(seq++, "ValueChanged", EventCallback.Factory.Create<bool>(this, _ => HandleRowClickAsync(item)));
                    builder.CloseComponent();
                    builder.CloseElement();
                }
            }

            foreach (var slot in slots)
            {
                if (slot.IsComposite && slot.CompositeMode == CompositeMode.Banded)
                {
                    var cells = CompositeRowCells(slot, r);
                    if (cells.Count == 0)
                    {
                        builder.OpenElement(seq++, "td");
                        builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.Td);
                        var width = CompositeHostColumns(slot);
                        if (width > 1) builder.AddAttribute(seq++, "colspan", width.ToString());
                        builder.CloseElement();
                    }
                    else
                    {
                        foreach (var (field, span) in cells)
                        {
                            builder.OpenElement(seq++, "td");
                            builder.AddAttribute(seq++, "class", $"{Css.Classes.DataGrid.Td} {Css.Classes.DataGrid.TdComposite}");
                            if (span > 1) builder.AddAttribute(seq++, "colspan", span.ToString());
                            if (field.Template is not null) builder.AddContent(seq++, field.Template(item));
                            else builder.AddContent(seq++, field.Field?.Invoke(item)?.ToString());
                            builder.CloseElement();
                        }
                    }
                }
                else if (r == 0)
                {
                    builder.OpenElement(seq++, "td");
                    var tdCls = Css.Classes.DataGrid.Td
                        + (_tdFrozenClass(slot) is { Length: > 0 } fc ? " " + fc : "")
                        + (slot.ClassFunc?.Invoke(item) is { Length: > 0 } cc ? " " + cc : "");
                    builder.AddAttribute(seq++, "class", tdCls);
                    if (rows > 1) builder.AddAttribute(seq++, "rowspan", rows.ToString());
                    if (slot.StyleFunc?.Invoke(item) is { Length: > 0 } cs) builder.AddAttribute(seq++, "style", cs);
                    builder.AddContent(seq++, RenderCell(slot, item));
                    builder.CloseElement();
                }
            }
            builder.CloseElement();
        }
    };
}
