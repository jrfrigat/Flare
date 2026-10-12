using Flare.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>Real pointer coordinates and top-layer placement against the chart's SVG hit geometry.</summary>
public sealed class ChartTooltipTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    public static IEnumerable<object[]> Cases =>
        from theme in new[] { "md3-expressive", "fluent2" }
        from dark in new[] { false, true }
        from type in Enum.GetNames<ChartType>()
        select new object[] { theme, dark, type };

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task Tooltip_FollowsCurrentPointer_InScaledSvg(string theme, bool dark, string type)
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 1440, Height = 1000 } });
        await context.AddInitScriptAsync("window.hideFlareSplash = () => window.flareTestReady = true;");
        var page = await context.NewPageAsync();
        var errors = new List<string>();
        page.PageError += (_, error) => errors.Add(error);
        await page.GotoAsync(fixture.BaseUrl + "/chart");
        await page.WaitForFunctionAsync("window.flareTestReady === true");
        await page.GetByRole(AriaRole.Combobox, new() { Name = "Theme", Exact = true }).SelectOptionAsync(theme);
        await page.GetByRole(AriaRole.Combobox, new() { Name = "Mode", Exact = true }).SelectOptionAsync(dark ? "Dark" : "Light");
        await page.GetByRole(AriaRole.Combobox, new() { Name = "Chart type", Exact = true }).SelectOptionAsync(type);
        await Expect(page.Locator("[data-flare-theme]")).ToHaveAttributeAsync("data-flare-theme", theme);
        await page.WaitForTimeoutAsync(250); // Initial size observation can replace SVG hit elements.
        var chart = page.GetByTestId("subject");
        await chart.EvaluateAsync("el => {el.style.transform='scale(.8)';el.style.transformOrigin='top left';}");
        await chart.ScrollIntoViewIfNeededAsync();
        var point = await InteriorAsync(chart);
        await page.Mouse.MoveAsync(point[0], point[1]);
        await FollowsAsync(chart, point[0], point[1]);
        // Staying in the same hit zone is the 0.43.0 regression: content stays equal but coordinates change.
        for (int i = 1; i <= 5; i++)
        {
            float dx = type == "Scatter" ? .1f : .5f, dy = type == "Scatter" ? .1f : .8f;
            await page.Mouse.MoveAsync(point[0] + i * dx, point[1] + i * dy);
            await FollowsAsync(chart, point[0] + i * dx, point[1] + i * dy);
        }
        await page.Mouse.MoveAsync(1400, 900);
        await Expect(chart.GetByRole(AriaRole.Tooltip)).ToHaveCountAsync(0);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ScrollResizeDialogAndLongLegend_KeepDetailsReachable()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 1000, Height = 900 } });
        await context.AddInitScriptAsync("window.hideFlareSplash = () => window.flareTestReady = true;");
        var page = await context.NewPageAsync();
        await page.GotoAsync(fixture.BaseUrl + "/chart");
        await page.WaitForFunctionAsync("window.flareTestReady === true");
        var nested = page.GetByTestId("nested");
        await nested.ScrollIntoViewIfNeededAsync();
        var point = await InteriorAsync(nested);
        await page.Mouse.MoveAsync(point[0], point[1]);
        await FollowsAsync(nested, point[0], point[1]);
        await page.Mouse.WheelAsync(0, 35);
        await page.WaitForFunctionAsync("document.querySelector('[data-testid=scroll]').scrollTop > 0");
        point = await InteriorAsync(nested);
        await page.Mouse.MoveAsync(point[0], point[1]);
        await FollowsAsync(nested, point[0], point[1]);
        await page.GetByTestId("subject").ScrollIntoViewIfNeededAsync();
        var other = await InteriorAsync(page.GetByTestId("subject"));
        await page.Mouse.MoveAsync(other[0], other[1]);
        await FollowsAsync(page.GetByTestId("subject"), other[0], other[1]);
        await Expect(nested.GetByRole(AriaRole.Tooltip)).ToHaveCountAsync(0);
        await page.GetByTestId("open").ClickAsync();
        var dialog = page.GetByTestId("dialog-chart");
        await Expect(dialog).ToBeVisibleAsync();
        point = await InteriorAsync(dialog);
        await page.Mouse.MoveAsync(point[0], point[1]);
        await FollowsAsync(dialog, point[0], point[1]);
        Assert.True(await dialog.GetByRole(AriaRole.Tooltip).EvaluateAsync<bool>("el => el.matches(':popover-open')"));
        await page.Keyboard.PressAsync("Escape");
        await Expect(page.GetByRole(AriaRole.Dialog)).ToBeHiddenAsync();
        await page.SetViewportSizeAsync(375, 900);
        await page.GetByTestId("long").GetByRole(AriaRole.Switch).CheckAsync();
        var chart = page.GetByTestId("subject");
        var labels = chart.Locator($".{Css.Classes.Chart.LegendItem} .{Css.Classes.Button.Label}");
        await Expect(labels).ToHaveCountAsync(3);
        Assert.True(await labels.EvaluateAllAsync<bool>("els => els.every(e => e.scrollWidth <= e.clientWidth + 1 && e.scrollHeight <= e.clientHeight + 1)"));
        point = await InteriorAsync(chart);
        await page.Mouse.MoveAsync(point[0], point[1]);
        await Expect(chart.GetByRole(AriaRole.Tooltip)).ToBeVisibleAsync();
        Assert.True(await chart.GetByRole(AriaRole.Tooltip).EvaluateAsync<bool>("el => {const r=el.getBoundingClientRect();return r.left>=0&&r.right<=innerWidth&&r.top>=0&&r.bottom<=innerHeight;}"));
    }

    [Fact]
    public async Task ViewportEdges_FlipAndClampWithoutCoveringPointer()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 640, Height = 480 } });
        await context.AddInitScriptAsync("window.hideFlareSplash = () => window.flareTestReady = true;");
        var page = await context.NewPageAsync();
        await page.GotoAsync(fixture.BaseUrl + "/chart");
        await page.WaitForFunctionAsync("window.flareTestReady === true");
        var chart = page.GetByTestId("subject");
        // The fixture isolates viewport collisions from the Gallery's unrelated page chrome.
        await chart.EvaluateAsync("el => {el.style.position='fixed';el.style.right='0';el.style.top='0';el.style.width='280px';el.style.zIndex='100';}");
        await page.WaitForTimeoutAsync(250);
        var point = await chart.Locator("[data-flare-chart-hit]").Last.EvaluateAsync<float[]>("""
            el => {const b=el.getBBox(),p=new DOMPoint(b.x+b.width-2,b.y+2).matrixTransform(el.getScreenCTM());return [p.x,p.y];}
            """);
        await page.Mouse.MoveAsync(point[0], point[1]);
        var tip = chart.GetByRole(AriaRole.Tooltip);
        await Expect(tip).ToBeVisibleAsync();
        var box = (await tip.BoundingBoxAsync())!;
        Assert.InRange(box.X, 0, 640 - box.Width);
        Assert.InRange(box.Y, 0, 480 - box.Height);
        Assert.True(box.Y >= point[1] + 7); // The top edge forces bottom placement.
        Assert.True(box.X + box.Width / 2 < point[0] - 2); // The right edge forces horizontal clamping.
    }

    private static Task<float[]> InteriorAsync(ILocator chart) => chart.Locator("[data-flare-chart-hit]").EvaluateAllAsync<float[]>("""
        els => {
            for (const el of els) {
                const b=el.getBBox(), m=el.getScreenCTM();
                for(const iy of [5,4,6,3,7,2,8,1,9])for(const ix of [5,4,6,3,7,2,8,1,9]) {
                    const p=new DOMPoint(b.x+b.width*ix/10,b.y+b.height*iy/10);
                    if(!el.isPointInFill(p))continue;
                    const q=p.matrixTransform(m);
                    if([[0,0],[.6,.6],[-.6,-.6]].every(([x,y])=>document.elementFromPoint(q.x+x,q.y+y)===el))return [q.x,q.y];
                }
            }
            throw Error('No visible interior SVG hit');
        }
        """);

    private static Task FollowsAsync(ILocator chart, float x, float y) => chart.EvaluateAsync("""
        async (chart,p) => {
            const matches=()=>{const el=chart.querySelector('[role=tooltip]');if(!el)return false;const r=el.getBoundingClientRect(),hit=chart.querySelector('[data-flare-chart-hit][aria-describedby]');return hit?.getAttribute('aria-describedby')===el.id&&el.hasAttribute('data-flare-placed')&&Math.abs(r.x+r.width/2-p[0])<2&&Math.abs(r.bottom+8-p[1])<2;};
            for(let i=0;i<100;i++){if(matches())return;await new Promise(r=>setTimeout(r,20));}
            throw Error('Tooltip did not follow current viewport coordinates: '+JSON.stringify({pointer:p,box:chart.querySelector('[role=tooltip]')?.getBoundingClientRect(),target:document.elementFromPoint(...p)?.outerHTML.slice(0,300)}));
        }
        """, new double[] { x, y });
}
