using Bunit;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-131: the List popup is a combobox over a list of times (the Fluent UI 2 time picker), and the column
/// popup announces its active cell through aria-activedescendant.
/// </summary>
public class TimePickerListTests : FlareTestContext
{
    private IRenderedComponent<FlareTimePicker> RenderList(TimeOnly? value = null, TimeOnly? min = null, TimeOnly? max = null,
        bool use24Hour = true, Action<TimeOnly?>? changed = null)
    {
        return Render<FlareTimePicker>(p =>
        {
            p.Add(x => x.PopupVariant, TimePickerVariant.List)
             .Add(x => x.MinuteStep, 30)
             .Add(x => x.Use24Hour, use24Hour)
             .Add(x => x.Value, value)
             .Add(x => x.Min, min)
             .Add(x => x.Max, max);
            if (changed is not null) p.Add(x => x.ValueChanged, (TimeOnly? v) => changed(v));
        });
    }

    private static AngleSharp.Dom.IElement Input(IRenderedComponent<FlareTimePicker> cut) => cut.Find($".{Css.Classes.Input.Control}");

    private static IReadOnlyList<AngleSharp.Dom.IElement> Options(IRenderedComponent<FlareTimePicker> cut) =>
        cut.FindAll("[role=option]");

    [Fact]
    public void Field_IsACombobox_ThatControlsTheList()
    {
        var cut = RenderList();
        var input = Input(cut);
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("listbox", input.GetAttribute("aria-haspopup"));
        Assert.Equal("false", input.GetAttribute("aria-expanded"));
        Assert.Null(input.GetAttribute("aria-activedescendant"));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        input = Input(cut);
        Assert.Equal("true", input.GetAttribute("aria-expanded"));
        var listbox = cut.Find("[role=listbox]");
        Assert.Equal(listbox.Id, input.GetAttribute("aria-controls"));
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void List_RunsFromMinToMax_EveryStep()
    {
        var cut = RenderList(min: new TimeOnly(9, 10), max: new TimeOnly(11, 0));
        Input(cut).KeyDown("ArrowDown");

        Assert.Equal(new[] { "09:30", "10:00", "10:30", "11:00" }, Options(cut).Select(o => o.TextContent.Trim()));
    }

    [Fact]
    public void Arrows_MoveTheActiveOption_AndEnterPicksIt()
    {
        TimeOnly? committed = null;
        var cut = RenderList(value: new TimeOnly(10, 0), changed: v => committed = v);

        Input(cut).KeyDown("ArrowDown");   // opens on the picked time
        var options = Options(cut);
        Assert.Equal(options[20].Id, Input(cut).GetAttribute("aria-activedescendant"));
        Assert.Equal("true", options[20].GetAttribute("aria-selected"));

        Input(cut).KeyDown("ArrowDown");
        Input(cut).KeyDown("ArrowDown");
        Input(cut).KeyDown("ArrowUp");
        Assert.Equal(Options(cut)[21].Id, Input(cut).GetAttribute("aria-activedescendant"));

        Input(cut).KeyDown("Enter");
        Assert.Equal(new TimeOnly(10, 30), committed);
        Assert.Empty(Options(cut));
        Assert.Equal("false", Input(cut).GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ActiveOption_StartsAtTheFirstTimeAfterAnOffGridValue()
    {
        var cut = RenderList(value: new TimeOnly(10, 7));
        Input(cut).KeyDown("ArrowDown");

        Assert.Equal(Options(cut)[21].Id, Input(cut).GetAttribute("aria-activedescendant"));   // 10:30
    }

    [Fact]
    public void Click_PicksAnOption()
    {
        TimeOnly? committed = null;
        var cut = RenderList(changed: v => committed = v);
        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Options(cut)[3].Click();

        Assert.Equal(new TimeOnly(1, 30), committed);
        Assert.Empty(Options(cut));
    }

    [Fact]
    public void Escape_ClosesTheList_WithoutPicking()
    {
        TimeOnly? committed = null;
        var cut = RenderList(value: new TimeOnly(8, 0), changed: v => committed = v);
        Input(cut).KeyDown("ArrowDown");
        Input(cut).KeyDown("ArrowDown");

        Input(cut).KeyDown("Escape");

        Assert.Empty(Options(cut));
        Assert.Null(committed);
    }

    [Fact]
    public void TwelveHourList_UsesTheCulturePeriod()
    {
        var cut = RenderList(use24Hour: false, min: new TimeOnly(12, 0), max: new TimeOnly(13, 0));
        Input(cut).KeyDown("ArrowDown");

        var expected = new[] { new TimeOnly(12, 0), new TimeOnly(12, 30), new TimeOnly(13, 0) }
            .Select(t => t.ToString("h:mm tt", System.Globalization.CultureInfo.CurrentUICulture));
        Assert.Equal(expected, Options(cut).Select(o => o.TextContent.Trim()));
    }

    [Fact]
    public void OtherVariants_KeepTheirDialogAndPlainField()
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown));
        Assert.Null(Input(cut).GetAttribute("role"));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.Single(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Columns_AnnounceTheActiveCell()
    {
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.PopupVariant, TimePickerVariant.Dropdown)
            .Add(x => x.Value, new TimeOnly(9, 15)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var columns = cut.Find($".{Css.Classes.TimePicker.Columns}");
        Assert.Equal("group", columns.GetAttribute("role"));

        var hourCell = cut.Find($"[role=listbox][aria-label] .{Css.Classes.TimePicker.CellActive}");
        Assert.Equal(hourCell.Id, columns.GetAttribute("aria-activedescendant"));
        Assert.Equal("09", hourCell.TextContent.Trim());

        columns.KeyDown("ArrowRight");
        columns = cut.Find($".{Css.Classes.TimePicker.Columns}");
        var active = cut.Find($"#{columns.GetAttribute("aria-activedescendant")}");
        Assert.Equal("15", active.TextContent.Trim());
        Assert.Contains(Css.Classes.TimePicker.CellActive, active.ClassList);
        Assert.Single(cut.FindAll($".{Css.Classes.TimePicker.CellActive}"));
    }
}
