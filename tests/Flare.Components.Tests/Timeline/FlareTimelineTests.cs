namespace Flare.Components.Tests;

public class FlareTimelineTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTimeline>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Timeline.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTimeline>(p => p
            .AddChildContent("<div class=\"custom-item\">Item</div>"));

        Assert.NotEmpty(cut.FindAll(".custom-item"));
    }

    [Fact]
    public void AlignRight_HasRightClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Right));

        Assert.Contains(Css.Classes.Timeline.Right, cut.Find($".{Css.Classes.Timeline.Root}").ClassName);
    }

    [Fact]
    public void AlignAlternate_HasAlternateClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Alternate));

        Assert.Contains(Css.Classes.Timeline.Alternate, cut.Find($".{Css.Classes.Timeline.Root}").ClassName);
    }

    [Fact]
    public void AlignLeft_HasNoAlignClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Left));

        var className = cut.Find($".{Css.Classes.Timeline.Root}").ClassName ?? string.Empty;
        Assert.DoesNotContain(Css.Classes.Timeline.Right, className);
        Assert.DoesNotContain(Css.Classes.Timeline.Alternate, className);
    }
}

// ------------------------------------------------------------------------------
// FlareRating  (6 tests from Wave3)
// ------------------------------------------------------------------------------
