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

    // --flare-x: <value>   at the start of a declaration -- the core DECLARING a token, not reading one.
    private static readonly Regex _tokenDeclaration =
        new(@"(^|[;{])\s*(--flare-[a-z0-9-]+)\s*:\s*([^;}]*)", RegexOptions.Compiled);

    // A literal dimension in the value. The mandate draws the line exactly here: pointing a token at
    // another SEMANTIC token is allowed, a hardcoded 16px is not.
    private static readonly Regex _literalDimension =
        new(@"(^|[\s(,])-?\d+(\.\d+)?(px|rem|em|ch|vh|vw)\b", RegexOptions.Compiled);

    // The debt this guard found on its first run, named rather than silently tolerated. It is a ratchet:
    // every other core stylesheet is clean and must stay clean; these come out one at a time, and a file
    // leaves this list for good once it does.
    //   switch.css    - a size class hardcodes track/thumb geometry over the theme's. Same shape badge.css
    //                   had before it got its per-size ramp in BadgeTokens; the fix is the same.
    //   menuitem.css  - hardcodes the item's padding and gap over MenuTokens.
    //   datagrid.css  - the filter editors re-declare what .flare-input-variant--outlined already says;
    //                   they should wear that class instead of copying it.
    //   input.css     - the variant classes (.flare-input-variant--filled/--outlined) hardcode 1px borders.
    //                   Arguable rather than clearly wrong: the variant exists to define a look INDEPENDENT
    //                   of the theme, and a 1px edge is part of what "outlined" means. Decide before fixing.
    private static readonly string[] _knownDebt =
        ["switch.css", "menuitem.css", "datagrid.css", "input.css"];

    [Fact]
    public void CoreCss_DoesNotDeclareATokenTheThemeSupplies()
    {
        // The same mandate as the fallback rule above, from the other side. A fallback hides a default
        // inside a read; this hides one inside a WRITE, and it is the more dangerous of the two: a
        // declaration on :root or a component root sits nearer the element than the theme's own
        // .flare-theme-* block, so the core value can win outright and the theme's is simply never seen.
        //
        // History: layout-shell.css opened with a :root block setting --flare-layout-drawer-width and four
        // neighbours to literals. Two of them (-appbar-height-dense, -appbar-bg) had no other source at all -
        // a name in the registry, no record member, nothing emitting them - so no theme could reach them.
        //
        // Setting a LOCAL (--_x: var(--flare-x)) is the correct pattern and is untouched here: this only
        // fires on writing a name a theme emits.
        var emitted = TokenParityTests.ThemeEmittedTokenNames();
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        Assert.True(Directory.Exists(cssDir), $"Core CSS folder not found: {cssDir}");

        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f, StringComparer.Ordinal))
        {
            var name0 = Path.GetFileName(file);
            if (_knownDebt.Contains(name0)) continue;
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                foreach (Match m in _tokenDeclaration.Matches(lines[i]))
                {
                    var name = m.Groups[2].Value;
                    if (!emitted.Contains(name)) continue;                       // a per-instance var the consumer owns
                    if (!_literalDimension.IsMatch(m.Groups[3].Value)) continue; // points at another token: allowed
                    offenders.Add($"{name0}:{i + 1}  {name}: {m.Groups[3].Value.Trim()}");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "The core CSS declares tokens the themes emit. A declaration in core can out-rank the theme's "
            + "own, so the theme's value never renders - and the literal is a default baked into core, which "
            + "the mandate forbids. Move the value into the token records and let the theme supply it:\n  "
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
