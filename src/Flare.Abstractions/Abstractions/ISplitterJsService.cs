using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for the splitter component: registers a sibling resize handle, applies keyboard
/// nudges, and detaches the handle. Wraps the shared JS module so the component injects a service
/// instead of calling <see cref="IJSRuntime"/> directly. The register/nudge methods are generic over
/// the component type exposing the <c>[JSInvokable]</c> size callback, so the contract stays free of
/// any UI component reference.
/// </summary>
public interface ISplitterJsService : IAsyncDisposable
{
    /// <summary>Attaches a drag handler to the splitter handle that resizes its two flex siblings.</summary>
    /// <param name="gutter">The splitter handle element.</param>
    /// <param name="axis">"horizontal", "vertical", or "auto" (detect from the parent flex direction).</param>
    /// <param name="minSize">Optional minimum neighbour size (e.g. "120px").</param>
    /// <param name="maxSize">Optional maximum size for the previous neighbour (e.g. "600px").</param>
    /// <param name="dotNetRef">Reference used to report the new size back to the component.</param>
    ValueTask RegisterAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(ElementReference gutter, string axis, string? minSize, string? maxSize,
        DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Applies a keyboard nudge (px) along the resolved axis.</summary>
    /// <param name="gutter">The splitter handle element.</param>
    /// <param name="axis">"horizontal", "vertical", or "auto".</param>
    /// <param name="deltaPx">Signed pixel delta to apply to the previous neighbour.</param>
    /// <param name="keyAxis">The arrow key's axis ("x" or "y"); nudges perpendicular to the resize axis are ignored.</param>
    /// <param name="minSize">Optional minimum neighbour size.</param>
    /// <param name="maxSize">Optional maximum size for the previous neighbour.</param>
    /// <param name="dotNetRef">Reference used to report the new size back to the component.</param>
    ValueTask NudgeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(ElementReference gutter, string axis, int deltaPx, string keyAxis,
        string? minSize, string? maxSize, DotNetObjectReference<T> dotNetRef) where T : class;

    /// <summary>Detaches the drag handler from the splitter handle.</summary>
    /// <param name="gutter">The splitter handle element.</param>
    ValueTask RemoveAsync(ElementReference gutter);

    /// <summary>Measures the share of the container's axis length taken by one element, so a pixel
    /// report from the splitter can be persisted as a fraction.</summary>
    /// <param name="scope">The container that defines the whole length along the axis.</param>
    /// <param name="target">The element whose share is measured (the first pane).</param>
    /// <param name="axis">"horizontal" for widths, "vertical" for heights.</param>
    /// <returns>The share from 0 to 1, or 0 when the container has no length.</returns>
    ValueTask<double> MeasureFractionAsync(ElementReference scope, ElementReference target, string axis);
}
