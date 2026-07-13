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

## P1 - state-layer paint tokens (clean, high-leverage; do first)

Add per-state COLOR tokens so the overlay's paint (not just its opacity) is theme-chosen:
`--flare-state-hover-color / -focus-color / -pressed-color / -dragged-color / -selected-color`.

1. `Css.Tokens.State`: add 5 const var-name strings.
2. `StateTokens` record: add 5 `required` members with `[CssVar(...)]`.
3. `CssVarMap`: add 5 flatten lines.
4. Themes: MD3 / MD2 / Aero / LiquidGlass / VisualStudio set each `= "currentColor"` (keeps today's look);
   FluentUI2 sets discrete per-state neutrals (its existing `--flare-fluent-subtle-*` values) at opacity 1.
5. Core CSS: every state `::before` becomes `background: var(--flare-state-<state>-color)` (NO fallback) +
   `opacity: var(--flare-state-<state>-opacity)`. Sweep `state-layer.css` + each component's `::before`.
6. Remove the now-redundant FluentUI2 override CSS that only suppressed `::before` + repainted (button/text/
   outlined hover-pressed, menu/list/tabs/listbox subtle fills) - those become pure token assignments.
7. Per-variant note: filled buttons want a darkened-brand hover, not a grey overlay - the theme sets
   `--flare-state-hover-color` per variant scope (e.g. `.flare-btn--filled { --flare-state-hover-color: ... }`).
   `currentColor` in a custom prop resolves at the `::before` use-site, so MD3's `currentColor` works.

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
