using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

public sealed class FileUploadLifetimeTests : FlareTestContext
{
    public sealed class Zone : FlareFileUploadZone
    {
        public Task SelectAsync(params IBrowserFile[] files) => HandleChangeAsync(new(files));
    }

    private sealed class FileItem(string name) : IBrowserFile
    {
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public long Size => 1;
        public string ContentType => "text/plain";
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default) => new MemoryStream([1]);
    }

    [Fact]
    public async Task DisposalCancelsTheActiveTransferWithoutStartingWaitingFiles()
    {
        var started = new List<string>();
        var active = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var completed = 0;
        async Task Upload(FlareUploadContext context)
        {
            started.Add(context.File.Name);
            if (context.File.Name == "first")
            {
                active.TrySetResult();
                await Task.Delay(Timeout.Infinite, context.CancellationToken);
            }
        }
        var cut = Render<Zone>(p => p.Add(x => x.Auto, true).Add(x => x.Uploader, Upload)
            .Add(x => x.OnAllCompleted, () => completed++));
        Task? running = null;
        await cut.InvokeAsync(() => { running = cut.Instance.SelectAsync(new FileItem("first"), new FileItem("waiting")); });
        await active.Task.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        await running!.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(new[] { "first" }, started);
        Assert.Equal(0, completed);
    }
}
