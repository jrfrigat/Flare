using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="ITocJsService" />
public sealed class TocJsService(IJSRuntime js) : ITocJsService
{
    /// <inheritdoc />
    public ValueTask InitAsync<T>(string handle, Microsoft.JSInterop.DotNetObjectReference<T> dotNetRef,
        string? rootSelector, string? headingSelector, string? scrollRootSelector) where T : class
        => js.InvokeVoidAsync("FlareToc.init", handle, dotNetRef, rootSelector, headingSelector, scrollRootSelector);

    /// <inheritdoc />
    public ValueTask RemoveAsync(string handle) => js.InvokeVoidAsync("FlareToc.dispose", handle);
}
