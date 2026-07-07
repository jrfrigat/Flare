# Flare capability gaps found while building the "Deka" design

**Source:** implementing the Claude Design mock *Deka Playlist Share* as a fully self-authored Flare
theme + UI in `PlaylistShared.Pwa` (Flare `0.1.1`).

**Overall:** the 0.1.x theme-agnostic model held up well. A bespoke theme with **zero `Flare.Theme.*`
dependency** was practical — every `DesignTokens` field and every component-token field is `required`,
so the theme supplies all values itself and no Material opinion leaks in. The gaps below are the places
where the *design* needed something the semantic-token / component model can't express today, so the app
fell back to custom CSS + `DesignTokens.Extended` (`--deka-*`) vars. None are blockers; each is a
candidate enhancement to widen what a theme/app can express without hand-written CSS.

---

## Status (addressed on `main`)

| # | Gap | Verdict | Outcome |
|---|-----|---------|---------|
| 1 | Bottom-sheet dialog | **Fixed in Flare** | `DialogOptions.Position` (`Center`/`Bottom`), `PanelClass`/`ScrimClass`, `DialogOptions.ShowGrabber`, and `IDialogService.ShowSheetAsync<T>` shipped. |
| 2 | Brand gradient | **Extended is correct (for now)** | Gradient is a `<image>`, not a color role - it does not belong in `ColorScheme`/`FlareColor`. Keep the `--deka-gradient-*` `Extended` vars. A first-class decorative *gradient fill* (background/text, separate from `Color`) is planned separately: see [flare-gradient-fill-proposal.md](flare-gradient-fill-proposal.md). |
| 3 | Glass / backdrop-blur | **Extended is correct** | A translucent surface with a baked alpha + `backdrop-filter` is a bespoke brand treatment; `--deka-glass` in `Extended` is the sanctioned home. An optional scalar `--flare-backdrop-blur` token is deferred (low value until multiple themes want it). |
| 4 | Faint / tertiary text tone | **Fixed in Flare** | Added the neutral `ColorScheme.OnSurfaceVariant2` role (CSS `--flare-color-on-surface-variant2`, `FlareColor.OnSurfaceVariant2`, `.flare-color-on-surface-variant2`) - a third step below `on-surface-variant`. The `2` suffix leaves room for a future `OnSurfaceVariant3`. Deka can drop `--deka-faint`. |
| 5 | Card media/overlay/FAB slot | **Extended/composition is correct** | `FlareCard` is already `position: relative; overflow: hidden`, so an absolutely-positioned scrim + corner-action compose on top today. A first-class `FlareCardMedia` overlay slot remains an optional nice-to-have, not required. |

The two "Fixed in Flare" items are verified (full build + Core/Components tests green + Gallery demo). The
per-section detail below is kept as the original report; the verdict above supersedes each "Proposed
enhancement".

---

## 1. `IDialogService` has no bottom-sheet (anchored) presentation

**Severity:** medium — the single biggest visual divergence from the mock.

**Design:** the *Share playlist* and *Add track to playlist* overlays are **slide-up bottom sheets** —
full-width, anchored to the bottom edge, rounded top corners only, a drag-grabber handle, and a
`translateY(100%) → 0` entrance.

**What Flare offers today:**
- `DialogOptions` exposes only `Size` (`Xs … Xl`, `FullScreen`), `AriaLabel`, `CloseOnScrimClick`,
  `CloseOnEsc`, `Divider`. The component dialog opened via `IDialogService.ShowAsync<T>(...)` is always
  **centered**. There is no `Position`/anchor and no bottom-sheet variant.
- `FlareDrawer Anchor="Bottom"` *can* render a bottom sheet declaratively (it has `Height` + a top/bottom
  `Radius`), but it is not wired to the imperative dialog model: no `FlareDialogInstance` cascade, no
  `DialogResult`/return value, no `DialogService`-style open-by-type-with-parameters.

**Consequence:** an overlay that (a) is opened imperatively with typed parameters, (b) returns a result,
and (c) presents as a bottom sheet is not expressible. The app opens both sheets as centered
`DialogSize.Sm` dialogs instead — functional, but not the mock's mobile bottom-sheet feel.

**Proposed enhancement:** add `DialogOptions.Position` (`Center` default, `Bottom`, maybe `Top`/`Fullscreen-sheet`)
— or a dedicated `IDialogService.ShowSheetAsync<T>` — that presents the same component-dialog contract as
a bottom sheet (grabber handle optional, rounded top, slide-up transition, safe-area padding). Ideally it
collapses to a centered dialog above a breakpoint so one call adapts across desktop/mobile.

**Status: DONE.** `DialogOptions.Position = DialogPosition.Bottom` (with `ShowGrabber`, `PanelClass`,
`ScrimClass`) and the `IDialogService.ShowSheetAsync<T>` sugar now present the exact same
component-dialog contract (typed parameters + cascaded `FlareDialogInstance` + `DialogResult`) as a
slide-up sheet: full-width, top-only radius, grabber, `env(safe-area-inset-bottom)` padding. The `Size`
max-width still applies, so on wide viewports it is a centered bottom card and on mobile it is full-width
(the existing `<600px` rule already bottom-aligns the scrim). `PanelClass`/`ScrimClass` also unblock the
glass-dialog and custom-animation cases without global CSS.

---

## 2. Palette roles are solid colors only — no brand-gradient support

**Severity:** low–medium.

**Design:** the brand mark, avatar initials, the primary-CTA "lift" shadow, and the boot/loading icon all
use `linear-gradient(150deg, <accent>, <accent-strong>)`.

**What Flare offers today:** every `ColorScheme` field is a single CSS color string. There is no gradient
token and no "brand gradient" role, so a gradient can't be themed through the palette.

**Workaround:** custom `Extended` vars — `--deka-accent-strong` (the gradient's second stop, with a
dark-mode override via `ExtendedDarkOverride`) and `--deka-gradient-accent`
(`linear-gradient(150deg, var(--flare-color-primary), var(--deka-accent-strong))`), consumed by hand in
scoped CSS.

**Proposed enhancement:** either (a) document `Extended` as the sanctioned home for brand gradients, or
(b) add an optional gradient surface — e.g. a `PrimaryGradient` role, or a small `GradientTokens` family
(from/to/angle) that components like `FlareAvatar`/`FlareButton` could opt into.

**Status: Extended is the sanctioned approach; a first-class gradient FILL is a separate planned task.**
A CSS gradient is an `<image>`, not a `<color>` - it cannot live in `ColorScheme`/`FlareColor` because the
`--fc-*` machinery is consumed by `color`/`background-color`/`border-color`/`color-mix()`/contrast math,
all of which are `<color>`-only and silently drop a gradient. So keep `--deka-gradient-*` in `Extended`
for now. The wanted "killer feature" (gradient background **and** text, opt-in) is designed as a distinct
`Background`/`Fill` concept separate from `Color`: see
[flare-gradient-fill-proposal.md](flare-gradient-fill-proposal.md).

---

## 3. No translucent / frosted-glass surface role (and no backdrop-blur token)

**Severity:** low–medium.

**Design:** the mini-player bar and the full-player header buttons are **frosted glass** — a
semi-transparent surface with `backdrop-filter: blur(20px) saturate(1.4)`.

**What Flare offers today:** all surface roles are opaque; none encodes translucency, and there is no
blur/backdrop token. A glass bar can't be built from the role set alone.

**Workaround:** `--deka-glass` extended var (a translucent surface color, dark-mode-overridden) plus a
hand-written `backdrop-filter` in scoped CSS.

**Proposed enhancement:** a translucent "glass"/"overlay-surface" role (e.g. `SurfaceGlass` with a baked-in
alpha per mode) and an optional `--flare-backdrop-blur` token, so bars/overlays can be themed as glass
without hardcoding alpha + blur in every app.

---

## 4. No intermediate "faint / tertiary" on-surface text tone

**Severity:** low.

**Design:** three text tones on surfaces — primary text (`on-surface`), muted labels
(`on-surface-variant`), and a **dimmer "faint"** tone for owner names, track counts, and footnotes.

**What Flare offers today:** the on-surface text ramp is `on-surface` → `on-surface-variant`, then it jumps
to `outline` (a border color, not really a text role). There is no third, fainter text tone.

**Workaround:** `--deka-faint` extended var (one step lighter than `on-surface-variant`, dark-mode-overridden).

**Proposed enhancement:** consider an `OnSurfaceFaint` (a.k.a. tertiary text) role in `ColorScheme` for the
common three-tier text hierarchy, so apps don't invent it per-theme.

**Status: DONE** (named `OnSurfaceVariant2`, not `OnSurfaceFaint`). Added a required neutral role
`ColorScheme.OnSurfaceVariant2` = CSS `--flare-color-on-surface-variant2`, with `FlareColor.OnSurfaceVariant2`,
`Colors.OnSurfaceVariant2` and the `.flare-color-on-surface-variant2` utility. It is a genuine third step on
the on-surface *text* ramp (a fainter alternative to `on-surface-variant`) - distinct from the Secondary/Tertiary
*accent* hues. The `Variant2` name follows the MD3 `variant` convention and leaves room for a future
`OnSurfaceVariant3`. Every in-box theme + the MD3/Fluent reference packages supply a value; Deka can drop
`--deka-faint` and use `FlareColor.OnSurfaceVariant2`.

---

## 5. (Minor) `FlareCard` has no first-class media/overlay + floating-action affordance

**Severity:** minor / nice-to-have.

**Design:** playlist tiles are a square cover image with a bottom gradient scrim, an overlaid count chip,
and a floating share/heart button pinned to a corner.

**What Flare offers today:** `FlareCard` is a solid container. The cover, scrim, overlaid chip, and floating
action are all custom CSS layered on top. This is arguably fine (cards are composable), but a
`CardMedia`/cover slot with an overlay layer + a corner-action slot would remove a lot of repeated custom
CSS for media-grid UIs.

**Proposed enhancement:** an optional media/cover region on `FlareCard` (image + gradient-scrim overlay
slot + top-corner action slot).

---

## Notes for reproduction

- Theme source: `PlaylistShared.Pwa/Theming/DekaTokens.cs`, `DekaColors.cs`, `DekaTheme.cs`,
  `DekaPalette.cs`. All custom vars live in `DesignTokens.Extended` (+ `DekaTheme.ExtendedDarkOverride`
  for the dark-mode values of `--deka-accent-strong` / `--deka-glass` / `--deka-faint`).
- Custom CSS that a fuller Flare surface would replace: `PlaylistShared.Pwa/wwwroot/css/deka.css`
  (glass bars, gradient marks, bottom-sheet-ish styling, faint text) and the two sheet components under
  `Components/Common/`.
