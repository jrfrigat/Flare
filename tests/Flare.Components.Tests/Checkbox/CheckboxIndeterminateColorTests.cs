namespace Flare.Components.Tests;

public class CheckboxIndeterminateColorTests : FlareTestContext
{
    [Fact]
    public void ACustomColor_WritesMainAndOnTokens()
    {
        var cut = Render<FlareCheckbox<bool?>>(p => p
            .Add(x => x.Value, null)
            .Add(x => x.Color, FlareColor.Custom("#ff0000")));

        var style = cut.Find("label").GetAttribute("style") ?? string.Empty;
        Assert.Contains("--fc-main:#ff0000", style);
        Assert.Contains("--fc-on:", style);
        Assert.Contains(Css.Classes.Checkbox.IndicatorIndeterminate, cut.Find($"span.{Css.Classes.Checkbox.Indicator}").ClassName ?? string.Empty);
    }

    [Fact]
    public void ARoleColor_UsesTheSharedClassInsteadOfInlineTokens()
    {
        var cut = Render<FlareCheckbox<bool?>>(p => p
            .Add(x => x.Value, null)
            .Add(x => x.Color, FlareColor.Secondary));

        var label = cut.Find("label");
        Assert.Contains("flare-color-secondary", label.ClassName ?? string.Empty);
        Assert.Null(label.GetAttribute("style"));
        Assert.Equal("mixed", cut.Find("input").GetAttribute("aria-checked"));
    }
}
