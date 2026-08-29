using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components;

/// <summary>
/// What <see cref="FlareFileUploadZone"/> and <see cref="FlareFileUploadButton"/> share: the hidden file
/// input, the accept/multiple/limit rules, the selected-file list and the change callback. Only the trigger
/// - a drop region or a button in a row - differs, and each subclass owns just that.
/// </summary>
public abstract class FlareFileUploadBase : FlareComponentBase
{
    /// <summary>Callback invoked with the list of selected files.</summary>
    [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>> OnFilesChanged { get; set; }
    /// <summary>Accepted file types as a MIME or extension filter string.</summary>
    [Parameter] public string? Accept { get; set; }
    /// <summary>Allows selecting multiple files when true.</summary>
    [Parameter] public bool Multiple { get; set; }
    /// <summary>Disables the control - the trigger stops opening the file picker.</summary>
    [Parameter] public bool Disabled { get; set; }
    /// <summary>Maximum number of files that can be selected.</summary>
    [Parameter] public int MaxFiles { get; set; } = 10;
    /// <summary>
    /// Largest accepted file, in bytes. Anything over it is dropped from the selection before
    /// <see cref="OnFilesChanged"/> sees it. Unlimited by default: a cap discards the user's file with no
    /// explanation, so it is the caller - who knows what the server will take - that opts in.
    /// Filtering here is UX only; always check the size again on the server.
    /// </summary>
    [Parameter] public long MaxFileSize { get; set; } = long.MaxValue;
    /// <summary>Shows the list of selected files under the trigger. Default true.</summary>
    [Parameter] public bool ShowFileList { get; set; } = true;

    /// <summary>
    /// The transfer itself. Given a <see cref="FlareUploadContext"/> - the file, a progress sink and a
    /// cancellation token - send the bytes however the application sends bytes, and report the running
    /// total to <see cref="FlareUploadContext.Progress"/>.
    /// </summary>
    /// <remarks>
    /// A delegate rather than a URL on purpose. A URL plus headers cannot express a token that refreshes
    /// mid-upload, a presigned <c>PUT</c>, a resumable protocol or an application's own retry policy - and
    /// an upload component that gets those wrong is one an application has to wrap. Flare owns the queue,
    /// the concurrency, the cancellation and the whole visual state; the application owns the wire.
    /// <para>
    /// Leave it null and the component is exactly what it was before: a file picker.
    /// </para>
    /// </remarks>
    [Parameter] public Func<FlareUploadContext, Task>? Uploader { get; set; }

    /// <summary>Starts the queue as soon as files are selected instead of waiting for
    /// <see cref="UploadAsync"/>. Ignored when <see cref="Uploader"/> is null.</summary>
    [Parameter] public bool Auto { get; set; }

    /// <summary>How many files may transfer at once. Default 1 - a server is more likely to be annoyed by
    /// a burst than by a queue.</summary>
    [Parameter] public int Concurrency { get; set; } = 1;

    /// <summary>Shows a cancel affordance on a file that is still queued or uploading. Default true.</summary>
    [Parameter] public bool AllowCancel { get; set; } = true;

    /// <summary>Shows a retry affordance on a file that failed or was cancelled. Default true.</summary>
    [Parameter] public bool AllowRetry { get; set; } = true;

    /// <summary>Raised when one file finishes successfully.</summary>
    [Parameter] public EventCallback<FlareUploadFile> OnUploadCompleted { get; set; }

    /// <summary>Raised when one file's transfer throws. The exception is not rethrown - the row carries
    /// the message and the queue continues.</summary>
    [Parameter] public EventCallback<FlareUploadFile> OnUploadFailed { get; set; }

    /// <summary>Raised once when no file is queued or uploading any more, whatever the outcomes.</summary>
    [Parameter] public EventCallback OnAllCompleted { get; set; }

    /// <summary>The files the user has selected, in selection order.</summary>
    protected readonly List<IBrowserFile> Files = [];

    /// <summary>Per-file queue state, in selection order. Empty until files are selected.</summary>
    protected readonly List<FlareUploadFile> Queue = [];

    private readonly Dictionary<string, CancellationTokenSource> _cancels = [];
    private bool _running;

    /// <summary>Id linking the trigger's <c>&lt;label for&gt;</c> to the hidden input.</summary>
    protected readonly string InputId = $"flare-fu-{Guid.NewGuid():N}";

    /// <summary>Reads the picked files, applies <see cref="MaxFiles"/> and <see cref="MaxFileSize"/>, and
    /// raises <see cref="OnFilesChanged"/>.</summary>
    protected async Task HandleChangeAsync(InputFileChangeEventArgs e)
    {
        if (Disabled) return;
        Files.Clear();
        Files.AddRange(e.GetMultipleFiles(MaxFiles).Where(f => f.Size <= MaxFileSize));

        CancelAll();
        Queue.Clear();
        foreach (var f in Files) Queue.Add(new FlareUploadFile(f));

        await OnFilesChanged.InvokeAsync(Files.AsReadOnly());
        if (Auto && Uploader is not null) await UploadAsync();
    }

    /// <summary>
    /// Runs the queue: every file still queued is sent through <see cref="Uploader"/>, at most
    /// <see cref="Concurrency"/> at a time. Returns when nothing is left in flight. Safe to call again
    /// after a failure - it picks up whatever is still queued and skips what already finished.
    /// </summary>
    public async Task UploadAsync()
    {
        if (Uploader is null || _running) return;
        _running = true;
        try
        {
            var lanes = Math.Max(1, Concurrency);
            using var gate = new SemaphoreSlim(lanes, lanes);
            var pending = Queue.Where(q => q.State == FlareUploadState.Queued).ToList();

            await Task.WhenAll(pending.Select(async item =>
            {
                await gate.WaitAsync();
                try { await SendAsync(item); }
                finally { gate.Release(); }
            }));
        }
        finally
        {
            _running = false;
        }

        await OnAllCompleted.InvokeAsync();
    }

    /// <summary>Cancels one file by its <see cref="FlareUploadFile.Id"/>. A file that already finished is
    /// left alone.</summary>
    /// <param name="id">The queue entry to cancel.</param>
    public Task CancelAsync(string id)
    {
        if (_cancels.TryGetValue(id, out var cts)) cts.Cancel();
        else if (Queue.FirstOrDefault(q => q.Id == id) is { State: FlareUploadState.Queued } queued)
            queued.State = FlareUploadState.Cancelled;   // never started; nothing to abort
        return InvokeAsync(StateHasChanged);
    }

    /// <summary>Puts a failed or cancelled file back in the queue and runs it again.</summary>
    /// <param name="id">The queue entry to retry.</param>
    public Task RetryAsync(string id)
    {
        if (Queue.FirstOrDefault(q => q.Id == id) is not { } item) return Task.CompletedTask;
        if (item.State is not (FlareUploadState.Failed or FlareUploadState.Cancelled)) return Task.CompletedTask;
        item.State = FlareUploadState.Queued;
        item.BytesSent = 0;
        item.Error = null;
        return UploadAsync();
    }

    private async Task SendAsync(FlareUploadFile item)
    {
        // Cancelled while it waited its turn in the queue.
        if (item.State != FlareUploadState.Queued) return;

        var cts = new CancellationTokenSource();
        _cancels[item.Id] = cts;
        item.State = FlareUploadState.Uploading;
        await InvokeAsync(StateHasChanged);

        // Progress arrives from whatever thread the caller's transfer runs on, so every touch of the
        // render state goes back through the dispatcher.
        var progress = new Progress<long>(sent =>
        {
            item.BytesSent = sent;
            _ = InvokeAsync(StateHasChanged);
        });

        try
        {
            await Uploader!(new FlareUploadContext(item.File, progress, cts.Token));
            item.State = FlareUploadState.Completed;
            item.BytesSent = item.File.Size;
            await OnUploadCompleted.InvokeAsync(item);
        }
        catch (OperationCanceledException)
        {
            item.State = FlareUploadState.Cancelled;
        }
        catch (Exception ex)
        {
            // One file failing must not take the queue down with it.
            item.State = FlareUploadState.Failed;
            item.Error = ex.Message;
            await OnUploadFailed.InvokeAsync(item);
        }
        finally
        {
            _cancels.Remove(item.Id);
            cts.Dispose();
            await InvokeAsync(StateHasChanged);
        }
    }

    private void CancelAll()
    {
        foreach (var cts in _cancels.Values) cts.Cancel();
        _cancels.Clear();
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        CancelAll();
        return base.DisposeAsync();
    }

    /// <summary>Renders a byte count as a short human-readable size (B / KB / MB).</summary>
    protected static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024):F1} MB",
    };
}
