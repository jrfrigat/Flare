using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareTreeView / FlareTreeItem drag-and-drop reordering
// ------------------------------------------------------------------------------

public class C_TreeDragDropTests : FlareTestContext
{
    private IRenderedComponent<FlareTreeView> RenderDragTree(Action<TreeDropEventArgs> onDrop) =>
        Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .Add(x => x.OnItemDrop, EventCallback.Factory.Create(this, onDrop))
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Alpha").Add(i => i.ItemData, "alpha"))
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Beta").Add(i => i.ItemData, "beta")));

    [Fact]
    public void Drop_ReportsDraggedSourceAndDropTarget()
    {
        TreeDropEventArgs? captured = null;
        var cut = RenderDragTree(a => captured = a);

        var items = cut.FindAll("li.flare-tree-item");
        // Pick up the first item, then drop it on the second.
        items[0].TriggerEvent("ondragstart", new DragEventArgs());
        items[1].TriggerEvent("ondrop", new DragEventArgs());

        Assert.NotNull(captured);
        Assert.Equal("alpha", captured!.SourceItem);
        Assert.Equal("beta", captured.TargetItem);
    }

    [Fact]
    public void DragEnd_ClearsSource_SoNextDropFallsBackToTarget()
    {
        TreeDropEventArgs? captured = null;
        var cut = RenderDragTree(a => captured = a);

        var items = cut.FindAll("li.flare-tree-item");
        items[0].TriggerEvent("ondragstart", new DragEventArgs());
        items[0].TriggerEvent("ondragend", new DragEventArgs());

        // With no active drag, a drop on Beta reports Beta as both source and target.
        items[1].TriggerEvent("ondrop", new DragEventArgs());

        Assert.NotNull(captured);
        Assert.Equal("beta", captured!.SourceItem);
        Assert.Equal("beta", captured.TargetItem);
    }
}
