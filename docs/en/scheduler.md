# Scheduler

`FlareScheduler<TItem>` adds calendar planning to an application's own event model. Month and Year
compose `FlareCalendar`; the resource Timeline composes `FlareGantt`. Day, Week, WorkWeek and Agenda
share the same window, event projection and editing contract. Calendar and Gantt remain usable independently.

## Setup

```sh
dotnet add package Flare.Components.Scheduler
```

After the ordinary Flare setup and a theme, register the shared timeline services:

```csharp
using Flare.Components.Scheduler;
builder.Services.AddFlareScheduler();
```

Add both stylesheets after the core Flare stylesheet:

```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
<link rel="stylesheet" href="_content/Flare.Components.Scheduler/css/scheduler.css" />
```

Use the normal `FlareThemeProvider` and `FlareDialogProvider`. The latter hosts the default event editor.
Scheduler needs interactive rendering. Give its parent a bounded height for a scrolling time grid.

## Your model

```razor
@using Flare.Components.Scheduler

<FlareStack Height="36rem">
    <FlareScheduler TItem="Meeting" Mapping="Mapping" Events="meetings" />
</FlareStack>

@code {
    private sealed record Meeting(string Id, string Name, DateTimeOffset From, DateTimeOffset Until);
    private readonly List<Meeting> meetings = [];
    private static readonly SchedulerMapping<Meeting> Mapping = new()
    {
        Key = m => m.Id, Title = m => m.Name, Start = m => m.From, End = m => m.Until
    };
}
```

No inheritance or mutable wrapper is required. `SchedulerMappings.EventData` is a ready mapping for
`SchedulerEventData`. Keys must be unique and stable. Timed intervals use inclusive Start and exclusive End;
all-day events use `SchedulerDateRange` with independent `DateOnly` bounds and an exclusive End.

`Date`/`DateChanged` and `View`/`ViewChanged` support controlled state. Without callbacks the component
owns navigation. `Views` limits the toolbar choices. `Culture` controls date formatting and the first weekday;
interface strings follow `CultureInfo.CurrentUICulture` with English fallback. `TimeZone` changes the display
of instants, never the dates of an all-day event. Missing DST slots are omitted; repeated times retain their
distinct offsets. Working hours shade the grid; `InitialScrollTime` defaults to `WorkStart`.

## Windowed data and recurrence

Instead of `Events`, supply `EventsProvider`. Its `SchedulerDataRequest` contains a half-open `Window`
and cancellation token. Return every event intersecting that window, including events beginning earlier.
Obsolete provider results are ignored even if the provider ignores cancellation. Call `ReloadAsync` after
external changes. `OnLoadError` reports failures.

The default path accepts ready backend occurrences. Supply stable `SeriesKey` and `RecurrenceId` when the
application needs recurring-event edits. The UI neither parses recurrence rules nor contacts a backend.
For a standalone client, install `Flare.Components.Scheduler.Ical` and explicitly set
`RecurrenceExpander="IcalSchedulerRecurrenceExpander.Default"` (namespace
`Flare.Components.Scheduler.Ical`). That optional adapter uses Ical.Net for RRULE/RDATE/EXDATE and overrides.
Only that package introduces the RFC dependency. Limits and cancellation bound local expansion; the
application still owns persistence, recurrence rules and splitting a series.

## Editing and composition

Enable `Editable` and handle `OnChange`. Each `SchedulerChangeEventArgs<TItem>` carries the original item,
stable occurrence identity, immutable proposed `Data`, `Action` and recurring `Scope` (Occurrence, Following,
Series). Validate and persist in the callback, update your collection, or set `Cancel = true` to refuse.
The component never mutates the original `TItem`. Cancellation of the editor discards its working copy.
`EditorTemplate` can replace the standard fields via `SchedulerEditContext<TItem>` and its Save/Cancel methods.

The grid reuses shared drag, drop and resize controls. Timeline gestures use Gantt's engine. F2 edits,
Delete deletes, Ctrl+arrows moves and Ctrl+Shift+arrows resizes. Arrow keys, Home/End and Space navigate and
select slots. Recurring changes ask for their scope before submission. Focus is restored after edits.

`EventTemplate` receives the original item and projected occurrence. Keep its content noninteractive because
the surrounding control owns selection. `ViewTemplate` and `ToolbarTemplate` receive a
`SchedulerViewContext<TItem>` with the loaded snapshot and owner callbacks. Compose the public Calendar,
TimeGrid, Agenda and Timeline projections instead of duplicating loading or editing. `Resources` labels
Timeline rows; unknown resource keys remain visible.

## Themes and overrides

Every component token falls back to base semantic tokens at its point of use. Any theme that supplies the
base contract therefore works without Scheduler-specific settings. Optional overrides use the same
`[CssVar]` mapper as Gantt, Kanban and other independent packages:

```csharp
using Flare.Theming;
using Flare.Components.Scheduler;

var overrides = new SchedulerTokens
{
    EventSurface = "var(--flare-color-tertiary-container)",
    EventColor = "var(--flare-color-on-tertiary-container)"
}.FlattenTokens();
themeService.SetCustomTokens(overrides);
```

`FlattenTokens` returns the complete record, including its base defaults. Filter the map when applying only
a few keys over existing customizations. A custom theme can put the same map in `DesignTokens.Extended`.
Overrides inherit normally and can be scoped to a parent in consumer CSS. `ClearCustomToken` restores the
fallback for a service override. There is no Scheduler-specific inline-style conversion API.

The [Gallery](https://jrfrigat.github.io/Flare/components/scheduler) demonstrates editing, templates,
resources, a windowed 50,000-event source and optional recurrence.
