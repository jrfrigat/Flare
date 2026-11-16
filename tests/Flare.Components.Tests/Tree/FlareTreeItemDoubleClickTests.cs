using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A double-click is two clicks followed by dblclick. The row keeps single-click selection, reports
/// the double-click after the clicks, and - when the host listens for it - the second click no longer
/// undoes the expansion the first one made.
/// </summary>
public class FlareTreeItemDoubleClickTests : FlareTestContext
{
    private static RenderFragment ChildMarkup() => builder => builder.AddMarkupContent(0, "<li id=\"child\">Child</li>");

    private static bool IsExpanded(IRenderedComponent<FlareTreeItem> cut) =>
        cut.Find($"li.{Css.Classes.TreeView.Item}").GetAttribute("aria-expanded") == "true";

    [Fact]
    public void DoubleClick_RunsAfterTheTwoClicks_AndKeepsTheSelection()
    {
        var clicks = 0;
        var selectionChanges = 0;
        MouseEventArgs? captured = null;

        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Leaf")
            .Add(x => x.Selected, true)
            .Add(x => x.SelectedChanged, (bool _) => selectionChanges++)
            .Add(x => x.OnClick, () => clicks++)
            .Add(x => x.OnDoubleClick, (MouseEventArgs e) => captured = e));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        row.Click(new MouseEventArgs { Detail = 1, ClientX = 10, ClientY = 20 });
        row.Click(new MouseEventArgs { Detail = 2, ClientX = 10, ClientY = 20 });

        Assert.Null(captured);

        row.DoubleClick(new MouseEventArgs { Detail = 2, ClientX = 10, ClientY = 20 });

        Assert.NotNull(captured);
        Assert.Equal(10, captured!.ClientX);
        Assert.Equal(20, captured.ClientY);
        Assert.Equal(2, clicks);
        Assert.Equal(0, selectionChanges);
        Assert.Equal("true", cut.Find($"li.{Css.Classes.TreeView.Item}").GetAttribute("aria-selected"));
    }

    [Fact]
    public void SingleClick_StillSelects_WhenOnDoubleClickIsSet()
    {
        var selection = new List<bool>();
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Leaf")
            .Add(x => x.SelectedChanged, (bool value) => selection.Add(value))
            .Add(x => x.OnDoubleClick, (MouseEventArgs _) => { }));

        cut.Find($".{Css.Classes.Tree.ItemRow}").Click(new MouseEventArgs { Detail = 1 });

        Assert.Equal(new[] { true }, selection);
    }

    [Fact]
    public void Parent_DoubleClick_ExpandsOnceAndStaysOpen()
    {
        var doubleClicks = 0;
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.ChildContent, ChildMarkup())
            .Add(x => x.OnDoubleClick, (MouseEventArgs _) => doubleClicks++));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        row.Click(new MouseEventArgs { Detail = 1 });
        Assert.True(IsExpanded(cut));

        row.Click(new MouseEventArgs { Detail = 2 });
        Assert.True(IsExpanded(cut), "The second click of the double-click must not collapse the branch again.");
        Assert.NotEmpty(cut.FindAll("#child"));

        row.DoubleClick(new MouseEventArgs { Detail = 2 });
        Assert.Equal(1, doubleClicks);
        Assert.True(IsExpanded(cut));
    }

    [Fact]
    public void WithoutOnDoubleClick_TheSecondClickKeepsTheOldToggle()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.ChildContent, ChildMarkup()));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        row.Click(new MouseEventArgs { Detail = 1 });
        Assert.True(IsExpanded(cut));

        row.Click(new MouseEventArgs { Detail = 2 });
        Assert.False(IsExpanded(cut), "Without a double-click callback the row keeps its previous click behaviour.");
    }

    [Fact]
    public void DoubleClick_IsRaisedByTheRowOnly()
    {
        var parentRaised = 0;
        var childRaised = 0;

        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Parent")
            .Add(x => x.Expanded, true)
            .Add(x => x.OnDoubleClick, (MouseEventArgs _) => parentRaised++)
            .Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<FlareTreeItem>(0);
                builder.AddAttribute(1, "Label", "Child");
                builder.AddAttribute(2, "OnDoubleClick", EventCallback.Factory.Create<MouseEventArgs>(this, (MouseEventArgs _) => childRaised++));
                builder.CloseComponent();
            })));

        cut.FindAll($".{Css.Classes.Tree.ItemRow}")[1].DoubleClick(new MouseEventArgs { Detail = 2 });

        Assert.Equal(1, childRaised);
        Assert.Equal(0, parentRaised);
    }
}
