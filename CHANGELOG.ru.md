# История изменений

Все значимые изменения Flare фиксируются здесь. Проект следует
[семантическому версионированию](https://semver.org/). Записи, отсутствующие в этом файле,
показываются на английском из `CHANGELOG.md`.

## [0.2.0] - 2026-07-13

Релиз про поля и слайдер: доработки по итогам сборки реальных приложений на Flare (админка Weir и плеер
PlaylistShared / Deka) - цветные зоны слайдера, клавиатурные события у семейства полей и два исправления
фокуса/видимости.

### Добавлено
- **Цветные зоны `FlareSlider`**: декларативный слот `<Zones>` с детьми `<FlareSliderZone Start End Color />`
  - статичные цветные области на дорожке (шкалы safe/warning/danger, полоса медиа-буфера или раскраска по
  шагам), каждая в своём `FlareColor`. Зоны рисуются под активной заливкой (которая всегда показывает
  текущее значение сверху) и работают в одиночном, диапазонном и вертикальном режимах.
- **Клавиатурные события у семейства полей**: `OnKeyDown` / `OnKeyUp` у `FlarePasswordField`,
  `FlareMaskedField`, `FlareTextArea` и `FlareNumericField` (проброс во внутренний input), так что паттерн
  «нажать Enter в поле пароля, чтобы отправить форму» работает без обработчика на обёртке.
  `FlareNumericField` вызывает их после встроенного шага стрелками ArrowUp/ArrowDown.

### Изменено
- **Разработка тем**: у `InputTokens` появились обязательные поля `FocusRing`, `FocusOutline` и
  `FocusOutlineOffset` - индикатор фокуса поля: либо box-shadow-кольцо, либо настоящий CSS-outline.
  Кастомные темы, создающие `InputTokens` напрямую, обязаны их задать; темы, наследующие встроенные через
  `with`, получают их автоматически.

### Исправлено
- **`FlareSwitch` в теме Visual Studio 2026** рисовался с «шариком» вкл-состояния, вылезающим за рельс:
  тема использовала размеры «шарика» из Material Design 3 (24px) на компактном рельсе 40x20. Теперь
  применяется геометрия Fluent v9 - «шарик» 14px, одинаковый в обоих состояниях, помещается в рельс.
- **Восстановлен индикатор фокуса поля**: у всех контролов на базе `FlareField` (`FlareField`,
  `FlarePasswordField`, `FlareTextArea`, `FlareNumericField`, `FlareSelect`, пикеры) не было индикации
  фокуса ни мышью, ни клавиатурой. Теперь на `:focus-within` рисуется layout-neutral индикатор,
  настраиваемый по теме и по варианту - box-shadow-кольцо (нижний active-индикатор или полное кольцо) или
  настоящий CSS-outline. MD3 и Fluent используют кольцо; Visual Studio - outline; варианты filled/outlined
  выбирают своё. Невалидные поля получают кольцо цвета ошибки.

## [0.1.9] - 2026-07-12

Полировочный релиз: доработки по итогам сборки реальных приложений на Flare (Weir dashboard и дизайн
PlaylistShared / Deka) - то, что раньше выражалось только через `Style`, стало полноценными параметрами.

### Добавлено
- **`FlareLayoutAppBar`**: `Height` (любая CSS-длина) и `Dense` (более узкий 48px бар для оконных /
  IDE-подобных оболочек) - оба задают токен `--flare-layout-appbar-height`; плюс отдельный токен
  `--flare-layout-appbar-bg`, чтобы приложение или тема могли поднять поверхность нава над канвой без
  инлайн-CSS.
- **Пикеры даты/времени**: `Autofocus` у `FlareDatePicker` / `FlareTimePicker` / `FlareDateTimePicker`
  (фокус на input при первом рендере), как в семействе редактируемых полей.

### Изменено
- **Высота sparkline у `FlareChart` теперь фиксированная в пикселях**: в режиме `Sparkline` `Height`
  пиннит CSS-высоту SVG (full-width, без масштабирования) вместо роста с шириной контейнера - настоящий
  контракт sparkline. Обычные чарты сохраняют аспект по ширине.

### Исправлено
- **Icon-only кнопки** (`FlareIconButton` и др.) смещали глиф на ~2px: optical-tuck (для иконки рядом с
  лейблом) не сбрасывался для случая без лейбла. Теперь одиночный глиф центрирован; у кнопок с иконкой и
  лейблом tuck сохранён.

## [0.1.8] - 2026-07-11

Релиз overlay/диалогов: доработки по кросс-фреймворк аудиту (vs MudBlazor / Blazorise / DevExpress /
Fluent UI Blazor) по всему семейству оверлеев — tooltip, меню, snackbar, popover и диалог.

### Добавлено
- **`FlareTooltip`**: `Delay` (задержка появления по наведению), независимые триггеры `ShowOnHover` /
  `ShowOnFocus` / `ShowOnClick`, `Arrow` (стрелка) и `Disabled` (rich-вариант теперь подключается при
  заданном `TooltipContent`).
- **Контекстное меню `FlareMenu`**: `Activation="RightClick"` превращает меню в контекстное (подавляя
  меню браузера), `PositionAtCursor` привязывает панель к курсору, `MaxHeight` прокручивает длинный
  список, а `FlareMenuItem.AutoClose="false"` держит меню открытым для переключаемых пунктов.
- **Snackbar**: `SnackbarOptions.PreventDuplicate` подавляет дубликаты, `ISnackbarService.Remove(id)` и
  `Clear()` программно гасят один или все, а `Show(RenderFragment, ...)` рендерит своё, компонентное
  содержимое вместо простого текста.
- **`FlarePopover`**: `Trigger="Hover"` (с `Delay` / `HideDelay`), прокрутка по `MaxHeight` и
  `MatchAnchorWidth` (ширина по якорю, как у выпадающего списка); `MinWidth` / `MaxWidth` теперь применяются.
- **`FlareDialog`**: `ShowCloseButton` (встроенный крестик в шапке), отменяемый гард `BeforeClose`
  (veto закрытия по scrim / Escape / крестику, например при несохранённых изменениях), а также
  `Draggable` + `Resizable` (перетаскивание за шапку, изменение размера за нижний правый угол).

### Изменено
- Диалоги теперь по умолчанию закрываются при навигации (`CloseOnNavigation`, отключаемо) — как в
  MudBlazor и Fluent UI Blazor.

## [0.1.7] - 2026-07-11

Релиз даты/времени и графиков: доработки пикеров по кросс-фреймворк аудиту и полная переработка
`FlareChart` (по-прежнему нативный SVG, zero-JS, на токенах темы).

### Добавлено
- **Пикеры даты/времени — публичный императивный API** у `FlareDatePicker` / `FlareTimePicker` /
  `FlareDateTimePicker`: `OpenAsync()`, `CloseAsync()`, `ToggleAsync()`, `ClearAsync()`, `FocusAsync()`,
  плюс события `Opened` / `Closed`.
- **`FlareDatePicker`**: `OpenTo` (Day/Month/Year — прыжок сразу на сетку года для дальних дат),
  `AutoClose`, `Inline` (всегда открытый календарь в потоке), `ShowWeekNumbers`, явный override
  `FirstDayOfWeek` и `DayClassFunc` для кастомного CSS по дням (праздники, подсветка).
- **`FlareTimePicker`**: `ShowSeconds`, `Min` / `Max` времени (ячейки вне диапазона отключаются) и `HourStep`.
- **`FlareChart` вырос с 4 до 13 типов** — добавлены `Area`, `StackedBar`, `Scatter`, `Bubble`,
  `Radar`, `HeatMap`, `Rose`, `PolarArea` и `Combo` (per-series `ChartSeriesKind` bar/line/area).
- **`FlareChart` спарклайн и заливки**: `Sparkline` (без обвязки, во всю ширину с чётким штрихом),
  `Area` (градиентная заливка), `Smooth` (сглаживание), `ShowMarkers`, и гранулярные тумблеры
  `ShowGrid` / `ShowLegend` / `ShowXAxisLabels` / `ShowYAxisLabels` / `LegendPosition` / `Padding`.
- **`FlareChart` конфиг и интерактив**: `YMin` / `YMax`, `YAxisFormat`, `XAxisTitle` / `YAxisTitle`,
  `Horizontal` бары, `ShowValues`, `OnPointClick`, `DonutRingRatio`, `BarWidthRatio`, интерактивная
  легенда (клик по метке скрывает серию), `TrendLine` (МНК-регрессия) и `Annotations`
  (линии порога / цели / полосы).
- **`FlareChart` полировка**: `Animate` (токен-driven CSS-анимация появления с учётом
  `prefers-reduced-motion` — то, чего нет у MudBlazor и что нативнее JS-анимации Chart.js) и `DataTable`
  (визуально скрытая таблица данных для скринридеров).

## [0.1.6] - 2026-07-11

Релиз семейства полей ввода: доработки по кросс-фреймворк аудиту (vs MudBlazor / Blazorise / Fluent UI
Blazor) по всему семейству текстовых полей.

### Добавлено
- **`FocusAsync()` по всему семейству редактируемых полей** — новая база `FlareEditableFieldBase` даёт
  `FlareField` / `FlareTextField`, `FlarePasswordField`, `FlareNumericField`, `FlareMaskedField` и
  `FlareTextArea` программный `FocusAsync()`; `FlareOtpField` фокусирует первую ячейку. Плюс `SelectAsync()` /
  `BlurAsync()` (и `SelectRangeAsync()` у `FlareField`) на трёх новых хелперах `IElementJsService`.
- **`Autofocus`** (фокус при первом рендере) и события **`OnFocus` / `OnBlur`** на текстовых полях.
- **`Pattern`** (regex) и **`InputMode`** у `FlareField` / `FlareTextField`; **`Autocomplete`** и
  **`Spellcheck`** на текстовых полях; **`DataList`** (нативные подсказки `<datalist>`).
- **`Clearable` на каждом редактируемом поле** (раньше только у `FlareField`) плюс **`OnClearButtonClick`**.
- **`FlareNumericField`**: публичные **`Increment()` / `Decrement()`** и **`SelectAllOnFocus`**.
- **`FlareTextArea`**: **`Resize`** (None/Vertical/Horizontal/Both) и **`Spellcheck`**.
- **`HelperTextOnFocus`** — показывает подсказку только при фокусе.

### Изменено
- **`FlareOtpField` теперь использует общую chrome поля** (`FlareFieldChrome`), как остальное семейство:
  получил `Label`, `HelperText` / `ErrorText` (реальная строка-сообщение, а не только bool `Error` для
  красных ячеек), `Required`, `ReadOnly` и валидацию `EditContext` / `For` — ошибка также краснит ячейки.
  Сам ряд ячеек не изменился.

## [0.1.5] - 2026-07-11

Релиз семейства кнопок: доработки по кросс-фреймворк аудиту (vs MudBlazor / Blazorise / Fluent UI Blazor)
по всему семейству кнопок.

### Добавлено
- **`FlareButton`**: `FocusAsync()` (программный фокус через захваченный `ElementReference`),
  `LoadingTemplate` (кастомный контент загрузки вместо спиннера по умолчанию) и явный `Rel`.
- **`FocusAsync()` по всему семейству кнопок** — `FlareIconButton`, `FlareToggleButton`, `FlareSplitButton`
  (фокус на основное действие) и `FlareFloatingActionButton`.
- **`ButtonEdge`** (`None`/`Start`/`End`) у `FlareIconButton` и `FlareToggleButton` — оптическое
  выравнивание к краю (отрицательный inline-margin) для аппбаров, тулбаров и слотов элементов списка.
- **`FlareToggleButton.Toggle()` / `SetToggledAsync(bool)`** — программное управление переключением.
- **`FlareToggleGroup.Mandatory`** (single-select нельзя очистить) и **`CheckMark`** (галочка на выбранном).
- **`FlareSplitButton`**: `Loading`, `FullWidth`, `Href`/`Target`/`Rel` (основное действие как ссылка),
  `Placement` (`MenuAnchor`) и публичные `Open()` / `Close()`. `FlareMenu` получил публичные
  `OpenAsync()` / `CloseAsync()`.
- **`FlareIconButton`**: `OnColor` и override `Rel`.

### Изменено
- **Кнопки-ссылки по умолчанию ставят `rel="noopener noreferrer"` при `Target="_blank"`** (`FlareButton`,
  `FlareIconButton`, `FlareSplitButton`) — защита от reverse-tabnabbing через `window.opener`. Переопределяется
  новым параметром `Rel`.
- **У `FlareFloatingActionButton` и `FlareFloatingActionMenuItem` `OnClick` теперь
  `EventCallback<MouseEventArgs>`** (был безаргументный `EventCallback`) — для консистентности с остальным
  семейством кнопок.

## [0.1.4] - 2026-07-11

### Добавлено
- **`FlareDescriptionList` / `FlareDescriptionItem`** — read-only панель ключ/значение (read-only аналог
  `FlareDataGrid`), рендерится как семантический `<dl>` в две колонки, чтобы метки и значения были
  выровнены по строкам независимо от ширины контента. Опции `Striped`, `Bordered` и `LabelWidth`; каждый
  элемент принимает простой `Label` или богатый `LabelContent`, а списки вкладываются размещением
  `FlareDescriptionList` внутрь значения.
- **`FlareCode`** — тематический инлайновый `<code>`-чип (моноширинный на приглушённом surface-container
  тональном чипе, extra-small радиус) для code-токена в тексте, совпадает с рецептом инлайн-кода рендерера
  Markdown.
- **`FlareText.Mono`** — переключает `FlareText` на моноширинный шрифт, сохраняя метрики шкалы;
  `code`, `kbd`, `samp` и `pre` добавлены в allow-list элементов, так что `<FlareText Element="kbd" Mono>`
  рисует реальные клавиши.
- **`FlareFileUpload.Variant`** (`FileUploadVariant.DropZone | Button`) — компактная кнопочная форма,
  открывающая диалог файлов ОС без drop-зоны, переиспользуя тот же список выбранных файлов, плюс
  локализованный `ButtonText`.
- **`FlareGrid.AutoFit`** — с заданным `MinColumnWidth` выдаёт `repeat(auto-fit, ...)` вместо `auto-fill`,
  чтобы карточки растягивались по строке без пустых хвостовых треков.
- **`FlareStack.StretchLast`** — зеркало `StretchFirst`: растёт только последний ребёнок, для фиксированной
  ведущей рейки рядом с панелью, заполняющей остальное.
