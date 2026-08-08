# Core decoupling P1+P2: tokenize the state-layer paint + disabled model

**Re-checked against the code 2026-08-07 - still open, and the file is accurate.** What is in the tree:
`StateTokens` carries the 4 `-layer` members (Hover/Focus/Pressed/Dragged) and no `SelectedLayer`, exactly
as recorded; the layer tokens are read by 4 core stylesheets only - `button.css`, `menuitem.css`,
`togglebutton.css` and the shared `state-layer.css` - i.e. Pattern A and nothing else. Pattern B's ~23
stylesheets still bake `color-mix(<role> X%, <base>)` directly. FluentUI2 still ships 253 lines of override
CSS across 5 files with 4 surviving `opacity: 0 !important` `::before` suppressions, so P1 items 2-3 have
not started. P2 has not started either: there is no `--flare-state-disabled-bg/-fg/-border` anywhere (the
only `DisabledBg` in the tree is `--flare-input-disabled-bg`, an input-specific token, not the core trio).

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

**P1 items 2-3 DONE FOR THE BUTTON FAMILY (0.12.2).** FluentUI2's button no longer suppresses the core
layer and repaints underneath: each variant assigns `--flare-state-*-layer` and the three
`opacity: 0 !important` blocks are gone. The black-overlay equivalence was measured in the browser
rather than argued - `rgba(0,0,0,0.13)` over the brand and `color-mix(brand 87%, #000)` agree to
0.0001 per channel, and the same for the 0.48/52% pressed step. The redundant focus-ring block went
too: it restated `ButtonTokens.FocusOutline/FocusOutlineOffset/FocusShadow`, which the core already
applies, so Fluent could not retune its own focus from its token record.

**The precedence question is settled - by a token, not by a rule.** The chosen answer was none of
(a)/(b)/(c) but a fourth: `--flare-state-focus-hover-layer`, applied by
`.flare-btn:hover:focus-visible:not(:active)::before`. Focus+hover is the ONLY contested pairing -
pressed outranks both in either language - so one token settles it and each theme states its own
answer (Material resolves to its focus wash, Fluent to its hover fill so the ring and the fill
coexist). `:not(:active)` keeps the pressed rule in charge, which it would otherwise outrank.

**Latent coupling found while doing it:** the state layer is an absolutely positioned `::before`, so
it painted ABOVE the in-flow label. That only ever worked because every layer so far was translucent;
Fluent's subtle greys are opaque and would have covered the label. `.flare-btn__label/__icon` are now
positioned so the content sits above the layer, which is where a state layer expects it. Any future
theme with an opaque state fill depended on this.

REMAINING P1:
1. **Pattern A sweep DONE** (commit `ff4b770`): button/menuitem/togglebutton `::before` now paint
   `var(--flare-state-<state>-layer)` at opacity:1 (base transparent). Zero visual change. So the core no
   longer bakes the state MODEL for the overlay components. Add `-selected-layer` when the first
   selected-state overlay is swept.

   **Pattern B: RECOMMEND CLOSING AS ACCEPTED, not doing.** Those 25 stylesheets bake a semantic ROLE
   (`on-surface` / `primary`) plus a state opacity into a solid hover background. Semantic roles are
   mandate-allowed, so this is a different and much weaker coupling than the `currentColor` overlay
   the issue was opened about - and sweeping 25 files would put the whole library's hover paint
   through one channel for little gain. Decide explicitly rather than leaving it open.
2. **Pattern A is now COMPLETE (0.12.2).** The menu item followed the button's recipe - its variants
   assign the layer tokens in FluentUI2's `surfaces.css` and the suppression is gone - and it needed
   the same two supporting changes: the focus-while-hovered pairing rule, and positioning its label
   and icon above the layer, since Fluent's subtle greys are opaque and would otherwise have covered
   the item's own text.

   An earlier note here claimed the toggle button carried the same shape. It does not: FluentUI2
   never overrode its state layer, only its disabled state, so it already ran on the theme's tokens.
   **There are now zero `opacity: 0 !important` suppressions anywhere in the FluentUI2 theme.**
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

## P2 - disabled model

**DONE FOR THE BUTTON FAMILY (0.13.0), and the wrinkle turned out to be real but only half-wide.**

The recommendation below asked whether "unset" could be neutral. It cannot, and it must not be tried:
parking a token at `initial` is banned by `ParkedTokenFallbackTests`, which exists because that exact
shortcut shipped collapsed geometry across three releases. So both models had to be expressed with
REAL values on both sides. Two of the three properties can be:

- **Dimming** -> `ButtonTokens.DisabledOpacity`. Material sets `var(--flare-state-disabled-opacity)`,
  Fluent sets `1`. Per component rather than the shared state token, because Fluent dims its other
  controls while repainting this one - a single shared value cannot say both.
- **Container repaint** -> `ButtonTokens.DisabledLayer`, painted over the container by the disabled
  `::before`. Material parks it at `transparent`, Fluent puts its flat fill there. Transparent is a
  real value and a genuine no-op, which is *why* the repaint is a layer rather than a
  `background-color`: overriding the element's own background has no neutral value at all.

**The foreground and the border cannot be tokenized this way, and that is not a gap to close later.**
Neither `color` nor `border-color` has a value meaning "leave this as the variant painted it";
`currentColor` on `color` resolves to the inherited colour, which loses a filled button's on-colour.
So a theme that fades has no way to neutralise a core-applied foreground repaint. Those two stay in
FluentUI2's own stylesheet by design. Recorded here so nobody re-opens it looking for a trick.

Result: FluentUI2's button disabled block went from three rules to one plus the stroke, and its
`opacity: 1 !important` is gone. Material renders unchanged - verified: element opacity still 0.38,
layer still paints nothing.

**Checkbox, radio, menu item and toggle button followed (0.14.0)**, each with a per-component
`DisabledOpacity`: Material defers to the shared state value, Fluent sets `1`. Their `opacity: 1
!important` overrides are gone. Only the repaint remains in FluentUI2's stylesheet, and for these it
cannot move: unlike the button, their indicators have no spare layer to take a fill, so there is not
even a background half to tokenize.

**The three recordless components got records (0.14.0).** `ListTokens`, `AccordionTokens` and
`CollapseTokens` were created rather than leaving the family half-converted - see the changelog for
what that cost and what else it bought. Their disabled opacity is now a token like the rest, and
because those three were being rewritten anyway they also moved to the state-layer model, which is
P3's job (below) rather than this one's.

**Remaining: six selectors still force opacity** in `surfaces.css` - nav link, tabs, tab scroll,
pagination button, link, bottom-nav item. All six have token records, so each is a one-line member and
a value per theme; the only reason they are not done is that they were not in the way. Note the return
is modest either way: the rule does not disappear, it loses one declaration of two.

Also still open: `fields.css` and `slider.css` force opacity the same way and were never in this
issue's scope.

## P2 - original analysis (kept for the reasoning)

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
