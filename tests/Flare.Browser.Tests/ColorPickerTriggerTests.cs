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

        var triggerBox = await trigger.BoundingBoxAsync();
        var referenceBox = await reference.BoundingBoxAsync();
        Assert.NotNull(triggerBox);
        Assert.NotNull(referenceBox);
        Assert.InRange(triggerBox!.Height, referenceBox!.Height - 0.5, referenceBox.Height + 0.5);

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

        var small = await page.Locator($"#cp-sm .{Css.Classes.Colorpicker.TriggerSwatch}").BoundingBoxAsync();
        var large = await page.Locator($"#cp-xl .{Css.Classes.Colorpicker.TriggerSwatch}").BoundingBoxAsync();
        Assert.NotNull(small);
        Assert.NotNull(large);
        Assert.InRange(small!.Width, small.Height - 0.5, small.Height + 0.5);
        Assert.True(large!.Width > small.Width, "the swatch does not grow with the button size");
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

    private static async Task ExpectSameCssAsync(ILocator a, ILocator b, string property)
    {
        var left = await a.EvaluateAsync<string>($"el => getComputedStyle(el).getPropertyValue('{property}')");
        var right = await b.EvaluateAsync<string>($"el => getComputedStyle(el).getPropertyValue('{property}')");
        Assert.Equal(right, left);
    }
}
