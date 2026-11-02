namespace Flare.Abstractions;

/// <summary>Coordinates close guards for one dialog. Cascaded by the dialog to its content and
/// shared with its service handle, so every close request checks the same registered guards.</summary>
public sealed class DialogCloseContext
{
    private readonly List<Registration> _guards = [];
    private int _checking;

    /// <summary>Registers a check that returns true to allow closing or false to keep the dialog open.
    /// Dispose the returned registration when its owner leaves the dialog.</summary>
    /// <param name="guard">An asynchronous check receiving the reason for closing.</param>
    /// <returns>A registration that removes only this check when disposed.</returns>
    public IDisposable RegisterGuard(Func<DialogCloseReason, ValueTask<bool>> guard)
    {
        ArgumentNullException.ThrowIfNull(guard);
        var registration = new Registration(this, guard);
        lock (_guards) _guards.Add(registration);
        return registration;
    }

    internal async Task<bool> TryCloseAsync(DialogCloseReason reason,
        Func<DialogCloseReason, ValueTask<bool>>? beforeClose, Func<ValueTask<bool>> close,
        CancellationToken cancellationToken = default)
    {
        // A second attempt must not show another prompt or close with a different result.
        if (Interlocked.CompareExchange(ref _checking, 1, 0) != 0) return false;
        try
        {
            if (cancellationToken.IsCancellationRequested) return false;
            if (beforeClose is not null && !await beforeClose(reason)) return false;
            Registration[] guards;
            lock (_guards) guards = _guards.ToArray();
            foreach (var registration in guards)
            {
                if (cancellationToken.IsCancellationRequested) return false;
                if (registration.Active && !await registration.Guard(reason)) return false;
            }
            return !cancellationToken.IsCancellationRequested && await close();
        }
        finally { Volatile.Write(ref _checking, 0); }
    }

    private sealed class Registration(DialogCloseContext owner,
        Func<DialogCloseReason, ValueTask<bool>> guard) : IDisposable
    {
        public Func<DialogCloseReason, ValueTask<bool>> Guard { get; } = guard;
        public bool Active { get; private set; } = true;

        public void Dispose()
        {
            lock (owner._guards)
            {
                Active = false;
                owner._guards.Remove(this);
            }
        }
    }
}
