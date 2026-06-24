using System.Globalization;

namespace Flare.Components;

// Public export surface: builds a snapshot of the visible columns and the current filtered/sorted
// rows for an IDataGridExporter, with type/format-aware display text matching what the grid renders.
public partial class FlareDataGrid<TItem>
{
    // Type/format-aware display text for a data column's cell (used by export and clipboard copy).
    private string CellDisplayText(GridColumn<TItem> col, TItem item)
    {
        if (col.Value is null) return string.Empty;
        var type = ResolveColumnDataType(col.Key, col.Type);
        return DataGridValueFormatter.FormatText(col.Value(item), type, col.Format, col.NullText, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Builds the export snapshot for the current grid state: the visible data columns in display
    /// order and the full filtered/sorted row set (all pages). Each column carries both the raw value
    /// (for structured exporters such as JSON) and type/format-aware display text (for CSV, TSV,
    /// Markdown and Excel) matching what the grid renders. Pass the result to an
    /// <see cref="IDataGridExporter{TItem}"/>, or wire it via <c>DataGridExport.Grid</c>.
    /// </summary>
    /// <param name="fileName">Base file name (without extension) the exporter applies its own extension to.</param>
    public DataGridExportData<TItem> GetExportData(string fileName)
    {
        EnsureColumnsBuilt();

        var columns = new List<DataGridExportColumn<TItem>>();
        foreach (var c in _visibleColumns)
        {
            if (c.Value is null) continue; // skip composite/template-only (no extractable value)
            var col = c;
            columns.Add(new DataGridExportColumn<TItem>(c.Title, c.Value)
            {
                Text = row => CellDisplayText(col, row),
            });
        }

        return new DataGridExportData<TItem>
        {
            Columns = columns,
            Rows = SortedUnpaged(),
            FileName = fileName,
        };
    }
}
