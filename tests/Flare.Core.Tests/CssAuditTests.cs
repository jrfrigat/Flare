using Flare.Tools.CssAudit;

namespace Flare.Core.Tests;

/// <summary>
/// Build-gate for the CSS contract: the same audits the <c>cssaudit</c> CLI runs, executed in-process
/// during <c>dotnet test</c>. One check guards CSS classes, the other guards <c>--flare-*</c> design
/// tokens. Each fails when Flare.Components CSS, the <c>Flare.Css</c> registry and the themes drift apart.
/// </summary>
public sealed class CssAuditTests
{
    [Fact]
    public void CssClasses_Components_And_Themes_StayInSync()
    {
        var report = CssAudit.Run();

        Assert.True(report.InSync,
            "CssClasses / Flare.Components CSS / themes drifted out of sync. " +
            "Run `dotnet run --project tools/Flare.CssAudit -- check` for details. Findings:\n  " +
            string.Join("\n  ", report.AllFindings));
    }

    /// <summary>
    /// The same contract for the optional packages, which the check above cannot see: it reads one
    /// stylesheet folder and one registry, while each of <c>src/Flare.Components.*</c> has its own pair.
    /// This fails when a package styles a class no constant names, keeps a constant nothing styles and
    /// nothing emits, or writes a name out instead of taking it from a registry.
    /// </summary>
    [Fact]
    public void OptionalPackages_CssAndRegistry_StayInSync()
    {
        var report = CssAudit.RunPackages();

        Assert.True(report.Packages.Count >= 9,
            $"Only {report.Packages.Count} optional packages were found, so this guard proves almost nothing.");
        Assert.True(report.InSync,
            "An optional package's CSS, registry and markup drifted apart. " +
            "Run `dotnet run --project tools/Flare.CssAudit -- packages` for details. Findings:\n  " +
            string.Join("\n  ", report.AllFindings));
    }

    /// <summary>
    /// Token counterpart of <see cref="CssClasses_Components_And_Themes_StayInSync"/>. Every token the
    /// component CSS reads has a <c>Css.Tokens</c> constant and every constant is read, so this fails when
    /// a <c>--flare-*</c> token is added to Flare.Components CSS without a <c>Css.Tokens</c> constant
    /// (<c>[T+]</c>), a constant loses its last CSS reference (<c>[T-]</c>), or a theme references a token
    /// that is neither declared nor present in the base CSS (<c>[T~]</c>).
    /// </summary>
    [Fact]
    public void CssTokens_Components_And_Themes_StayInSync()
    {
        var report = CssAudit.RunTokens();

        Assert.True(report.InSync,
            "Css.Tokens / Flare.Components CSS / themes drifted out of sync. " +
            "Run `dotnet run --project tools/Flare.CssAudit -- tokens` for details. Findings:\n  " +
            string.Join("\n  ", report.AllFindings));
    }
}
