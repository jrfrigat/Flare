using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.Zone}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.FileUpload.Progress}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.FileUpload.FileActions}"));
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.FileUploading}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.Progress}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.FileActions} button"));
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

        Assert.Contains("413 Payload Too Large", cut.Find($".{Css.Classes.FileUpload.FileError}").TextContent);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.FileFailed}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.FileActions} button"));
    }

    [Fact]
    public void A_completed_row_offers_neither_cancel_nor_retry()
    {
        var done = Q();
        done.State = FlareUploadState.Completed;

        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new List<FlareUploadFile> { done })
            .Add(x => x.Interactive, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.FileCompleted}"));
        // Named rather than counted: remove is offered on every row, including this one, so "no buttons"
        // would assert the absence of an affordance this test is not about.
        var labels = cut.FindAll($".{Css.Classes.FileUpload.FileActions} button")
            .Select(b => b.GetAttribute("aria-label"))
            .ToArray();
        Assert.DoesNotContain(FlareStrings.FileUpload_Cancel, labels);
        Assert.DoesNotContain(FlareStrings.FileUpload_Retry, labels);
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

        cut.Find($".{Css.Classes.FileUpload.FileActions} button").Click();

        Assert.Equal(failed.Id, retried);
    }
}
// ------------------------------------------------------------------------------
// The row surface: removal, and replacing the row wholesale.
// ------------------------------------------------------------------------------

/// <summary>
/// Remove is the third affordance and the odd one out: cancel belongs to a transfer in flight and retry
/// to one that stopped, but remove belongs to the ROW, so it does not depend on where the file got to.
/// </summary>
public class C_FileUploadRowTests : FlareTestContext
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

    private static FlareUploadFile Row(FlareUploadState state = FlareUploadState.Queued) =>
        new(new FakeFile("a.txt", 100)) { State = state };

    [Fact]
    public void RemoveReportsTheRowId()
    {
        var removed = string.Empty;
        var row = Row(FlareUploadState.Completed);
        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new[] { row })
            .Add(x => x.Interactive, true)
            .Add(x => x.OnRemove, EventCallback.Factory.Create<string>(this, id => removed = id)));

        cut.FindAll("button").Last().Click();

        Assert.Equal(row.Id, removed);
    }

    [Theory]
    [InlineData(FlareUploadState.Queued)]
    [InlineData(FlareUploadState.Uploading)]
    [InlineData(FlareUploadState.Completed)]
    [InlineData(FlareUploadState.Failed)]
    [InlineData(FlareUploadState.Cancelled)]
    public void RemoveIsAvailableWhateverTheRowsState(FlareUploadState state)
    {
        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new[] { Row(state) })
            .Add(x => x.Interactive, true));

        Assert.Contains(cut.FindAll("button"), b => b.GetAttribute("aria-label") == FlareStrings.FileUpload_Remove);
    }

    [Fact]
    public void RemoveCanBeTurnedOff()
    {
        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new[] { Row() })
            .Add(x => x.Interactive, true)
            .Add(x => x.AllowRemove, false));

        Assert.DoesNotContain(cut.FindAll("button"), b => b.GetAttribute("aria-label") == FlareStrings.FileUpload_Remove);
    }

    // The template replaces the row rather than decorating it: an application that supplies one owns the
    // whole row, affordances included, and gets the queue entry to build it from.
    [Fact]
    public void FileTemplateReplacesTheRowBody()
    {
        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, new[] { Row() })
            .Add(x => x.Interactive, true)
            .Add(x => x.FileTemplate, item => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "mine");
                builder.AddContent(2, item.File.Name);
                builder.CloseElement();
            }));

        Assert.Equal("a.txt", cut.Find(".mine").TextContent);
        Assert.Empty(cut.FindAll("button"));
    }
}
