using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-146: the range picker takes Culture, ReadOnly and per-end form fields (StartFor/EndFor) with the
/// meaning they have on the single date picker.
/// </summary>
public class DateRangePickerFormTests : FlareTestContext
{
    private sealed class Trip
    {
        [Required(ErrorMessage = "Pick a start")] public DateOnly? From { get; set; }
        [Required(ErrorMessage = "Pick an end")] public DateOnly? To { get; set; }
    }

    private IRenderedComponent<EditForm> RenderForm(Trip model, DateRangePickerMode mode)
    {
        var context = new EditContext(model);
        return Render<EditForm>(p => p
            .Add(x => x.EditContext, context)
            .Add(x => x.ChildContent, (RenderFragment<EditContext>)(_ => b =>
            {
                b.OpenComponent<DataAnnotationsValidator>(0);
                b.CloseComponent();
                b.OpenComponent<FlareDateRangePicker>(1);
                b.AddAttribute(2, nameof(FlareDateRangePicker.Mode), mode);
                b.AddAttribute(3, nameof(FlareDateRangePicker.StartDate), model.From);
                b.AddAttribute(4, nameof(FlareDateRangePicker.StartDateChanged),
                    EventCallback.Factory.Create<DateOnly?>(this, v => model.From = v));
                b.AddAttribute(5, nameof(FlareDateRangePicker.EndDate), model.To);
                b.AddAttribute(6, nameof(FlareDateRangePicker.EndDateChanged),
                    EventCallback.Factory.Create<DateOnly?>(this, v => model.To = v));
                b.AddAttribute(7, nameof(FlareDateRangePicker.StartFor), (System.Linq.Expressions.Expression<Func<DateOnly?>>)(() => model.From));
                b.AddAttribute(8, nameof(FlareDateRangePicker.EndFor), (System.Linq.Expressions.Expression<Func<DateOnly?>>)(() => model.To));
                b.CloseComponent();
            })));
    }

    [Fact]
    public void Calendar_ReportsBothEndsAndShowsTheUnfinishedEnd()
    {
        var model = new Trip();
        var cut = RenderForm(model, DateRangePickerMode.Calendar);

        cut.FindAll($".{Css.Classes.Picker.Day}:not([disabled])").First(d => d.TextContent.Trim() == "10").Click();

        Assert.NotNull(model.From);
        Assert.Null(model.To);
        Assert.Contains("Pick an end", cut.Find($".{Css.Classes.Input.HelperError}").TextContent);
    }

    [Fact]
    public void Fields_ShowTheMessageOnTheInnerField()
    {
        var model = new Trip();
        var cut = RenderForm(model, DateRangePickerMode.Fields);

        cut.Find("form").Submit();

        Assert.Contains(cut.FindAll($".{Css.Classes.Input.HelperError}"), e => e.TextContent.Contains("Pick a start"));
        Assert.Contains(cut.FindAll($".{Css.Classes.Input.HelperError}"), e => e.TextContent.Contains("Pick an end"));
    }

    [Fact]
    public void ReadOnly_IgnoresDayClicksAndPresets()
    {
        var calls = 0;
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.Mode, DateRangePickerMode.Calendar).Add(x => x.ReadOnly, true).Add(x => x.ShowPresets, true)
            .Add(x => x.StartDateChanged, (DateOnly? _) => calls++));

        cut.FindAll($".{Css.Classes.Picker.Day}").First(d => d.TextContent.Trim() == "10").Click();

        Assert.Equal(0, calls);
        Assert.All(cut.FindAll($".{Css.Classes.Daterangepicker.Preset}"), b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void ReadOnly_ReachesTheFields()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.ReadOnly, true));

        Assert.All(cut.FindAll("input"), i => Assert.True(i.HasAttribute("readonly")));
    }

    [Fact]
    public void Culture_NamesTheMonthAndReachesTheFields()
    {
        var de = new CultureInfo("de-DE");
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.Mode, DateRangePickerMode.Calendar).Add(x => x.Culture, de)
            .Add(x => x.StartDate, new DateOnly(2026, 3, 10)));
        Assert.Contains("März", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);

        var fields = Render<FlareDateRangePicker>(p => p.Add(x => x.Culture, de)
            .Add(x => x.StartDate, new DateOnly(2026, 3, 10)));
        Assert.Equal("10.03.2026", fields.FindAll("input")[0].GetAttribute("value"));
    }
}
