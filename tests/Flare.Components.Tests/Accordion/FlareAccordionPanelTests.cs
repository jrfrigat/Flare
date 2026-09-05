using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareAccordionPanelTests : FlareTestContext
{
    private IRenderedComponent<FlareAccordionPanel> RenderPanel(
        Action<ComponentParameterCollectionBuilder<FlareAccordionPanel>>? configure = null)
    {
        return Render<FlareAccordion>(p => p
            .AddChildContent<FlareAccordionPanel>(configure ?? (_ => { })))
            .FindComponent<FlareAccordionPanel>();
    }

    [Fact]
    public void RendersRootDiv()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "My Header"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Panel}"));
    }

    [Fact]
    public void RendersHeaderText()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Section Title"));

        Assert.Contains("Section Title", cut.Markup);
    }

    [Fact]
    public void RendersHeaderButton()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Clickable Header"));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Accordion.Header}"));
    }

    [Fact]
    public void CollapsedByDefault_AriaExpandedFalse()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Panel"));

        var btn = cut.Find($"button.{Css.Classes.Accordion.Header}");
        Assert.Equal("false", btn.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ExpandedParam_True_AriaExpandedTrue()
    {
        var cut = RenderPanel(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, true));

        var btn = cut.Find($"button.{Css.Classes.Accordion.Header}");
        Assert.Equal("true", btn.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ExpandedParam_True_HasExpandedClass()
    {
        var cut = RenderPanel(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Expanded}"));
    }

    [Fact]
    public void CollapsedByDefault_NoExpandedClass()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Panel"));

        Assert.Empty(cut.FindAll($".{Css.Classes.Accordion.Expanded}"));
    }

    [Fact]
    public void ToggleExpandsPanel_ClickHeader()
    {
        var cut = RenderPanel(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, false));

        cut.Find($"button.{Css.Classes.Accordion.Header}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Accordion.Expanded}"));
    }
}

// ------------------------------------------------------------------------------
// FlareStepper  (8 tests from Wave5)
// ------------------------------------------------------------------------------
