using AngleSharp.Dom;

namespace Flare.Components.Tests;

// The value axis: how many grid lines get drawn, where they land, and what the labels then read. The count
// and the rounding are one calculation - a test that pinned only one of them would pass while the labels
// drifted off the lines they name.
public class FlareChartAxisTests : FlareTestContext
{
    private static ChartData Data(params double[] values) =>
        new([new ChartSeries("A", values)], ["q1", "q2", "q3", "q4"]);

    private static List<IElement> MajorLines(IRenderedComponent<FlareChart> cut) =>
        cut.FindAll("line").Where(l => !(l.GetAttribute("style") ?? "").Contains("grid-minor")).ToList();

    private static List<IElement> MinorLines(IRenderedComponent<FlareChart> cut) =>
        cut.FindAll("line").Where(l => (l.GetAttribute("style") ?? "").Contains("grid-minor")).ToList();

    private static List<string> Labels(IRenderedComponent<FlareChart> cut) =>
        cut.FindAll("text").Select(t => t.TextContent.Trim()).ToList();

    [Fact]
    public void The_default_axis_still_draws_five_lines()
    {
        // The count is derived from the plot height now instead of hardcoded, so a default 220px chart has
        // to land on the same five lines it drew before or every existing chart silently changes.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8)));

        Assert.Equal(5, MajorLines(cut).Count);
    }

    [Fact]
    public void A_short_chart_draws_fewer_lines_than_a_tall_one()
    {
        var shortChart = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.NiceScale, false)
            .Add(x => x.Height, 100));
        var tallChart = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.NiceScale, false)
            .Add(x => x.Height, 600));

        Assert.True(MajorLines(shortChart).Count < MajorLines(tallChart).Count);
        Assert.Equal(3, MajorLines(shortChart).Count);
        Assert.Equal(9, MajorLines(tallChart).Count);
    }

    [Fact]
    public void YAxisTickCount_sets_the_line_count_exactly_when_rounding_is_off()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.NiceScale, false)
            .Add(x => x.YAxisTickCount, 8));

        Assert.Equal(8, MajorLines(cut).Count);
    }

    [Fact]
    public void Every_grid_line_carries_a_label()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.NiceScale, false)
            .Add(x => x.ShowXAxisLabels, false)
            .Add(x => x.YAxisTickCount, 7));

        Assert.Equal(7, MajorLines(cut).Count);
        Assert.Equal(7, Labels(cut).Count);
    }

    [Fact]
    public void NiceScale_rounds_the_axis_out_to_whole_steps()
    {
        // 94..470 over five lines wants a step of 94 - unreadable. Rounded to 100 the axis reads 0..500,
        // which is the whole reason a tick count is worth exposing at all.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(94, 200, 470, 310)));

        var labels = Labels(cut);
        Assert.Contains("0", labels);
        Assert.Contains("100", labels);
        Assert.Contains("500", labels);
        Assert.DoesNotContain("470", labels);
    }

    [Fact]
    public void Rounding_overshoots_the_asked_count_by_at_most_one()
    {
        // Snapping the step down and then extending both bounds outward compound: asking for seven lines
        // over 94..470 once produced ten. The step now climbs until the overshoot is inside what the
        // parameter promises.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(94, 200, 470, 310))
            .Add(x => x.YAxisTickCount, 7));

        var lines = MajorLines(cut).Count;
        Assert.InRange(lines, 2, 8);
        // and the labels are still round, which is what the extra line was bought with
        Assert.All(Labels(cut).Take(lines), l => Assert.EndsWith("0", l));
    }

    [Fact]
    public void NiceScale_off_keeps_the_raw_data_bounds()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(94, 200, 470, 310))
            .Add(x => x.NiceScale, false));

        Assert.Contains("470", Labels(cut));
    }

    [Fact]
    public void Rounding_comes_off_the_range_not_the_magnitude()
    {
        // A near-flat series high above zero must not be rounded down to a 0..1100 axis that flattens it
        // into the top edge. The nice step is derived from max-min, so the window stays tight.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1003, 1004, 1002, 1005)));

        var labels = Labels(cut);
        Assert.DoesNotContain("0", labels);
        Assert.Contains(labels, l => l.StartsWith("100", StringComparison.Ordinal));
    }

    [Fact]
    public void An_axis_pinned_at_both_ends_is_never_rounded()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(94, 200, 470, 310))
            .Add(x => x.YMin, 0.0)
            .Add(x => x.YMax, 470.0));

        // Verbatim bounds, and one uniform precision across the axis: a step of 117.5 makes every label
        // carry a decimal, including the ones that happen to be whole.
        Assert.Equal(["0.0", "117.5", "235.0", "352.5", "470.0"], Labels(cut).Take(5));
    }

    [Fact]
    public void Pinning_one_end_rounds_only_the_other()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(94, 200, 470, 310))
            .Add(x => x.YMin, 0.0));

        var labels = Labels(cut);
        Assert.Contains("0", labels);
        Assert.Contains("500", labels);
    }

    [Fact]
    public void Minor_ticks_add_lines_but_no_labels()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.NiceScale, false)
            .Add(x => x.ShowXAxisLabels, false)
            .Add(x => x.YAxisTickCount, 5)
            .Add(x => x.YAxisMinorTicks, 3));

        Assert.Equal(5, MajorLines(cut).Count);
        Assert.Equal(12, MinorLines(cut).Count);   // four bands x three divisions
        Assert.Equal(5, Labels(cut).Count);        // labels stay on the majors
    }

    [Fact]
    public void Minor_lines_are_themed_apart_from_the_majors()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.YAxisMinorTicks, 1));

        var style = MinorLines(cut)[0].GetAttribute("style") ?? "";
        Assert.Contains($"var({Css.Tokens.Chart.GridMinorColor})", style);
        Assert.Contains($"var({Css.Tokens.Chart.GridMinorWidth})", style);
    }

    [Fact]
    public void ShowVerticalGrid_adds_one_line_per_category()
    {
        var without = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8)));
        var with = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.ShowVerticalGrid, true));

        Assert.Equal(MajorLines(without).Count + 4, MajorLines(with).Count);
    }

    [Fact]
    public void ShowGrid_off_suppresses_every_line()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.ShowGrid, false)
            .Add(x => x.ShowVerticalGrid, true)
            .Add(x => x.YAxisMinorTicks, 2));

        Assert.Empty(cut.FindAll("line"));
    }

    [Fact]
    public void The_tick_count_reaches_bar_stacked_and_combo_alike()
    {
        // GridLines is shared, but each renderer resolves its own axis; one left on the old constant would
        // keep drawing five while the others obeyed.
        foreach (var type in new[] { ChartType.Bar, ChartType.StackedBar, ChartType.Combo })
        {
            var cut = Render<FlareChart>(p => p
                .Add(x => x.Type, type)
                .Add(x => x.Data, Data(1, 5, 3, 8))
                .Add(x => x.NiceScale, false)
                .Add(x => x.YAxisTickCount, 6));

            Assert.Equal(6, MajorLines(cut).Count(l => l.GetAttribute("y1") == l.GetAttribute("y2")));
        }
    }

    [Fact]
    public void A_horizontal_bar_chart_ticks_along_its_own_axis()
    {
        // The value axis runs across the width there, so its line count is budgeted against the width.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Bar)
            .Add(x => x.Data, Data(1, 5, 3, 8))
            .Add(x => x.Horizontal, true)
            .Add(x => x.NiceScale, false)
            .Add(x => x.YAxisTickCount, 7));

        Assert.Equal(7, MajorLines(cut).Count(l => l.GetAttribute("x1") == l.GetAttribute("x2")));
    }
}
