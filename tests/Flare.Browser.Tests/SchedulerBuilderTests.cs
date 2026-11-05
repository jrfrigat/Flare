using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class SchedulerBuilderTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync(fixture.BaseUrl + "/scheduler-builder");
        await Expect(page.Locator("#builder .flare-scheduler__slot").First).ToHaveCSSAsync("height", "40px");
        await BrowserFonts.WaitAsync(page.Locator("#builder"));
        await ListenerWait.ForAsync(page, ".flare-scheduler__time-grid", "keydown");
        return page;
    }

    [Fact]
    public async Task CustomDrawerOwnsActivationAndPreservesDraftCultureFocusAndCancel()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var primary = page.Locator("[data-primary='one']");
        await primary.ClickAsync();
        await Expect(page.Locator("#requests")).ToHaveTextAsync("1");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(1);
        var title = page.GetByRole(AriaRole.Textbox, new() { Name = "Title", Exact = true });
        await Expect(title).ToHaveValueAsync("Planning");
        await title.FillAsync("Unsaved draft");
        await title.PressAsync("ArrowLeft");
        var caret = await title.EvaluateAsync<int>("e => e.selectionStart");
        await title.PressAsync("Control+Alt+l");
        title = page.GetByRole(AriaRole.Textbox, new() { Name = "\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435", Exact = true });
        await Expect(title).ToHaveValueAsync("Unsaved draft");
        await Expect(title).ToBeFocusedAsync();
        Assert.Equal(caret, await title.EvaluateAsync<int>("e => e.selectionStart"));
        await page.GetByRole(AriaRole.Button, new() { Name = "\u041e\u0442\u043c\u0435\u043d\u0430", Exact = true }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(page.Locator("#original")).ToHaveTextAsync("Planning");
        await Expect(page.Locator("#submissions")).ToHaveTextAsync("0");
        await Expect(primary).ToBeFocusedAsync();
        await page.Locator("#next").ClickAsync();
        await Expect(page.Locator("#date")).ToHaveTextAsync("2026-11-11");
        await Expect(page.Locator("#other-date")).ToHaveTextAsync("2026-11-04");
    }

    [Fact]
    public async Task RefusalFailureAndBusySubmissionKeepTheSameDraftUntilAccepted()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await page.Locator("#reject").ClickAsync();
        await page.Locator("[data-primary='one']").ClickAsync();
        var title = page.GetByRole(AriaRole.Textbox, new() { Name = "Title", Exact = true });
        await title.FillAsync("Retained draft");
        var save = page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true });
        await save.ClickAsync();
        await Expect(page.Locator("#submissions")).ToHaveTextAsync("1");
        await Expect(page.Locator("#editor [role=alert]")).ToBeVisibleAsync();
        await Expect(title).ToHaveValueAsync("Retained draft");
        await page.Locator("#editor-reject").ClickAsync();
        await page.Locator("#editor-error").ClickAsync();
        await save.ClickAsync();
        await Expect(page.Locator("#submissions")).ToHaveTextAsync("2");
        await Expect(page.Locator("#editor [role=alert]")).ToContainTextAsync("could not be saved");
        await Expect(title).ToHaveValueAsync("Retained draft");
        await Expect(page.Locator("#original")).ToHaveTextAsync("Planning");
        await page.Locator("#editor-slow").ClickAsync();
        await save.ClickAsync();
        await Expect(save).ToBeDisabledAsync();
        await page.Keyboard.PressAsync("Escape");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(page.Locator("#submissions")).ToHaveTextAsync("3");
        await Expect(page.Locator("#original")).ToHaveTextAsync("Retained draft");
    }

    [Fact]
    public async Task CustomEditorDoesNotInterceptMoveResizeOrTimelineAxisKeys()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var primary = page.Locator("[data-primary='one']");
        await primary.PressAsync("Control+ArrowDown");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:09:30-10:30:Occurrence:False");
        await Expect(primary).ToBeFocusedAsync();
        await primary.PressAsync("Control+Shift+ArrowDown");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Resize:09:30-11:00:Occurrence:False");
        await Expect(page.Locator("#requests")).ToHaveTextAsync("0");
        await page.Locator("#timeline").ClickAsync();
        var timelineAction = page.Locator(".flare-scheduler__timeline [data-primary='one']");
        await Expect(timelineAction).ToBeVisibleAsync();
        await ListenerWait.ForAsync(page, ".flare-gantt", "keydown");
        await timelineAction.PressAsync("Control+ArrowRight");
        await Expect(page.Locator("#change")).ToHaveTextAsync("Move:10:30-12:00:Occurrence:False");
        await Expect(page.Locator("#requests")).ToHaveTextAsync("0");
    }

    [Fact]
    public async Task SharedAllDayBandAlignsExpandsAndSupportsExternalOverflowAtTwoHundredPercentRtl()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 1440, Height = 1100 } });
        var page = await OpenAsync(context);
        var band = page.Locator(".flare-scheduler__all-day");
        var sticky = await page.Locator("#builder").EvaluateAsync<double[]>("""
            root => {
                const viewport = root.querySelector('.flare-scheduler__viewport');
                const header = root.querySelector('.flare-scheduler__header-band').getBoundingClientRect();
                const frame = root.querySelector('.flare-scheduler__time-grid-frame');
                return [header.top - viewport.getBoundingClientRect().top, frame.clientHeight - viewport.clientHeight];
            }
            """);
        Assert.InRange(Math.Abs(sticky[0]), 0, 1);
        Assert.True(sticky[1] > 0);
        await Expect(band.Locator(".flare-scheduler__all-day-event")).ToHaveCountAsync(1);
        var segment = band.Locator(".flare-scheduler__all-day-event");
        await Expect(segment).ToHaveAttributeAsync("data-scheduler-span", "2");
        await band.Locator(".flare-scheduler__all-day-more button").First.ClickAsync();
        await Expect(band.Locator(".flare-scheduler__all-day-event")).ToHaveCountAsync(4);
        await band.GetByRole(AriaRole.Button, new() { Name = "Show less", Exact = true }).ClickAsync();
        await page.Locator("#external-more").ClickAsync();
        await band.Locator(".flare-scheduler__all-day-more button").First.ClickAsync();
        await Expect(page.Locator("#hidden-events")).ToHaveTextAsync("day-1,day-2,day-3");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(1);
        await page.Locator("#close-more").ClickAsync();
        await page.Locator("#rtl").ClickAsync();
        await page.EvaluateAsync("document.documentElement.style.fontSize = '200%'");
        await BrowserFonts.WaitAsync(page.Locator("#builder"));
        var geometry = await page.Locator("#builder").EvaluateAsync<double[]>("""
            root => {
                const a = root.querySelector('[data-scheduler-column="2"]').getBoundingClientRect();
                const days = [...root.querySelectorAll('.flare-scheduler__day')].map(e => e.getBoundingClientRect());
                const slot = root.querySelectorAll('.flare-scheduler__all-day-target')[2].getBoundingClientRect();
                return [a.right - days[2].right, a.left - days[3].left, slot.left - days[2].left];
            }
            """);
        Assert.All(geometry, difference => Assert.InRange(Math.Abs(difference), 0, 1));
        Assert.DoesNotContain("auto", await band.EvaluateAsync<string>("e => getComputedStyle(e).overflowY"));
        var background = band.Locator(".flare-scheduler__all-day-slot");
        await background.Nth(2).FocusAsync();
        await background.Nth(2).PressAsync("ArrowLeft");
        await Expect(background.Nth(3)).ToBeFocusedAsync();
    }
}
