# Flare.Components.Scheduler

Composable calendar UI for [Flare](https://github.com/jrfrigat/Flare), supporting .NET 8, 9 and 10.

```sh
dotnet add package Flare.Components.Scheduler
```

After AddFlare, call AddFlareScheduler() from Flare.Components.Scheduler. Load
_content/Flare.Components.Gantt/css/gantt.css and _content/Flare.Components.Scheduler/css/scheduler.css
after the core stylesheet. Use FlareThemeProvider; FlareDialogProvider is only needed for the
optional dialog presenter.

- Ready-made Scheduler or a headless scope with independent views, Surface and Toolbar.
- Day, Week, WorkWeek, Month, Year, Agenda and resource Timeline; existing Calendar/Gantt engines.
- Your TItem/Mapping, windowed cancellable data, application calendar filters and persistence.
- Independent editing session/form: Drawer, Dialog, Popover, inline or application-owned pages.
- Interactive event templates, shared drag/resize, date-only lanes and configurable +N overflow.
- Scoped live localization and culture; base tokens by default with typed CssVar overrides.
- Ready backend occurrences; optional local RFC expansion in Flare.Components.Scheduler.Ical.

[Guide and examples](https://github.com/jrfrigat/Flare/blob/main/docs/en/scheduler.md) ·
[Gallery](https://jrfrigat.github.io/Flare/components/scheduler) · [Русский](README.ru.md)
