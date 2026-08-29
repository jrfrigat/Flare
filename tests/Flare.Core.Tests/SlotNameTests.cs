using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Flare.Core.Tests;

/// <summary>
/// A named content slot must not be spelled the way an application would plausibly name one of its own
/// components.
/// </summary>
/// <remarks>
/// Razor decides "this child element is a named slot" by matching the child's TAG NAME against the
/// component's <see cref="RenderFragment"/> parameters, and a component name is checked first only when
/// no parameter matches. So an application component called <c>Icon</c> written inside a Flare component
/// that has an <c>Icon</c> slot binds to the slot and never renders - with no diagnostic, because both
/// spellings are legal.
///
/// <c>Icon</c> was the sharp case: eight components exposed it as a fragment while eight others exposed
/// <c>Icon</c> as a <see cref="Flare.Components.FlareIcon"/> VALUE, so the same parameter name meant two
/// different things depending on which component you were looking at. Renaming the fragments to
/// <c>IconContent</c> fixed both problems at once: the collision, and the ambiguity.
///
/// The remaining single-word slots are deliberate. <c>Columns</c> and <c>Grouping</c> on the data grid
/// are collection slots that every grid library spells this way, and renaming them would cost every
/// consumer far more than the collision risk is worth; <c>Leading</c>, <c>Trailing</c>, <c>Zones</c>,
/// <c>Composite</c> and <c>Activator</c> are positional or domain terms nobody names a component after.
/// </remarks>
public sealed class SlotNameTests
{
    // Names that read as a UI thing an application would build a component for. A fragment parameter must
    // not be spelled any of these.
    private static readonly string[] Forbidden =
    [
        "Icon", "Header", "Footer", "Avatar", "Badge", "Action", "Actions", "Title", "Label", "Content",
        "Body", "Card", "Menu", "Panel", "Toolbar", "Sidebar", "Placeholder", "Empty", "Counter",
        "Fallback", "Image", "Logo", "Banner", "Chip", "Tag",
    ];

    [Fact]
    public void NoContentSlot_IsNamedLikeAnApplicationComponent()
    {
        var offenders = typeof(Flare.Components.FlareChip).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsPublic: true } && typeof(ComponentBase).IsAssignableFrom(t))
            .SelectMany(t => t
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
                .Where(p => typeof(Delegate).IsAssignableFrom(p.PropertyType) && p.PropertyType.Name.StartsWith("RenderFragment", StringComparison.Ordinal))
                .Where(p => Forbidden.Contains(p.Name, StringComparer.Ordinal))
                .Select(p => $"{Bare(t)}.{p.Name}"))
            .Order(StringComparer.Ordinal)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Assert.True(offenders.Count == 0,
            "A content slot named like an application component silently swallows that component: Razor " +
            "binds the child element to the slot and it never renders, with nothing reported. Rename to " +
            "'XxxContent' - the pattern FlareAppBar.TitleContent and FlareCollapse.HeaderContent already " +
            "use:" + Environment.NewLine + string.Join(Environment.NewLine, offenders));
    }

    private static string Bare(Type t) =>
        t.Name.Contains('`', StringComparison.Ordinal)
            ? t.Name[..t.Name.IndexOf('`', StringComparison.Ordinal)]
            : t.Name;
}
