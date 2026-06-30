namespace Flare.Abstractions;

/// <summary>
/// The outcome of a component dialog: either a cancellation, or a confirmation that may carry a
/// payload returned by the dialog body. Construct via <see cref="Ok(object?)"/> / <see cref="Cancel"/>.
/// </summary>
public sealed class DialogResult
{
    private DialogResult(bool cancelled, object? data)
    {
        Cancelled = cancelled;
        Data = data;
    }

    /// <summary>
    /// <see langword="true"/> when the dialog was dismissed or cancelled rather than confirmed
    /// (for example via the cancel button, the scrim, or the Escape key).
    /// </summary>
    public bool Cancelled { get; }

    /// <summary>The payload returned by the dialog body, or null. Only meaningful when not <see cref="Cancelled"/>.</summary>
    public object? Data { get; }

    /// <summary>A confirmed result carrying an optional payload.</summary>
    /// <param name="data">The value to return to the caller, or null.</param>
    public static DialogResult Ok(object? data = null) => new(cancelled: false, data);

    /// <summary>A confirmed result carrying a strongly-typed payload.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="data">The value to return to the caller.</param>
    public static DialogResult Ok<T>(T data) => new(cancelled: false, data);

    /// <summary>A cancelled (dismissed) result with no payload.</summary>
    public static DialogResult Cancel() => new(cancelled: true, data: null);

    /// <summary>
    /// Gets the payload cast to <typeparamref name="T"/>, or <see langword="default"/> when the
    /// dialog was cancelled or the payload is absent or of a different type.
    /// </summary>
    /// <typeparam name="T">The expected payload type.</typeparam>
    public T? GetData<T>() => Data is T value ? value : default;
}
