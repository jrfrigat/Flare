using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// Date/Time cross-framework audit follow-ups (items 1-8). Render/attribute + imperative-instance level.
public class DateTimePickerAuditTests : FlareTestContext
{
    // Item 2: OpenTo=Year opens straight to the year grid (verified inline - no JS positioning needed).
    [Fact]
    public void DatePicker_OpenTo_Year_Inline_ShowsYearGrid()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(c => c.Inline, true)
            .Add(c => c.OpenTo, PickerOpenTo.Year));

        Assert.NotEmpty(cut.FindAll(".flare-datepicker__year-grid"));
        Assert.Equal(12, cut.FindAll(".flare-datepicker__year-btn").Count);
        Assert.Empty(cut.FindAll(".flare-picker__day")); // the day grid is not shown in year view
    }

    // Item 7: Inline renders the panel in normal flow (no scrim, calendar always visible).
    [Fact]
    public void DatePicker_Inline_RendersPanelInFlow_NoScrim()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(c => c.Inline, true));

        Assert.NotEmpty(cut.FindAll(".flare-picker__panel--inline"));
        Assert.Empty(cut.FindAll(".flare-picker__scrim"));
        Assert.NotEmpty(cut.FindAll(".flare-picker__day"));
    }

    // Item 5: ShowWeekNumbers renders a leading week-number column (1 header + 6 rows).
    [Fact]
    public void DatePicker_ShowWeekNumbers_Inline_RendersWeekColumn()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(c => c.Inline, true)
            .Add(c => c.ShowWeekNumbers, true));

        Assert.Equal(7, cut.FindAll(".flare-picker__weeknum").Count);
    }

    // Item 5: FirstDayOfWeek override changes which weekday the grid starts on.
    [Fact]
    public void MonthGrid_FirstDayOfWeek_Override_StartsGridOnThatDay()
    {
        var cut = Render<FlareMonthGrid>(p => p
            .Add(c => c.ViewYear, 2026)
            .Add(c => c.ViewMonth, 7)
            .Add(c => c.FirstDayOfWeek, DayOfWeek.Monday));

        var expected = new DateOnly(2026, 7, 1);
        while (expected.DayOfWeek != DayOfWeek.Monday) expected = expected.AddDays(-1);

        Assert.Equal(expected.Day.ToString(), cut.FindAll(".flare-picker__day")[0].TextContent);
    }

    // Item 8: DayClassFunc adds a custom class to matching day cells.
    [Fact]
    public void DatePicker_DayClassFunc_Inline_AddsCustomClass()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(c => c.Inline, true)
            .Add(c => c.Value, new DateOnly(2026, 7, 15))
            .Add(c => c.DayClassFunc, d => d.Day == 15 ? "holiday" : ""));

        Assert.NotEmpty(cut.FindAll(".flare-picker__day.holiday"));
    }

    // Item 1 + 4: imperative Open/Close drive the popup and fire Opened/Closed.
    [Fact]
    public async Task DatePicker_Imperative_OpenClose_FiresEvents()
    {
        var opened = 0;
        var closed = 0;
        var cut = Render<FlareDatePicker>(p => p
            .Add(c => c.Opened, EventCallback.Factory.Create(this, () => opened++))
            .Add(c => c.Closed, EventCallback.Factory.Create(this, () => closed++)));

        Assert.Empty(cut.FindAll(".flare-datepicker__panel"));

        await cut.InvokeAsync(() => cut.Instance.OpenAsync());
        Assert.NotEmpty(cut.FindAll(".flare-datepicker__panel"));
        Assert.Equal(1, opened);

        await cut.InvokeAsync(() => cut.Instance.CloseAsync());
        Assert.Empty(cut.FindAll(".flare-datepicker__panel"));
        Assert.Equal(1, closed);
    }

    // Item 3: AutoClose=false keeps the popup open after a day is selected.
    [Fact]
    public async Task DatePicker_AutoClose_False_KeepsOpenAfterSelect()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(c => c.AutoClose, false));
        await cut.InvokeAsync(() => cut.Instance.OpenAsync());

        cut.FindAll(".flare-picker__day")[15].Click();

        Assert.NotEmpty(cut.FindAll(".flare-datepicker__panel")); // still open
    }

    // Item 6: TimePicker ShowSeconds adds a third (seconds) column in the Dropdown variant.
    [Fact]
    public async Task TimePicker_ShowSeconds_Dropdown_RendersSecondsColumn()
    {
        var cut = Render<FlareTimePicker>(p => p
            .Add(c => c.PopupVariant, TimePickerVariant.Dropdown)
            .Add(c => c.ShowSeconds, true));

        await cut.InvokeAsync(() => cut.Instance.OpenAsync());

        Assert.Equal(3, cut.FindAll(".flare-timepicker__col").Count); // hours + minutes + seconds
    }

    // Item 1: the imperative API surface is present family-wide.
    [Fact]
    public void Pickers_Expose_Imperative_Api()
    {
        foreach (var t in new[] { typeof(FlareDatePicker), typeof(FlareTimePicker), typeof(FlareDateTimePicker) })
        {
            Assert.NotNull(t.GetMethod("OpenAsync"));
            Assert.NotNull(t.GetMethod("CloseAsync"));
            Assert.NotNull(t.GetMethod("ToggleAsync"));
            Assert.NotNull(t.GetMethod("FocusAsync"));
        }
    }
}
