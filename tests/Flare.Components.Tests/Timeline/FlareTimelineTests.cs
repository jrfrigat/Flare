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

    [Fact]
    public void Root_IsTheListItsItemsBelongTo()
    {
        var cut = Render<FlareTimeline>(p => p
            .AddChildContent<FlareTimelineItem>(i => i.Add(x => x.Title, "One"))
            .AddChildContent<FlareTimelineItem>(i => i.Add(x => x.Title, "Two")));

        var root = cut.Find($".{Css.Classes.Timeline.Root}");
        Assert.Equal("list", root.GetAttribute("role"));
        Assert.Equal(2, root.QuerySelectorAll(":scope > [role='listitem']").Length);
    }

    [Fact]
    public void NestedTimeline_IsItsOwnList()
    {
        var cut = Render<FlareTimeline>(p => p
            .AddChildContent<FlareTimelineItem>(outer => outer
                .Add(x => x.Title, "Outer")
                .AddChildContent<FlareTimeline>(inner => inner
                    .AddChildContent<FlareTimelineItem>(item => item.Add(x => x.Title, "Inner")))));

        // Each timeline owns its own items: the outer list has one, the nested one has the other.
        var lists = cut.FindAll($".{Css.Classes.Timeline.Root}");
        Assert.Equal(2, lists.Count);
        Assert.Single(lists[0].QuerySelectorAll(":scope > [role='listitem']"));
        Assert.Single(lists[1].QuerySelectorAll(":scope > [role='listitem']"));
    }
}
