using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

/// <summary>
/// Remove is the third affordance and the odd one out: cancel belongs to a transfer in flight and retry
/// to one that stopped, but remove belongs to the ROW, so it does not depend on where the file got to.
/// </summary>
public class FileUploadRowTests : FlareTestContext
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
