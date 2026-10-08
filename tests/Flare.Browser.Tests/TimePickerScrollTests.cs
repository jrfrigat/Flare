using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>Trusted browser input against real Blazor components, CSS and JS interop.</summary>
public sealed partial class TimePickerScrollTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    public static TheoryData<string> Themes => new()
    {
        "md3-expressive", "md3", "md2", "fluent2", "aero", "liquid-glass", "visualstudio"
    };

    [Theory]
    [MemberData(nameof(Themes))]
    public Task Wheel_SelectsShortAndLongColumns_WithoutAutoClose(string theme) => RunAsync(theme, false, async page =>
    {
        var picker = page.GetByTestId("short");
        await picker.GetByRole(AriaRole.Button).ClickAsync();
        await Selected(page, 0, 10);
        var scrollY = await page.EvaluateAsync<double>("window.scrollY");
        await Wheel(page, 1, 120, 45);
        await Wheel(page, 1, 120, 45); // End stops, rather than wrapping.
        await Wheel(page, 1, -120, 30);
        await Wheel(page, 2, 120, 30);
        await Wheel(page, 0, 120, 12);
        await Selected(page, 1, 30);
        await Selected(page, 2, 30);
        await Expect(page.GetByTestId("committed-short")).ToHaveTextAsync("10:30:20");
        Assert.Equal(scrollY, await page.EvaluateAsync<double>("window.scrollY"));
        await page.GetByRole(AriaRole.Group).PressAsync("Enter");
        await Expect(page.GetByTestId("committed-short")).ToHaveTextAsync("12:30:30");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);

        await page.GetByTestId("long").GetByRole(AriaRole.Button).ClickAsync();
        await Selected(page, 1, 30);
        await Wheel(page, 1, 120, 31);
        await Wheel(page, 1, -120, 30);
        await page.GetByRole(AriaRole.Group).PressAsync("Escape");
        await Expect(page.GetByTestId("committed-long")).ToHaveTextAsync("10:30:20");
        Assert.True(await page.EvaluateAsync<bool>("window.inputProof.wheel > 0"));
    });

    [Theory]
    [MemberData(nameof(Themes))]
    public Task Touch_SelectsShortAndLongColumns_AfterSwipeAndInertia(string theme) => RunAsync(theme, true, async page =>
    {
        await page.GetByTestId("short").GetByRole(AriaRole.Button).TapAsync();
        await Selected(page, 1, 30);
        var scrollY = await page.EvaluateAsync<double>("window.scrollY");
        await Swipe(page, 1, -90);
        await Selected(page, 1, 45);
        await Selected(page, 0, 10);
        await Selected(page, 2, 20);
        await Expect(page.GetByTestId("committed-short")).ToHaveTextAsync("10:30:20");
        await Swipe(page, 1, 100);
        // A fast swipe can move several items, but must stop on an available step.
        await page.WaitForFunctionAsync("Number(document.querySelector('[data-time-column=\"1\"]').dataset.timeCurrent) < 45");
        await Centered(page, 1);
        Assert.Equal(scrollY, await page.EvaluateAsync<double>("window.scrollY"));
        await page.GetByRole(AriaRole.Group).PressAsync("Escape");

        await page.GetByTestId("long").GetByRole(AriaRole.Button).TapAsync();
        await Selected(page, 1, 30);
        await Swipe(page, 1, -100);
        await page.WaitForFunctionAsync("Number(document.querySelector('[data-time-column=\"1\"]').dataset.timeCurrent) > 30");
        await Centered(page, 1);
        await Selected(page, 0, 10);
        await Selected(page, 2, 20);
        await Swipe(page, 0, -60);
        await page.WaitForFunctionAsync("Number(document.querySelector('[data-time-column=\"0\"]').dataset.timeCurrent) > 10");
        await Centered(page, 0);
        await Swipe(page, 2, -160);
        await page.WaitForFunctionAsync("Number(document.querySelector('[data-time-column=\"2\"]').dataset.timeCurrent) > 20");
        await Centered(page, 2);
        Assert.True(await page.EvaluateAsync<bool>("window.inputProof.touch > 0"));
        Assert.True(await page.EvaluateAsync<bool>("window.inputProof.afterReleaseDistance > 1"), "The browser must move after release, before .NET selection changes.");
        var proof = Path.Combine(AppContext.BaseDirectory, "TestResults", $"time-inertia-{theme}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(proof)!);
        await File.WriteAllTextAsync(proof, await page.EvaluateAsync<string>("JSON.stringify({ ...window.inputProof, released: null })"));
        await Expect(page.GetByTestId("committed-long")).ToHaveTextAsync("10:30:20");
        if (theme == "md3-expressive")
        {
            var screenshot = Path.Combine(AppContext.BaseDirectory, "TestResults", "time-picker-mobile.png");
            Directory.CreateDirectory(Path.GetDirectoryName(screenshot)!);
            await page.ScreenshotAsync(new() { Path = screenshot });
        }
    });

    [Fact]
    public Task Wheel_RespectsBounds_AndClickAutoCloseStillWorks() => RunAsync("md3-expressive", false, async page =>
    {
        await page.GetByTestId("short").GetByRole(AriaRole.Button).ClickAsync();
        await Selected(page, 0, 10);
        await Wheel(page, 0, -120, 8);
        await Wheel(page, 0, -120, 8); // Disabled hours below Min are skipped.
        await Wheel(page, 1, -120, 15);
        await Wheel(page, 1, -120, 15); // Minute zero is disabled at hour eight.
        await Wheel(page, 2, -120, 10);
        await Wheel(page, 2, -120, 10);
        await page.GetByRole(AriaRole.Group).PressAsync("Enter");
        await Expect(page.GetByTestId("committed-short")).ToHaveTextAsync("08:15:10");
        await page.GetByTestId("short").GetByRole(AriaRole.Button).ClickAsync();
        await Selected(page, 2, 10);
        await Column(page, 2).GetByRole(AriaRole.Option, new() { Name = "20", Exact = true }).ClickAsync();
        await Expect(page.GetByTestId("committed-short")).ToHaveTextAsync("08:15:20");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await page.GetByTestId("short").GetByRole(AriaRole.Button).ClickAsync();
        var group = page.GetByRole(AriaRole.Group);
        await group.PressAsync("Home"); // Hours receive initial focus on each opening.
        await group.PressAsync("End");
        await Selected(page, 0, 16);
        await group.PressAsync("ArrowRight");
        await group.PressAsync("End");
        await Selected(page, 1, 45);
        await group.PressAsync("ArrowRight");
        await group.PressAsync("End");
        await Selected(page, 2, 40);
        await Wheel(page, 0, 120, 16);
        await Wheel(page, 1, 120, 45);
        await Wheel(page, 2, 120, 40);
        await group.PressAsync("Enter");
        await Expect(page.GetByTestId("committed-short")).ToHaveTextAsync("16:45:40");
        await page.GetByRole(AriaRole.Button, new() { Name = "Toggle read only" }).ClickAsync();
        await Expect(page.GetByLabel("Short steps")).ToHaveAttributeAsync("readonly", "");
        await Expect(page.GetByTestId("short").GetByRole(AriaRole.Button)).ToBeDisabledAsync();
        await Expect(page.GetByLabel("Disabled time")).ToBeDisabledAsync();
    });

    private static ILocator Column(IPage page, int column) => page.Locator($"[data-time-column='{column}']");

    private static async Task Selected(IPage page, int column, int value)
    {
        await Expect(Column(page, column)).ToHaveAttributeAsync("data-time-current", value.ToString());
        await Expect(Column(page, column).Locator($"[data-time-value='{value}']")).ToHaveAttributeAsync("aria-selected", "true");
        await Centered(page, column);
    }

    private static async Task Centered(IPage page, int column) =>
        await page.WaitForFunctionAsync("""
            index => {
                const col = document.querySelector(`[data-time-column='${index}']`);
                const cell = col?.querySelector('[aria-selected="true"]');
                if (!cell) return false;
                const a = col.getBoundingClientRect(), b = cell.getBoundingClientRect();
                return Math.abs(b.top + b.height / 2 - a.top - col.clientHeight / 2) < 1;
            }
            """, column);

    private static async Task Wheel(IPage page, int column, int delta, int expected)
    {
        await Column(page, column).HoverAsync();
        await page.Mouse.WheelAsync(0, delta);
        await Selected(page, column, expected);
    }

    private static async Task Swipe(IPage page, int column, float distance)
    {
        var box = (await Column(page, column).BoundingBoxAsync())!;
        var x = box.X + box.Width / 2;
        var y = box.Y + box.Height / 2 - distance / 2;
        var cdp = await page.Context.NewCDPSessionAsync(page);
        try
        {
            await cdp.SendAsync("Input.dispatchTouchEvent", new()
            {
                ["type"] = "touchStart", ["touchPoints"] = new[] { new { x, y } }
            });
            for (var i = 1; i <= 5; i++)
            {
                await Task.Delay(16, TestContext.Current.CancellationToken);
                await cdp.SendAsync("Input.dispatchTouchEvent", new()
                {
                    ["type"] = "touchMove",
                    ["touchPoints"] = new[] { new { x, y = y + distance * i / 5 } }
                });
            }
            await cdp.SendAsync("Input.dispatchTouchEvent", new()
            {
                ["type"] = "touchEnd", ["touchPoints"] = Array.Empty<object>()
            });
        }
        finally { await cdp.DetachAsync(); }
    }

    private async Task RunAsync(string theme, bool mobile, Func<IPage, Task> test, string route = "/")
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = mobile ? 360 : 1280, Height = mobile ? 640 : 720 },
            HasTouch = mobile,
            IsMobile = mobile,
            Locale = "en-US"
        });
        await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });
        await context.AddInitScriptAsync("""
            window.inputProof = { wheel: 0, touch: 0, afterRelease: 0, afterReleaseDistance: 0, motions: [], released: null };
            document.addEventListener('wheel', e => { if (e.isTrusted) window.inputProof.wheel++; }, true);
            document.addEventListener('touchstart', e => {
                if (e.isTrusted) { window.inputProof.touch++; window.inputProof.released = null; }
            }, true);
            document.addEventListener('touchend', e => {
                const col = e.target.closest('[data-time-column]');
                if (col) {
                    window.inputProof.released = { col, top: col.scrollTop, current: col.dataset.timeCurrent, at: performance.now() };
                    window.inputProof.motions.push({ type: 'release', column: col.dataset.timeColumn, top: col.scrollTop, current: col.dataset.timeCurrent });
                }
            }, true);
            document.addEventListener('scroll', e => {
                const r = window.inputProof.released;
                if (e.target.matches?.('[data-time-column]')) window.inputProof.motions.push({ type: 'scroll', column: e.target.dataset.timeColumn, top: e.target.scrollTop, current: e.target.dataset.timeCurrent });
                // The component settles after 160 ms idle; earlier movement excludes its programmatic centering.
                if (r && e.target === r.col && performance.now() - r.at < 150 && r.current === r.col.dataset.timeCurrent && Math.abs(r.top - r.col.scrollTop) > 1) {
                    window.inputProof.afterRelease++;
                    window.inputProof.afterReleaseDistance = Math.max(window.inputProof.afterReleaseDistance, Math.abs(r.top - r.col.scrollTop));
                    window.inputProof.motions.push({ type: 'motion', column: r.col.dataset.timeColumn, top: r.col.scrollTop, current: r.col.dataset.timeCurrent, elapsed: performance.now() - r.at });
                }
            }, true);
            """);
        var page = await context.NewPageAsync();
        var errors = new List<string>();
        page.PageError += (_, error) => errors.Add(error);
        try
        {
            await page.GotoAsync(fixture.BaseUrl + route);
            await page.GetByLabel("Theme").SelectOptionAsync(theme);
            await Expect(page.Locator("[data-flare-theme]")).ToHaveAttributeAsync("data-flare-theme", theme);
            await test(page);
            Assert.Empty(errors);
            await context.Tracing.StopAsync();
        }
        catch
        {
            var path = Path.Combine(AppContext.BaseDirectory, "TestResults", $"time-{theme}-{mobile}-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path + ".json", await page.EvaluateAsync<string>("JSON.stringify({ ...window.inputProof, released: null })"));
            await page.ScreenshotAsync(new() { Path = path + ".png", FullPage = true });
            await context.Tracing.StopAsync(new() { Path = path + ".zip" });
            throw;
        }
    }
}
