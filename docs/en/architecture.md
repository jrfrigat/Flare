# Flare - Architecture

> [Русская версия ->](../ru/architecture.md) - [README](../../README.md)

How Flare is structured, how theming works, and the contracts components rely on.

---

## 1. Module Map

### Dependency Graph

```
Flare (umbrella)
+-- Flare.Components
    +-- Flare.Core

Theme packages (each references only Flare.Core, none referenced by the umbrella):
  Flare.Theme.MaterialDesign3Expressive  -> Flare.Core
  Flare.Theme.FluentUI2                   -> Flare.Core
  Flare.Theme.Aero                        -> Flare.Core
  Flare.Theme.LiquidGlass                 -> Flare.Core
  Flare.Theme.VisualStudio                -> Flare.Core

Optional component packages (each -> Flare.Core, some -> Flare.Components):
  Flare.Components.Carousel, .Kanban, .Transfer, .QrCode,
  .RichTextEditor, .Media, .IDE
  Flare.Icons

samples/Flare.Gallery        -> Flare (umbrella) + all theme packages
tests/Flare.Core.Tests       -> Flare.Core
tests/Flare.Components.Tests -> Flare.Components
```

> **Flare ships no themes of its own.** The umbrella `Flare.Blazor` package depends only on
> `Flare.Components`. Each design system is an independent `Flare.Theme.*` package, so an app
> references only the ones it uses.

### Flare.Core
**Purpose.** Theme- and component-agnostic abstractions, token records, and services.
- `Abstractions/` - `ITheme`, `IThemeService`, `IPaletteProvider`, `ICssVariableInjector`,
  `IThemeStorageService`, `IThemeJsService`, `IThemeValidator`, `ICollisionService`,
  `ISnackbarService`, `IDialogService`, `IMessageBoxService`.
- `Tokens/` - immutable `record` types: `DesignTokens` (non-color design tokens), `ColorScheme`
  (color roles), `Palette` (light + dark + optional high-contrast), `TypographyTokens`,
  `ShapeTokens`, `ElevationTokens`, `MotionTokens`, `StateTokens`, `SpacingTokens`, `TypeStyle`,
  plus per-component token records under `Tokens/Components/`. Also `ThemeMode`, `ThemeDelivery`,
  `ThemeSnapshot`, `PaletteFactory`.
- `Services/` - `ThemeService` (orchestrates the three theme axes), `CssVarMap` (token flattener),
  palette generators (`DefaultPaletteGenerator`, `IPaletteGenerator`, `PaletteSeed`), and the
  host-agnostic `SnackbarService`, `DialogService`, `MessageBoxService`.
- `Components/` - `FlareComponentBase` (abstract Blazor base class) and `FlareThemeProvider`.
- No static web assets of its own - the JS ES modules and CSS bundle ship from `Flare.Components`.

**NuGet:** `Flare.Core` - depends only on `Microsoft.AspNetCore.Components.Web`.

### Flare.Components
**Purpose.** The core UI components. Each component lives in its own sub-namespace folder.
- Every component inherits `FlareComponentBase`.
- CSS ships as a global, token-driven bundle in `wwwroot/css/` (aggregated into
  `flare-components.css`) - not scoped CSS. All rules consume `var(--flare-*)` tokens only.
- Hosts all static JS in `wwwroot/js/` (served at `_content/Flare.Components/js/`): the
  `flare-bootstrap.js` anti-FOUC head script and the lazily-imported interop ES modules
  (`flare-theme.js`, collision, color-extractor, version-check).
- Hosts the JS-interop-backed service implementations registered by `AddFlare` (`CssVariableInjector`,
  `CollisionService`, `ThemeJsService`, and the typed clipboard/download/color-extractor wrappers).
- Every `[Parameter]` carries a `/// <summary>` XML doc comment for IntelliSense on NuGet consumers
  (`GenerateDocumentationFile` is enabled solution-wide).

**NuGet:** `Flare.Components` - depends on `Flare.Core`.

### Flare.Theme.* (five design systems)
Each theme package provides one concrete `ITheme` plus its palettes and static style assets:

| Package | Theme class | `Id` | Default palette | Palettes |
|---------|-------------|------|-----------------|----------|
| `Flare.Theme.MaterialDesign3Expressive` | `Md3Theme` | `md3-expressive` | Violet | 5 |
| `Flare.Theme.FluentUI2` | `Fluent2Theme` | `fluent2` | Blue | 7 |
| `Flare.Theme.Aero` | `AeroTheme` | `aero` | Blue | 5 |
| `Flare.Theme.LiquidGlass` | `LiquidGlassTheme` | `liquid-glass` | Blue | 6 |
| `Flare.Theme.VisualStudio` | `VisualStudioTheme` | `visualstudio` | Blue | 5 |

- A theme = a design system (`DesignTokens`) + a `DefaultPaletteId` + `StyleAssets`. Light/dark is a
  **mode**, not a separate theme; colors come from a **palette**.
- Each package exposes public reference tokens (e.g. `Md3.DesignReference`, `Md3.LightColors`,
  `Md3.DarkColors`) so custom themes/palettes can be derived with `with` expressions.
- Each carries an `IPaletteGenerator` matching the design system's color rules (MD3 tonal / ramp).
- `StyleAssets` lists the static CSS the theme needs (fonts, base reset, generated token CSS) so
  the correct tokens are present on first paint (anti-FOUC).

**NuGet:** each package depends only on `Flare.Core`.

### Flare (umbrella)
**Purpose.** Single install target wiring up DI.
- `ServiceCollectionExtensions` - `AddFlare(opts)`, `AddFlareTheme(theme)`, `AddFlarePalette(palette)`
  and `FlareOptions`.
- `LocalStorageThemeStorage` (internal) - implements `IThemeStorageService` via `localStorage`.
- No UI code, tokens, or theme of its own.

**NuGet:** `Flare.Blazor` - depends on `Flare.Components`.

### samples/Flare.Gallery
Blazor WebAssembly PWA. Interactive component gallery with EN/RU language toggle, collapsible
syntax-highlighted code examples, and a live theme switcher (design system x palette x mode, plus
"generate a palette from a color"). Registers all five themes via `AddFlareTheme`. Docker-ready.

> `samples/Flare.Legacy` is a retained legacy sample and is not part of the published library.

---

## 2. Component Architecture

### FlareComponentBase Contract

```csharp
// Flare.Core.Components.FlareComponentBase
public abstract class FlareComponentBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter] protected IThemeService? ThemeService { get; set; } // theme operations
    [CascadingParameter] protected ThemeSnapshot? Theme { get; set; }        // current state (re-renders)
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    protected abstract string ComponentCssClass { get; }
    protected string BuildCssClass(params string?[] additionalClasses);
    public virtual ValueTask DisposeAsync();
}
```

Key contract rules for all components:
- `ComponentCssClass` returns the BEM block name (e.g. `"flare-btn"`).
- `BuildCssClass(...)` prepends `ComponentCssClass`, appends modifier classes, then appends the
  user-supplied `Class` parameter. Markup never builds the class list manually.
- `AdditionalAttributes` is forwarded via `@attributes` for arbitrary HTML attribute pass-through;
  `Style` is forwarded as the `style` attribute.
- Theme state arrives by **cascading value**: `FlareThemeProvider` cascades the `IThemeService` and
  an immutable `ThemeSnapshot`. When any axis changes, the snapshot reference changes and Blazor
  re-renders the consumers - no per-component event subscriptions.
- `IAsyncDisposable` is implemented - child classes that hold additional resources override
  `DisposeAsync` and call `await base.DisposeAsync()` first via the base virtual.

### CSS Naming Convention

All classes follow a BEM-variant pattern: `flare-[component]__[element]--[modifier]`.

| Context | Class |
|---------|-------|
| Block | `flare-btn` |
| Filled variant | `flare-btn--filled` |
| Leading icon slot | `flare-btn__icon flare-btn__icon--leading` |
| Active tab | `flare-tabs__tab flare-tabs__tab--active` |
| Sortable header | `flare-datagrid__th flare-datagrid__th--sortable` |
| Sticky app bar | `flare-appbar flare-appbar--sticky` |

All CSS rules consume only `var(--flare-*)` custom properties - no hard-coded colors, font sizes, or
animation timings.

### Compound Component Pattern (Cascading Parent)

Several components use a `CascadingValue` to pass a parent reference to children without explicit
binding:

1. **Parent registers children** (`FlareTabs`/`FlareTab`, `FlareDataGrid`/`FlareColumn`,
   `FlareAccordion`/`FlareAccordionPanel`): children register on mount and unregister on dispose;
   the parent owns state and re-renders after registration changes.
2. **Cascaded callback** (`FlarePopover`/`FlareMenu`): passes a close `Func<Task>` downward so a
   nested item can close its host without knowing the parent type.

---

## 3. Theming Architecture

### Three Axes

A rendered theme is the composition of three independently switchable axes:

1. **Design system** (`ITheme`) - typography, shape, motion, elevation geometry, component tokens.
2. **Palette** (`Palette`) - the colors, carrying a light and a dark `ColorScheme` (and optional
   high-contrast).
3. **Mode** (`ThemeMode`) - `Light` / `Dark` / `Auto` (Auto tracks `prefers-color-scheme`).

`IThemeService.SetThemeAsync` / `SetPaletteAsync` / `SetModeAsync` switch each axis at runtime.

### Token Hierarchy

```
ITheme
  +-- Id, DisplayName, DefaultPaletteId
  +-- StyleAssets (IReadOnlyList<string>)        - static CSS/fonts (anti-FOUC)
  +-- Palettes (IReadOnlyList<Palette>)          - colors that travel with the theme
  +-- PaletteGenerator (IPaletteGenerator?)      - design-system color rules (MD3 tonal / ramp)
  +-- ExtendedDarkOverride (dict?)               - rare dark-mode non-color extras
  +-- Design (DesignTokens)                       - the non-color half (mode-independent)
        +-- FocusRing (string)
        +-- Typography -> TypeStyle set (FontFamily, FontWeight, FontSize, LineHeight, LetterSpacing)
        +-- Shape, Elevation (geometry), Motion, State, Spacing
        +-- per-component token records (Button, Input, Select, Dialog, DataGrid, Card, ...)
        +-- Extended (dict) - theme-specific extras (e.g. Fluent focus-ring vars)

Palette
  +-- Id, Name, Source
  +-- Light (ColorScheme), Dark (ColorScheme)    - ~47 color roles each
  +-- HighContrast (ColorScheme?)
  +-- StyleAsset (string?)
```

`DesignTokens`, `ColorScheme`, and `Palette` are C# `record` types with `required init` properties -
construction is compile-time checked and immutable. Custom values are derived with `with`
expressions from the published reference instances (e.g. `Md3.LightColors with { Primary = "..." }`).

### Theme Delivery

`ThemeDelivery` selects how theme CSS reaches the document:

- **`ClassToggle`** (default, fastest) - the theme/palette/mode is a set of classes on `<html>`;
  switching is a class swap against a static, pre-generated stylesheet. `ThemeService` ensures the
  needed class-scoped CSS is present (`EnsureStaticCssAsync` / `RequireThemeAssetsAsync`).
- **`Inject`** - `ICssVariableInjector` flattens the active tokens via `CssVarMap` and writes them
  to `:root` through JS interop. SSR/prerender safe (JS-interop exceptions are swallowed; the static
  baseline CSS provides initial values).

### Anti-FOUC Bootstrap

A one-line bootstrap script (`_content/Flare.Components/js/flare-bootstrap.js`) applies the saved
theme/palette/mode classes before first paint, and each theme's `StyleAssets` provide the baseline
token CSS, so there is no flash of unstyled content.

### Persistence

`IThemeStorageService` (interface in `Flare.Core`, implemented by internal `LocalStorageThemeStorage`
in `Flare.Blazor`) reads/writes the selection in `localStorage`, SSR/prerender-guarded. `FlareThemeProvider`
restores the saved selection on first interactive render.

### Adding a New Theme

1. Create a `net10.0` Razor class library referencing `Flare.Core`.
2. Implement `ITheme` - provide `Id`, `DisplayName`, `Design` (a `DesignTokens`, typically derived
   from a reference theme with `with`), `DefaultPaletteId`, `Palettes`, and `StyleAssets`.
3. Ship the baseline/token CSS referenced by `StyleAssets` under `wwwroot/`.
4. Register it: `services.AddFlareTheme(new MyTheme());` (this also forces the assembly to load,
   which a bare reference does not in a trimmed/WASM app).

See [theme-creation-guide.md](theme-creation-guide.md) for the full walkthrough.

---

## 4. Service Layer

### IThemeService
Manages the three axes and applies their composition to the document.
- **State:** `CurrentTheme`, `CurrentPalette`, `Mode`, `IsDark`, `IsHighContrast`, `IsRtl`,
  `Delivery`, `Themes`, `Palettes`.
- **Switching:** `SetThemeAsync`, `SetPaletteAsync`, `SetModeAsync`, `SetRtlAsync`,
  `SetSystemDarkAsync`.
- **Registration:** `RegisterTheme`, `RegisterPalette`.
- **Generation:** `GeneratePalette(id, name, seed, source?)` using the current theme's generator.
- **Customization:** `CustomizeColors`, `CustomizeDesign`, `SetCustomToken(s)`,
  `ClearCustomToken`/`ClearAllCustomTokens`, `GetCustomTokens`.
- **Events:** `OnThemeChanged` (`event Func<Task>`) - fired after any axis change.
- **Implementation:** `ThemeService` (sealed). **DI lifetime:** Scoped.

### Registration model
`AddFlare` builds the `ThemeService` in a DI factory: it auto-discovers themes/palettes from loaded
assemblies (when `RegisterAllBuiltInThemes` is true), then adds those registered via `AddFlareTheme`
/ `AddFlarePalette`, then the configured `DefaultTheme`. Themes/palettes are registered directly into
the `ThemeService` instance (not as separate DI services) so the correct selection is available from
the first render.

> Auto-discovery only sees assemblies the app has actually loaded; a referenced-but-unused theme
> package may not be loaded in a trimmed/WASM app. Prefer explicit `AddFlareTheme`, which also forces
> the theme assembly to load.

### Other services registered by `AddFlare` (all Scoped)

| Service | Implementation |
|---------|----------------|
| `ICssVariableInjector` | `CssVariableInjector` |
| `IThemeService` | `ThemeService` (factory) |
| `IThemeStorageService` | `LocalStorageThemeStorage` |
| `ISnackbarService` | `SnackbarService` |
| `IDialogService` | `DialogService` |
| `IMessageBoxService` | `MessageBoxService` |
| `ICollisionService` | `CollisionService` |
| `IThemeJsService` | `ThemeJsService` |
| `IFlareClipboard` / `IFlareDownload` / `IFlareColorExtractor` | typed JS-interop wrappers |

`AddFlareIde()` (from `Flare.Components.IDE`) registers the IDE package's additional services.

### WASM vs. Server

Both hosts use the same code path. JS-interop services are loaded lazily and guard against
prerender by catching `InvalidOperationException` / `JSDisconnectedException`; the static baseline
CSS provides initial token values until interop is available. Scoped lifetime is per SignalR circuit
(Server) or per session (WASM).
