namespace Flare.Components.Tests;

public class FlareNumericFieldExtendedTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareFieldElement()
    {
        var cut = Render<FlareNumericField<double>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root}"));
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Label, "Quantity"));

        Assert.Contains("Quantity", cut.Find($"label.{Css.Classes.Input.Label}").TextContent);
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

        Assert.Contains(Css.Classes.Input.Disabled, cut.Find($".{Css.Classes.Input.Root}").ClassName ?? "");
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
            cut.Find($"span.{Css.Classes.Input.Helper}").TextContent);
    }
}
