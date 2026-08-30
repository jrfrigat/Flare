using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Flare.Core.Tests;

/// <summary>
/// A PascalCase attribute on a Flare component must be one of its parameters. Nearly every component
/// takes <c>CaptureUnmatchedValues</c>, so anything else compiles, renders as a literal HTML attribute,
/// and does nothing - no error, no warning, no failing test. That is how <c>Clickable="true"</c> sat on a
/// list item that has no such parameter, and it is the same silent-swallow shape as the slot-name
/// collision <see cref="SlotNameTests"/> guards.
///
/// Lowercase attributes are left alone: those are real HTML (<c>href</c>, <c>type</c>, <c>colspan</c>),
/// which is exactly what the catch-all is for. Only the PascalCase spelling is unambiguously a
/// parameter that was meant and missed.
/// </summary>
public sealed class ComponentAttributeTests
{
    /// <summary>
    /// The one PascalCase name Razor itself owns: it renames the implicit variable a child fragment
    /// receives, and never reaches the component as a parameter.
    /// </summary>
    private static readonly HashSet<string> RazorOwned = new(StringComparer.Ordinal) { "Context" };

    [Fact]
    public void NoRazorMarkupPassesAnUndeclaredParameterToAFlareComponent()
    {
        var parameters = ParametersByComponentName();
        Assert.NotEmpty(parameters);

        var root = FindRepoRoot();
        var offenders = new List<string>();

        foreach (var dir in new[]
                 {
                     Path.Combine(root, "src", "Flare.Components"),
                     Path.Combine(root, "samples", "Flare.Gallery"),
                 })
        foreach (var file in Directory.EnumerateFiles(dir, "*.razor", SearchOption.AllDirectories))
        {
            if (IsBuildArtifact(file)) continue;

            foreach (var (component, attribute, line) in AttributesInMarkup(File.ReadAllText(file)))
            {
                if (!parameters.TryGetValue(component, out var declared)) continue;
                if (declared.Contains(attribute) || RazorOwned.Contains(attribute)) continue;
                offenders.Add($"{Path.GetRelativePath(root, file)}:{line}  <{component} {attribute}=...>");
            }
        }

        Assert.True(offenders.Count == 0,
            "Attributes that are not parameters of the component they sit on (they land in "
            + "AdditionalAttributes and do nothing):\n  " + string.Join("\n  ", offenders));
    }

    /// <summary>
    /// Maps each public Flare component's simple name to every name it accepts in markup: its
    /// <see cref="ParameterAttribute"/> properties (including inherited ones) plus, for a generic
    /// component, its type parameters - <c>TValue="string"</c> is a type argument, not a parameter.
    /// </summary>
    private static Dictionary<string, HashSet<string>> ParametersByComponentName()
    {
        var map = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var assembly in new[] { typeof(Flare.Components.FlareChip).Assembly })
        foreach (var type in assembly.GetExportedTypes())
        {
            if (!typeof(IComponent).IsAssignableFrom(type) || type.IsAbstract) continue;

            var names = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null
                            || p.GetCustomAttribute<CascadingParameterAttribute>() is not null)
                .Select(p => p.Name)
                .ToHashSet(StringComparer.Ordinal);

            foreach (var arg in type.GetGenericArguments()) names.Add(arg.Name);

            // Razor writes a generic component's name without the arity suffix.
            var simple = type.Name.Split('`')[0];
            if (map.TryGetValue(simple, out var existing)) existing.UnionWith(names);
            else map[simple] = names;
        }

        return map;
    }

    /// <summary>
    /// Yields (component, attribute, line) for every PascalCase attribute written on a Flare component
    /// tag. Directives (<c>@bind-…</c>, <c>@ref</c>, <c>@onclick</c>) and lowercase HTML attributes are
    /// skipped; an attribute value is consumed as a unit so a "&gt;" inside <c>@(...)</c> or a string
    /// cannot end the tag early.
    /// </summary>
    private static IEnumerable<(string Component, string Attribute, int Line)> AttributesInMarkup(string markup)
    {
        foreach (Match tag in Regex.Matches(markup, @"<(Flare[A-Za-z0-9_]*)(?=[\s/>])"))
        {
            var component = tag.Groups[1].Value;
            var i = tag.Index + tag.Length;
            var depth = 0;

            while (i < markup.Length)
            {
                var c = markup[i];

                if (c == '"' || c == '\'') { i = SkipQuoted(markup, i); continue; }
                if (c == '(') { depth++; i++; continue; }
                if (c == ')') { depth--; i++; continue; }
                if (depth > 0) { i++; continue; }
                if (c == '>') break;
                if (c == '/' && i + 1 < markup.Length && markup[i + 1] == '>') break;

                var name = Regex.Match(markup[i..], @"^([A-Za-z_@][\w\-.:]*)\s*=");
                if (!name.Success) { i++; continue; }

                var attribute = name.Groups[1].Value;
                if (char.IsUpper(attribute[0]) && !attribute.Contains('-') && !attribute.Contains('.'))
                    yield return (component, attribute, LineOf(markup, i));

                i += name.Length;
            }
        }
    }

    private static int SkipQuoted(string s, int i)
    {
        var quote = s[i];
        var depth = 0;
        for (i++; i < s.Length; i++)
        {
            // A Razor expression inside an attribute value may carry the same quote character, so the
            // value ends at the quote that closes it at parenthesis depth zero.
            if (s[i] == '(') depth++;
            else if (s[i] == ')') depth--;
            else if (s[i] == quote && depth <= 0) return i + 1;
        }

        return s.Length;
    }

    private static int LineOf(string s, int index) => s.Take(index).Count(c => c == '\n') + 1;

    private static bool IsBuildArtifact(string path) =>
        path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
        || path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal);

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from " + AppContext.BaseDirectory);
    }
}
