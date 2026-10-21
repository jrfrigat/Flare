using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A skeleton is decoration: with reduced motion it shows a still placeholder, while the normal mode and
/// an explicit <c>Animation=None</c> keep their behaviour, and toggling the preference on a live page
/// takes effect without a reload.
/// </summary>
public sealed class SkeletonMotionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task Skeleton_StopsWaveAndPulseUnderReducedMotion()
    {
        await using var context = await fixture.Browser.NewContextAsync(new()
        {
            ReducedMotion = ReducedMotion.NoPreference,
            ViewportSize = new() { Width = 900, Height = 300 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/skeleton-motion");

        var wave = page.Locator("#wave");
        var pulse = page.Locator("#pulse");
        var none = page.Locator("#none");
        await page.WaitForFunctionAsync(
            $"() => {{ const el = document.querySelector('#pulse'); return el !== null && getComputedStyle(el).animationName === 'flare-pulse'; }}");
        Assert.NotEqual("none", await wave.EvaluateAsync<string>("el => getComputedStyle(el, '::after').display"));
        Assert.Equal("none", await none.EvaluateAsync<string>("el => getComputedStyle(el).animationName"));

        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });

        await Expect(pulse).ToHaveCSSAsync("animation-name", "none");
        Assert.Equal("none", await wave.EvaluateAsync<string>("el => getComputedStyle(el, '::after').display"));
        Assert.Equal("none", await none.EvaluateAsync<string>("el => getComputedStyle(el).animationName"));

        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.NoPreference });

        await Expect(pulse).ToHaveCSSAsync("animation-name", "flare-pulse");
        Assert.NotEqual("none", await wave.EvaluateAsync<string>("el => getComputedStyle(el, '::after').display"));
        Assert.Equal("none", await none.EvaluateAsync<string>("el => getComputedStyle(el).animationName"));
    }
}
