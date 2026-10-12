using Microsoft.AspNetCore.Components;

namespace Flare.Components.Services;

/// <summary>Browser keyboard focus and activation for a chart's accessible SVG hit zones.</summary>
public interface IChartInputJsService : IAsyncDisposable
{
    /// <summary>Binds roving focus and keyboard activation. The receiver implements ChartHitFocused(key,x,y)
    /// and ChartDetailDismissed; coordinates are CSS pixels in the viewport.</summary>
    ValueTask ObservePlotAsync(ElementReference plot, object receiver);

    /// <summary>Removes listeners and observers from the plot.</summary>
    ValueTask UnobservePlotAsync(ElementReference plot);
}
