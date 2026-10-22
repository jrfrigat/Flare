using Flare.Css.Tokens;
using Flare.Theme.FluentUI2.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Guards.Tests;

/// <summary>
/// TASK-84: an alert's severity is a theme decision, not a rule the core picks. The stylesheet may only
/// paint through the alert's own tokens, and Fluent points its alert at its status containers just as it
/// points its snackbar there - the Material containers stay the default for every other language.
/// </summary>
public class AlertSeverityTokenTests
{
    private static readonly string Css = File.ReadAllText(
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css", "alert.css"));

    [Theory]
    [InlineData("info")]
    [InlineData("success")]
    [InlineData("warning")]
    [InlineData("error")]
    public void Stylesheet_PaintsThroughTheSeverityTokens(string severity)
    {
        Assert.Contains($"var(--flare-alert-{severity}-bg)", Css);
        Assert.Contains($"var(--flare-alert-{severity}-color)", Css);
        Assert.Contains($"var(--flare-alert-{severity}-border)", Css);
    }

    [Fact]
    public void Stylesheet_NamesNoRoleDirectly() =>
        Assert.DoesNotContain("--flare-color-", Css);

    [Fact]
    public void FluentAlert_UsesItsStatusContainers()
    {
        var alert = FluentUI2Tokens.Design.Alert;

        Assert.Equal($"var({Color.SuccessContainer})", alert.SuccessBg);
        Assert.Equal($"var({Color.OnSuccessContainer})", alert.SuccessColor);
        Assert.Equal($"var({Color.WarningContainer})", alert.WarningBg);
        Assert.Equal($"var({Color.OnWarningContainer})", alert.WarningColor);
        Assert.Equal($"var({Color.InfoContainer})", alert.InfoBg);
        Assert.Equal($"var({Color.OnInfoContainer})", alert.InfoColor);
        Assert.Equal($"var({Color.ErrorContainer})", alert.ErrorBg);
    }

    [Fact]
    public void MaterialAlert_KeepsTheMaterialRoleStory()
    {
        var alert = MaterialDesign3Tokens.Design.Alert;

        Assert.Equal($"var({Color.SecondaryContainer})", alert.SuccessBg);
        Assert.Equal($"var({Color.TertiaryContainer})", alert.WarningBg);
        Assert.Equal($"var({Color.PrimaryContainer})", alert.InfoBg);
        Assert.Equal($"var({Color.ErrorContainer})", alert.ErrorBg);
    }

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;

        throw new DirectoryNotFoundException("Could not find the repository root from " + AppContext.BaseDirectory);
    }
}
