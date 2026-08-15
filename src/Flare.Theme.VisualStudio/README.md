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

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
