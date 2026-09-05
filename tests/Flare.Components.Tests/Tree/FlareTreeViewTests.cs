namespace Flare.Components.Tests;

public class FlareTreeViewTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTreeView>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TreeView.Root}"));
    }

    [Fact]
    public void HasRoleTree()
    {
        var cut = Render<FlareTreeView>();

        Assert.Equal("tree", cut.Find($".{Css.Classes.TreeView.Root}").GetAttribute("role"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTreeView>(p => p
            .AddChildContent("<li id=\"tree-child\">Node</li>"));

        Assert.NotEmpty(cut.FindAll("#tree-child"));
    }

    [Fact]
    public void AriaLabel_AppliedToElement()
    {
        var cut = Render<FlareTreeView>(p => p
            .Add(x => x.AriaLabel, "File tree"));

        Assert.Equal("File tree", cut.Find($".{Css.Classes.TreeView.Root}").GetAttribute("aria-label"));
    }
}
