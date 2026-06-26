# Flare - Руководство по началу работы

> [Главная README](../../README.ru.md) - [Архитектура](architecture.md)

---

## Требования

- .NET 10 SDK
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

---

## 3. Подключение стилей

**Blazor WASM (`wwwroot/index.html`) или Blazor Server (`App.razor` / `_Host.cshtml`):**

```html
<head>
    <!-- Одна строка: классы темы + анти-FOUC сплэш до первого кадра -->
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
- [Галерея](../../samples/Flare.Gallery/) - живые примеры всех компонентов
