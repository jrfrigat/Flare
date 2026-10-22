using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The carousel follows the reading direction end to end: the track shifts the other way in RTL, the
/// previous arrow moves to the right and its chevron mirrors, and the arrow keys move the way they point.
/// </summary>
public sealed class CarouselRtlTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Carousel_FollowsTheReadingDirection()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/carousel-rtl");
        // The theme injects the carousel tokens after the first paint; the arrows are positioned by
        // --flare-carousel-arrow-inset, so measuring before it is live puts both at their static spot.
        await page.WaitForFunctionAsync(
            $"() => {{ const el = document.querySelector('.{Css.Classes.Carousel.Root}'); "
            + $"return el !== null && getComputedStyle(el).getPropertyValue('{Css.Tokens.CarouselField.ArrowInset}').trim() !== ''; }}");
        await page.WaitForFunctionAsync(
            $"() => {{ const el = document.querySelector('.{Css.Classes.Carousel.Root}'); return el !== null && getComputedStyle(el).overflow === 'hidden'; }}");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('.{Css.Classes.Carousel.Root}')?.__flareCarouselKeyboard !== undefined");

        var root = page.Locator($".{Css.Classes.Carousel.Root}");
        var track = page.Locator($".{Css.Classes.Carousel.Track}");
        var prev = page.Locator($".{Css.Classes.Carousel.ArrowPrev}");
        var next = page.Locator($".{Css.Classes.Carousel.ArrowNext}");
        var box = await page.Locator("#carousel").BoundingBoxAsync()
            ?? throw new InvalidOperationException("The carousel has no box.");

        // LTR: the next slide lies to the right, so the track shifts left by one viewport.
        await next.ClickAsync();
        await Expect(track).ToHaveCSSAsync("transform", "matrix(1, 0, 0, 1, -600, 0)");
        await Expect(prev).ToBeInViewportAsync();
        var prevLtr = await prev.BoundingBoxAsync() ?? throw new InvalidOperationException("No previous arrow.");
        var nextLtr = await next.BoundingBoxAsync() ?? throw new InvalidOperationException("No next arrow.");
        Assert.True(prevLtr.X < box.X + box.Width / 2);
        Assert.True(nextLtr.X > box.X + box.Width / 2);

        // RTL: the same index shifts the track right, and the previous arrow takes the reading-start side.
        await page.Locator("#rtl").ClickAsync();
        await Expect(page.Locator("[dir='rtl']")).ToHaveCountAsync(1);
        await Expect(track).ToHaveCSSAsync("transform", "matrix(1, 0, 0, 1, 600, 0)");
        var prevRtl = await prev.BoundingBoxAsync() ?? throw new InvalidOperationException("No previous arrow.");
        var nextRtl = await next.BoundingBoxAsync() ?? throw new InvalidOperationException("No next arrow.");
        Assert.True(prevRtl.X > box.X + box.Width / 2);
        Assert.True(nextRtl.X < box.X + box.Width / 2);
        Assert.Equal("matrix(-1, 0, 0, 1, 0, 0)", await prev.Locator($".{Css.Classes.Icon.Root}")
            .EvaluateAsync<string>("el => getComputedStyle(el).transform"));

        // The right arrow points at the previous slide, which in RTL sits on the right.
        await root.FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(track).ToHaveCSSAsync("transform", "matrix(1, 0, 0, 1, 0, 0)");

        // Back in LTR the same key advances again.
        await page.Locator("#ltr").ClickAsync();
        await Expect(page.Locator("[dir='ltr']")).ToHaveCountAsync(1);
        await root.FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(track).ToHaveCSSAsync("transform", "matrix(1, 0, 0, 1, -600, 0)");
    }
}
