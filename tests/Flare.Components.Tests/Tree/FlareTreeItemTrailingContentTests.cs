using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TrailingContent is the per-row slot a consumer uses for status and actions. It renders inside the
/// row after the label and before the drag handle, and the component deliberately does not manage
/// click propagation for it: the host decides whether a click on its control also selects the row.
/// </summary>
public class FlareTreeItemTrailingContentTests : FlareTestContext
{
    private static RenderFragment Markup(string markup) => builder => builder.AddMarkupContent(0, markup);

    [Fact]
    public void TrailingContent_RendersInsideTheRow_AfterTheLabel()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents")
            .Add(x => x.TrailingContent, Markup("<span id=\"status\">Connected</span>")));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        var wrapper = row.QuerySelector($".{Css.Classes.TreeView.Trailing}");
        Assert.NotNull(wrapper);
        Assert.NotNull(wrapper!.QuerySelector("#status"));

        var label = row.QuerySelector($".{Css.Classes.TreeView.Label}")!;
        var status = row.QuerySelector("#status")!;
        var all = row.QuerySelectorAll("*").ToList();
        Assert.True(all.IndexOf(status) > all.IndexOf(label),
            "The slot must follow the label, not replace or precede it.");
    }

    [Fact]
    public void TrailingContent_RendersBeforeTheDragHandle()
    {
        var cut = Render<FlareTreeView>(p => p
            .Add(x => x.Draggable, true)
            .AddChildContent<FlareTreeItem>(ip => ip
                .Add(i => i.Label, "Documents")
                .Add(i => i.TrailingContent, Markup("<span id=\"status\">Connected</span>"))));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        var wrapper = row.QuerySelector($".{Css.Classes.TreeView.Trailing}");
        Assert.NotNull(wrapper?.QuerySelector("#status"));

        var children = row.Children.ToList();
        var wrapperIndex = children.IndexOf(wrapper!);
        var handleIndex = children.FindIndex(e => e.ClassList.Contains(Css.Classes.TreeView.DragHandle));

        Assert.True(wrapperIndex >= 0, "The slot renders inside the draggable row.");
        Assert.True(handleIndex > wrapperIndex, "The drag handle stays the last row item, after the slot.");
    }

    [Fact]
    public async Task TrailingControl_KeydownEnter_DoesNotSelectOrExpandTheRow()
    {
        var selectedChanges = 0;
        var expandedChanges = 0;

        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents")
            .Add(x => x.SelectedChanged, EventCallback.Factory.Create<bool>(this, _ => selectedChanges++))
            .Add(x => x.ExpandedChanged, EventCallback.Factory.Create<bool>(this, _ => expandedChanges++))
            .Add(x => x.ChildContent, (RenderFragment)(child => child.AddMarkupContent(0, "<li>Child</li>")))
            .Add(x => x.TrailingContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "id", "disconnect");
                builder.AddAttribute(2, "type", "button");
                builder.AddAttribute(3, "onkeydown",
                    EventCallback.Factory.Create<KeyboardEventArgs>(this, () => { }));
                builder.AddContent(4, "Disconnect");
                builder.CloseElement();
            })));

        await cut.Find("#disconnect").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(0, selectedChanges);
        Assert.Equal(0, expandedChanges);
        Assert.Empty(cut.FindAll($".{Css.Classes.TreeView.Children}"));
    }

    [Fact]
    public void TrailingContent_Control_StaysFocusableAndLabelled()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents")
            .Add(x => x.TrailingContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "id", "disconnect");
                builder.AddAttribute(2, "type", "button");
                builder.AddAttribute(3, "aria-label", "Disconnect");
                builder.AddContent(4, "Disconnect");
                builder.CloseElement();
            })));

        var button = cut.Find("#disconnect");
        Assert.Null(button.GetAttribute("tabindex"));
        Assert.Null(button.GetAttribute("aria-hidden"));
        Assert.Equal("Disconnect", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void TrailingControl_ThatStopsPropagation_DoesNotSelectOrExpandTheRow()
    {
        var selectedChanges = 0;
        var controlClicks = 0;

        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents")
            .Add(x => x.SelectedChanged, EventCallback.Factory.Create<bool>(this, _ => selectedChanges++))
            .Add(x => x.ChildContent, (RenderFragment)(child => child.AddMarkupContent(0, "<li>Child</li>")))
            .Add(x => x.TrailingContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "id", "disconnect");
                builder.AddAttribute(2, "type", "button");
                builder.AddAttribute(3, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => controlClicks++));
                builder.AddEventStopPropagationAttribute(4, "onclick", true);
                builder.CloseElement();
            })));

        cut.Find("#disconnect").Click();

        Assert.Equal(1, controlClicks);
        Assert.Equal(0, selectedChanges);
        Assert.Empty(cut.FindAll($".{Css.Classes.TreeView.Children}"));
    }

    [Fact]
    public void TrailingControl_WithoutStopPropagation_BubblesToTheRow()
    {
        var selectedChanges = 0;
        var controlClicks = 0;

        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Documents")
            .Add(x => x.SelectedChanged, EventCallback.Factory.Create<bool>(this, _ => selectedChanges++))
            .Add(x => x.ChildContent, (RenderFragment)(child => child.AddMarkupContent(0, "<li>Child</li>")))
            .Add(x => x.TrailingContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "id", "disconnect");
                builder.AddAttribute(2, "type", "button");
                builder.AddAttribute(3, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => controlClicks++));
                builder.CloseElement();
            })));

        cut.Find("#disconnect").Click();

        Assert.Equal(1, controlClicks);
        Assert.Equal(1, selectedChanges);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TreeView.Children}"));
    }
}
