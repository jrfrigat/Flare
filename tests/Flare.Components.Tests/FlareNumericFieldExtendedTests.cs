// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareNumericField tests  (8 tests)
// -----------------------------------------------------------------------------

public class FlareNumericFieldExtendedTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareFieldElement()
    {
        var cut = Render<FlareNumericField<double>>();

        Assert.NotEmpty(cut.FindAll(".flare-input"));
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Label, "Quantity"));

        Assert.Contains("Quantity", cut.Find("label.flare-input__label").TextContent);
    }

    [Fact]
    public void Min_And_Max_SetAttributesOnInput()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Min, 0.0)
            .Add(x => x.Max, 100.0));

        var input = cut.Find("input[type=number]");
        Assert.Equal("0", input.GetAttribute("min"));
        Assert.Equal("100", input.GetAttribute("max"));
    }

    [Fact]
    public void Step_SetsStepAttributeOnInput()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Step, 0.5));

        var input = cut.Find("input[type=number]");
        Assert.Equal("0.5", input.GetAttribute("step"));
    }

    [Fact]
    public void Disabled_True_DisablesInput()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Disabled, true));

        var input = cut.Find("input[type=number]");
        Assert.NotNull(input.GetAttribute("disabled"));
    }

    [Fact]
    public void Disabled_True_AddsDisabledClass()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("flare-input--disabled", cut.Find(".flare-input").ClassName ?? "");
    }

    [Fact]
    public void Value_SetsInputValue()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Value, 42));

        var input = cut.Find("input[type=number]");
        Assert.Equal("42", input.GetAttribute("value"));
    }

    [Fact]
    public void HelperText_RendersHelperSpan()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.HelperText, "Enter a number between 0 and 100"));

        Assert.Contains("Enter a number between 0 and 100",
            cut.Find("span.flare-input__helper").TextContent);
    }
}
