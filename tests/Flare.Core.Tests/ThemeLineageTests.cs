using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Every theme stylesheet is scoped to the class its theme id produces. A theme built on another keeps
/// its ancestors' stylesheets, so its root has to answer to the class of every generation - otherwise a
/// theme three generations deep loses the CSS of whichever ancestor a single "family" id did not name.
/// </summary>
public sealed class ThemeLineageTests
{
    private const string BetterCss = "_content/Better/css/better.css";
    private const string GoldCss = "_content/Gold/css/gold.css";

    private static readonly ITheme Md3e = new MaterialDesign3ExpressiveTheme();

    private static (ITheme Better, ITheme Gold) ThreeGenerations()
    {
        var better = Md3e.Derive("md3e-better", styleAssets: [BetterCss], scriptAssets: ["_content/Better/js/better.js"]);
        var gold = better.Derive("md3e-better-gold", styleAssets: [GoldCss]);
        return (better, gold);
    }

    [Fact]
    public void AThemeThatStandsAloneCarriesOnlyItsOwnClass()
    {
        Assert.Null(Md3e.Base);
        Assert.Equal([Md3e.Id], ThemeLineage.Ids(Md3e));
        Assert.Equal(Css.Classes.Theme.ForTheme(Md3e.Id), ThemeLineage.RootClasses(Md3e));
    }

    [Fact]
    public void EachGenerationCarriesTheClassOfEveryThemeItIsBuiltOn()
    {
        var (better, gold) = ThreeGenerations();

        Assert.Equal(["md3e-better-gold", "md3e-better", Md3e.Id], ThemeLineage.Ids(gold));
        Assert.Equal("flare-theme-md3e-better-gold flare-theme-md3e-better flare-theme-md3-expressive",
            ThemeLineage.RootClasses(gold));
        Assert.Equal("flare-theme-md3e-better flare-theme-md3-expressive", ThemeLineage.RootClasses(better));
    }

    /// <summary>
    /// The case a single family id could not express: the middle generation brings stylesheets of its own,
    /// and the grandchild keeps both its grandparent's and its parent's, in the order that lets a later
    /// generation win a tie.
    /// </summary>
    [Fact]
    public void AThemeWithItsOwnStylesheetsKeepsItsAncestorsStylesheetsInFrontOfItsOwn()
    {
        var (better, gold) = ThreeGenerations();

        Assert.Equal([.. Md3e.StyleAssets, BetterCss], better.StyleAssets);
        Assert.Equal([.. Md3e.StyleAssets, BetterCss, GoldCss], gold.StyleAssets);
        Assert.Equal([.. Md3e.ScriptAssets, "_content/Better/js/better.js"], gold.ScriptAssets);
    }

    [Fact]
    public void ListingTheBaseAssetsByHandDoesNotLoadThemTwice()
    {
        var derived = Md3e.Derive("brand", styleAssets: [.. Md3e.StyleAssets, BetterCss]);

        Assert.Equal([.. Md3e.StyleAssets, BetterCss], derived.StyleAssets);
    }

    [Fact]
    public void ADerivedThemeCanReplaceItsBaseStylesheetsAndStillAnswerToTheBaseClass()
    {
        var derived = Md3e.Derive("self-hosted", styleAssets: ["_content/Acme/css/all.css"], inheritStyleAssets: false);

        Assert.Equal(["_content/Acme/css/all.css"], derived.StyleAssets);
        Assert.Equal("flare-theme-self-hosted flare-theme-md3-expressive", ThemeLineage.RootClasses(derived));
    }

    [Fact]
    public void ABuiltThemeTakesItsParentFromWithBaseAndLoadsTheParentsAssetsFirst()
    {
        var own = new FlareThemeBuilder("acme", "Acme", Md3e.Design)
            .WithStyleAsset("_content/Acme/css/acme.css")
            .BuildUnsafe();
        Assert.Null(own.Base);

        var adopted = new FlareThemeBuilder("acme-dark", "Acme Dark", Md3e.Design)
            .WithStyleAsset("_content/Acme/css/acme-dark.css")
            .WithBase(own)
            .BuildUnsafe();

        Assert.Same(own, adopted.Base);
        Assert.Equal(["_content/Acme/css/acme.css", "_content/Acme/css/acme-dark.css"], adopted.StyleAssets);
        Assert.Equal("flare-theme-acme-dark flare-theme-acme", ThemeLineage.RootClasses(adopted));
    }

    [Fact]
    public void TheRootClassesAreBuiltOncePerTheme()
    {
        var (_, gold) = ThreeGenerations();

        Assert.Same(ThemeLineage.RootClasses(gold), ThemeLineage.RootClasses(gold));
        Assert.Same(ThemeLineage.Ids(gold), ThemeLineage.Ids(gold));
    }

    /// <summary>
    /// A derived root also carries its ancestors' classes, so their token blocks match it at the same
    /// specificity; the derived theme's tokens apply only if its block comes last, whatever order the
    /// themes were registered in.
    /// </summary>
    [Fact]
    public void TheTokenBundleEmitsATheme_AfterEveryThemeItIsBuiltOn()
    {
        var (better, gold) = ThreeGenerations();

        var css = TokensToCss.Bundle([gold, better, Md3e], []);

        var md3e = css.IndexOf(".flare-theme-md3-expressive{", StringComparison.Ordinal);
        var middle = css.IndexOf(".flare-theme-md3e-better{", StringComparison.Ordinal);
        var leaf = css.IndexOf(".flare-theme-md3e-better-gold{", StringComparison.Ordinal);
        Assert.True(md3e >= 0 && md3e < middle && middle < leaf, $"order was md3e={md3e}, better={middle}, gold={leaf}");
    }

    [Fact]
    public void AChainThatLoopsBackOnItselfIsAnError()
    {
        var a = new LoopingTheme("a");
        var b = new LoopingTheme("b") { Parent = a };
        a.Parent = b;

        var error = Assert.Throws<InvalidOperationException>(() => ThemeLineage.RootClasses(a));
        Assert.Contains("a -> b -> a", error.Message);
    }

    [Fact]
    public void ExportAndImportKeepTheParentAndTheAssets()
    {
        var (better, gold) = ThreeGenerations();

        var restored = ThemeJsonSerializer.ImportTheme(ThemeJsonSerializer.ExportTheme(gold), [Md3e, better]);

        Assert.Equal(gold.Id, restored.Id);
        Assert.Same(better, restored.Base);
        Assert.Equal(gold.StyleAssets, restored.StyleAssets);
        Assert.Equal(gold.ScriptAssets, restored.ScriptAssets);
        Assert.Equal(ThemeLineage.RootClasses(gold), ThemeLineage.RootClasses(restored));
    }

    [Fact]
    public void ImportingAThemeWhoseParentIsUnknownIsAnError()
    {
        var (_, gold) = ThreeGenerations();
        var json = ThemeJsonSerializer.ExportTheme(gold);

        var error = Assert.Throws<InvalidOperationException>(() => ThemeJsonSerializer.ImportTheme(json, [Md3e]));
        Assert.Contains("'md3e-better'", error.Message);
        Assert.Throws<InvalidOperationException>(() => ThemeJsonSerializer.ImportTheme(json));
    }

    /// <summary>
    /// Exports from 0.39-0.41 named the theme whose stylesheets styled this one as <c>styleFamilyId</c>;
    /// that is the same statement as naming the parent, so it imports as one.
    /// </summary>
    [Fact]
    public void AnExportThatNamesAStyleFamilyImportsWithThatThemeAsItsParent()
    {
        var brand = Md3e.Derive("brand");
        var legacy = ThemeJsonSerializer.ExportTheme(brand).Replace("\"baseId\"", "\"styleFamilyId\"");
        Assert.DoesNotContain("baseId", legacy);

        var restored = ThemeJsonSerializer.ImportTheme(legacy, [Md3e]);

        Assert.Same(Md3e, restored.Base);
    }

    [Fact]
    public void AnExportWhoseStyleFamilyIsItsOwnIdImportsWithoutAParent()
    {
        var own = new FlareThemeBuilder("legacy", "Legacy", Md3e.Design).BuildUnsafe();
        var legacy = ThemeJsonSerializer.ExportTheme(own).Replace("\"id\": \"legacy\",", "\"id\": \"legacy\", \"styleFamilyId\": \"legacy\",");
        Assert.Contains("styleFamilyId", legacy);

        var restored = ThemeJsonSerializer.ImportTheme(legacy);

        Assert.Null(restored.Base);
    }

    private sealed class LoopingTheme(string id) : ITheme
    {
        public ITheme? Parent { get; set; }
        public string Id => id;
        public string DisplayName => id;
        public DesignTokens Design => Md3e.Design;
        public string DefaultPaletteId => Md3e.DefaultPaletteId;
        public IReadOnlyList<string> StyleAssets => [];
        public ITheme? Base => Parent;
    }
}
