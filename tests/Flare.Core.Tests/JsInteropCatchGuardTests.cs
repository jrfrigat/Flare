using System.Runtime.CompilerServices;

namespace Flare.Core.Tests;

/// <summary>
/// Wherever a component swallows <c>JSDisconnectedException</c> (the circuit went away) it must also
/// swallow <c>JSException</c> (the call itself failed in the browser). The second is what a best-effort
/// call gets when the JS function is missing - a browser running an older cached
/// <c>_content/…/*.js</c> than the assembly, since those asset URLs are not fingerprinted - and it
/// otherwise takes the render, or the circuit, down for an enhancement the page could go without.
/// </summary>
public sealed class JsInteropCatchGuardTests
{
    private const string Disconnected = "catch (JSDisconnectedException)";
    private const string Generic = "catch (JSException)";

    [Fact]
    public void EveryJsDisconnectedCatchAlsoCatchesJsException()
    {
        var srcDir = Path.Combine(FindRepoRoot(), "src");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(srcDir, "*.*", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(file);
            if (ext is not (".razor" or ".cs") || IsBuildArtifact(file)) continue;

            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var at = lines[i].IndexOf(Disconnected, StringComparison.Ordinal);
                if (at < 0) continue;
                // The generic catch has to be the very next catch clause of the same try - which is
                // either the rest of this line (one-liner catches) or the line below.
                var rest = lines[i][(at + Disconnected.Length)..];
                var next = i + 1 < lines.Length ? lines[i + 1] : string.Empty;
                if (rest.Contains(Generic, StringComparison.Ordinal)
                    || next.Contains(Generic, StringComparison.Ordinal)) continue;
                offenders.Add($"{Path.GetRelativePath(srcDir, file)}:{i + 1}");
            }
        }

        Assert.True(offenders.Count == 0,
            "A best-effort JS call catches JSDisconnectedException but not JSException, so a browser " +
            "running a stale script takes the render down instead of losing one enhancement. Add " +
            $"'catch (JSException) {{ }}' directly after it. Offenders:\n  {string.Join("\n  ", offenders)}");
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
