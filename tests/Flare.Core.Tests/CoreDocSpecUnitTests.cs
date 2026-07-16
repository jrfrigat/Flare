using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// The core's public docs may not quote a design system's measurements. This is the same rule
/// <see cref="TokenDocLiteralTests"/> enforces on the token records, applied where it was still missing: the
/// components' own <c>[Parameter]</c> and enum docs. A size step is a LABEL - each theme maps it onto its own
/// tokens - so a doc that pins a number to it is unowned prose, and it drifts. It had: <c>SwitchSize.Md</c>
/// was documented as the "52x32dp" switch, which is Material Design 3's value; FluentUI2 draws it 40x20, so
/// the doc was already wrong for a shipped theme. <c>FabSize</c> promised a "40dp / 56dp / 96dp container"
/// for a component that has no size token at all - its size comes from padding around the glyph.
///
/// <b>dp</b> is the signal, and it is an exact one. It is a design-spec unit: it appears in no C# and no CSS,
/// so it can only turn up in the core when someone is quoting a design system. That keeps this guard free of
/// the judgement call that a blanket ban on numbers would need - a component's own behaviour may legitimately
/// be documented with a number (<c>Default: 300ms</c> for a debounce), and so may an example of a value the
/// CALLER supplies (<c>e.g. "48px"</c>). Those are the core's to state; a theme's geometry is not.
///
/// The theme packages are deliberately out of scope: quoting the spec they implement is exactly what their
/// comments are for.
/// </summary>
public sealed class CoreDocSpecUnitTests
{
    // A design-spec measurement: digits glued to "dp". Deliberately NOT anchored on the left: the real
    // offender read "52x32dp", where the 32 is preceded by an 'x', so a leading \b would have skipped the
    // very doc this guard exists for. The trailing \b still keeps "300dpi" out, and "dropdown" carries no
    // digits so it cannot match either way.
    private static readonly Regex _dp = new(@"\d+(\.\d+)?dp\b", RegexOptions.Compiled);
    private static readonly Regex _docLine = new(@"^\s*///(.*)$", RegexOptions.Compiled);

    [Fact]
    public void CoreDocs_DoNotQuoteADesignSystemsMeasurements()
    {
        var root = FindRepoRoot();
        var offenders = new List<string>();
        var scanned = 0;

        foreach (var dir in new[] { "Flare.Components", "Flare.Abstractions", "Flare.Theming" })
        {
            var path = Path.Combine(root, "src", dir);
            if (!Directory.Exists(path)) continue;

            foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                         .Where(f => f.EndsWith(".cs", StringComparison.Ordinal) || f.EndsWith(".razor", StringComparison.Ordinal))
                         .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                  && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                         .OrderBy(f => f, StringComparer.Ordinal))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var m = _docLine.Match(lines[i]);
                    if (!m.Success) continue;
                    scanned++;
                    foreach (Match d in _dp.Matches(m.Groups[1].Value))
                        offenders.Add($"{Path.GetFileName(file)}:{i + 1}  quotes '{d.Value}'");
                }
            }
        }

        Assert.True(scanned > 0, "Scanned no doc comments - the guard is not looking where it thinks it is.");
        Assert.True(offenders.Count == 0,
            "A core doc must not state a design system's measurement: the core owns no geometry, so the "
            + "number belongs to whichever theme is loaded and the doc is wrong under every other one. Say "
            + "what the member is FOR and leave the size to the theme - the size steps are labels, not "
            + "measurements:\n  "
            + string.Join("\n  ", offenders));
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
        throw new DirectoryNotFoundException("Could not locate the repo root (a folder containing src/Flare.Components).");
    }
}
