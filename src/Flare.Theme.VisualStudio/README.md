# Flare.Theme.VisualStudio

Visual Studio (2022/2026-inspired) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library.

```sh
dotnet add package Flare.Theme.VisualStudio
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new VisualStudioTheme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new VisualStudioTheme());
// await ThemeService.SetThemeAsync("visualstudio");
```

## Visual Studio shell -> Flare

Visual Studio is a product, not a published design system, so this table maps the parts of its SHELL -
the things you would point at in the IDE - onto the components that reproduce them. Flare's IDE family
exists for exactly this shape of application.

| Visual Studio | Flare | Notes |
| :-- | :-- | :-- |
| Menu bar (File, Edit, View...) | `FlareMenuBar` | |
| Toolbar / command bar | `FlareToolbar`, `FlareQuickAccessToolbar` | |
| Ribbon (Office-style hosts) | `FlareRibbon` | |
| Document tabs | `FlareDocumentTabs` | |
| Tool windows (Solution Explorer, Properties) | `FlareToolPanel` | docked panes |
| Solution Explorer tree | `FlareTreeView`, `FlareDataTree` | |
| Properties window | `FlarePropertyGrid` | |
| Status bar | `FlareStatusBar` | |
| Splitter between panes | `FlareSplitter` | |
| Output / error list | `FlareDataGrid` | |
| Options dialog | `FlareDialog` + `FlareNavMenu` | |
| Command palette (Ctrl+Q) | `FlareCombobox`; `FlareShortcuts` documents the bindings | |
| Backstage (File menu) | `FlareBackstage` | |
| The whole shell | `FlareIdeLayout` | composes the above into one frame |

Everything else - buttons, fields, checkboxes, grids - is the standard Flare component set wearing
this theme's tokens; VS does not rename them.

## What this theme changes beyond colour

- **Dense by default.** The type ramp and control heights are tuned for an IDE, not for touch.
- **Square corners and 1px strokes.** The shape scale is close to zero throughout.
- **Selection is a flat fill, not a tonal wash** - the same discrete-state model Fluent uses, which is
  why the theme sets its state-layer tokens rather than relying on the Material default.

## VS 2026 chrome roles

The dark shell follows the measured Visual Studio 2026 palette: menu/title bar `#1C1C1C`, command
bar and document tab strip `#262626`, active tab and tool windows `#282828`, editor `#1E1E1E`,
breadcrumb `#2B2B2B`, bottom panel `#1C1C1C`, status bar `#141414`, separators `#0E0E0E`/`#3D3D3D`,
outlines `#454545`, menu `#2C2C2C`, hover `#383838` and the `#9184EE` accent, drawn as 1px focus
lines. The light shell keeps its existing values.

The surfaces are exposed as theme variables, so a host can paint a chrome region without overriding
a generic color role:

| Variable | Region |
| :-- | :-- |
| `--flare-vs-chrome-title-bar` | Menu and title bar |
| `--flare-vs-chrome-command-bar` | Command bar |
| `--flare-vs-chrome-tab-strip` | Document tab strip |
| `--flare-vs-chrome-tab-active` | Active document tab |
| `--flare-vs-chrome-editor` | Editor |
| `--flare-vs-chrome-breadcrumb` | Breadcrumb bar |
| `--flare-vs-chrome-tool-window` | Tool window |
| `--flare-vs-chrome-tool-tab` | Tool tab strip and title |
| `--flare-vs-chrome-field` | Tool search field and input well |
| `--flare-vs-chrome-bottom-panel` | Bottom panel tab strip |
| `--flare-vs-chrome-status-bar` | Status bar |
| `--flare-vs-chrome-bar-edge` / `--flare-vs-chrome-divider` | Bar edge and separators |
| `--flare-vs-chrome-outline` | Control outlines |
| `--flare-vs-chrome-menu` / `--flare-vs-chrome-hover` | Menu panel and hover |
| `--flare-vs-chrome-accent` | Focus and selection accent |
| `--flare-vs-chrome-scrollbar-thumb` / `--flare-vs-chrome-scrollbar-track` | Scrollbar |

The optional IDE shell in `Flare.Components.IDE` reads the same roles through its own
`--flare-ide-*` hooks, which this theme fills in, so `FlareIdeLayout` renders the measured surfaces
without host overrides.

## VS 2026 component metrics

- **Fields.** A 23px well with a `#353535` fill, a 1px `#454545` stroke, 4px corners and an 8px text
  inset. The Xs..Xl ramp is 20/22/23/26/30px.
- **Command buttons.** 24px square, 16px glyph, 4px corners.
- **Focus.** A single 1px `#9184EE` accent line on fields, buttons, checkboxes and radios; no outer
  glow.
- **Scrollbars.** A 2px square `#9D9D9D` thumb on the panel surface. Chromium and WebView2 draw this
  from the WebKit scrollbar path, which the theme keeps enabled; Firefox keeps its native scrollbar.
- **Panels.** 6px corners on dropdowns and popovers; a tool search field takes the same 6px through
  `--flare-vs-chrome-search-radius`.

## VS 2026 type and icon scale

- **Chrome text.** Status bar, tool tabs and tool titles are 12px (BodyMedium); menu and tree labels
  stay 13px (BodyLarge).
- **Tool titles.** Sentence case, without the uppercase caption transform.
- **Icons and rows.** Tree rows are 24px high; tree and command icons are 16px, while the expander
  keeps a 24px target around its glyph.

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
