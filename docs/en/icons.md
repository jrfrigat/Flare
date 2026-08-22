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

## Animating the swap

By default a change of `Value` replaces the glyph on one frame. `Morph` turns that replacement into a
transition: the outgoing and incoming glyphs share one box and trade places, which is what a state-carrying
icon - play/pause, menu/close, a check that appears on success - wants instead of a repaint.

```razor
<FlareIconView Value="@(_playing ? FlareIcons.Pause : FlareIcons.PlayArrow)" Morph="FlareIconMorph.Scale" />
```

| mode | what it does |
| :-- | :-- |
| `None` (default) | instant swap; the view renders the icon element alone, with no wrapper |
| `Fade` | cross-fade in place |
| `Scale` | cross-fade while the outgoing glyph shrinks away and the incoming one grows in |
| `Rotate` | cross-fade while the pair turns through the theme's angle |

The motion is the theme's: `--flare-icon-morph-duration`, `--flare-icon-morph-easing` (the movement curve -
the fade itself rides the theme's standard easing so a spring cannot cut it short), `--flare-icon-morph-scale`
and `--flare-icon-morph-rotate`. A theme that wants icon swaps to stay instant parks the duration; one that
wants `Scale` and `Rotate` to be plain cross-fades parks the two geometry tokens.

### Turning it on everywhere

Leave `Morph` unset and the mode comes from the enclosing scope, so an app switches the whole library on at
the root - Flare's own chrome included, from an expander's chevron to a select's caret:

```razor
<FlareThemeProvider IconMorph="FlareIconMorph.Scale">
```

Scope it to part of a page with a plain cascading value instead:

```razor
<CascadingValue TValue="FlareIconMorph?" Value="FlareIconMorph.Rotate"> ... </CascadingValue>
```

An explicit `Morph` on a call site always wins, `FlareIconMorph.None` included - which is how one icon opts
out of a scope that is on.

## Morphing the outline itself

`Morph` transitions *between two icons*. `FlareMorphIcon` is the other thing: one `<path>` element that
stays in the document while its geometry is interpolated, so the shape flows instead of one glyph fading
into another.

```razor
<FlareIconView Value="@(_open ? FlareMorphIcons.Minus : FlareMorphIcons.Plus)" />
```

No `Morph` parameter - the icon type carries the transition, and `FlareIconView` leaves such an icon alone
even when a mode is on (cross-fading it would replace the element whose geometry is being interpolated).

The catch is the same one that makes path interpolation impossible for the catalog at large: **the two
outlines must share one command list** - the same commands in the same order, differing only in
coordinates. `FlareMorphIcons` ships pairs drawn that way (`Plus`/`Minus`, `ChevronDown`/`ChevronUp`); for
your own, draw both shapes with the same command list and pad the simpler one with degenerate, zero-length
segments. A mismatched pair does not error - it swaps discretely half way through.

It uses the CSS `d` property, so it costs no JavaScript. The geometry is emitted as the `d` attribute too,
so where a browser does not implement that property the icon still draws and the change simply lands in one
frame.

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
