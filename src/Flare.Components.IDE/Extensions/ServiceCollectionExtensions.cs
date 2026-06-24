using Flare.Components.IDE.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.IDE;

/// <summary>
/// DI registration for the Flare.Components.IDE package.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the IDE package's services (the typed JS-interop wrapper used for region resizing).
    /// Call this in addition to <c>AddFlare()</c> when using <see cref="FlareIdeLayout"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddFlareIde(this IServiceCollection services)
    {
        services.AddScoped<IIdeJsService, IdeJsService>();
        return services;
    }
}
