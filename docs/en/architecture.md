# Flare - Architecture

> [Русская версия ->](../ru/architecture.md) - [README](https://github.com/jrfrigat/Flare/blob/main/README.md)

How Flare is structured, how theming works, and the contracts components rely on.

---

## 1. Module Map

Flare is layered as a **clean / onion (ports & adapters)** stack. Dependencies point strictly inward;
the UI layer consumes **ports** (interfaces) and never references the host adapters. The composition
root (`Flare.Blazor`) is the only package that binds ports to their adapter implementations.

### Dependency Graph

```
            Flare.Abstractions   (Ring 0 - contracts)        no internal dependencies
               ^        ^      ^
               |        |      |
        Flare.Theming   |   Flare.Infrastructure   (Ring 1 engine / Ring 2 adapters)
        (-> Abstractions)   (-> Abstractions, Theming)
               ^                    ^
               |                    |
        Flare.Components   --------/   (Ring 3 - UI; -> Abstractions, Theming; NOT Infrastructure)
               ^
               |
        Flare (Flare.Blazor)   (Ring 4 - composition root; -> Components, Infrastructure)

Reference token packages (-> Flare.Abstractions only): hold the fully-populated baseline
DesignTokens each lineage derives from (the core itself ships no default token values):
  Flare.Theme.MaterialDesign3.Tokens (MaterialDesignTokens.Design),
  Flare.Theme.FluentUI2.Tokens (FluentUI2Tokens.Design)

Theme packages (each -> Flare.Abstractions + Flare.Theming + its lineage's *.Tokens package;
none referenced by the umbrella): the seven shipped design systems -
  Material lineage (-> MaterialDesign3.Tokens): .MaterialDesign3Expressive, .MaterialDesign3,
    .MaterialDesign2, .Aero, .LiquidGlass
  Fluent lineage (-> FluentUI2.Tokens): .FluentUI2, .VisualStudio

Optional component packages (each -> Flare.Components):
  Flare.Components.Carousel, .Kanban, .Transfer, .QrCode, .RichTextEditor, .Media, .IDE

samples/Flare.Gallery        -> Flare (umbrella) + all theme packages
tests/Flare.Core.Tests       -> Abstractions + Theming + Components + Infrastructure
tests/Flare.Components.Tests -> Abstractions + Theming + Components + Infrastructure
```

> **Flare ships no themes of its own.** The umbrella `Flare.Blazor` package depends on
> `Flare.Components` + `Flare.Infrastructure`. Each design system is an independent `Flare.Theme.*`
> package, so an app references only the ones it uses.

### Flare.Abstractions  (Ring 0 - contracts)
**Purpose.** The dependency-free core every other package builds on: the **ports** plus the
design-system model. No host or JS dependency.
- `Abstractions/` - the ports: `ITheme`, `IThemeService`, `IPaletteProvider`, `IPaletteGenerator`,
  `ICssVariableInjector`, `IThemeStorageService`, `IThemeJsService`, `IThemeValidator`,
  `ICollisionService`, `ISnackbarService`, `IDialogService`, `IMessageBoxService`,
  `IVersionCheckService`, and the JS-interop ports `IFlareClipboard`, `IFlareDownload`,
  `IFlareColorExtractor`, `ISplitterJsService`, `ITreeJsService`.
- `Tokens/` - immutable `record` token VALUE types: `DesignTokens`, `ColorScheme`, `Palette`,
  `TypographyTokens`, `ShapeTokens`, `ElevationTokens`, `MotionTokens`, `StateTokens`, `SpacingTokens`,
  `TypeStyle`, `PaletteSeed`, `ThemeMode`, `ThemeDelivery`, `ThemeSnapshot`, plus per-component records
  under `Tokens/Components/`.
- `Css/` - the CSS custom-property NAME registry (`Css.Tokens.*`, `Css.Classes.*`, `Vars`) and the
  `[CssVar]` attribute that links a token value property to the `--flare-*` name it populates.
- `JsInterop/` - `FlareJsModule`, the shared base for typed JS-interop services (used by adapters and
  add-on packs alike).
- `Security/` - pure `HtmlSanitizer` / `CssValidator` utilities (internal).
- No static web assets of its own.

**NuGet:** `Flare.Abstractions` - depends only on `Microsoft.AspNetCore.Components.Web`.

### Flare.Theming  (Ring 1 - engine)
**Purpose.** The theming engine - the application services that turn the token model into a rendered
theme. Depends only on `Flare.Abstractions`.
- `Services/` - `ThemeService` (orchestrates the three theme axes), `ScopedThemeService`,
  `TokensToCss`, `CssVarMap` (the token-to-CSS-variable flattener), `FlareBootstrap` (anti-FOUC script).
- `Palettes/` - `DefaultPaletteGenerator`, `PaletteFactory`.
- `Color/` - `ColorMath`, `FlareColor`, `FlareColorResolver`.
- `Builders/`, `Serialization/` - `FlareThemeBuilder`, `ThemeJsonSerializer`.

**NuGet:** `Flare.Theming` - depends on `Flare.Abstractions`.

### Flare.Infrastructure  (Ring 2 - adapters)
**Purpose.** The browser/host adapters - the concrete implementations of the ports. This is the only
ring that talks to `IJSRuntime`/`localStorage`. Depends on `Flare.Abstractions` (+ `Flare.Theming`
for the token flattening used by the CSS-variable injector).
- `JsInterop/` - `CssVariableInjector`, `CollisionService`, `ThemeJsService`, `SplitterJsService`,
  `TreeJsService`, and the typed `FlareClipboardService` / `FlareDownloadService` / `FlareColorExtractor`.
- `Storage/` - `LocalStorageThemeStorage`, `NullThemeStorage`.
- `Feedback/` - the UI-state services `DialogService`, `SnackbarService`, `MessageBoxService`.
- `VersionCheck/` - `VersionCheckService`.

**NuGet:** `Flare.Infrastructure` - depends on `Flare.Abstractions`, `Flare.Theming`.

### Flare.Components  (Ring 3 - UI)
**Purpose.** The UI components and nothing else - no service implementations live here. Each component
lives in its own sub-namespace folder; the base components are in `Base/`. Depends on
`Flare.Abstractions` + `Flare.Theming` and consumes adapters only through their ports (injected by DI).
**It does NOT reference `Flare.Infrastructure`** - that invariant is what makes the host swappable.
- Every component inherits `FlareComponentBase` (in `Base/`, namespace `Flare.Components`).
- CSS ships as a global, token-driven bundle in `wwwroot/css/` (aggregated into
  `flare-components.css`) - not scoped CSS. All rules consume `var(--flare-*)` tokens only.
- Hosts all static JS in `wwwroot/js/` (served at `_content/Flare.Components/js/`): the
  `flare-bootstrap.js` anti-FOUC head script and the lazily-imported interop ES modules. The
  Infrastructure adapters import these by URL (static assets have no assembly coupling).
- `Resources/` holds the EN/RU localization; `Theme/` holds the theme UI controls
  (`FlareColorCustomizer`, `FlareColorModeToggle`).
- Every `[Parameter]` carries a `/// <summary>` XML doc comment for IntelliSense on NuGet consumers
  (`GenerateDocumentationFile` is enabled solution-wide).

**NuGet:** `Flare.Components` - depends on `Flare.Abstractions`, `Flare.Theming`.

### Flare.Theme.* (seven design systems)
Each theme package provides one concrete `ITheme` plus its palettes and static style assets:

| Package | Theme class | `Id` | Default palette | Palettes |
|---------|-------------|------|-----------------|----------|
| `Flare.Theme.MaterialDesign3Expressive` | `Md3Theme` | `md3-expressive` | Violet | 5 |
| `Flare.Theme.MaterialDesign3` | `MaterialDesign3Theme` | `md3` | Violet | (shares MD3) |
| `Flare.Theme.MaterialDesign2` | `MaterialDesign2Theme` | `md2` | Purple | 6 |
| `Flare.Theme.FluentUI2` | `Fluent2Theme` | `fluent2` | Blue | 7 |
| `Flare.Theme.Aero` | `AeroTheme` | `aero` | Blue | 5 |
| `Flare.Theme.LiquidGlass` | `LiquidGlassTheme` | `liquid-glass` | Blue | 6 |
| `Flare.Theme.VisualStudio` | `VisualStudioTheme` | `visualstudio` | Blue | 5 |

A runtime-only **Dynamic Color** palette (`Palette.DynamicId = "dynamic"`) can also be enabled via
`FlareOptions.UseDynamicPalette`; it is generated from the OS accent through the active theme's generator.

- A theme = a design system (`DesignTokens`) + a `DefaultPaletteId` + `StyleAssets`. Light/dark is a
  **mode**, not a separate theme; colors come from a **palette**.
- Each package exposes public reference tokens (e.g. `Md3.DesignReference`, `Md3.LightColors`,
  `Md3.DarkColors`) so custom themes/palettes can be derived with `with` expressions.
- Each carries an `IPaletteGenerator` matching the design system's color rules (MD3 tonal / ramp).
- `StyleAssets` lists the static CSS the theme needs (fonts, base reset, generated token CSS) so
  the correct tokens are present on first paint (anti-FOUC).

**NuGet:** each theme package depends on `Flare.Abstractions` + `Flare.Theming` + its lineage's
reference token package (`Flare.Theme.MaterialDesign3.Tokens` or `Flare.Theme.FluentUI2.Tokens`),
from whose `Design` baseline it `with`-derives. The core stays default-free; the concrete values live
only in those two reference packages.

### Flare (umbrella / composition root)
**Purpose.** Single install target wiring up DI - the only ring that knows the Infrastructure adapters.
- `ServiceCollectionExtensions` - `AddFlare(opts)`, `AddFlareTheme(theme)`, `AddFlarePalette(palette)`
  and `FlareOptions`. `AddFlare` binds every port to its `Flare.Infrastructure` adapter.
- No UI code, tokens, or theme of its own. The adapter implementations (incl.
  `LocalStorageThemeStorage`) live in `Flare.Infrastructure`.

**NuGet:** `Flare.Blazor` - depends on `Flare.Components` + `Flare.Infrastructure`.

### samples/Flare.Gallery
Blazor WebAssembly PWA. Interactive component gallery with EN/RU language toggle, collapsible
syntax-highlighted code examples, and a live theme switcher (design system x palette x mode, plus
"generate a palette from a color"). Registers all five themes via `AddFlareTheme`. Docker-ready.

> `samples/Flare.Legacy` is a retained legacy sample and is not part of the published library.

---

## 2. Component Architecture

### FlareComponentBase Contract

```csharp
// Flare.Components.FlareComponentBase
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

### Compound Component Pattern (Cascading State)

Parent/child component families share state through a `CascadingValue`. The canonical form is a
**single typed cascading context object** the parent owns and cascades once; children consume it and
self-register. Prefer this over stacking several individually-named `CascadingValue`s.

1. **Typed context (preferred)** - `FlareTabs` cascades a `FlareTabsContext` (active tab + register/
   unregister/select callbacks + lazy flag); `FlareButtonGroup`, `FlareToggleGroup`, `FlareChipGroup`,
   `FlareRadioGroup` follow the same shape. `FlareTreeView` cascades one `FlareTreeContext` for the
   tree-wide drag config/callbacks/coordinator (per-row `Level` stays a separate cascade because it
   changes with depth).
2. **Parent registers children** - children register on mount and unregister on dispose; the parent
   owns state and re-renders after registration changes (`FlareTabs`/`FlareTab`,
   `FlareDataGrid`/`FlareColumn`, `FlareAccordion`/`FlareAccordionPanel`).
3. **Cascaded callback** - `FlarePopover`/`FlareMenu` pass a close `Func<Task>` downward so a nested
   item can close its host without knowing the parent type.

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
theme/palette/mode classes before first paint, so the page never flashes the wrong theme while the app
boots. Flare draws no loading splash itself - each app owns its own (background + animation), so it
matches the app's brand.

The script exposes a readiness signal instead: `window.hideFlareSplash()` dispatches a `flare:ready`
event and fades out the app's own splash element if it is tagged `id="flare-splash"` /
`[data-flare-splash]`. `FlareThemeProvider` fires it automatically (`ManageSplash`, default `true`): on
its first interactive render it applies the theme classes and static CSS, awaits each theme stylesheet's
`load` event and the document's web fonts (`document.fonts.ready`), then signals ready after the first
themed frame has painted. A safety timeout in the bootstrap fires anyway if the provider is absent or
boot fails; set `ManageSplash="false"` to signal readiness (`window.hideFlareSplash()` or the
`flare:ready` event) yourself.

### Persistence

`IThemeStorageService` (port in `Flare.Abstractions`, implemented by `LocalStorageThemeStorage` in
`Flare.Infrastructure`) reads/writes the selection in `localStorage`, SSR/prerender-guarded.
`FlareThemeProvider` restores the saved selection on first interactive render.

### Adding a New Theme

1. Create a Razor class library (net8.0/net9.0/net10.0) referencing `Flare.Abstractions` + `Flare.Theming`.
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
