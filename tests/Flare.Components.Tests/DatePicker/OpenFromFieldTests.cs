using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-190: a click in the field opens the picker's popup as well as the toggle does. Opened from the field the
/// popup leaves focus where the user is typing - no focus trap, not modal - until Arrow Down moves into it; Escape
/// in the field closes it. A locked field does not open.
/// </summary>
public class OpenFromFieldTests : FlareTestContext
{
    private int Traps() => JSInterop.Invocations.Count(i => i.Identifier == "trapFocus");

    public static IEnumerable<object[]> Pickers() =>
    [
        [typeof(FlareDatePicker)], [typeof(FlareDateTimePicker)], [typeof(FlareMonthPicker)],
        [typeof(FlareWeekPicker)], [typeof(FlareMultiDatePicker)], [typeof(FlareTimePicker)],
    ];

    private IRenderedComponent<IComponent> RenderPicker(Type type, bool disabled = false, bool readOnly = false) =>
        (IRenderedComponent<IComponent>)Render(b =>
        {
            b.OpenComponent(0, type);
            b.AddAttribute(1, nameof(FlareFieldBase.Disabled), disabled);
            b.AddAttribute(2, nameof(FlareFieldBase.ReadOnly), readOnly);
            b.CloseComponent();
        });

    [Theory]
    [MemberData(nameof(Pickers))]
    public void ClickInTheField_OpensWithoutTakingFocus(Type type)
    {
        var cut = RenderPicker(type);
        cut.Find("input").Click();

        var dialog = cut.Find("[role=dialog]");
        Assert.Equal("false", dialog.GetAttribute("aria-modal"));
        Assert.Equal(0, Traps());
    }

    [Theory]
    [MemberData(nameof(Pickers))]
    public void ArrowDown_MovesIntoThePopup_AndMakesItModal(Type type)
    {
        var cut = RenderPicker(type);
        cut.Find("input").Click();
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        Assert.Equal("true", cut.Find("[role=dialog]").GetAttribute("aria-modal"));
        Assert.Equal(1, Traps());
    }

    [Theory]
    [MemberData(nameof(Pickers))]
    public void EscapeInTheField_Closes(Type type)
    {
        var cut = RenderPicker(type);
        cut.Find("input").Click();
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Theory]
    [MemberData(nameof(Pickers))]
    public void TheToggle_StillOpensAModalPopup(Type type)
    {
        var cut = RenderPicker(type);
        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.Equal("true", cut.Find("[role=dialog]").GetAttribute("aria-modal"));
        Assert.Equal(1, Traps());
    }

    [Theory]
    [MemberData(nameof(Pickers))]
    public void ALockedField_DoesNotOpen(Type type)
    {
        var disabled = RenderPicker(type, disabled: true);
        var readOnly = RenderPicker(type, readOnly: true);
        disabled.Find("input").Click();
        readOnly.Find("input").Click();

        Assert.Empty(disabled.FindAll("[role=dialog]"));
        Assert.Empty(readOnly.FindAll("[role=dialog]"));
    }

    [Fact]
    public void TimeList_OpensItsListFromTheField()
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.List));
        cut.Find("input").Click();

        Assert.Equal("true", cut.Find("input").GetAttribute("aria-expanded"));
    }
}
