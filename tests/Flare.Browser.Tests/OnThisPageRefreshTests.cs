using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The table of contents follows content that changes after the first render: a refresh picks up new
/// headings, a root that is not in the page yields an empty list instead of the page's headings, and a
/// root that comes back is collected again.
/// </summary>
public sealed class OnThisPageRefreshTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task RefreshAndRootChanges_RescanTheContent()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 900, Height = 700 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/on-this-page-refresh");
        var links = page.Locator($".{Css.Classes.TableOfContents.Link}");

        // Only headings inside the named root count - the page's own "Outside" heading stays out.
        await Expect(links).ToHaveCountAsync(2);
        await Expect(links.Nth(0)).ToHaveTextAsync("First");
        await Expect(links.Nth(1)).ToHaveTextAsync("Second");

        // A heading rendered after the first scan appears once the app refreshes the list.
        await page.Locator("#add").ClickAsync();
        await page.Locator("#refresh").ClickAsync();
        await Expect(links).ToHaveCountAsync(3);
        await Expect(links.Nth(2)).ToHaveTextAsync("Third");

        // A root that is not in the page is an empty table of contents, not a scan of the whole document.
        await page.Locator("#missing").ClickAsync();
        await Expect(links).ToHaveCountAsync(0);

        // A real root again is picked up by the automatic re-registration.
        await page.Locator("#real").ClickAsync();
        await Expect(links).ToHaveCountAsync(3);
    }
}
