# MD3 Expressive theme - Flare fidelity audit

Companion to [`fui2-theme-fidelity.md`](fui2-theme-fidelity.md). Where the FUI2 audit found the
whole **state model** diverging (Flare core is MD3-native, so FUI2 needed override CSS), this audit
is the opposite case: the MD3 Expressive theme ships **pure token values on a MD3-native core** and
has **zero override CSS**. So most per-component states are correct by construction; the interesting
findings are (a) **Expressive-signature behaviors** the core cannot yet express, (b) exact
token-value divergences, (c) deliberate practicality deviations.

Status: IN PROGRESS (foundation verified in-thread; per-component sections being filled from the
parallel auditors). Analysis only - no fixes applied yet.

---

## 0. Method & sources

- Theme tokens: `src/Flare.Theme.MaterialDesign3.Tokens/MaterialDesignTokens.cs` (one file: per-component
  `XxxTokens` records + `LightColors`/`DarkColors`). The `Flare.Theme.MaterialDesign3` project itself
  has **no `wwwroot/css`** - confirmed via glob. So the theme = values only.
- Core render CSS: `src/Flare.Components/wwwroot/css/<comp>.css` (global bundle, MD3-shaped).
- Core state engine: `src/Flare.Components/wwwroot/css/state-layer.css` + per-component `::before`.
- Specs: `docs/spec/<comp>/md3-expressive-spec.md` (+ `_pallete`, `_foundation`).

---

## 1. Foundation axes (verified in-thread) - all MATCH except spring motion

| Axis | MD3 Expressive spec | Flare MD3 theme | Verdict |
|------|---------------------|-----------------|---------|
| State-layer opacity - hover | 0.08 | `HoverOpacity 0.08` | MATCH |
| State-layer opacity - focus | 0.10 | `FocusOpacity 0.10` | MATCH |
| State-layer opacity - pressed | 0.10 | `PressedOpacity 0.10` | MATCH |
| State-layer opacity - dragged | 0.16 | `DraggedOpacity 0.16` | MATCH |
| State-layer opacity - selected | 0.12 | `SelectedOpacity 0.12` | MATCH |
| Disabled content opacity | 0.38 | `DisabledOpacity 0.38` | MATCH |
| Disabled container opacity | 0.12 | `DisabledContainerOpacity 0.12` | MATCH |
| Shape scale | 0 / 4 / 8 / 12 / 16 / 28 / full | `None..Full = 0/4/8/12/16/28/9999` | MATCH |
| Elevation | 5 levels, umbra+penumbra | `Level0..5` matches MD3 geometry | MATCH |
| Type scale | Roboto display->label ramp | `Typography` = MD3 baseline ramp | MATCH |
| Color roles | MD3 baseline + Expressive on-container tone 30 | `LightColors`/`DarkColors` match; on-*-container = tone 30 | MATCH |
| **Motion - springs** | **Expressive = physics springs (damping/stiffness)** | cubic-bezier + fixed durations only; **no spring primitive** | **GAP (foundational)** |

**Foundational finding F1 - no spring motion primitive.** MD3 Expressive's signature is spring-based
motion (e.g. button press shape-morph spring damping 0.9 / stiffness 1400). Flare's `MotionTokens` are
all `cubic-bezier(...)` easings + fixed `ms` durations. CSS cannot run a true spring, but the Expressive
"springy overshoot" is normally approximated with a tuned `linear()` easing or a keyframe. Flare has
**no such token/primitive**, so every Expressive morph that depends on it degrades to a plain ease.
This is the root of several per-component NOT-IMPL findings below and is a candidate **core** addition
(a spring-approximation easing token + optionally a shape-morph transition helper).

---

## 2. Cross-cutting Expressive gaps (seeded; expand from auditors)

- **F1 spring motion** (above) - no spring/overshoot easing primitive.
- **Shape morph on interaction** - MD3 Expressive morphs container corners on press (round->squircle)
  and on toggle-select (shape inversion). Flare's state engine only animates `::before` opacity; there
  is no core mechanism to animate `border-radius` per size/state. Button confirmed locked to
  `shape-full` at all sizes (see buttons section).

---

## 3. Per-component findings

Verdict legend: MATCH / MINOR (cosmetic drift) / GAP (real divergence) / NOT-IMPL (Expressive behavior
absent) / DELIBERATE (intentional deviation). Only non-MATCH axes listed; unlisted axes match spec.

### Buttons family
- **Button** - GAP heights md/lg/xl = 48/56/64dp vs spec 56/96/136 (xs/sm match); GAP press shape-morph
  is opt-in (`PressMorph` default off) + wrong targets + no spring; MINOR outline width 1px all vs
  1/1/1/2/3; MINOR "Square" variant = 0dp vs spec square = rounded-rect. Icon/label/typo/state opacities
  /elevation MATCH. (Heights are a DELIBERATE practicality cap - see decisions.)
- **Split-button** - GAP inner-corner hover/press morph (grow) + trigger-round-when-open not implemented
  (only caret rotates). Rest inner corners 4/4/4/8/12dp MATCH; inherits button height gap.
- **Button-group** - GAP renders the legacy connected segmented row (flat interior, -1px border overlap,
  8dp outer) instead of the Expressive gapped-pill Connected + Standard variants; press/select corner
  morph + standard width-grow NOT-IMPL.
- **FAB** - GAP padding-sized (sm/md/lg) instead of fixed 40/56/80/96dp diameters; Medium (80dp) size
  missing. Container color/elevation (6/8dp)/states MATCH.
- **Badge** - MATCH (spec-complete). No action.

### Selection controls
- **Checkbox** - MINOR rest outline uses `outline` (#79747E) vs spec `on-surface-variant` (#49454F);
  inconsistent with radio. MINOR no focus/pressed state-layer fill (ring only). Geometry/selected MATCH.
- **Radio** - MINOR focus ring = 2px primary vs family/spec 3dp secondary (and radio has no focus tokens).
- **Switch** - GAP resting 1dp handle elevation (drop shadow) missing. Thumb sizes 16/24/28dp CONFIRMED
  MATCH; hover/focus/disabled/selected MATCH; motion eased not spring (MINOR).
- **Chip** - GAP no disabled state at all (no `--disabled` param/CSS); MINOR elevated hover = elevation-2
  vs spec 3dp; MINOR filter chip wants outline-variant. Assist/filter/input/suggestion are
  composition-only, not named variants (by design).
- **Slider** - faithful MD3E (thick track, active/inactive gap w/ rounded corners, morphing thin handle,
  stop dots, value bubble). MINOR default Medium size = 40/52dp vs MD3E canonical 16/44dp (Flare parks
  the canonical values at XS). Motion eased not spring (MINOR).

### Surfaces
- **Card** - GAP outlined border overridden to `outline` (#79747E) but spec = `outline-variant`
  (#CAC4D0); the `"(spec)"` comment at `MaterialDesignTokens.cs:825` is wrong. MINOR filled/outlined
  clickable cards have no hover lift; no pressed/dragged elevation. One-line token fix.
- **Carousel** - NOT-IMPL: a generic single-item slider, not the MD3E carousel (no
  multi-browse/hero/uncontained/full-screen, no dynamic item resize, no parallax, no 28dp rounded items).
- **Dialog** - basic dialog faithful (28dp, elevation-3, roles MATCH). GAP no true full-screen variant -
  `DialogSize.FullScreen` only widens a centered 28dp dialog (should be 0dp edge-to-edge + app-bar header).
- **Sheet** - no first-class Sheet (bottom -> dialog Bottom, side -> Drawer). GAP bottom sheet inherits
  dialog `surface-container-high` vs spec `surface-container-low`; grabber visual-only (not draggable);
  side width 360 vs spec 256; no detached (16dp) side sheet.
- **Divider** - MATCH (best-aligned; vertical+text are bonuses). MINOR inset indent 32px vs common ~16dp.

### Navigation
- **App-bar** - GAP component default `Elevation=4` contradicts MD3 (rest 0dp, on-scroll 3dp); only the
  small (64dp) size exists - medium 112 / large 152 / center-aligned / flexible + scroll-collapse NOT-IMPL.
- **Nav** - MINOR active label weight 600 vs spec 700; rail is a drawer-collapse, not a true 80dp rail
  with the fixed 56x32 centered pill. Indicator pill / roles / states MATCH.
- **Tabs** - primary tab faithful (3dp hugging primary indicator). GAP no dedicated secondary tab (2dp
  full-width, on-surface active label). Tonal/filled pill is correctly scoped - does NOT leak to default.
- **Toolbar** - NOT-IMPL: the entire Expressive toolbar family is absent (no `FlareToolbar`,
  `ToolbarTokens`, or `toolbar.css`) - docked + floating pill, standard/vibrant, FAB companion.
- **Menu** - Expressive 16dp island panel + per-group elevation islands present. GAP no selected-item
  state (spec = secondary-container fill); MINOR item height 44 vs 48dp; no vibrant color set.

### Inputs / pickers
- **Input** - GAP focus indicator renders 2dp vs spec Expressive 3dp; the `FocusBorderBottom = 3px`
  token is set but never consumed (dead). MINOR outlined floating label sits inside the field (no
  border-notch cut-out). Rest/hover/error/disabled MATCH. Fix cascades to the whole field family.
- **Search** - NOT-IMPL: no search-bar/search-view component at all (`FlareSearch.cs` is a scoring util).
  Confirm whether this is a deliberate omission.
- **Date-picker** - docked picker faithful (solid-primary selection, range, presets, week numbers).
  MINOR today = bold text vs spec 1dp primary outline ring; GAP picker panel radius 12dp vs docked 16dp;
  NOT-IMPL centered modal date-picker. (Event `FlareCalendar` inverts today/selected - DELIBERATE, it's
  an events surface not the MD3 picker.)
- **Time-picker** - dial clock present + faithful (256dp, tertiary period selector, primary-container
  time field). MINOR dial selector handle 36dp vs 48dp; MINOR HH:MM display 44px vs display-large 57pt.

### Feedback / misc
- **Progress** - determinate wavy linear + wavy ring + stop dot + gap + round caps all faithful. GAP
  indeterminate is NOT wavy (linear + circular indeterminate stay non-wavy) - the most common loading
  state misses the Expressive signature; MINOR circular no auto-grow to 48dp when wavy.
- **Snackbar** - MATCH palette/radius/elevation (L3 = 6dp correct). MINOR action-button hover is a
  hardcoded 12% color-mix instead of the 0.08 state-layer opacity.
- **Tooltip** - plain tooltip faithful. GAP rich tooltip radius stays 4dp (spec 12dp) AND base
  `pointer-events:none` is never re-enabled on `--rich`, so rich actions are unclickable (real bug);
  NOT-IMPL rich subhead/supporting/action structure (free fragment only).
- **List** - GAP no one/two/three-line container heights (56/72/88dp); GAP selected uses a primary 16%
  tint vs spec secondary-container / on-secondary-container; no focus indicator; NOT-IMPL Expressive
  per-state container shape-morph + overline/trailing-supporting-text. No `ListTokens` record exists.

---

## 4. Prioritized action plan

### Bucket A - safe token-value fixes (MD3 theme only, spec-correct, low risk)
These are pure value changes in `MaterialDesignTokens.cs` (or a token the core already consumes) - they
do not touch the theme-agnostic core and cannot regress other themes.
1. **Card** outlined border -> `outline-variant` (also fixes the wrong `"(spec)"` comment). `:825`.
2. **Input** focus indicator 2dp -> 3dp (`FocusRing = inset 0 -3px ...`); delete/w wire dead
   `FocusBorderBottom`. Cascades to Input/Numeric/TextArea/Select/pickers. `:340-342`.
3. **Nav** active label weight 600 -> 700. `:391`.
4. **Chip** elevated hover elevation-2 -> elevation-3.
5. **Snackbar** action hover -> drive from `--flare-state-hover-opacity` (0.08) not literal 12%.
6. **Menu** item height 44 -> 48dp (`ItemHeight`), if strict to spec.
7. **Divider** inset 32px -> 16dp (verify list-text alignment first).

### Bucket B - small core-CSS / token-plumbing fixes (contained; verify theme-agnostic placement)
Each needs a token the core reads (so it stays theme-set) or is a genuine cross-theme bug.
8. **Tooltip** rich: radius -> 12dp + `pointer-events:auto` on `--rich` (the pointer-events is a real
   cross-theme bug - rich actions unclickable). 
9. **Checkbox** rest outline -> on-surface-variant (align with radio).
10. **Radio** focus ring -> 3dp secondary (align with checkbox/switch; add the focus tokens radio lacks).
11. **Switch** resting 1dp handle elevation (new `--flare-switch-handle-shadow` token + core CSS).
12. **Chip** disabled state (new `Disabled` param + CSS: label 0.38 / container 0.12).
13. **Menu** selected-item state (secondary-container fill).
14. **List** line heights 56/72/88dp + selected roles -> secondary-container (natural home: a new
    `ListTokens` record).
15. **Tabs** secondary variant (2dp full-width, on-surface active label).
16. **Date-picker** today 1dp primary ring + picker panel radius -> 16dp.
17. **Time-picker** dial selector 48dp handle + display toward 57pt.

### Bucket C - Expressive-signature behaviors / net-new (feed the deferred CORE discussion)
Cannot be done as pure token edits; several are net-new components or need a new core primitive.
- **C1 spring motion primitive (F1)** - a `linear()` spring-approximation easing token. Unlocks button
  press-morph, switch thumb-morph, slider handle-morph springiness at once. **Core token addition.**
- **C2 interaction shape-morph** - button press-morph (default-on, per-size targets), split-button inner
  corners, button-group corners, list per-state container morph. Needs a core mechanism to animate
  `border-radius` per size/state (+ C1). **Core capability.**
- **C3 progress indeterminate wavy** - the most-used loading state. Core-CSS + a wavelength token.
- **C4 carousel** MD3E layouts (multi-browse / hero) - **new capability** (separate package).
- **C5 dialog** true full-screen variant - **new capability.**
- **C6 toolbar** family (docked + floating pill) - **net-new component** (`FlareToolbar` + `ToolbarTokens`).
- **C7 search** bar/view - **net-new component** (confirm product intent first).
- **C8 FAB** fixed sizes + Medium (80dp) - token + core-CSS.

### Decisions needed (deliberate deviations - the user's call, do NOT auto-"fix")
- **D1 Button heights** md/lg/xl: keep the 48/56/64 practicality cap, or adopt spec 56/96/136 (96/136dp
  are very large). Cascades to split-button + button-group.
- **D2 Slider default size**: keep 40/52dp Medium, or retune so Medium == the 16/44dp MD3E canonical
  (currently parked at XS).
- **D3 Button-group** flavor: adopt the Expressive gapped-pill Connected look, or keep the legacy segmented row.
- **D4 Search / Toolbar / Carousel-layouts**: build the net-new Expressive components, or accept the gap.

---

## 5. Cross-cutting conclusions

- The MD3 theme is **highly faithful on the axes the core already models** (state opacities, color roles,
  shape scale, elevation, type). Nearly all MATCH/MINOR.
- Every **GAP/NOT-IMPL clusters on two roots**: (a) **motion** - no spring primitive, so all Expressive
  morphs degrade to eased (F1/C1/C2); (b) **missing whole surfaces** - toolbar, search, MD3E carousel,
  full-screen dialog, first-class sheet, list line-heights.
- **Two real bugs** (theme-independent, worth fixing regardless): rich-tooltip `pointer-events` blocking
  clicks; input focus indicator 1dp short of spec (dead 3px token).
- Feeds the deferred **core** discussion: the FUI2 audit wanted *discrete state colors + flat disabled*;
  this audit wants a *spring/overshoot motion primitive + per-state shape-morph plumbing*. Together they
  define the two core additions that would let BOTH themes hit their specs without override hacks.
