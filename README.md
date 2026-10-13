<p align="center">
  <a href="https://jrfrigat.github.io/Flare/">
    <img src="assets/banner.png" alt="Flare - Theme-agnostic Blazor component library for .NET 10 powered by a semantic token system" width="860">
  </a>
</p>

# Flare - Blazor Component Library

<p align="center">🌐 <b>English</b> · <a href="README.ru.md">Русский</a></p>

[![CI](https://github.com/jrfrigat/Flare/actions/workflows/ci.yml/badge.svg)](https://github.com/jrfrigat/Flare/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Flare.Blazor.svg)](https://www.nuget.org/packages/Flare.Blazor/)
[![Downloads](https://img.shields.io/nuget/dt/Flare.Blazor.svg)](https://www.nuget.org/packages/Flare.Blazor/)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Live demo](https://img.shields.io/badge/live%20demo-gallery-7c3aed)](https://jrfrigat.github.io/Flare/)

A production-ready, **theme-agnostic** Blazor component library - **.NET 10**-first, also targeting **.NET 8 and .NET 9** - with **zero baked-in styling**. Build your own design system with semantic tokens, or start instantly with one of seven production-ready preset themes (Material Design 3 Expressive, Material Design 3, Material Design 2, Fluent UI 2, Aero, Liquid Glass, Visual Studio 2026), all shipped as independent, optional packages. Runtime theme switching, no page reload, no flash of unstyled content.

**Built by backend developers, for backend developers.** You should not need to know HTML or CSS to
build any screen you want with Flare - a dashboard, an admin grid, a wizard, a split editor is
assembled from components and their parameters, with no stylesheet of your own and no DevTools. And
nothing is locked: every visual is a semantic token a theme can repaint, and every component forwards
`Class`, `Style` and arbitrary HTML attributes. Both halves, on every component - the first is the
promise, the second is what keeps it from being a cage. Anywhere the first half fails is a bug, and is
filed as one.

**[Live demo / component gallery ->](https://jrfrigat.github.io/Flare/)**

**190+ components - build your own theme with semantic tokens - 7 optional preset design systems x 30+ palettes (+ Dynamic Color) x light/dark/auto - zero third-party CSS dependencies**

---

## Features

- **190+ production-ready components** across inputs, layout, navigation, data display, feedback, and utilities
- **Three independent theme axes** - swap the design system (MD3 Expressive / MD3 / MD2 / Fluent UI 2 / Aero / Liquid Glass / Visual Studio 2026), the color palette, and light/dark/auto separately, at runtime
- **Seven design systems, shipped as independent packages** - reference only the themes you use; the umbrella `Flare.Blazor` package ships no themes of its own
- **Class-toggle delivery** - switching is a class swap on `<html>` (no per-switch CSS-var injection); `ThemeDelivery.Inject` available as a fallback
- **Reusable palettes & generators** - built-in Material/Office/Aero/Liquid Glass/Visual Studio palettes (30+ in total), plus `Palette.FromColors(id, name, seed)` to derive a full light+dark palette (MD3 tonal / Fluent ramp per active theme)
- **Dynamic Color** - opt-in palette derived at runtime from the OS/browser accent color (Windows/macOS accent, Android Material You via the CSS `AccentColor` system color) through the active theme's generator; falls back to a curated `DynamicFallbackPalette` where the real accent is unavailable - notably Chrome/Edge, which return a fixed placeholder, not the genuine accent (`opts.UseDynamicPalette = true`)
- **Auto dark mode** - `ThemeMode.Auto` tracks `prefers-color-scheme`; a one-line bootstrap script kills the FOUC before first paint
- **Unified color API** - one `Color` parameter (`FlareColor`) on every color-aware component: pass a semantic role (`FlareColor.Primary`) for a cached theme class, or any custom value (`FlareColor.Custom("#E91E63")`) for inline tokens
- **Expressive Slider** - MD3 Expressive + Fluent in one component: sizes XS-XL, range mode (two handles), vertical orientation, step indicators, value bubble, start/end icons, and a custom fill anchor (`Init`)
- **SVG charts** - line, bar, pie, and donut rendered with theme colors, hover tooltips, and a legend (no chart dependency)
- **Nested navigation** - nav groups can nest, and auto-expand the whole chain when a child link becomes active
- **XML IntelliSense** - every `[Parameter]` on every component has a `/// <summary>` doc comment
- **Full EditContext integration** - form validation with `DataAnnotationsValidator` out of the box
- **Advanced DataGrid** - multi-sort, column resize, row selection, inline editing, CSV export, grouping, virtualization
- **Blazor Server - WASM - SSR** compatible - JS interop calls are guarded against prerender
- **No Bootstrap / no third-party CSS** - all styles use `var(--flare-*)` tokens exclusively

---

## Quick Start

```sh
dotnet add package Flare.Blazor
```

Flare ships no themes of its own - each design system is an independent `Flare.Theme.*`
package. Reference the ones you want and register them:

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
dotnet add package Flare.Theme.FluentUI2   # add any of the other theme packages you need
```

**`Program.cs`:**
```csharp
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.MaterialDesign3.Tokens;   // Md3Palettes
using Flare.Theme.FluentUI2;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new MaterialDesign3ExpressiveTheme();  // design system (a theme = design tokens)
    opts.DefaultPalette = Md3Palettes.Violet;                  // colors (a palette carries light + dark)
    opts.DefaultMode = ThemeMode.Auto;                         // Light / Dark / Auto
});

// Register every other theme you want selectable at runtime.
// Referencing a package alone does not register its theme.
// Each theme brings its own palettes via ITheme.Palettes.
builder.Services.AddFlareTheme(new FluentUI2Theme());
```

**`index.html` / `App.razor` `<head>`:**
```html
<!-- One line: sets theme classes before first paint + fires "flare:ready" (your app draws its own splash).
     Place after <base>, before all stylesheets; keep synchronous (no async/defer).
     data-default-* names the theme to paint before .NET boots - the same one you register in Program.cs. -->
<script src="_content/Flare.Components/js/flare-bootstrap.js"
        data-default-theme="md3-expressive" data-default-palette="md3-violet"></script>
<!-- Component styles -->
<link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
```

> Icons are inline SVG, so there is **no icon font to add** - the built-in `FlareIcons` set backs the default
> component chrome out of the box. See [Icons](#icons) for the optional per-provider icon packages.

**Wrap your app:**
```razor
<FlareThemeProvider>
    <Routes />
</FlareThemeProvider>
```

---

## Runtime Theme Switching

Three independent axes, switched at runtime:

```razor
@inject IThemeService ThemeService

await ThemeService.SetThemeAsync("fluent2");        // design system
await ThemeService.SetPaletteAsync("fluent-blue");  // colors
await ThemeService.SetModeAsync(ThemeMode.Dark);    // Light / Dark / Auto

// read the current selection
var theme = ThemeService.CurrentTheme;
var palette = ThemeService.CurrentPalette;
var isDark = ThemeService.IsDark;
```

**Auto dark mode** is built in: with `ThemeMode.Auto`, `FlareThemeProvider` tracks the OS
`prefers-color-scheme` and the bootstrap script applies it before first paint.

---

## Color API

Every color-aware component takes a single `Color` parameter of type `FlareColor`:

```razor
@* Semantic role -> shared, cached theme class (theme-aware, accessible) *@
<FlareButton Color="FlareColor.Primary">Primary</FlareButton>
<FlareProgressLinear Color="FlareColor.Success" Value="70" />

@* Custom value -> inline CSS tokens (sanitized) *@
<FlareSlider Color="FlareColor.Custom("#E91E63")" @bind-Value="_v" />
```

Implicit conversion from `string` also works (`Color="@Colors.Primary"`). Roles map to
`--fc-main / --fc-on / --fc-container / --fc-on-container` tokens; components read only the
subset they need, so MD3 and Fluent stay in sync automatically.

---

## Icons

Icons are **inline SVG by default** - no icon font, no network request, no FOUT. The built-in
`FlareIcons` set (~90 members, the `Flare.Icons` package) backs the default component chrome, and the
provider catalogs (Material Design 3, Material Design 2, Fluent UI 2, Font Awesome - SVG or webfont)
are optional packages; the SVG ones are `IsTrimmable`, so a trimmed Blazor WebAssembly publish ships
only the icons you reference.

Full guide: [docs/en/icons.md](docs/en/icons.md).

---

## Components

| Category | Count | Key Components |
|----------|------:|----------------|
| **Inputs** | 18 | Button, Input, Checkbox, Switch, Radio, Select, Autocomplete, DatePicker, TimePicker, DateRangePicker, NumericField, Slider (single + range), Rating, FileUpload, ColorPicker, TagInput, InputMask, FormBuilder |
| **Layout** | 11 | Stack, Grid, Col, Container, Hidden, Layout, LayoutAppBar, LayoutDrawer, LayoutContent, Card, Divider |
| **Navigation** | 11 | AppBar, NavMenu, NavGroup, NavLink, Tabs, Tab, Accordion, Breadcrumb, Pagination, Stepper, Drawer |
| **Data Display** | 14 | DataGrid, Column, Table, VirtualList, InfiniteScroll, TreeView, DataTree, List, Timeline, Chart, Calendar, Kanban, Transfer, Carousel |
| **Feedback** | 9 | Dialog, DialogProvider, MessageBoxProvider, Alert, Snackbar, Progress, Skeleton, Tooltip, Overlay |
| **Display** | 9 | Typography (FlareText), Avatar, Badge, Chip, Icon, Image, Link, Popover, ScrollTop |
| **Utilities** | 5 | Menu, MenuItem, SpeedDial, SpeedDialAction, Clipboard, RichTextEditor |
| **IDE** *(separate package)* | 6 | Ribbon, DocumentTabs, ToolPanel, Splitter, StatusBar, MenuBar |

---

## Architecture

Onion rings, dependencies pointing strictly inward: `Flare.Abstractions` (ports, design-token model,
CSS name registry) <- `Flare.Theming` + `Flare.Infrastructure` (adapters) <- `Flare.Components` (UI
only) <- `Flare` (composition root, `AddFlare` binds ports to adapters). Themes and the optional
component families ship as separate packages, so an app references only what it uses. Every component
inherits `FlareComponentBase`, which receives the current theme through a cascaded `ThemeSnapshot` and
forwards `Class`, `Style` and arbitrary HTML attributes.

Full module map: [docs/en/architecture.md](docs/en/architecture.md)

---

## Gallery

The interactive component gallery ships as a **Blazor WASM PWA** with:
- EN/RU language switching (header toggle)
- Collapsible syntax-highlighted code examples for every demo section
- Theme switcher (design system x palette x mode) with "generate a palette from a color"

```sh
cd samples/Flare.Gallery
dotnet run
```

---

## Custom Palettes & Themes

A **palette** is just colors (light + dark). Reuse a design system and only swap the colors,
or generate a whole palette from one brand color - no need to fill 45 roles by hand:

```csharp
using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

// Override only what you need from the reference scheme:
var ocean = new Palette
{
    Id = "ocean", Name = "Ocean", Source = "Custom",
    Light = MaterialDesign3Tokens.LightColors with { Primary = "#006782", PrimaryContainer = "#BCE9FF" },
    Dark  = MaterialDesign3Tokens.DarkColors  with { Primary = "#5DD5FC", PrimaryContainer = "#004E63" },
};

// ...or derive a full light+dark palette from a brand color:
var brand = Palette.FromColors("brand", "Brand", "#7B1FA2");

builder.Services.AddFlare(opts => opts.DefaultPalette = ocean);   // at startup
themeService.RegisterPalette(brand);                              // ...or at runtime
await themeService.SetPaletteAsync("brand");
```

**Dynamic Color** - follow the OS/browser accent (Windows/macOS accent, Android Material You). The
palette is generated by the active theme's generator, so it adapts to every theme:

```csharp
builder.Services.AddFlare(opts =>
{
    opts.UseDynamicPalette = true;                  // registers the "dynamic" palette (and makes it
    opts.DynamicFallbackPalette = Md3Palettes.Violet; // default if no other default palette is set)
});
// switch to it like any palette, or drive it from your own seed:
await themeService.SetPaletteAsync(Palette.DynamicId);
await themeService.ApplyDynamicPaletteAsync(new PaletteSeed("#3F51B5"));
```

> **Chrome/Edge do not expose the real OS accent** (they return a fixed placeholder to mitigate
> fingerprinting); only Firefox reflects the genuine accent. On Chromium the Dynamic palette uses
> `DynamicFallbackPalette` - set it to a curated palette so it degrades to real colors, not an
> arbitrary blue.

See [Theme creation -> Dynamic Color](docs/en/theme-creation-guide.md#dynamic-color-palette-from-the-os-accent).

Override **any** design token via the public reference records:

```csharp
var design = MaterialDesign3Tokens.Design with
{
    Shape = MaterialDesign3Tokens.Design.Shape with { Medium = "10px" },
};
```

A full custom **theme** = `DesignTokens` + a default palette (implement `ITheme`).
See [docs/en/theme-creation-guide.md](docs/en/theme-creation-guide.md) for the theme-system model and a step-by-step walkthrough.

---

## Building & Testing

```sh
dotnet build
# Once per machine, and after updating Microsoft.Playwright:
pwsh tests/Flare.Browser.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test         # Includes C# Playwright browser tests
```

Browser tests start and stop their own local Blazor host and headless Chromium. On Linux, use
`install --with-deps chromium` to install browser system dependencies too. Missing Chromium fails
the tests rather than skipping them. Failures save screenshots and Playwright traces under
`tests/Flare.Browser.Tests/bin/<configuration>/net10.0/TestResults`.

CI runs on pushes and pull requests to `main`: build -> install Chromium -> test -> pack NuGet.

Contributor workflow - branches, pull requests, conventions: [CONTRIBUTING.md](CONTRIBUTING.md).

---

## Documentation

Full docs live in **[docs/](docs/README.md)** (English + Russian).

| Document | Languages | Description |
|----------|-----------|-------------|
| [README.md](README.md) - [ru](README.ru.md) | EN - RU | Project README |
| [getting-started](docs/en/getting-started.md) - [ru](docs/ru/getting-started.md) | EN - RU | Install, DI, styles, first component |
| [icons](docs/en/icons.md) - [ru](docs/ru/icons.md) | EN - RU | FlareIcon system, built-in set, provider packages, trimming |
| [architecture](docs/en/architecture.md) - [ru](docs/ru/architecture.md) | EN - RU | Module map, component patterns, theming deep-dive |
| [theme-creation-guide](docs/en/theme-creation-guide.md) - [ru](docs/ru/theme-creation-guide.md) | EN - RU | Theme creation: design tokens, palettes, custom themes |
| [component-conventions](docs/en/component-conventions.md) - [ru](docs/ru/component-conventions.md) | EN - RU | Component code conventions (CSS tokens, unified color, XML docs) |
| [Full API Reference](docs/api/README.md) | EN | Autogenerated from public assemblies and XML documentation. Contains detailed technical specifications for all components. |

---

## License

MIT (c) 2026 FrigaT
