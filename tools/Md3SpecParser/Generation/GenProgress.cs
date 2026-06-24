namespace Flare.Tools.Md3SpecParser.Generation;

/// <summary>Phase of work reported for a type group.</summary>
public enum GenPhase
{
    /// <summary>A source is being downloaded.</summary>
    Download,

    /// <summary>A source is being parsed.</summary>
    Parse,

    /// <summary>The output file has been written.</summary>
    Written,

    /// <summary>The group was skipped (new-only mode).</summary>
    Skipped,

    /// <summary>Generation failed.</summary>
    Failed,
}

/// <summary>A progress update for one type group.</summary>
/// <param name="Type">Component type (display label).</param>
/// <param name="TotalSources">Number of sources for the type.</param>
/// <param name="Downloaded">Sources downloaded so far.</param>
/// <param name="Parsed">Sources parsed so far.</param>
/// <param name="Phase">Current phase.</param>
/// <param name="Current">Current source name (file/url), if any.</param>
/// <param name="CurrentBytes">Bytes downloaded for the current file (Download phase).</param>
/// <param name="CurrentTotalBytes">Total bytes of the current file if known.</param>
public sealed record GenProgress(
    string Type, int TotalSources, int Downloaded, int Parsed, GenPhase Phase,
    string? Current = null, long CurrentBytes = 0, long? CurrentTotalBytes = null);

/// <summary>An <see cref="IProgress{T}"/> that invokes its callback synchronously.</summary>
/// <remarks>
/// Unlike <see cref="Progress{T}"/>, this does not post to a synchronization context or
/// the thread pool, so console progress writes stay ordered with the surrounding output.
/// </remarks>
public sealed class SyncProgress<T>(Action<T> handler) : IProgress<T>
{
    /// <inheritdoc />
    public void Report(T value) => handler(value);
}
