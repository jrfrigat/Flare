using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class TimeColumnScrollTests : FlareTestContext
{
    private int Generation => (int)JSInterop.Invocations.Last(i => i.Identifier == "flareTimeColumns.sync").Arguments[2]!;

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 45)]
    [InlineData(2, 20)]
    public async Task ScrollEditsOnlyItsColumnAndDoesNotAutoClose(int column, int value)
    {
        TimeOnly? result = null;
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown)
            .Add(x => x.Value, new TimeOnly(9, 30, 10)).Add(x => x.ShowSeconds, true)
            .Add(x => x.MinuteStep, 15).Add(x => x.SecondStep, 10).Add(x => x.AutoClose, true)
            .Add(x => x.ValueChanged, (TimeOnly? v) => result = v));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        await cut.InvokeAsync(() => cut.Instance.SelectScrolledColumn(column, value, Generation));
        Assert.Single(cut.FindAll("[role=dialog]"));
        Assert.Null(result);
        var expected = new TimeOnly(column == 0 ? value : 9, column == 1 ? value : 30, column == 2 ? value : 10);
        var selected = cut.FindAll("[role=option][aria-selected=true]").Select(e => int.Parse(e.TextContent)).ToArray();
        Assert.Equal(new[] { expected.Hour, expected.Minute, expected.Second }, selected);
        cut.Find($".{Css.Classes.TimePicker.Columns}").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        cut.Find($".{Css.Classes.TimePicker.Columns}").KeyUp(new KeyboardEventArgs { Key = "Enter" });
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 8)]
    [InlineData(0, 18)]
    [InlineData(1, 31)]
    [InlineData(2, 11)]
    [InlineData(3, 0)]
    [InlineData(-1, 0)]
    public async Task RejectsUnavailableAndMalformedCells(int column, int value)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown)
            .Add(x => x.Value, new TimeOnly(9, 30, 10)).Add(x => x.ShowSeconds, true)
            .Add(x => x.MinuteStep, 15).Add(x => x.SecondStep, 10)
            .Add(x => x.Min, new TimeOnly(9, 15)).Add(x => x.Max, new TimeOnly(17, 0)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        await cut.InvokeAsync(() => cut.Instance.SelectScrolledColumn(column, value, Generation));
        Assert.Equal(new[] { "09", "30", "10" }, cut.FindAll("[role=option][aria-selected=true]").Select(e => e.TextContent));
    }

    [Fact]
    public async Task OldOpeningAndLockedFieldRejectPendingScroll()
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown)
            .Add(x => x.Value, new TimeOnly(9, 30)).Add(x => x.MinuteStep, 15));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var old = Generation;
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        await cut.InvokeAsync(() => cut.Instance.SelectScrolledColumn(1, 45, old));
        Assert.Equal("30", cut.FindAll("[role=option][aria-selected=true]")[1].TextContent);
        var current = Generation;
        cut.Render(p => p.Add(x => x.ReadOnly, true));
        await cut.InvokeAsync(() => cut.Instance.SelectScrolledColumn(1, 45, current));
        Assert.Equal("30", cut.FindAll("[role=option][aria-selected=true]")[1].TextContent);
        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "flareTimeColumns.release");
    }

    [Fact]
    public void SecondsStepAlsoControlsKeyboardAndAria()
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown)
            .Add(x => x.Value, new TimeOnly(9, 30, 10)).Add(x => x.ShowSeconds, true).Add(x => x.SecondStep, 10));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal(6, cut.Find("[data-time-column='2']").QuerySelectorAll("[role=option]").Length);
        var group = cut.Find($".{Css.Classes.TimePicker.Columns}");
        group.KeyDown("ArrowRight"); group.KeyDown("ArrowRight"); group.KeyDown("ArrowDown");
        Assert.EndsWith("-s-20", cut.Find($".{Css.Classes.TimePicker.Columns}").GetAttribute("aria-activedescendant"));
    }
}
