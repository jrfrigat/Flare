using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IElementJsService" />
public sealed class ElementJsService(IJSRuntime js) : IElementJsService
{
    /// <inheritdoc />
    public ValueTask FocusAndSelectAsync(ElementReference element) =>
        js.InvokeVoidAsync("flareOtp.focus", element);

    /// <inheritdoc />
    public ValueTask SelectAsync(ElementReference element) =>
        js.InvokeVoidAsync("flareField.select", element);

    /// <inheritdoc />
    public ValueTask BlurAsync(ElementReference element) =>
        js.InvokeVoidAsync("flareField.blur", element);

    /// <inheritdoc />
    public ValueTask SelectRangeAsync(ElementReference element, int start, int end) =>
        js.InvokeVoidAsync("flareField.selectRange", element, start, end);

    /// <inheritdoc />
    public ValueTask<ElementBounds> GetBoundsAsync(ElementReference element) =>
        js.InvokeAsync<ElementBounds>("flareGetBounds", element);
}
