using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class FabMenuPlacementTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private static ILocator PanelIn(IPage page, string containerId) =>
        page.Locator($"#{containerId} .{Css.Classes.FabMenu.List}[data-flare-placed]");

    private async Task<IPage> OpenAsync(IBrowserContext context, int width)
    {
        var page = await context.NewPageAsync();
        await page.SetViewportSizeAsync(width, 720);
        await page.GotoAsync($"{fixture.BaseUrl}/fab-menu-placement");
        await Expect(page.Locator("#clipped-fab")).ToBeVisibleAsync();
        return page;
    }

    private static async Task<ILocator> OpenMenuAsync(IPage page, string containerId, string triggerId)
    {
        await page.Locator($"#{triggerId}").ClickAsync();
        var panel = PanelIn(page, containerId);
        await Expect(panel).ToBeVisibleAsync();
        return panel;
    }

    [Fact]
    public async Task TheMenuEscapesAClippingAncestorAndStaysInTheViewport()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 320);
        var panel = await OpenMenuAsync(page, "clipper", "clipped-fab");

        var card = await page.Locator("#clipper").BoundingBoxAsync();
        var box = await panel.BoundingBoxAsync();
        Assert.NotNull(box);
        Assert.True(box!.Y + box.Height > card!.Y + card.Height + 5,
            "the list has to leave the overflow:hidden card");
        Assert.True(box.X >= 0 && box.X + box.Width <= 320.5,
            "the list has to stay inside the viewport");
        Assert.True(box.Y >= 0 && box.Y + box.Height <= 720.5);
    }

    [Fact]
    public async Task ALongLabelWrapsInsideThePanelInsteadOfLeavingTheViewport()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 320);
        var panel = await OpenMenuAsync(page, "clipper", "clipped-fab");
        var label = page.Locator($"#long-item .{Css.Classes.FabMenu.Label}");

        await Expect(label).ToBeVisibleAsync();
        var wraps = await label.EvaluateAsync<bool>("el => el.scrollWidth <= el.clientWidth + 1");
        Assert.True(wraps, "the long label has to wrap inside the panel");

        var labelBox = await label.BoundingBoxAsync();
        var panelBox = await panel.BoundingBoxAsync();
        Assert.NotNull(labelBox);
        Assert.NotNull(panelBox);
        Assert.InRange(labelBox!.X, panelBox!.X - 0.5, panelBox.X + panelBox.Width);
        Assert.InRange(labelBox.X + labelBox.Width, panelBox.X - 0.5, panelBox.X + panelBox.Width + 0.5);
    }

    [Fact]
    public async Task TheSideFlipsWhenThePreferredOneHasNoRoom()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 320);
        var panel = await OpenMenuAsync(page, "edge", "edge-fab");

        await Expect(panel).ToHaveAttributeAsync("data-flare-side", "left");
        var box = await panel.BoundingBoxAsync();
        Assert.True(box!.X >= 0 && box.X + box.Width <= 320.5);
    }

    [Fact]
    public async Task StartAlignsToTheRightEdgeInRtl()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 800);

        await page.Locator("#enable-rtl").ClickAsync();
        await Expect(page.Locator($".{Css.Classes.Theme.Root}")).ToHaveAttributeAsync("dir", "rtl");

        var trigger = page.Locator("#rtl-fab");
        var panel = await OpenMenuAsync(page, "centered", "rtl-fab");
        await Expect(panel).ToHaveAttributeAsync("data-flare-side", "top");

        var triggerBox = await trigger.BoundingBoxAsync();
        var panelBox = await panel.BoundingBoxAsync();
        Assert.NotNull(triggerBox);
        Assert.NotNull(panelBox);
        Assert.InRange(panelBox!.X + panelBox.Width,
            triggerBox!.X + triggerBox.Width - 1.5,
            triggerBox.X + triggerBox.Width + 1.5);
    }

    [Fact]
    public async Task StartOnASideStaysTopAlignedInRtl()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 800);

        await page.Locator("#enable-rtl").ClickAsync();
        await Expect(page.Locator($".{Css.Classes.Theme.Root}")).ToHaveAttributeAsync("dir", "rtl");

        var trigger = page.Locator("#rtl-side-fab");
        var panel = await OpenMenuAsync(page, "side", "rtl-side-fab");
        await Expect(panel).ToHaveAttributeAsync("data-flare-side", "right");

        var triggerBox = await trigger.BoundingBoxAsync();
        var panelBox = await panel.BoundingBoxAsync();
        Assert.NotNull(triggerBox);
        Assert.NotNull(panelBox);
        Assert.InRange(panelBox!.Y, triggerBox!.Y - 1.5, triggerBox.Y + 1.5);
    }

    [Fact]
    public async Task TheMenuOpensDownwardWhenThereIsNoRoomAbove()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 800);
        var trigger = page.Locator("#top-fab");
        var panel = await OpenMenuAsync(page, "top-edge", "top-fab");
        await Expect(panel).ToHaveAttributeAsync("data-flare-side", "bottom");

        var triggerBox = await trigger.BoundingBoxAsync();
        var panelBox = await panel.BoundingBoxAsync();
        Assert.NotNull(triggerBox);
        Assert.NotNull(panelBox);
        Assert.True(panelBox!.Y >= triggerBox!.Y + triggerBox.Height - 1.5,
            "the list has to open below the FAB");
        Assert.True(panelBox.Y + panelBox.Height <= 720.5);
    }

    [Fact]
    public async Task AClosedMenuDoesNotExpandThePage()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, 320);

        Assert.True(await OverflowAsync(page) <= 0, "a closed menu must not add scroll");
        await OpenMenuAsync(page, "clipper", "clipped-fab");
        await page.Locator("#clipped-fab").ClickAsync();
        await Expect(page.Locator("#clipped-fab")).ToHaveAttributeAsync("aria-expanded", "false");

        Assert.True(await OverflowAsync(page) <= 0, "closing the menu must leave the page as it was");
    }

    private static Task<int> OverflowAsync(IPage page) =>
        page.EvaluateAsync<int>("document.documentElement.scrollWidth - document.documentElement.clientWidth");
}
