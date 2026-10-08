using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed partial class DatePickerUxTests
{
    [Theory]
    [MemberData(nameof(DateThemes))]
    public Task OutsideSelection_KeepsItsColorsAndDiffersFromDisabled(string theme, bool dark) => RunAsync(async page =>
    {
        await ApplyThemeAsync(page, theme, dark);
        var grid = page.GetByTestId("outside-states");
        await ReadableAsync(grid.Locator("[aria-current=date]"), theme, dark, "outside-today");
        var selected = grid.Locator("[aria-selected=true]");
        await Expect(selected).ToHaveClassAsync(new System.Text.RegularExpressions.Regex(Css.Classes.Picker.DayOutside));
        await Expect(selected).ToHaveCSSAsync("opacity", "1");
        await page.Mouse.MoveAsync(0, 0);
        var colors = await selected.EvaluateAsync<string[]>("el => { const s=getComputedStyle(el); return [s.color,s.backgroundColor,s.backgroundImage]; }");
        await selected.HoverAsync();
        await Expect(selected).ToHaveCSSAsync("color", colors[0]);
        await Expect(selected).ToHaveCSSAsync("background-color", colors[1]);
        await Expect(selected).ToHaveCSSAsync("background-image", colors[2]);
        var disabled = grid.Locator($".{Css.Classes.Picker.DayOutside}:disabled");
        var available = grid.Locator($".{Css.Classes.Picker.DayOutside}:not(:disabled):not([aria-selected=true])").First;
        var dimmed = await disabled.EvaluateAsync<double>("el => Number(getComputedStyle(el).opacity)");
        var readable = await available.EvaluateAsync<double>("el => Number(getComputedStyle(el).opacity)");
        Assert.True(readable > dimmed && readable < 1, "Available adjacent dates must differ from disabled and current-month dates.");
    });

    [Theory]
    [MemberData(nameof(DateThemes))]
    public Task AvailableOutsideDays_RemainReadableInEveryTheme(string theme, bool dark) => RunAsync(async page =>
    {
        await ApplyThemeAsync(page, theme, dark);
        await Expect(page.Locator("[data-flare-theme]")).ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex(dark ? Css.Classes.Theme.ModeDark : Css.Classes.Theme.ModeLight));
        var panel = page.GetByTestId("subject").Locator($".{Css.Classes.DatePicker.Panel}");
        await page.GetByTestId("subject").Locator("button[aria-haspopup]").PressAsync("Enter");
        var outside = panel.Locator($".{Css.Classes.Picker.DayOutside}:not(:disabled)").First;
        await Expect(outside).ToBeVisibleAsync();
        await page.Mouse.MoveAsync(0, 0);
        await ReadableAsync(outside, theme, dark, "rest");
        await outside.HoverAsync();
        await ReadableAsync(outside, theme, dark, "hover");
        await page.Mouse.MoveAsync(0, 0);
        await outside.FocusAsync();
        await ReadableAsync(outside, theme, dark, "focus");
        await outside.PressAsync("Enter");
        await Expect(page.GetByTestId("value")).Not.ToHaveTextAsync("2026-10-15");
    });

    private static async Task ReadableAsync(ILocator day, string theme, bool dark, string state)
    {
        await day.EvaluateAsync("async el => { await Promise.all(el.getAnimations().map(a => a.finished.catch(() => {}))); }");
        var contrast = await day.EvaluateAsync<double>("""
            el => {
                const canvas = document.createElement('canvas');
                canvas.width = canvas.height = 1;
                const ctx = canvas.getContext('2d', {willReadFrequently:true});
                const parse = color => {
                    ctx.clearRect(0,0,1,1); ctx.fillStyle = color; ctx.fillRect(0,0,1,1);
                    const p = Array.from(ctx.getImageData(0,0,1,1).data);
                    return [p[0],p[1],p[2],p[3]/255];
                };
                const over = (f,b) => [0,1,2].map(i => f[i]*f[3]+b[i]*(1-f[3])).concat(1);
                const layers = [];
                let opaque = false;
                for (let n=el.parentElement; n; n=n.parentElement) {
                    const s=getComputedStyle(n), c=parse(s.backgroundColor);
                    layers.unshift(s);
                    if(c[3] === 1 && s.opacity === '1') { opaque=true; break; }
                }
                if (!opaque) throw new Error('An opaque backing surface is required for this contrast check');
                let backgrounds = [[255,255,255,1]];
                const paint = (s, underlying) => {
                    const colors = s.backgroundImage === 'none' ? [null]
                        : s.backgroundImage.match(/rgba?\([^)]*\)|color\([^)]*\)/g);
                    if (!colors) throw new Error('Unrecognized gradient: '+s.backgroundImage);
                    return colors.flatMap(color => underlying.map(b => {
                        const base = over(parse(s.backgroundColor),b);
                        const fill = color ? over(parse(color),base) : base;
                        return over(fill.slice(0,3).concat(Number(s.opacity)),b);
                    }));
                };
                for(const s of layers) backgrounds=paint(s,backgrounds);
                const s=getComputedStyle(el), text=parse(s.color), opacity=Number(s.opacity);
                const lum=c => c.slice(0,3).map(v=>v/255).map(v=>v<=0.04045?v/12.92:((v+0.055)/1.055)**2.4)
                    .reduce((sum,v,i)=>sum+v*[0.2126,0.7152,0.0722][i],0);
                return Math.min(...backgrounds.flatMap(parent => {
                    // Each gradient stop is checked, including the worst tint on hover.
                    const opaqueStyle = {backgroundImage:s.backgroundImage,backgroundColor:s.backgroundColor,opacity:'1'};
                    return paint(opaqueStyle,[parent]).map(fill => {
                        const foreground=over(over(text,fill).slice(0,3).concat(opacity),parent);
                        const background=over(fill.slice(0,3).concat(opacity),parent);
                        const a=lum(foreground),b=lum(background);
                        return (Math.max(a,b)+0.05)/(Math.min(a,b)+0.05);
                    });
                }));
            }
            """);
        Assert.True(contrast >= 4.5, $"{theme}/{(dark ? "dark" : "light")}/{state}: contrast {contrast:F3} < 4.5");
    }
}
