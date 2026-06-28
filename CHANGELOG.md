# Changelog

All notable changes to Flare are documented here. This project adheres to
[Semantic Versioning](https://semver.org/).

## [0.0.5] - 2026-06-28

### Added
- **`FlareNavMenu` framed layout** with new `Header` / `Footer` slots. Setting either slot pins that
  region while `ChildContent` scrolls between them, filling the menu's container height; the pinned
  regions hold ordinary nav items so they still collapse to icons in a mini-rail. New
  `flare-nav-menu--framed` / `__header` / `__scroll` / `__footer` / `__meta` CSS classes.
- **`IVersionCheckService.CurrentVersion`** - the version the app is currently running (in
  service-worker mode the first version read from the deployed assets manifest, otherwise the
  configured `CurrentVersion`), distinct from `LatestVersion`, which is only set once a newer build
  is detected.
- **Snackbar update-in-place**: a new `ISnackbarService.Show(SnackbarMessage)` overload that
  preserves the message `Id`, an `Update(SnackbarMessage)` method and `OnUpdate` event that replace
  a shown snackbar in place (keeping its position in the stack), and a `SnackbarMessage.ShowProgress`
  flag that renders an indeterminate progress bar below the message - e.g. morphing a "new version
  available" toast into an "updating..." one. New `flare-snackbar--with-progress` / `__progress`
  classes.

### Changed
- **Ribbon command heights aligned** (`Flare.Components.IDE`): icon-only, icon + label and dropdown
  commands now all stretch to the group height instead of floating at their intrinsic sizes; large
  commands stack the icon over the label via `.flare-btn__label`, and the dropdown caret no longer
  inherits the large icon size.
- **Gallery: IDE components split into their own pages** (Backstage, DocumentTabs, FormulaBar,
  MenuBar, PropertyGrid, QuickAccessToolbar, Ribbon, SheetTabs, StatusBar, ToolPanel, Toolbar) with
  focused per-component demos, replacing the single combined IDE page.
- **Gallery: new Settings page** consolidating the design-system / palette / mode theme switcher and
  the language toggle, reachable from the nav menu, which now also surfaces the running Gallery
  version.

### Fixed
- **A PWA on-demand update no longer lands back on the old version.** The service worker now calls
  `clients.claim()` on activate, and `flare-version-check.js` drives the pending worker through
  `skipWaiting` and reloads on the single resulting `controllerchange` instead of a fixed 10s timer -
  so the reload is always served by the new worker and never falls back onto the stale cache (the
  cause of the "dev"/previous version that previously needed a hard reload). The samples'
  `service-worker.published.js` adds the matching `clients.claim()`.
- **No stray focus ring on the page heading after navigation.** Blazor's `FocusOnNavigate` focuses
  the page heading (`tabindex="-1"`) after every navigation, which Chrome's `:focus-visible`
  heuristic painted a ring on even though it was not a keyboard interaction; the ring is now
  suppressed on focused `h1`-`h6[tabindex="-1"]` headings, while real interactive controls keep
  theirs.

## [0.0.4] - 2026-06-27

### Changed
- **Anti-FOUC splash is now revealed automatically by `FlareThemeProvider`** (new `ManageSplash`
  parameter, default `true`). The provider waits for the theme stylesheets (`load` event) and the
  document's web fonts (`document.fonts.ready`), then fades the bootstrap splash out after the first
  themed frame - so apps no longer flash unstyled content and no longer need to call
  `window.hideFlareSplash()` by hand. `IThemeJsService.EnsureStylesheetAsync` now resolves only once
  the stylesheet has loaded; new `WhenFontsReadyAsync` / `RevealAppAsync`. A safety timeout in
  `flare-bootstrap.js` (overridable via `data-splash-timeout`) reveals the page even without the
  provider.
- The Gallery and Legacy samples drop their hand-written `hideFlareSplash()` wiring; the splash is now
  revealed by `FlareThemeProvider` out of the box.

### Fixed
- **A PWA update no longer crashes the render when a stale service-worker cache serves a previous
  `flare-theme.js`.** Because `_content/.../js` modules load at a fingerprint-free URL (unlike the
  fingerprinted framework files), an updated build's C# could call `whenFontsReady`/`revealApp` on an
  older cached module and throw a `JSException` in `OnAfterRenderAsync`. `FlareThemeProvider` now
  catches it and falls back to the global `window.hideFlareSplash()`, so the app is never crashed or
  stranded during an update; the full fonts-ready + framed-fade path resumes once the new worker
  activates.

## [0.0.3] - 2026-06-27

### Added
- **Two new design systems**: **Material Design 3** (baseline, non-Expressive) and **Material Design 2**.
  Flare now ships 7 themes (MD3 Expressive, MD3, MD2, Fluent UI 2, Aero, Liquid Glass, Visual Studio 2026).
- **Dynamic Color palette** (`Palette.DynamicId`): an opt-in palette generated at runtime from the
  OS/browser accent color (Windows/macOS accent, Android Material You via the CSS `AccentColor`
  system color) through the *active theme's* generator, so it adapts to every theme. Enabled via
  `FlareOptions.UseDynamicPalette` / `DynamicPaletteFallbackSeed`; falls back to a seed where the
  accent is unavailable. New `IThemeService.ApplyDynamicPaletteAsync` / `IsDynamicPalette` and
  `IThemeJsService.GetAccentColorAsync`.

### Changed
- **Token model cleanup**: common component tokens that themes set through the `Extended` bag now
  have typed homes - new `NavTokens`, plus added `InputTokens` hover, `MenuTokens` group/island and
  `ProgressTokens` track/stop/wave fields. All themes set these via typed `DesignTokens` records;
  `Extended` is reserved for genuinely theme-specific keys.
- **Fluent UI 2** typography and motion aligned to the official Fluent 2 design tokens (Semibold
  heading ramp, real `fontSize`/`lineHeight`/duration values; emphasized easing = `curveEasyEase`).
- Solution folders split into `src/Core` and `src/Themes`.

### Fixed
- **MD3 Expressive** palette aligned to the updated spec: light-mode `on-*-container` roles now use
  tone 30 (were tone 10), in both the static palette and the tonal generator.
- **Visual Studio 2026** connected document tabs - the active tab now takes the editor surface color
  (was a lighter floating gray in dark mode).
- Gallery home "design systems" stat is now bound to the registered theme count (was hardcoded).

## [0.0.2] - 2026-06-27

### Architecture
- Rebuilt as a clean onion / ports-and-adapters stack with 5 rings - `Flare.Abstractions`
  (contracts + design-token model + CSS registry), `Flare.Theming` (engine), `Flare.Infrastructure`
  (JS-interop/storage/feedback adapters), `Flare.Components` (UI only) and `Flare.Blazor` (composition
  root). Dependencies point strictly inward; `Flare.Components` no longer ships service implementations,
  and the old `Flare.Core` grab-bag was retired. Namespaces realigned to the rings.

### Added
- **Multi-targeting**: the libraries are .NET 10-first but now also target **net8.0** and **net9.0**
  (per-TFM ASP.NET Core versions; no net10 regression - identical code).
- **`ITheme.Derive(...)`** to tweak a built-in theme by composition instead of subclassing.
- **Id constants** on every theme (`<Theme>.ThemeId`) and palette set (`<Palettes>.<Name>Id`) for
  string-free theme/palette switching.
- **`[CssVar]`** attribute linking every token value to its `--flare-*` name (guarded by a drift test),
  and typed `Vars.Var(Css.Tokens.*)` token values instead of magic `var(--flare-*)` strings.

### Changed
- Compound components (Tree, Menu/SubMenu, FAB) standardised on a single typed cascading context.

### Fixed
- `FlareColorModeToggle` instances now stay in sync: the toggle reflects the live theme mode, so
  switching the mode anywhere updates every toggle.

## [0.0.1] - 2026-06-23

Initial public release of Flare - a production-ready Blazor component library for .NET 10.

### Components
- 100+ components across inputs, layout, navigation, data display, feedback, display and utilities,
  all inheriting `FlareComponentBase` with unified `Class`/`Style`/attribute forwarding.
- Inputs: Button, Input, TextArea, Checkbox, Switch, Radio/RadioGroup, Select, MultiSelect,
  Autocomplete, NumericField, Slider (single + range, sizes XS-XL, vertical, steps, value bubble),
  Rating, ColorPicker, TagInput, InputMask, PasswordInput, DatePicker, TimePicker, DateRangePicker,
  ToggleButton, ButtonGroup, SplitButton, FileUpload, FormBuilder/Form with `DataAnnotationsValidator`.
- Select & MultiSelect share a fully themed popover dropdown (rounded surface, elevation, grouping,
  keyboard navigation) via the shared `flare-listbox` styles, with an `ItemTemplate` for custom
  option markup.
- Layout: Stack, Grid/Col, Container, Hidden, Card, Paper, Divider, and the Layout/AppBar/Drawer set.
- Navigation: AppBar, NavMenu/NavGroup/NavLink (nested, auto-expanding), Tabs, Accordion, Breadcrumb,
  Pagination, Stepper, Drawer.
- Data display: DataGrid, Table, VirtualList, InfiniteScroll, TreeView, VirtualTree, List, Timeline,
  Chart (SVG line/bar/pie/donut, no dependency), Calendar, Carousel, Kanban, Transfer.
- Feedback: Dialog/DialogProvider, MessageBoxProvider, Alert, Snackbar, Progress, Skeleton, Tooltip,
  Overlay, EmptyState.
- Display: Typography (FlareText), Avatar/AvatarGroup, Badge, Chip/ChipGroup, Icon, Image, Link,
  Popover, Highlighter, ScrollTop.
- Utilities: Menu/MenuItem, SpeedDial, DropZone.
- Full XML documentation on every public type and `[Parameter]` for IntelliSense.

### Theming
- Runtime theme switching across three independent axes - design system, color palette, and
  light/dark/auto mode - with no page reload and no flash of unstyled content.
- Five design systems shipped as independent packages: Material Design 3 Expressive, Fluent UI 2,
  Aero, Liquid Glass, and Visual Studio 2026. The umbrella `Flare` package ships no theme of its own.
- 28 built-in palettes across the themes; each palette carries light + dark (and optional
  high-contrast) color schemes.
- Class-toggle delivery (default) swaps theme classes on `<html>`; CSS-variable injection available
  as a fallback (`ThemeDelivery`).
- Auto dark mode tracks `prefers-color-scheme`; a one-line bootstrap script applies the saved
  selection before first paint.
- Palette generation from a seed color (`Palette.FromColors`, `IThemeService.GeneratePalette`) using
  each design system's color rules (MD3 tonal / Fluent ramp).
- Runtime customization: `CustomizeColors`, `CustomizeDesign`, and per-token overrides; RTL and
  high-contrast support via CSS logical properties. `FlareThemeBuilder` and JSON theme serialization.

### Color API
- Unified `FlareColor` color parameter on every color-aware component: a semantic role
  (`FlareColor.Primary`) maps to a cached theme class, or any custom value
  (`FlareColor.Custom("#E91E63")`) emits sanitized inline tokens. Implicit conversion from `string`.

### DataGrid
- State-driven architecture (`DataGridState<T>` + `DataGridCommands` + single-pass pipeline + cache).
- Multi-sort, column resize/reorder, row reorder, row selection, type-aware inline editing, batch
  editing, and Excel-like cell selection (keyboard active cell, range selection, Ctrl+C/Ctrl+V).
- Type-aware columns (`ColumnDataType`, `Format`, alignment), per-column sort/filter strategies, and
  stable column identity used consistently for sort/filter/visibility/order/persistence.
- Filtering: auto filter row and Excel-style header filter menu (searchable distinct-value list),
  standalone AND/OR filter builder, named filter presets, debounced quick filter, and value-aware
  comparison for numbers/dates/times.
- Grouping with footer aggregates, column bands (nested), composite columns (banded / card),
  tree-grid, conditional row/cell formatting, virtualization, interactive column visibility.
- `IQueryable`/EF Core server-side translation (`DataGridQuery`) and exporters for
  CSV/TSV/JSON/Markdown/Excel (.xlsx)/PDF (dependency-free), with a pluggable `IDataGridExporter`.
- Keyboard navigation, ARIA grid roles, and localized (EN/RU) screen-reader announcements.

### Packages
- Core: `Flare.Core`, `Flare.Components`, `Flare` (umbrella, `AddFlare`/`AddFlareTheme`/`AddFlarePalette`).
- Themes: `Flare.Theme.MaterialDesign3Expressive`, `Flare.Theme.FluentUI2`, `Flare.Theme.Aero`,
  `Flare.Theme.LiquidGlass`, `Flare.Theme.VisualStudio`.
- Optional component families: `Flare.Components.Carousel`, `.Kanban`, `.Transfer`, `.QrCode`,
  `.RichTextEditor`, `.Media` (SignaturePad, VideoPlayer, FileUpload), `.IDE` (Ribbon, DocumentTabs,
  ToolPanel, Splitter, StatusBar, MenuBar).
- `Flare.Icons` (~10,700 Material Symbols paths, recommended to lazy-load in WASM apps).

### Services
- `IVersionCheckService` (`AddFlareVersionCheck`) - a headless service that polls for a newer app
  version on a configurable interval and raises `NewVersionAvailable`; the consumer decides how to
  surface it (e.g. a toast). Renders nothing itself. Built-in `UseServiceWorker` mode registers the
  service worker (`ServiceWorkerPath`), reads the deployed version from the Blazor PWA assets
  manifest, and applies a waiting update via `ApplyUpdateAsync` - so apps need no service-worker
  registration/update JS of their own. Or supply your own `CheckForLatestVersion` probe (e.g. a
  `version.json` poll) for the non-PWA case.

### Tooling & platform
- Targets .NET 10. Blazor Server, WebAssembly and SSR compatible; JS interop is prerender-guarded.
- No Bootstrap or third-party CSS - all styles use `var(--flare-*)` tokens.
- Interactive Gallery PWA with EN/RU switching, syntax-highlighted examples, and a live theme
  switcher; Docker-ready (`docker compose up --build`).
- NuGet packaging with SourceLink, symbol packages, and MinVer-derived versioning.
