# Доказательный архитектурный и визуальный аудит Flare

Дата проверки: 1 сентября 2026 года  
Репозиторий: [jrfrigat/Flare](https://github.com/jrfrigat/Flare)  
Проверенная ревизия `main`: [`ebef333479a95fd83c7537fa37ba1e2adfff0e72`](https://github.com/jrfrigat/Flare/commit/ebef333479a95fd83c7537fa37ba1e2adfff0e72), версия `0.26.2`  
Состояние GitHub на момент аудита: [0 открытых и 0 закрытых issue](https://github.com/jrfrigat/Flare/issues?q=is%3Aissue); [1 открытый PR](https://github.com/jrfrigat/Flare/pulls?q=is%3Apr+is%3Aopen), не связанный с предметом аудита.

## 1. Краткий вывод

Основные пары, которые выглядят дублирующимися, в действительности отвечают разным уровням абстракции:

- `FlareAccordion` — координатор группы панелей, а `FlareCollapse` — автономный disclosure-контейнер. Публичные компоненты объединять не следует. Повторно используются только действительно общие части оформления заголовка; однако реализация анимации и несколько контрактов состояния разошлись и породили конкретные дефекты.
- `FlareScrollTop` — готовый UI-компонент, а `IScrollService` — низкоуровневая инфраструктура чтения, наблюдения и изменения scroll position. `FlareScrollTop` уже корректно построен поверх сервиса, поэтому дублирования старого JS-кода нет. Есть дефект реакции компонента на изменение параметров после первого render и противоречие в обещаниях о количестве JS-listener.
- Вводить новый общий `DataExporter` только из-за названия `DataGridExport` сейчас не обосновано. Экспортёры уже вызываются независимо от grid через публичные `DataGridExportData<T>`, `DataGridExportColumn<T>` и `IDataGridExporter<T>`, а `DataGridExport` является UI-адаптером. Нужнее документация standalone-сценария. Реальные проблемы здесь — некорректное кодирование TSV и Markdown и отсутствие тестов на управляющие/форматные символы.
- Невыровненные `FlareTextField` и `FlareSelect` в Barcode-примере — ошибка Flare, а не требование MD3 Expressive, не ограничение Blazor и не проблема `FlareStack`. У неё две независимые причины: medium-размер Select/MultiSelect обходит общий input-token, а TextField/TextArea создаёт пустой supporting-text row.

По итогам подготовлено 10 issue-кандидатов с высокой уверенностью. Ни один не был создан: ниже находится полный текст для предварительного согласования.

## 2. Методика и границы проверки

Проверено:

- актуальное состояние `origin/main`, история изменений и `CHANGELOG.md`;
- публичные Razor API, CSS, темы/tokens, JS interop и тесты;
- документация по архитектуре и controlled/uncontrolled contract;
- live gallery Barcode на desktop и narrow viewport, во всех доступных design systems, включая MD3 Expressive, MD3, Fluent UI 2 и Aero;
- состояния default/filled/outlined, default/explicit `Md`, отсутствие и наличие supporting text; влияние focus/error/disabled установлено по CSS box model;
- существующие GitHub issues и PR по словам `accordion`, `collapse`, `scroll`, `export`, `TSV`, `field`, `select`;
- первичные внешние источники: WAI-ARIA APG, WAI-ARIA 1.2, официальная Material Web документация, GFM и OWASP.

Проверки тестов:

- `Flare.Components.Tests`: 2351 passed, 0 failed, 0 skipped (`net10.0`, Release);
- `Flare.Core.Tests`: 160 passed, 0 failed, 0 skipped (`net10.0`, Release).

Важно: успешная текущая suite подтверждает отсутствие общего regression, но не опровергает найденные проблемы — для каждой из них установлено конкретное отсутствие соответствующей проверки.

Репозиторий не изменён. После тестов `git status --short` пуст.

## 3. Таблица решений

| Область | Решение | Что подтверждено | Issue |
|---|---|---|---|
| Accordion / Collapse | Разные публичные компоненты | group coordinator против standalone disclosure; разная визуальная семантика | Не объединять |
| Accordion state | Дефект | внешнее изменение `Expanded` игнорируется после первой инициализации | Да, P1 |
| Accordion guard | Дефект | auto-collapse обходит `OnBeforeToggle` | Да, P1 |
| Accordion animation | Дефект | раскрытый контент ограничен `max-height: 2000px`, Collapse уже умеет произвольную высоту | Да, P2 |
| Accordion ARIA | Дефект | toggle button не находится в heading | Да, P2/A11y |
| Headerless Collapse ARIA | Дефект | безымянный `role=region` | Да, P2/A11y |
| ScrollTop / ScrollService | Разные уровни; reuse правильный | UI-адаптер подписывается и вызывает сервис | Не объединять |
| ScrollTop parameters | Дефект | runtime-изменения selector/throttle/threshold не синхронизируют subscription/UI | Да, P1/P2 |
| Scroll listener ownership | Контракт противоречив | интерфейс и код — per subscription; remarks/JS header — per target | Да, P3 |
| DataGrid export / DataExporter | Общая абстракция уже существует, хотя названа grid-oriented | DTO + interface + exporters работают без `FlareDataGrid`; есть `DataProvider` | Новый `DataExporter` пока нет |
| TSV/Markdown | Дефект | tabs/newlines/pipes и spreadsheet-active prefixes не кодируются | Да, P1 |
| Barcode field alignment | Два дефекта Flare | Select Md padding не из token; TextField создаёт пустой support row | Два issue, P1 |
| OnThisPage / TableOfContents | Хороший reuse | collector непосредственно рендерит presentation-компонент | Нет |
| Progress / Meter | Разная семантика, общий foundation | внешний progress против суммы сегментов; общие track/zone primitives | Нет |
| Tabs/TOC direct scroll JS | Специализированная логика | вычисляет локальное состояние/геометрию и не повторяет универсальный сервис | Нет |

## 4. Accordion и Collapse

### 4.1. Почему это не дубли

`FlareAccordion` владеет групповой политикой `AllowMultiple` и координирует панели через cascading parent. При открытии панели он закрывает ранее открытую панель в single-expand режиме ([FlareAccordion.razor, строки 10–30](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Accordion/FlareAccordion.razor#L10-L30)). `FlareAccordionPanel` всегда имеет собственный header button и является дочерним элементом группы.

`FlareCollapse`, напротив, документирован в самом исходнике как один автономный регион, который может иметь встроенный заголовок либо управляться внешним toggle ([FlareCollapse.razor, строки 4–39](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Collapse/FlareCollapse.razor#L4-L39)). Он не знает о соседях и не задаёт group policy.

Это соответствует двум разным accessibility patterns:

- [WAI-ARIA Accordion Pattern](https://www.w3.org/WAI/ARIA/apg/patterns/accordion/) описывает набор связанных headers/panels;
- [WAI-ARIA Disclosure Pattern](https://www.w3.org/WAI/ARIA/apg/patterns/disclosure/) описывает один button, показывающий или скрывающий content.

Раздельные tokens тоже намеренны: комментарий в `CollapseTokens` прямо объясняет, что accordion — filled section внутри bordered container, а collapse — transparent standalone expander ([CollapseTokens.cs, строки 5–10](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Abstractions/Tokens/Components/CollapseTokens.cs#L5-L10)). История `CHANGELOG` фиксирует такое разделение начиная с ранних версий и выделение отдельных token families в `0.14`.

Правильный уровень reuse уже выбран: общая inline-layout разметка header text объединена одним CSS-rule, но spacing остаётся component-specific ([accordion.css, строки 64–79](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/accordion.css#L64-L79)). Оборачивать `FlareCollapse` внутрь публичного `FlareAccordionPanel` не следует: это смешает state ownership, ARIA и визуальные tokens. Внутренняя primitive для анимации disclosure была бы оправдана.

### 4.2. Найденные проблемы

#### A. `FlareAccordionPanel` нарушает общий bindable-state contract

Панель копирует `Expanded` во внутреннее `_expanded` только один раз и затем игнорирует изменения параметра ([FlareAccordionPanel.razor, строки 52–68](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Accordion/FlareAccordionPanel.razor#L52-L68)). Это противоречит задокументированному Flare-контракту: controlled-компонент с callback должен следовать параметру, uncontrolled-компонент — локальному состоянию до реального изменения параметра ([component-conventions.md, строки 141–170](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/docs/ru/component-conventions.md#L141-L170)). `FlareCollapse` реализует этот контракт через `_lastExpanded` ([FlareCollapse.razor, строки 61–78](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Collapse/FlareCollapse.razor#L61-L78)).

Тесты проверяют полный controlled/uncontrolled contract для Collapse, но для Accordion проверяют только initial value и пользовательские клики. External rerender с изменённым `Expanded` отсутствует.

#### B. Auto-collapse обходит `OnBeforeToggle`

Обычный `Toggle` вызывает guard до изменения state ([FlareAccordionPanel.razor, строки 70–82](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Accordion/FlareAccordionPanel.razor#L70-L82)). Но при открытии sibling родитель вызывает `CollapseAsync`, который сразу устанавливает `_expanded = false` без guard ([FlareAccordionPanel.razor, строки 85–93](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Accordion/FlareAccordionPanel.razor#L85-L93)). Поэтому заявленный use case «подтвердить закрытие панели с несохранёнными изменениями» не работает в наиболее важном single-expand сценарии.

Корректная семантика при veto: старая панель остаётся открытой, а новая не открывается — иначе `AllowMultiple=false` становится ложным.

#### C. Accordion обрезает высокий контент

Accordion анимирует `max-height` от `0` до theme token ([accordion.css, строки 98–105](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/accordion.css#L98-L105)); текущий token равен `2000px`. Контент выше этого значения остаётся обрезанным даже в expanded state. Collapse уже использует `grid-template-rows: 0fr → 1fr`, явно предназначенный для произвольной высоты ([collapse.css, строки 75–88](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/collapse.css#L75-L88)). Это хороший кандидат для общей внутренней disclosure-animation primitive.

#### D. Accordion header не имеет heading semantics

Сейчас header — голый `<button>` ([FlareAccordionPanel.razor, строки 5–21](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Accordion/FlareAccordionPanel.razor#L5-L21)). WAI-ARIA APG требует, чтобы accordion header button был единственным содержимым элемента с `heading` role и подходящим `aria-level` либо нативного heading. Нужен configurable heading level; жёстко выбирать `h3` библиотеке нельзя, поскольку правильный уровень зависит от документа.

Каждый panel также всегда получает `role=region`. APG считает этот landmark опциональным и советует избегать proliferation при большом числе одновременно раскрываемых панелей. Это стоит учесть в дизайне исправления, но отдельный issue без user case не нужен.

#### E. Headerless Collapse создаёт безымянный region

`FlareCollapse` всегда выводит `role="region"`, но `aria-labelledby` существует только при встроенном header ([FlareCollapse.razor, строки 32–39](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Collapse/FlareCollapse.razor#L32-L39)). По [WAI-ARIA 1.2 для role=region](https://www.w3.org/TR/wai-aria/#region) region должен иметь accessible name. Для headerless mode следует либо не создавать landmark, либо позволить передать `aria-label`/`aria-labelledby` и выводить роль только при наличии имени.

### 4.3. Противоположные гипотезы

**H1: Accordion с заголовком и Collapse с заголовком — дубли.** Не подтверждена. Одинаковый внешний affordance скрывает разные contracts: group coordination против standalone external control.

**H2: это разные компоненты, но они должны совместно использовать всё внутреннее.** Подтверждена частично. Header layout уже переиспользован. Общей должна стать intrinsic height animation; state machine и ARIA ownership должны остаться раздельными.

## 5. ScrollTop и ScrollService

### 5.1. Архитектура

`IScrollService` — DI-entry point для чтения позиции, подписки, программного scrolling и reference-counted body lock ([IScrollService.cs](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Abstractions/Abstractions/Scroll/IScrollService.cs)). `FlareScrollTop` — opinionated button: подписывается на сервис, показывает себя после threshold и вызывает `ScrollToTopAsync` ([FlareScrollTop.razor, строки 40–63](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/ScrollTop/FlareScrollTop.razor#L40-L63)). Это правильная dependency direction; старой отдельной JS-логики ScrollTop больше нет. `CHANGELOG` версии `0.21` прямо фиксирует замену старого пути на `IScrollService`.

Гипотеза о неверном initial visibility не подтвердилась: `FireImmediately` по умолчанию включён, сервис получает фактическую стартовую позицию и немедленно вызывает handler ([ScrollService.cs, строки 64–79](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Infrastructure/JsInterop/ScrollService.cs#L64-L79)).

### 5.2. Runtime-параметры `FlareScrollTop`

Subscription создаётся только на первом render. Если parent затем меняет `Selector` либо `ThrottleMs`, существующий listener остаётся на старой цели/с прежним throttle. При этом click читает текущее значение `Selector`, поэтому наблюдаемая и прокручиваемая цели могут разойтись. Изменение `Threshold` тоже не пересчитывает visibility до следующего scroll event.

Текущие utility tests проверяют markup, defaults и отдельное значение `Threshold`, но не подменяют `IScrollService` и не проверяют lifecycle subscription.

### 5.3. Противоречие listener-sharing contract

Здесь существуют две несовместимые версии архитектуры:

- публичный интерфейс обещает «one throttled JS listener per subscription» ([IScrollService.cs, строки 15–19](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Abstractions/Abstractions/Scroll/IScrollService.cs#L15-L19));
- комментарий JS обещает «one listener per scroll target, fanned out to every C# subscriber» ([flare-scroll.js, строки 1–5](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/js/flare-scroll.js#L1-L5)); тот же тезис есть в remarks `ScrollService` и комментарии теста;
- фактически `_subs` хранится по subscription id, и каждый `subscribe()` вызывает `listen(...)` отдельно ([flare-scroll.js, строки 29–56](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/js/flare-scroll.js#L29-L56)).

Две допустимые гипотезы:

- per-target fan-out был задуман, но не реализован;
- per-subscription listener выбран сознательно, потому что subscriptions могут иметь разный `ThrottleMs`, а старые comments не обновлены.

Публичный интерфейс совпадает с фактическим поведением, поэтому это нельзя уверенно назвать performance bug. Это высокоуверенный contract/documentation defect с открытым архитектурным решением. Issue должен потребовать выбрать и протестировать один контракт, а не заранее навязать sharing.

Прямые scroll listeners в Tabs, Table of Contents и overlay/highlighter не являются автоматически дублями сервиса. Tabs сообщает только изменение нескольких локальных booleans и объединяет scroll с `ResizeObserver`; TOC сканирует bounding rects headings и подавляет повторяющиеся значения; overlay/highlighter целиком остаются в JS. Перевод этих путей в generic .NET scroll stream увеличил бы interop traffic и не убрал бы специализированную DOM-логику.

## 6. DataGridExport и идея DataExporter

### 6.1. Что уже можно делать без DataGrid

Публичный контракт состоит из:

- `DataGridExportColumn<TItem>` — название и extractor значения;
- `DataGridExportData<TItem>` — rows, columns, filename;
- `IDataGridExporter<TItem>` — exporter, принимающий только этот data object и `IFlareDownload` ([IDataGridExporter.cs](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/DataGrid/IDataGridExporter.cs)).

Стандартные CSV/TSV/JSON/Markdown/Excel/PDF exporters не принимают `FlareDataGrid` или `DataGridContext`. `FlareDataGrid.GetExportData()` только строит snapshot из видимых columns/rows ([FlareDataGrid.Export.cs, строки 25–46](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/DataGrid/FlareDataGrid.Export.cs#L25-L46)). `DataGridExport` является toolbar/UI adapter и имеет `DataProvider`, полностью обходящий owner grid ([DataGridExport.razor, строки 70–83](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/DataGrid/DataGridExport.razor#L70-L83)). Unit tests вручную создают `DataGridExportData<Row>` и вызывают exporters без grid.

Следовательно, сценарии «выгрузить DTO/report rows из сервиса» и «выгрузить табличную проекцию данных chart/query» уже реализуемы. Проблема здесь прежде всего naming/discoverability, а не отсутствующая архитектурная возможность.

У зрелых библиотек export также часто остаётся grid capability: [Telerik Blazor Grid export](https://www.telerik.com/blazor-ui/documentation/components/grid/export/overview) предоставляет CSV/Excel/PDF на уровне Grid, а [Syncfusion DataGrid export](https://blazor.syncfusion.com/documentation/datagrid/excel-export-options) позволяет передать custom data source. Это не доказывает, что Flare никогда не нужен более общий API, но опровергает тезис «отдельный DataExporter обязателен по стандартной архитектуре».

### 6.2. Рекомендация

Сейчас не добавлять новый `DataExporter`, потому что он либо станет пустым alias существующего `IDataGridExporter<T>`, либо потребует заранее угадать контракт для нетабличных данных.

Небьющий следующий шаг:

1. Добавить documentation example «standalone tabular export» с ручным `DataGridExportData<T>` и `IFlareDownload`.
2. Если появятся два и более реальных non-grid consumers, рассмотреть нейтральные names (`TabularExportData`, `TabularExporter`) с compatibility aliases для grid API.
3. Не смешивать в один abstraction табличные formats и произвольные domain formats: у них разные schema, MIME, encoding и security rules.

Отдельный issue на новый abstraction пока был бы solution-first и низкоуверенным, поэтому в список готовых к созданию issue не включён.

### 6.3. Подтверждённые дефекты exporter-ов

`TsvGridExporter` соединяет значения через `\t` и строки через newline без encoding policy ([TsvGridExporter.cs, строки 16–24](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/DataGrid/Exporters/TsvGridExporter.cs#L16-L24)). Значение с tab/newline меняет число columns/rows. В отличие от CSV, нет защиты от spreadsheet formulas. [OWASP CSV Injection](https://owasp.org/www-community/attacks/CSV_Injection) отмечает опасные начальные символы `=`, `+`, `-`, `@`, tab и CR/LF; правило применимо к любому текстовому формату, открываемому spreadsheet-программой.

`MarkdownExporter` вставляет raw values между `|` ([MarkdownExporter.cs, строки 14–23](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/DataGrid/Exporters/MarkdownExporter.cs#L14-L23)). По [GFM tables extension](https://github.github.com/gfm/#tables-extension-) pipe внутри cell должен быть escaped, а raw line break разрушает row.

Текущие тесты содержат только простые значения и проверку extension/content basics. Tabs, newlines, pipes и active formula prefixes отсутствуют.

JSON exporter строит dictionaries по column title, поэтому duplicate titles могут приводить к потере данных. Это реальное ограничение, но пока не issue-кандидат: неизвестно, запрещает ли DataGrid duplicate titles и какой JSON schema ожидается как публичный контракт.

## 7. Визуальная проблема Barcode: TextField и Select

### 7.1. Что измерено

Пример: [Barcode live gallery](https://jrfrigat.github.io/Flare/components/barcode#barcode-live).

`FlareStack Row Wrap Align="End"` делает именно то, что задано: выравнивает нижние края дочерних root boxes. Корневые boxes имеют разную высоту, поэтому labels и control tops расходятся. Замена `Align` лишь переместила бы видимый разрыв; она не исправила бы box model.

Измерения desktop 1280×720, default state:

| Тема | TextField root / control | Select root / control | Наблюдение |
|---|---:|---:|---|
| MD3 Expressive | 82 / 54 px | 72 / 48 px | root +10 px, control +6 px |
| MD3 | 82 / 54 px | 72 / 48 px | тот же разрыв |
| Fluent UI 2 | 74 / 46 px | 70 / 46 px | control совпадает, root TextField +4 px |
| Aero | 76 / 51 px | 64 / 43 px | root +12 px, control +8 px |

На viewport 375×812 controls переходят на отдельные строки, но их высоты остаются различными; проблема маскируется отсутствием соседнего baseline, а не исчезает.

Filled/outlined меняют paint, radius и focus ring, но не medium padding. Focus выполнен layout-neutral через shadow/outline ([input.css, строки 30–52 и 98–107](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/input.css#L30-L52)); disabled/error также не меняют box dimensions. При одинаковом helper/error обеим полям добавляется одинаковый support row, но 6/8 px control mismatch остаётся.

MD3/MD3 Expressive спецификация Flare задаёт text field и select field одной высоты 56dp. Это согласуется и с официальной Material Web моделью: [Select tokens](https://github.com/material-components/material-web/blob/main/docs/components/select.md) относятся к той же text-field token family, что и [Text Field](https://github.com/material-components/material-web/blob/main/docs/components/text-field.md). Поэтому наблюдаемое расхождение не является требованием MD3 Expressive.

### 7.2. Причина 1: medium Select/MultiSelect обходит token

Обычный input применяет `padding: var(--flare-input-padding-md)` безусловно ([input.css, строки 114–133](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/input.css#L114-L133)). Select и MultiSelect вместо этого имеют base rule `padding: var(--flare-spacing-6) var(--flare-spacing-8)` ([select.css, строки 10–23](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/select.css#L10-L23)).

Shared size grid явно перечисляет Select/MultiSelect для `Xs`, `Sm`, `Lg`, `Xl`, но не для `Md` ([input.css, строки 151–168](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/input.css#L151-L168)). `FlareFieldChrome.SizeClass()` для `Md` возвращает `null`, поэтому default и явный `Size="Md"` не получают modifier ([FlareFieldChrome.razor, строки 122–129](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Combobox/FlareFieldChrome.razor#L122-L129)). В MD3 input token равен `1rem 1rem`, а hardcoded Select base — `.75rem 1rem`, что ровно объясняет 6 px разницы по высоте.

Это family-wide defect для `FlareSelect` и `FlareMultiSelect` в default/explicit `Md`; `FlareCombobox` использует обычный input control и не затронут.

### 7.3. Причина 2: пустой supporting-text row

`FlareFieldChrome` считает support существующим, если `CounterContent is not null`, и тогда выводит container ([FlareFieldChrome.razor, строки 20–33 и 93–105](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Combobox/FlareFieldChrome.razor#L20-L33)). `FlareField` всегда передаёт named `CounterContent` fragment, хотя внутри него `_showCounter` может быть false ([FlareField.razor, строки 83–88](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/Input/FlareField.razor#L83-L88)). RenderFragment ненулевой, но ничего не рендерит. Поэтому остаётся пустой `.flare-input__support`, а column `gap: var(--flare-spacing-2)` добавляет 4 px ([input.css, строки 6–15](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/wwwroot/css/input.css#L6-L15)). `FlareTextArea` повторяет ту же конструкцию ([FlareTextArea.razor, строки 56–61](https://github.com/jrfrigat/Flare/blob/ebef333479a95fd83c7537fa37ba1e2adfff0e72/src/Flare.Components/TextArea/FlareTextArea.razor#L56-L61)).

Fluent UI 2 маскирует первую проблему, потому что его medium token случайно совпадает с hardcoded spacing Select, но пустой row всё равно оставляет 4 px разницы. Это подтверждает независимость двух причин.

### 7.4. Почему текущие тесты не поймали проблему

- `FieldChromeCompositionTests` проверяет единый frame и support с непустым helper, но не отсутствие пустого support.
- `FieldChromeGuardTests` запрещает отдельным fields дублировать frame, но не анализирует условные fragments.
- `FieldSizeRampTests` проверяет монотонность token values, но не то, что default `Md` реально потребляет token каждый структурно особый control.
- Нет browser-level geometry assertion для representative family row.

## 8. Поиск похожих архитектурных проблем

### Корректные пары

`FlareOnThisPage` и `FlareTableOfContents` разделены правильно: первый собирает headings, второй отображает готовую модель; collector непосредственно рендерит presentation-компонент. Это образцовый reuse вместо копирования.

`FlareProgress` и `FlareMeter` имеют похожую дорожку, но различную семантику: progress получает внешний 0–100 value, meter определяет целое суммой segments и zones. Они совместно используют track sizing/zone infrastructure; слияние ухудшило бы API.

`FlareDrawer` и layout drawer различаются ownership: standalone overlay против зарегистрированной области layout shell. Повторяющиеся визуальные affordances сами по себе не доказывают duplication.

### Систематический сигнал

Наиболее полезная общая закономерность аудита: Flare хорошо унифицирует публичный markup через внутренние frames/contexts, но default value без CSS modifier и «ненулевой, но пустой RenderFragment» обходят статические guard-тесты. Рекомендуется добавить два вида invariant tests:

1. Для каждого `FieldSize` проверять computed consumption token всеми структурными controls, включая default `Md`.
2. Для optional named fragments проверять не только `fragment != null`, но и фактическое условие присутствия content либо передавать fragment условно.

## 9. Приоритетный список issue-кандидатов

| № | Приоритет | Confidence | Предлагаемый title |
|---:|---|---|---|
| 1 | P1 | High | `FlareAccordionPanel ignores external Expanded changes after initialization` |
| 2 | P1 | High | `Accordion single-expand auto-collapse bypasses OnBeforeToggle` |
| 3 | P1 | High | `Default Md Select and MultiSelect bypass the shared input padding token` |
| 4 | P1 | High | `TextField and TextArea render an empty supporting-text row when the counter is hidden` |
| 5 | P1 | High | `TSV and Markdown exporters do not encode format-significant cell content` |
| 6 | P2 | High | `FlareScrollTop does not react to runtime Selector, ThrottleMs, or Threshold changes` |
| 7 | P2 | High | `Accordion content is clipped above the ContentMaxHeight token` |
| 8 | P2 | High | `Accordion headers lack heading semantics` |
| 9 | P2 | High | `Headerless FlareCollapse emits an unnamed region landmark` |
| 10 | P3 | High (defect), Medium (chosen fix) | `ScrollService listener-sharing contract contradicts its implementation` |

## 10. Полные черновики issue

Черновики написаны на английском, поскольку код, XML docs, issue templates и история PR проекта ведутся преимущественно на английском.

### Issue 1

**Title:** `FlareAccordionPanel ignores external Expanded changes after initialization`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> `FlareAccordionPanel` copies the `Expanded` parameter into its local `_expanded` state only on the first parameter set. Later external updates to `Expanded` do not update the rendered `aria-expanded` state or panel visibility.
>
> This differs from Flare's documented controlled/uncontrolled state contract and from `FlareCollapse`, which follows the parameter whenever a change callback is bound and also recognizes genuine parameter changes in uncontrolled mode.
>
> Evidence on `0.26.2` / `ebef333`:
>
> - one-time synchronization: `FlareAccordionPanel.razor`, `OnParametersSet`, lines 58–68;
> - expected contract: `docs/ru/component-conventions.md`, lines 141–170;
> - reference implementation: `FlareCollapse.razor`, lines 68–78.
>
> ## Minimal reproduction
>
> 1. Render a panel with `Expanded="false"` and an `ExpandedChanged` callback.
> 2. Re-render the parent with `Expanded="true"`.
> 3. Observe that the panel button still has `aria-expanded="false"` and the content remains collapsed.
>
> The same failure can be captured as a bUnit `SetParametersAndRender` test.
>
> ## Expected behavior
>
> `FlareAccordionPanel` follows the same bindable-state contract as `FlareCollapse`:
>
> - controlled mode follows `Expanded` on every parameter set;
> - uncontrolled mode keeps local interaction state until the parameter itself actually changes;
> - sibling auto-collapse must not be undone merely by an unrelated parent re-render.
>
> ## Acceptance criteria
>
> - Implement controlled and uncontrolled synchronization without changing the public API.
> - Add tests parallel to `ControlledStateContractTests` for initial state, local toggle, unrelated parent re-render, and genuine external `Expanded` change.
> - Keep `ExpandedChanged` behavior correct during user toggle and accordion-driven collapse.
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; render-mode/browser independent; reproduced from component lifecycle and bUnit-level behavior on .NET 10.

### Issue 2

**Title:** `Accordion single-expand auto-collapse bypasses OnBeforeToggle`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> `OnBeforeToggle` is called for a direct click on a panel, but it is not called when the parent accordion auto-collapses that panel after a sibling opens in `AllowMultiple="false"` mode.
>
> `FlareAccordion.PanelExpandedAsync` calls `CollapseAsync()` on the old panel. `CollapseAsync()` changes `_expanded` to false and fires `ExpandedChanged(false)` without invoking the documented guard. As a result, a panel that vetoes closing because it contains unsaved edits is still closed by opening a sibling.
>
> ## Reproduction
>
> 1. Render a single-expand accordion with panel A initially expanded and panel B collapsed.
> 2. Give panel A `OnBeforeToggle` that returns `false` when the proposed state is `false`.
> 3. Open panel B.
> 4. Panel A closes without its guard being consulted.
>
> ## Expected behavior
>
> Every transition from expanded to collapsed uses the same guard semantics. If panel A vetoes auto-collapse, panel A remains expanded and panel B must not commit its expansion; otherwise `AllowMultiple="false"` would be violated.
>
> ## Acceptance criteria
>
> - Route both direct and accordion-driven transitions through one guarded state transition.
> - Make the parent coordination aware of a veto and keep single-expand invariants deterministic.
> - Fire `ExpandedChanged` only for transitions that actually commit.
> - Add a bUnit test for vetoed sibling auto-collapse and a positive test for an allowed auto-collapse.
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; render-mode/browser independent; .NET 10.

### Issue 3

**Title:** `Default Md Select and MultiSelect bypass the shared input padding token`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> `FlareSelect` and `FlareMultiSelect` use hard-coded base spacing for their default/explicit `Md` trigger instead of `--flare-input-padding-md`. The shared size rules cover Xs, Sm, Lg, and Xl, but Md intentionally emits no size modifier, so the base rule wins.
>
> In MD3 and MD3 Expressive this makes the Select trigger 48px high while the same-size TextField control is 54px in the current rendered box model. The public Barcode example shows the resulting misalignment. Fluent UI 2 happens to hide the control-height part because its Md token equals the hard-coded spacing; other themes do not.
>
> ## Reproduction
>
> Open `components/barcode#barcode-live` in MD3 Expressive at desktop width and inspect the default fields:
>
> - TextField field box: 54px;
> - Select trigger: 48px;
> - Text input uses `padding: var(--flare-input-padding-md)`;
> - Select/MultiSelect use `padding: var(--flare-spacing-6) var(--flare-spacing-8)`.
>
> Explicit `Size="Md"` behaves the same because `FlareFieldChrome.SizeClass()` returns no modifier for Md.
>
> ## Expected behavior
>
> Every structural control in the shared field family consumes the same theme-provided token for the same `FieldSize`. MD3/MD3 Expressive medium text and select fields should retain the theme's intended 56dp family sizing.
>
> ## Acceptance criteria
>
> - Make default and explicit Md Select/MultiSelect consume `--flare-input-padding-md`.
> - Preserve Xs/Sm/Lg/Xl behavior.
> - Verify Filled and Outlined variants and default/focus/error/disabled states without layout shifts.
> - Add a guard test that checks token consumption for every structurally distinct field control, including modifier-less Md.
>
> ## Environment
>
> Flare gallery `0.26.2`, commit `ebef333`; interactive WebAssembly; reproduced in the Codex in-app Chromium browser at 1280×720 and 375×812 across MD3 Expressive, MD3, Fluent UI 2, and Aero.

### Issue 4

**Title:** `TextField and TextArea render an empty supporting-text row when the counter is hidden`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> `FlareField` and `FlareTextArea` always supply a non-null `CounterContent` RenderFragment to `FlareFieldChrome`. The fragment renders no node when `_showCounter` is false, but `FlareFieldChrome.HasSupport` only checks whether the fragment is non-null.
>
> Therefore a field with no helper, no error, and no visible counter still renders an empty `.flare-input__support` element. The shared column gap adds 4px to its root height. A Select without supporting text does not render this row, which is the second independent cause of the Barcode example's alignment problem.
>
> ## Reproduction
>
> Render a default `FlareTextField` or `FlareTextArea` without `HelperText`, `ErrorText`, `ShowCharacterCount`, or a positive `MaxLength`. Inspect the DOM: an empty `.flare-input__support` is present.
>
> ## Expected behavior
>
> The support container is absent when there is no actual helper, error, or counter content. It appears when character count is explicitly enabled or auto-enabled by `MaxLength`.
>
> ## Acceptance criteria
>
> - Pass `CounterContent` conditionally or give `FlareFieldChrome` an explicit counter-presence signal.
> - Fix both `FlareField` and `FlareTextArea`.
> - Add markup tests proving the support row is absent when empty and present for helper, error, explicit counter, and MaxLength-enabled counter.
> - Preserve `aria-describedby` behavior.
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; render-mode/browser independent; visible in interactive WebAssembly and reproducible in bUnit on .NET 10.

### Issue 5

**Title:** `TSV and Markdown exporters do not encode format-significant cell content`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> `TsvGridExporter` joins raw cell strings with tabs/newlines. Cells containing a tab or line break change the exported table structure. It also lacks the spreadsheet-formula guard already present in the CSV exporter.
>
> `MarkdownExporter` joins raw values between `|` characters. A pipe creates an extra column and a line break terminates the row under GFM table syntax.
>
> Existing exporter tests use only simple cell values, so these cases are not covered.
>
> ## Minimal reproduction
>
> Export rows containing:
>
> - TSV: `"a\tb"`, `"line 1\nline 2"`, and `"=1+1"`;
> - Markdown: `"a|b"` and `"line 1\nline 2"`.
>
> Open the TSV in a spreadsheet and render the Markdown as GFM. The logical cell/row structure is not preserved; formula-prefixed TSV cells may be interpreted as active spreadsheet content.
>
> ## Expected behavior
>
> Each exporter has an explicit, documented encoding policy that preserves a cell as one cell and prevents spreadsheet-active text from being emitted unsafely.
>
> ## Acceptance criteria
>
> - Define and apply a TSV policy for tabs, CR/LF, and formula-active prefixes, consistent with CSV security behavior.
> - Escape Markdown pipes and normalize/encode line breaks using a documented GFM-compatible representation.
> - Apply the policy to both headers and values.
> - Add tests for tabs, CR, LF, pipes, backslashes where relevant, and `=`, `+`, `-`, `@` prefixes.
> - Document any intentionally lossy normalization.
>
> ## References
>
> - OWASP CSV Injection guidance: https://owasp.org/www-community/attacks/CSV_Injection
> - GFM tables extension: https://github.github.com/gfm/#tables-extension-
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; exporter logic is render-mode/browser independent; .NET 10.

### Issue 6

**Title:** `FlareScrollTop does not react to runtime Selector, ThrottleMs, or Threshold changes`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> `FlareScrollTop` subscribes to `IScrollService` only during its first render. Later parameter updates leave the active subscription unchanged:
>
> - changing `Selector` keeps observing the old target, while a click scrolls the current selector;
> - changing `ThrottleMs` does not update the listener;
> - changing `Threshold` does not recompute visibility until another scroll notification arrives.
>
> The first case can make the watched target and the target scrolled by the same component different.
>
> ## Reproduction
>
> 1. Render `FlareScrollTop Selector="#first"` with a fake/recording `IScrollService`.
> 2. Re-render it with `Selector="#second"` and a different `ThrottleMs`.
> 3. Verify that the old subscription was not disposed and no new subscription was created.
> 4. Click the button and observe that `ScrollToTopAsync` receives `#second`, while visibility is still driven by `#first`.
>
> ## Expected behavior
>
> Subscription-affecting parameter changes atomically dispose/recreate the subscription, and threshold changes immediately derive visibility from the latest known/current position.
>
> ## Acceptance criteria
>
> - Track the selector and throttle used by the active subscription.
> - Dispose and recreate it when either changes; do not leak subscriptions.
> - Re-evaluate visibility when `Threshold` changes without waiting for a user scroll.
> - Add lifecycle tests with a fake `IScrollService`, including disposal and click/watch target consistency.
> - Preserve prerender/disconnect safety.
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; component lifecycle issue; .NET 10.

### Issue 7

**Title:** `Accordion content is clipped above the ContentMaxHeight token`

**Suggested labels:** `bug`

**Body:**

> ## What happened?
>
> Expanded accordion content is capped by `--flare-accordion-content-max-height` (currently 2000px). A panel whose intrinsic content height exceeds that value remains clipped while `aria-expanded="true"`.
>
> `FlareCollapse` already avoids this correctness ceiling with the intrinsic `grid-template-rows: 0fr` to `1fr` animation.
>
> ## Reproduction
>
> Render an accordion panel containing content taller than 2000px, expand it, and compare the content element's `scrollHeight` and visible `clientHeight`. The bottom content cannot be reached because the panel itself keeps `overflow: hidden` at the capped max-height.
>
> ## Expected behavior
>
> Expanded content is never clipped by an arbitrary animation constant.
>
> ## Acceptance criteria
>
> - Replace the capped max-height transition with an intrinsic-height disclosure technique, preferably shared internally with Collapse where semantics permit.
> - Verify short and >2000px content in open, closed, and transition states.
> - Decide a compatibility/deprecation path for the public `ContentMaxHeight` theme token rather than silently leaving an unused contract.
> - Respect reduced-motion behavior if it is supported by the surrounding motion system.
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; CSS/layout issue across render modes and modern browsers.

### Issue 8

**Title:** `Accordion headers lack heading semantics`

**Suggested labels:** `bug`, `accessibility`

**Body:**

> ## What happened?
>
> `FlareAccordionPanel` renders its header toggle as a bare button. The WAI-ARIA Accordion Pattern requires each accordion header button to be the only content of an element with heading semantics and an appropriate level.
>
> Without a heading, screen reader heading navigation does not expose the accordion's document structure.
>
> ## Expected behavior
>
> Consumers can place every accordion header at the correct document heading level without invalid heading hierarchy.
>
> ## Acceptance criteria
>
> - Add a configurable heading level/heading semantics around the existing button; do not hard-code a universally correct `h3`.
> - Keep the button's `aria-expanded` and `aria-controls` contract.
> - Ensure no non-button persistent content is placed inside the heading wrapper.
> - Add semantic markup tests for the default/configured behavior.
> - Review the always-on panel `role=region` against APG's optional-region guidance, especially for accordions with more than about six simultaneously expandable panels.
>
> ## Reference
>
> WAI-ARIA APG Accordion Pattern: https://www.w3.org/WAI/ARIA/apg/patterns/accordion/
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; render-mode/browser independent accessibility markup.

### Issue 9

**Title:** `Headerless FlareCollapse emits an unnamed region landmark`

**Suggested labels:** `bug`, `accessibility`

**Body:**

> ## What happened?
>
> `FlareCollapse` always renders `role="region"`. In headerless mode it has no internal header id, so `aria-labelledby` is omitted and the region has no accessible name.
>
> WAI-ARIA requires a region landmark to have a brief accessible label.
>
> ## Reproduction
>
> Render `<FlareCollapse Expanded="true">...</FlareCollapse>` without `Header` or `HeaderContent` and inspect the region element. It has `role="region"` with neither `aria-label` nor `aria-labelledby`.
>
> ## Expected behavior
>
> Headerless disclosure content is not exposed as an unnamed landmark. Consumers that need a landmark can explicitly provide its accessible name.
>
> ## Acceptance criteria
>
> - Omit `role="region"` in headerless mode unless an accessible name is supplied, or add explicit region-label parameters and only emit the role when named.
> - Keep headered mode associated with its toggle via `aria-labelledby`.
> - Add tests for headered, headerless unnamed, and headerless explicitly named cases.
>
> ## Reference
>
> WAI-ARIA `region`: https://www.w3.org/TR/wai-aria/#region
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; render-mode/browser independent accessibility markup.

### Issue 10

**Title:** `ScrollService listener-sharing contract contradicts its implementation`

**Suggested labels:** `documentation`

**Body:**

> ## What happened?
>
> Flare currently describes two different listener ownership models:
>
> - `IScrollService` says there is one throttled JS listener per subscription;
> - `ScrollService` remarks, `flare-scroll.js`'s file header, and a test comment say one listener per target with fan-out to all C# subscribers;
> - the implementation stores records by subscription id and calls `listen(...)` once for every `subscribe(...)`, so it currently behaves per subscription.
>
> This makes performance expectations and the purpose of the service ambiguous. The current tests do not assert actual `addEventListener` ownership.
>
> ## Expected behavior
>
> Public docs, implementation comments, tests, and runtime behavior describe one intentional model.
>
> ## Acceptance criteria
>
> - Decide whether the supported contract is per subscription or shared per resolved target.
> - If sharing per target, define how different subscriber `ThrottleMs` values are scheduled and implement correct reference-counted detach/fan-out.
> - If per subscription, update the stale service/JS/test comments and any performance claims.
> - Add a JS/module-level test that counts listener attachment and detachment for two subscriptions to the same target.
> - Update the stale `IUiJsService` XML summary that still mentions scroll-to-top after those methods moved to `IScrollService`.
>
> ## Environment
>
> Flare `0.26.2`, commit `ebef333`; current behavior verified from C# and `flare-scroll.js`; .NET 10.

## 11. Что сознательно не предлагается создавать

- **«Merge Accordion and Collapse».** Публичные contracts различны; issue был бы архитектурно вредным.
- **«Create DataExporter».** Existing interface уже экспортирует arbitrary tabular data; сначала нужен standalone example и реальные non-grid requirements.
- **«Move every scroll listener to ScrollService».** Несколько listeners выполняют специализированные DOM calculations и не дублируют generic subscription.
- **«Fix Barcode by changing `Align=End`».** Это скрывает только позиционирование root boxes и не исправляет разные control heights/pустой support row.
- **JSON duplicate titles.** Ограничение найдено, но без подтверждённого expected schema и разрешённости duplicate column titles confidence недостаточен для готового bug issue.

## 12. Предлагаемый порядок реализации

1. Исправить два field-family дефекта вместе в одном release, но отдельными commits/issues: они независимо влияют на geometry и имеют разные regression tests.
2. Исправить Accordion state synchronization и guard coordination до дальнейшего API-расширения Accordion.
3. Закрыть TSV/Markdown encoding gap до позиционирования exporters как общего standalone API.
4. Затем исправить ScrollTop lifecycle и accessibility/height defects.
5. Listener-sharing issue решить после короткого design decision; здесь документация должна следовать выбранной реализации.

## 13. Approval gate

На этом аудит остановлен. Код, PR и issue не создавались. После одобрения можно создать все 10 issue либо выбранные номера; перед публикацией тексты стоит оставить раздельными, чтобы каждый issue имел один root cause, ясные acceptance criteria и независимый lifecycle.
