using Microsoft.AspNetCore.Components;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <c>FlareCodeBlock</c>, backed by <c>flare-highlight.js</c>: a dependency-free
/// syntax highlighter plus code-editor key handling (Tab indent and bracket auto-close). Wraps the
/// module so the component injects a service instead of importing it.
/// </summary>
public interface IHighlightJsService : IAsyncDisposable
{
    /// <summary>
    /// Returns syntax-highlighted HTML for <paramref name="code"/> in <paramref name="language"/>, or
    /// null when highlighting is unavailable (the component then keeps its plain-text encoding).
    /// </summary>
    ValueTask<string?> HighlightAsync(string code, string? language);

    /// <summary>Enables Tab-indent and bracket auto-close key handling on a textarea.</summary>
    /// <param name="textarea">The editable textarea element.</param>
    /// <param name="indentSize">Number of spaces inserted per Tab.</param>
    ValueTask EnableEditorKeysAsync(ElementReference textarea, int indentSize);

    /// <summary>Removes the editor key handling from a textarea.</summary>
    ValueTask DisableEditorKeysAsync(ElementReference textarea);
}
