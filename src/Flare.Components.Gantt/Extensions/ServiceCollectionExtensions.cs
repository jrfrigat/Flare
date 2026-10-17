using Flare.Components.Gantt.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Gantt;

/// <summary>DI registration for the Flare.Components.Gantt package.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Gantt package's services (the typed JS interop that measures and scrolls the chart).
    /// Call this in addition to <c>AddFlare()</c> when using <see cref="FlareGantt"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddFlareGantt(this IServiceCollection services)
    {
        services.AddScoped<IGanttJsService, GanttJsService>();
        return services;
    }
}
