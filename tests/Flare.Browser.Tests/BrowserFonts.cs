using Microsoft.Playwright;

namespace Flare.Browser.Tests;

// A theme class/token can arrive before its font-face stylesheet. Load the fonts actually used by
// the measured subtree, including newly mounted menus, rather than an earlier document.fonts.ready.
internal static class BrowserFonts
{
    public static async Task WaitAsync(ILocator root)
    {
        await root.Page.WaitForFunctionAsync("""
            () => [...document.querySelectorAll('link[rel="stylesheet"][href]')]
                .every(link => link.disabled || link.sheet !== null)
            """);
        await root.EvaluateAsync("""
            async root => {
                const elements = [root, ...root.querySelectorAll('*')];
                const fonts = new Set(elements.map(el => getComputedStyle(el).font).filter(Boolean));
                const text = root.textContent || ' ';
                await Promise.all([...fonts].map(font => document.fonts.load(font, text)));
                await document.fonts.ready;
                await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
            }
            """);
    }
}
