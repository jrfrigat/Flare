using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using System.Text.RegularExpressions;

namespace Flare.Browser.Tests;

public sealed class AccordionFillTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync()
    {
        var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(800, 720);
        await page.GotoAsync($"{fixture.BaseUrl}/accordion-fill");
        await Expect(page.Locator("#acc")).ToBeVisibleAsync();
        return page;
    }

    [Fact]
    public async Task TheOpenPanelTakesTheRestOfTheHeightAndScrolls()
    {
        var page = await OpenAsync();
        await using var _ = page.Context;

        var body = page.Locator($"#long-panel .{Css.Classes.Accordion.Body}");
        var metrics = await body.EvaluateAsync<int[]>("el => [el.scrollHeight, el.clientHeight]");
        Assert.True(metrics[0] > metrics[1] + 20,
            $"the body has to scroll inside the panel ({metrics[0]} vs {metrics[1]})");

        var frame = await page.Locator("#frame").BoundingBoxAsync();
        var accordion = await page.Locator("#acc").BoundingBoxAsync();
        Assert.NotNull(frame);
        Assert.NotNull(accordion);
        Assert.True(accordion!.Height <= frame!.Height + 0.5,
            "the accordion must not grow past the height it was given");
    }

    [Fact]
    public async Task CollapsingReturnsTheSpaceToTheHeaders()
    {
        var page = await OpenAsync();
        await using var _ = page.Context;

        var panel = page.Locator("#long-panel");
        var expanded = await panel.BoundingBoxAsync();
        await page.Locator($"#long-panel .{Css.Classes.Accordion.Header}").ClickAsync();
        await Expect(panel).Not.ToHaveClassAsync(new Regex("expanded"));

        // The collapse animates flex-grow, so wait for that transition rather than a fixed delay.
        await page.EvaluateAsync<bool>(@"() => new Promise(resolve => {
            const el = document.querySelector('#long-panel');
            let done = false;
            const finish = () => { if (done) return; done = true; el.removeEventListener('transitionend', finish); resolve(true); };
            el.addEventListener('transitionend', finish);
            setTimeout(finish, 900);
        })");

        var collapsed = await panel.BoundingBoxAsync();
        Assert.NotNull(expanded);
        Assert.NotNull(collapsed);
        Assert.True(collapsed!.Height < expanded!.Height / 2,
            $"the collapsed panel must shrink to its header ({collapsed.Height} vs {expanded.Height})");

        var other = await page.Locator("#other-panel").BoundingBoxAsync();
        Assert.NotNull(other);
        Assert.True(other!.Y + other.Height <= 320.5, "the other header stays inside the frame");
    }
}
