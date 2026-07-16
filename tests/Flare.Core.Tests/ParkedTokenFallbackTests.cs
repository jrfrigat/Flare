using System.Text.RegularExpressions;
using Flare.Theme.MaterialDesign3.Tokens;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Guards the "parked token" contract. A theme may set a geometry token to <c>initial</c> to mean "I do not
/// override this - use the component's own per-size default"; <c>CssVarMap</c> emits it verbatim so a theme
/// switch deterministically overwrites the previous theme's value. That works ONLY because <c>initial</c> is
/// the guaranteed-invalid value for a custom property: <c>var(--token, &lt;fallback&gt;)</c> skips it and takes
/// the fallback. If core CSS reads a parked token WITHOUT a fallback, the substitution yields nothing, the
/// whole declaration becomes invalid at computed-value time, and the component silently loses that geometry.
///
/// This shipped: the P3 pass stripped the slider's fallbacks as "dead" (on the premise that every theme emits
/// the token), which collapsed the slider rail to 0px under MaterialDesign3 / MD3-Expressive - the default
/// theme - across v0.2.0, v0.2.1 and v0.3.0. CssAudit compares NAMES, so it cannot see this; only the pairing
/// of "parked value" + "no fallback" reveals it.
/// </summary>
public sealed class ParkedTokenFallbackTests
{
    // MaterialDesign3 is the theme that uses the parked idiom (the others supply real values), so its token
    // set defines the contract to check.
    private static IEnumerable<string> ParkedTokens() =>
        MaterialDesignTokens.Design.FlattenDesign()
            .Where(kv => IsParked(kv.Value))
            .Select(kv => kv.Key);

    private static bool IsParked(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim() == "initial";

    [Fact]
    public void EveryParkedToken_IsReadWithAFallbackInCoreCss()
    {
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        Assert.True(Directory.Exists(cssDir), $"Core CSS folder not found: {cssDir}");

        var parked = ParkedTokens().ToList();
        Assert.NotEmpty(parked); // the idiom must still exist, or this guard is silently vacuous

        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css"))
        {
            var text = File.ReadAllText(file);
            foreach (var token in parked)
            {
                // A read with no fallback is literally `var(--token)` - the closing paren anchors the name,
                // so a longer token that merely starts with the same text cannot match.
                var noFallback = new Regex($@"var\(\s*{Regex.Escape(token)}\s*\)");
                if (noFallback.IsMatch(text))
                    offenders.Add($"{Path.GetFileName(file)}: var({token}) has no fallback, but a theme parks it at 'initial'");
            }
        }

        Assert.True(offenders.Count == 0,
            "A theme parks these tokens at 'initial' (= use the component default), but core CSS reads them "
            + "without a fallback, so they resolve to nothing and the declaration is dropped:\n  "
            + string.Join("\n  ", offenders));
    }

    // Walk up to the folder that contains src/Flare.Components (the test runs from bin/).
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
