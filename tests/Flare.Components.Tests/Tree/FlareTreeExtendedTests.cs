namespace Flare.Components.Tests;

public class FlareTreeExtendedTests : FlareTestContext
{
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
}
