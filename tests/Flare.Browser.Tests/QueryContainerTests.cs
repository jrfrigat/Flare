using Flare.Css.Classes;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The container-width hide utility follows the nearest query container, not the window: a column that
/// is narrower than the md breakpoint hides its content even while the viewport is wide.
/// </summary>
public sealed class QueryContainerTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task ContainerBelowMd_FollowsTheContainer_NotTheViewport()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1440, Height = 800 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/container-hidden");

        var row = page.Locator($"#{RowId}");
        var target = page.Locator($"#{TargetId}");
        await Expect(target).ToBeVisibleAsync();

        await row.EvaluateAsync("el => el.style.inlineSize = '900px'");
        await Expect(target).ToBeHiddenAsync();

        await row.EvaluateAsync("el => el.style.inlineSize = '1200px'");
        await Expect(target).ToBeVisibleAsync();
    }

    private const string RowId = "cq-row";
    private const string TargetId = "cq-target";
}
