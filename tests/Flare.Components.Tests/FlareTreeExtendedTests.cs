// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareTreeView / FlareTreeItem - lazy & extended  (6 tests)
// These supplement the Wave6 tree tests with focus on
// FlareTreeItem HasChildren logic and FlareDataTree params.
// -----------------------------------------------------------------------------

public class FlareTreeExtendedTests : FlareTestContext
{
    [Fact]
    public void FlareTreeView_RendersRootElement()
    {
        var cut = Render<FlareTreeView>();

        Assert.NotEmpty(cut.FindAll(".flare-tree-view"));
    }

    [Fact]
    public void FlareTreeItem_RendersLabelText()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents"));

        Assert.Contains("Documents", cut.Find(".flare-tree-item__label").TextContent);
    }

    [Fact]
    public void FlareTreeItem_WithChildren_RendersToggleButton()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .AddChildContent<FlareTreeItem>(bp => bp
                .Add(x => x.Label, "Child")));

        Assert.NotEmpty(cut.FindAll("button.flare-tree-item__toggle"));
    }

    [Fact]
    public void FlareTreeItem_Collapsed_ChildrenNotVisible()
    {
        // Default Expanded=false -> children should be hidden
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareTreeItem>(bp => bp
                .Add(x => x.Label, "Hidden Child")));

        Assert.Empty(cut.FindAll(".flare-tree-item__children"));
    }

    [Fact]
    public void FlareDataTree_HasChildrenParam_Exists()
    {
        // Verify HasChildren parameter is accepted without error
        var items = new[] { "Root" };
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void FlareDataTree_ChildrenProviderParam_Exists()
    {
        // Verify ChildrenProvider parameter is accepted without error
        var items = new[] { "Root" };
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.ChildrenProvider,
                (Func<string, Task<IEnumerable<string>>>)(_ => Task.FromResult(Enumerable.Empty<string>()))));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }
}
