using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// A card title with no spaces (Merito: a reward name typed as one word) has to wrap inside the header instead
/// of running past the card, without pushing the avatar and the action out of the row.
/// </summary>
public sealed class CardHeaderWrapTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData(360)]
    [InlineData(390)]
    [InlineData(412)]
    [InlineData(1280)]
    public async Task LongTitleWraps_AndKeepsAvatarAndAction(int width)
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = width, Height = 800 } });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/card-header");
        await Expect(page.Locator("#long-card")).ToBeVisibleAsync();

        // [title client, title scroll, subtitle client, subtitle scroll, avatar, action, card right, action right]
        var m = await page.EvaluateAsync<double[]>(@"() => {
            const card = document.getElementById('long-card');
            const t = card.querySelector('.flare-card__title'), s = card.querySelector('.flare-card__subtitle');
            const a = document.getElementById('avatar').getBoundingClientRect();
            const act = document.getElementById('action').getBoundingClientRect();
            return [t.clientWidth, t.scrollWidth, s.clientWidth, s.scrollWidth, a.width, act.width,
                card.getBoundingClientRect().right, act.right];
        }");

        Assert.True(m[1] <= m[0], $"title overflows: {m[1]} > {m[0]}");
        Assert.True(m[3] <= m[2], $"subtitle overflows: {m[3]} > {m[2]}");
        Assert.Equal(40, m[4], tolerance: 0.5);
        Assert.True(m[5] > 0 && m[7] <= m[6] + 0.5, $"action pushed out: right {m[7]} > card {m[6]}");
    }

    [Fact]
    public async Task ProseStillWrapsAtSpaces()
    {
        await using var context = await fixture.Browser.NewContextAsync(new() { ViewportSize = new() { Width = 360, Height = 800 } });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/card-header");
        await Expect(page.Locator("#prose-card")).ToBeVisibleAsync();

        // Every line break of the rendered title falls on a space: no word is split.
        var splitWord = await page.EvaluateAsync<bool>(@"() => {
            const t = document.querySelector('#prose-card .flare-card__title');
            const node = t.firstChild, text = node.textContent, range = document.createRange();
            let lastTop = null;
            for (let i = 0; i < text.length; i++) {
                range.setStart(node, i); range.setEnd(node, i + 1);
                const top = range.getBoundingClientRect().top;
                if (lastTop !== null && top > lastTop + 1 && text[i - 1] !== ' ') return true;
                lastTop = top;
            }
            return false;
        }");

        Assert.False(splitWord);
    }
}
