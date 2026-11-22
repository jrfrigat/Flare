using Flare.Css;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The Xs..Xl step names one height per theme: a button and a field at the same step must render the
/// same height, so a command row never jumps between a button and a select.
/// </summary>
public sealed class SizeGridTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    [Theory]
    [InlineData("fluent2", 24, 24, 32, 40, 48)]
    [InlineData("md3", 32, 36, 40, 48, 56)]
    [InlineData("md3e", 32, 40, 56, 96, 136)]
    [InlineData("md2", 28, 32, 36, 44, 52)]
    [InlineData("aero", 22, 24, 28, 36, 44)]
    [InlineData("liquidglass", 28, 36, 44, 52, 60)]
    [InlineData("visualstudio", 24, 24, 28, 36, 44)]
    public async Task FieldsMatchButtonsStepForStep(string theme, int xs, int sm, int md, int lg, int xl)
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/size-grid");
        await Expect(page.Locator($"#size-grid-{theme}")).ToBeVisibleAsync();

        await AssertGridAsync(page, theme, [xs, sm, md, lg, xl]);
    }

    private static async Task AssertGridAsync(IPage page, string theme, int[] expected)
    {
        var steps = new[] { "xs", "sm", "md", "lg", "xl" };
        for (var index = 0; index < steps.Length; index++)
        {
            var row = page.Locator($"#size-grid-{theme} .size-row[data-size={steps[index]}]");
            var button = row.Locator($".{Css.Classes.Button.Root}");
            var field = row.Locator($".{Css.Classes.Select.Control}");

            // The scope applies its tokens after the first paint, so a height is read until both boxes
            // settle on the same value instead of being sampled once mid-transition.
            var deadline = DateTime.UtcNow.AddSeconds(5);
            var buttonHeight = 0d;
            var fieldHeight = 0d;
            while (DateTime.UtcNow < deadline)
            {
                var buttonBox = await button.BoundingBoxAsync();
                var fieldBox = await field.BoundingBoxAsync();
                buttonHeight = buttonBox?.Height ?? 0;
                fieldHeight = fieldBox?.Height ?? 0;
                if (Math.Abs(buttonHeight - fieldHeight) <= 0.5
                    && Math.Abs(buttonHeight - expected[index]) <= 0.5)
                {
                    break;
                }

                await Task.Delay(50);
            }

            Assert.InRange(fieldHeight, buttonHeight - 0.5, buttonHeight + 0.5);
            Assert.InRange(buttonHeight, expected[index] - 0.5, expected[index] + 0.5);
        }
    }
}
