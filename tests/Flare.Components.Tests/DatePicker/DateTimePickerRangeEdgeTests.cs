namespace Flare.Components.Tests;

/// <summary>
/// TASK-160: on the first and last day of the DateTimeOffset range an offset can push the picked wall time past
/// the representable instants. The popup must not throw; OK waits until the time is representable.
/// </summary>
public class DateTimePickerRangeEdgeTests : FlareTestContext
{
    private IRenderedComponent<FlareDateTimePicker> Open(DateTimeOffset value, Action<DateTimeOffset?> changed)
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => changed(v)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut;
    }

    private static AngleSharp.Dom.IElement Ok(IRenderedComponent<FlareDateTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last();

    private static void PickDay(IRenderedComponent<FlareDateTimePicker> cut, string day) =>
        cut.FindAll("button[role=gridcell]").First(b => b.TextContent.Trim() == day).Click();

    [Fact]
    public void FirstDay_WithAPositiveOffset_DoesNotThrow_AndWaitsForARepresentableTime()
    {
        DateTimeOffset? committed = null;
        var cut = Open(new DateTimeOffset(1, 1, 2, 0, 0, 0, TimeSpan.FromHours(5)), v => committed = v);

        PickDay(cut, "1");   // 0001-01-01 00:00 +05:00 is before the first UTC instant
        Assert.True(Ok(cut).HasAttribute("disabled"));
        Ok(cut).Click();
        Assert.Null(committed);

        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[0].Change("6");
        Assert.False(Ok(cut).HasAttribute("disabled"));
        Ok(cut).Click();
        Assert.Equal(new DateTimeOffset(1, 1, 1, 6, 0, 0, TimeSpan.FromHours(5)), committed);
    }

    [Fact]
    public void LastDay_WithANegativeOffset_DoesNotThrow_AndWaitsForARepresentableTime()
    {
        DateTimeOffset? committed = null;
        var cut = Open(new DateTimeOffset(9999, 12, 31, 12, 0, 0, TimeSpan.FromHours(-5)), v => committed = v);

        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[0].Change("23");   // 23:00 -05:00 is past the last UTC instant
        Assert.True(Ok(cut).HasAttribute("disabled"));

        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[0].Change("18");
        Ok(cut).Click();
        Assert.Equal(new DateTimeOffset(9999, 12, 31, 18, 0, 0, TimeSpan.FromHours(-5)), committed);
    }
}
