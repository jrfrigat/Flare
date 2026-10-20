using System.Globalization;
using Flare.Components.Gantt;
using Xunit;

namespace Flare.Components.Gantt.Tests;

public sealed class GanttAxisTests
{
    private static readonly CultureInfo Ru = new("ru-RU");
    private static readonly CultureInfo Us = new("en-US");
    private static DateTime D(int y, int m, int d, int h = 0) => new(y, m, d, h, 0, 0);

    [Fact]
    public void Day_axis_counts_days_and_fractions()
    {
        var axis = new GanttAxis(GanttSpan.FromDays(new DateOnly(2026, 10, 12), new DateOnly(2026, 10, 23)), GanttScale.Day, Ru);

        Assert.Equal(12, axis.Units);
        Assert.Equal(0, axis.Offset(D(2026, 10, 12)));
        Assert.Equal(2.5, axis.Offset(D(2026, 10, 14, 12)));
        Assert.Equal(-1, axis.Offset(D(2026, 10, 11)));
        Assert.Equal(D(2026, 10, 14, 12), axis.At(2.5));
    }

    [Fact]
    public void Day_ticks_carry_number_and_weekday_and_months_above()
    {
        var axis = new GanttAxis(GanttSpan.FromDays(new DateOnly(2026, 10, 30), new DateOnly(2026, 11, 2)), GanttScale.Day, Ru);

        var ticks = axis.Ticks();
        Assert.Equal(["30", "31", "1", "2"], ticks.Select(t => t.Label));
        Assert.Equal(["\u043F\u0442", "\u0441\u0431", "\u0432\u0441", "\u043F\u043D"], ticks.Select(t => t.Caption));
        var months = axis.MajorTicks();
        Assert.Equal(["\u041E\u043A\u0442\u044F\u0431\u0440\u044C 2026", "\u041D\u043E\u044F\u0431\u0440\u044C 2026"], months.Select(t => t.Label));
        Assert.Equal([0d, 2d], months.Select(t => t.Offset));
        Assert.Equal([2d, 2d], months.Select(t => t.Width));
    }

    [Fact]
    public void Month_is_one_unit_whatever_its_length()
    {
        var axis = new GanttAxis(new GanttSpan(D(2026, 1, 1), D(2026, 4, 1)), GanttScale.Month, Ru);

        Assert.Equal(3, axis.Units);
        Assert.Equal(1, axis.Offset(D(2026, 2, 1)));
        Assert.Equal(1.5, axis.Offset(D(2026, 2, 15)));          // February has 28 days in 2026
        Assert.Equal(2.5, axis.Offset(D(2026, 3, 16, 12)), 6);    // March 16 noon is the middle of 31 days
        Assert.Equal(3, axis.Ticks().Count);
        Assert.Equal(["2026"], axis.MajorTicks().Select(t => t.Label));
    }

    [Fact]
    public void Week_starts_on_the_culture_first_day()
    {
        var thursday = D(2026, 10, 15);
        var ru = new GanttAxis(new GanttSpan(thursday, thursday.AddDays(14)), GanttScale.Week, Ru);
        var us = new GanttAxis(new GanttSpan(thursday, thursday.AddDays(14)), GanttScale.Week, Us);

        Assert.Equal(D(2026, 10, 12), ru.UnitOf(thursday).Start); // Monday
        Assert.Equal(D(2026, 10, 11), us.UnitOf(thursday).Start); // Sunday
        Assert.Equal(2, ru.Units, 6);
        Assert.Equal("12.10.2026", ru.Ticks()[0].Label);
        Assert.Equal(4 / 7d, ru.Ticks()[0].Width, 6);           // the first week is clipped to Thu-Sun
    }

    [Fact]
    public void Captions_use_the_gregorian_calendar_of_the_axis_units()
    {
        var fa = new CultureInfo("fa-IR");
        var months = new GanttAxis(new GanttSpan(D(2026, 10, 1), D(2026, 11, 1)), GanttScale.Month, fa);
        Assert.Equal("اکتبر", months.Ticks()[0].Label); // Gregorian October, not Mehr
        Assert.Equal("2026", months.MajorTicks()[0].Label);

        var days = new GanttAxis(new GanttSpan(D(2026, 10, 13), D(2026, 10, 15)), GanttScale.Day, Us);
        Assert.Equal(["13", "14"], days.Ticks().Select(t => t.Label));
        Assert.Equal(["Tue", "Wed"], days.Ticks().Select(t => t.Caption));

        var weeks = new GanttAxis(new GanttSpan(D(2026, 10, 15), D(2026, 10, 29)), GanttScale.Week, Us);
        Assert.Equal("10/11/2026", weeks.Ticks()[0].Label);
    }

    [Fact]
    public void Hour_quarter_and_year_scales()
    {
        var hours = new GanttAxis(new GanttSpan(D(2026, 10, 12, 9), D(2026, 10, 12, 18)), GanttScale.Hour, Ru);
        Assert.Equal(9, hours.Units);
        Assert.Equal("09:00", hours.Ticks()[0].Label);
        Assert.Single(hours.MajorTicks());

        var quarters = new GanttAxis(new GanttSpan(D(2026, 1, 1), D(2027, 1, 1)), GanttScale.Quarter, Ru);
        Assert.Equal(["Q1", "Q2", "Q3", "Q4"], quarters.Ticks().Select(t => t.Label));

        var years = new GanttAxis(new GanttSpan(D(2025, 1, 1), D(2027, 1, 1)), GanttScale.Year, Ru);
        Assert.Equal(2, years.Units);
        Assert.Empty(years.MajorTicks());
    }

    [Fact]
    public void Snap_goes_to_the_nearest_boundary()
    {
        var axis = new GanttAxis(GanttSpan.FromDays(new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 31)), GanttScale.Day, Ru);

        Assert.Equal(D(2026, 10, 5), axis.Snap(D(2026, 10, 5, 11)));
        Assert.Equal(D(2026, 10, 6), axis.Snap(D(2026, 10, 5, 13)));
    }

    [Fact]
    public void Spans_for_days_and_instants()
    {
        Assert.Equal(new GanttSpan(D(2026, 10, 12), D(2026, 10, 15)), GanttSpan.FromDays(new DateOnly(2026, 10, 12), new DateOnly(2026, 10, 14)));
        var moscow = TimeZoneInfo.CreateCustomTimeZone("msk", TimeSpan.FromHours(3), "msk", "msk");
        var span = GanttSpan.FromOffsets(new DateTimeOffset(2026, 10, 12, 6, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 12, 7, 0, 0, TimeSpan.Zero), moscow);
        Assert.Equal(D(2026, 10, 12, 9), span.Start);
        Assert.False(GanttSpan.Day(new DateOnly(2026, 10, 12)).Overlaps(GanttSpan.Day(new DateOnly(2026, 10, 13))));
    }
}

public sealed class GanttLaneTests
{
    private static GanttSpan S(int from, int to) => new(new DateTime(2026, 1, from), new DateTime(2026, 1, to));

    [Fact]
    public void Identical_spans_take_separate_lanes()
    {
        var layout = GanttLanes.Assign([("a", S(1, 3)), ("b", S(1, 3)), ("c", S(1, 3))]);

        Assert.Equal(3, layout.Count);
        Assert.Equal([0, 1, 2], new[] { "a", "b", "c" }.Select(k => layout.Lanes[k]));
    }

    [Fact]
    public void Nested_spans_go_below_the_longer_one()
    {
        var layout = GanttLanes.Assign([("inner", S(2, 3)), ("outer", S(1, 10))]);

        Assert.Equal(0, layout.Lanes["outer"]);
        Assert.Equal(1, layout.Lanes["inner"]);
    }

    [Fact]
    public void Chains_of_partial_overlaps_reuse_freed_lanes()
    {
        var layout = GanttLanes.Assign([("a", S(1, 4)), ("b", S(3, 6)), ("c", S(5, 8)), ("d", S(4, 5))]);

        Assert.Equal(2, layout.Count);
        Assert.Equal(0, layout.Lanes["a"]);
        Assert.Equal(1, layout.Lanes["b"]);
        Assert.Equal(0, layout.Lanes["d"]); // starts when a ends
        Assert.Equal(0, layout.Lanes["c"]); // starts when d ends
    }

    [Fact]
    public void Zero_length_points_at_the_same_moment_do_not_collide()
    {
        var layout = GanttLanes.Assign([("m1", S(5, 5)), ("m2", S(5, 5)), ("bar", S(1, 5))]);

        Assert.Equal(0, layout.Lanes["bar"]);
        Assert.NotEqual(layout.Lanes["m1"], layout.Lanes["m2"]);
    }

    [Fact]
    public void Empty_input_still_has_one_lane()
    {
        Assert.Equal(1, GanttLanes.Assign(Array.Empty<(string, GanttSpan)>()).Count);
    }

    [Fact]
    public void Rollup_spans_and_weights_progress_by_duration()
    {
        Assert.Equal(S(1, 10), GanttRollup.Span([S(3, 4), S(1, 2), S(5, 10)]));
        Assert.Null(GanttRollup.Span([]));
        Assert.Equal(0.25, GanttRollup.Progress([(S(1, 4), 0.0), (S(4, 5), 1.0)]), 6); // 3 days at 0, 1 day at 100%
        Assert.Equal(0.5, GanttRollup.Progress([(S(1, 1), 0.0), (S(1, 1), 1.0)]));
    }
}

public sealed class GanttGraphTests
{
    private static GanttSpan S(int from, int to) => new(new DateTime(2026, 1, from), new DateTime(2026, 1, to));

    [Theory]
    [InlineData(GanttLinkType.FinishToStart, 5, 7)]   // b must start when a ends (day 5)
    [InlineData(GanttLinkType.StartToStart, 3, 5)]    // b must start when a starts (day 3)
    [InlineData(GanttLinkType.FinishToFinish, 3, 5)]  // b must end when a ends (day 5)
    [InlineData(GanttLinkType.StartToFinish, 2, 4)]   // b is already fine: it ends after a starts... pushed only if broken
    public void Cascade_pushes_by_each_link_type(GanttLinkType type, int expectedStart, int expectedEnd)
    {
        var spans = new Dictionary<string, GanttSpan> { ["a"] = S(1, 3), ["b"] = S(2, 4) };

        var changes = GanttGraph.Cascade(spans, [new GanttLink("a", "b", type)], "a", S(3, 5));

        Assert.Equal(S(3, 5), changes["a"]);
        var b = changes.TryGetValue("b", out var moved) ? moved : spans["b"];
        Assert.Equal(S(expectedStart, expectedEnd), b);
    }

    [Fact]
    public void Start_to_finish_pushes_when_broken()
    {
        var spans = new Dictionary<string, GanttSpan> { ["a"] = S(1, 3), ["b"] = S(1, 2) };

        var changes = GanttGraph.Cascade(spans, [new GanttLink("a", "b", GanttLinkType.StartToFinish)], "a", S(4, 6));

        Assert.Equal(S(3, 4), changes["b"]); // b must finish no earlier than a starts (day 4)
    }

    [Fact]
    public void Cascade_is_transitive_respects_lag_and_never_pulls_back()
    {
        var spans = new Dictionary<string, GanttSpan> { ["a"] = S(1, 3), ["b"] = S(3, 5), ["c"] = S(10, 12) };
        var links = new[] { new GanttLink("a", "b"), new GanttLink("b", "c", Lag: TimeSpan.FromDays(1)) };

        var later = GanttGraph.Cascade(spans, links, "a", S(2, 4));
        Assert.Equal(S(4, 6), later["b"]);
        Assert.False(later.ContainsKey("c")); // c already starts after b ends plus one day

        var earlier = GanttGraph.Cascade(spans, links, "a", S(1, 2));
        Assert.Equal(["a"], earlier.Keys); // successors are never pulled earlier

        var none = GanttGraph.Cascade(spans, links, "a", S(5, 7), GanttScheduleMode.None);
        Assert.Equal(["a"], none.Keys);
    }

    [Fact]
    public void Cascade_terminates_on_a_cycle()
    {
        var spans = new Dictionary<string, GanttSpan> { ["a"] = S(1, 2), ["b"] = S(2, 3) };

        var changes = GanttGraph.Cascade(spans, [new GanttLink("a", "b"), new GanttLink("b", "a")], "a", S(2, 3));

        Assert.True(changes.Count >= 1);
    }

    [Fact]
    public void Cycles_self_links_and_duplicates_are_refused_without_throwing()
    {
        var links = new[] { new GanttLink("a", "b"), new GanttLink("b", "c") };

        var cycle = GanttGraph.Validate(links, new GanttLink("c", "a"));
        Assert.Equal(GanttValidationCode.Cycle, cycle.Code);
        Assert.Equal(["c", "a", "b", "c"], cycle.Path);
        Assert.Equal("\u0421\u0432\u044F\u0437\u044C \u0441\u043E\u0437\u0434\u0430\u0435\u0442 \u0446\u0438\u043A\u043B: c \u2192 a \u2192 b \u2192 c.", new GanttTexts().Describe(cycle, culture: new System.Globalization.CultureInfo("ru-RU")));

        Assert.Equal(GanttValidationCode.SelfLink, GanttGraph.Validate(links, new GanttLink("a", "a")).Code);
        Assert.Equal(GanttValidationCode.Duplicate, GanttGraph.Validate(links, new GanttLink("a", "b")).Code);
        Assert.True(GanttGraph.Validate(links, new GanttLink("a", "c")).IsValid);
    }

    [Fact]
    public void Critical_path_matches_a_hand_computed_plan()
    {
        // Plan (days):  A 1-4, B 4-6, C 4-5, D 6-9.  A->B->D and A->C->D.
        // Project end = 9. D: LF 9 (crit). B: LF = LS(D)=6 = its end (crit). C: LF 6 > end 5 (slack 1).
        // A: LF = min(LS(B)=4, LS(C)=5) = 4 = its end (crit). E 1-2 has no links: LF 9, slack.
        var spans = new Dictionary<string, GanttSpan>
        {
            ["A"] = S(1, 4), ["B"] = S(4, 6), ["C"] = S(4, 5), ["D"] = S(6, 9), ["E"] = S(1, 2)
        };
        var links = new[] { new GanttLink("A", "B"), new GanttLink("A", "C"), new GanttLink("B", "D"), new GanttLink("C", "D") };

        Assert.Equal(["A", "B", "D"], GanttGraph.CriticalPath(spans, links).Order());
        Assert.Empty(GanttGraph.CriticalPath(spans, [new GanttLink("A", "B"), new GanttLink("B", "A")]));
    }

    [Fact]
    public void Critical_path_with_start_to_start_and_lag()
    {
        // A 1-5, B 3-6 with SS lag 2 days: B may start no later than ... B is last (ends at 6 = project end) -> critical.
        // A: LF from SS = LS(B) - lag + d(A) = 3 - 2 + 4 = day 5 = A's end -> critical.
        var spans = new Dictionary<string, GanttSpan> { ["A"] = S(1, 5), ["B"] = S(3, 6) };

        var critical = GanttGraph.CriticalPath(spans, [new GanttLink("A", "B", GanttLinkType.StartToStart, TimeSpan.FromDays(2))]);

        Assert.Equal(["A", "B"], critical.Order());
    }
}

public sealed class GanttRoutingTests
{
    [Fact]
    public void Finish_to_start_with_room_goes_straight()
    {
        var path = GanttRouting.Path(new GanttBox(0, 0, 100, 20), new GanttBox(150, 40, 50, 20), GanttLinkType.FinishToStart, 8);

        Assert.Equal("M 100 10 H 108 V 50 H 150", path);
    }

    [Fact]
    public void Finish_to_start_backwards_detours_between_rows()
    {
        var path = GanttRouting.Path(new GanttBox(0, 0, 100, 20), new GanttBox(40, 40, 50, 20), GanttLinkType.FinishToStart, 8);

        Assert.Equal("M 100 10 H 108 V 30 H 32 V 50 H 40", path);
    }

    [Fact]
    public void Other_link_types_use_their_sides()
    {
        Assert.Equal("M 0 10 H -8 V 50 H 40", GanttRouting.Path(new GanttBox(0, 0, 100, 20), new GanttBox(40, 40, 50, 20), GanttLinkType.StartToStart, 8));
        Assert.Equal("M 100 10 H 108 V 50 H 90", GanttRouting.Path(new GanttBox(0, 0, 100, 20), new GanttBox(40, 40, 50, 20), GanttLinkType.FinishToFinish, 8));
        Assert.Equal("M 0.5 10 H -7.5 V 50 H -30", GanttRouting.Path(new GanttBox(0.5, 0, 100, 20), new GanttBox(-80, 40, 50, 20), GanttLinkType.StartToFinish, 8));
    }
}
