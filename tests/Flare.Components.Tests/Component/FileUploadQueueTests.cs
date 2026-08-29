using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The upload queue: what the component owns now that the transfer itself belongs to the caller.
/// </summary>
/// <remarks>
/// The contract is a delegate rather than a URL, so these drive the component with a fake transfer and
/// assert on the state machine - which is the half every application used to rewrite. A component with no
/// <c>Uploader</c> must still behave exactly as the file picker it was, and that is pinned too.
/// </remarks>
public class C_FileUploadQueueTests : FlareTestContext
{
    private sealed class FakeFile(string name, long size) : IBrowserFile
    {
        public string Name { get; } = name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public long Size { get; } = size;
        public string ContentType => "application/octet-stream";
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default) =>
            new MemoryStream(new byte[Size]);
    }

    private static FlareUploadFile Q(string name = "a.txt", long size = 100) => new(new FakeFile(name, size));

    [Fact]
    public void A_file_starts_queued_and_reports_no_progress()
    {
        var f = Q();

        Assert.Equal(FlareUploadState.Queued, f.State);
        Assert.Equal(0, f.Progress);
        Assert.True(f.IsActive);
    }

    [Fact]
    public void Progress_is_the_fraction_sent()
    {
        var f = Q(size: 200);

        f.BytesSent = 50;
        Assert.Equal(0.25, f.Progress);

        f.BytesSent = 200;
        Assert.Equal(1, f.Progress);
    }

    [Fact]
    public void Progress_never_leaves_zero_to_one_even_if_the_transfer_over_reports()
    {
        // A caller reporting a delta instead of a running total, or a chunked protocol double-counting a
        // retried chunk, must not drive a progress bar past its track.
        var f = Q(size: 100);
        f.BytesSent = 250;

        Assert.Equal(1, f.Progress);
    }

    [Fact]
    public void An_empty_file_reads_zero_until_it_completes_rather_than_dividing_by_zero()
    {
        var f = Q(size: 0);
        Assert.Equal(0, f.Progress);

        f.State = FlareUploadState.Completed;
        Assert.Equal(1, f.Progress);
    }

    [Fact]
    public void Only_a_queued_or_uploading_file_is_still_cancellable()
    {
        foreach (var active in new[] { FlareUploadState.Queued, FlareUploadState.Uploading })
        {
            var f = Q();
            f.State = active;
            Assert.True(f.IsActive);
        }

        foreach (var settled in new[] { FlareUploadState.Completed, FlareUploadState.Failed, FlareUploadState.Cancelled })
        {
            var f = Q();
            f.State = settled;
            Assert.False(f.IsActive);
        }
    }

    [Fact]
    public void Every_queue_entry_gets_its_own_identity()
    {
        // The id is the handle CancelAsync and RetryAsync take, so two files of the same name must not
        // share one.
        var a = Q("same.txt");
        var b = Q("same.txt");

        Assert.NotEqual(a.Id, b.Id);
        Assert.NotEmpty(a.Id);
    }

    [Fact]
    public void The_picker_surface_is_unchanged_when_no_uploader_is_given()
    {
        // Backward compatibility: without an Uploader the component renders the same list it always did,
        // with no progress, no actions and no state classes.
        var cut = Render<FlareFileUploadZone>();

        Assert.NotEmpty(cut.FindAll(".flare-file-upload__zone"));
        Assert.Empty(cut.FindAll(".flare-file-upload__progress"));
        Assert.Empty(cut.FindAll(".flare-file-upload__file-actions"));
    }

    [Fact]
    public void The_list_paints_a_running_transfer_with_a_bar_and_a_cancel()
    {
        var running = Q(size: 100);
        running.State = FlareUploadState.Uploading;
        running.BytesSent = 40;

        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new List<FlareUploadFile> { running })
            .Add(x => x.Interactive, true));

        Assert.NotEmpty(cut.FindAll(".flare-file-upload__file--uploading"));
        Assert.NotEmpty(cut.FindAll(".flare-file-upload__progress"));
        Assert.NotEmpty(cut.FindAll(".flare-file-upload__file-actions button"));
    }

    [Fact]
    public void A_failed_row_shows_its_message_and_offers_a_retry()
    {
        var failed = Q();
        failed.State = FlareUploadState.Failed;
        failed.Error = "413 Payload Too Large";

        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new List<FlareUploadFile> { failed })
            .Add(x => x.Interactive, true));

        Assert.Contains("413 Payload Too Large", cut.Find(".flare-file-upload__file-error").TextContent);
        Assert.NotEmpty(cut.FindAll(".flare-file-upload__file--failed"));
        Assert.NotEmpty(cut.FindAll(".flare-file-upload__file-actions button"));
    }

    [Fact]
    public void A_completed_row_offers_neither_cancel_nor_retry()
    {
        var done = Q();
        done.State = FlareUploadState.Completed;

        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new List<FlareUploadFile> { done })
            .Add(x => x.Interactive, true));

        Assert.NotEmpty(cut.FindAll(".flare-file-upload__file--completed"));
        Assert.Empty(cut.FindAll(".flare-file-upload__file-actions button"));
    }

    [Fact]
    public void Cancel_and_retry_report_the_row_they_belong_to()
    {
        var failed = Q();
        failed.State = FlareUploadState.Failed;
        string? retried = null;

        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new List<FlareUploadFile> { failed })
            .Add(x => x.Interactive, true)
            .Add(x => x.OnRetry, (string id) => retried = id));

        cut.Find(".flare-file-upload__file-actions button").Click();

        Assert.Equal(failed.Id, retried);
    }
}
