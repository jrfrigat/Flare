using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.IDE;

/// <summary>
/// DI registration for the Flare.Components.IDE package.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Retained for source compatibility. IDE components compose the base Flare services registered by
    /// <c>AddFlare()</c> and need no additional services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddFlareIde(this IServiceCollection services)
    {
        return services;
    }
}
