namespace Flare.Components.Tests;

/// <summary>
/// TASK-173: a field locked (Disabled or ReadOnly) after its popup opened takes no value from the popup or from
/// ClearAsync, as the date picker already does (TASK-162); the buttons that would do nothing are disabled.
/// </summary>
public class LockedPopupCommitTests : FlareTestContext
{
    private static readonly DateTimeOffset Noon = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Time_OkAfterLock_IsDisabled_AndCommitsNothing(bool disabled, bool readOnly)
    {
        var published = false;
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown)
            .Add(x => x.Value, new TimeOnly(10, 0)).Add(x => x.ValueChanged, (TimeOnly? _) => published = true));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.TimePicker.Col}")[0].QuerySelectorAll("button").First(b => b.TextContent.Trim() == "12").Click();
        cut.Render(p => p.Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));

        var ok = cut.FindAll($".{Css.Classes.TimePicker.Actions} button").Last();
        Assert.True(ok.HasAttribute("disabled"));
        ok.Click();
        Assert.False(published);
    }

    [Fact]
    public void Time_ListPickAfterLock_CommitsNothing()
    {
        var published = false;
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.List).Add(x => x.MinuteStep, 30)
            .Add(x => x.Use24Hour, true).Add(x => x.ValueChanged, (TimeOnly? _) => published = true));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Render(p => p.Add(x => x.ReadOnly, true));
        cut.FindAll("[role=option]")[3].Click();
        Assert.False(published);
    }

    [Fact]
    public async Task Time_ClearAsync_OnALockedField_DoesNothing()
    {
        var published = false;
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, new TimeOnly(10, 0)).Add(x => x.ReadOnly, true)
            .Add(x => x.ValueChanged, (TimeOnly? _) => published = true));
        await cut.InvokeAsync(() => cut.Instance.ClearAsync());
        Assert.False(published);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void DateTime_OkAndClearAfterLock_AreDisabled_AndCommitNothing(bool disabled, bool readOnly)
    {
        var published = false;
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Panels).Add(x => x.Value, Noon)
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => published = true));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll("button[role=gridcell]").First(b => b.TextContent.Trim() == "20").Click();
        cut.Render(p => p.Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));

        var buttons = cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button");
        Assert.All(buttons, b => Assert.True(b.HasAttribute("disabled")));
        buttons.Last().Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").First().Click();
        Assert.False(published);
    }

    [Fact]
    public void DateTime_DayPickAfterLock_DoesNotMoveTheDraft()
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Panels).Add(x => x.Value, Noon));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Render(p => p.Add(x => x.ReadOnly, true));
        cut.FindAll("button[role=gridcell]").First(b => b.TextContent.Trim() == "20").Click();
        var selected = cut.FindAll("button[role=gridcell][aria-selected=true]").Single();
        Assert.Equal("15", selected.TextContent.Trim());
    }

    [Fact]
    public void DateTime_UnlockedField_StillCommits()
    {
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Panels).Add(x => x.Value, Noon)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll("button[role=gridcell]").First(b => b.TextContent.Trim() == "20").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();
        Assert.Equal(Noon.AddDays(5), committed);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Date_FooterButtons_AreDisabledInALockedField(bool disabled, bool readOnly)
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Value, new DateOnly(2026, 1, 15))
            .Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));
        Assert.All(cut.FindAll($".{Css.Classes.DatePicker.Footer} button"), b => Assert.True(b.HasAttribute("disabled")));
    }
}
