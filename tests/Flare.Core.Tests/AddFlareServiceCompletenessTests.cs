using Flare.Components;
using Flare.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flare.Core.Tests;

/// <summary>
/// <c>AddFlare()</c> must be SUFFICIENT: every service a Flare component injects has to be registered by
/// it, so an application never has to guess a registration Flare depends on.
///
/// This is the guard for the reported defect that <c>AddFlare</c> did not register <see cref="TimeProvider"/>
/// while <c>FlareCalendar</c> and the three date pickers inject it - the Gallery registered it itself, so
/// the sample hid the hole and every other host hit "Cannot provide a value for property 'TimeProvider'"
/// the first time a picker rendered.
///
/// The assertion is over the service COLLECTION, not a built provider: half of Flare's services need
/// <c>IJSRuntime</c>, which the Blazor host supplies, so resolving them here would fail for a reason that
/// has nothing to do with the contract. Registration is exactly the thing being promised.
///
/// Services owned by the host rather than by Flare (JS interop, navigation, logging, configuration, HTTP,
/// localization) are exempt - those come with the hosting model, and registering them would be Flare
/// overreaching.
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

    // The specific regression, asserted on its own so a failure names the cause rather than a list.
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
