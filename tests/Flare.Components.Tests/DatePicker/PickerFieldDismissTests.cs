using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>Field-open popups permit pointer editing, and own dismissal only until close or modal entry.</summary>
public class PickerFieldDismissTests : FlareTestContext
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public Task FieldClick_RemainsEditableAndOutsideDismissCloses(int family) => Run(family, false);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public Task FieldClick_KeyboardEntryBecomesModalAndDetachesDismiss(int family) => Run(family, true);

    private Task Run(int family, bool enter) => family switch
    {
        0 => Verify<FlareDatePicker>(enter),
        1 => Verify<FlareDateTimePicker>(enter),
        2 => Verify<FlareMonthPicker>(enter),
        3 => Verify<FlareWeekPicker>(enter),
        4 => Verify<FlareMultiDatePicker>(enter),
        _ => Verify<FlareTimePicker>(enter),
    };

    private int Calls(string name) => JSInterop.Invocations.Count(i => i.Identifier == name);

    private async Task Verify<T>(bool enter) where T : ComponentBase
    {
        var cut = Render<T>();
        cut.Find("input").Click();
        Assert.Equal("false", cut.Find("[role='dialog']").GetAttribute("aria-modal"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Picker.Scrim}"));
        Assert.Equal(0, Calls("trapFocus"));
        Assert.Equal(1, Calls("registerDismiss"));

        cut.Find("input").Click();
        Assert.Single(cut.FindAll("[role='dialog']"));
        Assert.Equal(1, Calls("registerDismiss"));

        if (enter)
        {
            cut.Find("input").KeyDown("ArrowDown");
            Assert.Equal("true", cut.Find("[role='dialog']").GetAttribute("aria-modal"));
            Assert.Single(cut.FindAll($".{Css.Classes.Picker.Scrim}"));
            Assert.Equal(1, Calls("trapFocus"));
            Assert.Equal(1, Calls("removeDismiss"));
            cut.Find($".{Css.Classes.Picker.Scrim}").Click();
            Assert.Equal(1, Calls("releaseFocusTrap"));
        }
        else
        {
            var registration = JSInterop.Invocations.Single(i => i.Identifier == "registerDismiss");
            var reference = registration.Arguments[2]!;
            var popup = reference.GetType().GetProperty("Value")!.GetValue(reference)!;
            await cut.InvokeAsync(() => (Task)popup.GetType().GetMethod("DismissFromJs")!.Invoke(popup, null)!);
            cut.WaitForAssertion(() => Assert.Empty(cut.FindAll("[role='dialog']")));
            Assert.Equal(1, Calls("removeDismiss"));

            cut.Find("input").Click();
            Assert.Equal(2, Calls("registerDismiss"));
            await cut.InvokeAsync(() => ((IAsyncDisposable)cut.Instance).DisposeAsync().AsTask());
            Assert.Equal(2, Calls("removeDismiss"));
            return;
        }
        Assert.Empty(cut.FindAll("[role='dialog']"));
    }
}
