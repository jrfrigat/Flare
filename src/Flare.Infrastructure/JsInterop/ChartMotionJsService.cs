using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IChartMotionJsService" />
public sealed class ChartMotionJsService : FlareJsModule, IChartMotionJsService
{
    /// <param name="js">The JS runtime (injected).</param>
    public ChartMotionJsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-chart-motion.js") { }

    /// <inheritdoc />
    public ValueTask ObservePlotAsync(ElementReference plot)
        => InvokeVoidAsync("observePlot", plot);

    /// <inheritdoc />
    public ValueTask UnobservePlotAsync(ElementReference plot)
        => InvokeVoidAsync("unobservePlot", plot);
}
