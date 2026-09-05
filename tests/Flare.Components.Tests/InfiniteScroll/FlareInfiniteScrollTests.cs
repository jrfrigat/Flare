using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareInfiniteScrollTests : FlareTestContext
{
    [Fact]
    public void RendersChildContentAndSentinel()
    {
        var cut = Render<FlareInfiniteScroll>(p => p
            .AddChildContent("<div class=\"row\">item</div>"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Infinite.Scroll}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Infinite.ScrollSentinel}"));
        Assert.NotEmpty(cut.FindAll(".row"));
    }

    [Fact]
    public void NoMore_ShowsEndContent()
    {
        var cut = Render<FlareInfiniteScroll>(p => p
            .Add(x => x.HasMore, false)
            .Add(x => x.EndContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"end\">No more</span>"))));
        Assert.NotEmpty(cut.FindAll(".end"));
    }
}
