using System.Text.Json;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// Persists DataGrid state (sorts, filters, column order, page size, hidden columns)
/// to browser localStorage. Enables users to return to their preferred view.
/// </summary>
/// <remarks>
/// Uses the built-in <c>localStorage.getItem/setItem/removeItem</c> JS interop directly
/// (the same approach as the theme storage), so no custom Flare JS module is required.
/// All calls degrade to a no-op during SSR/prerender or when the circuit is gone.
/// </remarks>
public sealed class DataGridPersistence<TItem>
{
    private readonly IJSRuntime _js;
    private readonly string _storageKey;

    // CamelCase to match the on-the-wire shape used elsewhere; cached to avoid re-allocating
    // the options on every save/load.
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

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
            var json = JsonSerializer.Serialize(state, s_jsonOptions);
            await _js.InvokeVoidAsync("localStorage.setItem", _storageKey, json);
        }
        catch (InvalidOperationException) { } // SSR / prerender (no JS runtime)
        catch (JSDisconnectedException) { }
        catch (JSException) { } // storage blocked/quota (e.g. private mode)
    }

    /// <summary>Loads the persisted state from localStorage. Returns null if not found.</summary>
    public async Task<DataGridPersistedState?> LoadAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", _storageKey);
            if (string.IsNullOrEmpty(json)) return null;

            return JsonSerializer.Deserialize<DataGridPersistedState>(json, s_jsonOptions);
        }
        catch (InvalidOperationException) { } // SSR / prerender (no JS runtime)
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        catch (JsonException) { } // corrupt/old payload - ignore and fall back to defaults
        return null;
    }

    /// <summary>Clears the persisted state from localStorage.</summary>
    public async Task ClearAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", _storageKey);
        }
        catch (InvalidOperationException) { } // SSR / prerender (no JS runtime)
        catch (JSDisconnectedException) { }
        catch (JSException) { }
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
