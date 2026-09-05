using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests;

public class FlareFormFieldTests : FlareTestContext
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
