using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The page header (Alva TASK-57) sizes itself by its own width, not the screen's: a wide header keeps its actions
/// beside the text, a narrow one moves them under it and uses a smaller title.
/// </summary>
public sealed class PageHeaderLayoutTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    // [text bottom, actions top, actions left, text right, title font size]
    private const string Measure = @"id => {
        const h = document.getElementById(id);
        const text = h.querySelector('.flare-page-header__text').getBoundingClientRect();
        const actions = h.querySelector('.flare-page-header__actions').getBoundingClientRect();
        const title = h.querySelector('.flare-page-header__title');
        return [text.bottom, actions.top, actions.left, text.right, parseFloat(getComputedStyle(title).fontSize)];
    }";

    [Fact]
    public async Task AWideHeader_KeepsItsActionsBesideTheText_AndANarrowOneMovesThemUnder()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/page-header");
        await Expect(page.Locator($"#narrow .{Css.Classes.PageHeader.Title}")).ToBeVisibleAsync();
        // The theme's tokens land after the first render; until then every title reads the browser default.
        await page.WaitForFunctionAsync(
            $"parseFloat(getComputedStyle(document.querySelector('#wide .{Css.Classes.PageHeader.Title}')).fontSize) > 16");

        var wide = await page.EvaluateAsync<double[]>(Measure, "wide");
        var narrow = await page.EvaluateAsync<double[]>(Measure, "narrow");

        Assert.True(wide[2] >= wide[3] - 0.5, $"wide: the actions should sit after the text ({wide[2]} < {wide[3]})");
        Assert.True(narrow[1] >= narrow[0] - 0.5, $"narrow: the actions should sit under the text ({narrow[1]} < {narrow[0]})");
        Assert.True(narrow[4] < wide[4], $"narrow: the title should be smaller ({narrow[4]} vs {wide[4]})");
    }
}
