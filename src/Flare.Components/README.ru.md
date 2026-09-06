# Flare.Components

160+ готовых к production UI-компонентов Blazor - поля ввода, кнопки, раскладка, навигация,
отображение данных, обратная связь и оверлеи - с переключением темы в рантайме и без единой
зависимости от сторонних CSS.

## Установка

```sh
dotnet add package Flare.Components
dotnet add package Flare.Theme.MaterialDesign3Expressive   # тема обязательна
```

## Использование

```csharp
// Program.cs
builder.Services.AddFlare(opts => opts.DefaultTheme = new MaterialDesign3ExpressiveTheme());
```
```html
<!-- в <head> хост-страницы -->
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
```
```razor
<FlareThemeProvider>
    <FlareButton Variant="ButtonVariant.Filled">Нажми меня</FlareButton>
</FlareThemeProvider>
```

Дополнительные пакеты добавляют новые компоненты: `Flare.Components.Barcode`, `.Carousel`, `.IDE`,
`.Kanban`, `.Media`, `.QrCode`, `.Query`, `.RichTextEditor`, `.Transfer`.

Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
