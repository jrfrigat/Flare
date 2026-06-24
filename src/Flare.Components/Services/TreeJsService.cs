using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for <see cref="FlareTreeItem"/>: resolves which drop zone (before, inside, after)
/// the cursor is over for a tree row, since <see cref="Microsoft.AspNetCore.Components.Web.DragEventArgs"/>
/// reports the cursor position but not the target element's bounds. Wraps the shared <c>flare-theme.js</c>
/// module so the component injects a service instead of calling <see cref="IJSRuntime"/> directly.
/// </summary>
public interface ITreeJsService : IAsyncDisposable
{
    /// <summary>Returns the drop zone for the cursor over a tree row.</summary>
    /// <param name="row">The tree row element to measure against.</param>
    /// <param name="clientY">The cursor's vertical client coordinate (from the drag event).</param>
    /// <returns><c>"before"</c>, <c>"inside"</c>, or <c>"after"</c>.</returns>
    ValueTask<string> GetDropZoneAsync(ElementReference row, double clientY);
}

/// <inheritdoc cref="ITreeJsService" />
public sealed class TreeJsService : FlareJsModule, ITreeJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public TreeJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-theme.js") { }

    /// <inheritdoc />
    public ValueTask<string> GetDropZoneAsync(ElementReference row, double clientY)
        => InvokeAsync<string>("getDropZone", row, clientY);
}
