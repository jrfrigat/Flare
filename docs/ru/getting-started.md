# Flare - Руководство по началу работы

> [Главная README](https://github.com/jrfrigat/Flare/blob/main/README.ru.md) - [Архитектура](architecture.md)

---

## Требования

- .NET SDK 10 (для сборки). Библиотеки Flare **ориентированы на .NET 10, но мультитаргетятся на
  net8.0, net9.0 и net10.0**, поэтому работают в приложении на .NET 8, 9 или 10.
- Blazor WebAssembly **или** Blazor Server (оба поддерживаются)

---

## 1. Установка

```sh
dotnet add package Flare.Blazor
```

Это подключит ядро:
- `Flare.Abstractions` - порты + модель дизайн-токенов (без зависимостей)
- `Flare.Theming` - движок тем; `Flare.Infrastructure` - адаптеры браузера/хоста
- `Flare.Components` - 100+ UI-компонентов

Flare **не содержит тем** - каждая дизайн-система это отдельный пакет. Подключите нужные:

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
dotnet add package Flare.Theme.MaterialDesign3   # базовый Material Design 3 (не Expressive)
dotnet add package Flare.Theme.MaterialDesign2
dotnet add package Flare.Theme.FluentUI2
# а также при необходимости: Flare.Theme.Aero, Flare.Theme.LiquidGlass, Flare.Theme.VisualStudio
```

---

## 2. Настройка DI

**`Program.cs`:**
```csharp
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.FluentUI2;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new Md3Theme();        // дизайн-система по умолчанию
    opts.DefaultPalette = Md3Palettes.Violet;  // палитра по умолчанию
    opts.DefaultMode = ThemeMode.Auto;         // Light / Dark / Auto
});

// Зарегистрируйте остальные темы, которые должны быть доступны в рантайме.
// AddFlareTheme также принудительно загружает сборку темы (важно для trimmed/WASM).
builder.Services.AddFlareTheme(new Fluent2Theme());
```

> `AddFlare` сам регистрирует `ISnackbarService`, `IDialogService` и `IMessageBoxService` -
> отдельная регистрация не нужна.

> **Динамический цвет (Dynamic Color)** - хотите, чтобы палитра подстраивалась под акцентный цвет
> ОС/браузера (акцент Windows/macOS, Android Material You)? Добавьте `opts.UseDynamicPalette = true;` и
> запасную палитру: `opts.DynamicFallbackPalette = Md3Palettes.Violet;`. Учтите: Chrome/Edge не отдают
> реальный акцент ОС (возвращают фиксированный плейсхолдер); подлинный акцент работает в Firefox, в
> остальных случаях используется запасная палитра. Подробнее:
> [Создание тем -> Динамический цвет](theme-creation-guide.md#динамический-цвет-палитра-из-акцента-ос).

---

## 3. Подключение стилей

**Blazor WASM (`wwwroot/index.html`) или Blazor Server (`App.razor` / `_Host.cshtml`):**

```html
<head>
    <!-- Одна строка: классы темы + анти-FOUC сплэш до первого кадра.
         FlareThemeProvider сам уберёт сплэш, когда CSS темы и шрифты загрузятся. -->
    <script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
    <!-- Все стили компонентов -->
    <link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
    <!-- Иконки Material Symbols (опционально, но рекомендуется) -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />
</head>
```

> CSS активной темы (`ITheme.StyleAssets` - шрифты, базовые токены) подключается автоматически
> `FlareThemeProvider` при старте, поэтому вручную добавлять CSS темы не нужно.

---

## 4. Обёртка провайдером тем

**`App.razor`:**
```razor
<FlareThemeProvider>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
    </Router>
</FlareThemeProvider>
```

Автоматический тёмный режим включён по умолчанию: `FlareThemeProvider` следит за системным
`prefers-color-scheme`. Отключить - параметром `RespectSystemColorScheme="false"`.

### Загрузочный сплэш (анти-FOUC)

Сразу из коробки: бутстрап-скрипт рисует полноэкранный сплэш в цвете темы до первого кадра, а
`FlareThemeProvider` сам убирает его, когда применит классы темы, дождётся загрузки стилей темы и
веб-шрифтов (`document.fonts.ready`) и отрисует первый оформленный кадр. Никакого мигания и ничего
подключать вручную не нужно.

Настройка - через `data-*` атрибуты на теге бутстрапа (все необязательны):

```html
<script src="_content/Flare.Components/js/flare-bootstrap.js"
        data-default-theme="md3-expressive" data-default-palette="md3-violet" data-default-mode="auto"
        data-splash-light="#FEF7FF" data-splash-dark="#141218" data-splash-timeout="8000"></script>
```

`data-splash-timeout` (мс) - страховочное раскрытие на случай, если провайдера нет или загрузка
упала. Чтобы управлять сплэшем самостоятельно, задайте `ManageSplash="false"` на `FlareThemeProvider`
и вызывайте `window.hideFlareSplash()` сами.

---

## 5. Глобальные импорты

**`_Imports.razor`:**
```razor
@using Flare.Components
@using Flare.Abstractions
@using Flare.Abstractions.Tokens
```

---

## 6. Первый компонент

```razor
@page "/hello"
@inject IThemeService ThemeService

<FlareCard>
    <FlareText Typo="TypographyScale.HeadlineMedium">Привет, Flare!</FlareText>
    <FlareText Typo="TypographyScale.BodyMedium">
        Текущая тема: @ThemeService.CurrentTheme.DisplayName
    </FlareText>
    <FlareButton OnClick="SwitchTheme" Variant="ButtonVariant.Filled">
        Сменить тему
    </FlareButton>
</FlareCard>

@code {
    private async Task SwitchTheme()
    {
        var themes = ThemeService.Themes;
        var current = ThemeService.CurrentTheme;
        var next = themes.SkipWhile(t => t.Id != current.Id).Skip(1).FirstOrDefault()
                   ?? themes.FirstOrDefault();
        if (next is not null)
            await ThemeService.SetThemeAsync(next.Id);
    }
}
```

---

## 7. Переключатель тем

```razor
@inject IThemeService ThemeService

@foreach (var theme in ThemeService.Themes)
{
    <FlareButton Variant="ButtonVariant.Outlined"
                 OnClick="@(() => ThemeService.SetThemeAsync(theme.Id))">
        @theme.DisplayName
    </FlareButton>
}
```

Чтобы переключить **конкретную** тему или палитру, не запоминая строку-id, каждый пакет
экспортирует константы - `<Theme>.ThemeId` и `<Palettes>.<Name>Id`:

```csharp
await ThemeService.SetThemeAsync(Fluent2Theme.ThemeId);     // "fluent2"
await ThemeService.SetPaletteAsync(Fluent2Palettes.BlueId); // "fluent-blue"
```

Если включён динамический цвет (`opts.UseDynamicPalette = true`), переключайтесь на него как на
обычную палитру:

```csharp
await ThemeService.SetPaletteAsync(Palette.DynamicId);      // "dynamic"
```

---

## 8. Валидация форм

Flare полностью интегрируется со стандартным `EditContext` Blazor:

```razor
<EditForm Model="@_model" OnValidSubmit="Submit">
    <DataAnnotationsValidator />

    <FlareTextField @bind-Value="_model.Name" Label="Имя" />
    <FlareTextField @bind-Value="_model.Email" Label="Email" />
    <FlareSelect @bind-Value="_model.Role" Label="Роль" Items="@_roles" />

    <ValidationSummary />

    <FlareButton Type="ButtonType.Submit" Variant="ButtonVariant.Filled">
        Отправить
    </FlareButton>
</EditForm>
```

---

## 9. Dialog и Snackbar (через сервисы)

```razor
@inject IDialogService Dialog
@inject ISnackbarService Snackbar

<!-- Добавьте провайдеры один раз в макет: -->
<FlareDialogProvider />
<FlareSnackbarProvider />

@code {
    private async Task Delete()
    {
        var confirmed = await Dialog.ConfirmAsync(
            "Удалить запись",
            "Это действие нельзя отменить.",
            "Удалить", "Отмена");

        if (confirmed == true)
        {
            // выполните удаление...
            Snackbar.Show("Запись удалена", SnackbarSeverity.Success);
        }
    }
}
```

---

## 10. Docker

```sh
# Запустить Gallery PWA
docker compose up --build
# Откройте http://localhost:8080
```

---

## Дальнейшие шаги

- [Архитектура](architecture.md) - подробно о модулях, токенах и сервисах
- [Создание тем](theme-creation-guide.md) - дизайн-токены, палитры, кастомные темы
- [Галерея](https://flare.frigat.duckdns.org/) - живые примеры всех компонентов
