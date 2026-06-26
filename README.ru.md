<p align="center">
  <a href="https://flare.frigat.duckdns.org/">
    <img src="assets/banner.png" alt="Flare - библиотека компонентов Blazor для .NET 10 с Material Design 3 и Fluent UI 2" width="860">
  </a>
</p>

# Flare - Библиотека компонентов Blazor

> **[English version ->](README.md)**

[![CI](https://github.com/jrfrigat/Flare/actions/workflows/ci.yml/badge.svg)](https://github.com/jrfrigat/Flare/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Flare.Blazor.svg)](https://www.nuget.org/packages/Flare.Blazor/)
[![Downloads](https://img.shields.io/nuget/dt/Flare.Blazor.svg)](https://www.nuget.org/packages/Flare.Blazor/)
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Live demo](https://img.shields.io/badge/live%20demo-gallery-7c3aed)](https://flare.frigat.duckdns.org/)

Производственная библиотека компонентов Blazor для **.NET 10** с **переключением тем во время выполнения** между пятью дизайн-системами - Material Design 3 Expressive, Fluent UI 2, Aero, Liquid Glass и Visual Studio 2026 - без перезагрузки страницы и без мигания неоформленного контента.

**[Живое демо / галерея компонентов ->](https://flare.frigat.duckdns.org/)**

**100+ компонентов - 5 дизайн-систем x 28 палитр x светлая/тёмная/авто - единый API цвета - нет зависимостей от сторонних CSS - Docker-готовая галерея**

---

## Возможности

- **100+ готовых к production компонентов** - ввод данных, макет, навигация, отображение данных, обратная связь, утилиты
- **Три независимые оси темы** - отдельно переключаются дизайн-система (MD3 Expressive / Fluent UI 2 / Aero / Liquid Glass / Visual Studio 2026), палитра и режим светлая/тёмная/авто, во время выполнения
- **Пять дизайн-систем отдельными пакетами** - подключаете только нужные темы; umbrella-пакет `Flare.Blazor` не содержит собственных тем
- **Доставка class-toggle** - переключение это смена классов на `<html>` (без переинъекции CSS-переменных на каждом переключении); есть запасной `ThemeDelivery.Inject`
- **Переиспользуемые палитры и генераторы** - встроенные наборы Material/Office/Aero/Liquid Glass/Visual Studio (28 палитр) + `Palette.FromColors(id, name, seed)` для вывода полной палитры light+dark (MD3 тональный / Fluent рампа по активной теме)
- **Автоматический тёмный режим** - `ThemeMode.Auto` следует за `prefers-color-scheme`; одностроковый бутстрап-скрипт убирает FOUC до первого кадра
- **Единый API цвета** - один параметр `Color` (`FlareColor`) у каждого компонента с цветом: семантическая роль (`FlareColor.Primary`) даёт кэшируемый класс темы, либо любой произвольный цвет (`FlareColor.Custom("#E91E63")`) через инлайн-токены
- **Expressive-слайдер** - MD3 Expressive + Fluent в одном компоненте: размеры XS-XL, режим диапазона (два бегунка), вертикальная ориентация, индикаторы шагов, пузырёк значения, иконки начала/конца, якорь заполнения (`Init`)
- **SVG-графики** - линейный, столбчатый, круговой и кольцевой с цветами темы, подсказками при наведении и легендой (без сторонних зависимостей)
- **Вложенная навигация** - группы навигации можно вкладывать, и они авто-раскрывают всю цепочку при активной дочерней ссылке
- **XML IntelliSense** - каждый `[Parameter]` каждого компонента имеет `/// <summary>` doc-комментарий
- **Полная интеграция EditContext** - валидация форм с `DataAnnotationsValidator` из коробки
- **Расширенный DataGrid** - множественная сортировка, изменение размера столбцов, выбор строк, редактирование в ячейке, экспорт CSV, группировка, виртуализация
- **Blazor Server - WASM - SSR** - JS-вызовы защищены от предварительного рендера
- **Без Bootstrap / без сторонних CSS** - все стили используют только токены `var(--flare-*)`
- **Docker-готово** - одна команда `docker compose up --build` запускает Gallery PWA через nginx

---

## Быстрый старт

```sh
dotnet add package Flare.Blazor
```

Flare не содержит собственных тем - каждая дизайн-система это отдельный пакет
`Flare.Theme.*`. Подключите нужные и зарегистрируйте их:

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
dotnet add package Flare.Theme.FluentUI2   # добавьте любые другие пакеты тем
```

**`Program.cs`:**
```csharp
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.FluentUI2;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new Md3Theme();        // дизайн-система (тема = дизайн-токены)
    opts.DefaultPalette = Md3Palettes.Violet;  // цвета (палитра несёт light + dark)
    opts.DefaultMode = ThemeMode.Auto;         // Light / Dark / Auto
});

// Зарегистрируйте остальные темы, которые должны быть доступны в рантайме. AddFlareTheme также
// принудительно загружает сборку темы (чего не делает простая ссылка в trimmed/WASM-приложении),
// поэтому он предпочтительнее автодискавери FlareOptions.RegisterAllBuiltInThemes. Каждая тема
// приносит свои палитры через ITheme.Palettes.
builder.Services.AddFlareTheme(new Fluent2Theme());
```

**`index.html` / `App.razor` `<head>`:**
```html
<!-- Одна строка: классы темы + анти-FOUC сплэш до первого кадра -->
<script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
<!-- Стили компонентов -->
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
<!-- Шрифт Material Symbols (необязательно, но рекомендуется) -->
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />
```

**Оберните ваше приложение:**
```razor
<FlareThemeProvider>
    <Routes />
</FlareThemeProvider>
```

---

## Переключение тем во время выполнения

Три независимые оси, переключаемые в рантайме:

```razor
@inject IThemeService ThemeService

await ThemeService.SetThemeAsync("fluent2");        // дизайн-система
await ThemeService.SetPaletteAsync("fluent-blue");  // цвета
await ThemeService.SetModeAsync(ThemeMode.Dark);    // Light / Dark / Auto

// чтение текущего выбора
var theme = ThemeService.CurrentTheme;
var palette = ThemeService.CurrentPalette;
var isDark = ThemeService.IsDark;
```

**Автоматический тёмный режим** встроен: при `ThemeMode.Auto` `FlareThemeProvider` следит за
системным `prefers-color-scheme`, а бутстрап-скрипт применяет его до первого кадра.

---

## API цвета

Каждый компонент с цветом принимает один параметр `Color` типа `FlareColor`:

```razor
@* Семантическая роль -> общий кэшируемый класс темы (учитывает тему, доступность) *@
<FlareButton Color="FlareColor.Primary">Primary</FlareButton>
<FlareProgress Color="FlareColor.Success" Value="70" />

@* Произвольный цвет -> инлайн CSS-токены (с санитизацией) *@
<FlareSlider Color="FlareColor.Custom("#E91E63")" @bind-Value="_v" />
```

Работает и неявное преобразование из `string` (`Color="@Colors.Primary"`). Роли разворачиваются
в токены `--fc-main / --fc-on / --fc-container / --fc-on-container`; компонент читает только нужное
подмножество, поэтому MD3 и Fluent остаются согласованными автоматически.

---

## Docker

Запустите готовый образ Gallery из GitHub Container Registry (проще всего):

```sh
docker run -p 8080:80 ghcr.io/jrfrigat/flare-gallery:latest
# -> http://localhost:8080
```

Образ версионируется независимо от NuGet-пакетов (теги `gallery-v*`, напр.
`ghcr.io/jrfrigat/flare-gallery:0.0.1`).

Или соберите из исходников:

```sh
docker build -f samples/Flare.Gallery/Dockerfile -t flare-gallery .
docker run -p 8080:80 flare-gallery
# -> http://localhost:8080
```

`docker compose up --build` использует тот же `samples/Flare.Gallery/Dockerfile`, но прилагаемый
`docker-compose.yml` подключает контейнер к внешней сети `proxy_network` для развёртывания за
reverse-proxy (порт на хост не публикуется) - добавьте `ports:`, чтобы обращаться напрямую.

Многоэтапный `Dockerfile` создаёт образ `nginx:alpine`, раздающий статический, предсжатый
(brotli/gzip) вывод Blazor WASM.

---

## Компоненты

| Категория | Кол-во | Основные компоненты |
|-----------|------:|---------------------|
| **Ввод данных** | 18 | Button, Input, Checkbox, Switch, Radio, Select, Autocomplete, DatePicker, TimePicker, DateRangePicker, NumericField, Slider (одиночный + диапазон), Rating, FileUpload, ColorPicker, TagInput, InputMask, FormBuilder |
| **Макет** | 11 | Stack, Grid, Col, Container, Hidden, Layout, LayoutAppBar, LayoutDrawer, LayoutContent, Card, Divider |
| **Навигация** | 11 | AppBar, NavMenu, NavGroup, NavLink, Tabs, Tab, Accordion, Breadcrumb, Pagination, Stepper, Drawer |
| **Отображение данных** | 14 | DataGrid, Column, Table, VirtualList, InfiniteScroll, TreeView, DataTree, List, Timeline, Chart, Calendar, Kanban, Transfer, Carousel |
| **Обратная связь** | 9 | Dialog, DialogProvider, MessageBoxProvider, Alert, Snackbar, Progress, Skeleton, Tooltip, Overlay |
| **Дисплей** | 9 | Typography (FlareText), Avatar, Badge, Chip, Icon, Image, Link, Popover, ScrollTop |
| **Утилиты** | 5 | Menu, MenuItem, SpeedDial, SpeedDialAction, DropZone, RichTextEditor |
| **IDE** *(отдельный пакет)* | 6 | Ribbon, DocumentTabs, ToolPanel, Splitter, StatusBar, MenuBar |

---

## Архитектура

```
Flare.Abstractions      <- порты (ITheme, I*Service), модель дизайн-токенов, реестр имён CSS (без зависимостей)
Flare.Theming           <- движок тем: ThemeService, генерация палитр, ColorMath, токены->CSS
Flare.Infrastructure    <- адаптеры браузера/хоста: JS-interop, хранилище, dialog/snackbar/messagebox
Flare.Components        <- основные UI-компоненты + wwwroot/css (общий бандл на токенах)
Flare.Blazor (Flare)    <- корень композиции: AddFlare связывает порты -> адаптеры
Flare.Components.Carousel        <- Carousel
Flare.Components.Kanban          <- Kanban-доска
Flare.Components.Transfer        <- Transfer (двойной список)
Flare.Components.QrCode          <- QR-код
Flare.Components.RichTextEditor  <- Редактор форматированного текста
Flare.Components.Media           <- SignaturePad, VideoPlayer, FileUpload
Flare.Components.IDE             <- IDE-макет: Ribbon, DocumentTabs, ToolPanel, Splitter, StatusBar, MenuBar
Flare.Theme.MaterialDesign3Expressive <- дизайн-токены MD3 + палитры Material + тональный генератор
Flare.Theme.FluentUI2   <- дизайн-токены Fluent UI 2 + палитры Office + рампа-генератор
Flare.Theme.Aero        <- дизайн-токены Aero + палитры + рампа-генератор
Flare.Theme.LiquidGlass <- дизайн-токены Liquid Glass + палитры + рампа-генератор
Flare.Theme.VisualStudio <- дизайн-токены Visual Studio 2026 + палитры + рампа-генератор
Flare (umbrella)        <- DI-расширения AddFlare()/AddFlareTheme(), LocalStorageThemeStorage (зависит только от Flare.Components)
```

> Темы поставляются отдельными пакетами - umbrella-пакет `Flare.Blazor` не ссылается **ни на одну** тему,
> поэтому приложение подключает только нужные дизайн-системы. Опциональные семейства компонентов
> (`Carousel`, `Kanban`, `Transfer`, `QrCode`, `RichTextEditor`, `Media`, `IDE`) - тоже отдельные пакеты.

Все компоненты наследуют `FlareComponentBase`, который:
- Получает текущую тему через каскадный `ThemeSnapshot` и перерисовывается при её смене
- Предоставляет `BuildCssClass()` для построения BEM-классов
- Прокидывает `Class`, `Style` и произвольные HTML-атрибуты

Подробности архитектуры: [docs/ru/architecture.md](docs/ru/architecture.md)

---

## Галерея

Интерактивная галерея компонентов поставляется как **Blazor WASM PWA** с:
- Переключением языков EN/RU (кнопка в заголовке)
- Сворачиваемыми примерами кода с подсветкой синтаксиса для каждого раздела демонстрации
- Переключателем темы (дизайн-система x палитра x режим) с генерацией палитры из цвета

```sh
cd samples/Flare.Gallery
dotnet run
```

---

## Пользовательские палитры и темы

**Палитра** - это только цвета (light + dark). Переиспользуйте дизайн-систему и поменяйте
лишь цвета, либо сгенерируйте палитру из одного бренд-цвета - не задавая 45 ролей вручную:

```csharp
using Flare.Abstractions.Tokens;

// Переопределить только нужное от reference-схемы:
var ocean = new Palette
{
    Id = "ocean", Name = "Ocean", Source = "Custom",
    Light = Md3.LightColors with { Primary = "#006782", PrimaryContainer = "#BCE9FF" },
    Dark  = Md3.DarkColors  with { Primary = "#5DD5FC", PrimaryContainer = "#004E63" },
};

// ...или вывести полную палитру light+dark из бренд-цвета:
var brand = Palette.FromColors("brand", "Brand", "#7B1FA2");

builder.Services.AddFlare(opts => opts.DefaultPalette = ocean);   // на старте
themeService.RegisterPalette(brand);                              // ...или в рантайме
await themeService.SetPaletteAsync("brand");
```

Переопределить **любой** дизайн-токен через публичные reference-записи:

```csharp
var design = Md3.DesignReference with
{
    Shape = Md3.DesignReference.Shape with { Medium = "10px" },
};
```

Полноценная кастомная **тема** = `DesignTokens` + дефолтная палитра (реализуйте `ITheme`).
Модель системы тем и пошаговое руководство: [docs/ru/theme-creation-guide.md](docs/ru/theme-creation-guide.md).

---

## Сборка и тестирование

```sh
dotnet build        # 0 ошибок
dotnet test         # 1183 проходят (1125 Components + 58 Core)
docker run -p 8080:80 flare-gallery   # Gallery на http://localhost:8080
```

CI запускается при каждом пуше в `main`/`master`: сборка -> тесты -> упаковка NuGet -> публикация артефакта Gallery.

---

## Документация

Вся документация - в **[docs/](docs/README.md)** (English + Русский).

| Документ | Языки | Описание |
|----------|-------|----------|
| [README.md](README.md) - [ru](README.ru.md) | EN - RU | README проекта |
| [getting-started](docs/ru/getting-started.md) - [en](docs/en/getting-started.md) | RU - EN | Установка, DI, стили, первый компонент |
| [architecture](docs/ru/architecture.md) - [en](docs/en/architecture.md) | RU - EN | Карта модулей, паттерны компонентов, архитектура тем |
| [theme-creation-guide](docs/ru/theme-creation-guide.md) - [en](docs/en/theme-creation-guide.md) | RU - EN | Создание тем: дизайн-токены, палитры, кастомные темы |
| [component-conventions](docs/ru/component-conventions.md) - [en](docs/en/component-conventions.md) | RU - EN | Конвенции кода компонентов (CSS-токены, единый цвет, XML-доки) |

---

## Лицензия

MIT (c) 2026 FrigaT
