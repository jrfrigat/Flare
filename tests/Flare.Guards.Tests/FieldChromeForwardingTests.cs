using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// Every field renders <c>FlareFieldChrome</c> as its root, so the three parameters that address that
/// root - <c>Class</c>, <c>Style</c> and the splatted attributes - reach the DOM only if the shell hands
/// them over. Dropping one is silent: the parameter still exists, still compiles, still shows up in the
/// API reference, and does nothing. All thirteen fields dropped <c>Class</c> that way, which is why a
/// responsive utility on the Gallery's search box never hid it on a phone.
/// </summary>
public sealed class FieldChromeForwardingTests
{
    // Style has two spellings in practice: the parameter itself, or a computed variant of it
    // (FlareField composes a custom-color token into _inlineStyle before forwarding).
    private static readonly (string Name, string Pattern)[] Required =
    {
        ("Class", @"Class=""@Class"""),
        ("Style", @"Style=""@[A-Za-z_]+"""),
        ("Attributes", @"Attributes=""AdditionalAttributes"""),
    };

    [Fact]
    public void EveryFieldForwardsTheParametersThatAddressItsRoot()
    {
        var root = FindRepoRoot();
        var offenders = new List<string>();
        var checkedFiles = 0;

        foreach (var file in Directory.EnumerateFiles(Path.Combine(root, "src"), "*.razor", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);
            var open = text.IndexOf("<FlareFieldChrome", StringComparison.Ordinal);
            if (open < 0) continue;

            var close = text.IndexOf('>', open);
            if (close < 0) continue;

            checkedFiles++;
            var tag = text[open..close];
            var name = Path.GetFileName(file);

            foreach (var (parameter, pattern) in Required)
                if (!Regex.IsMatch(tag, pattern))
                    offenders.Add($"{name} does not forward {parameter}");
        }

        Assert.True(checkedFiles > 10, $"Expected the whole field family, found {checkedFiles} files.");
        Assert.True(offenders.Count == 0, string.Join("; ", offenders));
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
