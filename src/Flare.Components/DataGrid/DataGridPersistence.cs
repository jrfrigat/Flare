namespace Flare.Components;

/// <summary>
/// Persists DataGrid state (sorts, filters, column order, page size, hidden columns) to browser
/// localStorage via <see cref="IBrowserStorage"/>. Enables users to return to their preferred view.
/// </summary>
/// <remarks>
/// State (de)serialization and the SSR/quota/corrupt-payload safety net live in the
/// <see cref="IBrowserStorage"/> adapter, so this type is a thin, key-bound typed wrapper.
/// </remarks>
public sealed class DataGridPersistence<TItem>
{
    private readonly IBrowserStorage _storage;
    private readonly string _storageKey;

    /// <summary>Initializes a new <see cref="DataGridPersistence{TItem}"/> bound to the given storage key.</summary>
    public DataGridPersistence(IBrowserStorage storage, string storageKey = "flare-datagrid")
    {
        _storage = storage;
        _storageKey = storageKey;
    }

    /// <summary>Saves the current grid state to localStorage.</summary>
    public Task SaveAsync(DataGridPersistedState state) =>
        _storage.SetAsync(_storageKey, state).AsTask();

    /// <summary>Loads the persisted state from localStorage. Returns null if not found.</summary>
    public Task<DataGridPersistedState?> LoadAsync() =>
        _storage.GetAsync<DataGridPersistedState>(_storageKey).AsTask();

    /// <summary>Clears the persisted state from localStorage.</summary>
    public Task ClearAsync() =>
        _storage.RemoveAsync(_storageKey).AsTask();
}

/// <summary>
/// Serializable state that can be persisted to localStorage.
/// Contains the minimal set of user preferences that should survive page refreshes.
/// </summary>
public sealed class DataGridPersistedState
{
    /// <summary>Active sorts (column key + direction).</summary>
    public List<PersistedSort>? Sorts { get; set; }
    /// <summary>Active text filters (column key -> value).</summary>
    public Dictionary<string, string>? Filters { get; set; }
    /// <summary>Column display order (titles in order).</summary>
    public List<string>? ColumnOrder { get; set; }
    /// <summary>Hidden column keys.</summary>
    public List<string>? HiddenColumns { get; set; }
    /// <summary>Current page index.</summary>
    public int Page { get; set; }
    /// <summary>Rows per page.</summary>
    public int PageSize { get; set; }
}

/// <summary>Serializable sort instruction for persistence.</summary>
public sealed class PersistedSort
{
    /// <summary>Column key/title.</summary>
    public string Key { get; set; } = "";
    /// <summary>Sort direction (Ascending/Descending).</summary>
    public string Direction { get; set; } = "Ascending";
}
