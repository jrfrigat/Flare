using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A strip-only FlareDocumentTabs renders the document tabs without a panel region, so it can serve as
/// <see cref="FlareIdeLayout.CenterDockHeader"/> while the layout renders the selected document.
/// Selection, close, context menu, reorder and Dockable drag stay available.
/// </summary>
public class IdeDocumentTabsStripOnlyTests : FlareTestContext
{
    public IdeDocumentTabsStripOnlyTests() => Services.AddFlareIde();

    private static RenderFragment Tabs(params (string Key, string Title)[] tabs) => builder =>
    {
        var sequence = 0;
        foreach (var (key, title) in tabs)
        {
            builder.OpenComponent<FlareDocumentTab>(sequence++);
            builder.AddAttribute(sequence++, nameof(FlareDocumentTab.Key), key);
            builder.AddAttribute(sequence++, nameof(FlareDocumentTab.Title), title);
            builder.CloseComponent();
        }
    };

    private static string[] Selected(IRenderedComponent<FlareDocumentTabs> cut) =>
        cut.FindAll("[role=tab]").Select(tab => tab.GetAttribute("aria-selected")!).ToArray();

    [Fact]
    public void StripOnly_RendersTabsWithoutAPanelRegion()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(2, cut.FindAll("[role=tab]").Count);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.DocTabs.Panels}"));
        Assert.Empty(cut.FindAll("[role=tabpanel]"));
        Assert.All(cut.FindAll("[role=tab]"), tab => Assert.False(tab.HasAttribute("aria-controls")));
    }

    [Fact]
    public async Task StripOnly_KeepsControlledSelectionAndReportsClicks()
    {
        string? reported = null;
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ActiveKey, "b")
            .Add(x => x.ActiveKeyChanged, key => reported = key)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(["false", "true"], Selected(cut));

        await cut.FindAll("[role=tab]")[0].ClickAsync(new MouseEventArgs());

        Assert.Equal("a", reported);
        Assert.Equal(["false", "true"], Selected(cut));

        cut.Render(p => p.Add(x => x.ActiveKey, "a"));

        Assert.Equal(["true", "false"], Selected(cut));
    }

    [Fact]
    public async Task StripOnly_KeepsCloseAndContextMenu()
    {
        string? closed = null;
        RenderFragment<string> menu = key => builder => builder.AddContent(0, $"Actions for {key}");
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.OnCloseRequested, key => closed = key)
            .Add(x => x.ContextMenuContent, menu)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        await cut.FindAll($".{Css.Classes.Ide.DocTabs.Close}")[1].ClickAsync(new MouseEventArgs());
        Assert.Equal("b", closed);

        var activators = cut.FindAll($".{Css.Classes.Menu.Activator}");
        await activators[0].TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 4, ClientY = 6, Detail = 1 });
        Assert.Contains("Actions for a", cut.Markup);
    }

    [Fact]
    public async Task StripOnly_KeepsReorder()
    {
        IReadOnlyList<string>? order = null;
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.Reorderable, true)
            .Add(x => x.TabOrderChanged, value => order = value)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        await cut.FindAll("[role=tab]")[0].TriggerEventAsync("ondragstart", new DragEventArgs());
        await cut.FindAll("[role=tab]")[1].TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.Equal(["b", "a"], order);
    }

    [Fact]
    public void StripOnly_KeepsDockableDragSources()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.Dockable, true)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"))));

        var tab = cut.Find("[role=tab]");
        Assert.Equal("true", tab.GetAttribute("draggable"));
        Assert.Equal("a", tab.GetAttribute("data-flare-doc-key"));
    }

    [Fact]
    public void Layout_CenterDockHeaderUsesTheStripOnlyMode()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs"), new("b", "B.cs", IdeDocumentRegion.Right)])
            .Add(x => x.ActiveDocuments, Selection("a"))
            .Add(x => x.CenterDockHeader, builder =>
            {
                builder.OpenComponent<FlareDocumentTabs>(0);
                builder.AddAttribute(1, nameof(FlareDocumentTabs.StripOnly), true);
                builder.AddAttribute(2, nameof(FlareDocumentTabs.ActiveKey), "a");
                builder.AddAttribute(3, nameof(FlareDocumentTabs.ChildContent), Tabs(("a", "A.cs")));
                builder.CloseComponent();
            })
            .Add(x => x.DocumentContent, document => builder => builder.AddContent(0, $"Doc {document.Key}")));

        var header = cut.Find($".{Css.Classes.Ide.Layout.CenterHeader}");
        Assert.Single(header.QuerySelectorAll($".{Css.Classes.Ide.DocTabs.Root}"));
        Assert.Empty(header.QuerySelectorAll($".{Css.Classes.Ide.DocTabs.Panels}"));

        var document = cut.Find($".{Css.Classes.Ide.Layout.Document}[data-region=\"Center\"]");
        Assert.False(document.HasAttribute("hidden"));
        Assert.Contains("Doc a", document.TextContent);
    }

    [Fact]
    public void Layout_NullCenterSelectionReturnsToMainContent()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.ActiveDocuments, Selection("a"))
            .Add(x => x.CenterDockHeader, builder =>
            {
                builder.OpenComponent<FlareDocumentTabs>(0);
                builder.AddAttribute(1, nameof(FlareDocumentTabs.StripOnly), true);
                builder.AddAttribute(2, nameof(FlareDocumentTabs.ActiveKey), "a");
                builder.AddAttribute(3, nameof(FlareDocumentTabs.ChildContent), Tabs(("a", "A.cs")));
                builder.CloseComponent();
            })
            .Add(x => x.ChildContent, builder => builder.AddContent(0, "Main content")));

        cut.Render(p => p.Add(x => x.ActiveDocuments, Selection(null)));

        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.Main}").HasAttribute("inert"));
        Assert.True(cut.Find($".{Css.Classes.Ide.Layout.Document}[data-region=\"Center\"]").HasAttribute("hidden"));
    }

    [Fact]
    public void DefaultMode_StillRendersThePanelRegion()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"))));

        Assert.Single(cut.FindAll($".{Css.Classes.Ide.DocTabs.Panels}"));
        Assert.Single(cut.FindAll("[role=tabpanel]"));
        Assert.NotNull(cut.Find("[role=tab]").GetAttribute("aria-controls"));
    }

    private static Dictionary<IdeDocumentRegion, string?> Selection(string? center) =>
        new() { [IdeDocumentRegion.Center] = center };
}
