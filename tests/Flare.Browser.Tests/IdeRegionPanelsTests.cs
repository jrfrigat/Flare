using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// Two tool panels in one IDE region (Draftline STORY-11): they share the region's height and each scrolls its own
/// content, so a long list in the lower panel neither stretches the region nor carries the upper panel away.
/// </summary>
public sealed class IdeRegionPanelsTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SharedSplittersResizeAllRegionsAndKeepSizesAfterRerender(bool touch)
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { HasTouch = touch });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-region");
        var handles = page.Locator($"#resize .{Css.Classes.Splitter.Root}");
        await Expect(handles).ToHaveCountAsync(3);
        await ListenerWait.ForAsync(page, $"#resize .{Css.Classes.Splitter.Root}", "keydown");
        var left = page.Locator($"#resize .{Css.Classes.Ide.Layout.PanelLeft}");
        var right = page.Locator($"#resize .{Css.Classes.Ide.Layout.PanelRight}");
        var bottom = page.Locator($"#resize .{Css.Classes.Ide.Layout.Bottom}");
        await handles.Nth(0).FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(left).ToHaveCSSAsync("width", "188px");
        var rightBefore = (await right.BoundingBoxAsync())!.Width;
        await handles.Nth(1).FocusAsync();
        await page.Keyboard.PressAsync("Shift+ArrowLeft");
        await Expect(right).ToHaveCSSAsync("width", $"{rightBefore + 24}px");
        await handles.Nth(2).FocusAsync();
        await page.Keyboard.PressAsync("ArrowUp");
        await Expect(bottom).ToHaveCSSAsync("height", "168px");
        await handles.Nth(2).ScrollIntoViewIfNeededAsync();
        var box = (await handles.Nth(2).BoundingBoxAsync())!;
        var x = box.X + box.Width / 2;
        var y = box.Y + box.Height / 2;
        if (touch)
        {
            var cdp = await context.NewCDPSessionAsync(page);
            await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchStart", ["touchPoints"] = new[] { new { x, y } } });
            await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchMove", ["touchPoints"] = new[] { new { x, y = y - 32 } } });
            await cdp.SendAsync("Input.dispatchTouchEvent", new() { ["type"] = "touchEnd", ["touchPoints"] = Array.Empty<object>() });
        }
        else
        {
            await page.Mouse.MoveAsync(x, y);
            await page.Mouse.DownAsync();
            await page.Mouse.MoveAsync(x, y - 32, new() { Steps = 6 });
            await page.Mouse.UpAsync();
        }
        await Expect(bottom).ToHaveCSSAsync("height", "200px");
        await page.Locator("#rerender").ClickAsync();
        await Expect(bottom).ToHaveCSSAsync("height", "200px");
        await Expect(left).ToHaveCSSAsync("width", "188px");
        await Expect(handles.Nth(2)).ToHaveAttributeAsync("aria-valuenow", "200");
    }

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
