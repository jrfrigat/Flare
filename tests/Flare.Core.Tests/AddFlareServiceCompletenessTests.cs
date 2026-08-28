using Flare.Components;
using Flare.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flare.Core.Tests;

/// <summary>
/// <c>AddFlare()</c> must be sufficient on its own: every service a Flare component injects has to be
/// registered by it, so an application never has to guess a registration.
///
/// The assertion is over the service COLLECTION, not a built provider: half of Flare's services need
/// <c>IJSRuntime</c>, which the Blazor host supplies, so resolving them here would fail for a reason
/// unrelated to the contract. Services owned by the host (JS interop, navigation, logging,
/// configuration, HTTP, localization) are exempt.
/// </summary>
public sealed class AddFlareServiceCompletenessTests
{
    // Namespaces whose services the Blazor/ASP.NET host provides, not AddFlare.
    private static readonly string[] HostOwnedNamespacePrefixes =
    [
        "Microsoft.AspNetCore.",
        "Microsoft.JSInterop",
        "Microsoft.Extensions.Logging",
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.Localization",
        "System.Net.Http",
    ];

    [Fact]
    public void EveryServiceAComponentInjectsIsRegisteredByAddFlare()
    {
        var registered = RegisteredServiceTypes();

        var missing = new List<string>();
        foreach (var component in ComponentTypes())
        {
            foreach (var property in InjectedProperties(component))
            {
                var serviceType = property.PropertyType;
                if (IsHostOwned(serviceType) || registered.Contains(serviceType)) continue;
                missing.Add($"{component.FullName}.{property.Name} -> {serviceType.FullName}");
            }
        }

        Assert.True(missing.Count == 0,
            "AddFlare() does not register every service a Flare component injects, so these components " +
            "throw on first render in any app that did not guess the registration. Register them in " +
            $"ServiceCollectionExtensions.AddFlare:\n  {string.Join("\n  ", missing.Distinct().Order())}");
    }

    [Fact]
    public void AddFlare_RegistersTimeProvider()
    {
        Assert.Contains(typeof(TimeProvider), RegisteredServiceTypes());
    }

    [Fact]
    public void AddFlare_DoesNotReplaceAnApplicationsOwnTimeProvider()
    {
        var custom = new FakeTimeProvider();
        var services = new ServiceCollection();
        services.AddSingleton<TimeProvider>(custom);
        services.AddFlare();

        var clocks = services.Where(d => d.ServiceType == typeof(TimeProvider)).ToList();
        Assert.Single(clocks);
        Assert.Same(custom, clocks[0].ImplementationInstance);
    }

    private sealed class FakeTimeProvider : TimeProvider;

    private static HashSet<Type> RegisteredServiceTypes()
    {
        var services = new ServiceCollection();
        services.AddFlare();
        return [.. services.Select(d => d.ServiceType)];
    }

    private static IEnumerable<Type> ComponentTypes() =>
        typeof(FlareComponentBase).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IComponent).IsAssignableFrom(t));

    private static IEnumerable<PropertyInfo> InjectedProperties(Type component) =>
        component
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Where(p => p.GetCustomAttribute<InjectAttribute>() is not null);

    private static bool IsHostOwned(Type serviceType) =>
        serviceType.FullName is { } name
        && HostOwnedNamespacePrefixes.Any(p => name.StartsWith(p, StringComparison.Ordinal));
}
