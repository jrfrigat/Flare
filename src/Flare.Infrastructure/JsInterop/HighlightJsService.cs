using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IHighlightJsService" />
public sealed class HighlightJsService : FlareJsModule, IHighlightJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public HighlightJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-highlight.js") { }

    /// <inheritdoc />
    public ValueTask<string?> HighlightAsync(string code, string? language)
        => InvokeAsync<string?>("getHighlightedHtml", code, language);

    /// <inheritdoc />
    public ValueTask EnableEditorKeysAsync(ElementReference textarea, int indentSize)
        => InvokeVoidAsync("enableCodeEditorKeys", textarea, indentSize);

    /// <inheritdoc />
    public ValueTask DisableEditorKeysAsync(ElementReference textarea)
        => InvokeVoidAsync("disableCodeEditorKeys", textarea);

    /// <inheritdoc />
    public ValueTask<FlareCaretInfo?> GetCaretAsync(ElementReference textarea)
        => InvokeAsync<FlareCaretInfo?>("getCaretInfo", textarea);

    /// <inheritdoc />
    public ValueTask SetCaretAsync(ElementReference textarea, int offset)
        => InvokeVoidAsync("setCaret", textarea, offset);

    /// <inheritdoc />
    public ValueTask SetTextAsync(ElementReference textarea, string text)
        => InvokeVoidAsync("setText", textarea, text);

    /// <inheritdoc />
    public ValueTask AttachEditorSyncAsync(ElementReference textarea)
        => InvokeVoidAsync("attachEditorSync", textarea);

    /// <inheritdoc />
    public ValueTask SyncEditorScrollAsync(ElementReference textarea)
        => InvokeVoidAsync("syncEditorScroll", textarea);

    /// <inheritdoc />
    public ValueTask DetachEditorSyncAsync(ElementReference textarea)
        => InvokeVoidAsync("detachEditorSync", textarea);
}
