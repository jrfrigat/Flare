using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Tools.CssAudit;

internal static class Program
{
    private static string _root = "";

    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var root = FindRepoRoot();
        _root = root ?? "";
        if (root is null)
        {
            Console.Error.WriteLine("Could not locate the repo root (a folder containing src/Flare.Components).");
            return 2;
        }

        var cssDir = Path.Combine(root, "src", "Flare.Components", "wwwroot", "css");
        var cssClassesDir = Path.Combine(root, "src", "Flare.Abstractions", "Css", "Classes");

        if (!Directory.Exists(cssDir)) { Console.Error.WriteLine($"CSS folder not found: {cssDir}"); return 2; }
        if (!Directory.Exists(cssClassesDir)) { Console.Error.WriteLine($"CssClasses folder not found: {cssClassesDir}"); return 2; }

        // The css folder of every theme (Flare.Theme.*/wwwroot/css). Scanned only for the extra
        // "in a theme but not in Flare.Components" report below -- the main analysis stays
        // Flare.Components-vs-constants.
        var srcDir = Path.Combine(root, "src");
        var cssThemeDirs = Directory.GetDirectories(srcDir, "Flare.Theme.*")
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .ToArray();

        var css = CollectCssClasses(cssDir);
        var constants = CollectConstants(cssClassesDir);
        var themeCss = CollectCssClasses(cssThemeDirs);

        // Non-interactive mode.
        var cmd = args.Length > 0 ? args[0].Trim().ToLowerInvariant().TrimStart('-') : null;
        return cmd switch
        {
            "check" => Check(css, constants, themeCss, cssThemeDirs) ? 0 : 1,
            "generate" or "gen" => Generate(css, constants),
            "merge" => Merge(css, cssClassesDir),
            null => Menu(css, constants, cssDir, cssClassesDir, themeCss, cssThemeDirs),
            _ => Usage(),
        };
    }

    private static int Usage()
    {
        Console.WriteLine("Flare.CssAudit - keeps CssClasses in sync with component CSS.");
        Console.WriteLine("Usage: cssaudit [check|generate]");
        Console.WriteLine("  check     Report classes missing in CssClasses and constants missing in CSS (exit 1 on mismatch).");
        Console.WriteLine("  generate  Emit C# constants for CSS classes missing from CssClasses, grouped by CSS file.");
        Console.WriteLine("  (no arg)  Interactive menu.");
        return 0;
    }

    // Theme/root marker classes the theme system sets on the wrapper element and styles via
    // attribute or compound selectors (e.g. [dir=rtl], .flare-theme-x) -- never a standalone rule.
    internal static bool IsThemeInfrastructure(string cls) =>
        cls is "flare-root" or "flare-rtl"
        || cls.StartsWith("flare-theme-", StringComparison.Ordinal)
        || cls.StartsWith("flare-mode-", StringComparison.Ordinal);

    // Theme names parsed from "...Flare.Theme.<Name>/wwwroot/css" dir paths.
    private static string ThemeNames(string[]? themeDirs) =>
        themeDirs is { Length: > 0 }
            ? string.Join(", ", themeDirs.Select(d => Regex.Match(d, @"Flare\.Theme\.([^\\/]+)").Groups[1].Value)
                                         .Where(n => n.Length > 0))
            : "(none)";

    private static int Menu(SortedDictionary<string, SortedSet<string>> css, ConstSet constants, string cssDir, string cssClassesDir,
        SortedDictionary<string, SortedSet<string>>? themeCss = null, string[]? themeDirs = null)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Flare CSS Audit ===");
            Console.WriteLine($"  CSS folder : {cssDir}");
            Console.WriteLine($"  Constants  : {cssClassesDir}");
            Console.WriteLine($"  Themes     : {ThemeNames(themeDirs)}");
            Console.WriteLine($"  {css.Count} CSS classes, {constants.Values.Count} constants");
            Console.WriteLine();
            Console.WriteLine("  [1] Check (compare both directions)");
            Console.WriteLine("  [2] Generate missing constants (preview, grouped by CSS file)");
            Console.WriteLine("  [3] Merge missing constants into CssClasses/ partials (in place)");
            Console.WriteLine("  [0] Exit");
            Console.Write("> ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": Check(css, constants, themeCss); break;
                case "2": Generate(css, constants); break;
                case "3":
                    Merge(css, cssClassesDir);
                    constants = CollectConstants(cssClassesDir); // refresh after writing
                    break;
                case "0" or null: return 0;
                default: Console.WriteLine("Unknown choice."); break;
            }
        }
    }

    // ---- Comparison ----

    // The single source of truth for the three sync reports, shared by the CLI and CssAudit.Run.
    //   Plus  -> classes in Flare.Components CSS with no CssClasses constant
    //   Minus -> constants with no Flare.Components rule (a full class name, not a runtime prefix,
    //            and not theme-scoping infrastructure)
    //   Tilde -> classes a theme defines that the Flare.Components base lacks (infra excluded)
    internal static (List<string> Plus, List<string> Minus, List<string> Tilde) Compare(
        SortedDictionary<string, SortedSet<string>> css, ConstSet constants,
        SortedDictionary<string, SortedSet<string>>? themeCss)
    {
        var plus = css.Keys.Where(c => !constants.Values.Contains(c))
            .OrderBy(c => c, StringComparer.Ordinal).ToList();
        var minus = constants.Values
            .Where(v => !css.ContainsKey(v) && !v.EndsWith("-", StringComparison.Ordinal) && !IsThemeInfrastructure(v))
            .OrderBy(v => v, StringComparer.Ordinal).ToList();
        var tilde = (themeCss?.Keys ?? Enumerable.Empty<string>())
            .Where(c => !css.ContainsKey(c) && !IsThemeInfrastructure(c))
            .OrderBy(c => c, StringComparer.Ordinal).ToList();
        return (plus, minus, tilde);
    }

    private static bool Check(SortedDictionary<string, SortedSet<string>> css, ConstSet constants,
        SortedDictionary<string, SortedSet<string>>? themeCss = null, string[]? themeDirs = null)
    {
        var (missingInConstants, missingInCss, inThemeNotBase) = Compare(css, constants, themeCss);

        Console.WriteLine();
        Console.WriteLine($"Flare.Components CSS classes: {css.Count}");
        Console.WriteLine($"CssClasses constants:        {constants.Values.Count}");
        if (themeCss is not null)
            Console.WriteLine($"Theme CSS classes:           {themeCss.Count}  (themes: {ThemeNames(themeDirs)})");
        Console.WriteLine();

        var clean = missingInConstants.Count == 0 && missingInCss.Count == 0 && inThemeNotBase.Count == 0;
        if (clean)
        {
            Console.WriteLine("OK - CssClasses, Flare.Components CSS and themes are fully in sync.");
            return true;
        }

        if (missingInConstants.Count > 0)
        {
            Console.WriteLine($"--- {missingInConstants.Count} class(es) in CSS but MISSING from CssClasses ---");
            foreach (var c in missingInConstants)
                Console.WriteLine($"  [+] {c}   (in {string.Join(", ", css[c])})");
            Console.WriteLine();
        }

        if (missingInCss.Count > 0)
        {
            Console.WriteLine($"--- {missingInCss.Count} constant(s) in CssClasses but MISSING from CSS ---");
            foreach (var v in missingInCss)
                Console.WriteLine($"  [-] {v}   ({constants.LocationOf(v)})");
            Console.WriteLine("      (note: a constant may reference CSS that lives in Flare.Core, not Flare.Components)");
            Console.WriteLine();
        }

        if (inThemeNotBase.Count > 0)
        {
            Console.WriteLine($"--- {inThemeNotBase.Count} class(es) in a theme but MISSING from Flare.Components (add a stub) ---");
            foreach (var c in inThemeNotBase)
                Console.WriteLine($"  [~] {c}   (in {string.Join(", ", themeCss![c])})");
            Console.WriteLine();
        }

        return false;
    }

    // Returns an int exit code for symmetry with the other verb handlers; generation has no
    // failure path of its own, so the code is always 0.
    private static int Generate(SortedDictionary<string, SortedSet<string>> css, ConstSet constants) //-V3009
    {
        // Classes present in CSS but not yet declared as a constant, grouped by CSS file.
        var missing = css.Where(kv => !constants.Values.Contains(kv.Key)).ToList();
        if (missing.Count == 0)
        {
            Console.WriteLine("Nothing to generate - every CSS class already has a constant.");
            return 0;
        }

        var byFile = new SortedDictionary<string, List<string>>();
        foreach (var (cls, files) in missing)
        {
            var file = PickOwningFile(cls, files);
            (byFile.TryGetValue(file, out var list) ? list : byFile[file] = new()).Add(cls);
        }

        var sb = new StringBuilder();
        sb.AppendLine("// Generated by Flare.CssAudit - review names, then merge into CssClasses.cs.");
        sb.AppendLine("// One nested class per CSS file (split by file for readability).");
        sb.AppendLine();
        foreach (var (file, classes) in byFile)
        {
            var nested = Pascal(Path.GetFileNameWithoutExtension(file));
            var prefix = $"flare-{Path.GetFileNameWithoutExtension(file)}";
            sb.AppendLine($"// {file}");
            sb.AppendLine($"public static class {nested}");
            sb.AppendLine("{");
            var used = new HashSet<string>();
            foreach (var cls in classes.OrderBy(c => c))
            {
                var name = FieldName(cls, prefix);
                var orig = name;
                var n = 2;
                while (!used.Add(name)) name = orig + n++;
                sb.AppendLine($"    public const string {name} = \"{cls}\";");
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());

        Console.Write("Write to tools/Flare.CssAudit/generated-cssclasses.txt? [y/N] ");
        if (string.Equals(Console.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
        {
            var outPath = Path.Combine(_root, "tools", "Flare.CssAudit", "generated-cssclasses.txt");
            File.WriteAllText(outPath, sb.ToString());
            Console.WriteLine($"Written: {outPath}");
        }
        return 0;
    }

    // ---- In-place merge into CssClasses.cs ----

    private sealed class Block
    {
        public required string File;     // the partial file this block lives in
        public required string Name;
        public required int OpenBrace;   // line index of the '{'
        public required int CloseBrace;  // line index of the '}'
        public required string Indent;   // indentation of the class declaration
        public List<string> Values = new();
        public HashSet<string> Fields = new(StringComparer.Ordinal);
        public string Prefix = "";
    }

    private static int Merge(SortedDictionary<string, SortedSet<string>> css, string cssClassesDir)
    {
        // Read every per-component partial under CssClasses/ (the monolith is now near-empty).
        var filesLines = Directory.EnumerateFiles(cssClassesDir, "*.cs")
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToDictionary(p => p, p => File.ReadAllLines(p).ToList(), StringComparer.Ordinal);
        var blocks = ParseBlocks(filesLines);
        if (blocks.Count == 0) { Console.Error.WriteLine("Could not parse nested classes from CssClasses/ partials"); return 2; }

        var existing = new HashSet<string>(blocks.SelectMany(b => b.Values), StringComparer.Ordinal);
        foreach (var b in blocks) b.Prefix = LongestCommonPrefix(b.Values);

        var missing = css.Keys.Where(c => !existing.Contains(c)).ToList();
        if (missing.Count == 0) { Console.WriteLine("Nothing to merge - CssClasses already covers every CSS class."); return 0; }

        // Assign each missing class to an existing block (by longest matching prefix);
        // the rest are grouped by their own class prefix (flare-foo up to the first __/--).
        var addToBlock = new Dictionary<Block, List<(string Field, string Value)>>();
        var pending = new SortedDictionary<string, List<string>>(StringComparer.Ordinal); // classPrefix -> classes

        void AddToBlock(Block b, string cls, string prefix)
        {
            var name = Unique(RelName(cls, prefix), b.Fields);
            b.Fields.Add(name);
            (addToBlock.TryGetValue(b, out var l) ? l : addToBlock[b] = new()).Add((name, cls));
        }

        foreach (var cls in missing)
        {
            // Match an existing block whose prefix is a *segment* prefix of the class
            // (so "flare-col" does NOT swallow "flare-color-customizer").
            var target = blocks
                .Where(b => b.Prefix.Length >= "flare-a".Length && SegMatch(cls, b.Prefix))
                .OrderByDescending(b => b.Prefix.Length)
                .FirstOrDefault();

            if (target is not null) AddToBlock(target, cls, target.Prefix);
            else
            {
                var p = FirstSegment(cls); // group utilities by their first segment: flare-flex-1 -> flare-flex
                (pending.TryGetValue(p, out var l) ? l : pending[p] = new()).Add(cls);
            }
        }

        // Resolve pending groups: fold into an existing same-named block, else queue a new class.
        var newGroups = new SortedDictionary<string, List<string>>(StringComparer.Ordinal); // className -> (prefix, classes)
        var newPrefix = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (prefix, classes) in pending)
        {
            var name = Pascal(prefix.StartsWith("flare-", StringComparison.Ordinal) ? prefix["flare-".Length..] : prefix);
            var existingBlock = blocks.FirstOrDefault(b => b.Name == name);
            if (existingBlock is not null)
                foreach (var c in classes) AddToBlock(existingBlock, c, prefix);
            else { newGroups[name] = classes; newPrefix[name] = prefix; }
        }

        // 1) Insert into existing blocks. Group by partial file, then insert bottom-up
        //    within each file so earlier line indices stay valid.
        var changedFiles = new HashSet<string>(StringComparer.Ordinal);
        foreach (var fileGroup in addToBlock.GroupBy(kv => kv.Key.File))
        {
            var lines = filesLines[fileGroup.Key];
            foreach (var (block, consts) in fileGroup.OrderByDescending(kv => kv.Key.CloseBrace))
            {
                var insert = consts.OrderBy(c => c.Value)
                    .Select(c => $"{block.Indent}    public const string {c.Field} = \"{c.Value}\";");
                lines.InsertRange(block.CloseBrace, insert);
            }
            changedFiles.Add(fileGroup.Key);
        }

        // 2) Each brand-new nested class becomes its own partial file under CssClasses/,
        //    matching the per-component refactoring.
        foreach (var (nested, classes) in newGroups)
        {
            var prefix = newPrefix[nested];
            var used = new HashSet<string>(StringComparer.Ordinal);
            var file = new List<string>
            {
                "namespace Flare;",
                "",
                "public static partial class CssClasses",
                "{",
                $"    /// <summary>Classes for {prefix}* (generated by Flare.CssAudit).</summary>",
                $"    public static class {nested}",
                "    {",
            };
            foreach (var cls in classes.OrderBy(c => c))
            {
                var name = Unique(RelName(cls, prefix), used);
                used.Add(name);
                file.Add($"        public const string {name} = \"{cls}\";");
            }
            file.Add("    }");
            file.Add("}");

            var path = UniquePath(Path.Combine(cssClassesDir, nested + ".cs"));
            filesLines[path] = file;
            changedFiles.Add(path);
        }

        foreach (var file in changedFiles)
            File.WriteAllLines(file, filesLines[file]);

        Console.WriteLine($"Merged {missing.Count} class(es): {addToBlock.Sum(kv => kv.Value.Count)} into existing classes, " +
                          $"{newGroups.Sum(kv => kv.Value.Count)} into {newGroups.Count} new partial file(s).");
        Console.WriteLine($"Updated {changedFiles.Count} file(s) under: {cssClassesDir}");
        return 0;
    }

    // Avoid clobbering an existing partial file when emitting a brand-new nested class.
    private static string UniquePath(string path)
    {
        if (!File.Exists(path)) return path;
        var dir = Path.GetDirectoryName(path)!;
        var stem = Path.GetFileNameWithoutExtension(path);
        var n = 2;
        string candidate;
        do { candidate = Path.Combine(dir, $"{stem}{n++}.cs"); } while (File.Exists(candidate));
        return candidate;
    }

    private static List<Block> ParseBlocks(Dictionary<string, List<string>> filesLines)
    {
        var blocks = new List<Block>();
        foreach (var (file, lines) in filesLines)
            ParseBlocksInto(file, lines, blocks);
        return blocks;
    }

    private static void ParseBlocksInto(string file, List<string> lines, List<Block> blocks)
    {
        for (var i = 0; i < lines.Count; i++)
        {
            var m = NestedClassRx.Match(lines[i]);
            if (!m.Success || m.Groups[1].Value == "CssClasses") continue;

            // Find the opening brace (same or next non-empty line) and its matching close.
            var open = i;
            while (open < lines.Count && !lines[open].Contains('{')) open++;
            if (open >= lines.Count) continue;

            var depth = 0; var close = -1;
            for (var j = open; j < lines.Count; j++)
            {
                depth += lines[j].Count(c => c == '{') - lines[j].Count(c => c == '}');
                if (depth == 0) { close = j; break; }
            }
            if (close < 0) continue;

            var indent = new string(lines[i].TakeWhile(char.IsWhiteSpace).ToArray());
            var b = new Block { File = file, Name = m.Groups[1].Value, OpenBrace = open, CloseBrace = close, Indent = indent };
            for (var j = open; j <= close; j++)
            {
                var cm = ConstRx.Match(lines[j]);
                if (cm.Success) { b.Values.Add(cm.Groups[2].Value); b.Fields.Add(cm.Groups[1].Value); }
            }
            blocks.Add(b);
            i = close;
        }
    }

    private static string LongestCommonPrefix(IEnumerable<string> values)
    {
        string? prefix = null;
        foreach (var v in values)
        {
            if (prefix is null) { prefix = v; continue; }
            var n = Math.Min(prefix.Length, v.Length);
            var k = 0; while (k < n && prefix[k] == v[k]) k++;
            prefix = prefix[..k];
        }
        return prefix ?? "";
    }

    // True when prefix matches cls at a segment boundary (next char is a separator or end),
    // so "flare-col" matches "flare-col"/"flare-col-1"/"flare-col__x" but NOT "flare-color".
    private static bool SegMatch(string cls, string prefix)
        => cls.Length >= prefix.Length
           && string.CompareOrdinal(cls, 0, prefix, 0, prefix.Length) == 0
           && (cls.Length == prefix.Length || cls[prefix.Length] is '-' or '_');

    // First segment of a class after "flare-": flare-flex-1 -> flare-flex, flare-color-customizer -> flare-color.
    private static string FirstSegment(string cls)
    {
        if (!cls.StartsWith("flare-", StringComparison.Ordinal)) return cls;
        var rest = cls["flare-".Length..];
        var i = rest.IndexOfAny(new[] { '-', '_' });
        return "flare-" + (i < 0 ? rest : rest[..i]);
    }

    // The component prefix of a class: everything before the first "__" or "--" (else the whole class).
    private static string ClassBlockPrefix(string cls)
    {
        var i = cls.IndexOf("__", StringComparison.Ordinal);
        var j = cls.IndexOf("--", StringComparison.Ordinal);
        var cut = (i, j) switch
        {
            ( < 0, < 0) => cls.Length,
            ( < 0, _) => j,
            (_, < 0) => i,
            _ => Math.Min(i, j),
        };
        return cls[..cut];
    }

    private static string RelName(string cls, string prefix)
    {
        var rest = cls.Length > prefix.Length ? cls[prefix.Length..] : "";
        var name = Pascal(rest);
        return string.IsNullOrEmpty(name) ? "Root" : name;
    }

    private static string Unique(string name, HashSet<string> used)
    {
        if (!used.Contains(name)) return name;
        var n = 2; while (used.Contains(name + n)) n++;
        return name + n;
    }

    // ---- Parsing ----

    // Matches a class selector: a '.' followed by a flare-* identifier. No lookbehind, so compound
    // selectors (".a.b") are both captured; the "flare-" prefix rules out CSS decimals (.5rem).
    private static readonly Regex ClassRx =
        new(@"\.(flare-[A-Za-z0-9_-]+)", RegexOptions.Compiled);

    internal static SortedDictionary<string, SortedSet<string>> CollectCssClasses(params string[] cssDirs)
    {
        var map = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        foreach (var cssDir in cssDirs)
        {
            if (!Directory.Exists(cssDir)) continue;
            foreach (var path in Directory.EnumerateFiles(cssDir, "*.css", SearchOption.AllDirectories).OrderBy(p => p))
            {
                var text = StripCommentsAndStrings(File.ReadAllText(path));
                // For theme CSS, prefix the file with the theme name (button.css exists in several themes).
                var tm = Regex.Match(path, @"Flare\.Theme\.([^\\/]+)");
                var file = tm.Success ? $"{tm.Groups[1].Value}/{Path.GetFileName(path)}" : Path.GetFileName(path);
                foreach (Match m in ClassRx.Matches(text))
                {
                    var cls = m.Groups[1].Value;
                    (map.TryGetValue(cls, out var files) ? files : map[cls] = new(StringComparer.Ordinal)).Add(file);
                }
            }
        }
        return map;
    }

    private static readonly Regex NestedClassRx =
        new(@"public\s+static\s+class\s+([A-Za-z_]\w*)", RegexOptions.Compiled);
    private static readonly Regex ConstRx =
        new(@"public\s+const\s+string\s+([A-Za-z_]\w*)\s*=\s*""([^""]*)""", RegexOptions.Compiled);

    // Read constants from every per-component partial under CssClasses/ (the monolithic
    // CssClasses.cs is now near-empty after the refactoring).
    internal static ConstSet CollectConstants(string cssClassesDir)
    {
        var set = new ConstSet();
        foreach (var file in Directory.EnumerateFiles(cssClassesDir, "*.cs").OrderBy(p => p, StringComparer.Ordinal))
        {
            string current = "CssClasses";
            foreach (var raw in File.ReadAllLines(file))
            {
                var nm = NestedClassRx.Match(raw);
                if (nm.Success && nm.Groups[1].Value != "CssClasses")
                    current = nm.Groups[1].Value;

                var cm = ConstRx.Match(raw);
                if (cm.Success)
                    set.Add(cm.Groups[2].Value, current, cm.Groups[1].Value);
            }
        }
        return set;
    }

    private static string StripCommentsAndStrings(string css)
    {
        css = Regex.Replace(css, @"/\*.*?\*/", " ", RegexOptions.Singleline); // comments
        css = Regex.Replace(css, "\"[^\"]*\"|'[^']*'", " ");                  // string values
        return css;
    }

    // Prefer the file whose name matches the class prefix (avoids dumping into shared files like a11y.css).
    private static string PickOwningFile(string cls, SortedSet<string> files)
    {
        foreach (var f in files)
            if (cls.StartsWith("flare-" + Path.GetFileNameWithoutExtension(f), StringComparison.Ordinal))
                return f;
        return files.FirstOrDefault(f => !string.Equals(f, "a11y.css", StringComparison.OrdinalIgnoreCase)) ?? files.First();
    }

    // ---- Naming ----

    private static string FieldName(string cls, string filePrefix)
    {
        var rest = cls.StartsWith(filePrefix + "--", StringComparison.Ordinal) ? cls[(filePrefix.Length)..]
                 : cls.StartsWith(filePrefix + "__", StringComparison.Ordinal) ? cls[(filePrefix.Length)..]
                 : cls == filePrefix ? ""
                 : cls.StartsWith("flare-", StringComparison.Ordinal) ? cls["flare-".Length..]
                 : cls;
        var name = Pascal(rest);
        return string.IsNullOrEmpty(name) ? "Root" : name;
    }

    private static string Pascal(string s)
    {
        var parts = Regex.Split(s, @"[^A-Za-z0-9]+").Where(p => p.Length > 0);
        var sb = new StringBuilder();
        foreach (var p in parts)
            sb.Append(char.ToUpperInvariant(p[0])).Append(p[1..]);
        var name = sb.ToString();
        // Ensure a valid C# identifier (numeric span classes like flare-col--1 -> "N1").
        if (name.Length > 0 && char.IsDigit(name[0])) name = "N" + name;
        return name;
    }

    internal static string? FindRepoRoot()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
            dir = dir.Parent;
        }
        // Fall back to walking up from the binary location.
        dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
            dir = dir.Parent;
        }
        return null;
    }
}

/// <summary>Constants parsed from CssClasses.cs: value -> declaring nested class + field.</summary>
internal sealed class ConstSet
{
    private readonly Dictionary<string, (string Owner, string Field)> _byValue = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> Values => _byValue.Keys;

    public void Add(string value, string owner, string field) => _byValue[value] = (owner, field);

    public string LocationOf(string value) =>
        _byValue.TryGetValue(value, out var loc) ? $"Css.Classes.{loc.Owner}.{loc.Field}" : "?";
}
