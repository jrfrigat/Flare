namespace Flare.Components.Tests;

public class FlareNumericFieldTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareNumericField<int>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root}"));
    }

    [Fact]
    public void RendersInputElement()
    {
        var cut = Render<FlareNumericField<int>>();

        Assert.NotEmpty(cut.FindAll("input"));
    }

    [Fact]
    public void RendersInputTypeNumber()
    {
        var cut = Render<FlareNumericField<int>>();

        Assert.Equal("number", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Label, "Quantity"));

        Assert.Contains("Quantity", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersMinAttribute()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Min, 0.0));

        Assert.Equal("0", cut.Find("input").GetAttribute("min"));
    }

    [Fact]
    public void RendersMaxAttribute()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Max, 100.0));

        Assert.Equal("100", cut.Find("input").GetAttribute("max"));
    }

    [Fact]
    public void RendersStep()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Step, 5.0));

        Assert.Equal("5", cut.Find("input").GetAttribute("step"));
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Placeholder, "0"));

        Assert.Equal("0", cut.Find("input").GetAttribute("placeholder"));
    }
}

// ------------------------------------------------------------------------------
// FlareField FloatingLabel  (7 tests from Wave10)
// ------------------------------------------------------------------------------
