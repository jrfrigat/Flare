using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A group of bars belongs to one loading area. The bars are decorative and stay out of the
/// accessibility tree; the area owns a single status. This is checked against the browser's
/// accessibility tree rather than the DOM alone.
/// </summary>
public sealed class SkeletonAccessibilityTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task DecorationsStayOutOfTheTreeAndOneStatusOwnsTheArea()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/skeleton");
        var skeleton = $"#loading-area .{Css.Classes.Skeleton.Root}";
        await Expect(page.Locator(skeleton).First).ToBeVisibleAsync();

        Assert.Equal(3, await page.Locator(skeleton).CountAsync());

        var snapshot = await page.Locator("#loading-area").AriaSnapshotAsync();

        Assert.Equal(1, snapshot.Split("status").Length - 1);
        Assert.Equal(1, snapshot.Split("Loading").Length - 1);
    }
}
