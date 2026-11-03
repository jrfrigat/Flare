using System.Globalization;
using System.Resources;

namespace Flare.Components.Scheduler;

/// <summary>Localized default labels and accessible names, using the current UI culture like the base Flare components.</summary>
public static class SchedulerStrings
{
    /// <summary>Default accessible name of the scheduling region.</summary>
    public static string Schedule => Get("Schedule");
    /// <summary>Delete action.</summary>
    public static string Delete => Get("Delete");
    /// <summary>Move action.</summary>
    public static string Move => Get("Move");
    /// <summary>Resize action.</summary>
    public static string Resize => Get("Resize");
    private static readonly ResourceManager Resources = new("Flare.Components.Resources.SchedulerStrings", typeof(SchedulerStrings).Assembly);
    private static string Get(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    /// <summary>Previous</summary>
    public static string Previous => Get("Previous");
    /// <summary>Next</summary>
    public static string Next => Get("Next");
    /// <summary>Today</summary>
    public static string Today => Get("Today");
    /// <summary>Day</summary>
    public static string Day => Get("Day");
    /// <summary>Week</summary>
    public static string Week => Get("Week");
    /// <summary>Work week</summary>
    public static string WorkWeek => Get("WorkWeek");
    /// <summary>Month</summary>
    public static string Month => Get("Month");
    /// <summary>Agenda</summary>
    public static string Agenda => Get("Agenda");
    /// <summary>Timeline</summary>
    public static string Timeline => Get("Timeline");
    /// <summary>Year</summary>
    public static string Year => Get("Year");
    /// <summary>Loading events</summary>
    public static string Loading => Get("Loading");
    /// <summary>Events could not be loaded.</summary>
    public static string LoadError => Get("LoadError");
    /// <summary>No events</summary>
    public static string Empty => Get("Empty");
    /// <summary>All day</summary>
    public static string AllDay => Get("AllDay");
    /// <summary>Create event</summary>
    public static string Create => Get("Create");
    /// <summary>Edit event</summary>
    public static string Edit => Get("Edit");
    /// <summary>Save</summary>
    public static string Save => Get("Save");
    /// <summary>Cancel</summary>
    public static string Cancel => Get("Cancel");
    /// <summary>Title</summary>
    public static string Title => Get("Title");
    /// <summary>Start</summary>
    public static string Start => Get("Start");
    /// <summary>End (exclusive)</summary>
    public static string End => Get("End");
    /// <summary>Description</summary>
    public static string Description => Get("Description");
    /// <summary>Resource</summary>
    public static string Resource => Get("Resource");
    /// <summary>Repeat (RFC 5545 RRULE)</summary>
    public static string Recurrence => Get("Recurrence");
    /// <summary>For example: FREQ=WEEKLY;BYDAY=MO,WE. Leave empty for one event.</summary>
    public static string RecurrenceHint => Get("RecurrenceHint");
    /// <summary>Apply to</summary>
    public static string Scope => Get("Scope");
    /// <summary>This occurrence</summary>
    public static string Occurrence => Get("Occurrence");
    /// <summary>This and following</summary>
    public static string Following => Get("Following");
    /// <summary>Whole series</summary>
    public static string Series => Get("Series");
    /// <summary>Event change requested.</summary>
    public static string Changed => Get("Changed");
    /// <summary>Change canceled.</summary>
    public static string Cancelled => Get("Cancelled");
    /// <summary>Check the dates and recurrence rule.</summary>
    public static string Invalid => Get("Invalid");
    /// <summary>Unassigned</summary>
    public static string Unassigned => Get("Unassigned");
    /// <summary>Localized caption of a built-in view.</summary>
    public static string ViewName(SchedulerView view) => Get(view.ToString());
}

