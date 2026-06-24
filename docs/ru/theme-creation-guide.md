# Flare - Руководство по созданию тем

> [English version ->](../en/theme-creation-guide.md) - [README](../../README.ru.md) - [Архитектура](architecture.md)

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
using Flare.Core.Builders;

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

### Прямая реализация ITheme

```csharp
using Flare.Core.Abstractions;
using Flare.Core.Tokens;

public sealed class MyTheme : ITheme
{
    public string Id => "my-theme";
    public string DisplayName => "My Custom Theme";

    public DesignTokens Design => new()
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = MyTypography.Tokens,
        Shape = MyShape.Tokens,
        Elevation = MyElevation.Tokens,
        Motion = MyMotion.Tokens,
        State = MyState.Tokens,
        Button = MyButton.Tokens,
        Input = MyInput.Tokens,
        Dialog = MyDialog.Tokens,
        // ... все токены компонентов
    };

    public string DefaultPaletteId => "my-brand";
    public IReadOnlyList<string> StyleAssets => [
        "_content/MyApp/css/my-theme.css"
    ];
}
```

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
var palette = Palette.FromColors(
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
