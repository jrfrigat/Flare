# Flare - Архитектура

> [English version ->](../en/architecture.md) - [README](https://github.com/jrfrigat/Flare/blob/main/README.ru.md)

Как устроен Flare, как работает система тем и на какие контракты опираются компоненты.

---

## 1. Карта модулей

### Граф зависимостей

Flare построен как **чистая / луковичная (порты и адаптеры)** архитектура. Зависимости направлены строго
внутрь; слой UI использует **порты** (интерфейсы) и никогда не ссылается на адаптеры хоста. Корень
композиции (`Flare.Blazor`) - единственный пакет, связывающий порты с их реализациями-адаптерами.

```
            Flare.Abstractions   (Кольцо 0 - контракты)      без внутренних зависимостей
               ^        ^      ^
        Flare.Theming   |   Flare.Infrastructure   (Кольцо 1 движок / Кольцо 2 адаптеры)
        (-> Abstractions)   (-> Abstractions, Theming)
               ^                    ^
        Flare.Components   --------/   (Кольцо 3 - UI; -> Abstractions, Theming; НЕ Infrastructure)
               ^
        Flare (Flare.Blazor)   (Кольцо 4 - корень композиции; -> Components, Infrastructure)

Пакеты тем (каждый -> Flare.Abstractions + Flare.Theming; umbrella не ссылается ни на один):
  Flare.Theme.MaterialDesign3Expressive, .FluentUI2, .Aero, .LiquidGlass, .VisualStudio

Опциональные пакеты компонентов (каждый -> Flare.Components):
  Flare.Components.Carousel, .Kanban, .Transfer, .QrCode, .RichTextEditor, .Media, .IDE

samples/Flare.Gallery        -> Flare (umbrella) + все пакеты тем
tests/*                      -> Abstractions + Theming + Components + Infrastructure
```

> **Flare не содержит собственных тем.** Umbrella-пакет `Flare.Blazor` зависит от
> `Flare.Components` + `Flare.Infrastructure`. Каждая дизайн-система это отдельный пакет
> `Flare.Theme.*`, поэтому приложение подключает только нужные.

### Flare.Abstractions  (Кольцо 0 - контракты)
**Назначение.** Ядро без зависимостей, на котором строятся все остальные пакеты: **порты** плюс модель
дизайн-системы. Без зависимостей от хоста и JS.
- `Abstractions/` - порты: `ITheme`, `IThemeService`, `IPaletteProvider`, `IPaletteGenerator`,
  `ICssVariableInjector`, `IThemeStorageService`, `IThemeJsService`, `IThemeValidator`,
  `ICollisionService`, `ISnackbarService`, `IDialogService`, `IMessageBoxService`,
  `IVersionCheckService`, и JS-порты `IFlareClipboard`, `IFlareDownload`, `IFlareColorExtractor`,
  `ISplitterJsService`, `ITreeJsService`.
- `Tokens/` - неизменяемые `record`-типы ЗНАЧЕНИЙ токенов: `DesignTokens`, `ColorScheme`, `Palette`,
  `TypographyTokens`, `ShapeTokens`, `ElevationTokens`, `MotionTokens`, `StateTokens`, `SpacingTokens`,
  `TypeStyle`, `PaletteSeed`, `ThemeMode`, `ThemeDelivery`, `ThemeSnapshot` + записи в `Tokens/Components/`.
- `Css/` - реестр ИМЁН CSS-переменных (`Css.Tokens.*`, `Css.Classes.*`, `Vars`) и атрибут `[CssVar]`,
  связывающий свойство-значение токена с именем `--flare-*`, которое оно заполняет.
- `JsInterop/` - `FlareJsModule`, общая база для типизированных JS-interop сервисов.
- `Security/` - чистые утилиты `HtmlSanitizer` / `CssValidator` (internal).

**NuGet:** `Flare.Abstractions` - зависит только от `Microsoft.AspNetCore.Components.Web`.

### Flare.Theming  (Кольцо 1 - движок)
**Назначение.** Движок тем - сервисы приложения, превращающие модель токенов в готовую тему. Зависит
только от `Flare.Abstractions`.
- `Services/` - `ThemeService`, `ScopedThemeService`, `TokensToCss`, `CssVarMap`, `FlareBootstrap`.
- `Palettes/` - `DefaultPaletteGenerator`, `PaletteFactory`. `Color/` - `ColorMath`, `FlareColor`,
  `FlareColorResolver`. `Builders/`, `Serialization/` - `FlareThemeBuilder`, `ThemeJsonSerializer`.

**NuGet:** `Flare.Theming` - зависит от `Flare.Abstractions`.

### Flare.Infrastructure  (Кольцо 2 - адаптеры)
**Назначение.** Адаптеры браузера/хоста - конкретные реализации портов. Единственное кольцо, работающее
с `IJSRuntime`/`localStorage`. Зависит от `Flare.Abstractions` (+ `Flare.Theming`).
- `JsInterop/` - `CssVariableInjector`, `CollisionService`, `ThemeJsService`, `SplitterJsService`,
  `TreeJsService`, типизированные `FlareClipboardService`/`FlareDownloadService`/`FlareColorExtractor`.
- `Storage/` - `LocalStorageThemeStorage`, `NullThemeStorage`. `Feedback/` - `DialogService`,
  `SnackbarService`, `MessageBoxService`. `VersionCheck/` - `VersionCheckService`.

**NuGet:** `Flare.Infrastructure` - зависит от `Flare.Abstractions`, `Flare.Theming`.

### Flare.Components  (Кольцо 3 - UI)
**Назначение.** Только UI-компоненты - реализаций сервисов здесь нет. Каждый компонент в своей подпапке;
базовые компоненты в `Base/`. Зависит от `Flare.Abstractions` + `Flare.Theming` и использует адаптеры
только через их порты (внедряются через DI). **НЕ ссылается на `Flare.Infrastructure`** - этот инвариант
делает хост заменяемым.
- Каждый компонент наследует `FlareComponentBase` (в `Base/`, пространство имён `Flare.Components`).
- CSS поставляется единым бандлом на токенах в `wwwroot/css/` (агрегируется в `flare-components.css`) -
  не scoped CSS. Все правила используют только токены `var(--flare-*)`.
- Хранит весь статический JS в `wwwroot/js/` (`_content/Flare.Components/js/`); адаптеры Infrastructure
  импортируют его по URL (у статических ассетов нет связи на уровне сборок).
- `Resources/` - локализация EN/RU; `Theme/` - UI-контролы темы (`FlareColorCustomizer`, `FlareColorModeToggle`).
- Каждый `[Parameter]` имеет `/// <summary>` XML doc-комментарий.

**NuGet:** `Flare.Components` - зависит от `Flare.Abstractions`, `Flare.Theming`.

### Flare.Theme.* (семь дизайн-систем)
Каждый пакет темы предоставляет одну реализацию `ITheme` плюс палитры и статические ассеты:

| Пакет | Класс темы | `Id` | Палитра по умолчанию | Палитр |
|-------|------------|------|----------------------|--------|
| `Flare.Theme.MaterialDesign3Expressive` | `Md3Theme` | `md3-expressive` | Violet | 5 |
| `Flare.Theme.MaterialDesign3` | `MaterialDesign3Theme` | `md3` | Violet | (общие с MD3) |
| `Flare.Theme.MaterialDesign2` | `MaterialDesign2Theme` | `md2` | Purple | 6 |
| `Flare.Theme.FluentUI2` | `Fluent2Theme` | `fluent2` | Blue | 7 |
| `Flare.Theme.Aero` | `AeroTheme` | `aero` | Blue | 5 |
| `Flare.Theme.LiquidGlass` | `LiquidGlassTheme` | `liquid-glass` | Blue | 6 |
| `Flare.Theme.VisualStudio` | `VisualStudioTheme` | `visualstudio` | Blue | 5 |

Также можно включить рантайм-палитру **Dynamic Color** (`Palette.DynamicId = "dynamic"`) через
`FlareOptions.UseDynamicPalette`; она строится из акцента ОС генератором активной темы.

- Тема = дизайн-система (`DesignTokens`) + `DefaultPaletteId` + `StyleAssets`. Светлая/тёмная это
  **режим**, а не отдельная тема; цвета приходят из **палитры**.
- Каждый пакет экспортирует публичные reference-токены (например `Md3.DesignReference`, `Md3.LightColors`,
  `Md3.DarkColors`), чтобы выводить кастомные темы/палитры через `with`.
- Каждый несёт `IPaletteGenerator` по правилам цвета своей дизайн-системы (тональный MD3 / рампа).
- `StyleAssets` перечисляет статический CSS темы (шрифты, базовый сброс, сгенерированный CSS токенов),
  чтобы нужные токены присутствовали до первого кадра (анти-FOUC).

**NuGet:** каждый пакет зависит от `Flare.Abstractions` + `Flare.Theming`.

### Flare (umbrella / корень композиции)
**Назначение.** Единая цель установки, связывающая DI - единственное кольцо, знающее об адаптерах
Infrastructure.
- `ServiceCollectionExtensions` - `AddFlare(opts)`, `AddFlareTheme(theme)`, `AddFlarePalette(palette)`
  и `FlareOptions`. `AddFlare` связывает каждый порт с его адаптером из `Flare.Infrastructure`.
- Не содержит UI-кода, токенов или собственной темы. Реализации адаптеров (включая
  `LocalStorageThemeStorage`) живут в `Flare.Infrastructure`.

**NuGet:** `Flare.Blazor` - зависит от `Flare.Components` + `Flare.Infrastructure`.

### samples/Flare.Gallery
Blazor WebAssembly PWA. Интерактивная галерея с переключением EN/RU, сворачиваемыми примерами кода с
подсветкой и живым переключателем тем (дизайн-система x палитра x режим, плюс генерация палитры из
цвета). Регистрирует все пять тем через `AddFlareTheme`. Docker-готова.

> `samples/Flare.Legacy` - сохранённый legacy-пример, не часть публикуемой библиотеки.

---

## 2. Архитектура компонентов

### Контракт FlareComponentBase

```csharp
// Flare.Components.FlareComponentBase
public abstract class FlareComponentBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter] protected IThemeService? ThemeService { get; set; } // операции с темой
    [CascadingParameter] protected ThemeSnapshot? Theme { get; set; }        // текущее состояние (перерисовка)
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    protected abstract string ComponentCssClass { get; }
    protected string BuildCssClass(params string?[] additionalClasses);
    public virtual ValueTask DisposeAsync();
}
```

Ключевые правила контракта:
- `ComponentCssClass` возвращает BEM-имя блока (например `"flare-btn"`).
- `BuildCssClass(...)` ставит `ComponentCssClass` в начало, добавляет модификаторы, затем пользовательский
  `Class`. Список классов никогда не строится вручную в разметке.
- `AdditionalAttributes` прокидывается через `@attributes`; `Style` - в атрибут `style`.
- Состояние темы приходит **каскадным значением**: `FlareThemeProvider` каскадирует `IThemeService` и
  неизменяемый `ThemeSnapshot`. При смене любой оси ссылка на снимок меняется и Blazor перерисовывает
  потребителей - без пер-компонентных подписок на события.
- Реализован `IAsyncDisposable` - дочерние классы переопределяют `DisposeAsync` и вызывают
  `await base.DisposeAsync()`.

### CSS-конвенция именования

Все классы следуют BEM-варианту: `flare-[component]__[element]--[modifier]`.

| Контекст | Класс |
|----------|-------|
| Блок | `flare-btn` |
| Вариант Filled | `flare-btn--filled` |
| Слот ведущей иконки | `flare-btn__icon flare-btn__icon--leading` |
| Активная вкладка | `flare-tabs__tab flare-tabs__tab--active` |
| Сортируемый заголовок | `flare-datagrid__th flare-datagrid__th--sortable` |

Все CSS-правила используют только `var(--flare-*)` - никаких жёстко заданных цветов, размеров шрифтов или
тайминга анимаций.

### Паттерн составного компонента (Cascading Parent)

1. **Родитель регистрирует детей** (`FlareTabs`/`FlareTab`, `FlareDataGrid`/`FlareColumn`,
   `FlareAccordion`/`FlareAccordionPanel`): дети регистрируются при монтировании и снимаются при удалении;
   родитель владеет состоянием и перерисовывается после изменений.
2. **Каскадный callback** (`FlarePopover`/`FlareMenu`): передаёт `Func<Task>` закрытия вниз, чтобы
   вложенный элемент мог закрыть хост, не зная типа родителя.

---

## 3. Архитектура тем

### Три оси

Отрисованная тема это композиция трёх независимо переключаемых осей:

1. **Дизайн-система** (`ITheme`) - типографика, форма, движение, геометрия теней, токены компонентов.
2. **Палитра** (`Palette`) - цвета: несёт светлую и тёмную `ColorScheme` (и опц. high-contrast).
3. **Режим** (`ThemeMode`) - `Light` / `Dark` / `Auto` (Auto следит за `prefers-color-scheme`).

`IThemeService.SetThemeAsync` / `SetPaletteAsync` / `SetModeAsync` переключают каждую ось в рантайме.

### Иерархия токенов

```
ITheme
  +-- Id, DisplayName, DefaultPaletteId
  +-- StyleAssets (IReadOnlyList<string>)        - статический CSS/шрифты (анти-FOUC)
  +-- Palettes (IReadOnlyList<Palette>)          - цвета, путешествующие с темой
  +-- PaletteGenerator (IPaletteGenerator?)      - правила цвета дизайн-системы (тональный MD3 / рампа)
  +-- ExtendedDarkOverride (dict?)               - редкие нецветовые dark-расширения
  +-- Design (DesignTokens)                       - нецветовая половина (независима от режима)
        +-- FocusRing (string)
        +-- Typography -> набор TypeStyle (FontFamily, FontWeight, FontSize, LineHeight, LetterSpacing)
        +-- Shape, Elevation (геометрия), Motion, State, Spacing
        +-- пер-компонентные записи токенов (Button, Input, Select, Dialog, DataGrid, Card, ...)
        +-- Extended (dict) - специфичные для темы расширения (например var фокус-кольца Fluent)

Palette
  +-- Id, Name, Source
  +-- Light (ColorScheme), Dark (ColorScheme)    - ~47 цветовых ролей каждая
  +-- HighContrast (ColorScheme?)
  +-- StyleAsset (string?)
```

`DesignTokens`, `ColorScheme` и `Palette` это `record`-типы со свойствами `required init` -
конструирование проверяется на этапе компиляции и неизменяемо. Кастомные значения выводятся через
`with` из публичных reference-экземпляров (например `Md3.LightColors with { Primary = "..." }`).

### Доставка темы

`ThemeDelivery` выбирает, как CSS темы попадает в документ:

- **`ClassToggle`** (по умолчанию, быстрее всего) - тема/палитра/режим это набор классов на `<html>`;
  переключение это смена классов на фоне статического заранее сгенерированного CSS. `ThemeService`
  гарантирует наличие нужного class-scoped CSS (`EnsureStaticCssAsync` / `RequireThemeAssetsAsync`).
- **`Inject`** - `ICssVariableInjector` сплющивает активные токены через `CssVarMap` и пишет их в `:root`
  через JS-interop. Безопасно при SSR/prerender (исключения JS-interop проглатываются; статический
  базовый CSS даёт начальные значения).

### Бутстрап против FOUC

Одностроковый бутстрап-скрипт (`_content/Flare.Components/js/flare-bootstrap.js`) применяет сохранённые
классы темы/палитры/режима до первого кадра и рисует полноэкранный сплэш в цвете темы - поэтому пока
приложение грузится, страница никогда не показывает белый или неоформленный контент.

Дальше `FlareThemeProvider` раскрывает приложение автоматически (`ManageSplash`, по умолчанию `true`):
на первом интерактивном рендере он применяет классы темы и статический CSS, дожидается события `load`
каждого стиля темы и веб-шрифтов документа (`document.fonts.ready`), а затем плавно убирает сплэш
после первого оформленного кадра. Так потребитель получает полную защиту от FOUC из одной строки
`<script>` плюс провайдера - вручную ничего подключать не нужно. Страховочный таймаут в бутстрапе
раскрывает страницу в любом случае, если провайдера нет или загрузка упала; задайте
`ManageSplash="false"`, чтобы управлять `window.hideFlareSplash()` самостоятельно.

### Хранение

`IThemeStorageService` (порт в `Flare.Abstractions`, реализация `LocalStorageThemeStorage` в
`Flare.Blazor`) читает/пишет выбор в `localStorage`, с защитой от SSR/prerender. `FlareThemeProvider`
восстанавливает сохранённый выбор при первом интерактивном рендере.

### Как добавить новую тему

1. Создайте Razor-библиотеку (net8.0/net9.0/net10.0) со ссылкой на `Flare.Abstractions` + `Flare.Theming`.
2. Реализуйте `ITheme` - задайте `Id`, `DisplayName`, `Design` (`DesignTokens`, обычно выведенный из
   reference-темы через `with`), `DefaultPaletteId`, `Palettes` и `StyleAssets`.
3. Поставьте базовый CSS / CSS токенов, на который ссылается `StyleAssets`, в `wwwroot/`.
4. Зарегистрируйте: `services.AddFlareTheme(new MyTheme());` (это также принудительно загружает сборку,
   чего не делает простая ссылка в trimmed/WASM-приложении).

Полное руководство: [theme-creation-guide.md](theme-creation-guide.md).

---

## 4. Сервисный слой

### IThemeService
Управляет тремя осями и применяет их композицию к документу.
- **Состояние:** `CurrentTheme`, `CurrentPalette`, `Mode`, `IsDark`, `IsHighContrast`, `IsRtl`,
  `Delivery`, `Themes`, `Palettes`.
- **Переключение:** `SetThemeAsync`, `SetPaletteAsync`, `SetModeAsync`, `SetRtlAsync`, `SetSystemDarkAsync`.
- **Регистрация:** `RegisterTheme`, `RegisterPalette`.
- **Генерация:** `GeneratePalette(id, name, seed, source?)` генератором текущей темы.
- **Кастомизация:** `CustomizeColors`, `CustomizeDesign`, `SetCustomToken(s)`,
  `ClearCustomToken`/`ClearAllCustomTokens`, `GetCustomTokens`.
- **События:** `OnThemeChanged` (`event Func<Task>`) - после смены любой оси.
- **Реализация:** `ThemeService` (sealed). **Время жизни DI:** Scoped.

### Модель регистрации
`AddFlare` строит `ThemeService` в DI-фабрике: автодискаверит темы/палитры из загруженных сборок (когда
`RegisterAllBuiltInThemes` = true), затем добавляет зарегистрированные через `AddFlareTheme` /
`AddFlarePalette`, затем настроенную `DefaultTheme`. Темы/палитры регистрируются прямо в экземпляр
`ThemeService` (не как отдельные DI-сервисы), чтобы корректный выбор был доступен с первого рендера.

> Автодискавери видит только загруженные приложением сборки; ссылающийся-но-неиспользуемый пакет темы
> может не загрузиться в trimmed/WASM-приложении. Предпочитайте явный `AddFlareTheme`, который также
> принудительно загружает сборку темы.

### Прочие сервисы, регистрируемые `AddFlare` (все Scoped)

| Сервис | Реализация |
|--------|-----------|
| `ICssVariableInjector` | `CssVariableInjector` |
| `IThemeService` | `ThemeService` (factory) |
| `IThemeStorageService` | `LocalStorageThemeStorage` |
| `ISnackbarService` | `SnackbarService` |
| `IDialogService` | `DialogService` |
| `IMessageBoxService` | `MessageBoxService` |
| `ICollisionService` | `CollisionService` |
| `IThemeJsService` | `ThemeJsService` |
| `IFlareClipboard` / `IFlareDownload` / `IFlareColorExtractor` | типизированные обёртки JS-interop |

`AddFlareIde()` (из `Flare.Components.IDE`) регистрирует дополнительные сервисы пакета IDE.

### Различия WASM и Server

Оба хоста используют один путь кода. Сервисы JS-interop загружаются лениво и защищены от prerender
перехватом `InvalidOperationException` / `JSDisconnectedException`; статический базовый CSS даёт начальные
значения токенов, пока interop недоступен. Scoped-время жизни - на SignalR-канал (Server) или на сессию
(WASM).
