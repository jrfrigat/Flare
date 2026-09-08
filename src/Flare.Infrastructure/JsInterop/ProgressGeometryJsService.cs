using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IProgressGeometryJsService" />
public sealed class ProgressGeometryJsService : FlareJsModule, IProgressGeometryJsService
{
    /// <summary>Creates the browser adapter for circular progress geometry.</summary>
    /// <param name="js">The JS runtime.</param>
    public ProgressGeometryJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-progress-geometry.js") { }

    /// <inheritdoc />
    public ValueTask ObserveAsync(ElementReference element) => InvokeVoidAsync("observe", element);

    /// <inheritdoc />
    public ValueTask UnobserveAsync(ElementReference element) => InvokeVoidAsync("unobserve", element);
}
