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

    [Fact]
    public void WithoutStatus_TitleKeepsItsPlaceAndNoHeaderIsRendered()
    {
        var cut = Render<FlareTimelineItem>(p => p.Add(x => x.Title, "Build"));

        Assert.Empty(cut.FindAll($".{Css.Classes.Timeline.Header}"));
        Assert.NotNull(cut.Find($".{Css.Classes.Timeline.Body} > .{Css.Classes.Timeline.Title}"));
    }

    [Fact]
    public void Status_RendersABadgeInTheItemsColorOnTheTitlesLine()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Title, "Build")
            .Add(x => x.Status, "Running")
            .Add(x => x.Color, FlareColor.Warning));

        var header = cut.Find($".{Css.Classes.Timeline.Header}");
        Assert.Equal("Build", header.QuerySelector($".{Css.Classes.Timeline.Title}")!.TextContent);
        var badge = header.QuerySelector($".{Css.Classes.Timeline.Status} .{Css.Classes.Badge.Indicator}")!;
        Assert.Equal("Running", badge.TextContent.Trim());
        Assert.Contains(Css.Classes.Color.Warning, badge.ClassName);
        Assert.Contains(Css.Classes.Color.Warning, cut.Find($".{Css.Classes.Timeline.Item}").ClassName);
    }

    [Fact]
    public void StatusContent_TakesThePlaceOfTheStatusBadge()
    {
        var cut = Render<FlareTimelineItem>(p => p
            .Add(x => x.Title, "Build")
            .Add(x => x.Status, "Running")
            .Add(x => x.StatusContent, b => b.AddMarkupContent(0, "<em class=\"own-state\">3/5</em>")));

        var status = cut.Find($".{Css.Classes.Timeline.Status}");
        Assert.NotNull(status.QuerySelector(".own-state"));
        Assert.Null(status.QuerySelector($".{Css.Classes.Badge.Indicator}"));
    }
}
