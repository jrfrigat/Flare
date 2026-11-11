using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// Expansion of a tree grid is state a page owns: it can be seeded, bound to, and restored after the
/// page is recreated. The grid used to keep it in a private set seeded by InitiallyExpanded on every
/// flatten, which made a user collapse impossible to keep and left the toggle without aria or keyboard.
/// </summary>
public sealed class DataGridTreeExpansionTests : FlareTestContext
{
    private sealed class Node
    {
        public required string Name { get; init; }
        public string Kind { get; init; } = "node";
        public List<Node> Children { get; init; } = [];
    }

    private static Node Sample() => new()
    {
        Name = "root",
        Children =
        [
            new Node { Name = "child-one", Children = [new Node { Name = "grand-child" }] },
            new Node { Name = "child-two" },
        ],
    };

    private static RenderFragment NodeColumn() => b =>
    {
        b.OpenComponent<FlareColumn<Node>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Node, object?>)(n => n.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();
    };

    private static DataGridTreeConfig<Node> Config(Func<Node, bool>? initiallyExpanded = null) => new()
    {
        ChildrenSelector = n => n.Children,
        InitiallyExpanded = initiallyExpanded,
        KeySelector = n => n.Name,
    };

    private static RenderFragment TwoColumns() => b =>
    {
        b.OpenComponent<FlareColumn<Node>>(0);
        b.AddAttribute(1, "Title", "Kind");
        b.AddAttribute(2, "Field", (Func<Node, object?>)(n => n.Kind));
        b.CloseComponent();
        b.OpenComponent<FlareColumn<Node>>(10);
        b.AddAttribute(11, "Title", "Name");
        b.AddAttribute(12, "Field", (Func<Node, object?>)(n => n.Name));
        b.CloseComponent();
    };

    private static readonly string Toggle = "." + Css.Classes.DataGrid.TreeToggle;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ControlledExpandedItems_ShowsOnlyExpandedRows(bool virtualized)
    {
        var root = Sample();

        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root })
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.ExpandedItems, new HashSet<Node> { root })
            .Add(x => x.Tree, Config())
            .Add(x => x.Columns, NodeColumn()));

        Assert.Contains("child-one", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("child-two", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("grand-child", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void ExternalExpandedItemsChange_OpensAndClosesRows()
    {
        var root = Sample();

        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root })
            .Add(x => x.ExpandedItems, new HashSet<Node>())
            .Add(x => x.Tree, Config())
            .Add(x => x.Columns, NodeColumn()));

        Assert.DoesNotContain("child-one", cut.Markup, StringComparison.Ordinal);

        cut.Render(p => p.Add(x => x.ExpandedItems, new HashSet<Node> { root }));
        Assert.Contains("child-one", cut.Markup, StringComparison.Ordinal);

        cut.Render(p => p.Add(x => x.ExpandedItems, new HashSet<Node>()));
        Assert.DoesNotContain("child-one", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Toggle_RaisesBothEventsAndExpands()
    {
        var root = Sample();
        DataGridTreeExpandChanged<Node>? changed = null;
        HashSet<Node>? expandedSet = null;

        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root })
            .Add(x => x.Tree, Config())
            .Add(x => x.OnExpandedChanged, (DataGridTreeExpandChanged<Node> e) => changed = e)
            .Add(x => x.ExpandedItemsChanged, (HashSet<Node> set) => expandedSet = set)
            .Add(x => x.Columns, NodeColumn()));

        var toggle = cut.FindAll(Toggle)[0];
        Assert.Equal("false", toggle.GetAttribute("aria-expanded"));

        toggle.Click();

        Assert.True(cut.FindAll(Toggle)[0].GetAttribute("aria-expanded") is "true");
        Assert.NotNull(changed);
        Assert.Same(root, changed!.Item);
        Assert.True(changed.Expanded);
        Assert.NotNull(expandedSet);
        Assert.Contains(root, expandedSet!);
        Assert.Contains("child-one", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Toggle_KeyboardEnterAndSpace_ExpandAndCollapse()
    {
        var root = Sample();

        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root })
            .Add(x => x.Tree, Config())
            .Add(x => x.Columns, NodeColumn()));

        cut.FindAll(Toggle)[0].KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.Contains("child-one", cut.Markup, StringComparison.Ordinal);

        cut.FindAll(Toggle)[0].KeyDown(new KeyboardEventArgs { Key = " " });
        Assert.DoesNotContain("child-one", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void InitiallyExpanded_DoesNotReopenACollapsedRow()
    {
        var root = Sample();

        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root })
            .Add(x => x.Tree, Config(_ => true))
            .Add(x => x.Columns, NodeColumn()));

        Assert.Contains("child-one", cut.Markup, StringComparison.Ordinal);

        cut.FindAll(Toggle)[0].Click();
        Assert.DoesNotContain("child-one", cut.Markup, StringComparison.Ordinal);

        // A new parameter set re-runs the projection; the user's collapse must survive it.
        cut.Render(p => p.Add(x => x.Items, new[] { root }));
        Assert.DoesNotContain("child-one", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void ToggleColumnTitle_MovesTheToggleAndTheIndent()
    {
        var root = Sample();
        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root })
            .Add(x => x.ExpandedItems, new HashSet<Node> { root })
            .Add(x => x.Tree, new DataGridTreeConfig<Node>
            {
                ChildrenSelector = n => n.Children,
                KeySelector = n => n.Name,
                ToggleColumnTitle = "Name",
            })
            .Add(x => x.Columns, TwoColumns()));

        var toggle = cut.Find(Toggle);
        var toggleCell = toggle.ParentElement!;
        Assert.Equal(1, toggleCell.ParentElement!.QuerySelectorAll("td").ToList().IndexOf(toggleCell));

        var childRow = cut.FindAll("tbody tr").Single(r => r.TextContent.Contains("child-one", StringComparison.Ordinal));
        var childCells = childRow.QuerySelectorAll("td").ToList();
        Assert.Contains("padding-left", childCells[1].GetAttribute("style") ?? "", StringComparison.Ordinal);
        Assert.DoesNotContain("padding-left", childCells[0].GetAttribute("style") ?? "", StringComparison.Ordinal);
    }

    [Fact]
    public void ToggleColumnTitle_UnknownTitleFallsBackToTheFirstColumn()
    {
        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { Sample() })
            .Add(x => x.Tree, new DataGridTreeConfig<Node>
            {
                ChildrenSelector = n => n.Children,
                KeySelector = n => n.Name,
                ToggleColumnTitle = "Missing",
            })
            .Add(x => x.Columns, TwoColumns()));

        var toggle = cut.Find(Toggle);
        Assert.Equal(0, toggle.ParentElement!.ParentElement!.QuerySelectorAll("td").ToList().IndexOf(toggle.ParentElement!));
    }

    [Fact]
    public void TableAriaLabel_NamesTheTableElement()
    {
        var named = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { Sample() })
            .Add(x => x.Tree, Config())
            .Add(x => x.TableAriaLabel, "Task tree")
            .Add(x => x.Columns, NodeColumn()));
        Assert.Equal("treegrid", named.Find("table").GetAttribute("role"));
        Assert.Equal("Task tree", named.Find("table").GetAttribute("aria-label"));

        var plain = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { Sample() })
            .Add(x => x.Tree, Config())
            .Add(x => x.Columns, NodeColumn()));
        Assert.Null(plain.Find("table").GetAttribute("aria-label"));
    }
}
