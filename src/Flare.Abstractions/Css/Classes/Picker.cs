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
}
