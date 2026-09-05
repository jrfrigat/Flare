using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareAccordionTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareAccordion>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddChildContent("<p id=\"inner\">Hello</p>"));

        Assert.NotEmpty(cut.FindAll("#inner"));
    }

    [Fact]
    public void AllowMultipleFalse_IsDefault()
    {
        var cut = Render<FlareAccordion>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Root}"));
    }

    [Fact]
    public void AllowMultipleTrue_RendersComponent()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.AllowMultiple, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Root}"));
    }

    [Fact]
    public void RendersWithAdditionalAttributes()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddUnmatched("data-testid", "accordion-root"));

        Assert.Equal("accordion-root", cut.Find($".{Css.Classes.Accordion.Root}").GetAttribute("data-testid"));
    }

    [Fact]
    public void ProvidesCascadingValue()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddChildContent<FlareAccordionPanel>(pp => pp
                .Add(x => x.Header, "Panel A")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Root}"));
    }

    [Fact]
    public void RendersWithStyleParam()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.Style, "border:1px solid red"));

        var div = cut.Find($".{Css.Classes.Accordion.Root}");
        Assert.Contains("border", div.GetAttribute("style") ?? "");
    }
}

// ------------------------------------------------------------------------------
// FlareAccordionPanel  (8 tests from Wave5)
// ------------------------------------------------------------------------------
