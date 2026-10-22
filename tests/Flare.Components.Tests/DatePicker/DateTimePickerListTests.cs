using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-662: the combined picker's List time variant is a real list, not the number fields with a different
/// name. Picking a row edits the draft; OK commits it, like every other pane of this popup.
/// </summary>
public class DateTimePickerListTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");
    private static readonly TimeSpan Plus5 = TimeSpan.FromHours(5);
    private static readonly DateTimeOffset Initial = new(2026, 10, 15, 10, 30, 0, Plus5);

    private IRenderedComponent<FlareDateTimePicker> Picker(
        Action<ComponentParameterCollectionBuilder<FlareDateTimePicker>> extra,
        Action<DateTimeOffset?> changed) =>
        Render<FlareDateTimePicker>(p =>
        {
            p.Add(x => x.Culture, Ru).Add(x => x.Value, Initial).Add(x => x.ValueChanged, changed)
             .Add(x => x.Mode, DateTimeVariant.Panels)
             .Add(x => x.TimeVariant, TimePickerVariant.List)
             .Add(x => x.MinuteStep, 30);
            extra(p);
        });

    private static AngleSharp.Dom.IElement List(IRenderedComponent<FlareDateTimePicker> cut)
    {
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut.Find($".{Css.Classes.DateTimePicker.TimeList}");
    }

    private static void Ok(IRenderedComponent<FlareDateTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

    [Fact]
    public void List_ShowsOneRowPerStepAndMarksTheDraftTime()
    {
        var cut = Picker(_ => { }, _ => { });

        var list = List(cut);
        var rows = cut.FindAll("[role=option]");

        Assert.Equal("listbox", list.GetAttribute("role"));
        Assert.Equal("0", list.GetAttribute("tabindex"));
        Assert.Equal(48, rows.Count);
        Assert.Equal("true", rows[21].GetAttribute("aria-selected")); // 10:30 with a 30-minute step
        Assert.Equal($"{list.GetAttribute("id")}-opt-21", list.GetAttribute("aria-activedescendant"));
    }

    [Fact]
    public void ClickingARow_EditsTheDraftAndOkCommitsIt()
    {
        DateTimeOffset? result = null;
        var cut = Picker(_ => { }, v => result = v);

        List(cut);
        cut.FindAll("[role=option]")[19].Click(); // 09:30
        Ok(cut);

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 9, 30, 0, Plus5), result);
    }

    [Fact]
    public void ArrowKeysMoveTheHighlightAndEnterTakesIt()
    {
        DateTimeOffset? result = null;
        var cut = Picker(_ => { }, v => result = v);

        var list = List(cut);
        list.KeyDown(new KeyboardEventArgs { Key = "Home" });
        list = cut.Find($".{Css.Classes.DateTimePicker.TimeList}");
        Assert.Equal($"{list.GetAttribute("id")}-opt-0", list.GetAttribute("aria-activedescendant"));

        list.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        list = cut.Find($".{Css.Classes.DateTimePicker.TimeList}");
        list.KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Ok(cut);

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 0, 30, 0, Plus5), result);
    }
}
