using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace Flare.Components;

// Pagination and keyboard navigation (arrow keys, Home/End, Enter). Split out of FlareDataGrid.razor.
public partial class FlareDataGrid<TItem>
{
    // Anchor of the current cell-range selection (the active cell is _focusRow/_focusCol). -1 = none.
    private int _anchorRow = -1;
    private int _anchorCol = -1;
    // True while the mouse button is held to drag-select a cell range.
    private bool _dragSelecting;

    // Mouse down on a cell: start (or, with Shift, extend) a cell-range selection.
    private void OnCellMouseDown(int row, int col, MouseEventArgs e)
    {
        if (!CellSelection) return;
        _focusRow = row;
        _focusCol = col;
        if (!e.ShiftKey || _anchorRow < 0)
        {
            _anchorRow = row;
            _anchorCol = col;
        }
        _dragSelecting = true;
    }

    // Mouse entering a cell while dragging extends the range to it (anchor stays put).
    private void OnCellMouseEnter(int row, int col)
    {
        if (!_dragSelecting) return;
        _focusRow = row;
        _focusCol = col;
        StateHasChanged();
    }

    private void OnCellMouseUp() => _dragSelecting = false;

    // True when (rowIdx, colIdx) is inside the active cell-range rectangle (CellSelection only).
    private bool InCellRange(int rowIdx, int colIdx)
    {
        if (!CellSelection || _focusRow < 0 || _anchorRow < 0) return false;
        var (r0, r1) = (Math.Min(_anchorRow, _focusRow), Math.Max(_anchorRow, _focusRow));
        var (c0, c1) = (Math.Min(_anchorCol, _focusCol), Math.Max(_anchorCol, _focusCol));
        return rowIdx >= r0 && rowIdx <= r1 && colIdx >= c0 && colIdx <= c1;
    }
    private async Task SetPage(int page)
    {
        _page = Math.Clamp(page, 0, Math.Max(0, _totalPages - 1));
        _focusRow = -1;
        await RaisePageChangedAsync();
        if (_provider is not null)
            await LoadFromProviderAsync();
    }

    private async Task HandleGridKeyDown(KeyboardEventArgs e)
    {
        var rows = _pageItems.ToList();
        if (rows.Count == 0) return;

        if (_focusRow < 0) { _focusRow = 0; _focusCol = 0; _anchorRow = 0; _anchorCol = 0; return; }

        // Shift held with cell selection extends the range (anchor stays put); otherwise the anchor
        // collapses onto the moved active cell.
        var extend = CellSelection && e.ShiftKey;
        var isMove = e.Key is "ArrowDown" or "ArrowUp" or "ArrowRight" or "ArrowLeft" or "Home" or "End";

        switch (e.Key)
        {
            case "ArrowDown": _focusRow = Math.Min(_focusRow + 1, rows.Count - 1); break;
            case "ArrowUp": _focusRow = Math.Max(_focusRow - 1, 0); break;
            case "ArrowRight": _focusCol = Math.Min(_focusCol + 1, _totalColumnCount - 1); break;
            case "ArrowLeft": _focusCol = Math.Max(_focusCol - 1, 0); break;
            case "Home": _focusCol = 0; break;
            case "End": _focusCol = _totalColumnCount - 1; break;
            case "c" or "C" when e.CtrlKey && CellSelection:
                await CopyCellRangeAsync();
                return;
            case "v" or "V" when e.CtrlKey && CellSelection:
                await PasteCellRangeAsync();
                return;
            case "PageDown": await SetPage(_page + 1); break;
            case "PageUp": await SetPage(_page - 1); break;
            case "Enter" or " ":
                // Select/deselect focused row
                if (SelectionMode != SelectionMode.None && _focusRow >= 0 && _focusRow < rows.Count)
                {
                    await HandleRowClickAsync(rows[_focusRow]);
                    StateHasChanged();
                }
                break;
            case "Delete":
                // Start editing focused row if editable
                if (_focusRow >= 0 && _focusRow < rows.Count && _inlineEditEnabled)
                {
                    BeginEdit(rows[_focusRow]);
                    StateHasChanged();
                }
                break;
            case "Escape":
                // Cancel editing or deselect
                if (_editingItem is not null)
                {
                    await CancelRowAsync(_editingItem);
                    StateHasChanged();
                }
                else if (SelectionMode != SelectionMode.None)
                {
                    _selection.Clear();
                    await SelectedItemsChanged.InvokeAsync(_selection);
                    StateHasChanged();
                }
                break;
            case "a" when e.CtrlKey:
                // Ctrl+A: select all visible rows
                if (SelectionMode == SelectionMode.Multiple)
                {
                    foreach (var item in rows)
                        _selection.Add(item);
                    await SelectedItemsChanged.InvokeAsync(_selection);
                    StateHasChanged();
                }
                break;
        }

        // After moving the active cell, collapse the range onto it unless Shift is extending it.
        if (isMove && (!extend || _anchorRow < 0))
        {
            _anchorRow = _focusRow;
            _anchorCol = _focusCol;
        }
    }

    // Copies the current cell-range selection to the clipboard as tab-separated values (rows separated
    // by newlines), using each column's type/format-aware display text - paste-ready for spreadsheets.
    private async Task CopyCellRangeAsync()
    {
        var rows = _pageItems.ToList();
        if (_focusRow < 0 || rows.Count == 0) return;

        var visible = _gridColumns.Where(c => !_hiddenColumns.Contains(c.Key)).ToList();
        if (visible.Count == 0) return;

        var (r0, r1) = (Math.Min(_anchorRow, _focusRow), Math.Max(_anchorRow, _focusRow));
        var (c0, c1) = (Math.Min(_anchorCol, _focusCol), Math.Max(_anchorCol, _focusCol));
        r1 = Math.Min(r1, rows.Count - 1);
        c1 = Math.Min(c1, visible.Count - 1);

        var sb = new StringBuilder();
        for (var r = r0; r <= r1; r++)
        {
            for (var c = c0; c <= c1; c++)
            {
                if (c > c0) sb.Append('\t');
                sb.Append(CellDisplayText(visible[c], rows[r]));
            }
            if (r < r1) sb.Append('\n');
        }

        await Clipboard.CopyAsync(sb.ToString());
    }

    // Pastes the clipboard's tab-separated text into the cells, starting at the top-left of the current
    // selection, and raises OnPaste so the host applies the values (the grid can't write through the
    // read-only column accessors). No-op without an OnPaste handler.
    private async Task PasteCellRangeAsync()
    {
        if (!OnPaste.HasDelegate) return;
        var text = await Clipboard.ReadAsync();
        if (string.IsNullOrEmpty(text)) return;

        var rows = _pageItems.ToList();
        if (_focusRow < 0 || rows.Count == 0) return;
        var visible = _gridColumns.Where(c => !_hiddenColumns.Contains(c.Key)).ToList();
        if (visible.Count == 0) return;

        var startRow = Math.Min(_anchorRow, _focusRow);
        var startCol = Math.Min(_anchorCol, _focusCol);

        var grid = ParseTsv(text);
        var cells = new List<DataGridPasteCell<TItem>>();
        for (var r = 0; r < grid.Count; r++)
        {
            var tr = startRow + r;
            if (tr < 0 || tr >= rows.Count) continue;
            var line = grid[r];
            for (var c = 0; c < line.Length; c++)
            {
                var tc = startCol + c;
                if (tc < 0 || tc >= visible.Count) continue;
                cells.Add(new DataGridPasteCell<TItem>(rows[tr], visible[tc].Key, line[c]));
            }
        }
        if (cells.Count == 0) return;

        await OnPaste.InvokeAsync(new DataGridPaste<TItem>(cells));
        _sortedCache = null;
        StateHasChanged();
    }

    // Splits clipboard text into rows (newline) and columns (tab); tolerates CRLF and a trailing newline.
    private static List<string[]> ParseTsv(string text)
    {
        var result = new List<string[]>();
        foreach (var line in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            result.Add(line.Split('\t'));
        // Drop a single trailing empty line (from a terminating newline) so it doesn't clear a cell.
        if (result.Count > 1 && result[^1].Length == 1 && result[^1][0].Length == 0)
            result.RemoveAt(result.Count - 1);
        return result;
    }
}
