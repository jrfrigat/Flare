using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// A settled open/visible state must not carry an identity transform. <c>translate(0, 0)</c> and
/// <c>none</c> paint the same and behave differently: any transform other than <c>none</c> makes the
/// element the containing block for its <c>position: fixed</c> descendants, so every anchored overlay
/// opened inside it - select, menu, tooltip, autocomplete, date picker - has its viewport coordinates
/// measured from that element's corner and lands off-screen. An open drawer shipped this for several
/// versions: a select inside it drew 896px past the right edge of the window and read as an empty
/// dropdown. Nothing about the markup is wrong when it happens, so only a rule like this catches it.
/// </summary>
public sealed class SettledTransformTests
{
    // Modifiers that name a state the element rests in, as opposed to a transition it passes through.
    private static readonly string[] SettledModifiers = { "--open", "--visible", "--shown", "--expanded" };

    [Fact]
    public void NoSettledStateKeepsAnIdentityTransform()
    {
        var offenders = new List<string>();
        var checkedRules = 0;

        foreach (var file in CssFiles())
        {
            var css = StripKeyframes(File.ReadAllText(file));
            var name = Path.GetFileName(file);

            foreach (Match rule in Regex.Matches(css, @"([^{}]+)\{([^{}]*)\}"))
            {
                var selector = rule.Groups[1].Value;
                if (!SettledModifiers.Any(m => selector.Contains(m, StringComparison.Ordinal)))
                    continue;

                checkedRules++;

                var transform = Regex.Match(rule.Groups[2].Value, @"transform\s*:\s*([^;}]+)");
                if (!transform.Success) continue;

                var value = transform.Groups[1].Value
                    .Replace("!important", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Trim();

                if (IsIdentity(value))
                    offenders.Add($"{name}: {Collapse(selector)} -> transform: {value}");
            }
        }

        Assert.True(checkedRules > 5, $"Expected settled-state rules to scan, found {checkedRules}.");
        Assert.True(offenders.Count == 0,
            "A settled state must use `transform: none`, never an identity transform - it would become " +
            "the containing block for the fixed-position overlays inside it. Offenders: " +
            string.Join("; ", offenders));
    }

    // An identity transform is one whose every function is a no-op: zero translation or rotation,
    // unit scale. `none` is the correct value and is not one of these.
    private static bool IsIdentity(string value)
    {
        var functions = Regex.Matches(value, @"([a-zA-Z0-9]+)\s*\(([^)]*)\)");
        if (functions.Count == 0) return false;

        foreach (Match function in functions)
        {
            var name = function.Groups[1].Value.ToLowerInvariant();
            var args = function.Groups[2].Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var neutral = name.StartsWith("scale", StringComparison.Ordinal) ? "1" : "0";
            if (args.Any(a => StripUnit(a) != neutral))
                return false;
        }

        return true;
    }

    private static string StripUnit(string argument) =>
        Regex.Replace(argument, @"(px|rem|em|%|deg|rad|turn|vh|vw)$", string.Empty).TrimStart('+');

    // Keyframes legitimately end on an identity transform: the animation reverts to the base style
    // when it finishes, so nothing is left holding a containing block.
    private static string StripKeyframes(string css) =>
        Regex.Replace(css, @"@(-\w+-)?keyframes\s+[\w-]+\s*\{(?:[^{}]*\{[^{}]*\})*[^{}]*\}", string.Empty);

    private static IEnumerable<string> CssFiles()
    {
        var src = Path.Combine(FindRepoRoot(), "src");
        return Directory.EnumerateDirectories(src)
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.css", SearchOption.AllDirectories));
    }

    private static string Collapse(string text) => Regex.Replace(text, @"\s+", " ").Trim();

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
