# Flare.Components.Gantt

Универсальный таймлайн для Flare: диаграмма Ганта, дорожки ресурсов, бронирования,
roadmap — все, что раскладывается во времени по строкам.

> Пакет выращивается в репозитории Alva (Alva STORY-21) и переносится во Flare (Flare EPIC-4). Он ссылается
> только на Flare и ничего не знает о приложении, которое его использует.

## Граница
- **Рисует и сообщает, но не считает предметную область.** Емкость, перегрузку, риски, расписание людей
  считает потребитель и передает готовые даты, значения и статусы.
- **Не меняет данные потребителя.** Любое действие пользователя приходит отменяемым событием с набором
  изменений; применяет их потребитель.
- **Встроенные расчеты** — только общая геометрия графа (`GanttGraph`): каскад зависимостей, критический
  путь, проверка циклов; плюс свертка сводных полос (`GanttRollup`).

## Подключение
```xml
<ProjectReference Include="..\Flare.Components.Gantt\Flare.Components.Gantt.csproj" />
```
```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
```

## Модель
| Тип | Что это |
| --- | --- |
| `GanttSpan` | Полуинтервал [Start, End). `Day`, `FromDays(первый, последний)` для целых дней, `FromOffsets(…, зона)` для моментов. |
| `GanttScale` | Шаг оси: Hour, Day, Week, Month, Quarter, Year. |
| `GanttSegment(Kind, Span, Label?)` | Часть полосы: работа, ожидание, стадия. |
| `GanttMarker(Kind, At, RowKey?, Label?)` | Точка во времени: веха, срок, «сегодня». Без `RowKey` — на всю высоту. |
| `GanttRegion(Kind, Span, RowKey?, Label?)` | Подсвеченный диапазон: спринт, отпуск, ТО. |
| `GanttCell(RowKey, Span, Kind, Text?, Title?)` | Значение строки на отрезке оси со статусом: часы в день, занятость. |
| `GanttLink(From, To, Type, Lag, Kind?)` | Связь FS / SS / FF / SF с лагом. |
| `GanttChange(ItemKey, Span, RowKey?)` | Новое положение элемента; так же задаются предложения (`Suggested`). |

Строки и элементы — **ваши собственные типы** с селекторами, оберток нет.
**Kind** — строка-ключ: компонент выводит ее в класс `flare-gantt--kind-{kind}` и атрибут `data-kind`, а
оформление задаете вы.

## Компоненты
- `FlareGantt` — общая ось:
  - `Range`, `Scale`, `UnitWidth`, `RowHeight`, `BarInset`, `SideWidth`, `Culture`;
  - `NonWorking` (функция от дня);
  - глобальные `Markers` и `Regions`;
  - `SelectedKey`/`SelectedKeyChanged`, `Compact`, `Corner`, `Texts`;
  - `FillWidth` — ось тянется на видимую ширину, `UnitWidth` становится минимумом;
  - `ResizableSide` + `SideWidthChanged` — колонку шапок тянут за правый край;
  - `ScrollTo(момент)` — прокрутить ось к дате;
  - `Virtualize` — рисовать только строки и колонки рядом с видимой частью (для сотен строк и длинной оси; окно сообщает браузер при прокрутке);
  - `Endless` + `@bind-Range` — бесконечная ось: у края она просит больший диапазон (`RangeChanged`), при росте влево держит тот же день на экране; `ScrollTo` к дате вне диапазона сам расширяет ось; Alt + колесо прокручивает ось по горизонтали.
- `FlareGanttPanel<TRow, TItem>` — одна проекция. Панелей внутри `FlareGantt` может быть сколько угодно: у
  всех одна шкала, одна прокрутка и общий выбор по ключу элемента.
  - **Строки:** `Rows`, `RowKey`, `RowChildren` (дерево), `RowHeader`, `RowClass`, `ExpandedByDefault`.
  - **Элементы:** `Items`, `ItemKey`, `ItemRow`, `ItemSpan`, `ItemKind`, `ItemSegments`, `ItemProgress`,
    `ItemLabel`, `ItemTemplate`, `ItemTooltip`, `OnItemClick`.
  - **Слои:** `Cells` + `CellTemplate`, `Regions`, `Markers`, `Links` + `LinkTypes`, `ShowCriticalPath`,
    `Suggested`, `RollupParents` (сводная полоса по потомкам у строк без своих элементов).
  - **Взаимодействие:** `Editable`, `ScheduleMode`, `AllowRowChange`, `NewLinkType`, `OnChange`,
    `OnLinkCreate`, `OnLinkRejected`, `DropGroup`, `ItemCover` (что полоса должна покрывать: растяжение упирается в его края).

### Две проекции на одной оси
```razor
<FlareGantt Range="@_range" Scale="GanttScale.Day" NonWorking="IsWeekend" @bind-SelectedKey="_selected">
    <FlareGanttPanel TRow="Node" TItem="Work" Rows="_tree" RowKey="n => n.Id" RowChildren="n => n.Children"
                     Items="_work" ItemKey="w => w.Id" ItemRow="w => w.NodeId" ItemSpan="w => w.Span"
                     Links="_links" RollupParents="true" />
    <FlareGanttPanel TRow="Person" TItem="Work" Rows="_people" RowKey="p => p.Id"
                     Items="_work" ItemKey="w => w.Id" ItemRow="w => w.PersonId" ItemSpan="w => w.Span"
                     Cells="_hoursPerDay" Editable="true" OnChange="Apply" />
</FlareGantt>
```

## Взаимодействие
- **Указатель:** перетаскивание полосы (время и строка), растяжение за края. Предпросмотр идет с привязкой
  к шагу оси; уход указателя с панели отменяет жест.
- **Клавиатура:**
  - Ctrl+←/→ — сдвиг;
  - Ctrl+Shift+←/→ — конец;
  - Ctrl+↑/↓ — строка;
  - Enter/Space — выбор;
  - L — начать связь, Enter или клик по цели — создать, Esc — отменить.
  - Сообщения идут в `aria-live`.
- **`OnChange`** получает `GanttChangeEventArgs`:
  - первичное изменение и каскад последователей по `ScheduleMode`;
  - `Cancel` отключает объявление.
- **Внешнее перетаскивание:** при `DropGroup` каждая строка — одна `FlareDropZone`; день под указателем подсвечивает ось.
  В обработчике вашего `FlareDragContext`: `await gantt.ResolveDrop(e.TargetId)` (`@ref` на `FlareGantt`) вернет строку и шаг оси — `GanttDropTarget`.
  - `ItemDropGroup` делает зоной сброса саму полосу (не сводную и не предложенную). Цель разбирается через `GanttItemDropTarget.TryParse`.
    Так можно, например, бросить человека на задачу.
  - `DropAccepts` и `ItemDropAccepts` решают, какую нагрузку берут шаги оси и полосы. Остальное не подсвечивается и сброс не принимает.
  - Пока идет перетаскивание, все в дорожке, что нагрузку не берет, пропускает указатель к зонам под собой. Поэтому задача,
    которую принимают только шаги оси, ложится и на день под чужой полосой.

## Оформление
- Все — CSS-переменные `--flare-gantt-*` с запасом из общих токенов Flare (`--flare-color-*`,
  `--flare-shape-*`, `--flare-spacing-*`).
- Вид конкретного Kind задается переменными на его классе:
```css
.flare-gantt--kind-waiting {
    --flare-gantt-bar-bg: transparent;
    --flare-gantt-bar-fg: var(--flare-color-error);
    --flare-gantt-bar-border: var(--flare-color-error);
}
.flare-gantt__cell.flare-gantt--kind-overload { --flare-gantt-cell-bg: var(--flare-color-error); }
```
Основные переменные:

| Группа | Переменные |
| --- | --- |
| Полоса | `bar-bg`, `bar-fg`, `bar-border`, `bar-radius` |
| Состояния полосы | `selected`, `critical`, `suggested`, `progress-bg`, `summary-bg` |
| Сегмент | `segment-bg`, `segment-border`, `segment-border-style` |
| Слои | `region-bg`, `region-border`, `marker`, `cell-bg`, `cell-fg`, `nonworking-bg` |
| Связь | `link`, `link-width`, `link-dash` |
| Каркас | `line`, `header-bg`, `side-bg`, `scale-height`, `max-height` |

## Перенос во Flare — что отложено
- Записи `GanttTokens` в теме вместо CSS-переменных (имена переменных совпадают с будущими токенами).
- Пересадка геометрии оси и раскладки на движок планировщика (Flare STORY-21), чтобы таймлайн был один.
- Виртуализация строк для тысяч элементов.
- Базовые линии в UI: модель уже допускает сегмент `Kind = "baseline"`.
- Через JS-порт Flare:
  - перевод фокуса стрелками;
  - подавление прокрутки при Ctrl+стрелках;
  - pointer capture;
  - создание связи перетаскиванием от края полосы.
- Проверка тача на устройстве; страницы Gallery; генерация `docs/api`.
