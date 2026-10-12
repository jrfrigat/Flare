using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IChartInputJsService" />
public sealed class ChartInputJsService : FlareJsModule, IChartInputJsService
{
    /// <param name="js">The browser runtime.</param>
    public ChartInputJsService(IJSRuntime js) : base(js, "./_content/Flare.Components/js/flare-chart-input.js") { }

    /// <inheritdoc />
    public ValueTask ObservePlotAsync(ElementReference plot, object receiver) => InvokeVoidAsync("observePlot", plot, receiver);

    /// <inheritdoc />
    public ValueTask UnobservePlotAsync(ElementReference plot) => InvokeVoidAsync("unobservePlot", plot);
}
