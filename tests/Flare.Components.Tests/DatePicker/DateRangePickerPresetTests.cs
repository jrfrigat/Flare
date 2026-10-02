namespace Flare.Components.Tests;

/// <summary>
/// TASK-114: a quick-range preset must obey the same bounds as a manual pick — Min/Max and
/// IsDateDisabled — and a reversed preset range is normalised before it is applied.
/// </summary>
public class DateRangePickerPresetTests : FlareTestContext
{
    private sealed class Capture
    {
        public DateOnly? Start;
        public DateOnly? End;
    }

    private static DateRangePreset Preset(string label, DateOnly start, DateOnly end)
        => new(label, _ => (start, end));

    private (IRenderedComponent<FlareDateRangePicker> Cut, Capture Capture) RenderWith(
        DateRangePreset preset, DateOnly? min = null, DateOnly? max = null, Func<DateOnly, bool>? disabled = null)
    {
        var capture = new Capture();
        var cut = Render<FlareDateRangePicker>(p => p
            .Add(x => x.ShowPresets, true)
            .Add(x => x.Presets, new[] { preset })
            .Add(x => x.Min, min)
            .Add(x => x.Max, max)
            .Add(x => x.IsDateDisabled, disabled)
            .Add(x => x.StartDateChanged, (DateOnly? v) => capture.Start = v)
            .Add(x => x.EndDateChanged, (DateOnly? v) => capture.End = v));
        return (cut, capture);
    }

    private static void ClickPreset(IRenderedComponent<FlareDateRangePicker> cut)
        => cut.Find($".{Css.Classes.Daterangepicker.Preset}").Click();

    [Fact]
    public void OutOfRangePreset_IsNotApplied()
    {
        var (cut, capture) = RenderWith(
            Preset("Aug", new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 2)),
            min: new DateOnly(2026, 10, 1), max: new DateOnly(2026, 10, 31));

        ClickPreset(cut);

        Assert.Null(capture.Start);
        Assert.Null(capture.End);
    }

    [Fact]
    public void InRangePreset_IsApplied()
    {
        var (cut, capture) = RenderWith(
            Preset("Oct", new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 9)),
            min: new DateOnly(2026, 10, 1), max: new DateOnly(2026, 10, 31));

        ClickPreset(cut);

        Assert.Equal(new DateOnly(2026, 10, 5), capture.Start);
        Assert.Equal(new DateOnly(2026, 10, 9), capture.End);
    }

    [Fact]
    public void PresetWithADisabledEndpoint_IsNotApplied()
    {
        var (cut, capture) = RenderWith(
            Preset("Oct", new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 9)),
            min: new DateOnly(2026, 10, 1), max: new DateOnly(2026, 10, 31),
            disabled: d => d.Day == 5);

        ClickPreset(cut);

        Assert.Null(capture.Start);
        Assert.Null(capture.End);
    }

    [Fact]
    public void ReversedPreset_IsNormalisedAndApplied()
    {
        var (cut, capture) = RenderWith(
            Preset("Oct", new DateOnly(2026, 10, 9), new DateOnly(2026, 10, 5)),
            min: new DateOnly(2026, 10, 1), max: new DateOnly(2026, 10, 31));

        ClickPreset(cut);

        Assert.Equal(new DateOnly(2026, 10, 5), capture.Start);
        Assert.Equal(new DateOnly(2026, 10, 9), capture.End);
    }
}
