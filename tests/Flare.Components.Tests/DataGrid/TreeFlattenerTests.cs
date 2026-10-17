namespace Flare.Components.Tests;

public class TreeFlattenerTests
{
    private sealed record Node(string Key, params Node[] Children);

    private static readonly Node[] Roots =
    [
        new("a", new Node("a1", new Node("a1x")), new Node("a2")),
        new("b"),
    ];

    private static string Rows(IEnumerable<TreeNode<Node>> rows) =>
        string.Join(" ", rows.Select(r => $"{r.Item.Key}:{r.Depth}{(r.HasChildren ? (r.IsExpanded ? "-" : "+") : "")}"));

    [Fact]
    public void ExpandedNodesShowTheirChildrenInOrderWithTheirDepth()
    {
        var rows = TreeFlattener.Flatten(Roots, n => n.Children, _ => true);

        Assert.Equal("a:0- a1:1- a1x:2 a2:1 b:0", Rows(rows));
    }

    [Fact]
    public void ACollapsedNodeHidesItsWholeSubtree()
    {
        var rows = TreeFlattener.Flatten(Roots, n => n.Children, n => n.Key != "a");

        Assert.Equal("a:0+ b:0", Rows(rows));
    }

    [Fact]
    public void ACollapsedSubtreeIsNotWalkedWithoutAVisitor()
    {
        var asked = new List<string>();
        TreeFlattener.Flatten(Roots, n => { asked.Add(n.Key); return n.Children; }, n => n.Key != "a");

        Assert.Equal(["a", "b"], asked);
    }

    // The Gantt's summary bars cover every descendant, the hidden ones under a collapsed row as well.
    [Fact]
    public void TheVisitorSeesEveryNodeWithItsAncestors()
    {
        var seen = new List<string>();
        var rows = TreeFlattener.Flatten(Roots, n => n.Children, _ => false,
            (n, ancestors) => seen.Add($"{string.Join("/", ancestors.Select(a => a.Key))}>{n.Key}"));

        Assert.Equal(["a:0+ b:0"], [Rows(rows)]);
        Assert.Equal([">a", "a>a1", "a/a1>a1x", "a>a2", ">b"], seen);
    }

    [Fact]
    public void ANullOrEmptyChildListIsALeaf()
    {
        var rows = TreeFlattener.Flatten([new Node("leaf")], n => n.Children.Length == 0 ? null : n.Children, _ => true);

        Assert.Equal("leaf:0", Rows(rows));
    }
}
