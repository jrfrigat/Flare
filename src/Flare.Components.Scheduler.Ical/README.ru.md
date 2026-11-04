# Flare.Components.Scheduler.Ical

Необязательный локальный адаптер повторов RFC 5545 для Flare Scheduler на основе Ical.Net.
Нужен автономным клиентам, раскрывающим серии локально. Приложения с backend могут передавать
готовые вхождения без этого пакета.

```sh
dotnet add package Flare.Components.Scheduler.Ical
```

```razor
@using Flare.Components.Scheduler
@using Flare.Components.Scheduler.Ical

<FlareScheduler TItem="SchedulerEventData" Mapping="SchedulerMappings.EventData" Events="series"
                RecurrenceExpander="IcalSchedulerRecurrenceExpander.Default" />
```

Передайте `series` со стабильными ключами, датами, необязательными RRULE/RDATE/EXDATE и исключениями.
Раскрытие ограничено окном, отменой и лимитами количества/вычислений Scheduler. Адаптер не хранит
события, не разделяет серии и не реализует импорт/экспорт iCalendar.
Настройка, часовые пояса и контракт редактирования описаны в
[руководстве Scheduler](https://github.com/jrfrigat/Flare/blob/main/docs/ru/scheduler.md). [English](README.md).
