# Scheduler

Scheduler предоставляет детали календарного интерфейса поверх модели приложения. Можно использовать
готовую сборку или самостоятельно расположить виды, навигацию, действия и формы. Month/Year используют
FlareCalendar, Timeline использует FlareGantt, жесты сетки используют общие Draggable, DropZone и
Resizable. Calendar и Gantt остаются самостоятельными компонентами.

## Подключение и готовая сборка

Установите Flare.Components.Scheduler, выполните обычную настройку Flare и темы и зарегистрируйте:

```csharp
using Flare.Components.Scheduler;
builder.Services.AddFlareScheduler();
```

После основного CSS подключите:

```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
<link rel="stylesheet" href="_content/Flare.Components.Scheduler/css/scheduler.css" />
```

Используйте FlareThemeProvider и, для необязательного диалогового presenter, FlareDialogProvider.
Интерактивный режим включает команды и жесты; статический серверный рендеринг показывает данные без JS.
Задайте ограниченную высоту родителя, если сетка времени должна прокручиваться.

```razor
@using Flare.Components.Scheduler

<FlareStack Height="36rem">
    <FlareScheduler TItem="SchedulerEventData" Mapping="SchedulerMappings.EventData"
                    Events="events" Editable OnChange="Persist" />
</FlareStack>

@code {
    private List<SchedulerEventData> events = [];
    private void Persist(SchedulerChangeEventArgs<SchedulerEventData> change)
    {
        if (change.Original is { } original) events.Remove(original.Item);
        if (change.Action != SchedulerEditAction.Delete) events.Add(change.Data);
    }
}
```

FlareScheduler - отдельный scope данных и взаимодействий. Без ChildContent он собирает публичные
Surface, Toolbar и Dialog. При наличии ChildContent выводится только заданная композиция. В своей
сборке передавайте визуальные Class/Style/атрибуты выбранной поверхности. Сам scope не зависит от
IDialogService.

## Переход с 0.52.0

Используйте готовую сборку выше, если нужны стандартные представления, панель инструментов и редактор.
В собственной сборке перенесите прежние корневые параметры в соответствующие компоненты.

| В 0.52.0 | В 0.53.0 |
| --- | --- |
| `ViewTemplate` | Выведите представление в `FlareScheduler.ChildContent` или замените переключение видов через `FlareSchedulerSurface.ChildContent`. |
| `ToolbarTemplate` и `ShowToolbar` | В собственной сборке выведите свою панель, добавьте `FlareSchedulerToolbar` для стандартных команд или опустите панель. |
| `Views` | Задайте `FlareSchedulerToolbar.Views`, чтобы выбрать пункты переключателя представлений. |
| `EditorTemplate` | Передайте содержимое редактора в `FlareSchedulerDialog.ChildContent` или покажите `context.Edit` в своей компоновке. См. [пример с боковой панелью](#своя-поверхность-редактирования). |
| Корневой `EventTemplate` | Задайте `EventTemplate` на поверхности или отдельном представлении. Читайте событие через `context.Occurrence`, а для его основного действия используйте `FlareSchedulerEventAction`. |
| `AllDayHeight` | Используйте `AllDayRowHeight` (`--flare-scheduler-all-day-row-height`), чтобы задать минимальную высоту строки событий на весь день. |

Выбор слота обновляет выделение. Чтобы открыть сессию редактирования, вызовите
`context.Commands.CreateEventAsync(slot)` из действия создания. Сохраняйте принятые изменения в `OnChange`.

## Составные части

| Часть | Ответственность |
| --- | --- |
| FlareScheduler | Оконные данные, навигация, выделение, запросы редактирования, граница сохранения |
| SchedulerViewContext / ISchedulerCommands | Снимок и команды без конкретного визуального владельца |
| FlareSchedulerSurface | Необязательная рамка, Header/Footer, состояния и один активный вид |
| FlareSchedulerToolbar | Необязательные стандартные команды; параметр Views находится здесь |
| FlareSchedulerTimeGrid | Колонки Day/Week/WorkWeek, слоты, линия текущего времени и жесты |
| FlareSchedulerAllDay | Независимые уровни событий на весь день и действия переполнения |
| FlareSchedulerCalendar | Month/Year на сетке Calendar |
| FlareSchedulerAgenda | Хронологический список |
| FlareSchedulerEvent | Общая обертка interaction, drag и resize события |
| FlareSchedulerEventAction | Основное действие выбора и клавиатуры для своих шаблонов |
| FlareSchedulerTimeline | Виртуальные строки ресурсов с осью и редактированием Gantt |
| SchedulerEditContext | Независимый черновик, проверка и жизненный цикл Save/Cancel |
| FlareSchedulerEditor | Необязательная переиспользуемая форма без зависимости от диалога |
| FlareSchedulerDialog | Необязательный показ этой же сессии в диалоге |

Каждый вид получает Context. Собственный host может предоставить снимок и реализовать
ISchedulerCommands; конкретный FlareScheduler не нужен. Surface.ChildContent заменяет переключение
видов. Header/Footer и LoadingTemplate/ErrorTemplate/EmptyTemplate меняют окружение. Пустой вид
остается доступным для выбора, пока EmptyTemplate намеренно его не заменит. Скрытые виды не монтируются.

## Своя поверхность редактирования

Полная композиция ниже открывает стандартную форму в правом Drawer по клику на событие. Headless-сборка
не добавляет встроенный presenter. Рядом можно разместить sidebar, mini-calendar, фильтры приложения,
preview и собственные поля.

```razor
@using Flare.Components.Scheduler

<FlareScheduler TItem="SchedulerEventData" Mapping="SchedulerMappings.EventData"
                @ref="scheduler" Events="events" Editable OnChange="Persist" OnEventSelected="OpenEvent">
    <ChildContent Context="schedule">
        <FlareStack Height="36rem">
            <FlareSchedulerSurface TItem="SchedulerEventData" Context="schedule">
                <Header Context="navigation">
                    <FlareButton OnClick="navigation.Commands.TodayAsync">Today</FlareButton>
                    <FlareButton OnClick="() => Create(navigation)">New event</FlareButton>
                    <FlareText>@navigation.Caption</FlareText>
                </Header>
            </FlareSchedulerSurface>
        </FlareStack>
        @if (schedule.Edit is { } edit)
        {
            <FlareDrawer Open="true" OpenChanged="open => open ? Task.CompletedTask : edit.CancelAsync()"
                         Anchor="DrawerAnchor.Right" Width="min(30rem,100vw)"
                         CloseOnEsc="@(!edit.IsSaving)" CloseOnScrimClick="@(!edit.IsSaving)"
                         ContentPadding="FlareSpacing.Medium" AriaLabel="Event details">
                <FlareSchedulerEditor TItem="SchedulerEventData" Context="edit" />
            </FlareDrawer>
        }
    </ChildContent>
</FlareScheduler>

@code {
    private FlareScheduler<SchedulerEventData>? scheduler;
    private Task OpenEvent(SchedulerOccurrence<SchedulerEventData> occurrence) => scheduler!.EditEventAsync(occurrence);
    private static Task Create(SchedulerViewContext<SchedulerEventData> context)
    {
        var start = SchedulerClock.At(context.Date, new(9, 0), context.TimeZone);
        return context.Commands.CreateEventAsync(context.SelectedSlot ?? new(start, start.AddHours(1)));
    }
    private List<SchedulerEventData> events = [];
    private void Persist(SchedulerChangeEventArgs<SchedulerEventData> change)
    {
        if (change.Original is { } original) events.Remove(original.Item);
        if (change.Action != SchedulerEditAction.Delete) events.Add(change.Data);
    }
}
```

Встроенные подписи локализованы; подписи приложения из примера локализуйте в своем приложении.
В готовой сборке OnEditRequested передает показ обработчику и подавляет стандартный Dialog.
Храните эту сессию в своем Drawer, Popover, inline-форме или странице. Команды завершаются после
запроса сессии, а не после закрытия пользователем.

Выделение отделено от редактирования. OnEventSelected может показать preview либо явно вызвать
EditEventAsync. Клик или Space по слоту только выделяет интервал. Double-click/F2 запрашивает
создание; стандартная кнопка создания использует выделенный интервал. Для touch можно выбрать
интервал и нажать действие создания. All-day - свойство общей формы, отдельная кнопка в каждой
колонке не обязательна.

Сессия хранит неизменяемую запись Draft, Original, Action и область повторов Scope. Для изменения
полей заменяйте Draft через with. SaveAsync возвращает false при невалидных данных, отказе, ошибке
или повторной отправке и сохраняет черновик для новой попытки. Status описывает причину, Error
содержит исключение сохранения. Changed уведомляет отдельные формы. CancelAsync отбрасывает сессию
без изменения исходного объекта. Во время IsSaving повторные Save и Cancel игнорируются; блокируйте
закрытие до окончания запроса. Стандартный presenter запрещает закрытие во время сохранения.
Принятое сохранение закрывает сессию. Форму можно вывести непосредственно на странице.
Editor.ChildContent заменяет поля и действия; с самой сессией можно работать без Editor.

OnChange получает неизменяемое предложение Data, Original/идентификатор вхождения, Action и Scope.
Сохраните данные и обновите коллекцию либо установите Cancel. Библиотека не меняет TItem.
Переносы/resize повторов и удаление могут запрашивать Scope/подтверждение через ту же сессию.
Обычные переносы/resize отправляются сразу и сохраняют выбранный пользовательский editor flow.
Отказ жесту возвращает геометрию/фокус без открытия формы; OnChangeError сообщает ошибку жеста.
Ошибки редактора остаются в его сессии.

## События и представления

EventTemplate у вида/Surface заменяет визуальное содержимое внутри границы геометрии/drag/resize.
Он получает SchedulerEventContext: Occurrence, View, IsSelected, SelectAsync/EditAsync/DeleteAsync
и HandleKeyAsync. Вложенные интерактивные элементы допустимы: обязательной окружающей кнопки нет.
Для стандартного основного действия используйте FlareSchedulerEventAction с контекстом шаблона, а вторичные действия разместите рядом. Он использует типографику событий и перенос подписей; ChildContent меняет неинтерактивную подпись.
Для других основных controls задайте HandleKeyAsync и data-flare-edit-keys, чтобы
сохранить F2/Delete/модифицированные стрелки и блокировку нативной прокрутки. Не назначайте эти команды
вложенным полям и вторичным меню. Восстановление фокуса выбирает первый доступный интерактивный элемент.

Стандартная кнопка события поддерживает F2, Delete, Ctrl+стрелки для переноса и Ctrl+Shift+стрелки
для resize. Timeline использует шаг оси и команды ресурсов Gantt. Стрелки, Home/End и Space работают
со слотами, включая RTL и повторенные часы DST. Содержимое события должно помещаться в выделенную
временную геометрию; большие подробности показывайте в отдельной поверхности.

TimeGrid предоставляет DayHeaderTemplate, неинтерактивный SlotTemplate, ShowAllDay и AllDayTemplate.
Последний получает SchedulerTimeGridContext с View и точными видимыми Dates, включая рабочую неделю.
Calendar предоставляет заголовки месяца, дня недели и даты. Scheduler использует
Calendar.EventContainerTemplate, чтобы не вкладывать действия в стандартный control Calendar.
Timeline использует Gantt.ItemInteractionTemplate с сохранением команд объекта/оси.
Timeline также предоставляет Scale, UnitWidth, RowHeight и RowHeaderTemplate. Стандартная виртуализация требует измерения viewport; для небольшого статического серверного вида задайте Virtualize=false.
Отмечайте вторичные действия бара атрибутом data-gantt-no-drag; поля сохраняют нативное редактирование.

## Полоса событий на весь день

```razor
<FlareSchedulerTimeGrid TItem="SchedulerEventData" Context="schedule">
    <AllDayTemplate Context="band">
        <FlareSchedulerAllDay TItem="SchedulerEventData" Context="band.View" Dates="band.Dates"
                             MaxRows="3" />
    </AllDayTemplate>
</FlareSchedulerTimeGrid>
```

Общая полоса согласована с временными колонками и рисует много-дневное событие одним обрезанным
сегментом. Соприкасающиеся exclusive end делят уровень. По умолчанию полоса растет без внутренней
прокрутки. MaxRows добавляет +N; стандартное действие раскрывает/сворачивает полосу.
Expanded/ExpandedChanged позволяют управлять раскрытием извне. OnOverflow передает дату и скрытые
вхождения для своего Drawer, Popover или Agenda; его ShowAsync по-прежнему может раскрыть полосу.
OverflowTemplate заменяет действие. HeaderTemplate/SlotTemplate/SegmentTemplate настраивают
независимую полосу. Полный SegmentTemplate владеет interaction/drag/keyboard; EventTemplate сохраняет
стандартные обертки. ScrollOverflow с MaxHeight явно включает одну прокрутку всей полосы.
Отключите ShowAllDay у TimeGrid, чтобы разместить независимую полосу отдельно. AllDayRowHeight -
минимум, который не фиксирует высоту и не обрезает увеличенный текст.

## Данные, культура и темы

SchedulerMapping<TItem> подключает собственную модель без наследования. Стабильные ключи
идентифицируют события. Временные интервалы используют Start/End с instant-семантикой, all-day
SchedulerDateRange использует независимые DateOnly. Все End исключающие. Resources - строки
ресурсов, а не календари. CalendarId, права и бизнес-поля храните в модели приложения и фильтруйте
Events/запросы provider там же. Сохраняйте эти поля при применении предложений move/resize.

Date/DateChanged и View/ViewChanged поддерживают controlled-состояние; без callbacks навигация
внутренняя. Culture определяет форматирование и начало недели. UI использует scoped
[локализацию](localization.md) и стандартные переопределения IStringLocalizer<SchedulerStrings>.
Смена языка в браузере сохраняет сессию, черновик и фокус. Явный Culture остается приоритетным.
TimeZone меняет отображение instants, но не all-day даты. Отсутствующие DST-слоты пропускаются,
повторенные часы сохраняют разные смещения. InitialScrollTime по умолчанию равен WorkStart.

EventsProvider получает полуоткрытое Window и cancellation token. Включайте все пересекающие окно
события, даже начавшиеся раньше. Устаревшие результаты игнорируются независимо от соблюдения отмены.
ReloadAsync обновляет внешние изменения; OnLoadError и Surface.ErrorTemplate обрабатывают ошибки.
Оконные снимки и кэшированные индексы дней/слотов/уровней не перечитывают источник при рендерах
часов/выделения. MaximumOccurrences/MaximumEvaluationSteps ограничивают локальную работу.

Готовые вхождения backend работают без RFC-зависимости. SeriesKey/RecurrenceId сохраняют идентичность
серии. Flare.Components.Scheduler.Ical - необязательный локальный адаптер RRULE/RDATE/EXDATE;
явно задайте RecurrenceExpander=IcalSchedulerRecurrenceExpander.Default. Хранение, правила повторов,
разделение серии, приглашения и workflow остаются задачами приложения/backend.

Токены Scheduler по умолчанию используют семантические базовые токены. SchedulerTokens с [CssVar]
преобразуется общим mapper, как в остальных независимых пакетах:

```csharp
using Flare.Theming;
using Flare.Components.Scheduler;
using static Flare.Css.Tokens.Vars;

var values = new SchedulerTokens
{
    EventSurface = Var(Flare.Css.Tokens.Color.TertiaryContainer),
    EventColor = Var(Flare.Css.Tokens.Color.OnTertiaryContainer)
}.FlattenTokens();
themeService.SetCustomTokens(values);
```

FlattenTokens включает значения записи по умолчанию: отфильтруйте ключи для частичного изменения
поверх уже заданных overrides. Темы используют DesignTokens.Extended. Наследование CSS ограничивает
потребительские стили родителем; ClearCustomToken убирает сервисное переопределение. Публичные
константы CSS-классов обозначают части компонентов.

[Gallery](https://jrfrigat.github.io/Flare/components/scheduler) показывает готовую сборку,
sidebar/фильтры/preview/правый Drawer, быстрые и полные inline-формы с бизнес-полями,
внешний all-day overflow, оконные данные и необязательные повторы.
