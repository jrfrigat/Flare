namespace Flare.Components.Tests;

public class FlareTextFieldTests : FlareTestContext
{
    [Fact]
    public void Renders_AsInput_WithLabel()
    {
        var cut = RenderComponent<FlareTextField>(p => p
            .Add(c => c.Label, "Name"));

        Assert.Equal("Name", cut.Find("label.flare-input__label").TextContent);
        Assert.NotNull(cut.Find("input"));
    }

    [Fact]
    public void Value_RenderedAsInputValue()
    {
        var cut = RenderComponent<FlareTextField>(p => p
            .Add(c => c.Value, "hello"));

        Assert.Equal("hello", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void ValueChanged_FiredWithString_NoTValueNeeded()
    {
        string? captured = null;
        var cut = RenderComponent<FlareTextField>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input").Change("typed");

        Assert.Equal("typed", captured);
    }

    [Fact]
    public void Is_FlareFieldOfString()
    {
        Assert.True(typeof(FlareField<string>).IsAssignableFrom(typeof(FlareTextField)));
    }
}
