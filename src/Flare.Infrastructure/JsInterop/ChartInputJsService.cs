using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <inheritdoc cref="IChartInputJsService" />
public sealed class ChartInputJsService : FlareJsModule, IChartInputJsService
{
    /// <param name="js">The browser runtime.</param>
    public ChartInputJsService(IJSRuntime js) : base(js, "./_content/Flare.Components/js/flare-chart-input.js") { }

    /// <inheritdoc />
    public ValueTask ObservePlotAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
        ElementReference plot, DotNetObjectReference<T> receiver) where T : class
        => InvokeVoidAsync("observePlot", plot, receiver);

    /// <inheritdoc />
    public ValueTask UnobservePlotAsync(ElementReference plot) => InvokeVoidAsync("unobservePlot", plot);
}
