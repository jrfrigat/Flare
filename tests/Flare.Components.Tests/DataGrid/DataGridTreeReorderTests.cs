using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A tree row is dragged as a tree node, not as a line in a flat list: dropping on the middle of a row
/// makes the row a child, dropping on an edge makes it a sibling, and a row can never be dropped into
/// its own descendant. The keyboard gesture moves among siblings and never crosses a level.
/// </summary>
public sealed class DataGridTreeReorderTests : FlareTestContext
{
    private sealed class Node
    {
        public required string Name { get; init; }
        public List<Node> Children { get; init; } = [];
    }

    private static (Node RootA, Node RootB) Sample()
    {
        var rootA = new Node
        {
            Name = "root-a",
            Children = [new Node { Name = "child-one" }, new Node { Name = "child-two" }],
        };
        return (rootA, new Node { Name = "root-b" });
    }

    private static RenderFragment Columns() => b =>
    {
        b.OpenComponent<FlareColumn<Node>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Node, object?>)(n => n.Name));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Node>> RenderSampleTree(Node rootA, Node rootB,
        Action<DataGridRowReorder<Node>>? onReorder = null, bool virtualized = false)
        => Render<FlareDataGrid<Node>>(p =>
        {
            p.Add(x => x.Items, new[] { rootA, rootB })
                .Add(x => x.Virtual, virtualized)
                .Add(x => x.RowReorderable, true);
            if (onReorder is not null) p.Add(x => x.OnRowReordered, onReorder);
            p.Add(x => x.Tree, new DataGridTreeConfig<Node>
                {
                    ChildrenSelector = n => n.Children,
                    InitiallyExpanded = _ => true,
                    KeySelector = n => n.Name,
                })
                .Add(x => x.Columns, Columns());
        });

    private static Task DropAsync(IRenderedComponent<FlareDataGrid<Node>> cut,
        string sourceId, string edge, string overId, int index = -1)
    {
        var drag = cut.FindComponent<FlareDragContext<object>>();
        return cut.InvokeAsync(() => drag.Instance.OnDropAsync(sourceId, "flare-datagrid-rows", index, edge, overId));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DropInside_MakesTheTargetTheParent(bool virtualized)
    {
        var (rootA, rootB) = Sample();
        DataGridRowReorder<Node>? reported = null;
        var cut = RenderSampleTree(rootA, rootB, e => reported = e, virtualized);

        // Visible rows: 0 root-a, 1 child-one, 2 child-two, 3 root-b.
        await DropAsync(cut, "3", "into", "1");

        Assert.NotNull(reported);
        Assert.Same(rootB, reported!.Item);
        Assert.Same(rootA.Children[0], reported.Target);
        Assert.Same(rootA.Children[0], reported.Parent);
        Assert.Equal(DataGridRowDropPosition.Inside, reported.Position);
        Assert.Equal(2, reported.Depth);
    }

    [Fact]
    public async Task DropAfter_BecomesASiblingOfTheTarget()
    {
        var (rootA, rootB) = Sample();
        DataGridRowReorder<Node>? reported = null;
        var cut = RenderSampleTree(rootA, rootB, e => reported = e);

        await DropAsync(cut, "3", "after", "1", index: 2);

        Assert.NotNull(reported);
        Assert.Same(rootB, reported!.Item);
        Assert.Same(rootA.Children[0], reported.Target);
        Assert.Same(rootA, reported.Parent);
        Assert.Equal(DataGridRowDropPosition.After, reported.Position);
        Assert.Equal(1, reported.Depth);
    }

    [Fact]
    public async Task DropIntoOwnDescendant_IsRefused()
    {
        var (rootA, rootB) = Sample();
        DataGridRowReorder<Node>? reported = null;
        var cut = RenderSampleTree(rootA, rootB, e => reported = e);

        // root-a (0) dropped into child-one (1): child-one is inside root-a, so the branch would detach.
        await DropAsync(cut, "0", "into", "1");

        Assert.Null(reported);
    }

    [Fact]
    public async Task DropOnItself_IsIgnored()
    {
        var (rootA, rootB) = Sample();
        DataGridRowReorder<Node>? reported = null;
        var cut = RenderSampleTree(rootA, rootB, e => reported = e);

        await DropAsync(cut, "0", "into", "0");

        Assert.Null(reported);
    }

    [Fact]
    public async Task KeyboardMove_Down_UsesTheNextSiblingNotTheNextVisibleRow()
    {
        var (rootA, rootB) = Sample();
        DataGridRowReorder<Node>? reported = null;
        var cut = RenderSampleTree(rootA, rootB, e => reported = e);

        var table = cut.Find("table");
        // First keydown gives the grid focus; the second moves root-a to root-b's position.
        table.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        table.KeyDown(new KeyboardEventArgs { Key = "ArrowDown", AltKey = true });

        Assert.NotNull(reported);
        Assert.Same(rootA, reported!.Item);
        Assert.Same(rootB, reported.Target);
        Assert.Null(reported.Parent);
        Assert.Equal(DataGridRowDropPosition.After, reported.Position);
        Assert.Equal(0, reported.Depth);
        Assert.Equal(3, reported.NewIndex);
    }

    [Fact]
    public void KeyboardMove_AtTheEdgeOfASiblingGroup_DoesNotCrossLevels()
    {
        var (rootA, rootB) = Sample();
        DataGridRowReorder<Node>? reported = null;
        var cut = RenderSampleTree(rootA, rootB, e => reported = e);

        var table = cut.Find("table");
        table.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        table.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" }); // focus child-one
        table.KeyDown(new KeyboardEventArgs { Key = "ArrowUp", AltKey = true });

        Assert.Null(reported);
    }

    [Fact]
    public void TreeRows_CarryLevelAndExpandedAria()
    {
        var (rootA, rootB) = Sample();
        var cut = RenderSampleTree(rootA, rootB);

        var rows = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal("treegrid", cut.Find("table").GetAttribute("role"));
        Assert.Equal("1", rows[0].GetAttribute("aria-level"));
        Assert.Equal("true", rows[0].GetAttribute("aria-expanded"));
        Assert.Equal("2", rows[1].GetAttribute("aria-level"));
        Assert.Null(rows[1].GetAttribute("aria-expanded"));
    }

    [Fact]
    public void FlatRows_HaveNoTreeAria()
    {
        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { new Node { Name = "row" } })
            .Add(x => x.Columns, Columns()));

        var row = cut.Find($"tbody tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal("grid", cut.Find("table").GetAttribute("role"));
        Assert.Null(row.GetAttribute("aria-level"));
        Assert.Null(row.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void TreeDropZone_AcceptsInsideDrops()
    {
        var (rootA, rootB) = Sample();
        var cut = RenderSampleTree(rootA, rootB);

        var body = cut.Find("tbody");
        Assert.Equal("both", body.GetAttribute("data-flare-drop-placement"));
    }
}
