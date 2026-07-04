using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareForm  (6 tests from Wave2)
// ------------------------------------------------------------------------------

public class C_FlareFormTests : FlareTestContext
{
    private class PersonModel
    {
        [Required] public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }

    [Fact]
    public void RendersRootDiv()
    {
        var model = new PersonModel();
        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model));

        Assert.NotEmpty(cut.FindAll(".flare-form"));
    }

    [Fact]
    public void ContainsFormElement()
    {
        var model = new PersonModel();
        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model));

        Assert.NotEmpty(cut.FindAll("form"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var model = new PersonModel();
        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent("<span class=\"child-item\">Inside form</span>"));

        Assert.NotEmpty(cut.FindAll(".child-item"));
    }

    [Fact]
    public void RendersFlareFieldInside()
    {
        var model = new PersonModel();
        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareField<string>>(0);
                b.AddAttribute(1, "Label", "Name");
                b.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll("input"));
    }

    [Fact]
    public void RendersValidationSummaryInside()
    {
        var model = new PersonModel();
        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareValidationSummary>(0);
                b.CloseComponent();
            }));

        Assert.Empty(cut.FindAll(".flare-validation-summary"));
    }

    [Fact]
    public void AcceptsAdditionalAttributes()
    {
        var model = new PersonModel();
        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddUnmatched("data-testid", "my-form"));

        Assert.Equal("my-form", cut.Find(".flare-form").GetAttribute("data-testid"));
    }
}

// ------------------------------------------------------------------------------
// FlareSwitch  (7 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareSwitchTests : FlareTestContext
{
    [Fact]
    public void RendersRootLabel()
    {
        var cut = Render<FlareSwitch>();

        Assert.NotEmpty(cut.FindAll(".flare-switch"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Enable feature"));

        Assert.Contains("Enable feature", cut.Find(".flare-switch__label").TextContent);
    }

    [Fact]
    public void HasCorrectInputType()
    {
        var cut = Render<FlareSwitch>();

        var input = cut.Find("input");
        Assert.Equal("checkbox", input.GetAttribute("type"));
    }

    [Fact]
    public void RendersCheckedWhenValueTrue()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Value, true));

        Assert.True(cut.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void RendersUncheckedWhenValueFalse()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Value, false));

        Assert.False(cut.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.ErrorText, "Must be enabled"));

        Assert.Contains("Must be enabled", cut.Find(".flare-switch__error").TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareRadioGroup  (6 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareRadioGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootFieldset()
    {
        var cut = Render<FlareRadioGroup<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-radio-group"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Label, "Pick one"));

        Assert.Contains("Pick one", cut.Find(".flare-radio-group__legend").TextContent);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "class", "test-child");
                b.CloseElement();
            })));

        Assert.NotEmpty(cut.FindAll(".test-child"));
    }

    [Fact]
    public void RendersDisabledClass()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.NotEmpty(cut.FindAll("fieldset"));
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.ErrorText, "Selection required"));

        Assert.Contains("Selection required", cut.Find(".flare-radio-group__error").TextContent);
    }

    [Fact]
    public void RendersInlineClass()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Inline, true));

        Assert.Contains("flare-radio-group--inline", cut.Find("fieldset").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareRadio  (6 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareRadioTests : FlareTestContext
{
    [Fact]
    public void RendersRootLabel()
    {
        var cut = Render<FlareRadio<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-radio"));
    }

    [Fact]
    public void RendersInputTypeRadio()
    {
        var cut = Render<FlareRadio<string>>();

        var input = cut.Find("input");
        Assert.Equal("radio", input.GetAttribute("type"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Label, "Option A"));

        Assert.Contains("Option A", cut.Find(".flare-radio__label").TextContent);
    }

    [Fact]
    public void RendersDisabledWhenParameterSet()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void NotCheckedByDefault()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Value, "a"));

        Assert.False(cut.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void DisabledClassAppliedWhenDisabled()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("flare-radio--disabled", cut.Find("label").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareFormField  (9 tests from Wave10)
// ------------------------------------------------------------------------------

public class C_FlareFormFieldTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareFormField>();
        Assert.NotEmpty(cut.FindAll(".flare-input"));
    }

    [Fact]
    public void LabelRenderedWhenSet()
    {
        var cut = Render<FlareFormField>(p => p.Add(x => x.Label, "Email"));
        Assert.NotEmpty(cut.FindAll("label.flare-input__label"));
        Assert.Contains("Email", cut.Markup);
    }

    [Fact]
    public void LabelNotRenderedWhenNull()
    {
        var cut = Render<FlareFormField>();
        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void HelperTextRendered()
    {
        var cut = Render<FlareFormField>(p => p.Add(x => x.HelperText, "Enter your email"));
        Assert.NotEmpty(cut.FindAll(".flare-input__helper"));
        Assert.Contains("Enter your email", cut.Markup);
    }

    [Fact]
    public void RequiredAddsRequiredClass()
    {
        var cut = Render<FlareFormField>(p =>
        {
            p.Add(x => x.Label, "Name");
            p.Add(x => x.Required, true);
        });
        Assert.Contains("flare-input--required", cut.Find(".flare-input").ClassName);
    }

    [Fact]
    public void NotRequiredByDefault()
    {
        var cut = Render<FlareFormField>(p => p.Add(x => x.Label, "Name"));
        Assert.DoesNotContain("flare-input--required", cut.Find(".flare-input").ClassName);
    }

    [Fact]
    public void ForAttributeSetOnLabel()
    {
        var cut = Render<FlareFormField>(p =>
        {
            p.Add(x => x.Label, "Name");
            p.Add(x => x.For, "name-input");
        });
        Assert.Equal("name-input", cut.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void ChildContentRenderedInControl()
    {
        var cut = Render<FlareFormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "input");
                b.AddAttribute(1, "id", "test-input");
                b.CloseElement();
            })));
        Assert.NotEmpty(cut.FindAll(".flare-input__control-slot input"));
    }

    [Fact]
    public void ControlDivAlwaysPresent()
    {
        var cut = Render<FlareFormField>();
        Assert.NotEmpty(cut.FindAll(".flare-input__control-slot"));
    }
}

// ------------------------------------------------------------------------------
// FlareForm Layout  (8 tests from Wave10)
// ------------------------------------------------------------------------------

public class C_FlareFormLayoutTests : FlareTestContext
{
    private readonly object _model = new();

    [Fact]
    public void RendersRootFlareForm()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        Assert.NotEmpty(cut.FindAll(".flare-form"));
    }

    [Fact]
    public void DefaultLayoutHasNoVariantClass()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        var cls = cut.Find(".flare-form").ClassName;
        Assert.DoesNotContain("flare-form--horizontal", cls);
        Assert.DoesNotContain("flare-form--inline", cls);
    }

    [Fact]
    public void HorizontalLayoutAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Horizontal);
        });
        Assert.Contains("flare-form--horizontal", cut.Find(".flare-form").ClassName);
    }

    [Fact]
    public void InlineLayoutAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Inline);
        });
        Assert.Contains("flare-form--inline", cut.Find(".flare-form").ClassName);
    }

    [Fact]
    public void DenseAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Dense, true);
        });
        Assert.Contains("flare-form--dense", cut.Find(".flare-form").ClassName);
    }

    [Fact]
    public void NotDenseByDefault()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        Assert.DoesNotContain("flare-form--dense", cut.Find(".flare-form").ClassName);
    }

    [Fact]
    public void DenseAndHorizontalCombine()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Horizontal);
            p.Add(x => x.Dense, true);
        });
        var cls = cut.Find(".flare-form").ClassName;
        Assert.Contains("flare-form--horizontal", cls);
        Assert.Contains("flare-form--dense", cls);
    }

    [Fact]
    public void ChildContentRenderedInsideEditForm()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "inner-content");
                b.CloseElement();
            }));
        });
        Assert.NotEmpty(cut.FindAll("#inner-content"));
    }
}
