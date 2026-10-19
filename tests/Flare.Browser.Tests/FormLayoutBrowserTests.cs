using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

/// <summary>
/// The form's layout classes used to sit on a wrapper whose only child was the EditForm's form, so the
/// fields never saw them. These tests read real positions rather than class names.
/// </summary>
public sealed class FormLayoutBrowserTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private async Task<IPage> OpenAsync(IBrowserContext context, string query = "", int width = 900)
    {
        var page = await context.NewPageAsync();
        await page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await page.SetViewportSizeAsync(width, 900);
        await page.GotoAsync($"{fixture.BaseUrl}/form{query}");
        await Expect(page.Locator("#submit")).ToBeVisibleAsync();
        // Field heights feed the gap measurements; a late font would shift them mid-assertion.
        await page.EvaluateAsync<bool>("() => document.fonts.ready.then(() => true)");
        // The host applies a theme itself; wait for its token bundle so the layout rules resolve.
        await Expect(page.Locator($".{Css.Classes.Theme.Root}")).ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex(
                System.Text.RegularExpressions.Regex.Escape(Css.Classes.Theme.ForTheme("md3-expressive"))));
        await page.WaitForFunctionAsync(
            "token => getComputedStyle(document.querySelector('#form-root')).getPropertyValue(token).trim().length > 0",
            Css.Tokens.FormField.HorizontalColumns);
        return page;
    }

    private static ILocator Fields(IPage page) => page.Locator($".{Css.Classes.Form.Root} .{Css.Classes.Input.Root}");

    private static async Task<(float X, float Y, float Width, float Height)> BoxAsync(ILocator locator)
    {
        var box = await locator.BoundingBoxAsync();
        Assert.NotNull(box);
        return (box!.X, box.Y, box.Width, box.Height);
    }

    [Fact]
    public async Task VerticalFields_StackInOneColumn()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var fields = Fields(page);

        var first = await BoxAsync(fields.Nth(0));
        var second = await BoxAsync(fields.Nth(1));
        Assert.Equal(first.X, second.X, 1);
        Assert.True(second.Y > first.Y + first.Height, "the second field has to start below the first");
    }

    [Fact]
    public async Task InlineFields_FlowInARow()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, "?layout=inline");
        var fields = Fields(page);

        var first = await BoxAsync(fields.Nth(0));
        var second = await BoxAsync(fields.Nth(1));
        // align-items: flex-end lines the fields up on their bottom edge, not their top.
        Assert.Equal(first.Y + first.Height, second.Y + second.Height, 1);
        Assert.True(second.X > first.X, "the second field has to start to the right of the first");
    }

    [Fact]
    public async Task HorizontalFields_PutTheLabelBesideTheControl()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, "?layout=horizontal");
        var fields = Fields(page);

        foreach (var index in new[] { 0, 1, 2 })
        {
            var field = fields.Nth(index);
            var label = await BoxAsync(field.Locator($".{Css.Classes.Input.Label}"));
            var control = await BoxAsync(field.Locator($".{Css.Classes.Input.ControlSlot}"));
            Assert.True(label.X + label.Width <= control.X + 1, "the label has to sit before the control");
            Assert.Equal(label.Y, control.Y, 3);
        }

        var first = await BoxAsync(fields.Nth(0));
        var second = await BoxAsync(fields.Nth(1));
        Assert.True(second.Y >= first.Y + first.Height, "horizontal fields still stack");
    }

    [Fact]
    public async Task Dense_ReducesTheGapBetweenFields()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var regular = await GapAsync(page);

        await page.GotoAsync($"{fixture.BaseUrl}/form?dense=true");
        await Expect(page.Locator("#submit")).ToBeVisibleAsync();
        var dense = await GapAsync(page);

        Assert.True(dense < regular, $"dense gap {dense} should be smaller than {regular}");
    }

    private static async Task<float> GapAsync(IPage page)
    {
        var first = await BoxAsync(Fields(page).Nth(0));
        var second = await BoxAsync(Fields(page).Nth(1));
        return second.Y - (first.Y + first.Height);
    }

    [Fact]
    public async Task ALongLabelAt320_WrapsWithoutOverflowingThePage()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, "?layout=horizontal&long=true", width: 320);

        var overflow = await page.EvaluateAsync<int>(
            "document.documentElement.scrollWidth - document.documentElement.clientWidth");
        Assert.True(overflow <= 0, $"the page gained {overflow}px of horizontal scroll");

        var field = Fields(page).Nth(0);
        var label = await BoxAsync(field.Locator($".{Css.Classes.Input.Label}"));
        var control = await BoxAsync(field.Locator($".{Css.Classes.Input.ControlSlot}"));
        Assert.True(label.Height > 24, "a long label has to wrap onto more than one line");
        Assert.InRange(control.X + control.Width, 0, 321);
        await Expect(page.Locator("#name")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Submitting_ReachesTheEditForm()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        await page.Locator("#submit").ClickAsync();
        await Expect(page.Locator("#status")).ToHaveTextAsync("valid");
    }

    [Fact]
    public async Task AnInvalidModel_ReachesTheInvalidHandler()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context, "?empty=true");

        await page.Locator("#submit").ClickAsync();
        await Expect(page.Locator("#status")).ToHaveTextAsync("invalid");
    }

    [Fact]
    public async Task TheRootKeepsItsAttributesAndTheFormStaysNative()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);

        var root = page.Locator("#form-root");
        await Expect(root).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("custom-form"));
        await Expect(root).ToHaveAttributeAsync("data-testid", "form-root");
        Assert.Equal(1, await page.Locator("form").CountAsync());
        await Expect(page.Locator("#submit")).ToHaveAttributeAsync("type", "submit");
    }
}
