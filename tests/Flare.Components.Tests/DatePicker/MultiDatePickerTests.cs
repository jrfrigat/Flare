using System.Globalization;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-183: the multi-date picker toggles days into a sorted list without repeats, honours MaxCount, Min/Max and
/// the locked states, lists or counts the dates in the field and takes a typed list, on the culture's calendar.
/// </summary>
public class MultiDatePickerTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");
    private static readonly DateOnly Oct1 = new(2026, 10, 1);

    private IRenderedComponent<FlareMultiDatePicker> RenderPicker(Action<IReadOnlyList<DateOnly>> changed,
        IReadOnlyList<DateOnly>? values = null, int? max = null, CultureInfo? culture = null, bool inline = true,
        DateOnly? maxDate = null) =>
        Render<FlareMultiDatePicker>(p => p.Add(x => x.Inline, inline).Add(x => x.Culture, culture ?? Ru)
            .Add(x => x.Values, values ?? [Oct1]).Add(x => x.MaxCount, max).Add(x => x.Max, maxDate)
            .Add(x => x.ValuesChanged, (IReadOnlyList<DateOnly> v) => changed(v)));

    private static AngleSharp.Dom.IElement Day(IRenderedComponent<FlareMultiDatePicker> cut, int day) =>
        cut.FindAll($"button[role=gridcell]:not(.{Css.Classes.Picker.DayOutside})").First(b => b.TextContent.Trim() == day.ToString());

    [Fact]
    public void Clicks_ToggleDays_IntoASortedList()
    {
        IReadOnlyList<DateOnly> list = [];
        var cut = RenderPicker(v => list = v, [new DateOnly(2026, 10, 9)]);
        Day(cut, 3).Click();
        Assert.Equal(new[] { new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 9) }, list);
        cut.Render(p => p.Add(x => x.Values, list));
        Day(cut, 9).Click();
        Assert.Equal(new[] { new DateOnly(2026, 10, 3) }, list);
    }

    [Fact]
    public void Grid_IsMultiSelectable_AndMarksEachSelectedDay()
    {
        var cut = RenderPicker(_ => { }, [new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 9)]);
        Assert.Equal("true", cut.Find("[role=grid]").GetAttribute("aria-multiselectable"));
        Assert.Equal(2, cut.FindAll("button[role=gridcell][aria-selected=true]").Count);
    }

    [Fact]
    public void MaxCount_DisablesTheOtherDays_ButLetsASelectedOneGo()
    {
        IReadOnlyList<DateOnly> list = [];
        var cut = RenderPicker(v => list = v, [new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 9)], max: 2);
        Assert.True(Day(cut, 5).HasAttribute("disabled"));
        Assert.False(Day(cut, 3).HasAttribute("disabled"));
        Day(cut, 3).Click();
        Assert.Equal(new[] { new DateOnly(2026, 10, 9) }, list);
    }

    [Fact]
    public void Field_ListsUpToThreeDates_ThenCountsThem()
    {
        var three = RenderPicker(_ => { }, [new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 9), new DateOnly(2026, 10, 12)]);
        Assert.Equal("03.10.2026; 09.10.2026; 12.10.2026", three.Find("input").GetAttribute("value"));
        var four = RenderPicker(_ => { }, [Oct1, new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 9), new DateOnly(2026, 10, 12)]);
        Assert.Equal(string.Format(Ru, FlareStrings.MultiDatePicker_Count, 4), four.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void TypedList_IsCommittedWhole_OrNotAtAll()
    {
        IReadOnlyList<DateOnly>? list = null;
        var cut = RenderPicker(v => list = v, maxDate: new DateOnly(2026, 12, 31));
        cut.Find("input").Change("12.10.2026; 05.10.2026;12.10.2026");
        Assert.Equal(new[] { new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 12) }, list);
        list = null;
        cut.Find("input").Change("12.10.2026; tomorrow");
        Assert.Null(list);
        cut.Find("input").Change("12.10.2027");
        Assert.Null(list);
    }

    [Fact]
    public void EnterOnAFocusedDay_Toggles()
    {
        IReadOnlyList<DateOnly> list = [];
        var cut = RenderPicker(v => list = v);
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cut.Find("button[role=gridcell][tabindex='0']").Click(); // Enter/Space activate the focused day natively
        Assert.Equal(new[] { Oct1, new DateOnly(2026, 10, 2) }, list);
    }

    [Fact]
    public void Popup_StaysOpenWhileToggling_AndDoneClosesIt()
    {
        var cut = RenderPicker(_ => { }, inline: false);
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Day(cut, 3).Click();
        Assert.NotEmpty(cut.FindAll("[role=dialog]"));
        cut.FindAll($".{Css.Classes.DatePicker.Footer} button").Last().Click();
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void ReadOnlyAndDisabled_ChangeNothing()
    {
        var published = false;
        var ro = Render<FlareMultiDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.ReadOnly, true).Add(x => x.Culture, Ru)
            .Add(x => x.Values, [Oct1]).Add(x => x.ValuesChanged, (IReadOnlyList<DateOnly> _) => published = true));
        Day(ro, 5).Click();
        Assert.False(published);
        Assert.False(ro.FindAll($".{Css.Classes.DatePicker.Header} button[aria-label]").Last().HasAttribute("disabled"));
        var off = Render<FlareMultiDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Disabled, true).Add(x => x.Culture, Ru).Add(x => x.Values, [Oct1]));
        Assert.All(off.FindAll("button[role=gridcell]"), b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void OutsideList_ShowsTheMonthOfItsNewDate()
    {
        var cut = RenderPicker(_ => { });
        cut.Render(p => p.Add(x => x.Values, [Oct1, new DateOnly(2027, 3, 5)]));
        Assert.Equal(new DateOnly(2027, 3, 1).ToString("MMMM yyyy", Ru), cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
    }

    [Fact]
    public void Persian_ShowsThePersianMonth()
    {
        var fa = CultureInfo.GetCultureInfo("fa-IR");
        var cut = RenderPicker(_ => { }, [new DateOnly(2026, 10, 3)], culture: fa);
        Assert.Equal(new DateOnly(2026, 10, 3).ToString("MMMM yyyy", fa), cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
        Assert.Equal("11", cut.Find("button[role=gridcell][aria-selected=true]").TextContent.Trim());
    }

    [Fact]
    public void For_ReachesTheEditContext()
    {
        var model = new Holder();
        var context = new EditContext(model);
        var changed = false;
        context.OnFieldChanged += (_, _) => changed = true;
        var cut = Render<CascadingValue<EditContext>>(p => p.Add(x => x.Value, context).AddChildContent<FlareMultiDatePicker>(c => c
            .Add(x => x.Inline, true).Add(x => x.Culture, Ru).Add(x => x.Values, model.Days)
            .Add(x => x.ValuesChanged, (IReadOnlyList<DateOnly> v) => model.Days = v).Add(x => x.For, () => model.Days)));
        cut.FindAll($"button[role=gridcell]:not(.{Css.Classes.Picker.DayOutside})")[4].Click();
        Assert.True(changed);
        Assert.Single(model.Days);
    }

    private sealed class Holder
    {
        public IReadOnlyList<DateOnly> Days { get; set; } = [];
    }
}
