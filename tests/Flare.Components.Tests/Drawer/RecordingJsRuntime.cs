using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// Records module method calls in order, including their arguments; bUnit's own module reference keeps
/// neither. Module imports are forwarded to the wrapped runtime.
/// </summary>
internal sealed class RecordingJsRuntime(IJSRuntime runtime) : IJSRuntime
{
    public List<string> Calls { get; } = [];

    public List<(string Id, object?[]? Args)> Invocations { get; } = [];

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        => InvokeAsync<TValue>(identifier, default, args);

    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
        object?[]? args)
    {
        if (typeof(TValue) != typeof(IJSObjectReference))
            return await runtime.InvokeAsync<TValue>(identifier, cancellationToken, args);

        var reference = await runtime.InvokeAsync<IJSObjectReference>("import", cancellationToken, args);
        return (TValue)(object)new RecordingReference(reference, this);
    }

    private sealed class RecordingReference(IJSObjectReference inner, RecordingJsRuntime owner) : IJSObjectReference
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
            object?[]? args)
        {
            owner.Calls.Add(identifier);
            owner.Invocations.Add((identifier, args));
            return inner.InvokeAsync<TValue>(identifier, cancellationToken, args);
        }

        public ValueTask DisposeAsync() => inner.DisposeAsync();
    }
}
