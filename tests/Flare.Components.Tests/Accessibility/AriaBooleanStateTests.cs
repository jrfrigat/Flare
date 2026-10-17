namespace Flare.Components.Tests;

/// <summary>
/// A bool bound straight to an ARIA state renders as an HTML boolean attribute: true becomes
/// <c>aria-pressed=""</c> (an invalid value) and false drops the attribute, so "not selected" is never announced.
/// Each state below has to read "true" or "false" as text.
/// </summary>
public class AriaBooleanStateTests : FlareTestContext
{
    [Fact]
    public void Calendar_PressedStateIsTextForEveryDay()
    {
        var day = new DateOnly(2026, 10, 15);
        var cut = Render<FlareCalendar>(p => p.Add(x => x.InitialDate, day).Add(x => x.SelectedDate, day));

        var cells = cut.FindAll($".{Css.Classes.Calendar.Cell}[aria-label]");
        var values = cells.Select(c => c.GetAttribute("aria-pressed")).ToList();

        Assert.Single(values, v => v == "true");
        Assert.All(values, v => Assert.Contains(v, new[] { "true", "false" }));
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Switch_CheckedStateIsText(bool value, string expected)
    {
        var cut = Render<FlareSwitch>(p => p.Add(x => x.Value, value));

        Assert.Equal(expected, cut.Find("[role=switch]").GetAttribute("aria-checked"));
    }

    [Theory]
    [InlineData(false, "true")]
    [InlineData(true, null)]
    public void NavGroup_HidesOnlyCollapsedItems(bool expanded, string? expected)
    {
        var cut = Render<FlareNavGroup>(p => p.Add(x => x.Label, "Reports").Add(x => x.Expanded, expanded));

        Assert.Equal(expected, cut.Find("[role=group]").GetAttribute("aria-hidden"));
    }
}
