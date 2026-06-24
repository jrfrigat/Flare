namespace Flare.Components.Tests;

public class FlareCheckboxTests : FlareTestContext
{
    [Fact]
    public void Renders_Unchecked_ByDefault()
    {
        var cut = RenderComponent<FlareCheckbox>();

        var input = cut.Find("input[type='checkbox']");
        Assert.False(input.HasAttribute("checked"));
    }

    [Fact]
    public void Renders_Checked_WhenValueIsTrue()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Value, true));

        var input = cut.Find("input[type='checkbox']");
        Assert.True(input.HasAttribute("checked"));
    }

    [Fact]
    public void ValueChanged_FiredOnChange_WithTrue()
    {
        bool? captured = null;
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Value, false)
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input[type='checkbox']").Change(true);

        Assert.True(captured);
    }

    [Fact]
    public void ValueChanged_FiredOnChange_WithFalse()
    {
        bool? captured = null;
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Value, true)
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input[type='checkbox']").Change(false);

        Assert.False(captured);
    }

    [Fact]
    public void Disabled_SetsDisabledOnInput()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Disabled, true));

        Assert.True(cut.Find("input[type='checkbox']").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_AddsDisabledCssClass()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Disabled, true));

        var wrapper = cut.Find(".flare-checkbox--disabled");
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void NotDisabled_NoDisabledCssClass()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Disabled, false));

        Assert.Empty(cut.FindAll(".flare-checkbox--disabled"));
    }

    [Fact]
    public void Label_RenderedAsLabelSpan()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Label, "Accept terms"));

        var labelSpan = cut.Find(".flare-checkbox__label");
        Assert.Equal("Accept terms", labelSpan.TextContent);
    }

    [Fact]
    public void NoLabel_NoLabelSpanRendered()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.Label, (string?)null));

        Assert.Empty(cut.FindAll(".flare-checkbox__label"));
    }

    [Fact]
    public void ErrorText_RenderedAsErrorSpan()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.ErrorText, "Required"));

        var error = cut.Find(".flare-checkbox__error");
        Assert.Contains("Required", error.TextContent);
    }

    [Fact]
    public void ErrorText_AddsErrorCssClass()
    {
        var cut = RenderComponent<FlareCheckbox>(p => p
            .Add(c => c.ErrorText, "Required"));

        var wrapper = cut.Find(".flare-checkbox--error");
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void NoError_NoErrorSpanRendered()
    {
        var cut = RenderComponent<FlareCheckbox>();

        Assert.Empty(cut.FindAll(".flare-checkbox__error"));
    }

    [Fact]
    public void Renders_CheckboxIndicatorSpan()
    {
        var cut = RenderComponent<FlareCheckbox>();

        Assert.Single(cut.FindAll(".flare-checkbox__indicator"));
    }
}
