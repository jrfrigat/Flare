# Flare.Components.Scheduler.Ical

Optional local RFC 5545 recurrence adapter for Flare Scheduler, based on Ical.Net. For standalone clients
that need to expand series locally; server-backed applications can send ready occurrences without this package.

```sh
dotnet add package Flare.Components.Scheduler.Ical
```

```razor
@using Flare.Components.Scheduler
@using Flare.Components.Scheduler.Ical

<FlareScheduler TItem="SchedulerEventData" Mapping="SchedulerMappings.EventData" Events="series"
                RecurrenceExpander="IcalSchedulerRecurrenceExpander.Default" />
```

Supply `series` with stable keys, dates and optional RRULE, RDATE, EXDATE and override metadata.
Expansion is windowed, cancellable and bounded by Scheduler's occurrence/evaluation limits.
The adapter does not persist events, split series or implement an iCalendar import/export service.
See the [Scheduler guide](https://github.com/jrfrigat/Flare/blob/main/docs/en/scheduler.md)
for setup, time-zone semantics and the application-owned editing contract. [Русский](README.ru.md).
