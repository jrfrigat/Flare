using Microsoft.AspNetCore.Components;

namespace Flare.Components.Services;

/// <summary>
/// Typed JS-interop for a chart that moves between datasets instead of replacing one with the next.
/// Wraps <c>flare-chart-motion.js</c>, which watches the plot's geometry attributes and walks them
/// from their old values to their new ones.
/// <para>
/// Two calls for the life of a chart, and none per update: the browser already knows what changed, so
/// there is nothing for the update itself to marshal.
/// </para>
/// </summary>
public interface IChartMotionJsService : IAsyncDisposable
{
    /// <summary>Starts watching a plot's geometry.</summary>
    /// <param name="plot">The chart's plot element.</param>
    ValueTask ObservePlotAsync(ElementReference plot);

    /// <summary>Stops watching, abandoning anything still in flight.</summary>
    /// <param name="plot">The chart's plot element.</param>
    ValueTask UnobservePlotAsync(ElementReference plot);
}
