# Flare.Components.Scheduler

Calendar planning for [Flare](https://github.com/jrfrigat/Flare). Supports .NET 8, 9 and 10.

```sh
dotnet add package Flare.Components.Scheduler
```

After `AddFlare`, call `AddFlareScheduler()` from `Flare.Components.Scheduler`. Add the Gantt and Scheduler
stylesheets: `_content/Flare.Components.Gantt/css/gantt.css` and
`_content/Flare.Components.Scheduler/css/scheduler.css`. Use the normal theme and dialog providers.

- Day, Week, WorkWeek, Month, Year, Agenda and resource Timeline.
- Your own `TItem` with `SchedulerMapping<TItem>`; windowed, cancellable `EventsProvider`.
- Existing Calendar/Gantt projections; shared drag, drop and resize controls.
- Application-owned persistence through cancellable `OnChange`, including recurrence scope.
- Base theme tokens by default, optional `[CssVar]` `SchedulerTokens` overrides.
- Ready backend occurrences by default. Optional local RFC expansion is a separate
  `Flare.Components.Scheduler.Ical` package; it is not a dependency of this package.

[Guide and examples](https://github.com/jrfrigat/Flare/blob/main/docs/en/scheduler.md) ·
[Gallery](https://jrfrigat.github.io/Flare/components/scheduler) · [Русский](README.ru.md)
