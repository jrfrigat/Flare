using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A nested timeline keeps its own Align and Connector: the outer Right/Dashed/Alternating options
/// style only the outer list's items, and the outer last item does not hide the inner lines.
/// </summary>
public sealed class TimelineNestedTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task NestedTimeline_KeepsItsOwnLayout()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/timeline-nested");
        await Expect(page.Locator("#outer-1")).ToBeVisibleAsync();

        // The outer list mirrors its rows; the nested list keeps the default direction.
        await Expect(page.Locator("#outer-1")).ToHaveCSSAsync("flex-direction", "row-reverse");
        await Expect(page.Locator("#inner-1")).ToHaveCSSAsync("flex-direction", "row");
        await Expect(page.Locator("#alt-2")).ToHaveCSSAsync("flex-direction", "row-reverse");
        await Expect(page.Locator("#alt-inner-1")).ToHaveCSSAsync("flex-direction", "row");

        // The outer connector is dashed; the nested one stays solid (no border, painted by background).
        await Expect(LineOf(page, "#outer-1")).ToHaveCSSAsync("border-left-style", "dashed");
        await Expect(LineOf(page, "#inner-1")).ToHaveCSSAsync("border-left-style", "none");

        // The outer last item drops its line; the lines of the list nested inside it stay, except the
        // nested list's own last item, which ends its own list.
        await Expect(LineOf(page, "#outer-2")).ToHaveCSSAsync("display", "none");
        Assert.NotEqual("none", await LineOf(page, "#inner-1").EvaluateAsync<string>("el => getComputedStyle(el).display"));
        await Expect(LineOf(page, "#inner-2")).ToHaveCSSAsync("display", "none");

        // Right alignment reaches the outer bodies. (The nested body inherits text-align, which is how
        // text alignment is meant to behave inside a right-aligned region; its own layout is what the
        // assertions above prove is independent.)
        Assert.Equal("right", await BodyOf(page, "#outer-1").EvaluateAsync<string>("el => getComputedStyle(el).textAlign"));
    }

    private static ILocator LineOf(IPage page, string itemSelector)
        => page.Locator($"{itemSelector} > .{Css.Classes.Timeline.Connector} > .{Css.Classes.Timeline.Line}");

    private static ILocator BodyOf(IPage page, string itemSelector)
        => page.Locator($"{itemSelector} > .{Css.Classes.Timeline.Body}");
}
