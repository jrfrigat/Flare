using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareBreadcrumbCollapseTests : FlareTestContext
{
    private static readonly BreadcrumbItem[] Five =
    [
        new("Home", "/", false),
        new("A", "/a", false),
        new("B", "/b", false),
        new("C", "/c", false),
        new("D", null, false),
    ];

    [Fact]
    public void UnderMaxItems_NoExpander()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 10));
        Assert.Empty(cut.FindAll($".{Css.Classes.Breadcrumb.Expander}"));
        Assert.Equal(5, cut.FindAll($".{Css.Classes.Breadcrumb.Item}").Count);
    }

    [Fact]
    public void OverMaxItems_CollapsesMiddleWithExpander()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3));
        // first(1) + expander item + last(1) = 3 list items
        Assert.Single(cut.FindAll($".{Css.Classes.Breadcrumb.Expander}"));
        Assert.Contains("Home", cut.Markup);
        Assert.Contains("D", cut.Markup);
        Assert.DoesNotContain(">B<", cut.Markup);
    }

    [Fact]
    public void ClickingExpander_RevealsAllItems()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3));
        cut.Find($".{Css.Classes.Breadcrumb.Expander}").Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.Breadcrumb.Expander}"));
        Assert.Equal(5, cut.FindAll($".{Css.Classes.Breadcrumb.Item}").Count);
    }

    [Fact]
    public void ItemsAfterCollapseZero_StillKeepsTheCurrentPage()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3)
            .Add(x => x.ItemsAfterCollapse, 0));

        Assert.Single(cut.FindAll($".{Css.Classes.Breadcrumb.Expander}"));
        Assert.Contains("D", cut.Markup);
        Assert.Null(cut.Find($".{Css.Classes.Breadcrumb.Current}").QuerySelector("a"));
        Assert.Equal("page", cut.Find($".{Css.Classes.Breadcrumb.Current} .{Css.Classes.Breadcrumb.Text}")
            .GetAttribute("aria-current"));
    }

    [Fact]
    public void CountsCoveringTheWholePath_CollapseNothing()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3)
            .Add(x => x.ItemsBeforeCollapse, 10)
            .Add(x => x.ItemsAfterCollapse, 1));

        Assert.Empty(cut.FindAll($".{Css.Classes.Breadcrumb.Expander}"));
        Assert.Equal(5, cut.FindAll($".{Css.Classes.Breadcrumb.Item}").Count);
    }

    [Fact]
    public void ANegativeBeforeCount_StillLeavesAnExpanderToRevealTheMiddle()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3)
            .Add(x => x.ItemsBeforeCollapse, -5));

        Assert.Single(cut.FindAll($".{Css.Classes.Breadcrumb.Expander}"));
        Assert.Contains("D", cut.Markup);
    }

    [Fact]
    public void ExpandingGivesTheKeyboardAFocusTargetAmongTheRevealedCrumbs()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, Five)
            .Add(x => x.MaxItems, 3));

        cut.Find($".{Css.Classes.Breadcrumb.Expander}").Click();

        var target = cut.FindAll($".{Css.Classes.Breadcrumb.Item}")[1];
        Assert.Equal("-1", target.GetAttribute("tabindex"));
        Assert.False(string.IsNullOrEmpty(target.GetAttribute("id")));
    }
}
