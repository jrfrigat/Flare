using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Arrow keys belong to the carousel only when they come from its own surface or controls: a field
/// inside a slide keeps its arrows, while the focused carousel and its arrow buttons still navigate.
/// </summary>
public sealed class CarouselKeyboardTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task KeysInsideASlideStayThereWhileTheCarouselKeepsItsOwn()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/carousel-keyboard");
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('.{Css.Classes.Carousel.Root}')?.__flareCarouselKeyboard !== undefined");
        await Expect(page.Locator("#slide-input")).ToBeVisibleAsync();

        // A key pressed in the field moves the caret, not the carousel.
        await page.Locator("#slide-input").ClickAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await page.Keyboard.PressAsync("ArrowRight");
        await page.WaitForTimeoutAsync(400);
        Assert.Equal("0", await IndexAsync(page));

        // The root itself still navigates.
        await page.Locator($".{Css.Classes.Carousel.Root}").FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await WaitForIndexAsync(page, "1");

        // So does a key pressed on one of the carousel's own controls.
        await page.Locator($".{Css.Classes.Carousel.ArrowNext}").FocusAsync();
        await page.Keyboard.PressAsync("ArrowLeft");
        await WaitForIndexAsync(page, "0");
    }

    private static Task<string> IndexAsync(IPage page)
        => page.Locator($".{Css.Classes.Carousel.Track}").EvaluateAsync<string>(
            "el => el.style.getPropertyValue('--_carousel-index')");

    private static Task WaitForIndexAsync(IPage page, string expected)
        => page.WaitForFunctionAsync(
            $"() => document.querySelector('.{Css.Classes.Carousel.Track}')?.style.getPropertyValue('--_carousel-index') === '{expected}'");
}
