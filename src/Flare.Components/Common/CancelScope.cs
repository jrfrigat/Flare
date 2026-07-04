namespace Flare.Components;

/// <summary>
/// A single-in-flight cancellation scope for debounced / cancelable async work such as type-ahead
/// search. Each call to <see cref="Next"/> cancels AND disposes the previous token source before
/// issuing a fresh token, so a superseded operation stops cleanly and no <see cref="CancellationTokenSource"/>
/// is leaked (the hand-rolled "cancel then replace without dispose" pattern leaks one per keystroke).
/// </summary>
internal sealed class CancelScope : IDisposable
{
    private CancellationTokenSource? _cts;

    /// <summary>Cancels and disposes any in-flight token, then returns a fresh <see cref="CancellationToken"/>.</summary>
    public CancellationToken Next()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        return _cts.Token;
    }

    /// <summary>Cancels the current token without issuing a new one (e.g. on blur / when clearing).</summary>
    public void Cancel() => _cts?.Cancel();

    /// <inheritdoc/>
    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
