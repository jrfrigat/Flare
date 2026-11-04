using Flare.Tools.CssAudit;

namespace Flare.Core.Tests;

public sealed class CssAuditPackageDependencyTests
{
    [Fact]
    public void PackageCssMayUseRegisteredClassesOnlyFromItsDependencies()
    {
        var root = Path.Combine(Path.GetTempPath(), "flare-css-dependencies-" + Guid.NewGuid().ToString("N"));
        void Write(string path, string text)
        {
            var file = Path.Combine(root, path);
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            File.WriteAllText(file, text);
        }
        try
        {
            Write("src/Flare.Components/wwwroot/css/core.css", "");
            Write("src/Flare.Abstractions/Css/Classes/Core.cs", "");
            Write("src/Flare.Components.First/Flare.Components.First.csproj",
                "<Project><ItemGroup><ProjectReference Include='../Flare.Components.Second/Flare.Components.Second.csproj'/></ItemGroup></Project>");
            Write("src/Flare.Components.First/wwwroot/css/first.css", ".flare-second {} .flare-third {} .flare-unknown {}");
            Write("src/Flare.Components.Second/Flare.Components.Second.csproj",
                "<Project><ItemGroup><ProjectReference Include='../Flare.Components.First/Flare.Components.First.csproj'/></ItemGroup></Project>");
            Write("src/Flare.Components.Second/Css/Classes/Second.cs",
                "public static class Second { public const string Root = \"flare-second\"; }");
            Write("src/Flare.Components.Second/wwwroot/css/second.css", ".flare-second {}");
            Write("src/Flare.Components.Third/Css/Classes/Third.cs",
                "public static class Third { public const string Root = \"flare-third\"; }");
            Write("src/Flare.Components.Third/wwwroot/css/third.css", ".flare-third {}");
            var report = CssAudit.RunPackages(root);
            Assert.DoesNotContain(report.ClassesMissingConstant, s => s.Contains("flare-second", StringComparison.Ordinal));
            Assert.Contains(report.ClassesMissingConstant, s => s.Contains("flare-third", StringComparison.Ordinal));
            Assert.Contains(report.ClassesMissingConstant, s => s.Contains("flare-unknown", StringComparison.Ordinal));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, recursive: true); }
    }
}
