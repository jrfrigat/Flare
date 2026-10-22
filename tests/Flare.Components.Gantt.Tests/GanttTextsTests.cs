using System.Globalization;
using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Gantt.Tests;

/// <summary>Texts come from the package resources in the chart's culture; Texts replaces only what it sets.</summary>
public sealed class GanttTextsTests : GanttTestBase
{
    [Fact]
    public void Every_text_comes_from_the_resources_of_the_culture()
    {
        var en = GanttTexts.For(CultureInfo.GetCultureInfo("en-US"));
        var ru = GanttTexts.For(Ru);

        Assert.Equal("Expand", en.Expand);
        Assert.Equal("{0} moved to {1}.", en.Moved);
        Assert.NotEqual(en.Expand, ru.Expand);
        Assert.All(new[] { en.SelfLink, en.Cycle, en.Duplicate, en.Expand, en.Collapse, en.LinkStarted, en.LinkCreated, en.Moved, en.ResizeSide },
            text => Assert.False(string.IsNullOrEmpty(text) || text.StartsWith("Gantt_", StringComparison.Ordinal)));
    }

    [Fact]
    public void A_partial_override_replaces_only_what_it_sets()
    {
        var texts = new GanttTexts { Moved = "{0} -> {1}" }.Over(CultureInfo.GetCultureInfo("en-US"));

        Assert.Equal("{0} -> {1}", texts.Moved);
        Assert.Equal("Collapse", texts.Collapse);
    }

    [Fact]
    public void The_chart_uses_the_override_and_its_culture_for_the_rest()
    {
        var plan = new[] { new Work("a", "r1", Day(0), Day(1)) };
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r1") }).Add(x => x.Items, plan).Add(x => x.Editable, true),
            g => g.Add(x => x.Texts, new GanttTexts { Moved = "moved {0}" }));

        cut.Find(".flare-gantt__bar[data-key='a']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });

        Assert.Equal("moved a", cut.Find("[role=status]").TextContent);
        Assert.Equal(GanttTexts.For(Ru).ResizeSide, cut.Instance.EffectiveTexts.ResizeSide);
    }
}
