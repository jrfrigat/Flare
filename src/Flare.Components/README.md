# Flare.Components

130+ production-ready Blazor UI components - inputs, buttons, layout, navigation, data display,
feedback and overlays - with runtime theme switching and zero third-party CSS dependencies.

## Install

```sh
dotnet add package Flare.Components
dotnet add package Flare.Theme.MaterialDesign3Expressive   # a theme is required
```

## Use

```csharp
// Program.cs
builder.Services.AddFlare(opts => opts.DefaultTheme = new MaterialDesign3ExpressiveTheme());
```
```html
<!-- host page <head> -->
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
```
```razor
<FlareThemeProvider>
    <FlareButton Variant="ButtonVariant.Filled">Click me</FlareButton>
</FlareThemeProvider>
```

Add-on packages extend this with more components: `Flare.Components.Carousel`, `.IDE`, `.Kanban`,
`.Media`, `.QrCode`, `.RichTextEditor`, `.Transfer`.

Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
