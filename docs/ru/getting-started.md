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
    <!-- Одна строка: применяет сохранённые классы темы к <html> до первого кадра (без мигания темы)
         и шлёт "flare:ready", когда приложение оформлено. Сплэш НЕ рисует - его рисует приложение. -->
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

**Flare не рисует загрузочный сплэш - его рисует само приложение** (фон + анимация), чтобы он
совпадал с вашим брендом. Бутстрап-скрипт делает только (1) применяет сохранённые классы
темы/палитры/режима к `<html>` до первого кадра (нет мигания темы) и (2) шлёт сигнал готовности, когда
приложение оформлено: `FlareThemeProvider` вызывает его после применения классов темы, загрузки стилей
темы и веб-шрифтов (`document.fonts.ready`) и отрисовки первого оформленного кадра.

Добавьте свой сплэш в `index.html`. Дайте ему `id="flare-splash"` (или атрибут `data-flare-splash`) -
и Flare сам плавно уберёт его по готовности; классы темы на `<html>` позволяют учесть тёмный режим:

```html
<style>
    html { background: #fffbfe; }
    html.flare-mode-dark { background: #141218; }
    #flare-splash { position: fixed; inset: 0; z-index: 99999; display: flex;
        align-items: center; justify-content: center; background: #fffbfe; }
    html.flare-mode-dark #flare-splash { background: #141218; }
    /* ...ваш спиннер / логотип / анимация... */
</style>
...
<body>
    <div id="flare-splash"><!-- ваша анимация загрузки --></div>
    <div id="app">...</div>
</body>
```

Хотите убрать его сами? Слушайте событие:

```js
window.addEventListener('flare:ready', () => { /* спрятать свой сплэш */ });
```

Настройка бутстрап-скрипта - через `data-*` атрибуты (все необязательны):

```html
<script src="_content/Flare.Components/js/flare-bootstrap.js"
        data-default-theme="md3-expressive" data-default-palette="md3-violet" data-default-mode="auto"
        data-ready-timeout="8000"></script>
```

`data-ready-timeout` (мс) - страховочный сигнал на случай, если провайдера нет или загрузка упала.
Чтобы слать готовность самостоятельно, задайте `ManageSplash="false"` на `FlareThemeProvider` и
вызывайте `window.hideFlareSplash()` (или свою логику по `flare:ready`) сами.

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

Нужно больше, чем confirm/alert? `ShowAsync<TComponent>` показывает **любой** компонент как
модальное окно и возвращает типизированный результат - без встроенного `@bind-Visible`:

```razor
@inject IDialogService Dialog

@code {
    private async Task Edit(Person person)
    {
        var parameters = new DialogParameters()
            .Add(nameof(PersonEditDialog.Person), person);

        var result = await Dialog.ShowAsync<PersonEditDialog>(
            "Изменить профиль", parameters, new DialogOptions { Size = DialogSize.Sm });

        if (!result.Cancelled && result.GetData<Person>() is { } edited)
            Apply(edited);
    }
}
```

Тело диалога закрывает себя через каскадный `FlareDialogInstance`:

```razor
@code {
    [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
    [Parameter] public Person Person { get; set; } = default!;

    private void Save()   => Dialog.Close(_edited); // подтвердить с типизированным результатом
    private void Cancel() => Dialog.Cancel();        // отклонить
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
