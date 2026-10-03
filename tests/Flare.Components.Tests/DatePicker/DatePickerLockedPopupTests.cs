using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-162: a popup opened before the parent locks the field (Disabled or ReadOnly) still closes - by the API,
/// the scrim and Escape - while its days stay unavailable.
/// </summary>
public class DatePickerLockedPopupTests : FlareTestContext
{
    private IRenderedComponent<FlareDatePicker> OpenThenLock(bool disabled, bool readOnly, Action<DateOnly?>? changed = null)
    {
        var cut = Render<FlareDatePicker>(p =>
        {
            p.Add(x => x.Value, new DateOnly(2026, 10, 15));
            if (changed is not null) p.Add(x => x.ValueChanged, (DateOnly? v) => changed(v));
        });
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Render(p => p.Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));
        return cut;
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task CloseAsync_ClosesAfterTheLock(bool disabled, bool readOnly)
    {
        var cut = OpenThenLock(disabled, readOnly);
        await cut.InvokeAsync(() => cut.Instance.CloseAsync());
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ScrimAndEscape_CloseAfterTheLock(bool disabled, bool readOnly)
    {
        var cut = OpenThenLock(disabled, readOnly);
        cut.Find($".{Css.Classes.Picker.Scrim}").Click();
        Assert.Empty(cut.FindAll("[role=dialog]"));

        cut.Render(p => p.Add(x => x.Disabled, false).Add(x => x.ReadOnly, false));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Render(p => p.Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void LockedPopup_DoesNotPickADay_OrReopen()
    {
        DateOnly? committed = null;
        var cut = OpenThenLock(true, false, v => committed = v);
        cut.FindAll("button[role=gridcell]").First(b => b.TextContent.Trim() == "20").Click();
        Assert.Null(committed);

        cut.Find($".{Css.Classes.Picker.Scrim}").Click();
        cut.InvokeAsync(() => cut.Instance.OpenAsync());
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }
}
