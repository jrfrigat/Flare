namespace Flare.Components.Tests;

public class FlareTimelineImprovementsTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareTimeline()
    {
        var cut = Render<FlareTimeline>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Timeline.Root}"));
    }

    [Fact]
    public void AlignAlternate_AddsAlternateClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Alternate));

        Assert.Contains(Css.Classes.Timeline.Alternate, cut.Find($".{Css.Classes.Timeline.Root}").ClassName ?? "");
    }

    [Fact]
    public void AlignRight_AddsRightClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Right));

        Assert.Contains(Css.Classes.Timeline.Right, cut.Find($".{Css.Classes.Timeline.Root}").ClassName ?? "");
    }

    [Fact]
    public void FlareTimelineItem_RendersFlareTimelineItemClass()
    {
        var cut = Render<FlareTimelineItem>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Timeline.Item}"));
    }

    [Fact]
    public void FlareTimelineItem_Color_Secondary_AddsSecondaryClass()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Color, FlareColor.Secondary));

        Assert.Contains(Css.Classes.Color.Secondary, cut.Find($".{Css.Classes.Timeline.Item}").ClassName ?? "");
    }

    [Fact]
    public void Timeline_RendersChildFlareTimelineItems()
    {
        var cut = Render<FlareTimeline>(p => p
            .AddChildContent<FlareTimelineItem>(bp => bp
                .Add(x => x.Title, "Step 1")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Timeline.Item}"));
    }

    [Fact]
    public void FlareTimelineItem_Title_RendersTitle()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Title, "Released"));

        Assert.Contains("Released", cut.Find($".{Css.Classes.Timeline.Title}").TextContent);
    }
}
