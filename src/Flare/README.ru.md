# Flare.Blazor

Библиотека компонентов Blazor с переключением темы в рантайме, 160+ компонентами и без зависимостей
от сторонних CSS. Это мета-пакет для удобства: он тянет `Flare.Components` (сами компоненты) и
`Flare.Infrastructure` (адаптеры JS-interop и сервисов) и добавляет точку входа DI - `AddFlare`.
Темы поставляются отдельными пакетами `Flare.Theme.*`, поэтому приложение везет только те, что
использует.

## Установка

```sh
dotnet add package Flare.Blazor
dotnet add package Flare.Theme.MaterialDesign3Expressive   # нужна хотя бы одна тема
```

## Настройка

```csharp
// Program.cs
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();
    opts.DefaultPalette = Md3Palettes.Violet;
    opts.DefaultMode    = ThemeMode.Auto;        // Light / Dark / Auto
});
```

Подключите стили на хост-странице и оберните роутер в провайдер:

```html
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
```
```razor
<FlareThemeProvider>
    <Router ... />
</FlareThemeProvider>
```

## Ссылки

- Репозиторий: https://github.com/jrfrigat/Flare
- Начало работы: https://github.com/jrfrigat/Flare/blob/main/docs/ru/getting-started.md

Лицензия MIT.
