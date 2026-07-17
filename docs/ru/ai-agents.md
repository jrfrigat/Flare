# Flare для ИИ-агентов

> [English version ->](../en/ai-agents.md) - [Начало работы](getting-started.md) - [API Reference](../../api/index.md)

Компактный, насыщенный справочник для LLM-агентов, генерирующих код Blazor с **Flare**.
Прочитайте один раз - и сможете писать корректную разметку Flare без догадок. Точные имена и типы
параметров любого компонента смотрите в сгенерированном [API Reference](../../api/index.md).

---

## 1. Что такое Flare

- UI-библиотека для **.NET 8 / 9 / 10 Blazor** (приоритет net10). Работает и в Blazor WebAssembly, и в Blazor Server.
- **130+ компонентов**, все с префиксом `Flare*` (например `FlareButton`, `FlareDataGrid`).
- **Темизация на токенах - дизайн-система в ваших руках.** Компоненты не содержат вшитых стилей; каждый
  цвет, форма, размер и анимация берутся из темы через единый API семантических токенов. Семь готовых
  дизайн-систем (Material Design 3 Expressive, MD3, MD2, Fluent UI 2, Aero, Liquid Glass, Visual Studio)
  поставляются как независимые, необязательные пакеты - используйте готовую или создайте свою с нуля. CSS
  темы писать не нужно.
- **Варианты/размеры/формы компонентов - это enum'ы**, а не строки (например `Variant="ButtonVariant.Filled"`).

---

## 2. Главные правила (прочитать первыми)

1. **Каждый компонент начинается с `Flare`.** Отдельного `FlareIconButton` нет; кнопка-иконка - это
   `FlareButton` с параметром `Icon`. Если не уверены, что компонент существует, проверьте каталог в разделе 9.
2. **Двусторонняя привязка - через `@bind-Value`** (стандарт Blazor). Только чтение - `Value="..."`.
3. **Варианты - это enum'ы.** Используйте `ButtonVariant.Filled`, `TypographyScale.HeadlineMedium`,
   `SnackbarSeverity.Success` и т.д. Не передавайте строку там, где ожидается enum.
4. **Не пишите свой CSS для раскладки/отступов/цвета.** Используйте компоненты Flare, компоненты
   раскладки (`FlareStack`, `FlareGrid`, `FlareSpacer`) и систему токенов. `Style`/`Class` допустимы
   для точечных правок, но предпочитайте токены.
5. **Оборачивайте приложение в `<FlareThemeProvider>`** и регистрируйте хотя бы одну тему в DI, иначе
   ничего не отрисуется правильно.
6. **Сервисам оверлеев нужен их провайдер**, размещённый один раз в layout: `<FlareDialogProvider />`,
   `<FlareSnackbarProvider />`, `<FlareMessageBoxProvider />`.
7. **Формы используют `EditForm`/`EditContext` из Blazor** - поля Flare работают с
   `DataAnnotationsValidator` из коробки.

---

## 3. Установка

```sh
# Ядро (тянет Flare.Components (+ кольца Abstractions/Theming/Infrastructure), 130+ компонентов):
dotnet add package Flare.Blazor

# Тема - подключите preset-пакет ниже или создайте свою (docs/ru/theme-creation-guide.md):
dotnet add package Flare.Theme.MaterialDesign3Expressive
# другие: Flare.Theme.FluentUI2, Flare.Theme.Aero, Flare.Theme.LiquidGlass, Flare.Theme.VisualStudio
```

Тяжёлые опциональные компоненты - в своих пакетах, подключайте только нужное:

| Пакет | Компонент |
| :-- | :-- |
| `Flare.Components.Kanban` | `FlareKanban` |
| `Flare.Components.Media` | `FlareVideoPlayer` |
| `Flare.Components.RichTextEditor` | `FlareRichTextEditor` |
| `Flare.Components.Carousel` | `FlareCarousel` |
| `Flare.Components.Transfer` | `FlareTransfer` |
| `Flare.Components.QrCode` | `FlareQrCode` |
| `Flare.Components.IDE` | оболочка IDE (`FlareIdeLayout`, ribbon, докинг, ...) |

---

## 4. Настройка проекта (сделать все четыре пункта)

**a) DI - `Program.cs`:**
```csharp
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.FluentUI2;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();        // активная дизайн-система
    opts.DefaultPalette = Md3Palettes.Violet;    // базовая палитра
    opts.DefaultMode    = ThemeMode.Auto;        // Light | Dark | Auto
});

// Регистрируем дополнительные темы, доступные в рантайме:
builder.Services.AddFlareTheme(new FluentUI2Theme());
```
`AddFlare` также регистрирует `IDialogService`, `ISnackbarService` и `IMessageBoxService` -
отдельная регистрация не нужна. Другие точки входа: `AddFlareTheme`, `AddFlarePalette`, `AddFlareIde`,
`AddFlareVersionCheck`.

**b) Стили - `index.html` (WASM) или `App.razor`/`_Host.cshtml` (Server), в `<head>`:**
```html
<script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />
```
CSS темы (шрифты, базовые токены) подключается автоматически через `FlareThemeProvider`; вручную не добавляйте.

**c) Импорты - `_Imports.razor`:**
```razor
@using Flare.Components
@using Flare.Abstractions
@using Flare.Abstractions.Tokens
```

**d) Провайдер темы - оборачиваем роутер в `App.razor`:**
```razor
<FlareThemeProvider>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
    </Router>
</FlareThemeProvider>
```
Авто-тёмная тема следует за ОС по умолчанию; отключается через `RespectSystemColorScheme="false"`.

---

## 5. Базовые паттерны

**Текст и типографика:**
```razor
<FlareText Typo="TypographyScale.HeadlineMedium">Заголовок</FlareText>
<FlareText Typo="TypographyScale.BodyMedium">Основной текст.</FlareText>
```

**Кнопки:**
```razor
<FlareButton Variant="ButtonVariant.Filled" OnClick="Save">Сохранить</FlareButton>
<FlareButton Variant="ButtonVariant.Outlined" Icon="delete">Удалить</FlareButton>
<FlareButton Type="ButtonType.Submit" Variant="ButtonVariant.Filled">Отправить</FlareButton>
```

**Иконки** - это имена Material Symbols: `Icon="search"`, `Icon="delete"`, либо компонент
`<FlareIcon Name="..." />`.

**Двусторонняя привязка:**
```razor
<FlareTextField @bind-Value="_name" Label="Имя" />
<FlareSelect @bind-Value="_role" Label="Роль" Items="_roles" />
<FlareCheckbox @bind-Value="_agree">Согласен</FlareCheckbox>
```

---

## 6. Сервисы

```razor
@inject IDialogService Dialog
@inject ISnackbarService Snackbar
@inject IThemeService ThemeService

@code {
    private async Task Delete()
    {
        var ok = await Dialog.ConfirmAsync("Удалить запись", "Действие необратимо.", "Удалить", "Отмена");
        if (ok == true)
            Snackbar.Show("Запись удалена", SnackbarSeverity.Success);
    }

    // Переключение темы:
    private Task UseNext() =>
        ThemeService.SetThemeAsync(ThemeService.Themes.First().Id);
}
```
`IThemeService` предоставляет `CurrentTheme`, `Themes` и `SetThemeAsync(id)`. Помните: провайдеры
(`<FlareDialogProvider/>`, `<FlareSnackbarProvider/>`, `<FlareMessageBoxProvider/>`) должны быть один
раз в layout.

---

## 7. Формы и валидация

```razor
<EditForm Model="_model" OnValidSubmit="Submit">
    <DataAnnotationsValidator />

    <FlareTextField @bind-Value="_model.Name" Label="Имя" />
    <FlareTextField @bind-Value="_model.Email" Label="Email" />
    <FlareSelect @bind-Value="_model.Role" Label="Роль" Items="_roles" />

    <FlareButton Type="ButtonType.Submit" Variant="ButtonVariant.Filled">Отправить</FlareButton>
</EditForm>
```
Сообщения валидации рендерятся на поле Flare автоматически через общий `EditContext`.

---

## 8. Таблица данных (самый частый компонент)

```razor
<FlareDataGrid Items="_people" Filterable="true" Sortable="true" Pageable="true">
    <FlareColumn Field="@nameof(Person.Name)" Title="Имя" />
    <FlareColumn Field="@nameof(Person.Age)" Title="Возраст" Align="ColumnAlign.End" />
    <FlareColumn Field="@nameof(Person.Email)" Title="Email" />
</FlareDataGrid>
```
Таблица поддерживает сортировку (в т.ч. много-колоночную), фильтрацию, пагинацию, группировку,
inline/batch-редактирование, выделение, переупорядочивание/изменение размера/видимости колонок,
виртуализацию и бандлы колонок. Полный набор параметров `FlareDataGrid` / `FlareColumn` -
в [API Reference](../../api/index.md).

---

## 9. Каталог компонентов

Реальные имена всех компонентов (параметры каждого - в [API Reference](../../api/index.md)):

**Поля ввода и формы:** `FlareTextField`, `FlareTextArea`, `FlareNumericField`, `FlarePasswordField`,
`FlareMaskedField`, `FlareOtpField`, `FlareTagField`, `FlareSelect`, `FlareMultiSelect`,
`FlareAutocomplete`, `FlareListbox`, `FlareCheckbox`, `FlareSwitch`, `FlareRadio`, `FlareRadioGroup`,
`FlareSlider`, `FlareRating`, `FlareDatePicker`, `FlareDateRangePicker`, `FlareTimePicker`,
`FlareDateTimePicker`, `FlareCalendar`, `FlareClockDial`, `FlareColorPicker`, `FlareFileUploadZone`,
`FlareFileUploadButton`, `FlareSignaturePad`, `FlareField`, `FlareFormField`, `FlareForm`, `FlareFormBuilder`,
`FlareValidationSummary`.

**Кнопки и действия:** `FlareButton`, `FlareButtonGroup`, `FlareSplitButton`, `FlareToggleButton`,
`FlareToggleGroup`, `FlareFloatingActionButton`, `FlareFloatingActionMenu`, `FlareFloatingActionMenuItem`,
`FlareShortcuts`, `FlareShortcutEntry`, `FlareClipboard`.

**Отображение данных:** `FlareDataGrid` (+ `FlareColumn`, `FlareColumnBand`, `FlareColumnRow`,
`FlareDataGridPager`, `FlareDataGridQuickFilter`, `FlareDataGridFilterPresets`), `FlareTable`,
`FlareList`, `FlareListItem`, `FlareVirtualList`, `FlareTreeView`, `FlareDataTree`, `FlareTreeItem`,
`FlareTimeline`, `FlareTimelineItem`, `FlareChart`, `FlarePagination`, `FlareCard` (+ `FlareCardHeader`,
`FlareCardContent`, `FlareCardActions`, `FlareCardFooter`, `FlareCardMedia`), `FlarePaper`, `FlareBadge`,
`FlareChip`, `FlareChipGroup`, `FlareAvatar`, `FlareAvatarGroup`, `FlareSkeleton`, `FlareEmptyState`,
`FlareHighlighter`, `FlareImage`, `FlareIcon`, `FlarePropertyGrid`, `FlarePropertyGridItem`.

**Навигация:** `FlareAppBar`, `FlareNavMenu`, `FlareNavLink`, `FlareNavGroup`, `FlareTabs`, `FlareTab`,
`FlareStepper`, `FlareStep`, `FlareBreadcrumb`, `FlareMenu`, `FlareMenuItem`, `FlareMenuGroup`,
`FlareSubMenu`, `FlareMenuBar`, `FlareToolbar`, `FlareLink`, `FlareOnThisPage`, `FlareTableOfContents`,
`FlareTocLink`, `FlareBackstage`, `FlareBackstageItem`.

**Оверлеи и обратная связь:** `FlareDialog`, `FlareDialogProvider`, `FlareConfirmDialogProvider`,
`FlareSnackbarProvider`, `FlareMessageBoxProvider`, `FlareTooltip`, `FlarePopover`, `FlareOverlay`,
`FlareDrawer`, `FlareProgress`, `FlareAlert`.

**Раскладка и структура:** `FlareLayout` (+ `FlareLayoutAppBar`, `FlareLayoutContent`, `FlareLayoutDrawer`),
`FlareContainer`, `FlareGrid`, `FlareCol`, `FlareStack`, `FlareSpacer`, `FlareDivider`, `FlareSplitter`,
`FlareResizable`, `FlareAccordion`, `FlareAccordionPanel`, `FlareScrollTop`, `FlareInfiniteScroll`,
`FlareLazy`, `FlareMediaQuery`, `FlareHidden`.

**Контент:** `FlareText`, `FlareMarkdown`, `FlareCodeBlock`.

**Темизация:** `FlareThemeProvider`, `FlareThemeScope`, `FlareColorModeToggle`, `FlareColorCustomizer`.

**Office/IDE-оболочка (`Flare.Components.IDE`):** `FlareIdeLayout`, `FlareDocumentTab`, `FlareDocumentTabs`,
`FlareToolPanel`, `FlareRibbon` (+ `FlareRibbonTab`, `FlareRibbonGroup`, `FlareRibbonButton`,
`FlareRibbonDropdown`, `FlareRibbonSeparator`), `FlareQuickAccessToolbar`, `FlareStatusBar`,
`FlareFormulaBar`, `FlareSheetTabs`.

**Отдельные пакеты:** `FlareKanban`, `FlareVideoPlayer`, `FlareRichTextEditor`, `FlareCarousel`,
`FlareTransfer`, `FlareQrCode`.

---

## 10. Если вы редактируете сам Flare

При доработке исходников Flare (а не только при использовании) соблюдайте конвенции проекта:

- **Только ASCII** в коде, XML-доках и идентификаторах (без стрелок, длинных тире, многоточий, эмодзи).
  Исключение - только значения в локализуемых ресурсах (`.resx`).
- **XML-документация обязательна** на каждом public-типе, `[Parameter]`, методе и enum (она питает
  этот сгенерированный API-сайт).
- **CSS живёт в глобальном бандле** (`wwwroot/css/*.css`), не scoped; стилизация через семантические
  токены (`Css.Tokens.*`, `CssVarMap`, theme-records). `FlareButton` - эталонный компонент.
- **Никогда не хардкодьте видимые пользователю строки** в Gallery - добавляйте EN+RU ресурсы.

См. [Конвенции кода компонентов](component-conventions.md) и [Архитектуру](architecture.md).

---

## 11. Куда смотреть дальше

- **[API Reference](../../api/index.md)** - каждый public-тип, параметр и enum (сгенерировано из XML-доков).
- **[Начало работы](getting-started.md)** - та же настройка подробнее.
- **[Архитектура](architecture.md)** - модули, токены, сервисы, движок темизации.
- **Gallery** - живые интерактивные примеры каждого компонента (в `samples/Flare.Gallery`).
