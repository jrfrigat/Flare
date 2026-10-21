using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-119: a half-edited date must not be saved as a different month/year on blur, and a complete edit
/// still commits. The mask moves digits around, so an incomplete "11.02.026" must be rejected rather than
/// parsed leniently into 11 Feb 0026.
/// </summary>
public class DatePickerCommitTests : FlareTestContext
{
    [Fact]
    public void IncompleteDayEdit_CannotCommitDifferentMonthAndYear()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ru-RU"))
            .Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        cut.Find("input").Input("11.02.026");
        cut.Find("input").Change("11.02.026");

        // The half-edit is refused and never parsed as a different month/year; since TASK-223 it stays
        // in the field with its reason instead of being replaced by the committed value.
        Assert.Null(committed);
        Assert.Equal("11.02.026", cut.Find("input").GetAttribute("value"));
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void CompleteDayEdit_StillCommits()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ru-RU"))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        cut.Find("input").Input("20.10.2026");

        Assert.Equal(new DateOnly(2026, 10, 20), committed);
    }
}
