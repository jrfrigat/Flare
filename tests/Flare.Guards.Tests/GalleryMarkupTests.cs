using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// The Gallery demonstrates that a site is built from Flare components alone - a raw element or an
/// inline style there is a missing capability in the library, not a shortcut. Raw string literals
/// (sample code shown to the reader) and HTML kept as data inside an attribute value are not markup
/// and are left alone.
/// </summary>
public sealed class GalleryMarkupTests
{
    private static readonly string[] RawTags =
    [
        "div", "span", "strong", "b", "em", "i", "small", "code", "pre", "p",
        "ul", "ol", "li", "dl", "dt", "dd", "table", "thead", "tbody", "tfoot",
        "tr", "td", "th", "img", "input", "label", "button", "section", "a",
        "style", "br", "hr", "option", "optgroup", "select", "textarea", "form",
    ];

    [Fact]
    public void GallerySourcesRenderThroughFlareComponents()
    {
        var gallery = Path.Combine(FindRepoRoot(), "samples", "Flare.Gallery");
        var files = Directory
            .EnumerateFiles(gallery, "*.razor", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

        var offenders = new List<string>();
        var tagPattern = new Regex(@"<\s*(" + string.Join('|', RawTags) + @")(?=[\s/>])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var stylePattern = new Regex(@"\sstyle\s*=", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            text = Regex.Replace(text, "\"\"\"[\\s\\S]*?\"\"\"", string.Empty);
            text = Regex.Replace(text, "@\\*[\\s\\S]*?\\*@", string.Empty);
            text = Regex.Replace(text, "<!--[\\s\\S]*?-->", string.Empty);
            // Attribute values are data: an HTML string passed to an editor is not Gallery markup.
            text = Regex.Replace(text, "=\"[^\"]*\"", "=\"\"");

            var relative = Path.GetRelativePath(gallery, file);
            var lineNumber = 0;
            foreach (var line in text.Split('\n'))
            {
                lineNumber++;
                if (tagPattern.IsMatch(line))
                    offenders.Add($"{relative}:{lineNumber}: raw HTML element - {line.Trim()}");
                else if (stylePattern.IsMatch(line))
                    offenders.Add($"{relative}:{lineNumber}: inline style - {line.Trim()}");
            }
        }

        Assert.True(offenders.Count == 0,
            "The Gallery must be built from Flare components, without raw HTML or inline styles:\n  "
            + string.Join("\n  ", offenders));
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Flare.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Flare.slnx not found above the test directory.");
    }
}
