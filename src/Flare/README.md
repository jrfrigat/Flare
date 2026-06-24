# Flare.Blazor

Blazor component library with runtime theme switching, 67 components and zero third-party CSS
dependencies. This is the convenience meta-package: it pulls in `Flare.Core` + `Flare.Components`.
Themes ship as separate `Flare.Theme.*` packages, so your app only carries the ones it uses.

## Install

```sh
dotnet add package Flare.Blazor
dotnet add package Flare.Theme.MaterialDesign3Expressive   # pick at least one theme
```

## Setup

```csharp
// Program.cs
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new Md3Theme();
    opts.DefaultPalette = Md3Palettes.Violet;
    opts.DefaultMode    = ThemeMode.Auto;        // Light / Dark / Auto
});
```

Add the styles to your host page and wrap your router in the provider:

```html
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
```
```razor
<FlareThemeProvider>
    <Router ... />
</FlareThemeProvider>
```

## Links

- Repository: https://github.com/jrfrigat/Flare
- Getting started: https://github.com/jrfrigat/Flare/blob/main/docs/en/getting-started.md

MIT licensed.
