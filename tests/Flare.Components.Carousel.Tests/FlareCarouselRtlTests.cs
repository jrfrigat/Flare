using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// In RTL the carousel reads right to left, so every input follows the reading direction: the right arrow
/// key goes back, the left one advances, and a swipe towards the right advances.
/// </summary>
public class FlareCarouselRtlTests : FlareTestContext
{
    private static readonly string[] Slides = ["one", "two", "three"];
    private static readonly RenderFragment<string> Text = s => b => b.AddContent(0, s);

    private IRenderedComponent<FlareCarousel<string>> RenderCarousel(bool rtl)
        => Render<FlareThemeScope>(p => p
                .AddCascadingValue<IThemeService>(new StubThemeService { Rtl = rtl })
                .AddChildContent<FlareCarousel<string>>(c => c
                    .Add(x => x.Items, Slides)
                    .Add(x => x.ItemTemplate, Text)))
            .FindComponent<FlareCarousel<string>>();

    private static int Index(IRenderedComponent<FlareCarousel<string>> cut)
        => int.Parse((cut.Find(".flare-carousel__track").GetAttribute("style") ?? "")
            .Split("--_carousel-index:")[1], System.Globalization.CultureInfo.InvariantCulture);

    [Fact]
    public void Rtl_RightArrowGoesBackAndLeftArrowAdvances()
    {
        var cut = RenderCarousel(rtl: true);
        var root = cut.Find(".flare-carousel");

        root.KeyDown("ArrowRight");
        Assert.Equal(2, Index(cut));

        root.KeyDown("ArrowLeft");
        Assert.Equal(0, Index(cut));
    }

    [Fact]
    public void Ltr_ArrowsKeepTheirDirection()
    {
        var cut = RenderCarousel(rtl: false);

        cut.Find(".flare-carousel").KeyDown("ArrowRight");

        Assert.Equal(1, Index(cut));
    }

    [Fact]
    public void Rtl_SwipeTowardsTheRightAdvances()
    {
        var cut = RenderCarousel(rtl: true);
        var viewport = cut.Find(".flare-carousel__viewport");

        viewport.PointerDown(new PointerEventArgs { ClientX = 100 });
        viewport.PointerUp(new PointerEventArgs { ClientX = 200 });

        Assert.Equal(1, Index(cut));
    }

    [Fact]
    public void Ltr_SwipeTowardsTheLeftAdvances()
    {
        var cut = RenderCarousel(rtl: false);
        var viewport = cut.Find(".flare-carousel__viewport");

        viewport.PointerDown(new PointerEventArgs { ClientX = 200 });
        viewport.PointerUp(new PointerEventArgs { ClientX = 100 });

        Assert.Equal(1, Index(cut));
    }
}
