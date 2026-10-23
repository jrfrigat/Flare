using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A tree item's plain Space selects and Enter/Space toggle it, and the arrows expand and collapse, so
/// its reorder gesture is the Alt chord: Alt+ArrowUp/Down moves it one place among its siblings and
/// raises the same OnItemDrop the pointer path uses. Alt+Space does nothing: Windows owns it.
/// </summary>
public class TreeKeyboardReorderTests : FlareTestContext
{
    private static KeyboardEventArgs Key(string key, bool alt = false) => new() { Key = key, AltKey = alt };

    private IRenderedComponent<FlareTreeView> RenderTreeView(Action<TreeDropEventArgs> onDrop) =>
        Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .Add(x => x.OnItemDrop, onDrop)
            .AddChildContent((RenderFragment)(items =>
            {
                foreach (var (label, index) in new[] { ("Alpha", 0), ("Beta", 1) })
                {
                    items.OpenComponent<FlareTreeItem>(index);
                    items.AddAttribute(1, nameof(FlareTreeItem.Label), label);
                    items.AddAttribute(2, nameof(FlareTreeItem.ItemData), label);
                    items.CloseComponent();
                }
            })));

    [Fact]
    public void AltArrowMovesTheItemAmongItsSiblings()
    {
        TreeDropEventArgs? dropped = null;
        var cut = RenderTreeView(args => dropped = args);

        cut.FindAll("li[role=treeitem]")[0].KeyDown(Key("ArrowDown", alt: true));

        Assert.NotNull(dropped);
        Assert.Equal("Alpha", dropped!.SourceItem);
        Assert.Equal("Beta", dropped.TargetItem);
        Assert.Equal(TreeDropPosition.After, dropped.Position);
    }

    [Fact]
    public void PlainSpaceStillSelectsAndDoesNotReorder()
    {
        TreeDropEventArgs? dropped = null;
        var cut = RenderTreeView(args => dropped = args);

        cut.FindAll("li[role=treeitem]")[0].KeyDown(Key(" "));

        Assert.Null(dropped);
    }

    [Fact]
    public void AltSpaceDoesNotStartAMove()
    {
        TreeDropEventArgs? dropped = null;
        var cut = RenderTreeView(args => dropped = args);

        cut.FindAll("li[role=treeitem]")[0].KeyDown(Key(" ", alt: true));

        Assert.Null(dropped);
        Assert.Null(cut.FindAll("li[role=treeitem]")[0].GetAttribute("aria-grabbed"));
    }
}
