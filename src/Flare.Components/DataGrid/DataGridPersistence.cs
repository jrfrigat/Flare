using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// Persists DataGrid state (sorts, filters, column order, page size, hidden columns)
/// to browser localStorage. Enables users to return to their preferred view.
/// </summary>
public sealed class DataGridPersistence<TItem> : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private readonly string _storageKey;
    private IJSObjectReference? _module;

    /// <summary>Initializes a new <see cref="DataGridPersistence{TItem}"/> bound to the given storage key.</summary>
    public DataGridPersistence(IJSRuntime js, string storageKey = "flare-datagrid")
    {
        _js = js;
        _storageKey = storageKey;
    }

    /// <summary>Saves the current grid state to localStorage.</summary>
    public async Task SaveAsync(DataGridPersistedState state)
    {
        try
        {
            _module ??= await _js.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Flare.Components/js/flare-theme.js");

            var json = System.Text.Json.JsonSerializer.Serialize(state, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            });

            await _module.InvokeVoidAsync("setItem", _storageKey, json);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Loads the persisted state from localStorage. Returns null if not found.</summary>
    public async Task<DataGridPersistedState?> LoadAsync()
    {
        try
        {
            _module ??= await _js.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Flare.Components/js/flare-theme.js");

            var json = await _module.InvokeAsync<string?>("getItem", _storageKey);
            if (string.IsNullOrEmpty(json)) return null;

            return System.Text.Json.JsonSerializer.Deserialize<DataGridPersistedState>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            });
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        return null;
    }

    /// <summary>Clears the persisted state from localStorage.</summary>
    public async Task ClearAsync()
    {
        try
        {
            _module ??= await _js.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Flare.Components/js/flare-theme.js");

            await _module.InvokeVoidAsync("removeItem", _storageKey);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Disposes the persistence helper and releases its JS interop module.</summary>
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
            await _module.DisposeAsync();
    }
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
