using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for the <c>FlareColorPicker</c> saturation/lightness canvas: draws the gradient +
/// crosshair and reports pointer picks back to the component. Wraps the shared <c>flare-drag.js</c>
/// module so the component injects a service instead of importing it.
/// </summary>
public interface IColorCanvasJsService : IAsyncDisposable
{
    /// <summary>Initializes the canvas, draws it for the given HSL and wires pointer-drag picking.</summary>
    /// <param name="canvas">The canvas element.</param>
    /// <param name="dotNetRef">Reference used to report picks (<c>OnCanvasPick</c>).</param>
    /// <param name="hue">Initial hue (0-360).</param>
    /// <param name="saturation">Initial saturation (0-100).</param>
    /// <param name="lightness">Initial lightness (0-100).</param>
    ValueTask InitAsync<T>(ElementReference canvas, DotNetObjectReference<T> dotNetRef,
        double hue, double saturation, double lightness) where T : class;

    /// <summary>Redraws the canvas for a new hue, keeping the crosshair position.</summary>
    ValueTask SetHueAsync(ElementReference canvas, double hue);

    /// <summary>Moves the crosshair to a new saturation/lightness without changing the hue.</summary>
    ValueTask SetSatLAsync(ElementReference canvas, double saturation, double lightness);

    /// <summary>Detaches handlers and forgets the canvas state.</summary>
    ValueTask DestroyAsync(ElementReference canvas);
}
