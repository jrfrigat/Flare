namespace Flare.Components.Tests.Accessibility;

public class SliderAriaTests : FlareTestContext
{
    private static string Label => $".{Css.Classes.Slider.Label}";
    private static string Inputs => "input[type=range]";

    [Fact]
    public void AVisibleLabel_NamesTheSingleSlider()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Label, "Volume")
            .Add(x => x.Value, 40));

        var input = cut.Find(Inputs);
        Assert.Equal(cut.Find(Label).Id, input.GetAttribute("aria-labelledby"));
        Assert.Null(input.GetAttribute("aria-label"));
    }

    [Fact]
    public void AriaLabel_NamesASliderWithoutAVisibleLabel()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.AriaLabel, "Volume")
            .Add(x => x.Value, 40));

        Assert.Equal("Volume", cut.Find(Inputs).GetAttribute("aria-label"));
        Assert.Null(cut.Find(Inputs).GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void LabelContent_NamesTheSingleSlider()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.LabelContent, b => b.AddMarkupContent(0, "Volume"))
            .Add(x => x.Value, 40));

        var label = cut.Find(Label);
        Assert.Equal(label.Id, cut.Find(Inputs).GetAttribute("aria-labelledby"));
        Assert.Equal("Volume", label.TextContent.Trim());
    }

    [Fact]
    public void RangeHandles_HaveDistinctNames()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Label, "Volume")
            .Add(x => x.Range, true)
            .Add(x => x.Value, 20)
            .Add(x => x.ValueEnd, 80));

        var inputs = cut.FindAll(Inputs);
        Assert.Equal(2, inputs.Count);
        var lowIds = inputs[0].GetAttribute("aria-labelledby")!.Split(' ');
        var highIds = inputs[1].GetAttribute("aria-labelledby")!.Split(' ');
        Assert.NotEqual(inputs[0].GetAttribute("aria-labelledby"), inputs[1].GetAttribute("aria-labelledby"));
        Assert.Equal("Volume", cut.Find($"#{lowIds[0]}").TextContent.Trim());
        Assert.Equal("minimum", cut.Find($"#{lowIds[1]}").TextContent.Trim());
        Assert.Equal("maximum", cut.Find($"#{highIds[1]}").TextContent.Trim());
    }

    [Fact]
    public void RangeWithoutAVisibleLabel_UsesAriaLabelInBothNames()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.AriaLabel, "Volume")
            .Add(x => x.Range, true)
            .Add(x => x.Value, 20)
            .Add(x => x.ValueEnd, 80));

        var inputs = cut.FindAll(Inputs);
        Assert.Equal("Volume minimum", cut.Find($"#{inputs[0].GetAttribute("aria-labelledby")}").TextContent.Trim());
        Assert.Equal("Volume maximum", cut.Find($"#{inputs[1].GetAttribute("aria-labelledby")}").TextContent.Trim());
    }

    [Fact]
    public void HelperText_IsDescribedByTheInput()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Label, "Volume")
            .Add(x => x.HelperText, "0 to 100")
            .Add(x => x.Value, 40));

        Assert.Equal(cut.Find($".{Css.Classes.Slider.Helper}").Id, cut.Find(Inputs).GetAttribute("aria-describedby"));
        Assert.Null(cut.Find(Inputs).GetAttribute("aria-invalid"));
    }

    [Fact]
    public void RangeEndpointWords_AreLocalized()
    {
        var ru = System.Globalization.CultureInfo.GetCultureInfo("ru");
        Assert.Equal("минимум", Flare.Components.Resources.FlareStrings.ResourceManager.GetString("Slider_Minimum", ru));
        Assert.Equal("максимум", Flare.Components.Resources.FlareStrings.ResourceManager.GetString("Slider_Maximum", ru));
        Assert.Equal("minimum", Flare.Components.Resources.FlareStrings.ResourceManager.GetString("Slider_Minimum", System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void RangeEndpointLabels_CanBeOverridden()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.AriaLabel, "Volume")
            .Add(x => x.Range, true)
            .Add(x => x.RangeStartLabel, "lower")
            .Add(x => x.RangeEndLabel, "upper")
            .Add(x => x.Value, 20)
            .Add(x => x.ValueEnd, 80));

        var inputs = cut.FindAll(Inputs);
        Assert.Equal("Volume lower", cut.Find($"#{inputs[0].GetAttribute("aria-labelledby")}").TextContent.Trim());
        Assert.Equal("Volume upper", cut.Find($"#{inputs[1].GetAttribute("aria-labelledby")}").TextContent.Trim());
    }

    [Fact]
    public void AnError_IsDescribedMarkedInvalidAndAnnounced()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Label, "Volume")
            .Add(x => x.ErrorText, "Out of range")
            .Add(x => x.Value, 40));

        var input = cut.Find(Inputs);
        var error = cut.Find("[role=alert]");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal(error.Id, input.GetAttribute("aria-describedby"));
        Assert.Equal("Out of range", error.TextContent.Trim());
        Assert.DoesNotContain("Out of range", cut.Find(Label).TextContent);
    }
}
