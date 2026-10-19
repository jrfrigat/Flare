using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareCarouselIndexTests : FlareTestContext
{
    private static readonly string[] Slides = ["one", "two", "three"];
    private static readonly RenderFragment<string> Text = s => b => b.AddContent(0, s);

    private IRenderedComponent<FlareCarousel<string>> RenderCarousel(
        IReadOnlyList<string>? items, bool autoPlay = false, int interval = 20)
        => Render<FlareCarousel<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.ItemTemplate, Text)
            .Add(x => x.AutoPlay, autoPlay)
            .Add(x => x.AutoPlayIntervalMs, interval));

    private static string Track(IRenderedComponent<FlareCarousel<string>> cut)
        => cut.Find(".flare-carousel__track").GetAttribute("style") ?? string.Empty;

    [Fact]
    public void ShrinkingItems_ClampsTheSelectedSlide()
    {
        var cut = RenderCarousel(Slides);
        cut.FindAll(".flare-carousel__dot")[2].Click();
        Assert.Contains("-200%", Track(cut));

        cut.Render(p => p.Add(x => x.Items, new[] { "one", "two" }));

        Assert.Contains("-100%", Track(cut));
        Assert.Equal("true", cut.FindAll(".flare-carousel__dot")[1].GetAttribute("aria-selected"));
    }

    [Fact]
    public void UnchangedItems_KeepTheSelectedSlide()
    {
        var cut = RenderCarousel(Slides);
        cut.FindAll(".flare-carousel__dot")[1].Click();

        cut.Render(p => p.Add(x => x.Items, Slides));

        Assert.Contains("-100%", Track(cut));
    }

    [Fact]
    public async Task EmptyItems_AreSafeForKeysAndAutoPlay()
    {
        var cut = RenderCarousel(Slides, autoPlay: true);
        cut.FindAll(".flare-carousel__dot")[2].Click();

        cut.Render(p => p.Add(x => x.Items, Array.Empty<string>()));

        cut.Find(".flare-carousel").KeyDown("ArrowRight");
        cut.Find(".flare-carousel").KeyDown("ArrowLeft");
        await Task.Delay(60);

        Assert.Empty(cut.FindAll(".flare-carousel__slide"));
        Assert.Contains("-0%", Track(cut));
    }

    [Fact]
    public void AutoPlay_AdvancesWithinTheSlideCount()
    {
        var cut = RenderCarousel(new[] { "one", "two" }, autoPlay: true);

        cut.WaitForAssertion(() => Assert.Contains("-100%", Track(cut)), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => Assert.Contains("-0%", Track(cut)), TimeSpan.FromSeconds(2));
    }
}
