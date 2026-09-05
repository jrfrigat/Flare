using System.Runtime.CompilerServices;

namespace Flare.Core.Tests;

/// <summary>
/// Russian text in this repository is written without the letter yo: <c>U+0451</c> is spelled <c>U+0435</c>
/// and <c>U+0401</c> is spelled <c>U+0415</c>. The letter is optional in modern Russian orthography, so
/// either convention is defensible - but holding both at once is not. The RU resx values, the RU docs and
/// the RU changelog are one product; a term spelled one way in the Gallery and the other way in the
/// changelog is two terms to anyone searching it, and to any translation memory.
///
/// It needs a guard because nothing else looks at this text. The ASCII mandate covers code, XML docs and
/// comments, and explicitly exempts the localized RU resx values - which is exactly where the letter lives.
///
/// The letter is written here as an escape, because the source itself stays ASCII.
/// </summary>
public sealed class RussianTextTests
{
    private const char Yo = '\u0451';
    private const char YoCapital = '\u0401';

    // Text the repository authors. Binary assets, fonts and icon catalogues carry no prose.
    private static readonly string[] _extensions =
    [
        ".cs", ".razor", ".css", ".js", ".mjs", ".json", ".md", ".resx", ".xml",
        ".props", ".targets", ".csproj", ".html", ".yml", ".yaml", ".txt"
    ];

    // Build output, dependencies, and the working notes that never leave this machine.
    private static readonly string[] _skipDirectories =
        ["bin", "obj", "node_modules", ".git", ".claude", ".vs", "artifacts", "TestResults", "_site"];

    [Fact]
    public void NoRepositoryTextUsesTheYoLetter()
    {
        var root = FindRepoRoot();
        var offenders = new List<string>();
        var scanned = 0;

        foreach (var file in Walk(root))
        {
            if (!_extensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                continue;

            scanned++;
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf(Yo) < 0 && lines[i].IndexOf(YoCapital) < 0)
                    continue;

                var rel = Path.GetRelativePath(root, file).Replace('\\', '/');
                offenders.Add($"{rel}:{i + 1}");
            }
        }

        Assert.True(scanned > 500, $"Only {scanned} files were scanned - the walk is not reaching the repository.");
        Assert.True(
            offenders.Count == 0,
            $"Russian text here is written without the yo letter. Replace U+0451 with U+0435 and U+0401 with U+0415:{Environment.NewLine}"
                + string.Join(Environment.NewLine, offenders.Take(40)));
    }

    private static IEnumerable<string> Walk(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
            yield return file;

        foreach (var sub in Directory.EnumerateDirectories(directory))
        {
            if (_skipDirectories.Contains(Path.GetFileName(sub), StringComparer.OrdinalIgnoreCase))
                continue;

            foreach (var file in Walk(sub))
                yield return file;
        }
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
