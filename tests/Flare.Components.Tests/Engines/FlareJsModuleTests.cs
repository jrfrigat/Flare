using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareJsModuleTests
{
    private sealed class Probe(IJSRuntime js) : FlareJsModule(js, "./probe.js")
    {
        public Task<IJSObjectReference> Module() => ModuleAsync();
    }

    // Fails the first import, succeeds afterwards - a transient stale-module or dropped-circuit failure.
    private sealed class FlakyRuntime(bool failFirst) : IJSRuntime
    {
        public int Calls { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Calls++;
            if (failFirst && Calls == 1) throw new JSException("stale module");
            return ValueTask.FromResult((TValue)(object)new FakeModule());
        }
    }

    private sealed class FakeModule : IJSObjectReference
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => ValueTask.FromResult(default(TValue)!);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            ValueTask.FromResult(default(TValue)!);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [Fact]
    public async Task FailedImportIsRetriedOnTheNextCall()
    {
        var runtime = new FlakyRuntime(failFirst: true);
        var probe = new Probe(runtime);

        await Assert.ThrowsAsync<JSException>(() => probe.Module());
        var module = await probe.Module();

        Assert.IsType<FakeModule>(module);
        Assert.Equal(2, runtime.Calls);
    }

    [Fact]
    public async Task SuccessfulImportIsCached()
    {
        var runtime = new FlakyRuntime(failFirst: false);
        var probe = new Probe(runtime);

        var first = await probe.Module();
        var second = await probe.Module();

        Assert.Same(first, second);
        Assert.Equal(1, runtime.Calls);
    }
}
