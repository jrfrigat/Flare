# Flare.Components.Scheduler

Календарное планирование для [Flare](https://github.com/jrfrigat/Flare), .NET 8, 9 и 10.

```sh
dotnet add package Flare.Components.Scheduler
```

После `AddFlare` вызовите `AddFlareScheduler()` из `Flare.Components.Scheduler`. Подключите стили
`_content/Flare.Components.Gantt/css/gantt.css` и `_content/Flare.Components.Scheduler/css/scheduler.css`.
Нужны стандартные providers темы и диалогов.

- Day, Week, WorkWeek, Month, Year, Agenda и ресурсный Timeline.
- Собственный `TItem` через `SchedulerMapping<TItem>`, оконный `EventsProvider` с отменой.
- Проекции существующих Calendar/Gantt, общие drag/drop/resize-компоненты.
- Хранение в приложении через отменяемый `OnChange`, включая область изменения серии.
- Базовые токены темы по умолчанию, необязательные записи `SchedulerTokens` с `[CssVar]`.
- Готовые вхождения с backend по умолчанию. Локальное раскрытие RFC подключается отдельным
  пакетом `Flare.Components.Scheduler.Ical`, зависимости от него здесь нет.

[Руководство](https://github.com/jrfrigat/Flare/blob/main/docs/ru/scheduler.md) ·
[Gallery](https://jrfrigat.github.io/Flare/components/scheduler) · [English](README.md)
