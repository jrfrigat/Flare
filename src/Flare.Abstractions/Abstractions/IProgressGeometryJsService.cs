using Microsoft.AspNetCore.Components;

namespace Flare.Components.Services;

/// <summary>Fits circular progress geometry to browser-resolved CSS lengths.</summary>
public interface IProgressGeometryJsService : IAsyncDisposable
{
    /// <summary>Observes size, token and value changes. Animation remains entirely in CSS.</summary>
    /// <param name="element">The circular progress root.</param>
    ValueTask ObserveAsync(ElementReference element);

    /// <summary>Disconnects geometry observers for a circular progress root.</summary>
    /// <param name="element">The previously observed root.</param>
    ValueTask UnobserveAsync(ElementReference element);
}
