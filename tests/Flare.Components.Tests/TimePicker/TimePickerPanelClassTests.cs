namespace Flare.Components.Tests;

/// <summary>
/// TASK-108: both popup variants carry a modifier class from the CssClasses contract, so a theme can
/// target the dropdown panel as well as the dial one.
/// </summary>
public class TimePickerPanelClassTests : FlareTestContext
{
    [Theory]
    [InlineData(TimePickerVariant.Dial, Css.Classes.TimePicker.PanelDial)]
    [InlineData(TimePickerVariant.Dropdown, Css.Classes.TimePicker.PanelDropdown)]
    public void Popup_CarriesItsVariantClass(TimePickerVariant variant, string expected)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, variant));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.Contains(expected, cut.Find($".{Css.Classes.TimePicker.Panel}").ClassList);
    }
}
