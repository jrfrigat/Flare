using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The built-in dock strip exposes the tab actions an IDE host already has: a close request, a
/// host-supplied context menu and the unsaved marker. Close is a request, so the host keeps control
/// of prompts and removal.
/// </summary>
public class IdeLayoutDockTabTests : FlareTestContext
{
    public IdeLayoutDockTabTests() => Services.AddFlareIde();

    private IRenderedComponent<FlareIdeLayout> RenderDock(
        IReadOnlyList<IdeDocument> documents,
        Action<IdeDocument>? close = null,
        RenderFragment<IdeDocument>? contextMenu = null) =>
        Render<FlareIdeLayout>(p =>
        {
            p.Add(x => x.Documents, documents);
            if (close is not null)
                p.Add(x => x.OnDocumentCloseRequested, EventCallback.Factory.Create<IdeDocument>(this, close));
            if (contextMenu is not null) p.Add(x => x.DocumentContextMenu, contextMenu);
        });

    [Fact]
    public void CloseButton_RendersOnlyForCloseableDocumentsWithAHandler()
    {
        var withoutHandler = RenderDock([new("a", "Main.cs")]);
        Assert.Empty(withoutHandler.FindAll($".{Css.Classes.Ide.Layout.DockClose}"));

        var closeable = RenderDock([new("a", "Main.cs"), new("b", "Guide.md", Closeable: false)], _ => { });
        var close = Assert.Single(closeable.FindAll($".{Css.Classes.Ide.Layout.DockClose}"));
        Assert.Equal("Close tab: Main.cs", close.GetAttribute("aria-label"));
    }

    [Fact]
    public async Task CloseButton_ReportsTheDocumentWithoutSelectingIt()
    {
        IdeDocument? requested = null;
        var cut = RenderDock(
            [new("a", "Main.cs"), new("b", "Guide.md")],
            document => requested = document);

        await cut.FindAll($".{Css.Classes.Ide.Layout.DockClose}")[1].ClickAsync(new MouseEventArgs());

        Assert.Equal("b", requested?.Key);
        Assert.Equal("true", cut.FindAll($".{Css.Classes.Ide.Layout.DockSelect}")[0].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void ModifiedMarker_IsPaintedAndAnnouncedOnTheSelectButton()
    {
        var cut = RenderDock([new("a", "Main.cs", Modified: true), new("b", "Guide.md")]);
        var select = cut.FindAll($".{Css.Classes.Ide.Layout.DockSelect}")[0];

        Assert.Equal("Main.cs (Modified)", select.GetAttribute("aria-label"));
        Assert.Single(select.QuerySelectorAll($".{Css.Classes.Ide.Layout.DockModified}"));
        Assert.Equal("Guide.md", cut.FindAll($".{Css.Classes.Ide.Layout.DockSelect}")[1].GetAttribute("aria-label"));
    }

    [Fact]
    public async Task RightClick_OpensTheHostContextMenuForThatDocument()
    {
        var cut = RenderDock(
            [new("a", "Main.cs"), new("b", "Guide.md")],
            contextMenu: document => builder => builder.AddContent(0, $"Actions for {document.Key}"));

        var activator = cut.FindAll($".{Css.Classes.Menu.Activator}")[1];
        await activator.TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 8, ClientY = 12, Detail = 1 });

        Assert.Contains("Actions for b", cut.Markup);
    }
}
