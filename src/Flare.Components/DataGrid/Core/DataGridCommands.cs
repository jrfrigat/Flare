namespace Flare.Components;

/// <summary>
/// Immutable state mutations for DataGrid. Each command produces a new state.
/// </summary>
public static class DataGridCommands
{
    /// <summary>Sort by a column. If shift is held, adds to multi-sort.</summary>
    public static DataGridState<T> Sort<T>(DataGridState<T> state, GridColumn<T> column, bool shift) where T : notnull
    {
        var sorts = new List<DataGridSort>(state.Sorts);

        if (shift)
        {
            var existing = sorts.FindIndex(s => s.Key == column.Key);
            if (existing >= 0)
            {
                var current = sorts[existing];
                if (current.Direction == SortDirection.Ascending)
                    sorts[existing] = new DataGridSort(column.Key, SortDirection.Descending);
                else
                    sorts.RemoveAt(existing);
            }
            else
            {
                sorts.Add(new DataGridSort(column.Key, SortDirection.Ascending));
            }
        }
        else
        {
            if (sorts.Count == 1 && sorts[0].Key == column.Key)
            {
                if (sorts[0].Direction == SortDirection.Ascending)
                    sorts = [new DataGridSort(column.Key, SortDirection.Descending)];
                else
                    sorts.Clear();
            }
            else
            {
                sorts = [new DataGridSort(column.Key, SortDirection.Ascending)];
            }
        }

        return state.WithSort(sorts);
    }

    /// <summary>Apply a text filter to a column.</summary>
    public static DataGridState<T> Filter<T>(DataGridState<T> state, string columnKey, string value) where T : notnull
    {
        var filters = new Dictionary<string, string>(state.Filters);
        if (string.IsNullOrEmpty(value))
            filters.Remove(columnKey);
        else
            filters[columnKey] = value;

        return state.WithFilters(filters);
    }

    /// <summary>Clear all filters.</summary>
    public static DataGridState<T> ClearFilters<T>(DataGridState<T> state) where T : notnull =>
        state.WithFilters(new Dictionary<string, string>());

    /// <summary>Toggle item selection.</summary>
    public static DataGridState<T> ToggleSelection<T>(DataGridState<T> state, T item) where T : notnull
    {
        var selected = new HashSet<T>(state.SelectedItems);
        if (selected.Contains(item))
            selected.Remove(item);
        else
            selected.Add(item);

        return state.WithSelection(selected);
    }

    /// <summary>Select all visible items.</summary>
    public static DataGridState<T> SelectAll<T>(DataGridState<T> state, IReadOnlyList<T> visibleItems) where T : notnull
    {
        var selected = new HashSet<T>(state.SelectedItems);
        foreach (var item in visibleItems)
            selected.Add(item);

        return state.WithSelection(selected);
    }

    /// <summary>Deselect all items.</summary>
    public static DataGridState<T> DeselectAll<T>(DataGridState<T> state) where T : notnull =>
        state.WithSelection(new HashSet<T>());

    /// <summary>Toggle column visibility.</summary>
    public static DataGridState<T> ToggleColumn<T>(DataGridState<T> state, string columnKey) where T : notnull
    {
        var hidden = new HashSet<string>(state.HiddenColumns);
        if (hidden.Contains(columnKey))
            hidden.Remove(columnKey);
        else
            hidden.Add(columnKey);

        return state.WithHiddenColumns(hidden);
    }

    /// <summary>Reorder columns.</summary>
    public static DataGridState<T> ReorderColumn<T>(DataGridState<T> state, string fromKey, string toKey) where T : notnull
    {
        var order = state.ColumnOrder.ToList();
        order.Remove(fromKey);
        var idx = order.IndexOf(toKey);
        if (idx < 0) idx = order.Count;
        order.Insert(idx, fromKey);

        return state.WithColumnOrder(order);
    }

    /// <summary>Navigate to next page.</summary>
    public static DataGridState<T> NextPage<T>(DataGridState<T> state) where T : notnull =>
        state.WithPage(Math.Min(state.Page + 1, Math.Max(0, (int)Math.Ceiling((double)state.TotalCount / state.PageSize) - 1)));

    /// <summary>Navigate to previous page.</summary>
    public static DataGridState<T> PreviousPage<T>(DataGridState<T> state) where T : notnull =>
        state.WithPage(Math.Max(state.Page - 1, 0));
}
