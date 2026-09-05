using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The tree was the worst of the four drag surfaces: HTML5 drag-and-drop (so nothing at all on a touch
/// screen) PLUS an interop round trip on every <c>dragover</c> to ask which third of the row the cursor
/// was over - so continuous that it needed a "one measurement in flight at a time" coalescer to stay
/// usable. Those thirds are what <c>DropPlacement.Both</c> is, and the shared model resolves them in the
/// browser without asking anyone.
/// </summary>
public class TreeDragDropTests : FlareTestContext
{
    private IRenderedComponent<FlareTreeView> RenderDragTree(Action<TreeDropEventArgs> onDrop) =>
        Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .Add(x => x.OnItemDrop, EventCallback.Factory.Create(this, onDrop))
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Alpha").Add(i => i.ItemData, "alpha"))
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Beta").Add(i => i.ItemData, "beta")));

    private static string DragId(IRenderedComponent<FlareTreeView> cut, int index) =>
        cut.FindAll("li[data-flare-drag]")[index].GetAttribute("data-flare-drag")!;

    private static string RootZone(IRenderedComponent<FlareTreeView> cut) =>
        cut.Find("ul[data-flare-drop]").GetAttribute("data-flare-drop")!;

    [Fact]
    public void EveryItemIsDraggableAndTheTreeIsAZoneOfThirds()
    {
        var cut = RenderDragTree(_ => { });

        var root = cut.Find("ul[role=tree]");
        Assert.Equal("both", root.GetAttribute("data-flare-drop-placement"));
        Assert.Equal("flare-tree", root.GetAttribute("data-flare-drag-group"));

        var items = cut.FindAll("li[data-flare-drag]");
        Assert.Equal(2, items.Count);
        Assert.Contains(Css.Classes.Drag.Item, items[0].ClassName);
        Assert.Empty(cut.FindAll("[draggable]"));
    }

    [Fact]
    public void ATreeThatDoesNotDragSaysSoByOmission()
    {
        var cut = Render<FlareTreeView>(p => p
            .AddChildContent<FlareTreeItem>(ip => ip.Add(i => i.Label, "Alpha")));

        Assert.Null(cut.Find("ul[role=tree]").GetAttribute("data-flare-drop"));
        Assert.Empty(cut.FindAll("li[data-flare-drag]"));
    }

    // The thirds, which is the whole reason a tree needs Both: the same drop lands in three different
    // places depending on where in the row it was let go.
    [Theory]
    [InlineData("before", TreeDropPosition.Before)]
    [InlineData("into", TreeDropPosition.Inside)]
    [InlineData("after", TreeDropPosition.After)]
    public async Task TheThirdOfTheRowDecidesThePosition(string edge, TreeDropPosition expected)
    {
        TreeDropEventArgs? captured = null;
        var cut = RenderDragTree(a => captured = a);
        var drag = cut.FindComponent<FlareDragContext<object>>();

        await cut.InvokeAsync(() => drag.Instance.OnDropAsync(
            DragId(cut, 0), RootZone(cut), 0, edge, DragId(cut, 1)));

        Assert.NotNull(captured);
        Assert.Equal("alpha", captured!.SourceItem);
        Assert.Equal("beta", captured.TargetItem);
        Assert.Equal(expected, captured.Position);
    }

    // A drop on the empty part of a branch has no target item, and the tree's event has no way to
    // describe one - so nothing is raised, which is what the HTML5 handler did by only ever firing on
    // a row. It used to report the target as its own source when no drag was in flight; that could not
    // happen through this model, and it should never have been observable.
    [Fact]
    public async Task ADropWithNoItemUnderThePointerRaisesNothing()
    {
        TreeDropEventArgs? captured = null;
        var cut = RenderDragTree(a => captured = a);
        var drag = cut.FindComponent<FlareDragContext<object>>();

        await cut.InvokeAsync(() => drag.Instance.OnDropAsync(
            DragId(cut, 0), RootZone(cut), 2, "into", null));

        Assert.Null(captured);
    }

    [Fact]
    public async Task DragStartAndDragEndAreRaisedOnce()
    {
        object? started = null;
        var ended = 0;
        var cut = Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .Add(x => x.OnItemDragStart, EventCallback.Factory.Create<TreeDragEventArgs>(this, a => started = a.Item))
            .Add(x => x.OnItemDragEnd, EventCallback.Factory.Create(this, () => ended++))
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Alpha").Add(i => i.ItemData, "alpha")));

        var drag = cut.FindComponent<FlareDragContext<object>>();
        await cut.InvokeAsync(() => drag.Instance.OnDragStartAsync(DragId(cut, 0)));
        await cut.InvokeAsync(() => drag.Instance.OnDragEndAsync());

        Assert.Equal("alpha", started);
        Assert.Equal(1, ended);
    }

    // A node dropped inside its own subtree takes the subtree with it: the branch leaves the tree and
    // nothing in the UI puts it back. The zone id of a branch IS the drag id of the node that owns it,
    // so the refusal is a list of those ids - and it has to be a refusal rather than CanDrop, because
    // the tree declares its zones as markup and the context can only ask a predicate about the zones
    // registered with it as components.
    private IRenderedComponent<FlareTreeView> RenderNestedTree() =>
        Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Parent").Add(i => i.Expanded, true).Add(i => i.ItemData, "parent")
                .AddChildContent<FlareTreeItem>(cp => cp
                    .Add(c => c.Label, "Child").Add(c => c.Expanded, true).Add(c => c.ItemData, "child")
                    .AddChildContent<FlareTreeItem>(gp => gp
                        .Add(g => g.Label, "Grandchild").Add(g => g.ItemData, "grand"))))
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Uncle").Add(i => i.Expanded, true).Add(i => i.ItemData, "uncle")
                .AddChildContent<FlareTreeItem>(cp => cp
                    .Add(c => c.Label, "Cousin").Add(c => c.ItemData, "cousin"))));

    [Fact]
    public async Task ANodeRefusesEveryBranchInsideItself()
    {
        var cut = RenderNestedTree();
        var drag = cut.FindComponent<FlareDragContext<object>>();

        DragTargetRuling? ruling = null;
        await cut.InvokeAsync(async () => ruling = await drag.Instance.OnDragStartAsync(DragId(cut, 0)));

        Assert.NotNull(ruling?.Deny);
        Assert.False(ruling!.Permits(DragId(cut, 0)),
            "Its own branch would make the node its own child.");
        Assert.False(ruling.Permits(DragId(cut, 1)),
            "A branch deeper inside is the same move one level down.");
    }

    [Fact]
    public async Task ANodeStillReachesEverythingOutsideItself()
    {
        var cut = RenderNestedTree();
        var drag = cut.FindComponent<FlareDragContext<object>>();

        DragTargetRuling? ruling = null;
        await cut.InvokeAsync(async () => ruling = await drag.Instance.OnDragStartAsync(DragId(cut, 0)));

        Assert.True(ruling!.Permits(RootZone(cut)), "The tree root is where a node goes to leave its parent.");
        Assert.True(ruling.Permits(DragId(cut, 3)), "An unrelated branch is a legitimate destination.");
    }

    // A leaf owns no branch, so naming it would only pad a list that crosses to the browser on every
    // drag - and a subtree is mostly leaves.
    [Fact]
    public async Task TheRefusalNamesBranchesAndNotLeaves()
    {
        var cut = RenderNestedTree();
        var drag = cut.FindComponent<FlareDragContext<object>>();

        DragTargetRuling? ruling = null;
        await cut.InvokeAsync(async () => ruling = await drag.Instance.OnDragStartAsync(DragId(cut, 0)));

        Assert.Equal(new[] { DragId(cut, 0), DragId(cut, 1) }, ruling!.Deny);
    }

    // A branch is its own zone, so a node dropped between two children lands among THEM rather than
    // among their parent's siblings. Nothing said that before: the tree had one flat set of rows.
    [Fact]
    public void AnExpandedBranchIsItsOwnDropZone()
    {
        var cut = Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Parent")
                .Add(i => i.Expanded, true)
                .Add(i => i.ItemData, "parent")
                .AddChildContent<FlareTreeItem>(cp => cp
                    .Add(c => c.Label, "Child").Add(c => c.ItemData, "child"))));

        var zones = cut.FindAll("ul[data-flare-drop]");
        Assert.Equal(2, zones.Count);
        Assert.Equal(
            cut.FindAll("li[data-flare-drag]")[0].GetAttribute("data-flare-drag"),
            zones[1].GetAttribute("data-flare-drop"));
    }
}
