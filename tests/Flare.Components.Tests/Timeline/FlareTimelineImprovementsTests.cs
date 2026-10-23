namespace Flare.Components.Tests;

public class FlareTimelineImprovementsTests : FlareTestContext
{
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
