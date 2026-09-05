using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareBreadcrumbTests : FlareTestContext
{
    private static readonly BreadcrumbItem[] _items =
    [
        new("Home", "/"),
        new("Products", "/products"),
        new("Details"),
    ];

    [Fact]
    public void RendersRootNav()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Breadcrumb.Root}"));
    }

    [Fact]
    public void RendersAllItems()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Breadcrumb.Item}").Count);
    }

    [Fact]
    public void LastItem_HasCurrentClass()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        var items = cut.FindAll($".{Css.Classes.Breadcrumb.Item}");
        Assert.Contains(Css.Classes.Breadcrumb.Current, items[^1].ClassName);
    }

    [Fact]
    public void NonLastItems_HaveLinks()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        Assert.NotEmpty(cut.FindAll($"a.{Css.Classes.Breadcrumb.Link}"));
    }

    [Fact]
    public void SeparatorRendered_BetweenItems()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Separator, ">"));

        var seps = cut.FindAll($".{Css.Classes.Breadcrumb.Separator}");
        Assert.Equal(2, seps.Count);
    }

    [Fact]
    public void CustomSeparator_Rendered()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Separator, "»"));

        var sep = cut.Find($".{Css.Classes.Breadcrumb.Separator}");
        Assert.Contains("»", sep.TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlarePagination  (6 tests from Wave3)
// ------------------------------------------------------------------------------
