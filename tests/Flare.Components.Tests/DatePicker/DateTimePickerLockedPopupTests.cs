using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-169: a date-time popup opened before the parent locks the field still closes on Escape from the day grid,
/// whose key events do not bubble to the panel, while the arrows do not move the cursor (as TASK-162).
/// </summary>
public class DateTimePickerLockedPopupTests : FlareTestContext
{
    private IRenderedComponent<FlareDateTimePicker> OpenThenLock(bool disabled, bool readOnly)
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 0, 0, TimeSpan.Zero)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Render(p => p.Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));
        return cut;
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void EscapeInTheGrid_ClosesAfterTheLock(bool disabled, bool readOnly)
    {
        var cut = OpenThenLock(disabled, readOnly);
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void ArrowsInTheGrid_DoNotMoveTheCursor_AfterTheLock()
    {
        var cut = OpenThenLock(false, true);
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("15", cut.Find("button[role=gridcell][tabindex='0']").TextContent.Trim());
    }
}
