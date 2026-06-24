using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Core.Tests;

public class FlareBootstrapTests
{
    [Fact]
    public void GenerateScript_EmbedsIdsDefaultsAndModeLogic()
    {
        var js = FlareBootstrap.GenerateScript(
            ["md3-expressive", "fluent2"], ["md3-violet", "fluent-blue"],
            "md3-expressive", "md3-violet", ThemeMode.Auto);

        Assert.Contains("['md3-expressive','fluent2']", js);
        Assert.Contains("['md3-violet','fluent-blue']", js);
        Assert.Contains("if(T.indexOf(t)<0)t='md3-expressive'", js);
        Assert.Contains("if(P.indexOf(p)<0)p='md3-violet'", js);
        Assert.Contains("var m=s.getItem('flare-mode')||'auto'", js);
        Assert.Contains("prefers-color-scheme: dark", js);
        Assert.Contains("flare-mode-dark", js);
        Assert.Contains("flare-theme-'+t", js);
        Assert.Contains("flare-palette-'+p", js);
    }

    [Fact]
    public void GenerateScript_DefaultModeDark_IsLowercased()
    {
        var js = FlareBootstrap.GenerateScript(["t"], ["p"], "t", "p", ThemeMode.Dark);
        Assert.Contains("||'dark'", js);
    }

    [Fact]
    public void GenerateScript_EscapesQuotesInIds()
    {
        var js = FlareBootstrap.GenerateScript(["a'b"], ["p"], "a'b", "p", ThemeMode.Light);
        Assert.Contains("'a\\'b'", js);
    }
}
