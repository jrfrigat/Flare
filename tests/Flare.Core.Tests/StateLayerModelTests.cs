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
    /// There is no allowlist. The last two entries on it were table.css and datagrid.css, whose row
    /// hover painted on the CELLS - and a cell holds whatever the consumer put there, usually a bare
    /// text node that cannot be lifted above an opaque layer. The answer was not the per-cell stacking
    /// context that had been assumed and rejected (it would have trapped a popover opened from inside
    /// a cell, including Flare's own inline-edit dropdowns): the paint moved up to the ROW, where
    /// there is one background, one <c>currentColor</c>, and nothing to isolate. The frozen column is
    /// the single cell that still needs a layer of its own, and being sticky it was already a
    /// stacking context.
    /// </summary>
    [Fact]
    public void NoCoreStylesheet_ComputesAStateItselfFromTheOpacityScale()
    {
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f))
        {
            var name = Path.GetFileName(file);
            var lines = CssLinesWithoutComments(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(Css.Tokens.State.HoverOpacity, StringComparison.Ordinal))
                    offenders.Add($"{name}:{i + 1}  {lines[i].Trim()}");
            }
        }

        Assert.True(offenders.Count == 0,
            $"A core stylesheet is mixing an interaction state from {Css.Tokens.State.HoverOpacity} again. "
            + "That hard-codes one design language's idea of hover into the core and forces every other "
            + $"theme to override the rule. Paint a ::before layer from {Css.Tokens.State.HoverLayer} instead "
            + "- see tabs.css for the placement and the reasoning:\n  "
            + string.Join("\n  ", offenders));
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
                var lines = CssLinesWithoutComments(file);
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

    [Fact]
    public void NoInBoxTheme_SuppressesAStateLayerInsteadOfNamingItsPaint()
    {
        // The mirror image of the rule above, and the one that survived longest unnoticed: a theme
        // whose own gloss IS the feedback used to switch the core's layer off with
        // `opacity: 0 !important` on the ::before. That reads as the theme fighting the core, and it
        // is indiscriminate - the same declaration outranked the DISABLED layer, so it decided that
        // too. Setting the -layer tokens to transparent says the same thing through the contract.
        var root = FindRepoRoot();
        var offenders = new List<string>();

        foreach (var themeDir in Directory.EnumerateDirectories(Path.Combine(root, "src"), "Flare.Theme.*"))
        {
            var cssRoot = Path.Combine(themeDir, "wwwroot", "css");
            if (!Directory.Exists(cssRoot)) continue;

            foreach (var file in Directory.EnumerateFiles(cssRoot, "*.css", SearchOption.AllDirectories))
            {
                var lines = CssLinesWithoutComments(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Replace(" ", "", StringComparison.Ordinal);
                    if (line.Contains("opacity:0!important", StringComparison.OrdinalIgnoreCase))
                        offenders.Add($"{Path.GetFileName(themeDir)}/{Path.GetFileName(file)}:{i + 1}");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "An in-box theme is suppressing a state layer with `opacity: 0 !important` instead of "
            + "naming its paint. Set the --flare-state-*-layer tokens to transparent on the component "
            + "instead - same result, and the layer stays retunable:\n  " + string.Join("\n  ", offenders));
    }

    /// <summary>
    /// Returns the stylesheet's lines with every comment blanked out, newlines preserved so a reported
    /// line number still points at the real line.
    /// </summary>
    /// <remarks>
    /// Without this, a guard cannot tell a rule from prose about a rule - and the paragraph explaining
    /// why the old form was wrong has to quote it verbatim to be worth reading. Both of the themes fixed
    /// here, and FluentUI2's write-up of the same conversion, were reported as offenders by their own
    /// documentation. The alternative (forbid the words) would make the codebase unable to describe its
    /// own history.
    /// </remarks>
    private static string[] CssLinesWithoutComments(string path)
    {
        var css = File.ReadAllText(path);
        var stripped = new System.Text.StringBuilder(css.Length);
        var inComment = false;

        for (var i = 0; i < css.Length; i++)
        {
            if (!inComment && css[i] == '/' && i + 1 < css.Length && css[i + 1] == '*')
            {
                inComment = true;
                stripped.Append("  ");
                i++;
                continue;
            }

            if (inComment && css[i] == '*' && i + 1 < css.Length && css[i + 1] == '/')
            {
                inComment = false;
                stripped.Append("  ");
                i++;
                continue;
            }

            // Keep line breaks even inside a comment so the line numbering survives.
            stripped.Append(inComment && css[i] is not ('\n' or '\r') ? ' ' : css[i]);
        }

        return stripped.ToString().Split('\n');
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
