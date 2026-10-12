using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>Browser focus, pointer tracking and zoom gestures for a chart's SVG plot.</summary>
public interface IChartInputJsService : IAsyncDisposable
{
    /// <summary>Binds focus, activation, pointer tracking and zoom gestures to the plot.</summary>
    /// <typeparam name="T">The component exposing the chart input callbacks.</typeparam>
    /// <param name="plot">The plot containing accessible SVG hit zones.</param>
    /// <param name="receiver">Receives ChartHitFocused(key,x,y), ChartDetailDismissed,
    /// ChartPointerMoved(x,y), ChartZoomGesture(phase,x) and ChartZoomWheel(delta,x).
    /// Focus and pointer coordinates are viewport CSS pixels; zoom positions are SVG viewBox units.</param>
    ValueTask ObservePlotAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
        ElementReference plot, DotNetObjectReference<T> receiver) where T : class;

    /// <summary>Removes listeners and observers from the plot.</summary>
    ValueTask UnobservePlotAsync(ElementReference plot);
}
