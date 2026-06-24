using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

// Column band headers: lays out nested <FlareColumnBand> groups into a multi-row header with
// correct colspan/rowspan. Split out of FlareDataGrid.razor.
/// <summary>FlareDataGrid partial: column-band (grouped header) layout logic.</summary>
public partial class FlareDataGrid<TItem>
{
    // -- Band header rendering -----------------------------------------------

    private int _totalColumnCount => _gridColumns.Count(c => !_hiddenColumns.Contains(c.Key))
        + (SelectionMode == SelectionMode.Multiple ? 1 : 0)
        + (RowDetailTemplate != null ? 1 : 0)
        + (_inlineEditEnabled ? 1 : 0);

    // -- Banded header --------------------------------------------------------

    // One header cell in the banded layout: a band title (Band set) or a leaf column header (Column set).
    private sealed record HeaderCell(int Row, int RowSpan, int ColSpan, FlareColumnBand? Band, GridColumn<TItem>? Column);

    // Lay out a header node, appending its cell(s); returns the visible leaf count under it.
    private int LayoutHeaderNode(FlareColumnBase node, int depth, List<HeaderCell> cells)
    {
        if (node is FlareColumnBand band)
        {
            var index = cells.Count;
            cells.Add(new HeaderCell(depth, 1, 0, band, null));
            var leaves = 0;
            foreach (var child in band.Children)
                leaves += LayoutHeaderNode(child, depth + 1, cells);
            cells[index] = cells[index] with { ColSpan = leaves };
            return leaves;
        }

        // Leaf column: render its full header th, spanning down to the bottom leaf row.
        if (_hiddenColumns.Contains(node.ColumnKey)) return 0;
        var col = _gridColumns.FirstOrDefault(c => c.Key == node.ColumnKey);
        if (col is null) return 0;
        cells.Add(new HeaderCell(depth, _totalHeaderRows - depth, 1, null, col));
        return 1;
    }

    private RenderFragment RenderBandedHead() => builder =>
    {
        var cells = new List<HeaderCell>();
        foreach (var node in _orderedHeaderNodes)
            LayoutHeaderNode(node, 0, cells);

        var totalRows = _totalHeaderRows;
        var seq = 0;
        for (var row = 0; row < totalRows; row++)
        {
            builder.OpenElement(seq++, "tr");
            builder.AddAttribute(seq++, "role", "row");
            builder.AddAttribute(seq++, "aria-rowindex", (row + 1).ToString());

            // Prefix spacer cells span the full header height; render on the first row only.
            if (row == 0)
            {
                if (RowDetailTemplate != null) AddSpacerTh(builder, ref seq, totalRows);
                if (SelectionMode == SelectionMode.Multiple) AddSpacerTh(builder, ref seq, totalRows);
            }

            foreach (var cell in cells.Where(c => c.Row == row))
            {
                if (cell.Band is not null)
                {
                    builder.OpenElement(seq++, "th");
                    builder.AddAttribute(seq++, "role", "columnheader");
                    builder.AddAttribute(seq++, "class", $"{Css.Classes.DataGrid.Th} {Css.Classes.DataGrid.ThBand} {cell.Band.CssClass ?? ""}".TrimEnd());
                    if (cell.ColSpan > 1) builder.AddAttribute(seq++, "colspan", cell.ColSpan.ToString());
                    builder.AddContent(seq++, cell.Band.Title);
                    builder.CloseElement();
                }
                else
                {
                    builder.AddContent(seq++, RenderColumnHeaderTh(cell.Column!, cell.RowSpan));
                }
            }

            if (row == 0 && _inlineEditEnabled) AddSpacerTh(builder, ref seq, totalRows);

            builder.CloseElement();
        }
    };

    private void AddSpacerTh(RenderTreeBuilder builder, ref int seq, int rowSpan)
    {
        builder.OpenElement(seq++, "th");
        builder.AddAttribute(seq++, "class", Css.Classes.DataGrid.Th);
        if (rowSpan > 1) builder.AddAttribute(seq++, "rowspan", rowSpan.ToString());
        builder.CloseElement();
    }
}
