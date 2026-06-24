// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareDataTree extended  (7 tests)
// -----------------------------------------------------------------------------

public class FlareDataTreeExtendedTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareVtreeElement()
    {
        var items = Array.Empty<string>();
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void HasChildren_Param_Exists_RendersWithoutError()
    {
        var items = new[] { "Root" };
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void ChildrenProvider_Param_Exists_RendersWithoutError()
    {
        var items = new[] { "Root" };
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.ChildrenProvider,
                (Func<string, Task<IEnumerable<string>>>)(_ =>
                    Task.FromResult(Enumerable.Empty<string>()))));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void EmptyItems_RendersWithoutError()
    {
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, Array.Empty<string>())
            .Add(x => x.KeySelector, (Func<string, object>)(s => s)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void HasChildren_False_NodeRendersLeafSpacer()
    {
        // When HasChildren returns false the expand button is not rendered,
        // instead the leaf spacer span is rendered.
        var items = new[] { "Leaf" };
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree__leaf-spacer"));
    }

    [Fact]
    public void Items_WithContent_RendersNodes()
    {
        var items = new[] { "Folder A", "Folder B" };
        var cut = Render<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.Equal(2, cut.FindAll(".flare-vtree__node").Count);
    }

    [Fact]
    public void ComponentIsGeneric_AcceptsIntType()
    {
        var items = new[] { 1, 2, 3 };
        var cut = Render<FlareDataTree<int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<int, object>)(i => i)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }
}
