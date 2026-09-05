namespace Flare.Components.Tests;

public class FlareTimelineItemTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTimelineItem>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Timeline.Item}"));
    }

    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Title, "Release 1.0"));

        Assert.Contains("Release 1.0", cut.Find($".{Css.Classes.Timeline.Title}").TextContent);
    }

    [Fact]
    public void RendersTime()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Time, "2026-05-24"));

        Assert.Contains("2026-05-24", cut.Find($".{Css.Classes.Timeline.Time}").TextContent);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .AddChildContent("<p class=\"tl-body\">Details here</p>"));

        Assert.NotEmpty(cut.FindAll(".tl-body"));
    }

    [Fact]
    public void ColorSuccess_HasSuccessClass()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Color, FlareColor.Success));

        Assert.Contains(Css.Classes.Color.Success, cut.Find($".{Css.Classes.Timeline.Item}").ClassName);
    }
}
