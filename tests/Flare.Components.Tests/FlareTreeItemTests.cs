// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareTreeItem  (8 tests)
// -----------------------------------------------------------------------------

public class FlareTreeItemTests : FlareTestContext
{
    [Fact]
    public void RendersRootLiElement()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Root Node"));

        Assert.NotEmpty(cut.FindAll("li.flare-tree-item"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents"));

        Assert.Contains("Documents", cut.Find(".flare-tree-item__label").TextContent);
    }

    [Fact]
    public void HasRoleTreeitem()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Node"));

        Assert.Equal("treeitem", cut.Find("li").GetAttribute("role"));
    }

    [Fact]
    public void NoChildren_NoToggleButton()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Leaf"));

        Assert.Empty(cut.FindAll(".flare-tree-item__toggle"));
    }

    [Fact]
    public void WithChildren_HasToggleButton()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .AddChildContent("<li>Child</li>"));

        Assert.NotEmpty(cut.FindAll(".flare-tree-item__toggle"));
    }

    [Fact]
    public void WithChildren_CollapsedByDefault_ChildrenHidden()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .AddChildContent("<li id=\"nested\">Child</li>"));

        // By default Expanded=false, children are not rendered
        Assert.Empty(cut.FindAll("#nested"));
    }

    [Fact]
    public void Expanded_True_ChildrenVisible()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.Expanded, true)
            .AddChildContent("<li id=\"nested-visible\">Child</li>"));

        Assert.NotEmpty(cut.FindAll("#nested-visible"));
    }

    [Fact]
    public void Icon_RendersIconSpan()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "With Icon")
            .Add(x => x.Icon, "folder"));

        Assert.NotEmpty(cut.FindAll(".flare-tree-item__icon"));
        Assert.Contains("folder", cut.Find(".flare-tree-item__icon").TextContent);
    }
}
