using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The ColorPicker trigger is the shared FlareButton: with the same Variant and Size it must render the
/// same box and computed paint as a plain button, and its swatch follows the button's icon size.
/// </summary>
public sealed class ColorPickerTriggerTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("sm")]
    [InlineData("md")]
    [InlineData("xl")]
    public async Task TriggerMatchesAButtonOfTheSameSize(string size)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/colorpicker-trigger");

        var row = page.Locator($"#cp-grid .cp-row[data-size={size}]");
        var trigger = row.Locator($".{Css.Classes.Colorpicker.Trigger}");
        var reference = row.Locator("button").Last;
        await Expect(trigger).ToBeVisibleAsync();

        await ExpectSameHeightAsync(trigger, reference);

        await ExpectSameCssAsync(trigger, reference, "padding-left");
        await ExpectSameCssAsync(trigger, reference, "border-top-left-radius");
        await ExpectSameCssAsync(trigger, reference, "border-top-color");
        Assert.Contains(Css.Classes.Button.Outlined, (await trigger.GetAttributeAsync("class"))!);
    }

    [Fact]
    public async Task TextVariantTriggerMatchesATextButton()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/colorpicker-trigger");

        var row = page.Locator("#cp-grid .cp-row[data-size=text]");
        var trigger = row.Locator($".{Css.Classes.Colorpicker.Trigger}");
        var reference = row.Locator("button").Last;
        await Expect(trigger).ToBeVisibleAsync();

        await ExpectSameCssAsync(trigger, reference, "padding-left");
        await ExpectSameCssAsync(trigger, reference, "border-top-color");
        Assert.Contains(Css.Classes.Button.Text, (await trigger.GetAttributeAsync("class"))!);
    }

    [Fact]
    public async Task SwatchFollowsTheButtonIconSize()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/colorpicker-trigger");

        var smallSwatch = page.Locator($"#cp-sm .{Css.Classes.Colorpicker.TriggerSwatch}");
        var largeSwatch = page.Locator($"#cp-xl .{Css.Classes.Colorpicker.TriggerSwatch}");
        await SwatchesSettleAsync(smallSwatch, largeSwatch);
    }

    [Fact]
    public async Task TriggerOpensThePanelAndAnnouncesIt()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/colorpicker-trigger");

        var trigger = page.Locator($"#cp-md .{Css.Classes.Colorpicker.Trigger}");
        await Expect(trigger).ToHaveAttributeAsync("aria-haspopup", "dialog");
        await Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");

        await trigger.ClickAsync();

        await Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Expect(page.Locator("#cp-md [role=dialog]")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task PanelIsTheSharedPopoverAndClosesOnEscapeAndOutsideClick()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/colorpicker-trigger");

        var trigger = page.Locator($"#cp-md .{Css.Classes.Colorpicker.Trigger}");
        var panel = page.Locator($"#cp-md [role=dialog]");
        await trigger.ClickAsync();
        await Expect(panel).ToBeVisibleAsync();
        Assert.Contains(Css.Classes.Popover.Paper, (await panel.GetAttributeAsync("class"))!);
        Assert.False(string.IsNullOrEmpty(await panel.GetAttributeAsync("aria-label")),
            "the picker dialog has no accessible name");

        await page.Keyboard.PressAsync("Escape");
        await Expect(panel).ToHaveCountAsync(0);

        await trigger.ClickAsync();
        await Expect(panel).ToBeVisibleAsync();
        await page.Mouse.ClickAsync(5, 5);
        await Expect(panel).ToHaveCountAsync(0);
    }

    private static async Task ExpectSameHeightAsync(ILocator a, ILocator b)
    {
        // The theme tokens land after the first paint, so a single sample can catch the trigger
        // unstyled (about 18px) while the reference is already themed. Compare until both settle.
        var deadline = DateTime.UtcNow.AddSeconds(5);
        double left = 0;
        double right = 0;
        while (DateTime.UtcNow < deadline)
        {
            var boxA = await a.BoundingBoxAsync();
            var boxB = await b.BoundingBoxAsync();
            if (boxA is not null && boxB is not null)
            {
                left = boxA.Height;
                right = boxB.Height;
                if (Math.Abs(left - right) <= 0.5)
                {
                    return;
                }
            }

            await Task.Delay(50);
        }

        Assert.InRange(left, right - 0.5, right + 0.5);
    }

    private static async Task SwatchesSettleAsync(ILocator small, ILocator large)
    {
        // Same theme-token race: the swatch size comes from the button's icon tokens.
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < deadline)
        {
            var smallBox = await small.BoundingBoxAsync();
            var largeBox = await large.BoundingBoxAsync();
            if (smallBox is not null && largeBox is not null
                && Math.Abs(smallBox.Width - smallBox.Height) <= 0.5
                && largeBox.Width > smallBox.Width)
            {
                return;
            }

            await Task.Delay(50);
        }

        var settledSmall = await small.BoundingBoxAsync();
        var settledLarge = await large.BoundingBoxAsync();
        Assert.NotNull(settledSmall);
        Assert.NotNull(settledLarge);
        Assert.InRange(settledSmall!.Width, settledSmall.Height - 0.5, settledSmall.Height + 0.5);
        Assert.True(settledLarge!.Width > settledSmall.Width, "the swatch does not grow with the button size");
    }

    private static async Task ExpectSameCssAsync(ILocator a, ILocator b, string property)
    {
        // The theme tokens land after the first paint and border colour transitions, so compare until
        // both boxes settle instead of sampling once mid-transition.
        var deadline = DateTime.UtcNow.AddSeconds(5);
        var left = string.Empty;
        var right = string.Empty;
        while (DateTime.UtcNow < deadline)
        {
            left = await a.EvaluateAsync<string>($"el => getComputedStyle(el).getPropertyValue('{property}')");
            right = await b.EvaluateAsync<string>($"el => getComputedStyle(el).getPropertyValue('{property}')");
            if (left == right)
            {
                return;
            }

            await Task.Delay(50);
        }

        Assert.Equal(right, left);
    }
}
