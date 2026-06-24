using Flare.Tools.Md3SpecParser.Parsing.Model;
using System.Text.RegularExpressions;

namespace Flare.Tools.Md3SpecParser.Parsing;

/// <summary>
/// Extracts color-palette data (semantic roles and reference tonal palettes) from a
/// design system and renders it as markdown.
/// </summary>
public sealed partial class PaletteExtractor
{
    private const string SysColorPrefix = "md.sys.color.";
    private const string RefPalettePrefix = "md.ref.palette.";

    private readonly SystemModel _system;
    private readonly ValueResolver _resolver;

    /// <summary>Creates an extractor over the given system and resolver.</summary>
    public PaletteExtractor(SystemModel system, ValueResolver resolver)
    {
        _system = system;
        _resolver = resolver;
    }

    /// <summary>Builds a full snapshot: used roles, all roles, and reference palettes.</summary>
    public PaletteSnapshot Snapshot(IEnumerable<TokenModel> componentTokens) => new()
    {
        UsedRoles = UsedRoles(componentTokens),
        AllRoles = AllRoles(),
        RefPalettes = RefPalettes(),
    };

    /// <summary>Semantic roles referenced by the given component color tokens.</summary>
    public List<RoleEntry> UsedRoles(IEnumerable<TokenModel> componentTokens)
    {
        var roles = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var t in componentTokens.Where(t => t.TokenValueType == "COLOR"))
        {
            var role = _resolver.ColorRoleOf(t, Theme.Light);
            if (role is not null) roles.Add(role);
        }
        return roles.Select(BuildRole).ToList();
    }

    /// <summary>All semantic color roles present in the design system.</summary>
    public List<RoleEntry> AllRoles() =>
        _system.Tokens
            .Where(t => t.TokenName.StartsWith(SysColorPrefix, StringComparison.Ordinal))
            .Select(t => t.TokenName)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(n => n, StringComparer.Ordinal)
            .Select(BuildRole)
            .ToList();

    /// <summary>Reference tonal palettes grouped by family, tones ascending.</summary>
    public List<PaletteFamily> RefPalettes()
    {
        var families = new Dictionary<string, List<(int Order, ToneEntry Tone)>>(StringComparer.Ordinal);
        var familyOrder = new List<string>();

        foreach (var token in _system.Tokens
                     .Where(t => t.TokenName.StartsWith(RefPalettePrefix, StringComparison.Ordinal)))
        {
            var label = token.TokenName[RefPalettePrefix.Length..];
            var match = ToneRegex().Match(label);
            var family = match.Success ? match.Groups["fam"].Value : label;
            var order = match.Success ? int.Parse(match.Groups["tone"].Value) : 0;
            var hex = _resolver.ResolveColorChain(token.TokenName, Theme.Light).Hex;

            if (!families.TryGetValue(family, out var list))
            {
                families[family] = list = new List<(int, ToneEntry)>();
                familyOrder.Add(family);
            }
            list.Add((order, new ToneEntry(label, hex)));
        }

        return familyOrder
            .Select(f => new PaletteFamily(f,
                families[f].OrderBy(x => x.Order).Select(x => x.Tone).ToList()))
            .ToList();
    }

    private RoleEntry BuildRole(string role)
    {
        var light = _resolver.ResolveColorChain(role, Theme.Light);
        var dark = _resolver.ResolveColorChain(role, Theme.Dark);
        return new RoleEntry(role, light.Tone, light.Hex, dark.Tone, dark.Hex);
    }

    /// <summary>Renders the per-component <c>## Палитра</c> section, or empty if no roles.</summary>
    public static string RenderUsedRoles(IReadOnlyList<RoleEntry> roles)
    {
        if (roles.Count == 0) return string.Empty;
        var lines = new List<string> { "## Палитра", string.Empty };
        AppendRolesTable(lines, roles);
        return string.Join("\n", lines);
    }

    /// <summary>Renders the global palette document (all roles + tonal legend).</summary>
    public static string RenderGlobal(string title, IReadOnlyList<RoleEntry> roles, IReadOnlyList<PaletteFamily> families)
    {
        var lines = new List<string> { "# " + title, string.Empty, "## Семантические роли (light / dark)", string.Empty };
        AppendRolesTable(lines, roles);

        lines.Add(string.Empty);
        lines.Add("## Тональные палитры (легенда)");
        foreach (var family in families)
        {
            lines.Add(string.Empty);
            lines.Add("### " + family.Family);
            lines.Add(string.Empty);
            lines.Add("| Тон | Hex |");
            lines.Add("|-----|-----|");
            foreach (var tone in family.Tones)
                lines.Add($"| {tone.Tone} | {tone.Hex} |");
        }

        return string.Join("\n", lines);
    }

    private static void AppendRolesTable(List<string> lines, IReadOnlyList<RoleEntry> roles)
    {
        lines.Add("| Роль | Light тон | Light | Dark тон | Dark |");
        lines.Add("|------|-----------|-------|----------|------|");
        foreach (var r in roles)
            lines.Add($"| `{r.Role}` | {r.LightTone ?? "-"} | {r.LightHex} | {r.DarkTone ?? "-"} | {r.DarkHex} |");
    }

    [GeneratedRegex(@"^(?<fam>.+?)(?<tone>\d+)$")]
    private static partial Regex ToneRegex();
}
