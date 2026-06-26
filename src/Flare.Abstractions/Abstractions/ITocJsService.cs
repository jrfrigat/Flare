using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <c>FlareOnThisPage</c> (table of contents), backed by the global
/// <c>flare-components.js</c> bundle: it discovers headings in a content root, assigns anchor ids and
/// reports which headings are on screen. Inject instead of calling <see cref="IJSRuntime"/> directly.
/// </summary>
public interface ITocJsService
{
    /// <summary>
    /// Scans <paramref name="rootSelector"/> for headings, reports them via <c>SetHeadings</c> and the
    /// on-screen subset via <c>SetActive</c> as the user scrolls.
    /// </summary>
    /// <param name="handle">An opaque key for this TOC's observer registration.</param>
    /// <param name="dotNetRef">Reference whose <c>SetHeadings</c>/<c>SetActive</c> are invoked.</param>
    /// <param name="rootSelector">Selector of the content root to scan; null scans the document body.</param>
    /// <param name="headingSelector">Selector for headings within the root (e.g. "h2, h3").</param>
    /// <param name="scrollRootSelector">Optional scroll container to track; null auto-detects.</param>
    ValueTask InitAsync<T>(string handle, DotNetObjectReference<T> dotNetRef,
        string? rootSelector, string? headingSelector, string? scrollRootSelector) where T : class;

    /// <summary>Detaches the scroll listeners for the TOC registered under <paramref name="handle"/>.</summary>
    ValueTask RemoveAsync(string handle);
}
