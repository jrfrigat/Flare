using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class DataGridPersistenceTests
{
    private record Person(string Name);

    // Minimal IJSRuntime that implements an in-memory localStorage, so SaveAsync/LoadAsync/ClearAsync
    // exercise the real interop identifiers ("localStorage.setItem" etc.) end to end.
    private sealed class FakeLocalStorageJsRuntime : IJSRuntime
    {
        public readonly Dictionary<string, string> Store = new(StringComparer.Ordinal);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            switch (identifier)
            {
                case "localStorage.setItem":
                    Store[(string)args![0]!] = (string)args![1]!;
                    return new ValueTask<TValue>(default(TValue)!);
                case "localStorage.removeItem":
                    Store.Remove((string)args![0]!);
                    return new ValueTask<TValue>(default(TValue)!);
                case "localStorage.getItem":
                    var value = Store.TryGetValue((string)args![0]!, out var v) ? v : null;
                    return new ValueTask<TValue>((TValue)(object?)value!);
                default:
                    return new ValueTask<TValue>(default(TValue)!);
            }
        }
    }

    [Fact]
    public async Task SaveThenLoad_RoundTripsState_ThroughLocalStorage()
    {
        var js = new FakeLocalStorageJsRuntime();
        var persistence = new DataGridPersistence<Person>(new Flare.Infrastructure.BrowserStorage(js),"grid-key");

        var state = new DataGridPersistedState
        {
            Sorts = [new PersistedSort { Key = "Name", Direction = "Descending" }],
            Filters = new Dictionary<string, string> { ["Dept"] = "Eng" },
            ColumnOrder = ["Dept", "Name"],
            HiddenColumns = ["Score"],
            Page = 2,
            PageSize = 25,
        };

        await persistence.SaveAsync(state);

        // Proves the fix: state is actually written to localStorage under the key
        // (the old module-export code never reached real localStorage).
        Assert.True(js.Store.ContainsKey("grid-key"));

        var loaded = await persistence.LoadAsync();

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Page);
        Assert.Equal(25, loaded.PageSize);
        var sort = Assert.Single(loaded.Sorts!);
        Assert.Equal("Name", sort.Key);
        Assert.Equal("Descending", sort.Direction);
        Assert.Equal("Eng", loaded.Filters!["Dept"]);
        Assert.Equal(["Dept", "Name"], loaded.ColumnOrder);
        Assert.Equal(["Score"], loaded.HiddenColumns);
    }

    [Fact]
    public async Task Load_ReturnsNull_WhenNothingStored()
    {
        var js = new FakeLocalStorageJsRuntime();
        var persistence = new DataGridPersistence<Person>(new Flare.Infrastructure.BrowserStorage(js),"absent");

        Assert.Null(await persistence.LoadAsync());
    }

    [Fact]
    public async Task Clear_RemovesStoredState()
    {
        var js = new FakeLocalStorageJsRuntime();
        var persistence = new DataGridPersistence<Person>(new Flare.Infrastructure.BrowserStorage(js),"grid-key");
        await persistence.SaveAsync(new DataGridPersistedState { PageSize = 10 });
        Assert.True(js.Store.ContainsKey("grid-key"));

        await persistence.ClearAsync();

        Assert.False(js.Store.ContainsKey("grid-key"));
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid persistence wiring - a PersistStateKey grid must save the user's
// FIRST change even when storage starts empty. Regression guard: _persistenceLoaded
// was only set when prior saved state existed, so a brand-new grid silently dropped
// every change until something had already been stored.
// ------------------------------------------------------------------------------
