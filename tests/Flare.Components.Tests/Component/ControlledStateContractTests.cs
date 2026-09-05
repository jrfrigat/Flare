using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The two-way state contract every component with a bindable state parameter follows:
///
/// - UNCONTROLLED (no <c>XChanged</c> delegate): the component's own state survives a parent re-render.
///   Only a real change of the parameter overrides it.
/// - CONTROLLED (the caller listens): the parameter is authoritative - a parent that receives the change
///   and declines to move the parameter puts the component back.
///
/// The regression these lock down: the "did the parameter change" mirror being written from the event
/// handler with the LOCAL value, which makes the mirror disagree with the parameter, so the next parent
/// re-render for any unrelated reason is misread as an external change and reverts the component.
/// </summary>
public class ControlledStateContractTests : FlareTestContext
{
    // A parent that re-renders for a reason of its own, without touching the child's parameter.
    private sealed class UnrelatedRerenderHost : ComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int Tick { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "data-tick", Tick.ToString());
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }

    // ---- FlareCollapse -------------------------------------------------------------------------

    [Fact]
    public void Collapse_Uncontrolled_KeepsOpenStateAcrossAnUnrelatedParentRerender()
    {
        var cut = Render<UnrelatedRerenderHost>(ps => ps
            .Add(p => p.Tick, 0)
            .Add(p => p.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FlareCollapse>(0);
                b.AddAttribute(1, "Header", "Details");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>body</p>")));
                b.CloseComponent();
            })));

        cut.Find("button").Click();
        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Markup);

        // Something else on the page changes; the collapse must not care.
        cut.Render(ps => ps.Add(p => p.Tick, 1));
        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Markup);
    }

    [Fact]
    public void Collapse_Controlled_FollowsTheParameterWhenTheParentDeclinesTheChange()
    {
        var raised = 0;
        var cut = Render<FlareCollapse>(ps => ps
            .Add(p => p.Header, "Details")
            .Add(p => p.Expanded, false)
            // A parent that listens and deliberately keeps Expanded false (e.g. a guard that vetoes).
            .Add(p => p.ExpandedChanged, EventCallback.Factory.Create<bool>(this, _ => raised++))
            .Add(p => p.ChildContent, (RenderFragment)(c => c.AddMarkupContent(0, "<p>body</p>"))));

        cut.Find("button").Click();
        Assert.Equal(1, raised);

        // The parent re-renders without moving Expanded: the region goes back to what the parent says.
        cut.Render(ps => ps.Add(p => p.Expanded, false));
        Assert.DoesNotContain(Css.Classes.Collapse.Expanded, cut.Markup);
    }

    [Fact]
    public void Collapse_Controlled_AdoptsAnExternalChange()
    {
        var cut = Render<FlareCollapse>(ps => ps
            .Add(p => p.Header, "Details")
            .Add(p => p.Expanded, false)
            .Add(p => p.ExpandedChanged, EventCallback.Factory.Create<bool>(this, _ => { }))
            .Add(p => p.ChildContent, (RenderFragment)(c => c.AddMarkupContent(0, "<p>body</p>"))));

        cut.Render(ps => ps.Add(p => p.Expanded, true));
        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Markup);
    }

    [Fact]
    public void Collapse_Uncontrolled_StillAdoptsARealParameterChange()
    {
        var cut = Render<FlareCollapse>(ps => ps
            .Add(p => p.Header, "Details")
            .Add(p => p.Expanded, false)
            .Add(p => p.ChildContent, (RenderFragment)(c => c.AddMarkupContent(0, "<p>body</p>"))));

        cut.Render(ps => ps.Add(p => p.Expanded, true));
        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Markup);
    }

    // ---- FlareToggleButton ---------------------------------------------------------------------

    [Fact]
    public void ToggleButton_Uncontrolled_KeepsPressedStateAcrossAnUnrelatedParentRerender()
    {
        var cut = Render<UnrelatedRerenderHost>(ps => ps
            .Add(p => p.Tick, 0)
            .Add(p => p.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FlareToggleButton>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Bold")));
                b.CloseComponent();
            })));

        var button = cut.Find("button");
        button.Click();
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));

        cut.Render(ps => ps.Add(p => p.Tick, 1));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
    }

    // ---- FlareAccordionPanel -------------------------------------------------------------------

    // The panel used to copy Expanded once and ignore the parameter forever after, so a controlled parent
    // could not drive it at all. It now runs the same mirror as FlareCollapse above.
    [Fact]
    public void AccordionPanel_Controlled_FollowsAnExternalExpandedChange()
    {
        var cut = Render<FlareAccordionPanel>(ps => ps
            .Add(p => p.Header, "Details")
            .Add(p => p.Expanded, false)
            .Add(p => p.ExpandedChanged, EventCallback.Factory.Create<bool>(this, _ => { }))
            .Add(p => p.ChildContent, (RenderFragment)(c => c.AddMarkupContent(0, "<p>body</p>"))));

        Assert.Equal("false", cut.Find("button").GetAttribute("aria-expanded"));

        cut.Render(ps => ps.Add(p => p.Expanded, true));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-expanded"));
    }

    // The half the old one-shot sync was protecting, and the reason the fix is a mirror rather than a
    // plain assignment: reading the parameter on every set must NOT undo a local toggle when the parent
    // re-renders for its own reasons - which, inside an accordion, it does whenever a sibling moves.
    [Fact]
    public void AccordionPanel_Uncontrolled_KeepsOpenStateAcrossAnUnrelatedParentRerender()
    {
        var cut = Render<UnrelatedRerenderHost>(ps => ps
            .Add(p => p.Tick, 0)
            .Add(p => p.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FlareAccordionPanel>(0);
                b.AddAttribute(1, "Header", "Details");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>body</p>")));
                b.CloseComponent();
            })));

        cut.Find("button").Click();
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-expanded"));

        cut.Render(ps => ps.Add(p => p.Tick, 1));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-expanded"));
    }
}
