using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlarePaginationSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(PaginationSize.Xs, Css.Classes.Pagination.SizeXs)]
    [InlineData(PaginationSize.Sm, Css.Classes.Pagination.SizeSm)]
    [InlineData(PaginationSize.Lg, Css.Classes.Pagination.SizeLg)]
    [InlineData(PaginationSize.Xl, Css.Classes.Pagination.SizeXl)]
    public void Size_AppliesModifierClass(PaginationSize size, string expected)
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Pagination.Root}").ClassName);
    }

    // The medium step used to be the absence of a class, so nothing in the DOM said what size the
    // control was and no stylesheet could select it without excluding all four other steps by name.
    [Fact]
    public void Medium_NamesItselfLikeEveryOtherStep()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, PaginationSize.Md));

        Assert.Contains(Css.Classes.Pagination.SizeMd, cut.Find($".{Css.Classes.Pagination.Root}").ClassName);
    }

    // Exactly one step at a time: the default arm now returns a class, so a stray second one would mean
    // two size rules fighting rather than the last one winning cleanly.
    [Theory]
    [InlineData(PaginationSize.Xs)]
    [InlineData(PaginationSize.Sm)]
    [InlineData(PaginationSize.Md)]
    [InlineData(PaginationSize.Lg)]
    [InlineData(PaginationSize.Xl)]
    public void ExactlyOneSizeModifier(PaginationSize size)
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, size));

        var steps = cut.Find($".{Css.Classes.Pagination.Root}").ClassName!
            .Split(' ')
            .Count(c => c is Css.Classes.Pagination.SizeXs or Css.Classes.Pagination.SizeSm
                          or Css.Classes.Pagination.SizeMd or Css.Classes.Pagination.SizeLg
                          or Css.Classes.Pagination.SizeXl);

        Assert.Equal(1, steps);
    }
}
