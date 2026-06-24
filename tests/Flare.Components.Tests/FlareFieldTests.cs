namespace Flare.Components.Tests;

public class FlareFieldTests : FlareTestContext
{
    [Fact]
    public void Renders_WithLabel_LabelElementPresent()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Label, "Username"));

        var label = cut.Find("label.flare-input__label");
        Assert.Equal("Username", label.TextContent);
    }

    [Fact]
    public void Renders_WithoutLabel_NoLabelElement()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Label, (string?)null));

        Assert.Empty(cut.FindAll("label.flare-input__label"));
    }

    [Fact]
    public void Value_RenderedAsInputValue()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Value, "hello"));

        var input = cut.Find("input");
        Assert.Equal("hello", input.GetAttribute("value"));
    }

    [Fact]
    public void ValueChanged_FiredOnInputChange()
    {
        string? captured = null;
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input").Change("new value");

        Assert.Equal("new value", captured);
    }

    [Fact]
    public void Disabled_SetsDisabledAttribute()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void NotDisabled_NoDisabledAttribute()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Disabled, false));

        Assert.False(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void Type_ForwardedToInputElement()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Type, "email"));

        Assert.Equal("email", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void PasswordType_ForwardedToInputElement()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Type, "password"));

        Assert.Equal("password", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void Placeholder_ForwardedToInputElement()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Placeholder, "Enter text..."));

        Assert.Equal("Enter text...", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void HelperText_RenderedWhenProvided()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.HelperText, "Some hint"));

        var helper = cut.Find(".flare-input__helper");
        Assert.Contains("Some hint", helper.TextContent);
    }

    [Fact]
    public void ErrorText_RenderedAsErrorSpan()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.ErrorText, "This field is required"));

        var error = cut.Find(".flare-input__helper--error");
        Assert.Contains("This field is required", error.TextContent);
    }

    [Fact]
    public void ErrorText_SetsAriaInvalidOnInput()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.ErrorText, "Error!"));

        Assert.Equal("true", cut.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Disabled_AddsDisabledCssClass()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.Disabled, true));

        var wrapper = cut.Find(".flare-input--disabled");
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void IntValue_TwoWayBindingConvertsType()
    {
        int? captured = null;
        var cut = RenderComponent<FlareField<int>>(p => p
            .Add(c => c.Value, 0)
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input").Change("42");

        Assert.Equal(42, captured);
    }

    [Fact]
    public void ReadOnly_SetsReadonlyAttribute()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.ReadOnly, true));

        Assert.True(cut.Find("input").HasAttribute("readonly"));
    }

    [Fact]
    public void LeadingIcon_RendersLeadingIconSpan()
    {
        var cut = RenderComponent<FlareField<string>>(p => p
            .Add(c => c.LeadingIcon, b => b.AddMarkupContent(0, "<span>search</span>")));

        var icon = cut.Find(".flare-input__icon--leading");
        Assert.NotNull(icon);
        Assert.Contains("search", icon.InnerHtml);
    }
}
