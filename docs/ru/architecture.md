# Flare - Архитектура

> [English version ->](../en/architecture.md) - [README](../../README.ru.md)

Как устроен Flare, как работает система тем и на какие контракты опираются компоненты.

---

## 1. Карта модулей

### Граф зависимостей

```
Flare (umbrella)
+-- Flare.Components
    +-- Flare.Core

Пакеты тем (каждый ссылается только на Flare.Core, umbrella не ссылается ни на один):
  Flare.Theme.MaterialDesign3Expressive  -> Flare.Core
  Flare.Theme.FluentUI2                   -> Flare.Core
  Flare.Theme.Aero                        -> Flare.Core
  Flare.Theme.LiquidGlass                 -> Flare.Core
  Flare.Theme.VisualStudio                -> Flare.Core

Опциональные пакеты компонентов (каждый -> Flare.Core, некоторые -> Flare.Components):
  Flare.Components.Carousel, .Kanban, .Transfer, .QrCode,
  .RichTextEditor, .Media, .IDE

samples/Flare.Gallery        -> Flare (umbrella) + все пакеты тем
tests/Flare.Core.Tests       -> Flare.Core
tests/Flare.Components.Tests -> Flare.Components
```

> **Flare не содержит собственных тем.** Umbrella-пакет `Flare.Blazor` зависит только от
> `Flare.Components`. Каждая дизайн-система это отдельный пакет `Flare.Theme.*`, поэтому
> приложение подключает только нужные.

### Flare.Core
**Назначение.** Абстракции, токены и сервисы, независимые от тем и компонентов.
- `Abstractions/` - `ITheme`, `IThemeService`, `IPaletteProvider`, `ICssVariableInjector`,
  `IThemeStorageService`, `IThemeJsService`, `IThemeValidator`, `ICollisionService`,
  `ISnackbarService`, `IDialogService`, `IMessageBoxService`.
- `Tokens/` - неизменяемые `record`-типы: `DesignTokens` (нецветовые дизайн-токены), `ColorScheme`
  (цветовые роли), `Palette` (light + dark + опц. high-contrast), `TypographyTokens`, `ShapeTokens`,
  `ElevationTokens`, `MotionTokens`, `StateTokens`, `SpacingTokens`, `TypeStyle`, а также пер-компонентные
  записи токенов в `Tokens/Components/`. Плюс `ThemeMode`, `ThemeDelivery`, `ThemeSnapshot`, `PaletteFactory`.
- `Services/` - `ThemeService` (оркестрирует три оси темы), `CssVarMap` (сплющивание токенов),
  генераторы палитр (`DefaultPaletteGenerator`, `IPaletteGenerator`, `PaletteSeed`), а также
  хост-независимые `SnackbarService`, `DialogService`, `MessageBoxService`.
- `Components/` - `FlareComponentBase` (абстрактный базовый класс) и `FlareThemeProvider`.
- Своих статических веб-ассетов нет - JS ES-модули и CSS-бандл поставляются из `Flare.Components`.

**NuGet:** `Flare.Core` - зависит только от `Microsoft.AspNetCore.Components.Web`.

### Flare.Components
**Назначение.** Основные UI-компоненты. Каждый компонент в своей подпапке/пространстве имён.
- Каждый компонент наследует `FlareComponentBase`.
- CSS поставляется единым бандлом на токенах в `wwwroot/css/` (агрегируется в `flare-components.css`) -
  не scoped CSS. Все правила используют только токены `var(--flare-*)`.
- Хранит весь статический JS в `wwwroot/js/` (отдаётся как `_content/Flare.Components/js/`): head-скрипт
  `flare-bootstrap.js` (анти-FOUC) и лениво импортируемые interop-модули (`flare-theme.js`, коллизии,
  color-extractor, version-check).
- Содержит реализации сервисов на JS-interop, регистрируемые `AddFlare` (`CssVariableInjector`,
  `CollisionService`, `ThemeJsService`, типизированные обёртки clipboard/download/color-extractor).
- Каждый `[Parameter]` имеет `/// <summary>` XML doc-комментарий (`GenerateDocumentationFile` включён
  для всего решения).

**NuGet:** `Flare.Components` - зависит от `Flare.Core`.

### Flare.Theme.* (пять дизайн-систем)
Каждый пакет темы предоставляет одну реализацию `ITheme` плюс палитры и статические ассеты:

| Пакет | Класс темы | `Id` | Палитра по умолчанию | Палитр |
|-------|------------|------|----------------------|--------|
| `Flare.Theme.MaterialDesign3Expressive` | `Md3Theme` | `md3-expressive` | Violet | 5 |
| `Flare.Theme.FluentUI2` | `Fluent2Theme` | `fluent2` | Blue | 7 |
| `Flare.Theme.Aero` | `AeroTheme` | `aero` | Blue | 5 |
| `Flare.Theme.LiquidGlass` | `LiquidGlassTheme` | `liquid-glass` | Blue | 6 |
| `Flare.Theme.VisualStudio` | `VisualStudioTheme` | `visualstudio` | Blue | 5 |

- Тема = дизайн-система (`DesignTokens`) + `DefaultPaletteId` + `StyleAssets`. Светлая/тёмная это
  **режим**, а не отдельная тема; цвета приходят из **палитры**.
- Каждый пакет экспортирует публичные reference-токены (например `Md3.DesignReference`, `Md3.LightColors`,
  `Md3.DarkColors`), чтобы выводить кастомные темы/палитры через `with`.
- Каждый несёт `IPaletteGenerator` по правилам цвета своей дизайн-системы (тональный MD3 / рампа).
- `StyleAssets` перечисляет статический CSS темы (шрифты, базовый сброс, сгенерированный CSS токенов),
  чтобы нужные токены присутствовали до первого кадра (анти-FOUC).

**NuGet:** каждый пакет зависит только от `Flare.Core`.

### Flare (umbrella)
**Назначение.** Единая цель установки, связывающая DI.
- `ServiceCollectionExtensions` - `AddFlare(opts)`, `AddFlareTheme(theme)`, `AddFlarePalette(palette)`
  и `FlareOptions`.
- `LocalStorageThemeStorage` (internal) - реализует `IThemeStorageService` через `localStorage`.
- Не содержит UI-кода, токенов или собственной темы.

**NuGet:** `Flare.Blazor` - зависит от `Flare.Components`.

### samples/Flare.Gallery
Blazor WebAssembly PWA. Интерактивная галерея с переключением EN/RU, сворачиваемыми примерами кода с
подсветкой и живым переключателем тем (дизайн-система x палитра x режим, плюс генерация палитры из
цвета). Регистрирует все пять тем через `AddFlareTheme`. Docker-готова.

> `samples/Flare.Legacy` - сохранённый legacy-пример, не часть публикуемой библиотеки.

---

## 2. Архитектура компонентов

### Контракт FlareComponentBase

```csharp
// Flare.Core.Components.FlareComponentBase
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
классы темы/палитры/режима до первого кадра, а `StyleAssets` каждой темы дают базовый CSS токенов -
поэтому мигания неоформленного контента нет.

### Хранение

`IThemeStorageService` (интерфейс в `Flare.Core`, реализация internal `LocalStorageThemeStorage` в
`Flare.Blazor`) читает/пишет выбор в `localStorage`, с защитой от SSR/prerender. `FlareThemeProvider`
восстанавливает сохранённый выбор при первом интерактивном рендере.

### Как добавить новую тему

1. Создайте Razor-библиотеку `net10.0` со ссылкой на `Flare.Core`.
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
