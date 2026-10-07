using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class PickerOverlayInteropTests
{
    [Theory]
    [InlineData("positionAnchoredPanel")]
    [InlineData("positionAnchoredPanelById")]
    [InlineData("trapFocus")]
    public async Task InProcessModule_InvokesSynchronously(string operation)
    {
        var module = new ImmediateModule();
        await using var service = new OverlayJsService(new ImportRuntime(Task.FromResult<IJSObjectReference>(module)));
        var call = Invoke(service, operation);
        Assert.True(call.IsCompletedSuccessfully);
        await call;
        Assert.Equal(operation, Assert.Single(module.ImmediateCalls));
        Assert.Empty(module.AsyncCalls);
    }

    [Theory]
    [InlineData("positionAnchoredPanel")]
    [InlineData("positionAnchoredPanelById")]
    [InlineData("trapFocus")]
    public async Task RemoteModule_AwaitsTheOperation(string operation)
    {
        var module = new RemoteModule();
        await using var service = new OverlayJsService(new ImportRuntime(Task.FromResult<IJSObjectReference>(module)));
        var call = Invoke(service, operation);
        Assert.False(call.IsCompleted);
        Assert.Equal(operation, Assert.Single(module.AsyncCalls));
        module.Completion.SetResult();
        await call;
    }

    [Fact]
    public async Task ConcurrentCalls_WaitForTheSameImport()
    {
        var imported = new TaskCompletionSource<IJSObjectReference>();
        var runtime = new ImportRuntime(imported.Task);
        await using var service = new OverlayJsService(runtime);
        var placement = Invoke(service, "positionAnchoredPanel");
        var focus = Invoke(service, "trapFocus");
        Assert.False(placement.IsCompleted);
        Assert.False(focus.IsCompleted);
        Assert.Equal(1, runtime.Imports);
        var module = new ImmediateModule();
        imported.SetResult(module);
        await placement;
        await focus;
        Assert.Equal(2, module.ImmediateCalls.Count);
        Assert.Empty(module.AsyncCalls);
    }

    [Fact]
    public async Task FailedImmediateCall_DoesNotRepeatTheOperation()
    {
        var module = new ImmediateModule { Failure = new JSException("Panel was detached") };
        await using var service = new OverlayJsService(new ImportRuntime(Task.FromResult<IJSObjectReference>(module)));
        await Assert.ThrowsAsync<JSException>(() => service.TrapFocusAsync("trap", default).AsTask());
        Assert.Single(module.ImmediateCalls);
        Assert.Empty(module.AsyncCalls);
    }

    private static ValueTask Invoke(OverlayJsService service, string operation) => operation switch
    {
        "trapFocus" => service.TrapFocusAsync("trap", default),
        "positionAnchoredPanelById" => service.PositionAnchoredPanelByIdAsync("panel", "anchor", default),
        _ => service.PositionAnchoredPanelAsync("panel", default, default),
    };

    private sealed class ImportRuntime(Task<IJSObjectReference> imported) : IJSRuntime
    {
        public int Imports { get; private set; }
        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);
        public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Assert.Equal("import", identifier);
            Imports++;
            return (TValue)(object)await imported;
        }
    }

    private class RemoteModule : IJSObjectReference
    {
        public ConcurrentQueue<string> AsyncCalls { get; } = new();
        public TaskCompletionSource Completion { get; } = new();
        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);
        public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            AsyncCalls.Enqueue(identifier);
            await Completion.Task;
            return default!;
        }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class ImmediateModule : RemoteModule, IJSInProcessObjectReference
    {
        public ConcurrentQueue<string> ImmediateCalls { get; } = new();
        public JSException? Failure { get; init; }
        public TValue Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, params object?[]? args)
        {
            ImmediateCalls.Enqueue(identifier);
            if (Failure is not null) throw Failure;
            return default!;
        }
        public void Dispose() { }
    }
}

internal sealed class RemoteOverlayRuntime(IJSRuntime runtime) : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
        => InvokeAsync<TValue>(identifier, default, args);
    public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        var reference = await runtime.InvokeAsync<IJSObjectReference>(identifier, cancellationToken, args);
        return (TValue)(object)new RemoteReference(reference);
    }

    private sealed class RemoteReference(IJSObjectReference reference) : IJSObjectReference
    {
        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
            => reference.InvokeAsync<TValue>(identifier, args);
        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => reference.InvokeAsync<TValue>(identifier, cancellationToken, args);
        public ValueTask DisposeAsync() => reference.DisposeAsync();
    }
}
