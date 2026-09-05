using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareDateRangePicker Calendar mode: click a start day then an end day to select a range (swapping
// when the second click precedes the first). Fields mode (default) renders the two-input layout.
public class FlareDateRangeCalendarTests : FlareTestContext
{
    [Fact]
    public void Default_IsFieldsMode_NoInlineCalendar()
    {
        var cut = Render<FlareDateRangePicker>();
        Assert.Empty(cut.FindAll($".{Css.Classes.Daterangepicker.Calendar}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Daterangepicker.Fields}"));
    }

    [Fact]
    public async Task Calendar_TwoClicks_SelectOrderedRange()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(c => c.Mode, DateRangePickerMode.Calendar));
        await cut.InvokeAsync(() => cut.FindAll($".{Css.Classes.Picker.Day}")[10].Click());
        Assert.NotNull(cut.Instance.StartDate);
        Assert.Null(cut.Instance.EndDate);                       // first click sets only the start

        await cut.InvokeAsync(() => cut.FindAll($".{Css.Classes.Picker.Day}")[24].Click());
        Assert.NotNull(cut.Instance.EndDate);
        Assert.True(cut.Instance.StartDate <= cut.Instance.EndDate);
    }

    [Fact]
    public async Task Calendar_SecondClickEarlier_Swaps()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(c => c.Mode, DateRangePickerMode.Calendar));
        await cut.InvokeAsync(() => cut.FindAll($".{Css.Classes.Picker.Day}")[24].Click());   // later day first
        await cut.InvokeAsync(() => cut.FindAll($".{Css.Classes.Picker.Day}")[10].Click());   // earlier second
        Assert.True(cut.Instance.StartDate <= cut.Instance.EndDate);                       // swapped into order
    }

    [Fact]
    public void DefaultPresets_ArePublic_AndCombineWithCustom()
    {
        Assert.NotEmpty(FlareDateRangePicker.DefaultPresets);
        var combined = new List<DateRangePreset>(FlareDateRangePicker.DefaultPresets)
        {
            new("Sprint", t => (t.AddDays(-13), t)),
        };
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(c => c.ShowPresets, true)
            .Add(c => c.Presets, combined));
        // every built-in preset plus the custom one renders a chip
        Assert.Equal(combined.Count, cut.FindAll($".{Css.Classes.Daterangepicker.Preset}").Count);
    }
}
