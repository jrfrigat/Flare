using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class FieldLabelTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task CaptionsRemainAssociatedAcrossModesAndRenders()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        var issues = new List<string>();
        var cdp = await context.NewCDPSessionAsync(page);
        cdp.Event("Audits.issueAdded").OnEvent += (_, e) => issues.Add(e.ToString() ?? string.Empty);
        await cdp.SendAsync("Audits.enable");
        await page.GotoAsync($"{fixture.BaseUrl}/field-labels");
        await Expect(page.GetByRole(AriaRole.Combobox, new() { Name = "Choice", Exact = true })).ToBeVisibleAsync();
        await AssertLabelsAsync(page);
        var names = page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true });
        await Expect(names).ToHaveCountAsync(2);
        var ids = await names.EvaluateAllAsync<string[]>("xs => xs.map(x=>x.id)");
        Assert.Equal(2, ids.Distinct().Count());
        await page.GetByText("Name", new() { Exact = true }).First.ClickAsync();
        await Expect(names.First).ToBeFocusedAsync();
        await page.Locator("#mode").ClickAsync();
        var input = page.GetByRole(AriaRole.Combobox, new() { Name = "Choice", Exact = true });
        await Expect(input).ToHaveAttributeAsync("id", new System.Text.RegularExpressions.Regex(".+"));
        var nativeId = await input.GetAttributeAsync("id");
        await AssertLabelsAsync(page);
        await page.Locator("#rename").ClickAsync();
        await Expect(page.GetByRole(AriaRole.Combobox, new() { Name = "Destination", Exact = true })).ToHaveAttributeAsync("id", nativeId!);
        await page.GetByText("Destination", new() { Exact = true }).ClickAsync();
        await Expect(page.Locator("#" + nativeId)).ToBeFocusedAsync();
        await page.Locator("#readonly").ClickAsync();
        await page.Locator("#mode").ClickAsync();
        await AssertLabelsAsync(page);
        Assert.DoesNotContain(issues, issue => issue.Contains("FormLabel", StringComparison.Ordinal));
    }

    private static async Task AssertLabelsAsync(IPage page)
    {
        Assert.Empty(await page.Locator("label").EvaluateAllAsync<string[]>("xs => xs.filter(x=>!x.control).map(x=>x.outerHTML)"));
        Assert.Empty(await page.Locator("[aria-labelledby]").EvaluateAllAsync<string[]>(
            "xs => xs.flatMap(x=>x.getAttribute('aria-labelledby').split(/\\s+/)).filter(id=>id&&!document.getElementById(id))"));
    }
}
