# Flare gaps found while matching the Weir dashboard to its design comp

**Source:** aligning the Weir admin dashboard to its "Command Center" design comp (a dense DB-gateway
ops console). Flare `0.1.7`, single theme = Visual Studio 2026 geometry recoloured with a Command
Center palette. Follow-up to [weir-admin-flare-gaps.md](weir-admin-flare-gaps.md) (the app is otherwise
100% Flare) and to the now-resolved [flarechart-sparkline-mode.md](flarechart-sparkline-mode.md).

Two gaps blocked an exact match; both have working escape-hatches via a component `Style` prop, but
neither is expressible through a first-class parameter.

---

## 1. FlareChart sparkline height is aspect-locked to width (no fixed-pixel height)

**Severity:** medium -- the one that actually changes the visual.

**Need:** a full-width sparkline of a fixed, short pixel height (the comp uses `height:150px`, and Weir
wants ~120px) -- a wide, short strip whose height does **not** grow with the container width.

**What Flare offers today:** `Sparkline` + `Area` render correctly and stretch edge-to-edge (the
`0.1.7` additions work great). But `Height` sets the SVG **viewBox** height, not a CSS pixel height:
the `<svg class="flare-chart__svg">` is `width:100%` with `preserveAspectRatio="none"` and **no CSS
height**, so the browser derives the rendered height from the viewBox aspect ratio -- `renderedHeight =
containerWidth * Height / 400`. Measured in Weir: a chart in a 988px column with `Height="50"` renders
at **123px**; the same chart with `Height="90"` renders at **223px**. So a wide sparkline is tall, and
the only way to make it shorter is to keep lowering `Height` -- which is fragile because the result
still scales with width (a wider window makes every sparkline taller).

**Fallback used:** a scoped app-CSS override that pins the sparkline svg to a fixed height, leaning on
the `preserveAspectRatio="none"` the component already emits:
`.flare-chart--sparkline .flare-chart__svg { height: 120px; }`. This gives the true width-only
sparkline, but it is app CSS reaching into a Flare component's internals - exactly what the app tries
to avoid, and it would be unnecessary if `Height` mapped to a CSS pixel height in sparkline mode.

**Proposed enhancement:** in `Sparkline` mode (or behind an explicit flag), pin the SVG's CSS height to
`Height` px and keep `width:100%` + `preserveAspectRatio="none"` -- it already has the `preserveAspect`
half, so this is just emitting `height:{Height}px` (or `style="height:..."`) on the `<svg>`/plot. Then
`Height="120"` means "120px tall, full width" regardless of container width -- the actual sparkline
contract. Non-sparkline charts can keep the current aspect behaviour.

## 2. FlareLayoutAppBar has no height / density parameter

**Severity:** low.

**Need:** a slimmer app bar. The comp's top nav is **50px**; Flare's app bar renders at **64px**
(`--flare-appbar-height: 4rem`).

**What Flare offers today:** no `Height` or `Dense` parameter on `FlareLayoutAppBar`, and setting the
`--flare-appbar-height` / `--flare-layout-appbar-height` custom properties (on `:root` or the layout)
did **not** change the rendered header height -- it stayed ~64px (the bar's height is not driven by
those vars in a way an app can override). The bar is in normal flow (content flows directly beneath
it, no fixed offset), so a raw height override on the header does take effect and the content follows
correctly.

**Fallback used:** the whole bar is styled through the one `Style` prop:
`<FlareLayoutAppBar Style="height:50px;background:var(--flare-color-surface-container-low);border-bottom:1px solid var(--flare-color-outline);">`.
It works (measured: 50px bar, distinct surface, hairline border, tabs fit, content flush beneath), but
height, surface and border are all inline rather than parameters.

**Proposed enhancement:** a `Height` (any CSS length) and/or a `Dense` bool on `FlareLayoutAppBar`, plus
a `Surface` / `Bordered` option (see #3), and make the exposed `--flare-appbar-height` var authoritative
for the rendered bar height so an app can theme it. A dense, bordered app bar is a common request for
tool-window / IDE-style shells.

## 3. App bar has no distinct surface tone / bottom border

**Severity:** low.

Out of the box the app bar paints with the body/background tone (Weir palette `Surface` = `#0f1011`)
and no bottom border, so it does not read as a bar - it dissolves into the canvas. The comp's nav is a
slightly lifted surface (`#141517`) with a `1px` bottom border, which visually separates it. Matching
that needed inline `background` + `border-bottom` on the `Style` prop (see #2's fallback). A dedicated
app-bar surface role (defaulting to `Surface`; themes/apps can raise it) and a `Bordered` bool would
let an ops-console nav sit above the canvas without inline CSS.

---

## Summary

| # | Gap | Severity | Escape-hatch used |
|---|-----|----------|-------------------|
| 1 | `FlareChart` sparkline height scales with width (no fixed-px height) | medium | app-CSS `.flare-chart--sparkline .flare-chart__svg { height:120px }` |
| 2 | `FlareLayoutAppBar` no `Height`/`Dense` param (and the height var is not overridable) | low | `Style="height:50px;..."` |
| 3 | App bar has no distinct surface tone / bottom border (dissolves into the canvas) | low | `Style="...background:...;border-bottom:..."` |

None is a blocker; all three are "expressible only through a `Style` prop or an app-CSS override, not a
parameter" -- the same class of finding as the original Weir report. #1 is the one worth doing first
(it is the real sparkline contract); #2 and #3 together are the "make the app bar themeable" ask.
