using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="ITocJsService" />
public sealed class TocJsService : FlareJsModule, ITocJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public TocJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-components.js") { }

    /// <inheritdoc />
    public ValueTask InitAsync<T>(string handle, Microsoft.JSInterop.DotNetObjectReference<T> dotNetRef,
        string? rootSelector, string? headingSelector, string? scrollRootSelector) where T : class
        => InvokeVoidAsync("FlareToc.init", handle, dotNetRef, rootSelector, headingSelector, scrollRootSelector);

    /// <inheritdoc />
    public ValueTask RemoveAsync(string handle) => InvokeVoidAsync("FlareToc.dispose", handle);
}
