using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace Flare.Components;

// Cell rendering and inline/batch editing: composite cell builder, edit inputs, row edit actions,
// and the batch edit buffer. Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    private RenderFragment RenderCell(GridColumn<TItem> col, TItem item) => builder =>
    {
        if (col.IsComposite)
            BuildCompositeCell(builder, col, item);
        else if (col.CellTemplate is not null)
            builder.AddContent(0, col.CellTemplate(item));
        else
            BuildTypedValue(builder, col.Value?.Invoke(item), col.Type, col.Format, col.NullText);
    };

    // Renders a raw cell value type-aware: a boolean as a read-only checkbox icon, everything else as
    // culture/format-aware text via DataGridValueFormatter. ColumnDataType.Auto infers from the value.
    private void BuildTypedValue(RenderTreeBuilder builder, object? value, ColumnDataType type, string? format, string? nullText)
    {
        var resolved = DataGridValueFormatter.Resolve(type, value);
        if (resolved == ColumnDataType.Boolean && value is bool b)
        {
            var stateClass = b ? Css.Classes.DataGrid.BoolOn : Css.Classes.DataGrid.BoolOff;
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", $"material-symbols-rounded {Css.Classes.DataGrid.BoolCell} {stateClass}");
            builder.AddAttribute(2, "role", "img");
            builder.AddAttribute(3, "aria-label", b ? "true" : "false");
            builder.AddContent(4, b ? "check_box" : "check_box_outline_blank");
            builder.CloseElement();
            return;
        }
        builder.AddContent(0, DataGridValueFormatter.FormatText(
            value, resolved, format, nullText, System.Globalization.CultureInfo.CurrentCulture));
    }

    // Renders a composite host column's cell as a CSS grid of labelled, stacked fields.
    private void BuildCompositeCell(RenderTreeBuilder builder, GridColumn<TItem> col, TItem item)
    {
        var rows = col.CompositeRows!;
        var fieldsByRow = rows.Select(r => r.Fields.Cast<FlareColumn<TItem>>().ToList()).ToList();
        var columns = fieldsByRow.Count > 0 ? fieldsByRow.Max(r => r.Sum(f => Math.Max(1, f.ColSpan))) : 1;

        var seq = 0;
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.Composite);
        builder.AddAttribute(seq++, "style", $"grid-template-columns:repeat({columns},minmax(0,1fr));");
        foreach (var fields in fieldsByRow)
        {
            foreach (var field in fields)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.CompositeField);
                builder.AddAttribute(seq++, "style", $"grid-column:span {Math.Max(1, field.ColSpan)};");
                if (!string.IsNullOrEmpty(field.Title))
                {
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.CompositeLabel);
                    builder.AddContent(seq++, field.Title);
                    builder.CloseElement();
                }
                builder.OpenElement(seq++, "span");
                builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.CompositeValue);
                if (field.Template is not null)
                    builder.AddContent(seq++, field.Template(item));
                else
                    BuildTypedValue(builder, field.Field?.Invoke(item), field.Type, field.Format, field.NullText);
                builder.CloseElement();
                builder.CloseElement();
            }
        }
        builder.CloseElement();
    }

    // Edit-aware cell: while the row is inline-edited and the column is Editable, render its
    // EditTemplate (if any) or a compact FlareField bound to the edit buffer; otherwise display.
    private RenderFragment RenderCellOrInput(GridColumn<TItem> col, TItem item, bool isEditing) => builder =>
    {
        if (isEditing && col.Editable)
        {
            if (col.EditTemplate is not null)
            {
                builder.AddContent(0, col.EditTemplate(item));
                return;
            }
            if (col.Value is not null)
            {
                var key = col.Title;
                if (!_editValues.ContainsKey(key))
                    _editValues[key] = EditSeed(col, item);
                BuildTypedEditInput(builder, col, key);
                return;
            }
        }
        builder.AddContent(0, RenderCell(col, item));
    };

    // Seeds the edit buffer with a round-trip-safe string for the column type: invariant numbers and
    // ISO dates/times so the matching typed editor (and the consumer of GetEditValues) can parse them.
    private string EditSeed(GridColumn<TItem> col, TItem item)
    {
        var raw = col.Value?.Invoke(item);
        if (raw is null) return string.Empty;
        return ResolveColumnDataType(col.Key, col.Type) switch
        {
            ColumnDataType.Boolean => raw is bool b ? b.ToString() : raw.ToString() ?? string.Empty,
            ColumnDataType.Number => raw is IFormattable nf ? nf.ToString(null, CultureInfo.InvariantCulture) : raw.ToString() ?? string.Empty,
            ColumnDataType.Date => AsDateTime(raw)?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? raw.ToString() ?? string.Empty,
            ColumnDataType.DateTime => AsDateTime(raw)?.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture) ?? raw.ToString() ?? string.Empty,
            ColumnDataType.Time => AsTime(raw)?.ToString(@"hh\:mm") ?? raw.ToString() ?? string.Empty,
            _ => raw.ToString() ?? string.Empty,
        };
    }

    private static DateTime? AsDateTime(object raw) => raw switch
    {
        DateTime dt => dt,
        DateTimeOffset dto => dto.DateTime,
        DateOnly d => d.ToDateTime(TimeOnly.MinValue),
        _ => null,
    };

    private static TimeSpan? AsTime(object raw) => raw switch
    {
        TimeOnly t => t.ToTimeSpan(),
        TimeSpan ts => ts,
        DateTime dt => dt.TimeOfDay,
        _ => null,
    };

    // Picks the inline editor from the column's data type: bool -> checkbox, enum -> select,
    // number/date/time -> typed input, otherwise a text box. All write back into the string buffer.
    private void BuildTypedEditInput(RenderTreeBuilder builder, GridColumn<TItem> col, string key)
    {
        var current = _editValues.TryGetValue(key, out var v) ? v : string.Empty;
        switch (ResolveColumnDataType(col.Key, col.Type))
        {
            case ColumnDataType.Boolean:
                builder.OpenComponent<FlareCheckbox>(0);
                builder.AddAttribute(1, "Value", (bool?)string.Equals(current, "true", StringComparison.OrdinalIgnoreCase));
                builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<bool?>(this, b => _editValues[key] = (b == true).ToString()));
                builder.CloseComponent();
                break;
            case ColumnDataType.Enum:
                builder.OpenComponent<FlareSelect<string>>(0);
                builder.AddAttribute(1, "Items", DistinctValues(col.Key).ToList());
                builder.AddAttribute(2, "Value", current);
                builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string?>(this, val => _editValues[key] = val ?? string.Empty));
                builder.AddAttribute(4, "Size", FieldSize.Xs);
                builder.CloseComponent();
                break;
            case ColumnDataType.Number: BuildEditInput(builder, key, "number"); break;
            case ColumnDataType.Date: BuildEditInput(builder, key, "date"); break;
            case ColumnDataType.DateTime: BuildEditInput(builder, key, "datetime-local"); break;
            case ColumnDataType.Time: BuildEditInput(builder, key, "time"); break;
            default: BuildEditInput(builder, key, "text"); break;
        }
    }

    // Compact FlareField bound to the per-row edit buffer for the given column key.
    private void BuildEditInput(RenderTreeBuilder builder, string key, string inputType = "text")
    {
        builder.OpenComponent<FlareField<string>>(0);
        builder.AddAttribute(1, "Value", _editValues.TryGetValue(key, out var v) ? v : string.Empty);
        builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(
            this, val => _editValues[key] = val ?? string.Empty));
        builder.AddAttribute(3, "Immediate", true);
        builder.AddAttribute(4, "Size", FieldSize.Xs);
        builder.AddAttribute(5, "Type", inputType);
        builder.CloseComponent();
    }

    // Icon-only FlareButton used for the row edit/save/cancel actions.
    private void BuildIconButton(RenderTreeBuilder builder, int seq, string icon, string ariaLabel, Func<Task> onClick)
    {
        builder.OpenComponent<FlareButton>(seq);
        builder.AddAttribute(seq + 1, "Variant", ButtonVariant.Text);
        builder.AddAttribute(seq + 2, "Size", ButtonSize.Xs);
        builder.AddAttribute(seq + 3, "AriaLabel", ariaLabel);
        builder.AddAttribute(seq + 4, "OnClick", EventCallback.Factory.Create<MouseEventArgs>(this, async _ => await onClick()));
        builder.AddAttribute(seq + 5, "ChildContent", (RenderFragment)(b =>
        {
            b.OpenElement(0, "span");
            b.AddAttribute(1, "class", "material-symbols-rounded");
            b.AddAttribute(2, "style", "font-size:1.125rem;");
            b.AddContent(3, icon);
            b.CloseElement();
        }));
        builder.CloseComponent();
    }

    private RenderFragment RenderEditActions(TItem item) => builder =>
    {
        // EqualityComparer.Default (not ReferenceEquals) so a struct TItem is not boxed into two
        // distinct references that never match -- and to stay consistent with selection equality.
        var isEditing = _editingItem is not null && EqualityComparer<TItem>.Default.Equals(_editingItem, item);
        if (isEditing)
        {
            BuildIconButton(builder, 0, "check", FlareStrings.DataGrid_Save, () => SaveRowAsync(item));
            BuildIconButton(builder, 10, "close", FlareStrings.DataGrid_Cancel, () => CancelRowAsync(item));
        }
        else
        {
            BuildIconButton(builder, 0, "edit", FlareStrings.DataGrid_Edit, () => { BeginEdit(item); return Task.CompletedTask; });
        }
    };

    private void BeginEdit(TItem item)
    {
        _editingItem = item;
        _editValues.Clear();
        foreach (var col in _gridColumns.Where(c => c.Editable && c.Value is not null))
            _editValues[col.Title] = EditSeed(col, item);
    }

    private async Task SaveRowAsync(TItem item)
    {
        _editingItem = default;
        await OnRowSaved.InvokeAsync(item);
        _editValues.Clear();
    }

    private async Task CancelRowAsync(TItem item)
    {
        _editingItem = default;
        _editValues.Clear();
        await OnRowCancelled.InvokeAsync(item);
    }

    /// <summary>Returns the current edit values keyed by column title. Available during OnRowSaved.</summary>
    public IReadOnlyDictionary<string, string> GetEditValues() => _editValues;

    // -- Batch editing -------------------------------------------------------

    private bool _batchMode => EditMode == DataGridEditMode.Batch;

    private void BeginBatchEdit(TItem item)
    {
        _batchEditingItems.Add(item);
        if (!_batchEditValues.ContainsKey(item))
        {
            _batchEditValues[item] = new Dictionary<string, string>();
            foreach (var col in _gridColumns.Where(c => c.Editable && c.Value is not null))
                _batchEditValues[item][col.Title] = EditSeed(col, item);
        }
    }

    private void ToggleBatchEdit(TItem item)
    {
        if (_batchEditingItems.Contains(item))
        {
            _batchEditingItems.Remove(item);
            _batchEditValues.Remove(item);
        }
        else
        {
            BeginBatchEdit(item);
        }
    }

    private void UpdateBatchEditValue(TItem item, string columnTitle, string value)
    {
        if (_batchEditValues.TryGetValue(item, out var values))
            values[columnTitle] = value;
    }

    private IReadOnlyDictionary<string, string>? GetBatchEditValues(TItem item)
    {
        return _batchEditValues.TryGetValue(item, out var values) ? values : null;
    }

    private bool IsBatchEditing(TItem item) => _batchEditingItems.Contains(item);

    private async Task SaveBatchAsync()
    {
#pragma warning disable CS8714
        var snapshot = new Dictionary<TItem, IReadOnlyDictionary<string, string>>();
#pragma warning restore CS8714
        foreach (var item in _batchEditingItems)
        {
            if (_batchEditValues.TryGetValue(item, out var values))
                snapshot[item] = values;
        }

        _batchEditingItems.Clear();
        _batchEditValues.Clear();

        await OnBatchSave.InvokeAsync(snapshot);
    }

    private async Task CancelBatchAsync()
    {
        _batchEditingItems.Clear();
        _batchEditValues.Clear();
        await OnBatchCancel.InvokeAsync();
    }
}
