using Flare.Theming;
using Flare.Abstractions.Tokens;

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
        Assert.Contains(Css.Classes.Theme.ModeDark, js);
        Assert.Contains($"{Css.Classes.Theme.ThemePrefix}'+t", js);
        Assert.Contains($"{Css.Classes.Theme.PalettePrefix}'+p", js);
    }

    /// <summary>
    /// A saved derived theme is styled by its base theme's stylesheets, which answer only to the base's
    /// class; without it the first frame paints unstyled until .NET starts.
    /// </summary>
    [Fact]
    public void GenerateScript_AddsTheStyleFamilyClassOfADerivedTheme()
    {
        var js = FlareBootstrap.GenerateScript(
            ["md3-expressive", "brand"], ["p"], "md3-expressive", "p", ThemeMode.Auto,
            new Dictionary<string, string> { ["brand"] = "md3-expressive", ["md3-expressive"] = "md3-expressive" });

        Assert.Contains("var F={'brand':'md3-expressive'};", js);
        Assert.Contains($"d.classList.add('{Css.Classes.Theme.ThemePrefix}'+F[t])", js);
    }

    [Fact]
    public void GenerateScript_WithoutFamilies_EmitsNoLookup()
    {
        var js = FlareBootstrap.GenerateScript(["t"], ["p"], "t", "p", ThemeMode.Auto, styleFamilies: null);
        Assert.DoesNotContain("var F=", js);
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
