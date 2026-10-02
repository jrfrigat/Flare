namespace Flare.Components;

/// <summary>Visual style of the <c>FlareTimePicker</c> popup.</summary>
public enum TimePickerVariant
{
    /// <summary>Analog clock dial, 12-hour with AM/PM or 24-hour with two rings: pick the hour, then the
    /// minute, by pointer or by typing digits. Unlike the Material 2/3 time picker dialog, the popup has no
    /// keyboard-entry mode of its own - typing goes into the field, which is always editable.</summary>
    Dial,
    /// <summary>Scrollable hour, minute and (with seconds) second columns. Used by every theme, including
    /// Fluent UI 2, whose own time picker is a single combobox list of times; that list is not provided.</summary>
    Dropdown,
}
