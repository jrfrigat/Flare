using System.Globalization;

namespace Flare.Components;

// Reordering of columns (by header) and rows, both through the shared pointer-based drag model.
//
// Both work on a touch screen: the grid names its two zones, tags its rows and headers with an id,
// and answers two questions. What is left here is the arithmetic of the move.
public partial class FlareDataGrid<TItem>
{
    // The zone ids double as the drag groups, which is what keeps a row out of the header and a header
    // out of the body while one context serves both.
    private const string RowZone = "flare-datagrid-rows";
    private const string ColumnZone = "flare-datagrid-columns";

    // A row is identified by its position on the page rather than by a key: the grid does not require
    // one (RowKey is optional and falls back to the item itself), and both questions the browser asks
    // are answered against the same render the ids were written in.
    private static string RowDragId(int index) => index.ToString(CultureInfo.InvariantCulture);

    private static string ColumnDragId(string key) => "c:" + key;

    // The context holds two kinds of thing, so the resolver is what tells them apart. An id that does
    // not resolve refuses the drag rather than falling through to something plausible.
    private bool TryResolveDragItem(string id, out object payload)
    {
        if (id.StartsWith("c:", StringComparison.Ordinal))
        {
            var key = id[2..];
            payload = key;
            return _gridColumns.Any(c => c.Key == key);
        }

        var rows = _pageItems;
        if (int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)
            && index >= 0 && index < rows.Count && rows[index] is { } item)
        {
            payload = item;
            return true;
        }

        payload = null!;
        return false;
    }

    private Task OnDragDropped(FlareDropEventArgs<object> e) => e.TargetId switch
    {
        ColumnZone => MoveColumnAsync(e),
        RowZone => MoveRowAsync(e),
        _ => Task.CompletedTask,
    };

    // Columns are identified by their stable Key (Id/SortKey/Title), so reorder survives duplicated or
    // localized titles.
    private async Task MoveColumnAsync(FlareDropEventArgs<object> e)
    {
        if (e.Payload is not string dragged) return;

        var order = _gridColumns.Select(c => c.Key).ToList();
        order.Remove(dragged);
        var index = e.Index < 0 || e.Index > order.Count ? order.Count : e.Index;
        order.Insert(index, dragged);

        await SetColumnOrderAsync(order);
    }

    private async Task MoveRowAsync(FlareDropEventArgs<object> e)
    {
        if (e.Payload is not TItem dragged) return;

        var rows = _pageItems;
        var oldIndex = IndexOf(rows, dragged);
        if (oldIndex < 0) return;

        if (_treeMode)
        {
            await MoveTreeRowAsync(e, rows, dragged, oldIndex);
            return;
        }

        // The drop reports the position WITHOUT the dragged row, which is the index it ends up at.
        // OnRowReordered has always described the move as "this row, over that row", so the neighbour
        // the new position belongs to is what it is told about.
        var newIndex = e.Index < 0 || e.Index >= rows.Count ? rows.Count - 1 : e.Index;
        if (newIndex == oldIndex) return;

        var target = e.HasOverPayload && e.OverPayload is TItem over ? over : rows[newIndex];

        await OnRowReordered.InvokeAsync(new DataGridRowReorder<TItem>(dragged, target, oldIndex, newIndex));
        StateHasChanged();
    }

    // A tree drop carries the new parent and depth: before/after place the row among the target's
    // siblings, inside makes it the target's last child. A row cannot be dropped into its own
    // descendant - that would cut the branch out of the tree.
    private async Task MoveTreeRowAsync(FlareDropEventArgs<object> e, IList<TItem> rows, TItem dragged, int oldIndex)
    {
        if (!e.HasOverPayload || e.OverPayload is not TItem target) return;

        var targetIndex = IndexOf(rows, target);
        if (targetIndex < 0 || EqualityComparer<TItem>.Default.Equals(target, dragged)) return;

        var targetDepth = TreeDepthOf(target);
        var position = e.Edge switch
        {
            DropEdge.Before => DataGridRowDropPosition.Before,
            DropEdge.After => DataGridRowDropPosition.After,
            _ => DataGridRowDropPosition.Inside,
        };

        TItem? parent;
        int depth;
        if (position == DataGridRowDropPosition.Inside)
        {
            if (TreeIsDescendantOf(rows, dragged, targetIndex)) return;
            parent = target;
            depth = targetDepth + 1;
        }
        else
        {
            parent = TreeParentOf(rows, targetIndex);
            depth = targetDepth;
        }

        await OnRowReordered.InvokeAsync(new DataGridRowReorder<TItem>(dragged, target, oldIndex, targetIndex)
        {
            Parent = parent,
            Position = position,
            Depth = depth,
        });
        StateHasChanged();
    }

    private static int IndexOf(IList<TItem> rows, TItem item)
    {
        for (var i = 0; i < rows.Count; i++)
            if (EqualityComparer<TItem>.Default.Equals(rows[i], item))
                return i;
        return -1;
    }
}
