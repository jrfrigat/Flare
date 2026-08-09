namespace Flare.Core.Tests;

/// <summary>
/// Guards the state-layer model against sliding back into the one it replaced.
///
/// Core stylesheets used to compute an interaction state themselves:
/// <code>background: color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-hover-opacity) * 100%), transparent)</code>
/// which is the core deciding what hover MEANS - an MD3-shaped translucent wash of the surface's
/// content colour - on behalf of every theme. A design language that signals hover with a discrete
/// neutral fill could only get its own look by overriding the whole rule with <c>!important</c>, and
/// every such override was a place where the theme and the core disagreed about who owns the paint.
///
/// The replacement is a <c>::before</c> layer painted from <c>--flare-state-hover-layer</c> (and its
/// focus / pressed / focus-hover siblings): the core says where the paint goes, the theme says what it
/// is. It takes a gradient as readily as a colour, which is how the Aero theme expresses a hover the
/// old model could not represent at all.
///
/// This test exists because the old form is the kind of thing that comes back by imitation - the next
/// person adding a hover copies the rule next to it. Nothing else would catch it: the CSS is valid,
/// CssAudit sees only well-formed token names, and the component renders correctly under the theme the
/// literal was written for.
/// </summary>
public sealed class StateLayerModelTests
{
    /// <summary>
    /// The two files still on the old model, and why. Both paint their hover on table CELLS, and a
    /// cell holds whatever the consumer put in it - usually a bare text node, which cannot be lifted
    /// above an opaque layer. The under-the-content placement used everywhere else needs a stacking
    /// context per cell, which would change how a popover rendered inside a cell stacks against its
    /// neighbours. Tracked in docs/issues/core-state-model-tokenization-p1-p2.md.
    ///
    /// Shrink this list, never grow it: an addition means a component went the wrong way.
    /// </summary>
    private static readonly string[] KnownHoldouts = ["datagrid.css", "table.css"];

    [Fact]
    public void NoCoreStylesheet_ComputesAStateItselfFromTheOpacityScale()
    {
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f))
        {
            var name = Path.GetFileName(file);
            if (KnownHoldouts.Contains(name)) continue;

            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("--flare-state-hover-opacity", StringComparison.Ordinal))
                    offenders.Add($"{name}:{i + 1}  {lines[i].Trim()}");
            }
        }

        Assert.True(offenders.Count == 0,
            "A core stylesheet is mixing an interaction state from --flare-state-hover-opacity again. "
            + "That hard-codes one design language's idea of hover into the core and forces every other "
            + "theme to override the rule. Paint a ::before layer from --flare-state-hover-layer instead "
            + "- see tabs.css for the placement and the reasoning:\n  "
            + string.Join("\n  ", offenders));
    }

    [Fact]
    public void EveryHoldout_IsStillReal()
    {
        // A stale allowlist is worse than none: it would silently excuse a file that was converted and
        // then regressed. If a holdout no longer uses the old form, delete it from the list.
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

        foreach (var name in KnownHoldouts)
        {
            var path = Path.Combine(cssDir, name);
            Assert.True(File.Exists(path), $"{name} is on the state-model allowlist but no longer exists.");
            Assert.Contains("--flare-state-hover-opacity", File.ReadAllText(path), StringComparison.Ordinal);
        }
    }

    [Fact]
    public void NoInBoxTheme_ForcesOpacityToUndoACoreFade()
    {
        // The other half of the same story: where the core faded a disabled control on every theme's
        // behalf, a language that repaints instead had to force its way back to opaque. Each of those
        // is now a per-component DisabledOpacity token, so not one `opacity: 1 !important` should be
        // left in any in-box theme.
        var root = FindRepoRoot();
        var offenders = new List<string>();

        foreach (var themeDir in Directory.EnumerateDirectories(Path.Combine(root, "src"), "Flare.Theme.*"))
        {
            var cssRoot = Path.Combine(themeDir, "wwwroot", "css");
            if (!Directory.Exists(cssRoot)) continue;

            foreach (var file in Directory.EnumerateFiles(cssRoot, "*.css", SearchOption.AllDirectories))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Replace(" ", "", StringComparison.Ordinal);
                    if (line.Contains("opacity:1!important", StringComparison.OrdinalIgnoreCase))
                        offenders.Add($"{Path.GetFileName(themeDir)}/{Path.GetFileName(file)}:{i + 1}");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "An in-box theme is forcing opacity back to 1 to undo a fade the core applied. Give the "
            + "component a DisabledOpacity token and let the theme set it, the way every other "
            + "component now does:\n  " + string.Join("\n  ", offenders));
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
