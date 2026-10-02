namespace Flare.Components;

/// <summary>Visual style of the <c>FlareTimePicker</c> popup.</summary>
public enum TimePickerVariant
{
    /// <summary>Analog clock dial, 12-hour with AM/PM or 24-hour with two rings: pick the hour, then the
    /// minute, by pointer or by typing digits. Some design systems put a keyboard-entry mode inside the dial
    /// dialog; this popup has none - typing goes into the field, which is always editable.</summary>
    Dial,
    /// <summary>Scrollable hour, minute and (with seconds) second columns, offered by every theme. Design systems
    /// whose time picker is a single combobox list of times get these columns too; that list is not provided.</summary>
    Dropdown,
}
