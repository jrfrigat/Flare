using System.Runtime.CompilerServices;

namespace Flare.Core.Tests;

/// <summary>
/// Architecture guard for the SVG icon migration: no component may render a Material Symbols
/// <em>ligature span</em> - a <c>&lt;span class="material-symbols-*"&gt;</c> whose text content is an icon
/// name the web font is expected to substitute with a glyph.
///
/// The icon system is <c>FlareIcon</c> / <c>FlareIconView</c>, which inlines SVG and needs no font. A
/// component that still emits a ligature renders the literal word ("edit", "check_box") in every app that
/// does not happen to load the Material Symbols font - which is how three DataGrid call sites shipped
/// broken while the Gallery, which loads the font for its own chrome, showed them correct.
///
/// The Material Symbols icon <em>packages</em> are exempt: emitting that class is their entire job.
/// </summary>
public sealed class IconFontLigatureGuardTests
{
    private const string Marker = "material-symbols";

    // Packages whose purpose IS the icon font. Everything else is a component and must use FlareIconView.
    private static readonly string[] ExemptProjects =
    [
        "Flare.Icons.MaterialDesign2.Symbols",
        "Flare.Icons.MaterialDesign3.Symbols",
        "Flare.Icons.FontAwesome.Symbols",
    ];

    [Fact]
    public void NoComponentRendersAnIconFontLigature()
    {
        var srcDir = Path.Combine(FindRepoRoot(), "src");
        Assert.True(Directory.Exists(srcDir), $"Missing src dir: {srcDir}");

        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(srcDir, "*.*", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(file);
            if (ext is not (".razor" or ".cs")) continue;
            if (IsBuildArtifact(file) || IsExempt(file)) continue;

            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains(Marker, StringComparison.OrdinalIgnoreCase)) continue;
                offenders.Add($"{Path.GetRelativePath(srcDir, file)}:{i + 1}");
            }
        }

        Assert.True(offenders.Count == 0,
            "A component renders a Material Symbols ligature span. Icons are SVG descriptors: render " +
            "<FlareIconView Value=\"@FlareIcons.X\" /> instead, and add the icon to FlareIcons if it is " +
            $"missing. Offenders:\n  {string.Join("\n  ", offenders)}");
    }

    private static bool IsExempt(string path)
    {
        var sep = Path.DirectorySeparatorChar;
        return ExemptProjects.Any(p => path.Contains($"{sep}{p}{sep}", StringComparison.Ordinal));
    }

    private static bool IsBuildArtifact(string path)
    {
        var sep = Path.DirectorySeparatorChar;
        return path.Contains($"{sep}bin{sep}", StringComparison.Ordinal)
            || path.Contains($"{sep}obj{sep}", StringComparison.Ordinal);
    }

    // The compile-time path of THIS test file, walked up until the folder that contains "src".
    private static string FindRepoRoot([CallerFilePath] string thisFile = "")
    {
        var dir = Path.GetDirectoryName(thisFile);
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src")))
            dir = Path.GetDirectoryName(dir);
        Assert.False(dir is null, "Could not locate the repository root (no ancestor 'src' folder).");
        return dir!;
    }
}
