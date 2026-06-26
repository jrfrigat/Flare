using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="ITreeJsService" />
public sealed class TreeJsService : FlareJsModule, ITreeJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public TreeJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-drag.js") { }

    /// <inheritdoc />
    public ValueTask<string> GetDropZoneAsync(ElementReference row, double clientY)
        => InvokeAsync<string>("getDropZone", row, clientY);
}
