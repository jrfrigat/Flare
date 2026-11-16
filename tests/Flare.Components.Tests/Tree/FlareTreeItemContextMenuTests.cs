using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The context-menu event belongs to the row, not to the tree item's whole subtree. Right-clicking a
/// child row raises that child's callback, the empty space of a branch raises nothing, and a right
/// click never selects or expands the row.
/// </summary>
public class FlareTreeItemContextMenuTests : FlareTestContext
{
    // Blazor renders `@oncontextmenu:preventDefault="true"` as this attribute on the element.
    private const string PreventContextMenu = "blazor:oncontextmenu:preventdefault";

    private IRenderedComponent<FlareTreeItem> RenderWithChild(
        Action<MouseEventArgs> parentContextMenu, Action<MouseEventArgs> childContextMenu)
    {
        return Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.Expanded, true)
            .Add(x => x.OnContextMenu, EventCallback.Factory.Create<MouseEventArgs>(this, parentContextMenu))
            .Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<FlareTreeItem>(0);
                builder.AddAttribute(1, "Label", "Child");
                builder.AddAttribute(2, "OnContextMenu", EventCallback.Factory.Create<MouseEventArgs>(this, childContextMenu));
                builder.CloseComponent();
            })));
    }

    [Fact]
    public void RightClick_FiresOnContextMenu_WithoutSelectingOrExpanding()
    {
        var selectedChanges = 0;
        var expandedChanges = 0;
        MouseEventArgs? captured = null;

        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.SelectedChanged, EventCallback.Factory.Create<bool>(this, _ => selectedChanges++))
            .Add(x => x.ExpandedChanged, EventCallback.Factory.Create<bool>(this, _ => expandedChanges++))
            .Add(x => x.OnContextMenu, EventCallback.Factory.Create<MouseEventArgs>(this, e => captured = e))
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<li id=\"child\">Child</li>"))));

        cut.Find($".{Css.Classes.Tree.ItemRow}").ContextMenu(new MouseEventArgs { ClientX = 12, ClientY = 34 });

        Assert.NotNull(captured);
        Assert.Equal(12, captured!.ClientX);
        Assert.Equal(34, captured.ClientY);
        Assert.Equal(0, selectedChanges);
        Assert.Equal(0, expandedChanges);
        Assert.Empty(cut.FindAll("#child"));
        Assert.Equal("false", cut.Find($"li.{Css.Classes.TreeView.Item}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ContextMenu_IsRaisedByTheRowOnly_NotByTheExpandedBranch()
    {
        var parentRaised = 0;
        var childRaised = 0;
        var cut = RenderWithChild(_ => parentRaised++, _ => childRaised++);

        // The empty space of the branch belongs to the ul, which carries no handler at all.
        var branch = cut.Find($"ul.{Css.Classes.TreeView.Children}");
        Assert.False(branch.HasAttribute("blazor:oncontextmenu"));
        Assert.Equal(0, parentRaised);
        Assert.Equal(0, childRaised);

        // The child row has its own handler; the parent row is not its ancestor.
        cut.FindAll($".{Css.Classes.Tree.ItemRow}")[1].ContextMenu();
        Assert.Equal(1, childRaised);
        Assert.Equal(0, parentRaised);

        cut.FindAll($".{Css.Classes.Tree.ItemRow}")[0].ContextMenu();
        Assert.Equal(1, parentRaised);
        Assert.Equal(1, childRaised);
    }

    [Fact]
    public void PreventDefault_IsApplied_WhenAHandlerIsSet()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Row")
            .Add(x => x.OnContextMenu, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));

        Assert.True(cut.Find($".{Css.Classes.Tree.ItemRow}").HasAttribute(PreventContextMenu),
            "A consumer that handles the context menu expects its own menu instead of the browser's.");
    }

    [Fact]
    public void PreventDefault_CanBeTurnedOff()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Row")
            .Add(x => x.OnContextMenu, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { }))
            .Add(x => x.PreventContextMenuDefault, false));

        Assert.False(cut.Find($".{Css.Classes.Tree.ItemRow}").HasAttribute(PreventContextMenu));
    }

    [Fact]
    public void PreventDefault_IsNotApplied_WithoutAHandler()
    {
        var cut = Render<FlareTreeItem>(p => p.Add(x => x.Label, "Row"));

        Assert.False(cut.Find($".{Css.Classes.Tree.ItemRow}").HasAttribute(PreventContextMenu),
            "An item that does not handle the context menu must keep the browser's own menu.");
    }
}
