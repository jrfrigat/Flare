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
        Assert.Contains($"{Css.Tokens.LocalColor.Main}:#ff0000", style);
        Assert.Contains($"{Css.Tokens.LocalColor.On}:", style);
        Assert.Contains(Css.Classes.Checkbox.IndicatorIndeterminate, cut.Find($"span.{Css.Classes.Checkbox.Indicator}").ClassName ?? string.Empty);
    }

    [Fact]
    public void ARoleColor_UsesTheSharedClassInsteadOfInlineTokens()
    {
        var cut = Render<FlareCheckbox<bool?>>(p => p
            .Add(x => x.Value, null)
            .Add(x => x.Color, FlareColor.Secondary));

        var label = cut.Find("label");
        Assert.Contains(Css.Classes.Color.Secondary, label.ClassName ?? string.Empty);
        Assert.Null(label.GetAttribute("style"));
        Assert.Equal("mixed", cut.Find("input").GetAttribute("aria-checked"));
    }
}
