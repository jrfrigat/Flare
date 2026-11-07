using Flare.Components.Gantt;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler;

/// <summary>Registers the existing time-axis services used by scheduler timeline projections.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds Gantt's shared timeline services; call after AddFlare. Scheduler has no separate JS engine.</summary>
    public static IServiceCollection AddFlareScheduler(this IServiceCollection services) => services.AddFlareGantt();
}
