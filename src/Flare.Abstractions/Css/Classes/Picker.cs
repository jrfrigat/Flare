namespace Flare.Css.Classes;

/// <summary>
/// Shared classes for the date/time picker popups: the fixed, elevated panel and the full-screen
/// scrim behind it. Used by FlareDatePicker, FlareTimePicker and FlareDateTimePicker; each component
/// adds only its own size/shape deltas on top of these.
/// </summary>
public static class Picker
{
    /// <summary>The <c>flare-picker__scrim</c> CSS class: the full-screen click-catcher behind a picker popup.</summary>
    public const string Scrim = "flare-picker__scrim";
    /// <summary>The <c>flare-picker__panel</c> CSS class: the fixed, elevated picker popup surface.</summary>
    public const string Panel = "flare-picker__panel";
    /// <summary>The <c>flare-picker__day-headers</c> CSS class: the weekday header row of the month grid.</summary>
    public const string DayHeaders = "flare-picker__day-headers";
    /// <summary>The <c>flare-picker__grid</c> CSS class: the 7-column month day grid.</summary>
    public const string Grid = "flare-picker__grid";
    /// <summary>The <c>flare-picker__day</c> CSS class: a single day cell in the month grid.</summary>
    public const string Day = "flare-picker__day";
    /// <summary>The <c>flare-picker__day--today</c> CSS class.</summary>
    public const string DayToday = "flare-picker__day--today";
    /// <summary>The <c>flare-picker__day--selected</c> CSS class.</summary>
    public const string DaySelected = "flare-picker__day--selected";
    /// <summary>The <c>flare-picker__day--outside</c> CSS class: a day outside the shown month.</summary>
    public const string DayOutside = "flare-picker__day--outside";
    /// <summary>The <c>flare-picker__day--disabled</c> CSS class.</summary>
    public const string DayDisabled = "flare-picker__day--disabled";
}
