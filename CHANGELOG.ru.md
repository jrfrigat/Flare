# История изменений

Все значимые изменения Flare фиксируются здесь. Проект следует
[семантическому версионированию](https://semver.org/). Записи, отсутствующие в этом файле,
показываются на английском из `CHANGELOG.md`.

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
