using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// FlareTreeItem follows the project's two-way state contract: with an ExpandedChanged listener the
/// Expanded parameter is authoritative; without one, local state survives an unrelated parent render
/// and only a real parameter change is adopted. Every user gesture raises the callback.
/// </summary>
public class FlareTreeItemExpansionTests : FlareTestContext
{
    private static RenderFragment ChildMarkup() => builder => builder.AddMarkupContent(0, "<li id=\"child\">Child</li>");

    private IRenderedComponent<FlareTreeItem> RenderParent(bool expanded, Action<bool>? changed = null) =>
        Render<FlareTreeItem>(p =>
        {
            p.Add(x => x.Label, "Parent")
                .Add(x => x.Expanded, expanded)
                .Add(x => x.ChildContent, ChildMarkup());
            if (changed is not null)
                p.Add(x => x.ExpandedChanged, EventCallback.Factory.Create<bool>(this, changed));
        });

    private static bool IsExpanded(IRenderedComponent<FlareTreeItem> cut) =>
        cut.Find($"li.{Css.Classes.TreeView.Item}").GetAttribute("aria-expanded") == "true";

    [Fact]
    public void Controlled_ExternalChangeAfterMount_OpensTheItemAndRaisesNothing()
    {
        var changes = new List<bool>();
        var cut = RenderParent(expanded: false, changes.Add);

        Assert.False(IsExpanded(cut));
        Assert.Empty(cut.FindAll("#child"));

        cut.Render(p => p.Add(x => x.Expanded, true));

        Assert.True(IsExpanded(cut));
        Assert.NotEmpty(cut.FindAll("#child"));
        Assert.Empty(changes);
    }

    [Fact]
    public void Controlled_ParentThatDeclinesTheChange_PutsTheItemBack()
    {
        var changes = new List<bool>();
        var cut = RenderParent(expanded: false, changes.Add);

        cut.Find($".{Css.Classes.Tree.ItemRow}").Click();

        Assert.Equal(new[] { true }, changes);
        Assert.NotEmpty(cut.FindAll($"ul.{Css.Classes.TreeView.Children}"));

        // The parent listened and deliberately kept Expanded false.
        cut.Render(p => p.Add(x => x.Expanded, false));

        Assert.False(IsExpanded(cut));
        Assert.Empty(cut.FindAll($"ul.{Css.Classes.TreeView.Children}"));
    }

    [Fact]
    public void RowClick_RaisesExpandedChanged_ForBothDirections()
    {
        var changes = new List<bool>();
        var cut = RenderParent(expanded: false, changes.Add);

        cut.Find($".{Css.Classes.Tree.ItemRow}").Click();
        Assert.True(IsExpanded(cut));

        cut.Find($".{Css.Classes.Tree.ItemRow}").Click();
        Assert.False(IsExpanded(cut));

        Assert.Equal(new[] { true, false }, changes);
    }

    [Fact]
    public void ToggleButton_RaisesExpandedChanged()
    {
        var changes = new List<bool>();
        var cut = RenderParent(expanded: false, changes.Add);

        cut.Find($"button.{Css.Classes.TreeView.Toggle}").Click();

        Assert.Equal(new[] { true }, changes);
        Assert.True(IsExpanded(cut));
    }

    [Fact]
    public void ArrowRightExpands_ArrowLeftCollapses_AndBothRaise()
    {
        var changes = new List<bool>();
        var cut = RenderParent(expanded: false, changes.Add);
        var item = cut.Find($"li.{Css.Classes.TreeView.Item}");

        item.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.True(IsExpanded(cut));

        cut.Find($"li.{Css.Classes.TreeView.Item}").KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        Assert.False(IsExpanded(cut));

        Assert.Equal(new[] { true, false }, changes);
    }

    [Fact]
    public void Uncontrolled_KeepsLocalStateAcrossAnUnrelatedParameterRender()
    {
        var cut = RenderParent(expanded: false);

        cut.Find($".{Css.Classes.Tree.ItemRow}").Click();
        Assert.True(IsExpanded(cut));

        // The parent re-renders for its own reason and does not touch Expanded.
        cut.Render(p => p.Add(x => x.Icon, FlareIcons.Folder));

        Assert.True(IsExpanded(cut));
        Assert.NotEmpty(cut.FindAll("#child"));
    }

    [Fact]
    public void Uncontrolled_AdoptsARealParameterChange()
    {
        var cut = RenderParent(expanded: false);

        cut.Render(p => p.Add(x => x.Expanded, true));

        Assert.True(IsExpanded(cut));
        Assert.NotEmpty(cut.FindAll("#child"));
    }
}
