using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareListTests : FlareTestContext
{
    [Fact]
    public void RendersRootUl()
    {
        var cut = Render<FlareList<object>>();

        Assert.NotEmpty(cut.FindAll($"ul.{Css.Classes.List.Root}"));
    }

    [Fact]
    public void HasRoleList()
    {
        var cut = Render<FlareList<object>>();

        Assert.Equal("list", cut.Find($"ul.{Css.Classes.List.Root}").GetAttribute("role"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareList<object>>(p => p
            .AddChildContent("<li id=\"custom-li\">Item</li>"));

        Assert.NotEmpty(cut.FindAll("#custom-li"));
    }

    [Fact]
    public void Dense_HasDenseClass()
    {
        var cut = Render<FlareList<object>>(p => p
            .Add(x => x.Dense, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Dense}"));
    }

    [Fact]
    public void NotDense_NoDenseClass()
    {
        var cut = Render<FlareList<object>>(p => p
            .Add(x => x.Dense, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.List.Dense}"));
    }

    [Fact]
    public void RendersListItems()
    {
        var cut = Render<FlareList<object>>(p => p
            .AddChildContent<FlareListItem>(li =>
                li.Add(x => x.Primary, "First Item")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Item}"));
    }
}

// ------------------------------------------------------------------------------
// FlareListItem  (8 tests from Wave5)
// ------------------------------------------------------------------------------
