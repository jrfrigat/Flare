using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class DockTabCompressionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private const string First = "First long database report";
    private const string Second = "Second long database report";
    private const string Iconless = "Notes without a document icon";
    private const string Fourth = "Fourth long database report";
    private const int CustomCompactWidth = 320;

    [Theory]
    [InlineData("dock-adaptive", false)]
    [InlineData("dock-custom", true)]
    public async Task TabsProgressFromTruncatedLabelsToIconsToOverflowAndBack(string id, bool custom)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-overflow");
        var frame = page.Locator($"#{id}-frame");
        var workspace = page.Locator($"#{id}");
        var tab = workspace.GetByRole(AriaRole.Tab, new() { Name = First, Exact = true });
        var label = tab.Locator(".flare-dock__label");
        var arrows = workspace.Locator($".{Css.Classes.Tabs.Scroll}");
        var next = workspace.Locator($".{Css.Classes.Tabs.ScrollNext}");
        var compactWidth = custom ? CustomCompactWidth : 360;

        await Expect(label).ToBeVisibleAsync();
        var fullWidth = (await label.BoundingBoxAsync())!.Width;
        await SetWidthAsync(frame, 550);
        await Expect(label).ToBeVisibleAsync();
        Assert.True((await label.BoundingBoxAsync())!.Width < fullWidth);
        await Expect(arrows).ToHaveCountAsync(0);

        await SetWidthAsync(frame, compactWidth);
        await Expect(label).ToBeHiddenAsync();
        var icon = custom ? $".flare-dock__compact-icon .{Css.Classes.Icon.Root}" : $".flare-dock__grip .{Css.Classes.Icon.Root}";
        await Expect(tab.Locator(icon)).ToBeVisibleAsync();
        await Expect(tab).ToHaveAttributeAsync("title", First);
        var iconless = workspace.GetByRole(AriaRole.Tab, new() { Name = Iconless });
        if (custom)
        {
            await Expect(iconless.GetByText(Iconless, new() { Exact = true }).Filter(new() { Visible = true }))
                .ToBeVisibleAsync();
        }
        else
        {
            await Expect(iconless.Locator(".flare-dock__label")).ToBeHiddenAsync();
            await Expect(iconless.Locator(icon)).ToBeVisibleAsync();
        }
        await Expect(next).ToBeHiddenAsync();

        await SetWidthAsync(frame, custom ? 220 : 120);
        await Expect(next).ToBeEnabledAsync();
        await next.ClickAsync();
        var bar = workspace.GetByRole(AriaRole.Tablist);
        await page.WaitForFunctionAsync("el => el.scrollLeft > 1", await bar.ElementHandleAsync());

        await SetWidthAsync(frame, compactWidth);
        await Expect(arrows).ToHaveCountAsync(0);
        await Expect(label).ToBeHiddenAsync();
        await SetWidthAsync(frame, 1200);
        await Expect(label).ToBeVisibleAsync();
        Assert.InRange((await label.BoundingBoxAsync())!.Width, fullWidth - 1, fullWidth + 1);
        if (custom)
            await Expect(tab.Locator("[data-test-title]")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task CompactTabsRetainCustomActionsKeyboardAndDrag()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-overflow");
        var workspace = page.Locator("#dock-custom");
        var first = workspace.GetByRole(AriaRole.Tab, new() { Name = First });
        var second = workspace.GetByRole(AriaRole.Tab, new() { Name = Second });
        await Expect(first).ToBeVisibleAsync();
        await SetWidthAsync(page.Locator("#dock-custom-frame"), CustomCompactWidth);
        await Expect(first.Locator("[data-test-title]")).ToBeHiddenAsync();

        await first.PressAsync("ArrowRight");
        await Expect(second).ToBeFocusedAsync();
        await second.PressAsync("Enter");
        await Expect(second).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(first.Locator($".flare-dock__compact-icon .{Css.Classes.Icon.Root}")).ToBeVisibleAsync();

        var source = (await first.Locator(".flare-dock__grip").BoundingBoxAsync())!;
        var fourth = workspace.GetByRole(AriaRole.Tab, new() { Name = Fourth });
        var target = (await fourth.BoundingBoxAsync())!;
        await page.Mouse.MoveAsync(source.X + source.Width / 2, source.Y + source.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(target.X + target.Width * 0.8f, target.Y + target.Height / 2, new() { Steps = 12 });
        await page.Mouse.UpAsync();
        await Expect(workspace.GetByRole(AriaRole.Tab).Last).ToHaveAccessibleNameAsync(First);

        await workspace.GetByRole(AriaRole.Button, new() { Name = "Close from template" }).ClickAsync();
        await Expect(first).ToHaveCountAsync(0);
        await second.PressAsync("Delete");
        await Expect(second).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task CompressionCanBeTurnedOffAndOn()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-overflow");
        var workspace = page.Locator("#dock-custom");
        var tab = workspace.GetByRole(AriaRole.Tab, new() { Name = First });
        var template = tab.Locator("[data-test-title]");
        var toggle = page.GetByRole(AriaRole.Button, new() { Name = "Toggle tab compression" });
        var next = workspace.Locator($".{Css.Classes.Tabs.ScrollNext}");
        await Expect(tab).ToBeVisibleAsync();
        await SetWidthAsync(page.Locator("#dock-custom-frame"), CustomCompactWidth);
        await Expect(template).ToBeHiddenAsync();
        await Expect(next).ToBeHiddenAsync();

        await toggle.ClickAsync();
        await Expect(template).ToBeVisibleAsync();
        await Expect(next).ToBeEnabledAsync();

        await toggle.ClickAsync();
        await Expect(template).ToBeHiddenAsync();
        await Expect(next).ToBeHiddenAsync();
    }

    [Fact]
    public async Task SelectedGroupsCompressIndependentlyWithoutRemounting()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/dock-tab-overflow");
        var workspace = page.Locator("#dock-selective");
        var left = workspace.GetByRole(AriaRole.Tab, new() { Name = First });
        var right = workspace.GetByRole(AriaRole.Tab, new() { Name = Fourth });
        var leftLabel = left.Locator(".flare-dock__label");
        var rightLabel = right.Locator(".flare-dock__label");
        var leftBar = left.Locator("xpath=ancestor::*[@role='tablist']");

        await Expect(left).ToHaveAttributeAsync("title", First);
        await Expect(leftLabel).ToBeHiddenAsync();
        await Expect(right).Not.ToHaveAttributeAsync("title", Fourth);
        await Expect(rightLabel).ToBeVisibleAsync();
        var leftId = await left.GetAttributeAsync("id");

        await page.GetByRole(AriaRole.Button, new() { Name = "Switch compressed group" }).ClickAsync();

        await Expect(left).ToHaveAttributeAsync("id", leftId!);
        await Expect(left).Not.ToHaveAttributeAsync("title", First);
        await Expect(leftLabel).ToBeVisibleAsync();
        await Expect(leftBar).Not.ToHaveAttributeAsync("data-flare-dock-compact", string.Empty);
        await Expect(right).ToHaveAttributeAsync("title", Fourth);
        await Expect(rightLabel).ToBeHiddenAsync();
    }

    private static Task SetWidthAsync(ILocator frame, int width) =>
        frame.EvaluateAsync("(el, width) => { el.style.width = width + 'px'; }", width);
}
