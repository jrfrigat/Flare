namespace Flare.Components;

/// <summary>An event shown on a day cell of <c>FlareCalendar</c>.</summary>
/// <param name="Title">Event label.</param>
/// <param name="Date">Day the event falls on.</param>
/// <param name="Color">Accent color for the event chip; defaults to the theme accent.</param>
public sealed record CalendarEvent(
    string Title,
    DateOnly Date,
    FlareColor Color = default);
