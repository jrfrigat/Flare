using Bunit;
using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Flare.Components.Gantt.Tests;

/// <summary>The row-header width changes from the keyboard and buttons, not only by dragging.</summary>
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

    [Fact]
    public void The_separator_is_focusable_and_reports_the_width()
    {
        var cut = Resizable([]);

        var handle = cut.Find(".flare-gantt__side-handle");
        Assert.Equal("0", handle.GetAttribute("tabindex"));
        Assert.Equal("vertical", handle.GetAttribute("aria-orientation"));
        // 22rem = 352px; the floor and the cap set the separator's value range.
        Assert.Equal("352", handle.GetAttribute("aria-valuenow"));
        Assert.Equal("120", handle.GetAttribute("aria-valuemin"));
        Assert.Equal("1200", handle.GetAttribute("aria-valuemax"));
    }

    [Fact]
    public void Arrow_keys_change_the_width_and_Home_resets_it()
    {
        var seen = new List<string>();
        var cut = Resizable(seen);

        cut.Find(".flare-gantt__side-handle").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("368px", Assert.Single(seen));
        Assert.Contains("--_gantt-side:368px", cut.Find(".flare-gantt").GetAttribute("style"));

        cut.Find(".flare-gantt__side-handle").KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        Assert.Equal("352px", seen[^1]);

        cut.Find(".flare-gantt__side-handle").KeyDown(new KeyboardEventArgs { Key = "Home" });
        Assert.Equal("22rem", seen[^1]);
    }

    [Fact]
    public void The_step_buttons_change_and_reset_the_width_without_dragging()
    {
        var seen = new List<string>();
        var cut = Resizable(seen);

        cut.FindAll(".flare-gantt__side-button")[1].Click(); // wider
        Assert.Equal("368px", seen[^1]);

        cut.FindAll(".flare-gantt__side-button")[0].Click(); // narrower
        Assert.Equal("352px", seen[^1]);

        cut.FindAll(".flare-gantt__side-button")[2].Click(); // reset
        Assert.Equal("22rem", seen[^1]);
    }
}
