using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class RangeRenderTests : FlareTestContext
{
    private static readonly DateOnly Start = new(2026, 10, 10);
    private static readonly DateOnly End = new(2026, 10, 20);
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");

    private IRenderedComponent<FlareDateRangePicker> Range(DateRangePickerMode mode) =>
        Render<FlareDateRangePicker>(p => p.Add(x => x.Mode, mode).Add(x => x.StartDate, Start)
            .Add(x => x.EndDate, End).Add(x => x.Culture, Culture));

    [Theory]
    [InlineData(DateRangePickerMode.Fields)]
    [InlineData(DateRangePickerMode.Calendar)]
    public async Task UnchangedParameters_DoNotRenderTheRange(DateRangePickerMode mode)
    {
        var cut = Range(mode);
        void Push() => cut.Render(p => p.Add(x => x.Mode, mode).Add(x => x.StartDate, Start)
            .Add(x => x.EndDate, End).Add(x => x.Culture, Culture));
        Push();
        await cut.InvokeAsync(() =>
        {
            var count = cut.RenderCount;
            Push();
            Assert.Equal(count, cut.RenderCount);
        });
    }

    [Theory]
    [InlineData(DateRangePickerMode.Fields)]
    [InlineData(DateRangePickerMode.Calendar)]
    public async Task MutablePredicates_RefreshAnOpenCalendar(DateRangePickerMode mode)
    {
        var disabled = Start;
        var css = "before";
        Func<DateOnly, bool> predicate = day => day == disabled;
        Func<DateOnly, string> classes = _ => css;
        var cut = Range(mode);
        void Push() => cut.Render(p => p.Add(x => x.Mode, mode).Add(x => x.StartDate, Start)
            .Add(x => x.EndDate, End).Add(x => x.Culture, Culture)
            .Add(x => x.IsDateDisabled, predicate).Add(x => x.DayClassFunc, classes));
        Push();
        if (mode == DateRangePickerMode.Fields)
            await cut.InvokeAsync(cut.FindComponents<FlareDatePicker>()[0].Instance.OpenAsync);
        disabled = Start.AddDays(1);
        css = "after";
        Push();
        var cell = cut.Find($"[role=gridcell][aria-label='{disabled.ToString("D", Culture)}']");
        Assert.True(cell.HasAttribute("disabled"));
        Assert.Contains(css, cell.ClassList);
        Assert.False(cut.Find($"[role=gridcell][aria-label='{Start.ToString("D", Culture)}']").HasAttribute("disabled"));
    }

    [Theory]
    [InlineData(DateRangePickerMode.Fields)]
    [InlineData(DateRangePickerMode.Calendar)]
    public async Task MutableTemplate_IsNotSkipped(DateRangePickerMode mode)
    {
        var text = "before";
        RenderFragment<DateOnly> template = _ => builder => builder.AddContent(0, text);
        var cut = Range(mode);
        void Push() => cut.Render(p => p.Add(x => x.Mode, mode).Add(x => x.StartDate, Start)
            .Add(x => x.EndDate, End).Add(x => x.Culture, Culture).Add(x => x.DayTemplate, template));
        Push();
        if (mode == DateRangePickerMode.Fields)
            await cut.InvokeAsync(cut.FindComponents<FlareDatePicker>()[0].Instance.OpenAsync);
        text = "after";
        Push();
        Assert.All(cut.FindAll("[role=gridcell]"), cell => Assert.Equal(text, cell.TextContent));
    }

    [Fact]
    public void SkippedParameters_DoNotSwallowLocalNavigation()
    {
        var cut = Range(DateRangePickerMode.Calendar);
        cut.Render(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar).Add(x => x.StartDate, Start)
            .Add(x => x.EndDate, End).Add(x => x.Culture, Culture));
        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();
        Assert.Equal("November 2026", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }

    [Fact]
    public void SkippedParameters_StillReplaceCallbacks()
    {
        var first = 0;
        var second = 0;
        var cut = Range(DateRangePickerMode.Calendar);
        void Push(Action<DateOnly?> callback) => cut.Render(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar)
            .Add(x => x.StartDate, Start).Add(x => x.EndDate, End).Add(x => x.Culture, Culture)
            .Add(x => x.StartDateChanged, callback));
        Push(_ => first++);
        Push(_ => first++);
        Push(_ => second++);
        cut.Find($"[role=gridcell][aria-label='{Start.AddDays(1).ToString("D", Culture)}']").Click();
        Assert.Equal(0, first);
        Assert.Equal(1, second);
    }

    [Fact]
    public void SkippedParameters_StillAcceptNewValue()
    {
        var cut = Range(DateRangePickerMode.Calendar);
        cut.Render();
        cut.Render(p => p.Add(x => x.StartDate, new DateOnly(2026, 11, 10))
            .Add(x => x.EndDate, new DateOnly(2026, 11, 20)));
        Assert.NotNull(cut.Find("[role=gridcell][aria-label='Tuesday, November 10, 2026']"));
    }

    [Fact]
    public void UnchangedParentValue_ReplacesLocallySelectedRange()
    {
        var cut = Range(DateRangePickerMode.Calendar);
        void Push() => cut.Render(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar)
            .Add(x => x.StartDate, Start).Add(x => x.EndDate, End).Add(x => x.Culture, Culture));
        Push();
        cut.Find("[role=gridcell][aria-label='Sunday, October 11, 2026']").Click();
        cut.Find("[role=gridcell][aria-label='Sunday, October 18, 2026']").Click();
        Push();
        Assert.Contains(Css.Classes.Daterangepicker.DayStart,
            cut.Find("[role=gridcell][aria-label='Saturday, October 10, 2026']").ClassList);
        Assert.Contains(Css.Classes.Daterangepicker.DayEnd,
            cut.Find("[role=gridcell][aria-label='Tuesday, October 20, 2026']").ClassList);
    }

    [Fact]
    public void AmbientCultureChange_RefreshesTheCalendar()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = Culture;
            var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar)
                .Add(x => x.StartDate, Start).Add(x => x.EndDate, End));
            void Push() => cut.Render(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar)
                .Add(x => x.StartDate, Start).Add(x => x.EndDate, End));
            Push();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            Push();
            Assert.Equal("octobre 2026", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
        }
        finally { CultureInfo.CurrentUICulture = previous; }
    }
}
