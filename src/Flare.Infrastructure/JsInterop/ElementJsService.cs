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
    public ValueTask<ElementBounds> GetBoundsAsync(ElementReference element) =>
        js.InvokeAsync<ElementBounds>("flareGetBounds", element);
}
