using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// A <c>&lt;button&gt;</c> with no <c>type</c> is a submit button - that is the HTML default, not an
/// omission - so every command button a component draws inside a form was one Enter or one click away
/// from posting it. Thirty-seven of them shipped that way: a calendar's month arrows, a pagination
/// strip, a tab, a stepper's next button, a carousel arrow, the rich-text toolbar, a dialog's own
/// actions. <c>FlareButton</c> never had the problem - it renders its <c>ButtonType</c>, which defaults
/// to <c>button</c> - and the raw ones simply bypassed that contract.
/// </summary>
/// <remarks>
/// Reading the markup rather than rendering it is what makes this complete: a component whose button
/// only appears in a state no test happens to render would be missed by any number of bUnit cases, and
/// the defect is in the markup either way. Flare.Components.Tests holds the companion that
/// renders representative controls inside an <c>EditForm</c> and proves nothing submits.
/// </remarks>
public sealed class ButtonTypeTests
{
    [Fact]
    public void EveryRawButtonDeclaresItsType()
    {
        var offenders = new List<string>();

        foreach (var file in RazorFiles())
        {
            var text = Blanked(File.ReadAllText(file));
            foreach (Match m in Regex.Matches(text, @"<button\b[\s\S]*?>"))
            {
                if (Regex.IsMatch(m.Value, @"\stype\s*=")) continue;
                var line = text[..m.Index].Count(c => c == '\n') + 1;
                offenders.Add($"{Path.GetFileName(file)}:{line}");
            }
        }

        Assert.True(offenders.Count == 0,
            "These <button> elements declare no type, so HTML makes them submit buttons and they post "
            + "the nearest form when pressed: " + string.Join(", ", offenders)
            + ". Write type=\"button\" for a command, or type=\"submit\" where submitting IS the point.");
    }

    // Comments are blanked rather than removed so the reported line numbers still point at the source.
    private static string Blanked(string razor) => razor
        .Replace("\r\n", "\n", StringComparison.Ordinal)
        .Pipe(s => Regex.Replace(s, @"@\*[\s\S]*?\*@", m => Blank(m.Value)))
        .Pipe(s => Regex.Replace(s, @"<!--[\s\S]*?-->", m => Blank(m.Value)))
        .Pipe(s => Regex.Replace(s, @"^[ \t]*//.*$", m => Blank(m.Value), RegexOptions.Multiline));

    private static string Blank(string s) => new string(s.Select(c => c == '\n' ? '\n' : ' ').ToArray());

    private static IEnumerable<string> RazorFiles() =>
        Directory.EnumerateFiles(Path.Combine(FindRepoRoot(), "src"), "*.razor", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                     && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

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

internal static class PipeExtensions
{
    public static string Pipe(this string value, Func<string, string> f) => f(value);
}
