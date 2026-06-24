using Flare.Tools.CssAudit;

namespace Flare.Core.Tests;

/// <summary>
/// Build-gate for the CSS contract: the same audit the <c>cssaudit</c> CLI runs, executed in-process
/// during <c>dotnet test</c>. Fails when a class is added to Flare.Components CSS without a
/// <c>Flare.Css.Classes</c> constant, a constant loses its CSS rule, or a theme introduces a class
/// the base does not define.
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
}
