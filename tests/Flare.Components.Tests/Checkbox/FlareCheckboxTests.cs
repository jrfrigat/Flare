namespace Flare.Components.Tests;

public class FlareCheckboxTests : FlareTestContext
{
    [Fact]
    public void Renders_Unchecked_ByDefault()
    {
        var cut = Render<FlareCheckbox<bool>>();

        var input = cut.Find("input[type='checkbox']");
        Assert.False(input.HasAttribute("checked"));
    }

    [Fact]
    public void Renders_Checked_WhenValueIsTrue()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Value, true));

        var input = cut.Find("input[type='checkbox']");
        Assert.True(input.HasAttribute("checked"));
    }

    [Fact]
    public void ValueChanged_FiredOnChange_WithTrue()
    {
        bool? captured = null;
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Value, false)
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input[type='checkbox']").Change(true);

        Assert.True(captured);
    }

    [Fact]
    public void ValueChanged_FiredOnChange_WithFalse()
    {
        bool? captured = null;
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Value, true)
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input[type='checkbox']").Change(false);

        Assert.False(captured);
    }

    [Fact]
    public void Disabled_SetsDisabledOnInput()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Disabled, true));

        Assert.True(cut.Find("input[type='checkbox']").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_AddsDisabledCssClass()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Disabled, true));

        var wrapper = cut.Find($".{Css.Classes.Checkbox.Disabled}");
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void NotDisabled_NoDisabledCssClass()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Disabled, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Checkbox.Disabled}"));
    }

    [Fact]
    public void Label_RenderedAsLabelSpan()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Label, "Accept terms"));

        var labelSpan = cut.Find($".{Css.Classes.Checkbox.Label}");
        Assert.Equal("Accept terms", labelSpan.TextContent);
    }

    [Fact]
    public void NoLabel_NoLabelSpanRendered()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.Label, (string?)null));

        Assert.Empty(cut.FindAll($".{Css.Classes.Checkbox.Label}"));
    }

    [Fact]
    public void ErrorText_RenderedAsErrorSpan()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.ErrorText, "Required"));

        var error = cut.Find($".{Css.Classes.Checkbox.ErrorMsg}");
        Assert.Contains("Required", error.TextContent);
    }

    [Fact]
    public void ErrorText_AddsErrorCssClass()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(c => c.ErrorText, "Required"));

        var wrapper = cut.Find($".{Css.Classes.Checkbox.Error}");
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void NoError_NoErrorSpanRendered()
    {
        var cut = Render<FlareCheckbox<bool>>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Checkbox.ErrorMsg}"));
    }

    [Fact]
    public void Renders_CheckboxIndicatorSpan()
    {
        var cut = Render<FlareCheckbox<bool>>();

        Assert.Single(cut.FindAll($".{Css.Classes.Checkbox.Indicator}"));
    }

    // The third state belongs to the bound type rather than to a flag beside it: a bool? that is null is
    // the indeterminate box, and a bool has no way to be one.
    [Fact]
    public void NullValue_OnANullableCheckbox_IsIndeterminate()
    {
        var cut = Render<FlareCheckbox<bool?>>(p => p
            .Add(c => c.Value, (bool?)null));

        Assert.Equal("mixed", cut.Find("input[type='checkbox']").GetAttribute("aria-checked"));
        Assert.Single(cut.FindAll($".{Css.Classes.Checkbox.IndicatorIndeterminate}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Checkbox.Indeterminate}"));
    }

    [Fact]
    public void ClickingAnIndeterminateCheckbox_ResolvesItToTrue()
    {
        bool? captured = null;
        var cut = Render<FlareCheckbox<bool?>>(p => p
            .Add(c => c.Value, (bool?)null)
            .Add(c => c.ValueChanged, v => { captured = v; }));

        cut.Find("input[type='checkbox']").Change(true);

        // A click answers the question; it does not cycle back into "unanswered".
        Assert.True(captured);
    }

    [Fact]
    public void TwoStateCheckbox_IsNeverIndeterminate()
    {
        var cut = Render<FlareCheckbox<bool>>();

        Assert.Equal("false", cut.Find("input[type='checkbox']").GetAttribute("aria-checked"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Checkbox.IndicatorIndeterminate}"));
    }

    // A checkbox holds a yes/no answer, so bool and bool? are the only value types it accepts. The
    // compiler cannot say so, and the alternative to throwing is a box that renders unchecked forever
    // and never explains why.
    [Fact]
    public void AValueTypeThatIsNotBoolean_ThrowsRatherThanRenderingNothing()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Render<FlareCheckbox<string>>());

        Assert.Contains("bool", ex.Message, StringComparison.Ordinal);
    }
}
