# Flare.Components.Gantt

Лента времени для библиотеки компонентов [Flare](https://github.com/jrfrigat/Flare): дерево задач, люди,
переговорные или станки на одной оси времени. Пакет-дополнение к `Flare.Components`.

```sh
dotnet add package Flare.Components.Gantt
```

## Подключение

```csharp
builder.Services.AddFlare();
builder.Services.AddFlareGantt();
```

```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
```

Нужна тема `Flare.Theme.*`: внешний вид ленты - это токены темы `GanttTokens`.

## Граница

- **Рисует и сообщает, но не считает предметную область.** Емкость, перегрузку, риски и расписание считает
  приложение и передает готовые даты, значения и статусы.
- **Не меняет данные приложения.** Любое действие пользователя приходит отменяемым событием с предложенными
  положениями; применяет их приложение.
- **Встроенные расчеты** - только геометрия графа (`GanttGraph`): каскад последователей, критический путь, проверка
  циклов; плюс свертка сводных полос (`GanttRollup`).

## Модель

| Тип | Что это |
| --- | --- |
| `GanttSpan` | Полуинтервал [Start, End). `Day`, `FromDays(первый, последний)` для целых дней, `FromOffsets(..., зона)` для моментов. |
| `GanttScale` | Шаг оси: Hour, Day, Week, Month, Quarter, Year. |
| `GanttSegment(Kind, Span, Label?)` | Часть полосы: работа, ожидание, стадия. |
| `GanttMarker(Kind, At, RowKey?, Label?)` | Точка во времени: веха, срок, сегодня. Без `RowKey` - на всю высоту. |
| `GanttRegion(Kind, Span, RowKey?, Label?)` | Подсвеченный диапазон: спринт, отпуск, ТО. |
| `GanttCell(RowKey, Span, Kind, Text?, Title?)` | Значение строки на отрезке оси со статусом: часы в день, занятость. |
| `GanttLink(From, To, Type, Lag, Kind?)` | Связь FS / SS / FF / SF с лагом. |
| `GanttChange(ItemKey, Span, RowKey?)` | Новое положение элемента; так же задаются предложения (`Suggested`). |

Строки и элементы - **ваши собственные типы** с селекторами, оберток нет. **Kind** - строка-ключ: она выводится в
класс `flare-gantt--kind-{kind}` и атрибут `data-kind`.

## Компоненты

- `FlareGantt` - общая ось: `Range`, `Scale`, `UnitWidth`, `RowHeight`, `BarInset`, `SideWidth`, `Culture`,
  `NonWorking`, общие `Markers` и `Regions`, `SelectedKey`/`SelectedKeyChanged`, `Compact`, `Corner`, `Texts`,
  `FillWidth`, `Virtualize` (рисует только строки и колонки рядом с видимой частью), `Endless` + `@bind-Range`,
  `ResizableSide` + `SideWidthChanged` (край колонки - `FlareSplitter`), `ScrollTo(момент)`.
- `FlareGanttPanel<TRow, TItem>` - одна проекция. Панели на одной оси делят шкалу, прокрутку и выбор по ключу
  элемента. Строки (`Rows`, `RowKey`, `RowChildren`, `RowHeader`, `RowClass`), элементы (`Items`, `ItemKey`, `ItemRow`,
  `ItemSpan`, `ItemKind`, `ItemSegments`, `ItemProgress`, `ItemLabel`, `ItemTemplate`, `ItemTooltip`, `OnItemClick`),
  слои (`Cells`, `Regions`, `Markers`, `Links`, `ShowCriticalPath`, `Suggested`, `RollupParents`), редактирование
  (`Editable`, `ScheduleMode`, `AllowRowChange`, `ItemCover`, `NewLinkType`, `OnChange`, `OnLinkCreate`,
  `OnLinkRejected`, `DropGroup`, `ItemDropGroup`).
- `FlareGanttLegend` - легенда, образцы которой - те же элементы, что рисует график.

## Взаимодействие

- **Указатель:** перетаскивание полосы (время и строка), растяжение за края, связь - перетаскиванием точки на конце
  полосы на другую полосу. Предпросмотр рисует браузер, в .NET один вызов в конце жеста. Escape отменяет.
- **Клавиатура:** Ctrl+стрелки влево/вправо - сдвиг, Ctrl+Shift+влево/вправо - конец, Ctrl+вверх/вниз - строка
  (фокус остается на полосе), Enter/Space - выбор, L - начать связь, Enter или клик по цели - создать, Escape -
  отменить. Сообщения идут в `aria-live`.
- **Внешнее перетаскивание:** при `DropGroup` каждая строка - одна `FlareDropZone`; шаг оси под указателем дает
  `await gantt.ResolveDrop(e.TargetId)`. `ItemDropGroup` делает зонами сами полосы (`GanttItemDropTarget`).

## Оформление и тексты

- Все оформление - токены `--flare-gantt-*` (`GanttTokens`), которые задает тема. Вид одного Kind меняется теми же
  переменными на его классе:

```css
.flare-gantt--kind-waiting {
    --flare-gantt-bar-bg: transparent;
    --flare-gantt-bar-border: var(--flare-color-error);
}
```

- Тексты берутся из ресурсов пакета (английский и русский) в культуре графика `Culture`. Любой можно заменить:
  `Texts="new GanttTexts { Moved = ... }"`; остальные остаются.

## Переход с инкубатора Alva

Пакет вырос в репозитории Alva. Отличия от той копии: обязателен `AddFlareGantt()`; `GanttTexts` - необязательные
замены вместо целых наборов `English`/`Russian`; колонку шапок меняет `FlareSplitter` (кнопок «уже/шире/сброс»
больше нет, сброс - Home или двойной щелчок, `SideWidthChanged` сообщает пиксели); уход указателя с графика больше
не отменяет перетаскивание (отменяет Escape); оформление - токены темы `GanttTokens` вместо запасных значений в CSS.

Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
