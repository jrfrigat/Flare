using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// Enforces the other half of the token mandate: <b>the component CSS carries no default of its own</b>.
///
/// A fallback on a THEME-EMITTED token - <c>var(--flare-btn-gap-xs, 0.25rem)</c> - is a theme opinion
/// hiding in the core. Every token record member is <c>required</c>, so every theme supplies a value and the
/// fallback can never render; it is dead code that quietly re-introduces a default the mandate forbids, and
/// it lets a theme get away with not supplying a value.
///
/// The distinction that matters (and that a blanket sweep gets wrong): a <c>--flare-*</c> var is NOT
/// automatically a theme token. Some are per-instance vars the CONSUMER sets - <c>--flare-col-span</c> on a
/// grid cell, <c>--flare-slider-length</c> on a vertical slider, the <c>--flare-ide-*</c> pane sizes. Those
/// are never emitted by a theme, so their fallback is the live default and must stay. This guard therefore
/// keys off the theme-emitted name set rather than the <c>--flare-</c> prefix.
///
/// History: the 0.2.0 sweep stripped fallbacks on the premise "every theme emits the token, so the fallback
/// is dead". That premise was false while themes could park a token at <c>initial</c>, and it shipped broken
/// geometry for three releases (see <see cref="ParkedTokenFallbackTests"/>). Parking is now banned and
/// guarded, which is what finally makes this rule safe to enforce.
/// </summary>
public sealed class DeadFallbackTests
{
    // var(--flare-x)            -> no fallback, fine.
    // var(--flare-x, <stuff>)   -> captures the name and the fallback text.
    private static readonly Regex _varWithFallback = new(@"var\(\s*(--flare-[a-z0-9-]+)\s*,([^;]*)", RegexOptions.Compiled);

    [Fact]
    public void CoreCss_HasNoFallbackOnATokenTheThemeSupplies()
    {
        var emitted = TokenParityTests.ThemeEmittedTokenNames();
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        Assert.True(Directory.Exists(cssDir), $"Core CSS folder not found: {cssDir}");

        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f, StringComparer.Ordinal))
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                foreach (Match m in _varWithFallback.Matches(lines[i]))
                {
                    var name = m.Groups[1].Value;
                    if (!emitted.Contains(name)) continue; // a per-instance var: its fallback is the real default
                    offenders.Add($"{Path.GetFileName(file)}:{i + 1}  var({name}, ...) - the theme always supplies this");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "These fallbacks sit on tokens every theme supplies, so they can never render - they are dead "
            + "code that hides a theme default inside the core. Drop the fallback and let the theme's value "
            + "come through:\n  "
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
