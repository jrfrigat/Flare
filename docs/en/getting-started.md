# Flare - Getting Started

> [Русская версия ->](../ru/getting-started.md) - [README](../../README.md) - [Architecture](architecture.md)

---

## Requirements

- .NET SDK 10 (to build). The Flare libraries are **.NET 10-first but multi-target net8.0, net9.0 and
  net10.0**, so they run on a .NET 8, 9, or 10 app.
- Blazor WebAssembly **or** Blazor Server (both are supported)

---

## 1. Installation

```sh
dotnet add package Flare.Blazor
```

This pulls in the core:
- `Flare.Abstractions` - ports + design-token model (dependency-free)
- `Flare.Theming` - theme engine; `Flare.Infrastructure` - browser/host adapters
- `Flare.Components` - 100+ UI components

Flare ships **no themes** - each design system is a separate package. Add the ones you need:

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
dotnet add package Flare.Theme.MaterialDesign3   # baseline Material Design 3 (non-Expressive)
dotnet add package Flare.Theme.MaterialDesign2
dotnet add package Flare.Theme.FluentUI2
# and, if needed: Flare.Theme.Aero, Flare.Theme.LiquidGlass, Flare.Theme.VisualStudio
```

---

## 2. DI setup

**`Program.cs`:**
```csharp
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.FluentUI2;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new Md3Theme();        // default design system
    opts.DefaultPalette = Md3Palettes.Violet;  // default palette
    opts.DefaultMode = ThemeMode.Auto;         // Light / Dark / Auto
});

// Register the other themes that should be available at runtime.
// AddFlareTheme also force-loads the theme assembly (important for trimmed/WASM builds).
builder.Services.AddFlareTheme(new Fluent2Theme());
```

> `AddFlare` registers `ISnackbarService`, `IDialogService` and `IMessageBoxService` itself -
> no separate registration is needed.

> **Dynamic Color** - want the palette to follow the OS/browser accent (Windows/macOS accent,
> Android Material You)? Add `opts.UseDynamicPalette = true;`. See
> [Theme creation -> Dynamic Color](theme-creation-guide.md#dynamic-color-palette-from-the-os-accent).

---

## 3. Wiring up the styles

**Blazor WASM (`wwwroot/index.html`) or Blazor Server (`App.razor` / `_Host.cshtml`):**

```html
<head>
    <!-- One line: theme classes + anti-FOUC splash before the first frame -->
    <script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
    <!-- All component styles -->
    <link rel="stylesheet" href="_content/Flare.Components/css/flare-components.css" />
    <!-- Material Symbols icons (optional, but recommended) -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />
</head>
```

> The active theme's CSS (`ITheme.StyleAssets` - fonts, base tokens) is wired up automatically by
> `FlareThemeProvider` at startup, so you do not need to add theme CSS by hand.

---

## 4. Wrap with the theme provider

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

Automatic dark mode is on by default: `FlareThemeProvider` watches the system
`prefers-color-scheme`. Turn it off with `RespectSystemColorScheme="false"`.

---

## 5. Global imports

**`_Imports.razor`:**
```razor
@using Flare.Components
@using Flare.Abstractions
@using Flare.Abstractions.Tokens
```

---

## 6. First component

```razor
@page "/hello"
@inject IThemeService ThemeService

<FlareCard>
    <FlareText Typo="TypographyScale.HeadlineMedium">Hello, Flare!</FlareText>
    <FlareText Typo="TypographyScale.BodyMedium">
        Current theme: @ThemeService.CurrentTheme.DisplayName
    </FlareText>
    <FlareButton OnClick="SwitchTheme" Variant="ButtonVariant.Filled">
        Switch theme
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

## 7. Theme switcher

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

To switch a **specific** theme or palette without remembering its id string, each package exposes
constants - `<Theme>.ThemeId` and `<Palettes>.<Name>Id`:

```csharp
await ThemeService.SetThemeAsync(Fluent2Theme.ThemeId);     // "fluent2"
await ThemeService.SetPaletteAsync(Fluent2Palettes.BlueId); // "fluent-blue"
```

If you enabled Dynamic Color (`opts.UseDynamicPalette = true`), switch to it like any palette:

```csharp
await ThemeService.SetPaletteAsync(Palette.DynamicId);      // "dynamic"
```

---

## 8. Form validation

Flare integrates fully with Blazor's standard `EditContext`:

```razor
<EditForm Model="@_model" OnValidSubmit="Submit">
    <DataAnnotationsValidator />

    <FlareTextField @bind-Value="_model.Name" Label="Name" />
    <FlareTextField @bind-Value="_model.Email" Label="Email" />
    <FlareSelect @bind-Value="_model.Role" Label="Role" Items="@_roles" />

    <ValidationSummary />

    <FlareButton Type="ButtonType.Submit" Variant="ButtonVariant.Filled">
        Submit
    </FlareButton>
</EditForm>
```

---

## 9. Dialog and Snackbar (via services)

```razor
@inject IDialogService Dialog
@inject ISnackbarService Snackbar

<!-- Add the providers once in your layout: -->
<FlareDialogProvider />
<FlareSnackbarProvider />

@code {
    private async Task Delete()
    {
        var confirmed = await Dialog.ConfirmAsync(
            "Delete record",
            "This action cannot be undone.",
            "Delete", "Cancel");

        if (confirmed == true)
        {
            // perform the delete...
            Snackbar.Show("Record deleted", SnackbarSeverity.Success);
        }
    }
}
```

---

## 10. Docker

```sh
# Run the Gallery PWA
docker compose up --build
# Open http://localhost:8080
```

---

## Next steps

- [Architecture](architecture.md) - modules, tokens and services in detail
- [Theme creation](theme-creation-guide.md) - design tokens, palettes, custom themes
- [Gallery](../../samples/Flare.Gallery/) - live examples of every component
