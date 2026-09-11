using Flare.Abstractions;
using System.Text.RegularExpressions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Every theme stylesheet is scoped to the class its theme id produces. A theme derived from another
/// only re-values tokens and keeps the base theme's stylesheets, so it has to keep answering to the
/// base theme's class as well - otherwise deriving a theme silently renders it unstyled.
/// </summary>
public sealed class ThemeStyleFamilyTests
{
    [Fact]
    public void AThemeThatShipsItsOwnStylesheetsIsItsOwnFamily()
    {
        ITheme theme = new MaterialDesign3ExpressiveTheme();
        Assert.Equal(theme.Id, theme.StyleFamilyId);
        Assert.Equal(Css.Classes.Theme.ForTheme(theme.Id),
            Css.Classes.Theme.ForTheme(theme.Id, theme.StyleFamilyId));
    }

    [Fact]
    public void ADerivedThemeKeepsTheFamilyThatStylesIt()
    {
        ITheme baseTheme = new MaterialDesign3ExpressiveTheme();
        var derived = baseTheme.Derive("md3-expressive-brand", displayName: "Brand");

        Assert.Equal("md3-expressive-brand", derived.Id);
        Assert.Equal(baseTheme.Id, derived.StyleFamilyId);
        Assert.Equal(baseTheme.StyleAssets, derived.StyleAssets);
    }

    [Fact]
    public void ADerivedThemeCarriesBothClasses()
    {
        var derived = new MaterialDesign3ExpressiveTheme().Derive("md3-expressive-brand");
        var classes = Css.Classes.Theme.ForTheme(derived.Id, derived.StyleFamilyId).Split(' ');

        Assert.Contains("flare-theme-md3-expressive-brand", classes);   // selectable as itself
        Assert.Contains("flare-theme-md3-expressive", classes);         // and styled by its family
    }

    [Fact]
    public void ADerivedThemeThatBringsItsOwnStylesheetsCanLeaveTheFamily()
    {
        var derived = new MaterialDesign3ExpressiveTheme()
            .Derive("standalone", styleAssets: ["_content/Acme/css/all.css"], styleFamilyId: "standalone");

        Assert.Equal("standalone", derived.StyleFamilyId);
        Assert.Equal(Css.Classes.Theme.ForTheme("standalone"),
            Css.Classes.Theme.ForTheme(derived.Id, derived.StyleFamilyId));
    }

    [Fact]
    public void ABuiltThemeDefaultsToItsOwnFamilyAndCanBeGivenAnother()
    {
        var design = new MaterialDesign3ExpressiveTheme().Design;

        var own = new FlareThemeBuilder("acme", "Acme", design)
            .WithStyleAssets(["_content/Acme/css/acme.css"]).BuildUnsafe();
        Assert.Equal("acme", own.StyleFamilyId);

        var adopted = new FlareThemeBuilder("acme-dark", "Acme Dark", design)
            .WithStyleAssets(["_content/Acme/css/acme.css"])
            .WithStyleFamily("acme").BuildUnsafe();
        Assert.Equal("acme", adopted.StyleFamilyId);
    }

    [Fact]
    public void ExportAndImportKeepTheFamilyAndTheModules()
    {
        var original = new FlareThemeBuilder("acme-dark", "Acme Dark", new MaterialDesign3ExpressiveTheme().Design)
            .WithStyleAssets(["_content/Acme/css/acme.css"])
            .WithScriptAssets(["_content/Acme/js/acme.js"])
            .WithStyleFamily("acme")
            .BuildUnsafe();

        var restored = ThemeJsonSerializer.ImportTheme(ThemeJsonSerializer.ExportTheme(original));

        Assert.Equal(original.Id, restored.Id);
        Assert.Equal(original.StyleFamilyId, restored.StyleFamilyId);
        Assert.Equal(original.StyleAssets, restored.StyleAssets);
        Assert.Equal(original.ScriptAssets, restored.ScriptAssets);
    }

    [Fact]
    public void AnExportWrittenBeforeFamiliesExistedImportsAsItsOwnFamily()
    {
        var current = ThemeJsonSerializer.ExportTheme(
            new FlareThemeBuilder("legacy", "Legacy", new MaterialDesign3ExpressiveTheme().Design)
                .WithStyleAssets(["_content/Legacy/css/legacy.css"]).BuildUnsafe());

        // Strip the two fields such a file predates, leaving a document an older Flare would have written.
        var legacy = Regex.Replace(current, @"\s*""(styleFamilyId|scriptAssets)"":[^,]*,", string.Empty);
        Assert.DoesNotContain("styleFamilyId", legacy);

        var restored = ThemeJsonSerializer.ImportTheme(legacy);

        Assert.Equal("legacy", restored.StyleFamilyId);
        Assert.Empty(restored.ScriptAssets);
    }
}
