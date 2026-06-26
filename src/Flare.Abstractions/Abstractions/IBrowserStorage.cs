namespace Flare.Components;

/// <summary>
/// Typed wrapper over the browser's <c>localStorage</c>. Inject this instead of calling
/// <see cref="Microsoft.JSInterop.IJSRuntime"/> directly; values are JSON-serialized on write and
/// deserialized on read. Every method degrades to a no-op (or <c>default</c>) during SSR/prerender,
/// when no JS runtime is available, so callers need no guard of their own.
/// </summary>
public interface IBrowserStorage
{
    /// <summary>Reads and deserializes the value stored under <paramref name="key"/>; returns <c>default</c> when absent.</summary>
    ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Serializes <paramref name="value"/> and stores it under <paramref name="key"/>.</summary>
    ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>Removes the entry stored under <paramref name="key"/> (a no-op when it does not exist).</summary>
    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Returns true when a non-empty entry exists under <paramref name="key"/>.</summary>
    ValueTask<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default);
}
