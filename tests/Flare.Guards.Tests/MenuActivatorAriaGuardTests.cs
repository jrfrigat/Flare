using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// A menu hands its activator slot the ARIA of the popup - <c>aria-haspopup</c>, <c>aria-expanded</c>
/// and <c>aria-controls</c> - and the caller is the one who puts it on the element that takes the
/// focus. A caller who forgets compiles, renders and looks right: the only thing missing is what a
/// screen reader says, which is exactly the defect this replaced. So every activator in this repository
/// that contains a button has to spread the context's attributes onto it.
///
/// An activator without a button is left alone: a right-click context menu hangs off a surface, and
/// there is no focusable element there to carry the attributes.
/// </summary>
public sealed class MenuActivatorAriaGuardTests
{
    [Fact]
    public void EveryButtonActivatorSpreadsTheMenusAriaOntoItself()
    {
        var root = FindRepoRoot();
        var offenders = new List<string>();

        foreach (var dir in new[] { "src", "samples" })
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, dir), "*.razor",
                         SearchOption.AllDirectories))
            {
                if (IsBuildArtifact(file)) continue;

                var text = File.ReadAllText(file);
                foreach (var block in Regex.Matches(text, "<Activator(?: [^>]*)?>(.*?)</Activator>",
                             RegexOptions.Singleline).Cast<Match>())
                {
                    var body = block.Groups[1].Value;
                    if (!body.Contains("<button", StringComparison.OrdinalIgnoreCase)
                        && !body.Contains("Button", StringComparison.Ordinal)) continue;
                    if (body.Contains(".Attributes", StringComparison.Ordinal)) continue;

                    var line = text.Take(block.Index).Count(c => c == '\n') + 1;
                    offenders.Add($"{Path.GetRelativePath(root, file)}:{line}");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "A menu activator holds a button but never spreads the activator context's attributes onto "
            + "it, so that button announces neither the menu nor whether it is open. Add "
            + "'@attributes=\"context.Attributes\"' to it. Offenders:\n  "
            + string.Join("\n  ", offenders));
    }

    private static bool IsBuildArtifact(string path)
    {
        var sep = Path.DirectorySeparatorChar;
        return path.Contains($"{sep}bin{sep}", StringComparison.Ordinal)
            || path.Contains($"{sep}obj{sep}", StringComparison.Ordinal);
    }

    private static string FindRepoRoot([CallerFilePath] string thisFile = "")
    {
        var dir = Path.GetDirectoryName(thisFile);
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src")))
            dir = Path.GetDirectoryName(dir);
        Assert.False(dir is null, "Could not locate the repository root (no ancestor 'src' folder).");
        return dir!;
    }
}
