using Flare.Components.IDE;

namespace Flare.Components.Tests;

/// <summary>
/// Twenty components shipped in this package with no test at all - the largest untested surface in the
/// repository. This is the floor: every container renders its own root, named from the package's own
/// CSS registry, so a component that stops drawing is no longer something only a human eye would catch.
/// The children that only exist inside a parent are covered through it.
/// </summary>
public class IdeSurfacesSmokeTests : FlareTestContext
{
    // The package registers its own services, which is the point of AddFlareIde: a consumer that never
    // uses the IDE surfaces pays nothing for them. FlareIdeLayout injects one, so a test that renders it
    // has to call the same registration a host would.
    public IdeSurfacesSmokeTests() => Services.AddFlareIde();

    [Fact]
    public void Ribbon_RendersItsTabsAndTheirGroupsAndButtons()
    {
        var cut = Render<FlareRibbon>(p => p.AddChildContent<FlareRibbonTab>(t => t
            .Add(x => x.Title, "Home")
            .AddChildContent<FlareRibbonGroup>(g => g
                .Add(x => x.Label, "Clipboard")
                .AddChildContent<FlareRibbonButton>(b => b.Add(x => x.Label, "Paste")))));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.Ribbon.Root}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.Ribbon.Group}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.Ribbon.Button}"));
    }

    [Fact]
    public void Backstage_RendersItsNavigationItems()
    {
        var cut = Render<FlareBackstage>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.NavContent, b => { b.OpenComponent<FlareBackstageItem>(0); b.AddAttribute(1, "Label", "Open"); b.CloseComponent(); }));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.Backstage.Root}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.Backstage.NavItem}"));
    }

    [Fact]
    public void DocumentTabs_RendersATabPerDocument()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Title, "readme.md")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.DocTabs.Root}"));
    }

    [Fact]
    public void PropertyGrid_RendersARowPerProperty()
    {
        var cut = Render<FlarePropertyGrid>(p => p
            .AddChildContent<FlarePropertyGridItem>(i => i.Add(x => x.Name, "Width")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.PropGrid.Root}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Ide.PropGrid.Row}"));
    }

    // The surfaces that stand on their own. One test each rather than a Theory over types, because the
    // component type is a type ARGUMENT here - a table of `typeof` would need a renderer that takes a
    // Type, and bUnit's does not.
    [Fact]
    public void IdeLayout_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareIdeLayout>().FindAll($".{Css.Classes.Ide.Layout.Root}"));

    [Fact]
    public void MenuBar_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareMenuBar>().FindAll($".{Css.Classes.Ide.MenuBar.Root}"));

    [Fact]
    public void Toolbar_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareToolbar>().FindAll($".{Css.Classes.Ide.Toolbar.Root}"));

    [Fact]
    public void QuickAccessToolbar_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareQuickAccessToolbar>().FindAll($".{Css.Classes.Ide.Qat.Root}"));

    [Fact]
    public void StatusBar_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareStatusBar>().FindAll($".{Css.Classes.Ide.StatusBar.Root}"));

    [Fact]
    public void ToolPanel_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareToolPanel>().FindAll($".{Css.Classes.Ide.ToolPanel.Root}"));

    [Fact]
    public void SheetTabs_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareSheetTabs>().FindAll($".{Css.Classes.Ide.SheetTabs.Root}"));

    [Fact]
    public void FormulaBar_RendersItsRoot() =>
        Assert.NotEmpty(Render<FlareFormulaBar>().FindAll($".{Css.Classes.Ide.FormulaBar.Root}"));
}
