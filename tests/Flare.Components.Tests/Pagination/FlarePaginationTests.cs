using Flare.Components.Resources;
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

    [Fact]
    public void Info_DerivesPageSizeWhenNotSet()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 10)
            .Add(x => x.TotalItems, 250)
            .Add(x => x.ShowInfo, true));

        Assert.Equal(
            string.Format(FlareStrings.Pagination_Range, 1, 25, 250),
            cut.Find($".{Css.Classes.Pagination.Info}").TextContent.Trim());
    }

    [Fact]
    public void Info_NegativePageSizeUsesTheDerivedRange()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 10)
            .Add(x => x.TotalItems, 250)
            .Add(x => x.PageSize, -5)
            .Add(x => x.ShowInfo, true));

        Assert.Equal(
            string.Format(FlareStrings.Pagination_Range, 1, 25, 250),
            cut.Find($".{Css.Classes.Pagination.Info}").TextContent.Trim());
    }

    [Fact]
    public void Info_ExplicitPageSizeIsUnchanged()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 10)
            .Add(x => x.TotalItems, 250)
            .Add(x => x.PageSize, 10)
            .Add(x => x.ShowInfo, true));

        Assert.Equal(
            string.Format(FlareStrings.Pagination_Range, 1, 10, 250),
            cut.Find($".{Css.Classes.Pagination.Info}").TextContent.Trim());
    }

    [Fact]
    public void Info_ClampsCurrentToTheLastPage()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 10)
            .Add(x => x.TotalItems, 250)
            .Add(x => x.Current, 99)
            .Add(x => x.ShowInfo, true));

        Assert.Equal(
            string.Format(FlareStrings.Pagination_Range, 226, 250, 250),
            cut.Find($".{Css.Classes.Pagination.Info}").TextContent.Trim());
    }

    [Fact]
    public void InfoTemplate_ReceivesTheDerivedPageSize()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 10)
            .Add(x => x.TotalItems, 250)
            .Add(x => x.ShowInfo, true)
            .Add(x => x.InfoTemplate, (PaginationInfoContext context) => builder =>
            {
                builder.AddContent(0, context.PageSize);
            }));

        Assert.Equal("25", cut.Find($".{Css.Classes.Pagination.Info}").TextContent.Trim());
    }
}
