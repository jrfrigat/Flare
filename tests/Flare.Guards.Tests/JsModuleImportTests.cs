using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// Every name a JS module takes from <c>flare-dom.js</c> must appear in that module's import list.
/// The failure this guards is silent and total: the helpers are called at module scope
/// (<c>const _x = registry();</c>), so a missing import is a ReferenceError while the module is being
/// evaluated. The module never finishes loading, every service that imports it rejects, and the
/// component simply does nothing - no compile error, no test failure, and in the case of
/// <c>flare-theme.js</c> no design tokens on the page at all, which renders the entire library
/// unstyled. It cost exactly that once.
/// </summary>
public sealed class JsModuleImportTests
{
    [Fact]
    public void EveryModuleImportsTheSharedHelpersItCalls()
    {
        var jsDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "js");
        var shared = SharedExportNames(Path.Combine(jsDir, "flare-dom.js"));
        Assert.NotEmpty(shared);

        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(jsDir, "*.js"))
        {
            var name = Path.GetFileName(file);
            if (name == "flare-dom.js") continue;

            var raw = File.ReadAllText(file);
            var code = StripCommentsAndStrings(raw);
            var imported = ImportedNames(raw);

            foreach (var helper in shared)
            {
                var escaped = Regex.Escape(helper);
                // A call expression is the only use that matters, and it is unambiguous: a bare word
                // followed by "(" cannot be a property access or a declaration of something else.
                if (!Regex.IsMatch(code, $@"(?<![.\w${{]){escaped}\s*\(")) continue;
                if (imported.Contains(helper)) continue;
                // A module is free to bind the same name itself - as a declaration, or as a parameter
                // (`new Promise((resolve) => ...)` is the one that actually occurs).
                if (Regex.IsMatch(code, $@"(?:function|const|let|var)\s+{escaped}\b")) continue;
                if (Regex.IsMatch(code, $@"[(,]\s*{escaped}\s*[),=]")) continue;
                offenders.Add($"{name} calls {helper}() without importing it");
            }
        }

        Assert.True(offenders.Count == 0, "JS modules using an unimported helper:\n  " + string.Join("\n  ", offenders));
    }

    [Fact]
    public void EveryImportFromTheSharedModuleResolvesToAnExport()
    {
        var jsDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "js");
        var shared = SharedExportNames(Path.Combine(jsDir, "flare-dom.js"));
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(jsDir, "*.js"))
        {
            var name = Path.GetFileName(file);
            if (name == "flare-dom.js") continue;

            foreach (var imported in ImportedNames(File.ReadAllText(file)))
                if (!shared.Contains(imported))
                    offenders.Add($"{name} imports {imported}, which flare-dom.js does not export");
        }

        Assert.True(offenders.Count == 0, "Unresolvable imports:\n  " + string.Join("\n  ", offenders));
    }

    private static HashSet<string> SharedExportNames(string path)
    {
        var code = StripCommentsAndStrings(File.ReadAllText(path));
        return Regex.Matches(code, @"export\s+function\s+([A-Za-z_$][\w$]*)")
            .Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>
    /// Reads the import list from the RAW source: the module path is a string literal, so the
    /// comment/string stripper would erase the very thing this has to match on.
    /// </summary>
    private static HashSet<string> ImportedNames(string code)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in Regex.Matches(code, @"import\s*\{([^}]*)\}\s*from\s*['""]\./flare-dom\.js['""]"))
            foreach (var part in m.Groups[1].Value.Split(','))
            {
                var trimmed = part.Trim();
                if (trimmed.Length > 0) names.Add(trimmed);
            }

        return names;
    }

    /// <summary>
    /// Blanks out comments and string/template literals so prose mentioning a helper by name, or a
    /// dotted interop call string, cannot be mistaken for a call site.
    /// </summary>
    private static string StripCommentsAndStrings(string code)
    {
        var sb = new StringBuilder(code.Length);
        for (var i = 0; i < code.Length; i++)
        {
            var c = code[i];
            if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
            {
                while (i < code.Length && code[i] != '\n') i++;
                sb.Append('\n');
                continue;
            }

            if (c == '/' && i + 1 < code.Length && code[i + 1] == '*')
            {
                i += 2;
                while (i + 1 < code.Length && !(code[i] == '*' && code[i + 1] == '/')) i++;
                i++;
                continue;
            }

            if (c is '\'' or '"' or '`')
            {
                var quote = c;
                i++;
                while (i < code.Length && code[i] != quote)
                {
                    if (code[i] == '\\') i++;
                    i++;
                }

                sb.Append("''");
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

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
