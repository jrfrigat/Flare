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
    [InlineData(PaginationSize.Sm, Css.Classes.Pagination.Sm)]
    [InlineData(PaginationSize.Lg, Css.Classes.Pagination.Lg)]
    [InlineData(PaginationSize.Xl, Css.Classes.Pagination.SizeXl)]
    public void Size_AppliesModifierClass(PaginationSize size, string expected)
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Pagination.Root}").ClassName);
    }

    [Fact]
    public void Medium_HasNoSizeModifier()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Size, PaginationSize.Md));
        var cls = cut.Find($".{Css.Classes.Pagination.Root}").ClassName;
        Assert.DoesNotContain("flare-pagination--", cls);
    }
}
