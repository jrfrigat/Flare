using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareDateRangePickerTests : FlareTestContext
{
    [Fact]
    public void RendersRoot()
    {
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.StartDate, new DateOnly(2026, 1, 1))
            .Add(x => x.EndDate, new DateOnly(2026, 1, 31)));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Daterangepicker.Root}"));
    }

    [Fact]
    public void ShowPresets_RendersDefaultPresetChips()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.ShowPresets, true));
        // 7 default presets: Today, Yesterday, Last 7/30 days, This/Last month, This year
        Assert.Equal(7, cut.FindAll($".{Css.Classes.Daterangepicker.Preset}").Count);
    }

    [Fact]
    public void NoPresets_ByDefault()
    {
        var cut = Render<FlareDateRangePicker>();
        Assert.Empty(cut.FindAll($".{Css.Classes.Daterangepicker.Preset}"));
    }

    [Fact]
    public void ClickingTodayPreset_SetsBothDatesToToday()
    {
        var today = DateOnly.FromDateTime(TimeProvider.System.GetLocalNow().DateTime);
        DateOnly? start = null, end = null;
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.ShowPresets, true)
            .Add(x => x.StartDateChanged, d => start = d)
            .Add(x => x.EndDateChanged, d => end = d));

        cut.FindAll($".{Css.Classes.Daterangepicker.Preset}")[0].Click(); // Today
        Assert.Equal(today, start);
        Assert.Equal(today, end);
    }

    [Fact]
    public void CustomPresets_OverrideDefaults()
    {
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.ShowPresets, true)
            .Add(x => x.Presets, new List<DateRangePreset>
            {
                new("Q1", t => (new DateOnly(t.Year, 1, 1), new DateOnly(t.Year, 3, 31))),
            }));

        var chips = cut.FindAll($".{Css.Classes.Daterangepicker.Preset}");
        Assert.Single(chips);
        Assert.Equal("Q1", chips[0].TextContent);
    }
}
