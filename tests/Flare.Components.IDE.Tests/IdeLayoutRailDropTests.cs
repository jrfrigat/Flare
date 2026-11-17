using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A collapsed document region keeps its rail as the drop target, so a document does not have to be
/// expanded before it can be docked. The rail highlights during the drag and the bottom rail, which has
/// no document region, stays inert.
/// </summary>
public class IdeLayoutRailDropTests : FlareTestContext
{
    private const string ModulePath = "./_content/Flare.Components.IDE/js/flare-ide.js";

    public IdeLayoutRailDropTests() => Services.AddFlareIde();

    private static string RailSelector(string side) =>
        $".{Css.Classes.Ide.Layout.Panel}.{side} .{Css.Classes.Ide.Layout.Rail}";

    [Fact]
    public async Task LeftRail_AcceptsAnInternalDragAndHighlights()
    {
        IdeDocumentMove? moved = null;
        bool? expanded = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => expanded = value))
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value)));
        var railSelector = RailSelector(Css.Classes.Ide.Layout.PanelLeft);

        await cut.Find($".{Css.Classes.Ide.Layout.DockSelect}").TriggerEventAsync("ondragstart", new DragEventArgs());
        await cut.Find(railSelector).TriggerEventAsync("ondragenter", new DragEventArgs());
        Assert.Contains(Css.Classes.Ide.Layout.RailDropTarget, cut.Find(railSelector).ClassList);

        await cut.Find(railSelector).TriggerEventAsync("ondragleave", new DragEventArgs());
        Assert.DoesNotContain(Css.Classes.Ide.Layout.RailDropTarget, cut.Find(railSelector).ClassList);

        await cut.Find(railSelector).TriggerEventAsync("ondragenter", new DragEventArgs());
        await cut.Find(railSelector).TriggerEventAsync("ondrop", new DragEventArgs());
        cut.WaitForAssertion(() => Assert.NotNull(moved));

        Assert.Equal("a", moved!.Document.Key);
        Assert.Equal(IdeDocumentRegion.Left, moved.Destination);
        Assert.False(expanded);
        Assert.DoesNotContain(Css.Classes.Ide.Layout.PanelCollapsed,
            cut.Find($".{Css.Classes.Ide.Layout.Panel}.{Css.Classes.Ide.Layout.PanelLeft}").ClassList);
        // The host has not moved the document yet, so the left region draws no strip row.
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Left\"]"));
    }

    [Fact]
    public async Task RightRail_ReadsAnExternalDropThroughTheModule()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupVoid("registerDocumentDragSource");
        module.Setup<string?>("takeDroppedDocument").SetResult("a");
        IdeDocumentMove? moved = null;
        bool? expanded = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.RightCollapsed, true)
            .Add(x => x.RightCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => expanded = value))
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value)));

        cut.WaitForAssertion(() => module.VerifyInvoke("registerDocumentDragSource"));
        await cut.Find(RailSelector(Css.Classes.Ide.Layout.PanelRight))
            .TriggerEventAsync("ondrop", new DragEventArgs());
        cut.WaitForAssertion(() => Assert.NotNull(moved));

        Assert.Equal("a", moved!.Document.Key);
        Assert.Equal(IdeDocumentRegion.Right, moved.Destination);
        Assert.False(expanded);
    }

    [Fact]
    public async Task BottomRail_IgnoresTheDragAndDrop()
    {
        IdeDocumentMove? moved = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.BottomCollapsed, true)
            .Add(x => x.BottomPanel, builder =>
            {
                builder.OpenComponent<FlareToolPanel>(0);
                builder.AddAttribute(1, nameof(FlareToolPanel.Position), ToolPanelPosition.Bottom);
                builder.AddAttribute(2, nameof(FlareToolPanel.Title), "Output");
                builder.CloseComponent();
            })
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value)));
        var railSelector = $".{Css.Classes.Ide.Layout.Bottom} .{Css.Classes.Ide.Layout.Rail}";

        await cut.Find($".{Css.Classes.Ide.Layout.DockSelect}").TriggerEventAsync("ondragstart", new DragEventArgs());
        await cut.Find(railSelector).TriggerEventAsync("ondragenter", new DragEventArgs());
        Assert.DoesNotContain(Css.Classes.Ide.Layout.RailDropTarget, cut.Find(railSelector).ClassList);

        await cut.Find(railSelector).TriggerEventAsync("ondrop", new DragEventArgs());
        Assert.Null(moved);
    }

    [Fact]
    public void CollapsedRegion_HidesItsDockStrip()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [])
            .Add(x => x.LeftCollapsed, true));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Left\"]"));
        Assert.NotEmpty(cut.FindAll(RailSelector(Css.Classes.Ide.Layout.PanelLeft)));
    }

    [Fact]
    public async Task Rail_ClickStillExpandsTheRegion()
    {
        bool? expanded = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [])
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => expanded = value)));

        await cut.Find(RailSelector(Css.Classes.Ide.Layout.PanelLeft)).ClickAsync(new());

        Assert.False(expanded);
    }
}
