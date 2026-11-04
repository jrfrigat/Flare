# Scheduler

`FlareScheduler<TItem>` добавляет календарное планирование поверх модели приложения. Month и Year
используют `FlareCalendar`, ресурсный Timeline использует `FlareGantt`. Day, Week, WorkWeek и Agenda
разделяют окно загрузки, проекцию событий и контракт редактирования. Calendar и Gantt остаются самостоятельными.

## Подключение

```sh
dotnet add package Flare.Components.Scheduler
```

После обычной настройки Flare и темы зарегистрируйте общие службы шкалы:

```csharp
using Flare.Components.Scheduler;
builder.Services.AddFlareScheduler();
```

После основного CSS подключите оба файла:

```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
<link rel="stylesheet" href="_content/Flare.Components.Scheduler/css/scheduler.css" />
```

Нужны стандартные `FlareThemeProvider` и `FlareDialogProvider`: второй размещает редактор события.
Scheduler требует интерактивного рендеринга. Ограничьте высоту родителя для прокрутки сетки времени.

## Своя модель

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

Наследование и изменяемые обертки не нужны. Для `SchedulerEventData` есть готовая
`SchedulerMappings.EventData`. Ключи должны быть уникальными и стабильными. Временные интервалы включают
Start и исключают End. События на весь день используют независимые от часового пояса границы `DateOnly`
в `SchedulerDateRange`; End также исключается.

`Date`/`DateChanged` и `View`/`ViewChanged` позволяют приложению управлять навигацией. Без callbacks состояние
ведет компонент. `Views` ограничивает список видов. `Culture` задает формат дат и начало недели, тексты
интерфейса следуют `CultureInfo.CurrentUICulture` с английским запасным вариантом. `TimeZone` меняет показ
моментов времени, но не даты событий на весь день. При переходе DST отсутствующие слоты пропускаются,
повторенные часы сохраняют разные смещения. Рабочие часы выделяются фоном;
`InitialScrollTime` по умолчанию равен `WorkStart`.

## Загрузка окна и повторы

Вместо `Events` задайте `EventsProvider`. Запрос содержит полуоткрытое `Window` и cancellation token.
Верните все события, пересекающие окно, включая начавшиеся раньше. Устаревший ответ игнорируется даже
если provider не соблюдает отмену. `ReloadAsync` обновляет данные после внешних изменений,
`OnLoadError` сообщает ошибки.

По умолчанию backend передает готовые вхождения. Для редактирования повторов передавайте стабильные
`SeriesKey` и `RecurrenceId`. UI не разбирает правила и не обращается к backend самостоятельно.
Для автономного клиента можно установить `Flare.Components.Scheduler.Ical` и явно задать
`RecurrenceExpander="IcalSchedulerRecurrenceExpander.Default"` из пространства имен
`Flare.Components.Scheduler.Ical`. Адаптер использует Ical.Net для RRULE/RDATE/EXDATE и исключений.
Только этот необязательный пакет добавляет RFC-зависимость. Лимиты и отмена ограничивают локальное
раскрытие; хранение, правила и разделение серии остаются ответственностью приложения.

## Редактирование и композиция

Включите `Editable` и обработайте `OnChange`. `SchedulerChangeEventArgs<TItem>` содержит исходный объект,
идентификатор вхождения, неизменяемое предложение `Data`, `Action` и область `Scope`: Occurrence,
Following или Series. Проверьте и сохраните изменение, обновите свою коллекцию либо установите
`Cancel = true`. Компонент никогда не меняет исходный `TItem`. Отмена редактора отбрасывает рабочую копию.
`EditorTemplate` заменяет стандартные поля через `SchedulerEditContext<TItem>` и его методы Save/Cancel.

Сетка использует общие drag/drop/resize-компоненты, Timeline использует жесты Gantt. F2 редактирует,
Delete удаляет, Ctrl+стрелки переносит, Ctrl+Shift+стрелки меняет длительность. Стрелки, Home/End и Space
перемещают фокус и выбирают слот. Для повторов перед отправкой выбирается область изменения.
После редактирования фокус восстанавливается.

`EventTemplate` получает исходный объект и проекцию вхождения. Не вкладывайте в него интерактивные
элементы: выбором управляет окружающий control. `ViewTemplate` и `ToolbarTemplate` получают
`SchedulerViewContext<TItem>` с загруженным снимком и callbacks владельца. Публичные проекции Calendar,
TimeGrid, Agenda и Timeline позволяют менять представление без копирования загрузки и редактирования.
`Resources` задает подписи строк Timeline; неизвестные ключи ресурсов также остаются видимыми.

## Темы и переопределения

Компонентные токены обращаются к базовым семантическим токенам в месте использования. Любая тема,
задающая базовый контракт, работает без настроек Scheduler. Для переопределений используется общий
mapper `[CssVar]`, как в Gantt, Kanban и других независимых пакетах:

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

`FlattenTokens` возвращает всю запись, включая базовые значения. Отфильтруйте карту, если нужно изменить
несколько ключей поверх существующей настройки. Собственная тема может положить карту в
`DesignTokens.Extended`. Переопределения наследуются; CSS потребителя может ограничить их родителем.
`ClearCustomToken` возвращает сервисное переопределение к базовому значению. Отдельного преобразования
Scheduler в inline style нет.

[Gallery](https://jrfrigat.github.io/Flare/components/scheduler) показывает редактирование, шаблоны,
ресурсы, оконный источник на 50 000 событий и необязательные повторы.
