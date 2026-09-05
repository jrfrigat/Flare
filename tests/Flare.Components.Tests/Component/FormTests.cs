using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareSwitch  (7 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareSwitchTests : FlareTestContext
{
    [Fact]
    public void RendersRootLabel()
    {
        var cut = Render<FlareSwitch>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Switch.Root}"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Enable feature"));

        Assert.Contains("Enable feature", cut.Find($".{Css.Classes.Switch.Label}").TextContent);
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

        Assert.Contains("Must be enabled", cut.Find($".{Css.Classes.Switch.Error}").TextContent);
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Radio.Root}"));
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

        Assert.Contains("Option A", cut.Find($".{Css.Classes.Radio.Label}").TextContent);
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

        Assert.Contains(Css.Classes.Radio.Disabled, cut.Find("label").ClassName);
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root}"));
    }

    [Fact]
    public void LabelRenderedWhenSet()
    {
        var cut = Render<FlareFormField>(p => p.Add(x => x.Label, "Email"));
        Assert.NotEmpty(cut.FindAll($"label.{Css.Classes.Input.Label}"));
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Helper}"));
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
        Assert.Contains(Css.Classes.Input.Required, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void NotRequiredByDefault()
    {
        var cut = Render<FlareFormField>(p => p.Add(x => x.Label, "Name"));
        Assert.DoesNotContain(Css.Classes.Input.Required, cut.Find($".{Css.Classes.Input.Root}").ClassName);
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.ControlSlot} input"));
    }

    [Fact]
    public void ControlDivAlwaysPresent()
    {
        var cut = Render<FlareFormField>();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.ControlSlot}"));
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Form.Root}"));
    }

    [Fact]
    public void DefaultLayoutHasNoVariantClass()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        var cls = cut.Find($".{Css.Classes.Form.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Form.Horizontal, cls);
        Assert.DoesNotContain(Css.Classes.Form.Inline, cls);
    }

    [Fact]
    public void HorizontalLayoutAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Horizontal);
        });
        Assert.Contains(Css.Classes.Form.Horizontal, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void InlineLayoutAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Inline);
        });
        Assert.Contains(Css.Classes.Form.Inline, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void DenseAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Dense, true);
        });
        Assert.Contains(Css.Classes.Form.Dense, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void NotDenseByDefault()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        Assert.DoesNotContain(Css.Classes.Form.Dense, cut.Find($".{Css.Classes.Form.Root}").ClassName);
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
        var cls = cut.Find($".{Css.Classes.Form.Root}").ClassName;
        Assert.Contains(Css.Classes.Form.Horizontal, cls);
        Assert.Contains(Css.Classes.Form.Dense, cls);
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
