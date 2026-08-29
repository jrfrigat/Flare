using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components;

/// <summary>Where one file stands in an upload queue.</summary>
public enum FlareUploadState
{
    /// <summary>Selected and waiting for its turn.</summary>
    Queued = 0,
    /// <summary>Bytes are moving.</summary>
    Uploading,
    /// <summary>Finished without error.</summary>
    Completed,
    /// <summary>The transfer threw; <see cref="FlareUploadFile.Error"/> says what.</summary>
    Failed,
    /// <summary>Cancelled by the user before it finished.</summary>
    Cancelled,
}

/// <summary>
/// What the caller's transfer delegate is handed for one file: the file, a sink to report bytes sent,
/// and the token that goes red when the user cancels.
/// </summary>
/// <param name="File">The browser file to send.</param>
/// <param name="Progress">Report the running total of bytes sent, not a delta.</param>
/// <param name="CancellationToken">Cancelled when the user presses cancel or the component is disposed.</param>
public readonly record struct FlareUploadContext(
    IBrowserFile File,
    IProgress<long> Progress,
    CancellationToken CancellationToken);

/// <summary>
/// One file's place in the queue, as the component tracks it.
/// </summary>
/// <remarks>
/// Deliberately a mutable class rather than a record: the file list re-renders from these as bytes move,
/// and replacing the instance on every progress tick would churn the diff for a component that can be
/// showing dozens of rows.
/// </remarks>
public sealed class FlareUploadFile
{
    /// <param name="file">The selected browser file.</param>
    public FlareUploadFile(IBrowserFile file)
    {
        File = file;
        Id = Guid.NewGuid().ToString("N");
    }

    /// <summary>Stable identity for the row, and the handle <c>CancelAsync</c> takes.</summary>
    public string Id { get; }

    /// <summary>The selected file.</summary>
    public IBrowserFile File { get; }

    /// <summary>Where this file stands.</summary>
    public FlareUploadState State { get; internal set; } = FlareUploadState.Queued;

    /// <summary>Bytes the transfer has reported so far.</summary>
    public long BytesSent { get; internal set; }

    /// <summary>Message from the exception that failed the transfer; null unless
    /// <see cref="State"/> is <see cref="FlareUploadState.Failed"/>.</summary>
    public string? Error { get; internal set; }

    /// <summary>How far through, from 0 to 1. A zero-length file reads 1 once it completes rather than
    /// dividing by zero.</summary>
    public double Progress => File.Size > 0
        ? Math.Clamp((double)BytesSent / File.Size, 0, 1)
        : State == FlareUploadState.Completed ? 1 : 0;

    /// <summary>True while this file can still be cancelled.</summary>
    public bool IsActive => State is FlareUploadState.Queued or FlareUploadState.Uploading;
}
