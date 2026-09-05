using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// What a screen reader is told about the two disclosure components. Both defects here are of the same
/// kind: the markup was correct for someone looking at it and wrong for someone navigating it - an
/// accordion whose sections were invisible to heading navigation, and a collapse that added an entry to
/// the landmark list with no name to identify it by.
/// </summary>
public sealed class DisclosureAccessibilityTests : FlareTestContext
{
    private static RenderFragment Panels(int? panelLevel = null) => b =>
    {
        b.OpenComponent<FlareAccordionPanel>(0);
        b.AddAttribute(1, "Header", "First");
        if (panelLevel is not null) b.AddAttribute(2, "HeadingLevel", panelLevel);
        b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddContent(0, "body")));
        b.CloseComponent();
    };

    [Fact]
    public void AccordionHeader_SitsInsideAHeading()
    {
        var cut = Render<FlareAccordion>(p => p.Add(x => x.ChildContent, Panels()));

        var heading = cut.Find($".{Css.Classes.Accordion.Heading}");
        Assert.Equal("heading", heading.GetAttribute("role"));
        Assert.Equal("3", heading.GetAttribute("aria-level"));
        // The pattern asks for the button to be the heading's only content: anything else placed there
        // is unreachable by the navigation the heading exists for.
        Assert.Single(heading.Children);
        Assert.Equal("button", heading.Children[0].LocalName);
    }

    [Fact]
    public void AccordionHeadingLevel_FollowsTheSurroundingDocument()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.HeadingLevel, 2)
            .Add(x => x.ChildContent, Panels()));

        Assert.Equal("2", cut.Find($".{Css.Classes.Accordion.Heading}").GetAttribute("aria-level"));
    }

    [Fact]
    public void APanelMayOverrideTheAccordionsLevel()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.HeadingLevel, 2)
            .Add(x => x.ChildContent, Panels(panelLevel: 4)));

        Assert.Equal("4", cut.Find($".{Css.Classes.Accordion.Heading}").GetAttribute("aria-level"));
    }

    // An accordion used as a control rather than as a section of the document should not inject
    // headings into the page outline.
    [Fact]
    public void HeadingLevelZero_DropsTheHeadingSemantics()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.HeadingLevel, 0)
            .Add(x => x.ChildContent, Panels()));

        var heading = cut.Find($".{Css.Classes.Accordion.Heading}");
        Assert.Null(heading.GetAttribute("role"));
        Assert.Null(heading.GetAttribute("aria-level"));
    }

    [Fact]
    public void AccordionToggleContract_SurvivesTheHeadingWrapper()
    {
        var cut = Render<FlareAccordion>(p => p.Add(x => x.ChildContent, Panels()));

        var button = cut.Find($".{Css.Classes.Accordion.Header}");
        Assert.Equal("false", button.GetAttribute("aria-expanded"));
        Assert.False(string.IsNullOrEmpty(button.GetAttribute("aria-controls")));

        button.Click();
        Assert.Equal("true", cut.Find($".{Css.Classes.Accordion.Header}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void HeaderedCollapse_IsARegionNamedByItsHeader()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Header, "Advanced")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "body"))));

        var region = cut.Find($".{Css.Classes.Collapse.Region}");
        Assert.Equal("region", region.GetAttribute("role"));
        Assert.Equal(cut.Find($".{Css.Classes.Collapse.Header}").Id, region.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void HeaderlessCollapse_IsNotAnUnnamedLandmark()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Expanded, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "body"))));

        var region = cut.Find($".{Css.Classes.Collapse.Region}");
        Assert.Null(region.GetAttribute("role"));
        Assert.Null(region.GetAttribute("aria-labelledby"));
        Assert.Null(region.GetAttribute("aria-label"));
    }

    [Fact]
    public void HeaderlessCollapse_BecomesALandmarkOnceNamed()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Expanded, true)
            .Add(x => x.RegionLabel, "Advanced filters")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "body"))));

        var region = cut.Find($".{Css.Classes.Collapse.Region}");
        Assert.Equal("region", region.GetAttribute("role"));
        Assert.Equal("Advanced filters", region.GetAttribute("aria-label"));
    }
}
