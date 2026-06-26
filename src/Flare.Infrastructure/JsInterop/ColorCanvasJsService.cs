using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IColorCanvasJsService" />
public sealed class ColorCanvasJsService : FlareJsModule, IColorCanvasJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public ColorCanvasJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-drag.js") { }

    /// <inheritdoc />
    public ValueTask InitAsync<T>(ElementReference canvas, DotNetObjectReference<T> dotNetRef,
        double hue, double saturation, double lightness) where T : class
        => InvokeVoidAsync("flareColorPicker.init", canvas, dotNetRef, hue, saturation, lightness);

    /// <inheritdoc />
    public ValueTask SetHueAsync(ElementReference canvas, double hue)
        => InvokeVoidAsync("flareColorPicker.setHue", canvas, hue);

    /// <inheritdoc />
    public ValueTask SetSatLAsync(ElementReference canvas, double saturation, double lightness)
        => InvokeVoidAsync("flareColorPicker.setSatL", canvas, saturation, lightness);

    /// <inheritdoc />
    public ValueTask DestroyAsync(ElementReference canvas)
        => InvokeVoidAsync("flareColorPicker.destroy", canvas);
}
