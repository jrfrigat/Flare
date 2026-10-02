namespace Flare.Components.Tests;

/// <summary>
/// TASK-118: a partial edit must not let the mask rewrite the neighbouring segment. The mask now lays the
/// digits out on the HH:mm[:ss] skeleton without clamping, so the browser scenario 23:59 -&gt; Home, Right,
/// Right, Backspace, type the hour digit again keeps 21:59 instead of scrambling the minutes into 21:39.
/// </summary>
public class TimePickerMaskTests : FlareTestContext
{
    [Theory]
    [InlineData(null, false, "")]
    [InlineData("", false, "")]
    [InlineData("2", false, "2")]
    [InlineData("25", false, "25")]
    [InlineData("2:59", false, "25:9")]      // no destructive clamp of the hour
    [InlineData("215:9", false, "21:59")]
    [InlineData("2359", false, "23:59")]
    [InlineData("9959", false, "99:59")]     // out-of-range is left for validation, not silently corrected
    [InlineData("14305", true, "14:30:5")]
    [InlineData("143045", true, "14:30:45")]
    public void MaskTime_LaysDigitsOut_WithoutClampingNeighbours(string? raw, bool showSeconds, string expected)
        => Assert.Equal(expected, MaskedInput.MaskTime(raw, showSeconds));

    [Fact]
    public void HourReplacement_KeepsMinutes()
    {
        TimeOnly? committed = null;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(23, 59))
            .Add(x => x.ValueChanged, (TimeOnly? v) => committed = v));

        // 23:59, caret after the hour, Backspace: the raw "2:59" must stay on the skeleton, not be
        // rewritten into a bad hour ("23:9") that would swallow the first minute digit.
        cut.Find("input").Input("2:59");
        Assert.Equal("25:9", cut.Find("input").GetAttribute("value"));

        // Typing the replacement hour digit reconstructs the original value again.
        cut.Find("input").Input("215:9");
        Assert.Equal(new TimeOnly(21, 59), committed);
    }
}
