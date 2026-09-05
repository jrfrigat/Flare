namespace Flare.Components.Tests;

public class FlareTreeExtendedTests : FlareTestContext
{
    [Fact]
    public void FlareTreeView_RendersRootElement()
    {
        var cut = Render<FlareTreeView>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TreeView.Root}"));
    }

    [Fact]
    public void FlareTreeItem_RendersLabelText()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents"));

        Assert.Contains("Documents", cut.Find($".{Css.Classes.TreeView.Label}").TextContent);
    }

    [Fact]
    public void FlareTreeItem_WithChildren_RendersToggleButton()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .AddChildContent<FlareTreeItem>(bp => bp
                .Add(x => x.Label, "Child")));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.TreeView.Toggle}"));
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

        Assert.Empty(cut.FindAll($".{Css.Classes.TreeView.Children}"));
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TreeView.VTree}"));
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TreeView.VTree}"));
    }
}
