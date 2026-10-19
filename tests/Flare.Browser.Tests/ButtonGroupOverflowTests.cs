using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class ButtonGroupOverflowTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static string Panel => $".{Css.Classes.ButtonGroup.OverflowList}";
    private static string MoreButton => $".{Css.Classes.ButtonGroup.More} .{Css.Classes.Menu.Activator} button";

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/button-group");
        await Expect(page.Locator("#bold")).ToBeVisibleAsync();
        return page;
    }

    private static async Task FoldAsync(IPage page)
    {
        await page.Locator("#narrow").ClickAsync();
        await Expect(page.Locator(MoreButton)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task TheOverflowPanel_DoesNotDuplicateIdsAndKeepsItsReferencesInside()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await FoldAsync(page);

        await page.Locator(MoreButton).ClickAsync();
        await Expect(page.Locator(Panel)).ToBeVisibleAsync();

        var result = await page.EvaluateAsync<string[]>($@"() => {{
            const panel = document.querySelector('{Panel}');
            const ids = [...document.querySelectorAll('[id]')].map(e => e.id);
            const control = panel.querySelector('[aria-describedby]');
            const id = control?.getAttribute('aria-describedby');
            const target = id ? document.getElementById(id) : null;
            return [ids.join(','), String(ids.length === new Set(ids).size),
                String(!!target && panel.contains(target))];
        }}");

        Assert.Equal("true", result[1]);
        Assert.Equal("true", result[2]);
    }

    [Fact]
    public async Task AControlledToggle_KeepsItsStateAcrossTheFold()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await FoldAsync(page);
        await page.Locator(MoreButton).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Underline" }).ClickAsync();
        await Expect(page.Locator("#state")).ToHaveTextAsync("on");
        await Expect(page.Locator("#toggles")).ToHaveTextAsync("1");

        await page.EvaluateAsync("document.getElementById('sizer').style.width = '760px'");
        await Expect(page.Locator(MoreButton)).ToBeHiddenAsync();
        await Expect(page.Locator("#underline")).ToHaveAttributeAsync("aria-pressed", "true");

        await page.Locator("#underline").ClickAsync();
        await Expect(page.Locator("#toggles")).ToHaveTextAsync("2");
        await Expect(page.Locator("#state")).ToHaveTextAsync("off");
    }

    [Fact]
    public async Task TheUncontrolledCopies_AreIndependent()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await FoldAsync(page);
        await page.Locator(MoreButton).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Free" }).ClickAsync();
        await page.EvaluateAsync("document.getElementById('sizer').style.width = '760px'");
        await Expect(page.Locator(MoreButton)).ToBeHiddenAsync();

        await Expect(page.Locator("#free")).ToHaveAttributeAsync("aria-pressed", "false");
    }

    [Fact]
    public async Task FocusReturnsToTheGroup_WhenTheFoldDisappears()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await FoldAsync(page);
        await page.Locator(MoreButton).ClickAsync();
        await Expect(page.Locator(Panel)).ToBeVisibleAsync();

        await page.EvaluateAsync("document.getElementById('sizer').style.width = '760px'");

        await Expect(page.Locator("#bold")).ToBeFocusedAsync();
        await Expect(page.Locator(MoreButton)).ToBeHiddenAsync();
    }
}
