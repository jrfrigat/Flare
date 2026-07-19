# Icons

Flare icons are a **polymorphic value type**, not a single component. `FlareIcon` is an abstract descriptor;
each provider is a concrete `FlareIcon` that drops into any parameter typed `FlareIcon` (a button's `Icon`, a
nav item, a field adornment, ...) or renders standalone through `<FlareIconView>`.

Everything is **inline SVG by default** - no icon font, no network request, no flash of unstyled content, and
theme-agnostic (icons inherit `currentColor`).

All icon types live in the **`Flare.Icons`** namespace: `FlareIcon`, `FlareSvgIcon` and the built-in
`FlareIcons` set ship in the small `Flare.Icons` package; the provider packages below add their catalogs to
the same namespace. Add `using Flare.Icons` (or `@using Flare.Icons`). The render component `FlareIconView`
lives in `Flare.Components`.

## The built-in set: `FlareIcons`

The `Flare.Icons` package ships Flare's own dependency-free SVG set (`FlareIcons`, ~90 icons) that backs the
default component chrome (chevrons, close, sort, tree toggles, ...). `Flare.Components` depends on it, so it
works out of the box with no extra package.

```razor
<FlareIconView Value="@FlareIcons.Home" />
<FlareIconButton Icon="@FlareIcons.Settings" AriaLabel="Settings" />
```

- Always reference an icon by its **typed member** - there is no lookup by name string. That is deliberate:
  a name lookup would defeat trimming (the whole catalog would have to be kept) and add a runtime cost.
- `FlareIcons.All` and `FlareIcons.Find(id)` remain as an explicit catalog API for the built-in set (e.g. an
  icon-browser page that enumerates it); they do not resolve provider (Material/Fluent) icons.

## Provider packages

Core depends on no third-party icon set. Add only the package you need; each is optional.

| Package | Type / catalog | Delivery |
| :-- | :-- | :-- |
| `Flare.Icons.MaterialDesign3.Svg` | `MaterialDesign3Icons.Regular.*` / `.Filled.*` (3894) | inline SVG |
| `Flare.Icons.MaterialDesign2.Svg` | `MaterialDesign2Icons.*` (2122, filled) | inline SVG |
| `Flare.Icons.FluentUI.Svg` | `FluentUIIcons.Regular.*` / `.Filled.*` (~5000) | inline SVG |
| `Flare.Icons.MaterialDesign3.Symbols` | `FlareMaterialDesign3Icon` | Material Symbols variable webfont |
| `Flare.Icons.MaterialDesign2.Symbols` | `FlareMaterialDesign2Icon` | Material Icons webfont |
| `Flare.Icons.FontAwesome.Symbols` | `FlareFontAwesomeIcon` | Font Awesome webfont |

- The `.Svg` packages are self-contained (the SVG artwork is embedded) - nothing to load at runtime.
- The `.Symbols` packages render a `<span>`/`<i>` with the provider's font class; the **host app loads that
  font** (e.g. a Google Fonts `<link>` for Material Symbols, or a Font Awesome stylesheet).

```razor
@* SVG catalogs - self-contained *@
<FlareIconView Value="@MaterialDesign3Icons.Regular.Home" />
<FlareIconView Value="@MaterialDesign3Icons.Filled.Home" />   @* the same icon, filled *@
<FlareIconButton Icon="@FluentUIIcons.Regular.Settings" AriaLabel="Settings" />

@* Font providers - the host loads the font; axes/styles are provider options *@
<FlareIconView Value="@(new FlareMaterialDesign3Icon { Name = "home", Fill = true, Weight = 500 })" />
<FlareIconView Value="@(new FlareFontAwesomeIcon { Name = "house", Variant = FontAwesomeVariant.Solid })" />
```

The `.Svg` version of a set is preferred (self-contained, themeable, no FOUT); reach for `.Symbols` only when
you already load that webfont or specifically want the variable-font axes.

Which is "Fluent UI 2"? There is a single Microsoft icon set - **Fluent UI System Icons** - shipped here as
`FluentUIIcons`. It *is* the Fluent 2 icon set; there is no separate "FluentUI2" icon library.

## Custom SVG

Pass any SVG directly - path data or full inner markup - via `FlareSvgIcon`:

```razor
<FlareIconView Value="@(new FlareSvgIcon { Data = "M3 18h18v-2H3v2z" })" />
<FlareIconView Value="@(new FlareSvgIcon { Data = "<path .../><path .../>", ViewBox = "0 -960 960 960" })" />
```

> Security: `FlareSvgIcon.Data` (and any font provider `Name`) is emitted verbatim. Pass only trusted,
> developer-authored values - never untrusted or user input.

## Sizing & color

`FlareIconView` and every `FlareIcon` accept `Size` (any CSS length) or `SizePx`, and `Color` (a `FlareColor`
role or a custom color). Icons inherit `currentColor` otherwise, so they match surrounding text.

```razor
<FlareIconView Value="@FlareIcons.Star" SizePx="32" Color="FlareColor.Primary" />
<FlareIconView Value="@FlareIcons.Bolt" Size="3rem" Color="@FlareColor.Custom("#FFB300")" />
```

## Performance: only ship the icons you use

Every catalog icon is its own static member, and the SVG packages are marked `IsTrimmable`. So a **trimmed
Blazor WebAssembly publish** (the Release default) drops every catalog member you do not reference - you pay
only for the icons you actually use.

- **Reference icons by their typed member** (`MaterialDesign3Icons.Regular.Home`), not a string. A static
  member is traceable by the IL linker; a string name is not.
- Avoid rooting a whole catalog from always-loaded code (e.g. a "browse every icon" page that enumerates the
  type by reflection) - that keeps the entire set. Drive such pages from an explicit list of typed members.

Measured on the Flare Gallery: it references ~160 of the 3894 Material Symbols, and a trimmed publish shrinks
`Flare.Icons.MaterialDesign3.Svg` from **8.9 MB to ~180 KB**. A package you never reference (e.g.
`FluentUI.Svg` in that build) ships nothing at all.
