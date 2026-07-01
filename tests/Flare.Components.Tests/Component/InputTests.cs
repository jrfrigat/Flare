namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlarePasswordField  (6 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlarePasswordFieldTests : FlareTestContext
{
    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Label, "Password"));

        var label = cut.Find("label.flare-input__label");
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

        var toggleBtn = cut.Find("button.flare-btn");
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

        var helper = cut.Find(".flare-input__helper");
        Assert.Contains("At least 8 characters", helper.TextContent);
    }

    [Fact]
    public void RendersErrorState()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.ErrorText, "Password too short"));

        Assert.NotEmpty(cut.FindAll(".flare-input__helper--error"));
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

        Assert.Contains("flare-input-variant--outlined", cut.Find(".flare-input").ClassName);
    }

    [Fact]
    public void FullWidth_False_ForwardsAutoClass()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.FullWidth, false));

        Assert.Contains("flare-input--auto", cut.Find(".flare-input").ClassName);
    }

    [Fact]
    public void Margin_Dense_ForwardsToField()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Margin, FieldMargin.Dense));

        Assert.Contains("flare-input--margin-dense", cut.Find(".flare-input").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareTextArea  (8 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareTextAreaTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareTextArea>();

        Assert.NotEmpty(cut.FindAll(".flare-textarea"));
    }

    [Fact]
    public void RendersTextareaElement()
    {
        var cut = Render<FlareTextArea>();

        Assert.NotEmpty(cut.FindAll("textarea"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Label, "Comments"));

        Assert.Contains("Comments", cut.Find(".flare-textarea__label").TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Placeholder, "Enter text..."));

        Assert.Equal("Enter text...", cut.Find("textarea").GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("textarea").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.HelperText, "Max 500 chars"));

        Assert.Contains("Max 500 chars", cut.Find(".flare-textarea__helper").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.ErrorText, "Field is required"));

        Assert.Contains("Field is required", cut.Find(".flare-textarea__helper--error").TextContent);
    }

    [Fact]
    public void RendersRows()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Rows, 6));

        Assert.Equal("6", cut.Find("textarea").GetAttribute("rows"));
    }
}

// ------------------------------------------------------------------------------
// FlareNumericField  (9 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareNumericFieldTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareNumericField<int>>();

        Assert.NotEmpty(cut.FindAll(".flare-input"));
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

        Assert.Contains("Quantity", cut.Find(".flare-input__label").TextContent);
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

public class C_FlareFieldFloatingLabelTests : FlareTestContext
{
    [Fact]
    public void NoFloatingByDefault()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Label, "Name"));
        var cls = cut.Find(".flare-input").ClassName;
        Assert.DoesNotContain("flare-input--floating", cls);
    }

    [Fact]
    public void FloatingLabelAppliesClass()
    {
        var cut = Render<FlareField<string>>(p =>
        {
            p.Add(x => x.Label, "Name");
            p.Add(x => x.FloatingLabel, true);
        });
        Assert.Contains("flare-input--floating", cut.Find(".flare-input").ClassName);
    }

    [Fact]
    public void NonFloatingLabelRenderedBeforeField()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Label, "Email"));
        var label = cut.Find("label.flare-input__label");
        Assert.NotNull(label);
        Assert.DoesNotContain("flare-input__label--floating", label.ClassName);
    }

    [Fact]
    public void FloatingLabelRenderedInsideField()
    {
        var cut = Render<FlareField<string>>(p =>
        {
            p.Add(x => x.Label, "Email");
            p.Add(x => x.FloatingLabel, true);
        });
        var label = cut.Find(".flare-input__field label.flare-input__label--floating");
        Assert.NotNull(label);
        Assert.Contains("Email", label.TextContent);
    }

    [Fact]
    public void NoLabelNoLabelElement()
    {
        var cut = Render<FlareField<string>>();
        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void FloatingAndNoLabelNoLabelElement()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.FloatingLabel, true));
        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void FloatingLabelHasForAttribute()
    {
        var cut = Render<FlareField<string>>(p =>
        {
            p.Add(x => x.Label, "Search");
            p.Add(x => x.FloatingLabel, true);
        });
        var label = cut.Find(".flare-input__label--floating");
        Assert.NotNull(label.GetAttribute("for"));
    }
}
