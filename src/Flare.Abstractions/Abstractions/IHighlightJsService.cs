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

    /// <summary>
    /// Reads where the caret is, both as an offset and as a position inside the textarea's own box,
    /// so a suggestion list can be put beside it. Null when the element is not there.
    /// </summary>
    /// <param name="textarea">The editable textarea element.</param>
    ValueTask<FlareCaretInfo?> GetCaretAsync(ElementReference textarea);

    /// <summary>Puts the caret at an offset and focuses the textarea, after text was replaced.</summary>
    /// <param name="textarea">The editable textarea element.</param>
    /// <param name="offset">Zero-based offset to place the caret at.</param>
    ValueTask SetCaretAsync(ElementReference textarea, int offset);
}

/// <summary>
/// Where the caret is. The offsets say which characters it sits between; the coordinates say where
/// that is on screen, measured from the textarea's own top left corner.
/// </summary>
/// <param name="Start">Zero-based offset the selection starts at.</param>
/// <param name="End">Zero-based offset the selection ends at. Equal to Start when nothing is selected.</param>
/// <param name="Left">Distance from the textarea's left edge, in pixels.</param>
/// <param name="Top">Distance from the textarea's top edge, in pixels.</param>
/// <param name="LineHeight">Height of one line, so a caller can place something below the caret.</param>
public sealed record FlareCaretInfo(int Start, int End, double Left, double Top, double LineHeight);
