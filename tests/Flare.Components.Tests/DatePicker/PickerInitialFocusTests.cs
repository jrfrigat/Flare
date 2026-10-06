using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>Modal pickers declare one starting cursor for the shared focus trap.</summary>
public class PickerInitialFocusTests : FlareTestContext
{
    private int FocusCalls => JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    [Theory]
    [InlineData(PickerOpenTo.Day)]
    [InlineData(PickerOpenTo.Month)]
    [InlineData(PickerOpenTo.Year)]
    public void DateOpen_DeclaresAvailableCursorWithoutSecondFocus(PickerOpenTo view)
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US")).Add(x => x.OpenTo, view));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var cursor = Assert.Single(cut.FindAll("[data-flare-initial-focus='true']"));
        Assert.Equal("0", cursor.GetAttribute("tabindex"));
        Assert.False(cursor.HasAttribute("disabled"));
        Assert.Equal(0, FocusCalls);
        Assert.Single(JSInterop.Invocations, i => i.Identifier == "trapFocus");
    }

    [Fact]
    public void DateOpen_DisabledValueChoosesAvailableCursor()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.IsDateDisabled, d => d.Day == 15));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var cursor = Assert.Single(cut.FindAll("[data-flare-initial-focus='true']"));
        Assert.False(cursor.HasAttribute("disabled"));
        Assert.Equal("0", cursor.GetAttribute("tabindex"));
    }

    [Fact]
    public void DateOpen_NoAvailableDaysLeavesFocusToNavigation()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.IsDateDisabled, _ => true));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Empty(cut.FindAll("[data-flare-initial-focus='true']"));
        Assert.Equal(0, FocusCalls);
    }

    [Theory]
    [InlineData(TimePickerVariant.Dial, "slider")]
    [InlineData(TimePickerVariant.Dropdown, "group")]
    public void TimeOpen_DeclaresCompositeCursorWithoutSecondFocus(TimePickerVariant variant, string role)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, variant));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var cursor = Assert.Single(cut.FindAll("[data-flare-initial-focus='true']"));
        Assert.Equal(role, cursor.GetAttribute("role"));
        Assert.Equal("0", cursor.GetAttribute("tabindex"));
        Assert.Equal(0, FocusCalls);
    }

    [Theory]
    [InlineData(TimePickerVariant.Dial)]
    [InlineData(TimePickerVariant.Dropdown)]
    public void TimeFieldOpen_DoesNotMoveFocus(TimePickerVariant variant)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, variant));
        cut.Find("input").Click();
        Assert.Equal(0, FocusCalls);
        Assert.DoesNotContain(JSInterop.Invocations, i => i.Identifier == "trapFocus");
        Assert.Equal("false", cut.Find("[role='dialog']").GetAttribute("aria-modal"));
    }
}
