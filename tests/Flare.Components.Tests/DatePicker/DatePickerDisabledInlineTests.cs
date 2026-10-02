namespace Flare.Components.Tests;

/// <summary>
/// TASK-104: a disabled or read-only date field must not be editable through the inline calendar either.
/// </summary>
public class DatePickerDisabledInlineTests : FlareTestContext
{
    [Fact]
    public void DisabledInlineCalendar_DoesNotChangeValue()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Disabled, true)
            .Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        cut.Find($".{Css.Classes.Picker.Grid} button[role='gridcell']").Click();

        Assert.Null(committed);
    }

    [Fact]
    public void ReadOnlyInlineCalendar_DoesNotChangeValue()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.ReadOnly, true)
            .Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        cut.Find($".{Css.Classes.Picker.Grid} button[role='gridcell']").Click();

        Assert.Null(committed);
    }

    [Fact]
    public void InlineCalendar_WithoutDisable_StillChangesValue()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        cut.Find($".{Css.Classes.Picker.Grid} button[role='gridcell']").Click();

        Assert.NotNull(committed);
    }
}
