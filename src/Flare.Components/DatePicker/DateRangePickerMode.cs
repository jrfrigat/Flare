namespace Flare.Components;

/// <summary>How a <see cref="FlareDateRangePicker"/> presents the range selection.</summary>
public enum DateRangePickerMode
{
    /// <summary>Two linked date inputs (start and end), each with its own calendar popup. The default.</summary>
    Fields,

    /// <summary>A single inline calendar where you click the start day then the end day; the days in
    /// between are highlighted and a live preview follows the pointer while choosing the end.</summary>
    Calendar,
}
