# Core decoupling P1+P2: tokenize the state-layer paint + disabled model

**Decision (2026-07-13):** Flare's core component CSS bakes ONE theme's interaction model (MD3): the
state layer is a translucent `currentColor` `::before` overlay, and disabled is whole-element `opacity`.
A theme with a different model (Fluent = discrete per-state colors + flat disabled palette) cannot express
it through tokens and must override the CSS (this is the whole FUI2-override pile; MD3 gets a free ride).
The guards `CoreRing_DoesNotReferenceAnyThemePackage`, `AbstractionsTokenRecords_ShipNoLiteralDefaults`
and `CoreSource_NamesNoConcreteTheme` cover dependency/token-default/name coupling - but NOT this
mechanism coupling. This issue removes it.

**Scope chosen:** P1 + P2 only. Fallback policy = **neutral (clean)**: core CSS carries no visual
fallback; ALL 6 themes must set the new tokens. Do this BEFORE finishing the remaining MD3E fidelity
fixes (so those go through tokens, not core CSS). P3 (sweep literal fallbacks) + P4 (vocabulary) are
deferred - see `core-theme-decoupling-p3-p4.md`.

## Mechanics (verified)

- Token record: `src/Flare.Abstractions/Tokens/StateTokens.cs` - 7 `required` members, each `[CssVar(State.X)]`.
- CSS var names: `src/Flare.Abstractions/Css/Tokens/StateTokens.cs` (`Css.Tokens.State`).
- Flatten: `src/Flare.Theming/Services/CssVarMap.cs` lines ~95-101 (manual per-member map).
- The 6 themes each set `State`: Aero, FluentUI2, LiquidGlass, MaterialDesign2, MaterialDesign3(.Tokens base),
  VisualStudio. `required` members => a new token forces ALL 6 to compile-set it in the SAME commit.
- Guard `AbstractionsTokenRecords_ShipNoLiteralDefaults` => new members must be `required` (no default).
- **CssAudit `check` couples add-token to use-token:** a new `Css.Tokens.State` const that is NOT used in
  any CSS fails `check` (exit 1). So "add the tokens" and "switch core CSS to consume them" CANNOT be
  split across commits - each new token must land together with the core CSS that reads it. This is why
  P1 is one coherent unit (record + Css.Tokens + CssVarMap + all 6 themes + core CSS consumption), not a
  safe inert foundation slice.
- Core state layer lives BOTH in the shared `wwwroot/css/state-layer.css` opt-in utilities AND duplicated
  per-component `::before` (button, menuitem, ... ) - each must be swept. checkbox/radio use a 40dp
  state-CIRCLE `::before`, switch uses hover shadows - different mechanisms, sweep each on its own terms.

## P1 - state-layer paint tokens

DESIGN (refined from the original separate-colour idea): use ONE combined per-state token that carries the
full overlay background value **colour incl. alpha** - `--flare-state-{hover,focus,pressed,dragged,selected}
-layer` - consumed at `opacity:1`. This avoids the transitional hazard of the separate colour+opacity split
(opacity is a single global token; flipping it to 1 for one theme would blow out every not-yet-swept
currentColor overlay). With the combined token, MD3 sets `color-mix(in srgb, currentColor
calc(var(--flare-state-<state>-opacity) * 100%), transparent)` (== today), Fluent sets its discrete fill;
core `::before` is always `background: var(--flare-state-<state>-layer); opacity: 1` with an opacity-0 base
so the fade is preserved. Verified on a live build: resolves to currentColor at the state opacity.

**FOUNDATION DONE** (commit `0f1ef3e`): the 4 hover/focus/pressed/dragged `-layer` tokens exist (record +
Css.Tokens + CssVarMap, all 6 themes set the currentColor wash) and the shared `state-layer.css` utility
consumes them. Zero visual change. `-selected-layer` was NOT added yet (CssAudit `check` fails on an unused
const - add it in the same commit as the first component that uses a selected state layer).

Two distinct core patterns were found (grep `state-<x>-opacity` in wwwroot/css):
- **Pattern A - `::before` currentColor overlay** (the canonical MD3 state layer): button, menuitem,
  togglebutton + the shared state-layer.css utility (chip). These map 1:1 onto the layer token.
- **Pattern B - direct `background: color-mix(<role> X%, <base>)` on `:hover`/`--selected`** (list, tabs,
  nav, accordion, collapse, breadcrumb, datagrid, datepicker, calendar, input/numeric, listbox, pagination,
  stepper, table, timepicker, tree, virtualtree, colormodetoggle, colorpicker, confirmdialog, messagebox,
  scrolltop). These bake a SPECIFIC role (on-surface / primary / on-primary) + the state opacity into a
  solid hover bg - a SOFTER coupling (semantic roles are mandate-allowed), NOT the currentColor overlay.

REMAINING P1:
1. **Pattern A sweep DONE** (commit `ff4b770`): button/menuitem/togglebutton `::before` now paint
   `var(--flare-state-<state>-layer)` at opacity:1 (base transparent). Zero visual change. So the core no
   longer bakes the state MODEL for the overlay components. Add `-selected-layer` when the first
   selected-state overlay is swept. **Pattern B tokenization is a separate follow-up** (decide whether to
   route those through the layer token too, or accept the semantic-role coupling).
2. FluentUI2 discretisation: set FUI2's `-layer` tokens to its discrete subtle fills (globally and/or
   per-variant, e.g. `.flare-btn--filled { --flare-state-hover-layer: <darkened brand> }`), at effectively
   opacity 1. `currentColor` in a custom prop resolves at the `::before` use-site, so MD3 stays correct.
   - Exact value map for FUI2 button (verified equivalent): filled hover = `color-mix(in srgb, #000 13%,
     transparent)`, pressed = `color-mix(in srgb, #000 48%, transparent)` (a black overlay composites to
     brand*0.87/0.52 + black - identical to today's `color-mix(brand 87%/52%, #000)`); text hover/pressed
     = `--flare-fluent-subtle-hover/-pressed`; outlined hover/pressed = `transparent` (keep the
     `border-color` stroke rule - a fill token can't darken a border); tonal/elevated stay on the
     currentColor wash (default). Then delete the `::before { opacity: 0 !important }` suppressions + the
     filled/text `background-color` repaints from FUI2 button.css.
   - **OPEN SUBTLETY (state precedence) - decide before coding:** the core orders the state `::before`
     rules hover -> focus -> active (equal specificity, last-in-source wins), so `:focus-visible` overrides
     `:hover` when both apply. Today FUI2 filled paints the hover darken on the ELEMENT, so it persists
     under focus. Routing it through the layer token means a per-variant `--flare-state-focus-layer:
     transparent` (to keep focus-alone ring-only, the Fluent look) makes the focus+hover case lose the
     darken. Options: (a) accept the edge case; (b) set focus-layer = the hover value (focus-alone then
     shows a slight fill - minor divergence); (c) make the core state precedence configurable (hover could
     win) - a core change affecting all themes. Needs a call + dual-mode Gallery verification. This is why
     FUI2 button discretisation was NOT rushed - avoid regressing the FUI2 fidelity.
3. Remove the now-redundant FluentUI2 override CSS (button/controls/fields/surfaces hover-pressed subtle
   fills) that only suppressed `::before` + repainted - now a pure token assignment.
4. Verify BOTH themes per component (hover/focus/pressed, light+dark) in the Gallery.

## P2 - disabled model (has a genuine two-model wrinkle - design before coding)

MD3 disabled = DIM the element (`opacity: 0.38`). Fluent disabled = REPAINT discrete (bg/fg/border flat
palette, opacity 1). A single always-applied core rule cannot do both purely, because "keep the element's
own colors while dimming" (MD3) has no clean neutral-token expression once you also force
`background: var(--flare-state-disabled-bg)`.

Recommended resolution (validate before committing):
- Keep `opacity: var(--flare-state-disabled-opacity)` on the disabled rule (themeable: MD3 `0.38`, Fluent `1`).
- Add core tokens `--flare-state-disabled-bg / -fg / -border`. The disabled rule applies them, but each is
  itself defaulted BY THE THEME to a no-op that preserves the element (MD3 sets them so nothing repaints,
  Fluent sets the flat palette). Concretely: MD3 sets `-bg: transparent`? No - that blanks a filled control.
  So the cleaner form is a **two-track disabled**: components that today dim keep dimming (opacity token);
  the discrete palette is applied only under a theme opt-in class the theme's tokens toggle. i.e. promote
  FluentUI2's current `--flare-fluent-disabled-*` pattern into first-class CORE tokens
  (`--flare-state-disabled-*`) consumed by the components, with MD3 leaving them unset (and disabled-opacity
  0.38 doing the dim) and Fluent setting them (and disabled-opacity 1). The open question to settle: whether
  "unset" can be neutral without a core fallback, or whether disabled needs a small structural fallback
  exception. Decide during implementation; keep it minimal.
- Then remove FluentUI2's disabled override CSS (button/controls/fields/surfaces/slider) once the core
  tokens carry it.

## Touch-list (component CSS with a state `::before` and/or disabled to sweep)

state-layer.css (shared), button.css, menuitem.css, list.css, tabs.css, chip.css (via utility),
checkbox.css + radio.css (state-circle variant), switch.css (shadows), select/listbox/multiselect,
nav.css, bottomnav.css, pagination.css, togglebutton.css, accordion/collapse, link.css, slider.css,
plus any other `:disabled`/`--disabled` rule using `--flare-state-disabled-opacity`.

## Verification (per batch)

- Build all TFMs (single-TFM under-reports .razor) OR `dotnet test` for the true count.
- **`dotnet test` the guard suite** (ThemeIndependenceTests) - do NOT rely on build+CssAudit alone
  (that miss is how "MD3" comments slipped into core CSS this session).
- CssAudit `check` green (new core vars must be registered in `Css.Tokens`).
- Gallery dual-theme spot-check: MD3-Expressive AND FluentUI2, hover/focus/pressed/disabled on button +
  a menu/list item + checkbox + a disabled control, light AND dark. This is the delicate part - the
  whole point is that BOTH themes still render correctly with the core no longer baking the model.

## Done-when

MD3 and FUI2 both render identically to today, but the FluentUI2 theme's state/disabled override CSS is
gone (or drastically reduced) and the difference lives entirely in token values. A new theme can pick
either state model purely by setting tokens.
