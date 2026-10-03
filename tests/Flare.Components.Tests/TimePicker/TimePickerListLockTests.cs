namespace Flare.Components.Tests;

/// <summary>
/// TASK-168: a time list left open when the field becomes read-only still closes on Escape, while the arrows and
/// Enter do nothing in the locked field (as the date picker, TASK-162).
/// </summary>
public class TimePickerListLockTests : FlareTestContext
{
    private IRenderedComponent<FlareTimePicker> OpenThenLock(Action<TimeOnly?> changed)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.List)
            .Add(x => x.MinuteStep, 30).Add(x => x.Use24Hour, true).Add(x => x.Value, new TimeOnly(10, 0))
            .Add(x => x.ValueChanged, (TimeOnly? v) => changed(v)));
        cut.Find($".{Css.Classes.Input.Control}").KeyDown("ArrowDown");
        Assert.NotEmpty(cut.FindAll("[role=option]"));
        cut.Render(p => p.Add(x => x.ReadOnly, true));
        return cut;
    }

    [Fact]
    public void Escape_ClosesTheList_AfterTheFieldIsLocked()
    {
        var cut = OpenThenLock(_ => { });
        cut.Find($".{Css.Classes.Input.Control}").KeyDown("Escape");
        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public void ArrowsAndEnter_DoNothing_InTheLockedField()
    {
        TimeOnly? committed = null;
        var cut = OpenThenLock(v => committed = v);
        var input = cut.Find($".{Css.Classes.Input.Control}");
        var active = input.GetAttribute("aria-activedescendant");

        input.KeyDown("ArrowDown");
        cut.Find($".{Css.Classes.Input.Control}").KeyDown("Enter");

        Assert.Equal(active, cut.Find($".{Css.Classes.Input.Control}").GetAttribute("aria-activedescendant"));
        Assert.Null(committed);
    }
}
