namespace Flare.Css.Classes;

/// <summary>Scheduler projection classes; override these or the scheduler tokens to supply a design system.</summary>
public static class Scheduler
{
    /// <summary>Primary event activation with standard scheduler keyboard semantics.</summary>
    public const string EventAction = "flare-scheduler__event-action";
    /// <summary>Active projection and loading-state boundary.</summary>
    public const string Body = "flare-scheduler__body";
    /// <summary>Shared width boundary of headers, all-day band and timed columns.</summary>
    public const string TimeGridFrame = "flare-scheduler__time-grid-frame";
    /// <summary>Sticky common header and all-day band.</summary>
    public const string HeaderBand = "flare-scheduler__header-band";
    /// <summary>Aligned date headings.</summary>
    public const string DayHeaders = "flare-scheduler__day-headers";
    /// <summary>Shared all-day lane grid.</summary>
    public const string AllDayGrid = "flare-scheduler__all-day-grid";
    /// <summary>All-day band heading.</summary>
    public const string AllDayCaption = "flare-scheduler__all-day-caption";
    /// <summary>All-day date drop boundary.</summary>
    public const string AllDayTarget = "flare-scheduler__all-day-target";
    /// <summary>Background date-selection control.</summary>
    public const string AllDaySlot = "flare-scheduler__all-day-slot";
    /// <summary>Positioned date-only segment.</summary>
    public const string AllDayEvent = "flare-scheduler__all-day-event";
    /// <summary>Overflow action for a date.</summary>
    public const string AllDayMore = "flare-scheduler__all-day-more";
    /// <summary>Focus boundary around the shared resize widget.</summary>
    public const string ResizeFrame = "flare-scheduler__resize-frame";
    /// <summary>Standard event editor.</summary>
    public const string Editor = "flare-scheduler__editor";
    /// <summary>Event drag wrapper that composes the shared handle and resizable.</summary>
    public const string EventDrag = "flare-scheduler__event-drag";
    /// <summary>Resource timeline projection boundary.</summary>
    public const string Timeline = "flare-scheduler__timeline";
    /// <summary>The Root scheduler element.</summary>
    public const string Root = "flare-scheduler";
    /// <summary>The Toolbar scheduler element.</summary>
    public const string Toolbar = "flare-scheduler__toolbar";
    /// <summary>The Viewport scheduler element.</summary>
    public const string Viewport = "flare-scheduler__viewport";
    /// <summary>The TimeGrid scheduler element.</summary>
    public const string TimeGrid = "flare-scheduler__time-grid";
    /// <summary>The Day scheduler element.</summary>
    public const string Day = "flare-scheduler__day";
    /// <summary>The DayHeader scheduler element.</summary>
    public const string DayHeader = "flare-scheduler__day-header";
    /// <summary>The Slots scheduler element.</summary>
    public const string Slots = "flare-scheduler__slots";
    /// <summary>The Slot scheduler element.</summary>
    public const string Slot = "flare-scheduler__slot";
    /// <summary>The SlotOutside scheduler element.</summary>
    public const string SlotOutside = "flare-scheduler__slot--outside";
    /// <summary>The SlotWeekend scheduler element.</summary>
    public const string SlotWeekend = "flare-scheduler__slot--weekend";
    /// <summary>The SlotToday scheduler element.</summary>
    public const string SlotToday = "flare-scheduler__slot--today";
    /// <summary>The Event scheduler element.</summary>
    public const string Event = "flare-scheduler__event";
    /// <summary>The EventBody scheduler element.</summary>
    public const string EventBody = "flare-scheduler__event-body";
    /// <summary>The AllDay scheduler element.</summary>
    public const string AllDay = "flare-scheduler__all-day";
    /// <summary>The Agenda scheduler element.</summary>
    public const string Agenda = "flare-scheduler__agenda";
    /// <summary>The AgendaItem scheduler element.</summary>
    public const string AgendaItem = "flare-scheduler__agenda-item";
    /// <summary>The Now scheduler element.</summary>
    public const string Now = "flare-scheduler__now";
    /// <summary>The Status scheduler element.</summary>
    public const string Status = "flare-scheduler__status";
    /// <summary>The Month scheduler element.</summary>
    public const string Month = "flare-scheduler__month";
    /// <summary>The Year projection of the month element; keeps the scrolling multi-month grid.</summary>
    public const string MonthYear = "flare-scheduler__month--year";
    /// <summary>Start time shown before an event title in the month projection.</summary>
    public const string EventTime = "flare-scheduler__event-time";
    /// <summary>Container of the month panels.</summary>
    public const string MonthGrid = "flare-scheduler__month-grid";
    /// <summary>One month panel in the month projection.</summary>
    public const string MonthPanel = "flare-scheduler__month-panel";
    /// <summary>Month title row.</summary>
    public const string MonthHeader = "flare-scheduler__month-header";
    /// <summary>Weekday heading row of a month panel.</summary>
    public const string MonthWeekdays = "flare-scheduler__month-weekdays";
    /// <summary>One week row of a month panel.</summary>
    public const string MonthWeek = "flare-scheduler__month-week";
    /// <summary>Absolute band layer holding the spanning event bars of a week.</summary>
    public const string MonthBars = "flare-scheduler__month-bars";
    /// <summary>One continuous bar of a multi-day event inside a week.</summary>
    public const string MonthBar = "flare-scheduler__month-bar";
    /// <summary>Single-day event list inside a day cell.</summary>
    public const string MonthCellEvents = "flare-scheduler__month-cell-events";
}

