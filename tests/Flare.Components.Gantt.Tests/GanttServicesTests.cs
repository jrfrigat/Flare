using System.Reflection;
using Flare.Components.Gantt;
using Flare.Components.Gantt.Services;
using Flare.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Gantt.Tests;

/// <summary>
/// <c>AddFlare()</c> plus <c>AddFlareGantt()</c> must be all an application registers: every service a Gantt
/// component injects comes from one of them, and no component reaches the browser around a port.
/// </summary>
public sealed class GanttServicesTests
{
    private static IEnumerable<PropertyInfo> Injected() =>
        typeof(FlareGantt).Assembly.GetTypes()
            .Where(t => typeof(IComponent).IsAssignableFrom(t) && !t.IsAbstract)
            .SelectMany(t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(p => p.GetCustomAttribute<InjectAttribute>() is not null);

    [Fact]
    public void NoGanttComponentInjectsTheJsRuntime()
    {
        var direct = Injected().Where(p => p.PropertyType == typeof(IJSRuntime)).Select(p => $"{p.DeclaringType!.Name}.{p.Name}");

        Assert.Empty(direct);
    }

    [Fact]
    public void AddFlareAndAddFlareGanttRegisterEveryServiceAGanttComponentInjects()
    {
        var services = new ServiceCollection().AddFlare().AddFlareGantt();
        var registered = services.Select(d => d.ServiceType).ToHashSet();

        var missing = Injected()
            .Where(p => !p.PropertyType.Namespace!.StartsWith("Microsoft.", StringComparison.Ordinal))
            .Where(p => !registered.Contains(p.PropertyType))
            .Select(p => $"{p.DeclaringType!.Name}.{p.Name} -> {p.PropertyType.Name}");

        Assert.Empty(missing);
        Assert.Contains(typeof(IGanttJsService), registered);
    }
}
