# Remove the string -> FlareIcon conversion; move icon usage to typed values

**Status: DONE.** The `implicit operator FlareIcon(string)` and `FlareIconView`'s `Name`/`Icon` string
shortcuts are removed; all icon usage across core, samples and tests is typed. `FlareIconView` now takes only
a typed `Value`. `FlareIcons.Find(id)`/`FlareIcons.All` are kept as an explicit built-in-catalog API (used by
the icon-browser page), which does not defeat trimming. The historical context below is left for reference.

## Context

Core `Flare.Components` is now independent of any third-party icon set. A bare string still resolves to an
icon for convenience:

- `implicit operator FlareIcon(string)` -> `FlareIcons.Find(id) ?? FlareIcons.Empty`
- `FlareIconView`'s `Name` / `Icon` string shortcuts use the same rule.

It resolves ONLY against the built-in SVG set (`FlareIcons`, ~84 ids) and **never** falls back to a Material
font. An id that is not built in resolves to `FlareIcons.Empty` (an invisible icon).

## The problem

Because there is no fallback, any call site that passes a non-built-in id as a bare string renders nothing.
The Gallery still has demo sites using ids that are not in the built-in set (e.g. `volume_up`, `content_cut`,
`north_west`, `zoom_in`, ...) - those now show an empty icon until migrated to a typed value.

## The migration

Drop the string convenience entirely and require typed icons everywhere:

- Built-in: `Icon="@FlareIcons.Home"`.
- Material (font): `@(new FlareMaterialDesign3Icon { Name = "volume_up" })` (package `Flare.Icons.MaterialDesign3.Symbols`).
- Material (SVG): `@MaterialDesign2Icons.VolumeUp` (package `Flare.Icons.MaterialDesign2.Svg`).
- Fluent (SVG): `@FluentUIIcons.Regular.Speaker2` (package `Flare.Icons.FluentUI.Svg`).
- Font Awesome (font): `@(new FlareFontAwesomeIcon { Name = "volume-high" })` (package `Flare.Icons.FontAwesome.Symbols`).

Steps:

1. Rewire every `<FlareIconView Name="..."/>` / `Icon="@("...")"` / `Icon="@("...")"` call site (~400, mostly
   Gallery demos) to a typed value from the appropriate package. All the SVG catalogs now ship, so no source
   is blocking: built-in `FlareIcons.*`, `MaterialDesign2Icons.*`, `MaterialDesign3Icons.Regular/Filled.*`,
   `FluentUIIcons.Regular/Filled.*`. Font Awesome is intentionally font-only
   (`Flare.Icons.FontAwesome.Symbols`); there is no `.Svg` variant.
2. Remove `implicit operator FlareIcon(string)` from `FlareIcon`.
3. Remove the `Name` / `Icon` (string) shortcuts from `FlareIconView`, leaving only `Value`.
4. `FlareIcons.Empty` can stay as an explicit "no icon" value or be removed with the conversion.
