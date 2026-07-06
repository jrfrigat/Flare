using Flare.Tools.CssAudit;

namespace Flare.Core.Tests;

/// <summary>
/// Smoke coverage for the <c>--flare-*</c> token audit (the same scan the <c>cssaudit tokens</c> CLI
/// runs). This is REPORT-ONLY by design: unlike CSS classes, tokens are not yet in full sync, so this
/// test deliberately does NOT assert <see cref="TokenAuditReport.InSync"/> and never fails on drift.
/// It only guarantees the audit executes and returns a well-formed report, so the tool code cannot rot.
/// Use <c>dotnet run --project tools/Flare.CssAudit -- tokens</c> to see the current findings.
/// </summary>
public sealed class TokenAuditTests
{
    [Fact]
    public void TokenAudit_Runs_And_Produces_A_WellFormed_Report()
    {
        var report = CssAudit.RunTokens();

        Assert.NotNull(report);
        Assert.NotNull(report.TokensMissingConstant);
        Assert.NotNull(report.ConstantsMissingCss);
        Assert.NotNull(report.ThemeOnlyTokens);
        // AllFindings is the union of the three buckets; enumerating it must not throw.
        Assert.Equal(
            report.TokensMissingConstant.Count + report.ConstantsMissingCss.Count + report.ThemeOnlyTokens.Count,
            report.AllFindings.Count());
    }
}
