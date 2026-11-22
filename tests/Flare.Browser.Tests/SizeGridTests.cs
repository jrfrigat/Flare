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
            var buttonBox = await button.BoundingBoxAsync();
            var fieldBox = await field.BoundingBoxAsync();
            Assert.NotNull(buttonBox);
            Assert.NotNull(fieldBox);
            Assert.InRange(fieldBox!.Height, buttonBox!.Height - 0.5, buttonBox.Height + 0.5);
            Assert.InRange(buttonBox.Height, expected[index] - 0.5, expected[index] + 0.5);
        }
    }
}
