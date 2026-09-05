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
- `Flare.Components` - 130+ UI-компонентов

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
    opts.DefaultTheme = new MaterialDesign3ExpressiveTheme();        // дизайн-система по умолчанию
    opts.DefaultPalette = Md3Palettes.Violet;  // палитра по умолчанию
    opts.DefaultMode = ThemeMode.Auto;         // Light / Dark / Auto
});

// Зарегистрируйте остальные темы, которые должны быть доступны в рантайме.
// AddFlareTheme также принудительно загружает сборку темы (важно для trimmed/WASM).
builder.Services.AddFlareTheme(new FluentUI2Theme());
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
    <!-- Одна строка: применяет сохраненные классы темы к <html> до первого кадра (без мигания темы)
         и шлет "flare:ready", когда приложение оформлено. Сплэш НЕ рисует - его рисует приложение. -->
    <script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
    <!-- Все стили компонентов -->
    <link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
</head>
```

> Иконки - это inline-SVG (встроенный набор `FlareIcons`), поэтому **иконочный шрифт подключать не нужно**. Про
> пакеты провайдеров (Material Symbols / Fluent / Font Awesome) см. [Иконки](icons.md).

> CSS активной темы (`ITheme.StyleAssets` - шрифты, базовые токены) подключается автоматически
> `FlareThemeProvider` при старте, поэтому вручную добавлять CSS темы не нужно.

---

## 4. Обертка провайдером тем

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

Автоматический темный режим включен по умолчанию: `FlareThemeProvider` следит за системным
`prefers-color-scheme`. Отключить - параметром `RespectSystemColorScheme="false"`.

### Загрузочный сплэш (анти-FOUC)

**Flare не рисует загрузочный сплэш - его рисует само приложение** (фон + анимация), чтобы он
совпадал с вашим брендом. Бутстрап-скрипт делает только (1) применяет сохраненные классы
темы/палитры/режима к `<html>` до первого кадра (нет мигания темы) и (2) шлет сигнал готовности, когда
приложение оформлено: `FlareThemeProvider` вызывает его после применения классов темы, загрузки стилей
темы и веб-шрифтов (`document.fonts.ready`) и отрисовки первого оформленного кадра.

Добавьте свой сплэш в `index.html`. Дайте ему `id="flare-splash"` (или атрибут `data-flare-splash`) -
и Flare сам плавно уберет его по готовности; классы темы на `<html>` позволяют учесть темный режим:

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
await ThemeService.SetThemeAsync(FluentUI2Theme.ThemeId);     // "fluent2"
await ThemeService.SetPaletteAsync(Fluent2Palettes.BlueId); // "fluent-blue"
```

Если включен динамический цвет (`opts.UseDynamicPalette = true`), переключайтесь на него как на
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

## 10. Страница во весь экран

Страница, которая занимает окно, а не прокручивается вместе с ним - таблица под панелью фильтров, две
половины рядом, график над таблицей, - это раскладка, а именно на раскладке библиотека компонентов
обычно возвращает вам таблицу стилей. В Flare это три параметра и ни строчки CSS.

Цепочка держится на определенных высотах. `FlareLayout` - это `100dvh`, его область контента - строка
`1fr` этой сетки. Не хватало рамки вокруг страницы: своей высоты у нее не было, поэтому любой процент
ниже разрешался в `auto`. `FillHeight` ее дает, и это одно и то же слово на всех уровнях - способ
компонента сказать «трать высоту, которую мне дали, а не расти по содержимому».

```razor
<FlareLayoutContent FillHeight="true">
    <FlareTabs FillHeight="true">
        <FlareTab Label="Заказы">
            @* Без PageSize: все строки на одной странице, без пагинатора, с прилипшей шапкой.
               FillHeight: рамка равна тому, что осталось в панели вкладки. *@
            <FlareDataGrid Items="@_orders" FillHeight="true">
                <Columns>
                    <FlareColumn Title="Заказ" Field="@(o => o.Number)" />
                    <FlareColumn Title="Клиент" Field="@(o => o.Customer)" />
                </Columns>
            </FlareDataGrid>
        </FlareTab>
    </FlareTabs>
</FlareLayoutContent>
```

Три правила:

- **`FillHeight` нужен предок с высотой.** Странице ее дает `FlareLayoutContent FillHeight`. В любом
  другом месте - в демо, в диалоге - высоту нужно задать самому внешнему боксу (`Style="height:24rem"`).
- **Бокс с собственной высотой ее сохраняет.** Заполнение и «будь такой высоты» противоречат друг
  другу - заполнение работает через замену собственной высоты элемента долей родительской, - поэтому
  если `Style` объявляет `height`, побеждает эта высота, а `FillHeight` на этом боксе игнорируется.
  Угадывать правильную комбинацию не нужно: вы получаете написанное число.
- **Нужен каждому звену ниже.** Таблице во вкладках в карточке нужен `FillHeight` и на таблице, и на
  наборе вкладок, и на карточке, *и* на области контента; одно пропущенное звено разрешается в `auto`,
  и вся цепочка схлопывается до высоты содержимого. Переключатель есть у всего семейства контейнеров -
  `FlareCard`, `FlarePaper`, `FlareStack`, `FlareGrid`, `FlareCol`, `FlareTabs`, `FlareDataGrid`,
  `FlareLayoutContent` - и означает на всех одно и то же.
- **`Height` и `FillHeight` - альтернативы.** `Height="400px"` - число, которое написали вы;
  `FillHeight` - число, которое вычислила раскладка. Если заданы оба, побеждает `FillHeight`.
- **`Height` ограничивает ВЕСЬ компонент и действует в любом режиме.** Тулбар, пагинатор и футер
  входят в бюджет; прокручиваются только строки. Абсолютная длина - это ограничение сверху: если
  строк меньше, грид высотой ровно в свои строки. Процент - это высота, а не ограничение, и ему
  по-прежнему нужен предок с определенной высотой - то же самое `FillHeight` говорит без числа.
  `Height="auto"` (или `null`) снимает ограничение, и тогда прокручивается страница.

Размер, пагинация и переработка строк - три отдельных вопроса `FlareDataGrid`, а не один
переключатель режима. Прокрутки среди них нет: грид прокручивается, когда строки не помещаются в
отведенную высоту, - с пагинацией или без.

| Параметр | Что решает | По умолчанию |
| :-- | :-- | :-- |
| `Height` / `FillHeight` | какой высоты компонент, а значит - прокручивается ли он | `400px` |
| `PageSize` | `0` кладет все строки на одну страницу; положительный размер добавляет пагинатор | `0` |
| `StickyHeader` | остается ли шапка на месте, пока строки едут под ней | `true` |
| `Virtual` | в DOM живут только видимые строки - и больше ничего не меняется | авто |

`Virtual` сочетается со всем перечисленным: виртуализированный грид пагинируется, группируется,
рисует отступы дерева, раскрывает детальные строки и переставляет их ровно так же, как невиртуальный.
Если не задан - решает сам: набор в памяти больше 500 строк в гриде с высотой, в которой есть что
прокручивать, переработается. Ставьте `false`, когда все строки обязаны быть в DOM ради поиска
браузера или печати.

---

## 11. Docker

```sh
# Запустить Gallery PWA
docker compose up --build
# Откройте http://localhost:8080
```

---

## Дальнейшие шаги

- [Архитектура](architecture.md) - подробно о модулях, токенах и сервисах
- [Создание тем](theme-creation-guide.md) - дизайн-токены, палитры, кастомные темы
- [Галерея](https://jrfrigat.github.io/Flare/) - живые примеры всех компонентов
