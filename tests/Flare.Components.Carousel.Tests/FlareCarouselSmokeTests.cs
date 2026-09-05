using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The package shipped with no test at all, which means nothing said whether it still rendered. This is
/// the floor, not the coverage it deserves: the component draws its root, its items and its controls.
/// </summary>
public class FlareCarouselSmokeTests : FlareTestContext
{
    private static readonly string[] Slides = ["one", "two", "three"];

    [Fact]
    public void RendersItsRootAndOneSlidePerItem()
    {
        var cut = Render<FlareCarousel<string>>(p => p
            .Add(x => x.Items, Slides)
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(s => b => b.AddContent(0, s))));

        Assert.NotEmpty(cut.FindAll(".flare-carousel"));
        Assert.Equal(Slides.Length, cut.FindAll(".flare-carousel__slide").Count);
    }

    [Fact]
    public void WithoutItems_StillRendersAndDrawsNoSlide()
    {
        var cut = Render<FlareCarousel<string>>(p => p.Add(x => x.Items, []));

        Assert.NotEmpty(cut.FindAll(".flare-carousel"));
        Assert.Empty(cut.FindAll(".flare-carousel__slide"));
    }
}
