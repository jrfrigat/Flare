using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class StepperLayoutTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    public static IEnumerable<object[]> Cases =>
        from theme in new[] { "md3-expressive", "fluent2" }
        from rtl in new[] { false, true }
        from russian in new[] { false, true }
        select new object[] { theme, rtl, russian };

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IndicatorsAndConnectors_StayAligned_WithUnequalLabels(string theme, bool rtl, bool russian)
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1100, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/stepper?rtl={rtl}&russian={russian}");
        await page.GetByLabel("Theme", new() { Exact = true }).SelectOptionAsync(theme);
        await Expect(page.Locator("main")).ToHaveAttributeAsync("data-ready-theme", theme);
        foreach (var width in new[] { 280, 900 })
        foreach (var size in new[] { 24, 32, 48 })
        {
            await page.GetByLabel("Width", new() { Exact = true }).SelectOptionAsync(width.ToString());
            await page.GetByLabel("Circle size", new() { Exact = true }).SelectOptionAsync(size.ToString());
            var horizontal = page.GetByTestId("horizontal");
            await Expect(horizontal).ToHaveAttributeAsync("style", $"width:{width}px");
            await Expect(horizontal.Locator($".{Css.Classes.Stepper.Root}")).ToHaveAttributeAsync("style",
                $"{Css.Tokens.Stepper.CircleSize}:{size}px;{Css.Tokens.Stepper.ConnectorThickness}:3px");
            await CheckGeometry(horizontal, false, size);
            await CheckGeometry(page.GetByTestId("vertical"), true, size);

            var tabs = horizontal.GetByRole(AriaRole.Tab);
            await tabs.First.FocusAsync();
            await tabs.First.PressAsync("End");
            await Expect(tabs.Last).ToBeFocusedAsync();
            await Expect(tabs.First).ToHaveAttributeAsync("aria-selected", "true");
            await tabs.Last.PressAsync("Enter");
            await Expect(tabs.Last).ToHaveAttributeAsync("aria-selected", "true");
            await CheckGeometry(horizontal, false, size);
            await Expect(tabs.Last).ToBeInViewportAsync();
            await tabs.Last.PressAsync("Home");
            await Expect(tabs.First).ToBeFocusedAsync();
            await tabs.First.PressAsync("Enter");
            await Expect(tabs.First).ToHaveAttributeAsync("aria-selected", "true");
        }
    }

    private static async Task CheckGeometry(ILocator subject, bool vertical, int size)
    {
        var geometry = await subject.EvaluateAsync<double[]>("""
            (root, args) => {
                const circles = [...root.querySelectorAll('.' + args.circle)].map(e => e.getBoundingClientRect());
                const lines = [...root.querySelectorAll('.' + args.connector)].map(e => e.getBoundingClientRect());
                const center = r => args.vertical ? r.x + r.width / 2 : r.y + r.height / 2;
                const centers = [...circles, ...lines].map(center);
                const labels = [...root.querySelectorAll('.' + args.labels)].map(e => e.getBoundingClientRect().height);
                return [Math.max(...centers) - Math.min(...centers),
                    Math.max(...circles.map(r => Math.abs(r.width - args.size))),
                    Math.max(...circles.map(r => Math.abs(r.height - args.size))),
                    Math.max(...labels) - Math.min(...labels)];
            }
            """, new { circle = Css.Classes.Stepper.Circle, connector = Css.Classes.Stepper.Connector,
                labels = Css.Classes.Stepper.Labels, vertical, size });
        Assert.True(geometry[3] > 10, "The fixture must contain labels of unequal heights.");
        Assert.True(geometry[0] <= 1, $"Indicator/connector centers differ by {geometry[0]}px (vertical={vertical}, size={size}).");
        Assert.True(geometry[1] <= 1 && geometry[2] <= 1, "Indicator borders must fit inside the configured circle size.");
    }
}
