namespace Flare.Components.Tests;

/// <summary>
/// TASK-102: Min/Max bound the time the picker accepts, on the text path as well as the popup. An
/// out-of-range typed time is not committed; an in-range one is.
/// </summary>
public class TimePickerRangeTests : FlareTestContext
{
    [Fact]
    public void OutOfRangeTypedTime_IsNotCommitted()
    {
        TimeOnly? committed = null;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Min, new TimeOnly(9, 0))
            .Add(x => x.Max, new TimeOnly(17, 0))
            .Add(x => x.ValueChanged, (TimeOnly? v) => committed = v));

        cut.Find("input").Input("23:59");

        Assert.Null(committed);
    }

    [Fact]
    public void InRangeTypedTime_IsCommitted()
    {
        TimeOnly? committed = null;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Min, new TimeOnly(9, 0))
            .Add(x => x.Max, new TimeOnly(17, 0))
            .Add(x => x.ValueChanged, (TimeOnly? v) => committed = v));

        cut.Find("input").Input("14:30");

        Assert.Equal(new TimeOnly(14, 30), committed);
    }
}
