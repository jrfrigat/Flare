// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareTimeline improvements  (7 tests)
// Note: ConnectorVariant and Alternating were not added to the source;
// the component uses TimelineAlign enum (Left/Right/Alternate) instead.
// Tests cover what actually exists in the source.
// -----------------------------------------------------------------------------

public class FlareTimelineImprovementsTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareTimeline()
    {
        var cut = RenderComponent<FlareTimeline>();

        Assert.NotEmpty(cut.FindAll(".flare-timeline"));
    }

    [Fact]
    public void AlignAlternate_AddsAlternateClass()
    {
        var cut = RenderComponent<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Alternate));

        Assert.Contains("flare-timeline--alternate", cut.Find(".flare-timeline").ClassName ?? "");
    }

    [Fact]
    public void AlignRight_AddsRightClass()
    {
        var cut = RenderComponent<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Right));

        Assert.Contains("flare-timeline--right", cut.Find(".flare-timeline").ClassName ?? "");
    }

    [Fact]
    public void FlareTimelineItem_RendersFlareTimelineItemClass()
    {
        var cut = RenderComponent<FlareTimelineItem>();

        Assert.NotEmpty(cut.FindAll(".flare-timeline-item"));
    }

    [Fact]
    public void FlareTimelineItem_Color_Secondary_AddsSecondaryClass()
    {
        var cut = RenderComponent<FlareTimelineItem>(p => p
            .Add(x => x.Color, FlareColor.Secondary));

        Assert.Contains("flare-color-secondary", cut.Find(".flare-timeline-item").ClassName ?? "");
    }

    [Fact]
    public void Timeline_RendersChildFlareTimelineItems()
    {
        var cut = RenderComponent<FlareTimeline>(p => p
            .AddChildContent<FlareTimelineItem>(bp => bp
                .Add(x => x.Title, "Step 1")));

        Assert.NotEmpty(cut.FindAll(".flare-timeline-item"));
    }

    [Fact]
    public void FlareTimelineItem_Title_RendersTitle()
    {
        var cut = RenderComponent<FlareTimelineItem>(p => p
            .Add(x => x.Title, "Released"));

        Assert.Contains("Released", cut.Find(".flare-timeline-item__title").TextContent);
    }
}
