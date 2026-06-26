using System.Text.Json;
using Flare.Components;
using Microsoft.JSInterop;

namespace Flare.Infrastructure;

/// <summary>
/// <see cref="IBrowserStorage"/> backed by the browser <c>localStorage</c>. Values are stored as
/// camel-cased JSON. Degrades to a no-op / default during SSR/prerender or when storage is
/// unavailable: the JS runtime is not yet available (<see cref="InvalidOperationException"/>), the
/// circuit is gone (<see cref="JSDisconnectedException"/>), storage is blocked or over quota
/// (<see cref="JSException"/>, e.g. private mode), or a stored payload is corrupt
/// (<see cref="JsonException"/>).
/// </summary>
public sealed class BrowserStorage : IBrowserStorage
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IJSRuntime _js;

    /// <summary>Creates the storage over the given JS runtime.</summary>
    public BrowserStorage(IJSRuntime js) => _js = js;

    /// <inheritdoc />
    public async ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json, Json);
        }
        catch (InvalidOperationException) { return default; } // SSR / prerender
        catch (JSDisconnectedException) { return default; }
        catch (JSException) { return default; }              // storage blocked / quota
        catch (JsonException) { return default; }             // corrupt / old payload
    }

    /// <inheritdoc />
    public async ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, JsonSerializer.Serialize(value, Json));
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { } // storage blocked / quota (e.g. private mode)
    }

    /// <inheritdoc />
    public async ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", cancellationToken, key);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { } // storage blocked / quota (e.g. private mode)
    }

    /// <inheritdoc />
    public async ValueTask<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);
            return !string.IsNullOrEmpty(json);
        }
        catch (InvalidOperationException) { return false; }
        catch (JSDisconnectedException) { return false; }
        catch (JSException) { return false; } // storage blocked / quota (e.g. private mode)
    }
}
