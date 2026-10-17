using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A virtualized board (Aiko TASK-301): a column of 500 cards keeps only its visible window in the DOM, and a card
/// dragged onto the middle of that window lands beside the card it was dropped on, not at the position the window's
/// rendered cards count to.
/// </summary>
public sealed class KanbanVirtualTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task AColumnOfHundredsOfCards_KeepsOnlyItsWindowInTheDom()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/kanban-virtual");
        await Expect(page.Locator("[data-flare-drag=\"t0\"]")).ToBeVisibleAsync();

        var m = await page.EvaluateAsync<double[]>(@"() => {
            const list = document.querySelector('[data-flare-drop=""todo""] .flare-kanban__cards');
            return [list.querySelectorAll('[data-flare-drag]').length, list.clientHeight, list.scrollHeight];
        }");

        Assert.True(m[0] < 60, $"the column rendered {m[0]} of 500 cards");
        Assert.True(m[2] > m[1] * 10, "the column should scroll over the full height of its cards");
    }

    [Fact]
    public async Task ACardDroppedInAScrolledColumn_LandsBesideTheCardUnderThePointer()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/kanban-virtual");
        await Expect(page.Locator("[data-flare-drag=\"d0\"]")).ToBeVisibleAsync();

        await page.EvaluateAsync(@"() => {
            const list = document.querySelector('[data-flare-drop=""todo""] .flare-kanban__cards');
            list.scrollTop = list.scrollHeight / 2;
        }");
        // Which card ends up in the middle depends on when Virtualize replaced its size estimate with measured
        // heights, so the test takes whichever card is there. The window re-renders over the circuit after a
        // scroll; under load that lands late, so wait until the middle card stays the same before measuring it.
        await page.WaitForFunctionAsync(@"async () => {
            const list = document.querySelector('[data-flare-drop=""todo""] .flare-kanban__cards');
            const mid = () => {
                const r = list.getBoundingClientRect();
                return document.elementFromPoint(r.left + r.width / 2, r.top + r.height / 2)?.closest('[data-flare-drag]')?.dataset.flareDrag;
            };
            const first = mid();
            await new Promise(r => setTimeout(r, 250));
            return first !== undefined && first === mid();
        }");

        // The card in the middle of the column's visible area and a point in its upper half: "before" it.
        var target = await page.EvaluateAsync<double[]>(@"() => {
            const list = document.querySelector('[data-flare-drop=""todo""] .flare-kanban__cards');
            const r = list.getBoundingClientRect();
            const card = document.elementFromPoint(r.left + r.width / 2, r.top + r.height / 2).closest('[data-flare-drag]');
            const c = card.getBoundingClientRect();
            return [Number(card.dataset.flareDrag.slice(1)), c.left + c.width / 2, c.top + c.height / 4];
        }");
        Assert.True(target[0] > 100, $"the column should have scrolled into its middle, at t{target[0]}");

        var source = await page.Locator("[data-flare-drag=\"d0\"]").BoundingBoxAsync();
        await page.Mouse.MoveAsync(source!.X + source.Width / 2, source.Y + source.Height / 2);
        await page.Mouse.DownAsync();
        await page.Mouse.MoveAsync(source.X + source.Width / 2 + 10, source.Y + source.Height / 2, new() { Steps = 2 });
        await page.Mouse.MoveAsync((float)target[1], (float)target[2], new() { Steps = 8 });
        await page.Mouse.UpAsync();

        await Expect(page.Locator("#result")).ToHaveTextAsync($"d0@{(int)target[0]}");
    }
}
