using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Base class for Flare's typed JS-interop services. Lazily imports a single ES module on first use,
/// caches the reference, and exposes typed invoke helpers - so components inject a strongly-typed
/// service instead of hand-rolling <see cref="IJSRuntime"/> imports and string module/method paths.
/// Register derived services in DI (see AddFlare) and inject them like any other service.
/// </summary>
public abstract class FlareJsModule : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private readonly string _modulePath;
    private Task<IJSObjectReference>? _module;

    /// <param name="js">The JS runtime (injected).</param>
    /// <param name="modulePath">Static-web-asset path to the ES module, e.g.
    /// <c>./_content/Flare.Components.Media/js/colorextractor.js</c>.</param>
    protected FlareJsModule(IJSRuntime js, string modulePath)
    {
        _js = js;
        _modulePath = modulePath;
    }

    /// <summary>Imports (once) and returns the module reference.</summary>
    protected Task<IJSObjectReference> ModuleAsync() =>
        _module ??= _js.InvokeAsync<IJSObjectReference>("import", _modulePath).AsTask();

    /// <summary>Invokes an exported function and returns its result.</summary>
    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object?[] args) =>
        await (await ModuleAsync()).InvokeAsync<T>(identifier, args);

    /// <summary>Invokes an exported function with no return value.</summary>
    protected async ValueTask InvokeVoidAsync(string identifier, params object?[] args) =>
        await (await ModuleAsync()).InvokeVoidAsync(identifier, args);

    /// <summary>Disposes the cached module reference (best-effort; ignores teardown races). Override to
    /// release additional interop state (e.g. a self <see cref="DotNetObjectReference{T}"/>), then call
    /// <c>base.DisposeAsync()</c>.</summary>
    public virtual async ValueTask DisposeAsync()
    {
        if (_module is null) return;
        try
        {
            var module = await _module;
            await module.DisposeAsync();
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }
        GC.SuppressFinalize(this);
    }
}
