using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IElementJsService" />
public sealed class ElementJsService : FlareJsModule, IElementJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public ElementJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-components.js") { }

    /// <inheritdoc />
    public ValueTask FocusAndSelectAsync(ElementReference element) =>
        InvokeVoidAsync("flareOtp.focus", element);

    /// <inheritdoc />
    public ValueTask SelectAsync(ElementReference element) =>
        InvokeVoidAsync("flareField.select", element);

    /// <inheritdoc />
    public ValueTask BlurAsync(ElementReference element) =>
        InvokeVoidAsync("flareField.blur", element);

    /// <inheritdoc />
    public ValueTask SelectRangeAsync(ElementReference element, int start, int end) =>
        InvokeVoidAsync("flareField.selectRange", element, start, end);

    /// <inheritdoc />
    public ValueTask<int[]?> GetSelectionAsync(ElementReference element) =>
        InvokeAsync<int[]?>("flareField.selection", element);

    /// <inheritdoc />
    public ValueTask SetValueAndCaretAsync(ElementReference element, string value, int start, int end) =>
        InvokeVoidAsync("flareField.setValueAndCaret", element, value, start, end);

    /// <inheritdoc />
    public ValueTask<ElementBounds> GetBoundsAsync(ElementReference element) =>
        InvokeAsync<ElementBounds>("flareGetBounds", element);

    /// <inheritdoc />
    public ValueTask SyncTimeColumnsAsync(ElementReference element, object receiver, int generation) =>
        InvokeVoidAsync("flareTimeColumns.sync", element, receiver, generation);

    /// <inheritdoc />
    public ValueTask ReleaseTimeColumnsAsync(ElementReference element) =>
        InvokeVoidAsync("flareTimeColumns.release", element);
}
