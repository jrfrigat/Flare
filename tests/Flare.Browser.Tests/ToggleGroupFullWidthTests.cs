using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// FlareToggleGroup in a phone-wide column (Merito: a 295px dialog). FullWidth splits the column into equal
/// segments that fit it; without FullWidth the group keeps its natural width and scrolls.
/// </summary>
public sealed class ToggleGroupFullWidthTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private sealed record Geometry(double Container, double Group, double ScrollWidth, double[] Segments);

    private async Task<Geometry> MeasureAsync(bool full)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/toggle-group?full={full}");
        await Expect(page.Locator("#group")).ToBeVisibleAsync();
        // [container, group, scrollWidth, ...segments]
        var m = await page.EvaluateAsync<double[]>(@"() => {
            const group = document.getElementById('group');
            return [document.getElementById('narrow').getBoundingClientRect().width,
                group.getBoundingClientRect().width, group.scrollWidth,
                ...[...group.children].map(b => b.getBoundingClientRect().width)];
        }");
        return new Geometry(m[0], m[1], m[2], m[3..]);
    }

    [Fact]
    public async Task FullWidth_SplitsTheContainerIntoEqualSegments()
    {
        var g = await MeasureAsync(full: true);

        Assert.Equal(g.Container, g.Group, tolerance: 1.0);
        Assert.True(g.ScrollWidth <= g.Group + 1, $"group scrolls: {g.ScrollWidth} > {g.Group}");
        // Within the 1px frame the first and last segments draw on their outer edge.
        Assert.All(g.Segments, w => Assert.Equal(g.Segments[0], w, tolerance: 1.5));
        Assert.Equal(g.Group, g.Segments.Sum(), tolerance: 1.0);
    }

    [Fact]
    public async Task WithoutFullWidth_TheGroupScrollsInsteadOfShrinking()
    {
        var g = await MeasureAsync(full: false);

        Assert.True(g.ScrollWidth > g.Group + 1, $"expected overflow: {g.ScrollWidth} <= {g.Group}");
    }
}
