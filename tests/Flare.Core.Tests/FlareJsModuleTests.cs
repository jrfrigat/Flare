using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Core.Tests;

public class FlareJsModuleTests
{
    [Fact]
    public async Task ConcurrentCallersShareAnImportAndDisposeItsResultOnce()
    {
        var js = new ModuleRuntime();
        var service = new ProbeService(js);
        var first = service.ImportAsync();
        var second = service.ImportAsync();

        Assert.Same(first, second);
        Assert.Equal(1, js.Imports);

        var disposal = service.DisposeAsync();
        Assert.False(disposal.IsCompleted);
        Assert.Throws<ObjectDisposedException>(() => { _ = service.ImportAsync(); });

        js.Import.SetResult(js);
        await disposal;
        await service.DisposeAsync();
        Assert.Equal(1, js.Disposals);
    }

    [Fact]
    public async Task FailedImportIsRetried()
    {
        var js = new ModuleRuntime();
        await using var service = new ProbeService(js);
        var first = service.ImportAsync();
        js.Import.SetException(new JSException("Import failed."));
        await Assert.ThrowsAsync<JSException>(() => first);

        js.Import = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var retry = service.ImportAsync();
        js.Import.SetResult(js);
        Assert.Same(js, await retry);
        Assert.Equal(2, js.Imports);
    }

    [Fact]
    public async Task CancelingEitherWaitKeepsTheSharedImportAndRuntimeTimeout()
    {
        var js = new ModuleRuntime();
        await using var service = new ProbeService(js);
        using var firstSource = new CancellationTokenSource();
        using var laterSource = new CancellationTokenSource();
        var first = service.WaitForImportAsync(firstSource.Token);
        var later = service.WaitForImportAsync(laterSource.Token);
        var shared = service.ImportAsync();
        var testToken = TestContext.Current.CancellationToken;
        try
        {
            laterSource.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => later.WaitAsync(TimeSpan.FromSeconds(1), testToken));
            firstSource.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => first.WaitAsync(TimeSpan.FromSeconds(1), testToken));
            Assert.False(shared.IsCompleted);
            Assert.Equal(1, js.Imports);
            Assert.Equal(1, js.DefaultImports);
        }
        finally
        {
            js.Import.SetResult(js);
        }

        Assert.Same(js, await shared);
        Assert.Same(shared, service.ImportAsync());
    }

    private sealed class ProbeService(IJSRuntime js) : FlareJsModule(js, "./test.js")
    {
        public Task<IJSObjectReference> ImportAsync() => ModuleAsync();
        public Task<IJSObjectReference> WaitForImportAsync(CancellationToken token) => ModuleAsync(token);
    }

    private sealed class ModuleRuntime : IJSRuntime, IJSObjectReference
    {
        public TaskCompletionSource<IJSObjectReference> Import { get; set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Imports { get; private set; }
        public int DefaultImports { get; private set; }
        public int Disposals { get; private set; }

        public ValueTask<T> InvokeAsync<T>(string identifier, object?[]? args)
        {
            DefaultImports++;
            return InvokeAsync<T>(identifier, CancellationToken.None, args);
        }

        public async ValueTask<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Assert.Equal("import", identifier);
            Imports++;
            return (T)(object)await Import.Task.WaitAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            Disposals++;
            return ValueTask.CompletedTask;
        }
    }
}
