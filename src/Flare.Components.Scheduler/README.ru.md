# Flare.Components.Scheduler

Конструктор календарного UI для [Flare](https://github.com/jrfrigat/Flare), .NET 8, 9 и 10.

```sh
dotnet add package Flare.Components.Scheduler
```

После AddFlare вызовите AddFlareScheduler() из Flare.Components.Scheduler. После основного CSS подключите
_content/Flare.Components.Gantt/css/gantt.css и _content/Flare.Components.Scheduler/css/scheduler.css.
Используйте FlareThemeProvider; FlareDialogProvider нужен только необязательному диалоговому presenter.

- Готовый Scheduler или headless scope с независимыми видами, Surface и Toolbar.
- Day, Week, WorkWeek, Month, Year, Agenda и ресурсный Timeline; движки существующих Calendar/Gantt.
- Собственная модель/Mapping, оконная загрузка с отменой, фильтры календарей и хранение в приложении.
- Независимые сессия и форма: Drawer, Dialog, Popover, inline или страница приложения.
- Интерактивные шаблоны событий, общие drag/resize, date-only уровни и настраиваемый +N.
- Scoped-переключение языка и культура; базовые токены с типизированными CssVar overrides.
- Готовые вхождения backend; локальное раскрытие RFC отдельно в Flare.Components.Scheduler.Ical.

[Руководство и примеры](https://github.com/jrfrigat/Flare/blob/main/docs/ru/scheduler.md) ·
[Gallery](https://jrfrigat.github.io/Flare/components/scheduler) · [English](README.md)
