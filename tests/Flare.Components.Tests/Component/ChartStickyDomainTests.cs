namespace Flare.Components.Tests.Component;

/// <summary>
/// A chart fed live data re-derived its value axis from whatever dataset it currently held, so one new
/// point among twenty moved the other nineteen: the curve reshaped wholesale every few seconds and the
/// eye had nothing stable to read a trend against. `YMin`/`YMax` pin the window, but a live metric
/// whose range is not known in advance cannot use them, and `ScaleMode` chooses which SERIES the domain
/// is measured from rather than whether it is re-measured at all.
///
/// A sticky domain is the third answer: grow to fit, do not shrink back on your own. Asserted through
/// the rendered path, because the domain is only observable as where the points landed.
/// </summary>
public sealed class ChartStickyDomainTests : FlareTestContext
{
    private static ChartData Series(params double[] values) =>
        new([new ChartSeries("live", values)], values.Select((_, i) => i.ToString()).ToList());

    private static string LinePath(IRenderedComponent<FlareChart> cut) =>
        cut.Find("path.flare-chart__line").GetAttribute("d") ?? "";

    // The Y of each point, which is where the domain becomes observable: X is the category slot and
    // says nothing about the scale.
    private static IReadOnlyList<string> PointYs(IRenderedComponent<FlareChart> cut) =>
        System.Text.RegularExpressions.Regex
            .Matches(LinePath(cut), @"[ML]\s+[\d.-]+\s+([\d.-]+)")
            .Select(m => m.Groups[1].Value)
            .ToList();

    private IRenderedComponent<FlareChart> RenderLive(ChartData data, bool sticky) =>
        Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.StickyDomain, sticky)
            .Add(x => x.NiceScale, false)
            .Add(x => x.Data, data));

    // The defect, stated as a test: the same four values land somewhere else because a FIFTH arrived.
    [Fact]
    public void WithoutIt_ANewPointMovesTheOldOnes()
    {
        var cut = RenderLive(Series(10, 20, 30, 40), sticky: false);
        var before = LinePath(cut);

        cut.Render(p => p.Add(x => x.Data, Series(10, 20, 30, 40, 90)));

        Assert.DoesNotContain(before[..20], LinePath(cut), StringComparison.Ordinal);
    }

    // The defect and the fix, on the same data: the series maximum drops from 100 to 80 and nothing
    // else changes.
    //
    // A refitting chart re-derives the domain from the new data, so the top of the plot IS the new
    // maximum - the value that changed stays glued to the top and the value that did NOT change slides
    // up under it. That is the wrong way round, and it is why a live chart reads as noise: the points
    // that moved are the ones nobody touched.
    //
    // A sticky chart keeps the range it is holding, so the changed value moves down to where 80 belongs
    // and the untouched ones stay put.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheValueThatChangedIsTheOneThatMoves(bool sticky)
    {
        var cut = RenderLive(Series(0, 50, 100), sticky);
        var before = PointYs(cut);

        cut.Render(p => p.Add(x => x.Data, Series(0, 50, 80)));
        var after = PointYs(cut);

        // The floor is the floor either way.
        Assert.Equal(before[0], after[0]);

        if (sticky)
        {
            Assert.Equal(before[1], after[1]);      // untouched, and it stays
            Assert.NotEqual(before[2], after[2]);   // changed, and it moves
        }
        else
        {
            Assert.NotEqual(before[1], after[1]);   // untouched, and it moves anyway
            Assert.Equal(before[2], after[2]);      // changed, and it does not
        }
    }

    // Growth still happens - it must, or the new point would be drawn outside the plot.
    [Fact]
    public void WithIt_TheDomainStillGrowsToFitNewData()
    {
        var cut = RenderLive(Series(0, 50, 100), sticky: true);
        var before = LinePath(cut);

        cut.Render(p => p.Add(x => x.Data, Series(0, 50, 100, 400)));

        Assert.NotEqual(before, LinePath(cut));
    }

    // A domain that never shrinks is ruined for good by one spike, so it gives the space back once the
    // data has genuinely moved into a smaller range - less than half the range being held.
    // Whether a domain is being held is only visible by comparing the SAME data drawn with and without
    // a history: a series always spans its own min..max, so a chart that refits every time draws the
    // identical path for (0,500,1000) and (0,50,100). The question is where a dataset lands when
    // something wider came before it.
    [Fact]
    public void WithIt_DataThatFitsIsDrawnAgainstTheHeldRange()
    {
        var withHistory = RenderLive(Series(0, 1000), sticky: true);
        withHistory.Render(p => p.Add(x => x.Data, Series(0, 300, 600)));

        var fresh = RenderLive(Series(0, 300, 600), sticky: true);

        // 600 is more than half of the 1000 held, so the domain stays and the points sit low in it.
        Assert.NotEqual(LinePath(fresh), LinePath(withHistory));
    }

    // A domain that never shrinks is ruined for good by one spike, so it gives the space back once the
    // data has genuinely moved into a smaller range - less than half the range being held.
    [Fact]
    public void WithIt_AGenuinelySmallerRangeGetsTheSpaceBack()
    {
        var withHistory = RenderLive(Series(0, 1000), sticky: true);
        withHistory.Render(p => p.Add(x => x.Data, Series(0, 100, 200)));

        var fresh = RenderLive(Series(0, 100, 200), sticky: true);

        // A fifth of the held range: the reader would rather have the resolution back than the
        // stillness, so the plot is exactly what a chart with no history would have drawn.
        Assert.Equal(LinePath(fresh), LinePath(withHistory));
    }

    [Fact]
    public void ResetDomain_ForgetsWhatIsHeld()
    {
        var cut = RenderLive(Series(0, 1000), sticky: true);

        cut.Render(p => p.Add(x => x.Data, Series(0, 600)));
        var held = LinePath(cut);

        cut.InvokeAsync(() => cut.Instance.ResetDomain());
        cut.Render(p => p.Add(x => x.Data, Series(0, 600)));

        Assert.NotEqual(held, LinePath(cut));
    }

    // Pinning both ends is the caller's window verbatim, and a held range must not creep into it.
    [Fact]
    public void PinnedBoundsWin()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.StickyDomain, true)
            .Add(x => x.NiceScale, false)
            .Add(x => x.YMin, 0d)
            .Add(x => x.YMax, 100d)
            .Add(x => x.Data, Series(0, 50, 100)));
        var pinned = LinePath(cut);

        cut.Render(p => p.Add(x => x.Data, Series(0, 50, 100, 1000)));
        cut.Render(p => p.Add(x => x.Data, Series(0, 50, 100)));

        Assert.Equal(pinned, LinePath(cut));
    }
}
