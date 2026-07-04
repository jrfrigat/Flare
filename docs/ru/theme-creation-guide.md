# Flare - Руководство по созданию тем

> [English version ->](../en/theme-creation-guide.md) - [README](https://github.com/jrfrigat/Flare/blob/main/README.ru.md) - [Архитектура](architecture.md)

## Обзор

Отрисованная тема в Flare - это композиция трёх независимо переключаемых осей:
1. **Дизайн-система** (`ITheme`, нецветовые токены): типографика, форма, движение, состояния, тени, токены компонентов
2. **Цветовая палитра** (`Palette`, цветовые токены): ~47 семантических цветовых ролей, со светлым + тёмным (и опциональным high-contrast) вариантами
3. **Режим** (`ThemeMode`): Light / Dark / Auto - выбирает, какая `ColorScheme` палитры применяется

Это руководство покрывает создание первых двух: темы дают дизайн-токены, палитры дают цвета, и
вы можете сочетать любую тему с любой палитрой. Режим - это рантайм-выбор, его не нужно создавать.

## Быстрый старт

### Через FlareThemeBuilder (рекомендуется)

```csharp
using Flare.Theming;

var myTheme = new FlareThemeBuilder("my-theme", "My Custom Theme")
    .WithTypography(new TypographyTokens
    {
        BodyLarge = new TypeStyle
        {
            FontFamily = "Inter",
            FontWeight = "400",
            FontSize = "1rem",
            LineHeight = "1.5rem",
            LetterSpacing = "0em"
        },
        // ... остальные type styles
    })
    .WithShape(new ShapeTokens
    {
        None = "0px",
        ExtraSmall = "4px",
        Small = "8px",
        Medium = "12px",
        Large = "16px",
        ExtraLarge = "28px",
        Full = "9999px"
    })
    .WithStyleAsset("_content/MyApp/css/my-theme.css")
    .WithDefaultPalette("my-brand")
    .Build();
```

### Деривация от встроенной темы

Чтобы взять встроенную тему (MD3, Fluent UI 2, Aero, ...) и переопределить лишь несколько параметров,
используйте `Derive` - это композиция, а не наследование (классы тем намеренно `sealed`, что сохраняет
чистыми авто-дискавери тем и модель переопределения через `with`):

```csharp
using Flare.Theming;
using Flare.Theme.FluentUI2;

var myFluent = new Fluent2Theme().Derive(
    id: "my-fluent",                 // обязательно: отдельный id
    displayName: "My Fluent",
    design: d => d with { Shape = d.Shape with { Medium = "6px" } });

services.AddFlareTheme(myFluent);
```

`Derive` форвардит все члены базовой темы (палитры, палитра по умолчанию, style-ассеты, генератор палитр,
dark-оверрайды), кроме переданных; в `design` приходит базовый `DesignTokens`, так что через `with` вы
меняете только нужное.

Каждый пакет темы также экспортирует свои reference-токены (`Md3`, `Fluent2`, `Aero`, `LiquidGlass`,
`VisualStudio`) для прямой композиции при реализации `ITheme` с нуля:

```csharp
public DesignTokens Design => Fluent2.DesignReference with { /* оверрайды */ };
// цвета палитры:  Fluent2.LightColors with { Primary = "#0F6CBD" }
```

### Прямая реализация ITheme

Core Flare (`Flare.Abstractions`) **не несёт значений токенов по умолчанию** - каждая группа в
`DesignTokens` и каждый член каждой записи `*Tokens` объявлены `required`, поэтому core не содержит
"зашитого" дизайн-мнения (охраняется `ThemeIndependenceTests`). Голый `new DesignTokens { ... }`
поэтому обязан задать *все* токены, что непрактично. Вместо этого **производите от опубликованного
reference-пакета** и `with`-переопределяйте только отличия - именно так сделаны встроенные темы:

- `Flare.Theme.MaterialDesign3.Tokens` -> `MaterialDesignTokens.Design` (база линейки Material)
- `Flare.Theme.FluentUI2.Tokens` -> `FluentUI2Tokens.Design` (база линейки Fluent)

```csharp
using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;   // или Flare.Theme.FluentUI2.Tokens

public sealed class MyTheme : ITheme
{
    public string Id => "my-theme";
    public string DisplayName => "My Custom Theme";

    // Стартуем от полностью заполненного reference; переопределяем только нужные токены.
    public DesignTokens Design => MaterialDesignTokens.Design with
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Shape = MaterialDesignTokens.Design.Shape with { Medium = "6px" },
        Button = MaterialDesignTokens.Design.Button with { HeightMd = "2.5rem" },
        // ... только токены, отличающиеся от базы
    };

    public string DefaultPaletteId => "my-brand";
    public IReadOnlyList<string> StyleAssets => [
        "_content/MyApp/css/my-theme.css"
    ];
}
```

Если вам действительно нужна дизайн-система с нуля, без наследования от Material/Fluent, соберите
полный `DesignTokens` сами (задав каждую `required`-группу) - компилятор (CS9035) перечислит все
пропущенные токены.

## Регистрация темы

```csharp
// В Program.cs или Startup.cs
services.AddFlare(options =>
{
    options.DefaultTheme = new MyTheme();
    options.DefaultPalette = myBrandPalette;
    options.RegisterAllBuiltInThemes = false; // регистрируем только нужное
});

// Или регистрация в рантайме
public void ConfigureThemeService(IThemeService themeService)
{
    themeService.RegisterTheme(new MyTheme());
    themeService.RegisterPalette(myBrandPalette);
}
```

## Создание палитры

### Из seed-цветов

```csharp
var palette = PaletteFactory.FromColors(
    id: "my-brand",
    name: "My Brand Colors",
    main: "#6750A4",      // цвет бренда
    background: "#FFFBFE" // опциональный фоновый оттенок
);
```

### Ручная палитра

```csharp
var palette = new Palette
{
    Id = "my-brand",
    Name = "My Brand",
    Source = "Custom",
    Light = new ColorScheme
    {
        Primary = "#6750A4",
        OnPrimary = "#FFFFFF",
        PrimaryContainer = "#EADDFF",
        OnPrimaryContainer = "#21005D",
        // ... все 45+ цветовых ролей
    },
    Dark = new ColorScheme
    {
        Primary = "#D0BCFF",
        OnPrimary = "#381E72",
        PrimaryContainer = "#4F378B",
        OnPrimaryContainer = "#EADDFF",
        // ... все 45+ цветовых ролей
    }
};
```

### Динамический цвет (палитра из акцента ОС)

Flare умеет в рантайме строить полную светлую + тёмную палитру из **акцентного цвета ОС/браузера** -
акцента Windows/macOS или Android Material You - читая его через CSS-системный цвет `AccentColor`.
Палитра генерируется генератором **активной темы** (MD3 tonal, Fluent ramp, ...), поэтому
подстраивается под выбранную тему и пересоздаётся при её смене.

Включается один раз в `AddFlare`:

```csharp
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new Md3Theme();
    opts.UseDynamicPalette = true;                  // регистрирует палитру "dynamic"
    opts.DynamicFallbackPalette = Md3Palettes.Violet; // готовая палитра, если акцент ОС недоступен
});
```

Если другая палитра по умолчанию не задана, динамическая становится палитрой по умолчанию. Иначе её
можно выбрать в рантайме как обычную палитру (например, из переключателя палитр):

```csharp
await ThemeService.SetPaletteAsync(Palette.DynamicId);   // "dynamic"
```

`FlareThemeProvider` читает акцент при старте, перечитывает его при возврате фокуса в окно и при смене
light/dark в ОС, и пересоздаёт палитру новым генератором при смене темы - ничего дополнительно
подключать не нужно.

> **Важно - Chromium не отдаёт реальный акцент ОС.** Акцент берётся из CSS-системного цвета
> `AccentColor`. Ради защиты от фингерпринтинга **Chrome и Edge возвращают фиксированный плейсхолдер**
> (`#0075FF`, одинаковый у всех пользователей в светлой и тёмной теме, даже в установленных PWA) вместо
> настоящего акцента Windows/macOS. Только **Firefox** (и движки, отдающие подлинный акцент) отражает
> реальный цвет ОС; в Android Chrome акцент отражает Material You. Flare считает этот плейсхолдер
> Chromium «акцент недоступен» и использует fallback ниже, поэтому динамическая палитра не показывает
> произвольный синий, одинаковый для всех. Более глубокого API «палитры из обоев» в вебе нет.

**Запасная палитра (fallback).** Когда акцент ОС недоступен (Chrome/Edge или старые движки без
`AccentColor`), задайте `DynamicFallbackPalette` - готовую палитру, чьи точные цвета примет
динамическая палитра вместо приближения. Это рекомендуемый вариант. Если нужен генерируемый fallback,
задайте вместо неё `DynamicPaletteFallbackSeed` (seed-цвет) - палитра построится из него по правилам
активной темы. Подлинный акцент (Firefox) всё равно перекрывает любой fallback.

**Из своего seed.** Чтобы построить динамическую палитру из любого цвета (например, извлечённого из
изображения через `IFlareColorExtractor`), примените seed напрямую - он сгенерируется по правилам
активной темы:

```csharp
await ThemeService.ApplyDynamicPaletteAsync(new PaletteSeed("#3F51B5"));
```

## Система токенов

### Доступные token-записи

| Токен | Назначение | Поля |
|-------|---------|--------|
| `TypographyTokens` | Шрифты, размеры, насыщенность | 15 type scales |
| `ShapeTokens` | Радиусы скругления | 7 уровней |
| `ElevationTokens` | Тени | 6 уровней |
| `MotionTokens` | Длительности + easings | 6 длительностей + 4 easing |
| `StateTokens` | Уровни прозрачности | 6 состояний |
| `ButtonTokens` | Геометрия кнопки | ~30 полей |
| `InputTokens` | Геометрия поля ввода | 23 поля |
| `DialogTokens` | Модальный диалог | 26 полей |
| `DrawerTokens` | Навигационный drawer | 18 полей |
| `SnackbarTokens` | Уведомления | 22 поля |
| `SelectTokens` | Выпадающие списки | 24 поля |
| `TooltipTokens` | Тултипы | 15 полей |
| `PopoverTokens` | Поповеры | 12 полей |
| `DataGridTokens` | Таблицы данных | 33 поля |
| `CardTokens` | Карточки | 20 полей |
| `AvatarTokens` | Аватары | 17 полей |
| `ProgressTokens` | Индикаторы прогресса | 18 полей |
| `SwitchTokens` | Переключатели | 28 полей |
| `NavTokens` | Элемент навигации + индикатор | 4 поля |
| `RatingTokens` | Рейтинг (звёзды) | 4 поля |
| `PaginationTokens` | Пагинация | 4 поля |
| `TimelineTokens` | Точка + коннектор таймлайна | 7 полей |
| `StepperTokens` | Круг + коннектор степпера | 8 полей |
| `TreeTokens` | Строки дерева | 6 полей |
| `CalendarTokens` | Сетка месяца/дней календаря | 9 полей |

Это репрезентативная выборка; полный набор записей токенов компонентов лежит в
`Flare.Abstractions/Tokens/Components/`. Все члены каждой записи объявлены `required`, поэтому
компилятор перечислит все пропущенные, если вы собираете `DesignTokens` с нуля.

### Использование токенов в CSS

```css
/* Используйте var()-ссылки на токены */
.my-component {
    background: var(--flare-color-primary);
    color: var(--flare-color-on-primary);
    border-radius: var(--flare-shape-medium);
    padding: var(--flare-input-padding);
    font-family: var(--flare-typescale-body-large-font);
    transition: all var(--flare-motion-duration-short2) var(--flare-motion-easing-standard);
}
```

## Валидация темы

```csharp
var validator = new ThemeValidator();
var errors = validator.Validate(myTheme);

if (errors.Count > 0)
{
    foreach (var error in errors)
        Console.WriteLine(error);
}
```

## Импорт/экспорт темы

```csharp
// Экспорт в JSON
string json = ThemeJsonSerializer.ExportTheme(myTheme);

// Импорт из JSON
ITheme importedTheme = ThemeJsonSerializer.ImportTheme(json);

// Экспорт палитры
string paletteJson = ThemeJsonSerializer.ExportPalette(myPalette);

// Импорт палитры
Palette importedPalette = ThemeJsonSerializer.ImportPalette(paletteJson);
```

## Архитектура CSS

### Структура файлов

```
MyTheme/
+-- css/
|   +-- my-theme-base.css      # Базовый reset, импорты типографики
|   +-- components/
|       +-- button.css          # Доводки кнопки
|       +-- input.css           # Доводки полей ввода
|       +-- dialog.css          # Доводки диалога
|       +-- ...                 # Доводки прочих компонентов
```

### Базовый CSS

```css
/* my-theme-base.css */
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

/* Доводки конкретной темы */
.flare-theme-my-theme {
    --flare-typescale-body-large-font: 'Inter', sans-serif;
}
```

### Доводки компонентов

```css
/* components/button.css */
.flare-theme-my-theme .flare-btn {
    border-radius: var(--flare-shape-medium);
    font-family: var(--flare-typescale-label-large-font);
}
```

## Режим высокой контрастности

```csharp
var palette = new Palette
{
    Id = "my-brand",
    Name = "My Brand",
    Light = lightScheme,
    Dark = darkScheme,
    HighContrast = new ColorScheme
    {
        // Цвета высокой контрастности (WCAG AAA)
        Primary = "#000000",
        OnPrimary = "#FFFFFF",
        // ... все роли с контрастом >= 7:1
    }
};
```

## Лучшие практики

1. **Используйте токены, а не зашитые значения** - все цвета, размеры и отступы должны ссылаться на CSS-переменные
2. **Следуйте BEM-нэймингу** - `flare-{component}__{element}--{modifier}`
3. **Тестируйте оба режима** - и светлый, и тёмный должны выглядеть хорошо
4. **Тестируйте RTL** - вёрстка должна работать в языках с письмом справа налево
5. **Валидируйте тему** - используйте `ThemeValidator` перед регистрацией
6. **Держите темы минимальными** - переопределяйте только нужное; остальное наследуйте из дефолтов
7. **Документируйте токены** - добавляйте XML-доки к кастомным token-записям
```
