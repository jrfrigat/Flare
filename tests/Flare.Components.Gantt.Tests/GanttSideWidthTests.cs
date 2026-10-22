using Bunit;
using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Flare.Components.Gantt.Tests;

/// <summary>The row-header column is resized with the shared separator, and yields to the axis on a narrow chart.</summary>
public sealed class GanttSideWidthTests : GanttTestBase
{
    private IRenderedComponent<FlareGantt> Resizable(List<string> seen) =>
        Chart(p => p.Add(x => x.Rows, new[] { new Node("r") }),
            g => g.Add(x => x.ResizableSide, true)
                  .Add(x => x.SideWidth, "22rem")
                  .Add(x => x.DefaultSideWidth, "22rem")
                  .Add(x => x.SideWidthChanged, (string width) => seen.Add(width)));

    // On a narrow screen the requested 22rem (352px) is wider than the chart itself (about 284px), and the axis started
    // off screen. The column yields to the axis: no more than seven tenths of the chart - the same cap as the
    // drag in gantt.js, so a width from the parameter and one after a drag behave alike.
    [Fact]
    public void The_column_yields_to_the_axis_when_the_chart_is_narrow()
    {
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") }),
            g => g.Add(x => x.SideWidth, "22rem"));

        // Until the browser reports sizes the width is taken as is: nothing changes on a wide screen.
        Assert.Contains("--_gantt-side:22rem", cut.Find(".flare-gantt").GetAttribute("style"));

        cut.InvokeAsync(() => cut.Instance.Viewport(284, 0));

        // 284 x 0.7 = 198.8: the column grows no wider, and the axis keeps about 85px.
        cut.WaitForAssertion(() => Assert.Contains("--_gantt-side:min(22rem, 199px)", cut.Find(".flare-gantt").GetAttribute("style")));
    }

    // The column's edge is the shared FlareSplitter controlling one width: it carries the width, its limits and the
    // default a reset returns to, and the browser moves it (keys, steps, drag, double click) - see the browser tests.
    [Fact]
    public void The_edge_is_a_shared_separator_that_controls_the_column_width()
    {
        var cut = Resizable([]);

        var edge = cut.Find(".flare-gantt__side-splitter");
        Assert.Equal("separator", edge.GetAttribute("role"));
        Assert.Equal("0", edge.GetAttribute("tabindex"));
        Assert.Equal("--_gantt-side", edge.GetAttribute("data-flare-splitter-variable"));
        // 22rem = 352px; the floor and the fallback cap until the browser reports the chart's width.
        Assert.Equal("352", edge.GetAttribute("aria-valuenow"));
        Assert.Equal("352", edge.GetAttribute("data-flare-splitter-default"));
        Assert.Equal("120", edge.GetAttribute("aria-valuemin"));
        Assert.Equal("1200", edge.GetAttribute("aria-valuemax"));
    }

    // A reset follows the parameters as they are now, not the values the chart happened to mount with.
    [Fact]
    public void A_changed_width_and_default_reach_the_reset_target()
    {
        var cut = Resizable([]);
        Assert.Equal("352", cut.Find(".flare-gantt__side-splitter").GetAttribute("data-flare-splitter-default"));

        cut.Render(p => p.Add(x => x.SideWidth, "18rem").Add(x => x.DefaultSideWidth, "18rem"));

        Assert.Equal("288", cut.Find(".flare-gantt__side-splitter").GetAttribute("data-flare-splitter-default"));
    }

    // The separator's limit is the column's: seven tenths of the chart once the browser has reported its width.
    [Fact]
    public void The_separator_follows_the_column_cap_on_a_narrow_chart()
    {
        var cut = Resizable([]);

        cut.InvokeAsync(() => cut.Instance.Viewport(284, 0));

        cut.WaitForAssertion(() => Assert.Equal("199", cut.Find(".flare-gantt__side-splitter").GetAttribute("aria-valuemax")));
        Assert.Equal("199", cut.Find(".flare-gantt__side-splitter").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void A_width_the_separator_settles_on_is_reported_and_kept()
    {
        var seen = new List<string>();
        var cut = Resizable(seen);

        cut.InvokeAsync(() => cut.FindComponent<FlareSplitter>().Instance.OnSplitChanged("368px"));

        Assert.Equal("368px", Assert.Single(seen));
        Assert.Contains("--_gantt-side:368px", cut.Find(".flare-gantt").GetAttribute("style"));
        Assert.Equal("368", cut.Find(".flare-gantt__side-splitter").GetAttribute("aria-valuenow"));
    }
}
