namespace Flare.Components;

/// <summary>Visual style of the <c>FlareTimePicker</c> popup.</summary>
public enum TimePickerVariant
{
    /// <summary>Analog clock dial, 12-hour with AM/PM or 24-hour with two rings: pick the hour, then the
    /// minute, by pointer or by typing digits. A button in the dialog switches to keyboard entry (an hour and a
    /// minute text field), see <c>FlareTimePicker.ShowKeyboardToggle</c>.</summary>
    Dial,
    /// <summary>Scrollable hour, minute and (with seconds) second columns, offered by every theme.</summary>
    Dropdown,
    /// <summary>A single list of times under the field, as in a combobox (the time picker of combobox-based design systems): the field
    /// keeps focus, the arrow keys move through the list and Enter picks a time. The list runs from Min to Max
    /// every <c>MinuteStep</c> minutes, so set a step such as 15 or 30 - the default of 1 lists all 1440 minutes.
    /// <c>FlareDateTimePicker</c> shows its number fields for this value, as for <see cref="Dropdown"/>.</summary>
    List,
}
