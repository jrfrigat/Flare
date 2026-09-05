using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlarePaginationTests : FlareTestContext
{
    [Fact]
    public void RendersRootNav()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Pagination.Root}"));
    }

    [Fact]
    public void RendersPageButtons()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 3)
            .Add(x => x.Current, 1));

        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 3);
    }

    [Fact]
    public void PreviousButton_DisabledOnFirstPage()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Current, 1));

        var prevBtn = cut.Find("button[aria-label='Previous page']");
        Assert.True(prevBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void NextButton_DisabledOnLastPage()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Current, 5));

        var nextBtn = cut.Find("button[aria-label='Next page']");
        Assert.True(nextBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void ActivePage_HasActiveClass()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Current, 2));

        var activeBtn = cut.Find("button[aria-current='page']");
        Assert.Contains(Css.Classes.Pagination.BtnActive, activeBtn.ClassName);
    }

    [Fact]
    public void LargePageCount_ShowsEllipsis()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 20)
            .Add(x => x.Current, 10));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Pagination.Ellipsis}"));
    }
}

// ------------------------------------------------------------------------------
// FlareNavLink  (6 tests from Wave6)
// ------------------------------------------------------------------------------
