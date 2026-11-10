using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A sortable column sorts a tree by levels: each group of siblings in the chosen order, children
/// still under their parent. The grid used to flatten the tree first and sort the flat list, so a
/// sorted child could move away from its parent.
/// </summary>
public sealed class DataGridTreeSortTests : FlareTestContext
{
    private sealed class Node
    {
        public required string Name { get; init; }
        public int Rank { get; init; }
        public List<Node> Children { get; init; } = [];
    }

    private sealed record SortRow(string? Value, int Id);

    // Declared roots are b then a. The ranks disagree with the names on purpose: root-a has Rank 1,
    // root-b Rank 2, and the children inside each group share one rank so a second sort key can act.
    private static (Node RootB, Node RootA) Sample()
    {
        var rootB = new Node
        {
            Name = "root-b",
            Rank = 2,
            Children = [new Node { Name = "child-d", Rank = 2 }, new Node { Name = "child-c", Rank = 2 }],
        };
        var rootA = new Node
        {
            Name = "root-a",
            Rank = 1,
            Children = [new Node { Name = "child-b", Rank = 1 }, new Node { Name = "child-a", Rank = 1 }],
        };
        return (rootB, rootA);
    }

    private static RenderFragment Columns() => b =>
    {
        b.OpenComponent<FlareColumn<Node>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Node, object?>)(n => n.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();

        b.OpenComponent<FlareColumn<Node>>(4);
        b.AddAttribute(5, "Title", "Rank");
        b.AddAttribute(6, "Field", (Func<Node, object?>)(n => n.Rank));
        b.AddAttribute(7, "Sortable", true);
        b.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Node>> RenderSampleTree(Node rootB, Node rootA,
        DataGridTreeSortMode sortMode = DataGridTreeSortMode.Hierarchical, Func<Node, bool>? quickFilter = null)
        => Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { rootB, rootA })
            .Add(x => x.QuickFilter, quickFilter)
            .Add(x => x.Tree, new DataGridTreeConfig<Node>
            {
                ChildrenSelector = n => n.Children,
                InitiallyExpanded = _ => true,
                SortMode = sortMode,
            })
            .Add(x => x.Columns, Columns()));

    private static void ClickHeader(IRenderedComponent<FlareDataGrid<Node>> cut, string title, bool shift = false)
    {
        var th = cut.FindAll("th").First(t => t.TextContent.Contains(title, StringComparison.Ordinal));
        if (shift) th.TriggerEvent("onclick", new MouseEventArgs { ShiftKey = true });
        else th.Click();
    }

    private static void AssertOrder(string markup, params string[] names)
    {
        var previous = -1;
        foreach (var name in names)
        {
            var index = markup.IndexOf(name, StringComparison.Ordinal);
            Assert.True(index >= 0, name + " is missing from the rendered tree");
            Assert.True(index > previous, name + " is out of order");
            previous = index;
        }
    }

    [Fact]
    public void HierarchicalSort_Ascending_KeepsChildrenUnderParents()
    {
        var (rootB, rootA) = Sample();
        var cut = RenderSampleTree(rootB, rootA);

        ClickHeader(cut, "Rank");

        AssertOrder(cut.Markup,
            "root-a", "child-b", "child-a",
            "root-b", "child-d", "child-c");
    }

    [Fact]
    public void HierarchicalSort_Descending_KeepsChildrenUnderParents()
    {
        var (rootB, rootA) = Sample();
        var cut = RenderSampleTree(rootB, rootA);

        ClickHeader(cut, "Rank");
        ClickHeader(cut, "Rank");

        AssertOrder(cut.Markup,
            "root-b", "child-d", "child-c",
            "root-a", "child-b", "child-a");
    }

    [Fact]
    public void ShiftClick_AddsASecondSortWithinSiblings()
    {
        var (rootB, rootA) = Sample();
        var cut = RenderSampleTree(rootB, rootA);

        ClickHeader(cut, "Rank");
        ClickHeader(cut, "Name", shift: true);

        // Rank decides the groups; Name orders equal ranks inside each group.
        AssertOrder(cut.Markup,
            "root-a", "child-a", "child-b",
            "root-b", "child-c", "child-d");
    }

    [Fact]
    public void QuickFilter_SurvivesHierarchicalSort()
    {
        var (rootB, rootA) = Sample();
        var cut = RenderSampleTree(rootB, rootA, quickFilter: n => n.Name != "child-d");

        ClickHeader(cut, "Rank");

        Assert.DoesNotContain("child-d", cut.Markup, StringComparison.Ordinal);
        AssertOrder(cut.Markup, "root-a", "child-b", "child-a", "root-b", "child-c");
    }

    [Fact]
    public void FlatMode_KeepsThePre055FlatSort()
    {
        var (rootB, rootA) = Sample();
        var cut = RenderSampleTree(rootB, rootA, DataGridTreeSortMode.Flat);

        ClickHeader(cut, "Rank");

        // Flat ascending: every rank-1 row before every rank-2 row, so the rank-1 children leave
        // root-b behind and stand above it.
        AssertOrder(cut.Markup,
            "root-a", "child-b", "child-a",
            "root-b", "child-d", "child-c");
    }

    [Fact]
    public void BuildComparison_OrdersEmptyValuesLikeTheFlatPipeline()
    {
        var rows = new[]
        {
            new SortRow("b", 1),
            new SortRow(null, 2),
            new SortRow("a", 3),
            new SortRow("", 4),
        };

        var ascending = DataGridPipeline<SortRow>.BuildComparison(
            [new DataGridSort("Value", SortDirection.Ascending)]);
        Assert.NotNull(ascending);
        var ordered = rows.OrderBy(x => x, Comparer<SortRow>.Create(ascending!)).Select(x => x.Value).ToArray();
        Assert.Equal(new string?[] { null, "", "a", "b" }, ordered);

        var descending = DataGridPipeline<SortRow>.BuildComparison(
            [new DataGridSort("Value", SortDirection.Descending)]);
        Assert.NotNull(descending);
        ordered = rows.OrderBy(x => x, Comparer<SortRow>.Create(descending!)).Select(x => x.Value).ToArray();
        Assert.Equal(new string?[] { "b", "a", "", null }, ordered);
    }

    [Fact]
    public void BuildComparison_AppliesEachSortKeyInOrder()
    {
        var rows = new[]
        {
            new SortRow("x", 1),
            new SortRow("x", 3),
            new SortRow("x", 2),
        };

        var comparison = DataGridPipeline<SortRow>.BuildComparison(
        [
            new DataGridSort("Value", SortDirection.Ascending),
            new DataGridSort("Id", SortDirection.Descending),
        ]);

        Assert.NotNull(comparison);
        var ordered = rows.OrderBy(x => x, Comparer<SortRow>.Create(comparison!)).Select(x => x.Id).ToArray();
        Assert.Equal(new[] { 3, 2, 1 }, ordered);
    }
}
