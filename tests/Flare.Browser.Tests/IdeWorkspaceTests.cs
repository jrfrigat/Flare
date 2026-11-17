using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class IdeWorkspaceTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Tabs_MeasureOverflowAndKeepEditorsThroughKeyboardReorderAndRename()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var bar = page.Locator("#narrow [role=tablist]");
        var tabs = bar.GetByRole(AriaRole.Tab);
        var firstId = await tabs.First.GetAttributeAsync("id") ?? throw new InvalidOperationException("Tab has no ID.");

        await Expect(page.Locator("#narrow button[aria-label='Scroll tabs forward']")).ToBeVisibleAsync();
        await ListenerWait.ForAsync(page, "#narrow [role=tablist]", "keydown");
        await tabs.First.FocusAsync();
        await page.Keyboard.PressAsync("Alt+ArrowLeft");
        await Expect(tabs.Nth(1)).ToHaveAttributeAsync("id", firstId);
        await Expect(tabs.Nth(1)).ToBeFocusedAsync();

        var editor = page.Locator("#first-tab-input");
        await editor.FillAsync("Uncommitted draft");
        await tabs.First.ClickAsync(new() { Button = MouseButton.Right });
        await Expect(page.GetByRole(AriaRole.Menu)).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Escape");
        await Expect(page.GetByRole(AriaRole.Menu)).ToHaveCountAsync(0);
        await tabs.First.ClickAsync(new() { Button = MouseButton.Right });
        await page.GetByRole(AriaRole.Menuitem, new() { Name = "Show document" }).ClickAsync();
        await Expect(page.Locator("#context-key")).ToHaveTextAsync("b");
        await tabs.First.ClickAsync();
        await Expect(page.Locator("#tab-key")).ToHaveTextAsync("b");
        await tabs.Nth(1).ClickAsync();
        await page.Locator("#rename").ClickAsync();
        await Expect(tabs.Nth(1)).ToContainTextAsync("renamed.cs");
        await Expect(tabs.Nth(1)).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(editor).ToHaveValueAsync("Uncommitted draft");
        var panelId = await tabs.Nth(1).GetAttributeAsync("aria-controls");
        await Expect(page.Locator($"[id='{panelId}']")).ToHaveAttributeAsync("role", "tabpanel");

        await page.Locator("#narrow").EvaluateAsync("el => el.style.width = '95vw'");
        await Expect(page.Locator("#narrow button[aria-label='Scroll tabs forward']")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Docking_MovesTheSameEditorByDragAndKeyboardAndKeepsLayoutsIndependent()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var editor = page.Locator("#editor-a");
        var dock = page.Locator("#dock");
        await editor.FillAsync("Persistent draft");

        // A side region without documents draws no dock strip, so its tool body is the drop target.
        var heading = dock.GetByRole(AriaRole.Button, new() { Name = "Document A", Exact = true });
        await heading.DragToAsync(page.Locator(
            "#other-dock .flare-ide-layout__panel--left .flare-ide-layout__panel-body"));
        await Expect(page.Locator("#other-moved")).ToHaveTextAsync("False");
        await heading.DragToAsync(dock.Locator(
            ".flare-ide-layout__panel--right .flare-ide-layout__panel-body"));
        var right = dock.Locator(".flare-ide-layout__dock-tabs[data-region=Right]");
        await Expect(right.GetByRole(AriaRole.Button, new() { Name = "Document A", Exact = true })).ToBeFocusedAsync();
        await Expect(dock.Locator(".flare-ide-layout__document[data-region=Right] #editor-a")).ToHaveValueAsync("Persistent draft");
        await Expect(page.Locator("#other-moved")).ToHaveTextAsync("False");

        await right.GetByRole(AriaRole.Button, new() { Name = "Move document: Document A" }).PressAsync("Enter");
        var menu = page.GetByRole(AriaRole.Menu);
        await Expect(menu).ToBeFocusedAsync();
        var firstAction = await menu.GetByRole(AriaRole.Menuitem, new() { Name = "Left tools" }).GetAttributeAsync("id");
        await Expect(menu).ToHaveAttributeAsync("aria-activedescendant", firstAction!);
        await page.Keyboard.PressAsync("ArrowDown");
        var secondAction = await menu.GetByRole(AriaRole.Menuitem, new() { Name = "Documents", Exact = true }).GetAttributeAsync("id");
        await Expect(menu).ToHaveAttributeAsync("aria-activedescendant", secondAction!);
        await page.Keyboard.PressAsync("Enter");
        await Expect(dock.Locator(".flare-ide-layout__document[data-region=Center] #editor-a")).ToHaveValueAsync("Persistent draft");
    }

    [Fact]
    public async Task Toolbar_ReordersGroupsWithPointerAndAccessibleActionsInRtl()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var toolbar = page.Locator("#toolbar");
        var handles = toolbar.Locator(".flare-toolbar__handle");
        var groups = toolbar.Locator(".flare-toolbar__group");

        await handles.First.DragToAsync(groups.Nth(1));
        await Expect(groups.First).ToHaveAttributeAsync("aria-label", "B");
        await handles.First.PressAsync("Enter");
        await Expect(page.GetByRole(AriaRole.Menu)).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Enter");
        await Expect(groups.First).ToHaveAttributeAsync("aria-label", "A");
    }

    [Fact]
    public async Task DiagnosticList_ActivatesTheSourcePositionFromAKeyboard()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        await Expect(page.Locator("#diagnostics").GetByRole(AriaRole.Columnheader, new() { Name = "Message" })).ToBeVisibleAsync();
        var grid = page.Locator("#diagnostics").GetByRole(AriaRole.Grid);
        var row = grid.GetByRole(AriaRole.Button, new() { Name = "Undefined symbol" });
        await Expect(row).ToBeVisibleAsync();
        Assert.InRange(await grid.GetByRole(AriaRole.Row).CountAsync(), 2, 100);
        await row.PressAsync("Enter");
        await Expect(page.Locator("#diagnostic-key")).ToHaveTextAsync("main:12:3");
        await Expect(row).ToHaveAttributeAsync("aria-pressed", "true");
    }

    [Fact]
    public async Task ShortcutCapture_LeavesTabAvailableAndDoesNotExecuteRegisteredCommands()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var first = page.Locator("#shortcut input");
        var second = page.Locator("#shortcut-other input");
        await ListenerWait.ForAsync(page, "#shortcut [data-flare-shortcut-capture]", "keydown");
        await ListenerWait.ForAsync(page, "#shortcut-other [data-flare-shortcut-capture]", "keydown");

        await first.PressAsync("Control+Shift+k");
        await Expect(page.Locator("#shortcut [role=status]")).ToHaveTextAsync("This shortcut is already assigned.");
        await Expect(first).ToHaveValueAsync("ctrl+shift+p");
        await Expect(page.Locator("#runs")).ToHaveTextAsync("0");

        await second.PressAsync("Meta+Shift+k");
        await Expect(second).ToHaveValueAsync("ctrl+shift+k");
        await second.PressAsync("Tab");
        await Expect(second).Not.ToBeFocusedAsync();
        await page.Locator("#shortcut-run").PressAsync("Control+Shift+k");
        await Expect(page.Locator("#runs")).ToHaveTextAsync("1");

        await page.Locator("#shortcut").GetByRole(AriaRole.Button, new() { Name = "Clear" }).ClickAsync();
        await Expect(first).ToHaveValueAsync("");
        await page.Locator("#shortcut").GetByRole(AriaRole.Button, new() { Name = "Reset" }).ClickAsync();
        await Expect(first).ToHaveValueAsync("ctrl+shift+p");
    }

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/ide-workspace");
        await Expect(page.Locator("#narrow [role=tab]")).ToHaveCountAsync(3);
        return page;
    }
}
