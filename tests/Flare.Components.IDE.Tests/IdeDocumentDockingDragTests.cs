using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A tab dragged from FlareDocumentTabs reaches FlareIdeLayout through the IDE module, which carries
/// the document key because Blazor cannot read DataTransfer. The move menu is the keyboard alternative.
/// </summary>
public class IdeDocumentDockingDragTests : FlareTestContext
{
    private const string ModulePath = "./_content/Flare.Components.IDE/js/flare-ide.js";

    public IdeDocumentDockingDragTests() => Services.AddFlareIde();

    // A side region with no documents draws no dock strip, so its tool body is the drop target.
    private static string SideBodySelector(string sideClass) =>
        $".{Css.Classes.Ide.Layout.Panel}.{sideClass} .{Css.Classes.Ide.Layout.PanelBody}";

    private static RenderFragment OneTab(string key = "a", string title = "A.cs") => builder =>
    {
        builder.OpenComponent<FlareDocumentTab>(0);
        builder.AddAttribute(1, nameof(FlareDocumentTab.Key), key);
        builder.AddAttribute(2, nameof(FlareDocumentTab.Title), title);
        builder.CloseComponent();
    };

    [Fact]
    public void Dockable_MarksTabsAsDragSourcesEvenWithoutReorder()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.Dockable, true)
            .AddChildContent(OneTab()));

        var tab = cut.Find("[role=tab]");
        Assert.Equal("true", tab.GetAttribute("draggable"));
        Assert.Equal("a", tab.GetAttribute("data-flare-doc-key"));
    }

    [Fact]
    public void PlainStrip_IsNotADragSource()
    {
        var cut = Render<FlareDocumentTabs>(p => p.AddChildContent(OneTab()));

        var tab = cut.Find("[role=tab]");
        Assert.Null(tab.GetAttribute("draggable"));
        Assert.Null(tab.GetAttribute("data-flare-doc-key"));
    }

    [Fact]
    public async Task MoveMenu_ReportsTheDocumentAndChosenTarget()
    {
        FlareDocumentDockRequest? request = null;
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.DockTargets,
            [
                new FlareDocumentDockTarget("left", "Left"),
                new FlareDocumentDockTarget("right", "Right")
            ])
            .Add(x => x.OnDockRequested,
                EventCallback.Factory.Create<FlareDocumentDockRequest>(this, value => request = value))
            .AddChildContent(OneTab()));

        await cut.Find($".{Css.Classes.Ide.DocTabs.Move} button").ClickAsync(new());
        var items = cut.FindAll("[role=menuitem]");
        Assert.Equal(["Left", "Right"], items.Select(item => item.TextContent.Trim()));
        await items[1].ClickAsync(new());

        Assert.Equal("a", request?.DocumentKey);
        Assert.Equal("right", request?.Target.Key);
    }

    [Fact]
    public void MoveMenu_NeedsAHandler()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.DockTargets, [new FlareDocumentDockTarget("left", "Left")])
            .AddChildContent(OneTab()));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.DocTabs.Move}"));
    }

    [Fact]
    public async Task Layout_ReadsAnExternalDropThroughTheModule()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupVoid("registerDocumentDragSource");
        module.Setup<string?>("takeDroppedDocument", _ => true).SetResult("a");
        IdeDocumentMove? moved = null;

        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs"), new("b", "B.cs", IdeDocumentRegion.Right)])
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value)));

        cut.WaitForAssertion(() => module.VerifyInvoke("registerDocumentDragSource"));
        var target = cut.Find(SideBodySelector(Css.Classes.Ide.Layout.PanelLeft));
        await target.TriggerEventAsync("ondrop", new DragEventArgs());
        cut.WaitForAssertion(() => Assert.NotNull(moved));

        Assert.Equal("a", moved!.Document.Key);
        Assert.Equal(IdeDocumentRegion.Left, moved.Destination);
    }

    [Fact]
    public async Task Layout_IgnoresAnExternalKeyThatIsNotADocument()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupVoid("registerDocumentDragSource");
        module.Setup<string?>("takeDroppedDocument", _ => true).SetResult("missing");
        IdeDocumentMove? moved = null;

        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value)));

        cut.WaitForAssertion(() => module.VerifyInvoke("registerDocumentDragSource"));
        var target = cut.Find(SideBodySelector(Css.Classes.Ide.Layout.PanelLeft));
        await target.TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.Null(moved);
    }

    [Fact]
    public async Task Layout_StillMovesItsOwnDockTabWithoutTheModule()
    {
        IdeDocumentMove? moved = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value)));

        var select = cut.Find($".{Css.Classes.Ide.Layout.DockSelect}");
        await select.TriggerEventAsync("ondragstart", new DragEventArgs());
        var target = cut.Find(SideBodySelector(Css.Classes.Ide.Layout.PanelLeft));
        await target.TriggerEventAsync("ondrop", new DragEventArgs());
        cut.WaitForAssertion(() => Assert.NotNull(moved));

        Assert.Equal("a", moved!.Document.Key);
        Assert.Equal(IdeDocumentRegion.Left, moved.Destination);
    }

    [Fact]
    public void Layouts_HaveDistinctDockScopes()
    {
        var first = Render<FlareIdeLayout>();
        var second = Render<FlareIdeLayout>();

        string? Scope(AngleSharp.Dom.IElement root) => root.GetAttribute("data-flare-ide-dock-scope");

        var firstScope = Scope(first.Find($".{Css.Classes.Ide.Layout.Root}"));
        var secondScope = Scope(second.Find($".{Css.Classes.Ide.Layout.Root}"));

        Assert.False(string.IsNullOrEmpty(firstScope));
        Assert.False(string.IsNullOrEmpty(secondScope));
        Assert.NotEqual(firstScope, secondScope);
    }
}
