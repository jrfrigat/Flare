using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Two tool panels in one IDE region (Draftline STORY-11): they share the region's height and each scrolls its own
/// content, so a long list in the lower panel neither stretches the region nor carries the upper panel away.
/// </summary>
public sealed class IdeRegionPanelsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task TwoPanels_ShareTheRegion_AndScrollOnTheirOwn()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-region");
        await Expect(page.Locator("#screens")).ToBeVisibleAsync();

        // [body client, body scroll, panel1 h, panel2 h, list content client, list content scroll, props top before, props top after]
        var m = await page.EvaluateAsync<double[]>(@"async () => {
            const layout = document.getElementById('two');
            const body = layout.querySelector('.flare-ide-layout__panel--right .flare-ide-layout__panel-body');
            const panels = [...body.querySelectorAll(':scope > .flare-toolpanel')];
            const list = panels[1].querySelector('.flare-toolpanel__content');
            // The upper panel's top measured from the region's: it moves only if the region itself scrolls, not
            // when the theme's late styles change a header's height.
            const top = () => panels[0].getBoundingClientRect().top - body.getBoundingClientRect().top;
            const before = top();
            list.scrollTop = list.scrollHeight;
            await new Promise(r => requestAnimationFrame(() => requestAnimationFrame(r)));
            const after = top();
            return [body.clientHeight, body.scrollHeight, panels[0].getBoundingClientRect().height,
                panels[1].getBoundingClientRect().height, list.clientHeight, list.scrollHeight, before, after];
        }");

        Assert.True(m[1] <= m[0] + 1, $"the region body scrolls: {m[1]} > {m[0]}");
        Assert.Equal(m[2], m[3], tolerance: 1.0);
        Assert.Equal(m[0], m[2] + m[3], tolerance: 1.0);
        Assert.True(m[5] > m[4], "the lower panel's list should scroll inside it");
        Assert.Equal(m[6], m[7], tolerance: 0.5);
    }

    [Fact]
    public async Task OnePanel_StillFillsTheRegion()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-region");
        await Expect(page.Locator("#one")).ToBeVisibleAsync();

        var m = await page.EvaluateAsync<double[]>(@"() => {
            const body = document.querySelector('#one .flare-ide-layout__panel--right .flare-ide-layout__panel-body');
            return [body.clientHeight, body.querySelector('.flare-toolpanel').getBoundingClientRect().height];
        }");

        Assert.Equal(m[0], m[1], tolerance: 1.0);
    }
}
