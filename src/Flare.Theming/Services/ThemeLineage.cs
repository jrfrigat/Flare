using System.Runtime.CompilerServices;
using Flare.Abstractions;

namespace Flare.Theming;

/// <summary>
/// Resolves the chain a theme is built on - the theme, its <see cref="ITheme.Base"/>, that theme's base,
/// and so on - and the root classes that chain produces. Every theme stylesheet is scoped to the class
/// of its own theme id, so a root that carries one class per generation is styled by the CSS of every
/// ancestor. Results are computed once per theme instance and cached, so a render asks for them for free.
/// </summary>
public static class ThemeLineage
{
    private sealed record Resolved(IReadOnlyList<string> Ids, string RootClasses);

    private static readonly ConditionalWeakTable<ITheme, Resolved> s_cache = new();

    /// <summary>
    /// Ids of the theme and its ancestors, the theme itself first and the root of the chain last. An id
    /// that appears twice in the chain is listed once, at its first position.
    /// </summary>
    /// <param name="theme">The theme whose chain to resolve.</param>
    /// <returns>The chain of ids, never empty.</returns>
    /// <exception cref="InvalidOperationException">The chain of <see cref="ITheme.Base"/> references loops back on itself.</exception>
    public static IReadOnlyList<string> Ids(ITheme theme) => Resolve(theme).Ids;

    /// <summary>
    /// The theme classes a root element carries for this theme: <c>flare-theme-{id}</c> for the theme and
    /// for each ancestor, the theme's own class first, separated by spaces. The same string instance is
    /// returned on every call for the same theme instance.
    /// </summary>
    /// <param name="theme">The theme whose root classes to build.</param>
    /// <returns>The space-separated class list.</returns>
    /// <exception cref="InvalidOperationException">The chain of <see cref="ITheme.Base"/> references loops back on itself.</exception>
    public static string RootClasses(ITheme theme) => Resolve(theme).RootClasses;

    /// <summary>The inherited assets followed by the theme's own, each listed once at its first position.</summary>
    internal static string[] Compose(IEnumerable<string> inherited, IEnumerable<string> own) =>
        inherited.Concat(own).Distinct(StringComparer.Ordinal).ToArray();

    private static Resolved Resolve(ITheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        return s_cache.GetValue(theme, static t =>
        {
            var chain = new List<ITheme>();
            for (var current = t; current is not null; current = current.Base)
            {
                if (chain.Exists(seen => ReferenceEquals(seen, current)))
                    throw new InvalidOperationException(
                        $"Theme '{t.Id}' is built on itself: {string.Join(" -> ", chain.Select(c => c.Id))} -> {current.Id}.");
                chain.Add(current);
            }

            var ids = chain.Select(c => c.Id).Distinct(StringComparer.Ordinal).ToArray();
            return new Resolved(ids, string.Join(' ', ids.Select(Css.Classes.Theme.ForTheme)));
        });
    }
}
