using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// FlareDataTree as a WAI-ARIA tree (Alva TASK-63): one row in the Tab order, arrows move between visible rows,
/// Right/Left open, close and walk the hierarchy, Enter or Space acts like a click.
/// </summary>
public class FlareDataTreeKeyboardTests : FlareTestContext
{
    private readonly List<string> _clicked = [];

    private IRenderedComponent<FlareDataTree<string>> RenderDataTree() => Render<FlareDataTree<string>>(p => p
        .Add(x => x.Items, new[] { "A", "B" })
        .Add(x => x.KeySelector, (Func<string, object>)(s => s))
        .Add(x => x.HasChildren, (Func<string, bool>)(s => s == "A"))
        .Add(x => x.ChildrenProvider, (Func<string, Task<IEnumerable<string>>>)(s =>
            Task.FromResult<IEnumerable<string>>(s == "A" ? ["A1", "A2"] : [])))
        .Add(x => x.ItemTemplate, (RenderFragment<string>)(s => b => b.AddContent(0, s)))
        .Add(x => x.AriaLabel, "Projects")
        .Add(x => x.ItemClick, EventCallback.Factory.Create<string>(this, s => _clicked.Add(s))));

    private static List<string> Rows(IRenderedComponent<FlareDataTree<string>> cut) =>
        cut.FindAll("[role=treeitem]").Select(r => r.TextContent).ToList();

    private static string TabStop(IRenderedComponent<FlareDataTree<string>> cut)
    {
        var stops = cut.FindAll("[role=treeitem][tabindex='0']");
        Assert.Single(stops);
        return stops[0].TextContent;
    }

    private static void Press(IRenderedComponent<FlareDataTree<string>> cut, string row, string key) =>
        cut.FindAll("[role=treeitem]").Single(r => r.TextContent == row).KeyDown(new KeyboardEventArgs { Key = key });

    [Fact]
    public void Root_IsANamedTree()
    {
        var root = RenderDataTree().Find($".{Css.Classes.TreeView.VTree}");

        Assert.Equal("tree", root.GetAttribute("role"));
        Assert.Equal("Projects", root.GetAttribute("aria-label"));
    }

    [Fact]
    public void WithoutSelection_TheFirstRowIsTheOnlyTabStop() => Assert.Equal("A", TabStop(RenderDataTree()));

    [Fact]
    public void ExpandButton_StaysOutOfTheTabOrder() =>
        Assert.Equal("-1", RenderDataTree().Find($".{Css.Classes.TreeView.VExpandBtn}").GetAttribute("tabindex"));

    [Fact]
    public void ArrowsHomeAndEnd_MoveTheTabStop()
    {
        var cut = RenderDataTree();

        Press(cut, "A", "ArrowDown");
        Assert.Equal("B", TabStop(cut));
        Press(cut, "B", "ArrowUp");
        Assert.Equal("A", TabStop(cut));
        Press(cut, "A", "End");
        Assert.Equal("B", TabStop(cut));
        Press(cut, "B", "Home");
        Assert.Equal("A", TabStop(cut));
    }

    [Fact]
    public void RightAndLeft_OpenWalkAndCloseTheHierarchy()
    {
        var cut = RenderDataTree();

        Press(cut, "A", "ArrowRight");
        cut.WaitForAssertion(() => Assert.Equal(["A", "A1", "A2", "B"], Rows(cut)));
        Assert.Equal("A", TabStop(cut));

        Press(cut, "A", "ArrowRight");
        Assert.Equal("A1", TabStop(cut));

        Press(cut, "A1", "ArrowLeft");
        Assert.Equal("A", TabStop(cut));

        Press(cut, "A", "ArrowLeft");
        cut.WaitForAssertion(() => Assert.Equal(["A", "B"], Rows(cut)));
    }

    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void EnterAndSpace_ActLikeAClick(string key)
    {
        var cut = RenderDataTree();

        Press(cut, "B", key);

        Assert.Equal(["B"], _clicked);
        Assert.Equal("B", TabStop(cut));
        Assert.Equal("true", cut.FindAll("[role=treeitem]").Single(r => r.TextContent == "B").GetAttribute("aria-selected"));
    }
}
