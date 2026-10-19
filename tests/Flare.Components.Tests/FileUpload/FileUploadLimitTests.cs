using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

/// <summary>
/// The limit policy around a picked selection. It used to ask the input API for at most
/// <c>MaxFiles</c> entries, which throws when more were picked, and it dropped oversized files without a
/// word; a selection with nothing usable also cleared whatever the user had already chosen.
/// </summary>
public class FileUploadLimitTests : FlareTestContext
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

    // HandleChangeAsync is protected: the test drives the same path the InputFile's change event does,
    // including the render Blazor performs after an event handler returns.
    private sealed class ZoneHarness : FlareFileUploadZone
    {
        public async Task SelectAsync(IReadOnlyList<IBrowserFile> files)
        {
            await HandleChangeAsync(new InputFileChangeEventArgs(files));
            StateHasChanged();
        }

        public IReadOnlyList<IBrowserFile> Selected => Files;
    }

    private sealed class ButtonHarness : FlareFileUploadButton
    {
        public async Task SelectAsync(IReadOnlyList<IBrowserFile> files)
        {
            await HandleChangeAsync(new InputFileChangeEventArgs(files));
            StateHasChanged();
        }
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previous = CultureInfo.CurrentUICulture;

        public CultureScope(string name) => CultureInfo.CurrentUICulture = new CultureInfo(name);

        public void Dispose() => CultureInfo.CurrentUICulture = _previous;
    }

    private static IReadOnlyList<IBrowserFile> Files(params (string Name, long Size)[] files) =>
        files.Select(f => (IBrowserFile)new FakeFile(f.Name, f.Size)).ToList();

    private IRenderedComponent<ZoneHarness> RenderZone(
        List<IReadOnlyList<IBrowserFile>> received, int maxFiles = 10, long maxSize = long.MaxValue)
        => Render<ZoneHarness>(p => p
            .Add(x => x.Multiple, true)
            .Add(x => x.MaxFiles, maxFiles)
            .Add(x => x.MaxFileSize, maxSize)
            .Add(x => x.ShowFileList, true)
            .Add(x => x.OnFilesChanged,
                EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>>(this, received.Add)));

    private static string RejectionsText<TComponent>(IRenderedComponent<TComponent> cut)
        where TComponent : IComponent =>
        cut.Find($"ul.{Css.Classes.FileUpload.Errors}").TextContent;

    [Fact]
    public async Task AValidSelection_ReplacesTheFilesAndRaisesOneCallback()
    {
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = RenderZone(received);

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("a.txt", 10), ("b.txt", 20))));

        Assert.Single(received);
        Assert.Equal(["a.txt", "b.txt"], received[0].Select(f => f.Name));
        Assert.Empty(cut.FindAll($"ul.{Css.Classes.FileUpload.Errors}"));
    }

    [Fact]
    public async Task AnOversizeOnlySelection_KeepsThePreviousSelectionAndRaisesNoCallback()
    {
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = RenderZone(received, maxSize: 100);
        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("small.txt", 10))));

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("huge.bin", 5000))));

        Assert.Single(received);
        Assert.Equal(["small.txt"], cut.Instance.Selected.Select(f => f.Name));
        var messages = RejectionsText(cut);
        Assert.Contains("huge.bin", messages, StringComparison.Ordinal);
        Assert.Contains("100 B", messages, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AMixedSelection_KeepsTheValidFilesAndExplainsTheRejectedOne()
    {
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = RenderZone(received, maxSize: 1000);

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("small.txt", 10), ("huge.bin", 5000))));

        Assert.Single(received);
        Assert.Equal(["small.txt"], received[0].Select(f => f.Name));
        Assert.Contains("huge.bin", RejectionsText(cut), StringComparison.Ordinal);
    }

    [Fact]
    public async Task MoreThanTheFileLimit_DoesNotThrowAndExplainsTheExtraFile()
    {
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = RenderZone(received, maxFiles: 2);

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(
            Files(("a.txt", 10), ("b.txt", 10), ("c.txt", 10))));

        Assert.Single(received);
        Assert.Equal(["a.txt", "b.txt"], received[0].Select(f => f.Name));
        var messages = RejectionsText(cut);
        Assert.Contains("c.txt", messages, StringComparison.Ordinal);
        Assert.Contains("(2)", messages, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AValidSelectionAfterARejectedOne_ClearsTheMessages()
    {
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = RenderZone(received, maxSize: 100);
        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("huge.bin", 5000))));
        Assert.NotEmpty(cut.FindAll($"ul.{Css.Classes.FileUpload.Errors}"));

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("small.txt", 10))));

        Assert.Empty(cut.FindAll($"ul.{Css.Classes.FileUpload.Errors}"));
        Assert.Equal(["small.txt"], received[^1].Select(f => f.Name));
    }

    [Fact]
    public async Task TheMessages_AreLocalized()
    {
        using var scope = new CultureScope("ru");
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = RenderZone(received, maxSize: 100);

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("huge.bin", 5000))));

        var messages = RejectionsText(cut);
        Assert.Contains("не добавлен", messages, StringComparison.Ordinal);
        Assert.Contains("huge.bin", messages, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TheButton_ReportsTheLimitTheSameWay()
    {
        var received = new List<IReadOnlyList<IBrowserFile>>();
        var cut = Render<ButtonHarness>(p => p
            .Add(x => x.Multiple, true)
            .Add(x => x.MaxFiles, 1)
            .Add(x => x.OnFilesChanged,
                EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>>(this, received.Add)));

        await cut.InvokeAsync(() => cut.Instance.SelectAsync(Files(("a.txt", 10), ("b.txt", 10))));

        Assert.Equal(["a.txt"], received[^1].Select(f => f.Name));
        Assert.Contains("b.txt", RejectionsText(cut), StringComparison.Ordinal);
        Assert.Equal("alert", cut.Find($"ul.{Css.Classes.FileUpload.Errors}").GetAttribute("role"));
    }
}
