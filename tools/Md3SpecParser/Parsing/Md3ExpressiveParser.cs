using Flare.Tools.Md3SpecParser.Configuration;
using Flare.Tools.Md3SpecParser.Parsing.Model;
using System.Text.Json;

namespace Flare.Tools.Md3SpecParser.Parsing;

/// <summary>
/// Converts a Material Design <c>TOKEN_TABLE</c> JSON into the markdown used under
/// <c>docs/spec/&lt;component&gt;/md3-expressive-spec.md</c>.
/// </summary>
/// <remarks>
/// One markdown section is produced per token set. Display groups become <c>##</c>
/// sub-headings (ungrouped tokens render first, directly under the <c>#</c> heading).
/// Theme-dependent sets (where Light and Dark resolve differently) are emitted twice
/// with a <c>(контекст: Default, Light/Dark)</c> suffix; theme-independent sets are
/// emitted once without a suffix. <c>[Deprecated]</c> sets are skipped. Value
/// resolution (context defaults, alias chains, formatting) is handled by
/// <see cref="ValueResolver"/>.
/// </remarks>
public sealed class Md3ExpressiveParser : ISpecParser
{
    private const string TableHeader = "| Display Name | Token Name | Value |";
    private const string TableSeparator = "|--------------|------------|-------|";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <inheritdoc />
    public ParseResult Parse(string json, SpecSource source)
    {
        var system = JsonSerializer.Deserialize<TokenTableRoot>(json, Options)?.System
                     ?? throw new InvalidDataException("Token table JSON has no 'system' object.");

        var resolver = new ValueResolver(system);
        var setsByName = system.TokenSets.ToDictionary(s => s.Name, StringComparer.Ordinal);
        var category = string.Equals(source.SectionOrder, "category", StringComparison.OrdinalIgnoreCase);

        var lines = new List<string>();
        var componentTokens = new List<TokenModel>();
        foreach (var setName in OrderedSetNames(system, category))
        {
            if (!setsByName.TryGetValue(setName, out var set)) continue;
            if (IsDeprecated(set.DisplayName)) continue;

            var tokens = resolver.OrderedTokens(set.Name);
            if (tokens.Count == 0) continue;
            componentTokens.AddRange(tokens);

            var lightDiffersFromDark = tokens.Any(t =>
                resolver.Resolve(t, Theme.Light) != resolver.Resolve(t, Theme.Dark));

            if (lightDiffersFromDark)
            {
                RenderSection(lines, resolver, set, tokens, Theme.Light, " (контекст: Default, Light)");
                RenderSection(lines, resolver, set, tokens, Theme.Dark, " (контекст: Default, Dark)");
            }
            else
            {
                RenderSection(lines, resolver, set, tokens, Theme.Light, suffix: string.Empty);
            }
        }

        if (lines.Count == 0)
            throw new InvalidDataException("No renderable token sets found in JSON.");

        var palette = new PaletteExtractor(system, resolver).Snapshot(componentTokens);
        return new ParseResult(string.Join("\n", lines), palette);
    }

    private static void RenderSection(
        List<string> lines, ValueResolver resolver, TokenSetModel set,
        IReadOnlyList<TokenModel> tokens, Theme theme, string suffix)
    {
        if (lines.Count > 0) lines.Add(string.Empty);
        lines.Add("# " + set.DisplayName + suffix);

        string? currentGroup = null;
        var rowOpen = false;
        foreach (var token in tokens)
        {
            var group = resolver.GroupNameOf(token);
            if (!rowOpen || group != currentGroup)
            {
                if (group is null)
                {
                    lines.Add(string.Empty);
                }
                else
                {
                    lines.Add(string.Empty);
                    lines.Add("## " + group);
                    lines.Add(string.Empty);
                }
                lines.Add(TableHeader);
                lines.Add(TableSeparator);
                currentGroup = group;
                rowOpen = true;
            }

            lines.Add($"| {token.DisplayName} | `{token.TokenName}` | {resolver.Resolve(token, theme)} |");
        }
    }

    /// <summary>
    /// Yields token-set resource names in render order. Component ordering is
    /// authoritative; unreferenced sets are appended. With <paramref name="category"/>
    /// set, the result is reordered: Color sets first, then Size sets ascending, then
    /// the rest (preserving relative order within each bucket).
    /// </summary>
    private static IEnumerable<string> OrderedSetNames(SystemModel system, bool category)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var ordered = new List<string>();

        foreach (var component in system.Components)
            foreach (var setName in component.TokenSets)
                if (seen.Add(setName))
                    ordered.Add(setName);

        foreach (var set in system.TokenSets.OrderBy(s => s.Order))
            if (seen.Add(set.Name))
                ordered.Add(set.Name);

        if (!category) return ordered;

        var displayByName = system.TokenSets.ToDictionary(s => s.Name, s => s.DisplayName, StringComparer.Ordinal);
        string Display(string n) => displayByName.TryGetValue(n, out var d) ? d : string.Empty;

        return ordered
            .Select((name, index) => (name, index))
            .OrderBy(x => CategoryRank(Display(x.name)))
            .ThenBy(x => SizeRank(Display(x.name)))
            .ThenBy(x => x.index)
            .Select(x => x.name)
            .ToList();
    }

    private static int CategoryRank(string displayName)
    {
        if (displayName.Contains(" - Color - ", StringComparison.OrdinalIgnoreCase)) return 0;
        if (displayName.Contains(" - Size - ", StringComparison.OrdinalIgnoreCase)) return 1;
        return 2;
    }

    private static int SizeRank(string displayName)
    {
        var i = displayName.LastIndexOf(" - ", StringComparison.Ordinal);
        var size = (i >= 0 ? displayName[(i + 3)..] : displayName).Trim().ToLowerInvariant();
        return size switch
        {
            "xsmall" => 0,
            "small" => 1,
            "medium" => 2,
            "large" => 3,
            "xlarge" => 4,
            _ => 99,
        };
    }

    private static bool IsDeprecated(string displayName)
        => displayName.TrimStart().StartsWith("[Deprecated]", StringComparison.OrdinalIgnoreCase);
}
