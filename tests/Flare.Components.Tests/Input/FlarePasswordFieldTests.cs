namespace Flare.Components.Tests;

public class FlarePasswordFieldTests : FlareTestContext
{
    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Label, "Password"));

        var label = cut.Find($"label.{Css.Classes.Input.Label}");
        Assert.Equal("Password", label.TextContent);
    }

    [Fact]
    public void RendersPasswordTypeInitially()
    {
        var cut = Render<FlarePasswordField>();

        Assert.Equal("password", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void RendersToggleButton()
    {
        var cut = Render<FlarePasswordField>();

        var toggleBtn = cut.Find($"button.{Css.Classes.Button.Root}");
        Assert.NotNull(toggleBtn);
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.HelperText, "At least 8 characters"));

        var helper = cut.Find($".{Css.Classes.Input.Helper}");
        Assert.Contains("At least 8 characters", helper.TextContent);
    }

    [Fact]
    public void RendersErrorState()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.ErrorText, "Password too short"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.HelperError}"));
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        // Regression: the inner @bind-Value used to only assign the local Value field and never
        // invoked the component's own ValueChanged, so a consumer's @bind-Value stayed empty forever.
        string? captured = null;
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Value, "")
            .Add(x => x.ValueChanged, v => { captured = v; }));

        cut.Find("input").Change("s3cret");

        Assert.Equal("s3cret", captured);
    }

    [Fact]
    public void Immediate_CommitsValueOnKeystroke()
    {
        string? captured = null;
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.ValueChanged, v => { captured = v; }));

        cut.Find("input").Input("typing");

        Assert.Equal("typing", captured);
    }

    [Fact]
    public void NotImmediate_DoesNotCommitOnKeystroke()
    {
        string? captured = null;
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.ValueChanged, v => { captured = v; }));

        cut.Find("input").Input("typing"); // oninput is gated behind Immediate

        Assert.Null(captured);
    }

    [Fact]
    public void Required_EmitsRequiredAttribute()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Required, true));

        Assert.True(cut.Find("input").HasAttribute("required"));
    }

    [Fact]
    public void NotRequired_NoRequiredAttribute()
    {
        var cut = Render<FlarePasswordField>();

        Assert.False(cut.Find("input").HasAttribute("required"));
    }

    [Fact]
    public void Variant_Outlined_ForwardsToField()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Variant, InputVariant.Outlined));

        Assert.Contains(Css.Classes.Input.VariantOutlined, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void FullWidth_False_ForwardsAutoClass()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.FullWidth, false));

        Assert.Contains(Css.Classes.Input.Auto, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void Margin_Dense_ForwardsToField()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Margin, FieldMargin.Dense));

        Assert.Contains(Css.Classes.Input.MarginDense, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareTextArea  (8 tests from Wave4)
// ------------------------------------------------------------------------------
