using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;

namespace Flare.Components.Tests;

/// <summary>
/// A collapsed panel leaves only visual space behind: inert removes its descendants from the tab order
/// and the accessibility tree, and a programmatic collapse with focus inside returns the focus to the
/// header.
/// </summary>
public class FlareAccordionAccessibilityTests : FlareTestContext
{
    [Fact]
    public void CollapsedContent_IsInert()
    {
        var cut = Render<FlareAccordionPanel>(p => p
            .Add(x => x.Header, "Panel")
            .AddChildContent("<input id=\"inner\" />"));

        Assert.True(cut.Find($".{Css.Classes.Accordion.Content}").HasAttribute("inert"));
    }

    [Fact]
    public void ExpandedContent_IsNotInert()
    {
        var cut = Render<FlareAccordionPanel>(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, true)
            .AddChildContent("<input id=\"inner\" />"));

        Assert.False(cut.Find($".{Css.Classes.Accordion.Content}").HasAttribute("inert"));
    }

    [Fact]
    public void CollapsingWithFocusInside_ReturnsFocusToHeader()
    {
        var cut = Render<FlareAccordionPanel>(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, true)
            .AddChildContent("<input id=\"inner\" />"));
        cut.Find($".{Css.Classes.Accordion.Content}").FocusIn();

        cut.Render(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, false)
            .AddChildContent("<input id=\"inner\" />"));

        JSInterop.VerifyFocusAsyncInvoke();
    }
}
