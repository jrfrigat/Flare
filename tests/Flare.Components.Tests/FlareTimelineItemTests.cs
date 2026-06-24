// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareTimelineItem  (5 tests)
// -----------------------------------------------------------------------------

public class FlareTimelineItemTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = RenderComponent<FlareTimelineItem>();

        Assert.NotEmpty(cut.FindAll(".flare-timeline-item"));
    }

    [Fact]
    public void RendersTitle()
    {
        var cut = RenderComponent<FlareTimelineItem>(p => p
            .Add(x => x.Title, "Release 1.0"));

        Assert.Contains("Release 1.0", cut.Find(".flare-timeline-item__title").TextContent);
    }

    [Fact]
    public void RendersTime()
    {
        var cut = RenderComponent<FlareTimelineItem>(p => p
            .Add(x => x.Time, "2026-05-24"));

        Assert.Contains("2026-05-24", cut.Find(".flare-timeline-item__time").TextContent);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = RenderComponent<FlareTimelineItem>(p => p
            .AddChildContent("<p class=\"tl-body\">Details here</p>"));

        Assert.NotEmpty(cut.FindAll(".tl-body"));
    }

    [Fact]
    public void ColorSuccess_HasSuccessClass()
    {
        var cut = RenderComponent<FlareTimelineItem>(p => p
            .Add(x => x.Color, FlareColor.Success));

        Assert.Contains("flare-color-success", cut.Find(".flare-timeline-item").ClassName);
    }
}
