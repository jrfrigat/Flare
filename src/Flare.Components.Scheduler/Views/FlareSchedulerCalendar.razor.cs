using System.Globalization;
using Flare.Components.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

// Month and Year projection of the scheduler. The layout is the scheduler's own: multi-day events are
// continuous bars per week, single-day events stay in the day cells. FlareCalendar is not used here so
// the calendar component does not grow a scheduler-specific span model.
public partial class FlareSchedulerCalendar<TItem> : FlareSchedulerViewBase<TItem>
{
    /// <summary>Optional month title content.</summary>
    [Parameter] public RenderFragment<DateOnly>? MonthHeaderTemplate { get; set; }

    /// <summary>Optional date heading inside each day cell.</summary>
    [Parameter] public RenderFragment<DateOnly>? DateTemplate { get; set; }

    /// <summary>Optional weekday heading content.</summary>
    [Parameter] public RenderFragment<DayOfWeek>? DayHeaderTemplate { get; set; }

    private IReadOnlyList<SchedulerMonthPanel<TItem>> _panels = [];
    private (IReadOnlyList<SchedulerOccurrence<TItem>> Occurrences, SchedulerWindow Window, TimeZoneInfo Zone,
        int Version, DateOnly Date, SchedulerView View, CultureInfo Culture)? _projectionKey;
    private readonly record struct CalendarRender(SchedulerViewContext<TItem> Context, string? Class, string? Style,
        DateOnly Today, FlareLocalizationState? Localization);
    private CalendarRender? _lastDefaultRender;
    private bool _keyboardRegistered;
    private ElementReference _root;

    private CultureInfo Culture => Context.Culture;
    private DateTimeFormatInfo Format => Culture.DateTimeFormat;
    private DayOfWeek FirstDayOfWeek => Format.FirstDayOfWeek;
    private DateOnly Today => DateOnly.FromDateTime(Clock.GetLocalNow().DateTime);
    private DateOnly? SelectedDate => Context.SelectedSlot is not { } slot ? null : slot.AllDay?.Start
        ?? DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(slot.Start, Context.TimeZone).DateTime);

    private string RootClass => Context.View == SchedulerView.Year
        ? BuildCssClass(Css.Classes.Scheduler.Month, Css.Classes.Scheduler.MonthYear)
        : BuildCssClass(Css.Classes.Scheduler.Month);

    private bool CanReuseRender => GetType() == typeof(FlareSchedulerCalendar<TItem>) &&
        MonthHeaderTemplate is null && DateTemplate is null && DayHeaderTemplate is null && EventTemplate is null &&
        AdditionalAttributes is null && Context.Focus.Identity is null && Context.Culture.IsReadOnly;

    protected override bool ShouldRender()
    {
        // Application templates can depend on state outside Context. The standard month can keep the
        // last projection when the context, current day and outer attributes agree.
        if (!CanReuseRender || _lastDefaultRender is not { } previous) return true;
        return previous.Context != Context || previous.Class != Class || previous.Style != Style ||
            previous.Today != Today || !ReferenceEquals(previous.Localization, LocalizationState) ||
            !ReferenceEquals(previous.Context.Culture, Context.Culture) ||
            !ReferenceEquals(previous.Context.Commands, Context.Commands) ||
            !ReferenceEquals(previous.Context.Occurrences, Context.Occurrences) ||
            previous.Context.Date != Context.Date || previous.Context.View != Context.View;
    }

    protected override void OnParametersSet()
    {
        var key = (Context.Occurrences, Context.Window, Context.TimeZone, Context.Version,
            Context.Date, Context.View, Context.Culture);
        if (_projectionKey == key) return;
        _projectionKey = key;

        var anchor = Context.View == SchedulerView.Year
            ? new DateOnly(Context.Date.Year, 1, 1)
            : new DateOnly(Context.Date.Year, Context.Date.Month, 1);
        var count = Context.View == SchedulerView.Year ? 12 : 1;
        var events = SchedulerMonthLayout.Project(Context.Occurrences, Context.TimeZone, Context.Culture);
        var panels = new List<SchedulerMonthPanel<TItem>>(count);
        for (var i = 0; i < count; i++)
            panels.Add(SchedulerMonthLayout.BuildMonth(anchor.AddMonths(i), FirstDayOfWeek, events));
        _panels = panels;
    }

    private string MonthTitle(DateOnly month) => Culture.TextInfo.ToTitleCase(month.ToString("MMMM yyyy", Culture));

    private static string WeekStyle(SchedulerMonthWeek<TItem> week) =>
        FormattableString.Invariant($"--month-bands:{week.BandCount}");

    private static string BarStyle(SchedulerMonthBar<TItem> bar) =>
        FormattableString.Invariant($"grid-row:{bar.Band + 1};grid-column:{bar.StartColumn + 1} / span {bar.Length}");

    private static string CellClass(bool other, bool today, bool selected) => string.Concat(
        Css.Classes.Calendar.Cell,
        other ? " " + Css.Classes.Calendar.CellOther : "",
        today ? " " + Css.Classes.Calendar.CellToday : "",
        selected ? " " + Css.Classes.Calendar.CellSelected : "");

    // Event chips are children, not the calendar root; keep the shared event class and the color role.
    private static string EventCssClass(FlareColor color) =>
        color.CssClass is { Length: > 0 } role
            ? string.Concat(Css.Classes.Calendar.Event, " ", role)
            : Css.Classes.Calendar.Event;

    private Task SelectDayAsync(DateOnly date, bool other)
    {
        if (other) return Task.CompletedTask;
        var dates = new SchedulerDateRange(date, date.AddDays(1));
        var window = SchedulerClock.Window(dates, Context.TimeZone);
        return Context.Commands.SelectSlotAsync(new(window.Start, window.End, dates));
    }

    private Task DayKey(KeyboardEventArgs e, DateOnly date, bool other) =>
        e.Key is "Enter" or " " ? SelectDayAsync(date, other) : Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) { await Ui.RegisterTreeKeyboardAsync(_root); _keyboardRegistered = true; }
    }

    /// <summary>Releases the independent view's native keyboard listener.</summary>
    public override async ValueTask DisposeAsync()
    {
        try { if (_keyboardRegistered) await Ui.RemoveTreeKeyboardAsync(_root); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        await base.DisposeAsync();
    }
}
