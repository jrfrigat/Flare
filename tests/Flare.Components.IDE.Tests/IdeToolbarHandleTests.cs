using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// <see cref="FlareToolbar.ShowReorderHandles"/> hides the per-group <c>MoreVert</c> drag/move handle
/// while the groups and their content stay, and no element is rendered in its place.
/// </summary>
public class IdeToolbarHandleTests : FlareTestContext
{
    public IdeToolbarHandleTests() => Services.AddFlareIde();

    private static RenderFragment Content(string text) => builder => builder.AddContent(0, text);

    private IRenderedComponent<FlareToolbar> RenderToolbar(bool reorderable, bool showHandles = true,
        IReadOnlyList<string>? order = null) =>
        Render<FlareToolbar>(p => p
            .Add(x => x.Groups,
            [
                new("a", "A", Content("Command A")),
                new("b", "B", Content("Command B")),
            ])
            .Add(x => x.Reorderable, reorderable)
            .Add(x => x.ShowReorderHandles, showHandles)
            .Add(x => x.GroupOrder, order));

    [Fact]
    public void Toolbar_ShowsTheHandlesForReorderableGroupsByDefault()
    {
        var cut = RenderToolbar(reorderable: true);

        var handles = cut.FindAll($".{Css.Classes.Ide.Toolbar.Handle}");
        Assert.Equal(2, handles.Count);
        Assert.All(handles, handle => Assert.Equal("true", handle.GetAttribute("draggable")));
    }

    [Fact]
    public void Toolbar_HidesTheHandlesWithoutLosingGroupsOrContent()
    {
        var cut = RenderToolbar(reorderable: true, showHandles: false);

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Toolbar.Handle}"));
        var groups = cut.FindAll("[role=group]");
        Assert.Equal(["A", "B"], groups.Select(group => group.GetAttribute("aria-label")));
        Assert.Equal(["Command A", "Command B"], groups.Select(group => group.TextContent.Trim()));
    }

    [Fact]
    public void Toolbar_HiddenHandlesKeepTheControlledOrder()
    {
        var cut = RenderToolbar(reorderable: true, showHandles: false, order: ["b", "a"]);

        Assert.Equal(["B", "A"],
            cut.FindAll("[role=group]").Select(group => group.GetAttribute("aria-label")));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Toolbar.Handle}"));
    }

    [Fact]
    public void Toolbar_NonReorderableGroupsRenderNoHandleRegardlessOfTheSwitch()
    {
        var cut = RenderToolbar(reorderable: false);

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Toolbar.Handle}"));
    }
}
