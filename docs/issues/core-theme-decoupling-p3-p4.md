# Core decoupling P3 + P4 (deferred): literal-fallback sweep + role vocabulary

Follow-ups to `core-state-model-tokenization-p1-p2.md`. Deferred by decision (2026-07-13) - do P1+P2
first, keep these as tracked scope.

## P3 - sweep literal visual fallbacks out of core component CSS

**STATUS: DONE** (commits `082b1f3`, then `f260e95` in 0.5.0): 515 dead `var(--flare-X, <literal>)` fallbacks stripped
from 53 core CSS files, for tokens that are typed `[CssVar]` record members (emitted by EVERY theme, so
the fallback never rendered). Safe-by-construction; verified in MD3 that the stripped tokens resolve to the
theme's real values (switch-track-width -> 52px etc.) and that KEPT fallbacks (component-internal opt-in
vars not theme-emitted) still resolve. Tooling in scratchpad: `typed-tokens.mjs` (derives the all-theme
typed set from source) + `strip-fallbacks.mjs` (paren-aware stripper). REMAINING P3: (a) ~52 nested-class
tokens (Button.Gap/Height/Radius, SplitButton.*) whose `[CssVar]` refs the extractor didn't resolve, so
their fallbacks were conservatively kept - finish the nested-class resolver to strip them; (b) formalise
the two scripts as a tool + add the guard test below.

**UPDATE (0.5.0): both remaining items are closed.** (a) The nested-class resolver was finished and stripped
the last 89 dead fallbacks (Button.Gap/Height/Radius, SplitButton.*), while correctly KEEPING the 38 live
consumer fallbacks (per-instance vars a theme never emits - `--flare-col-span`, `--flare-slider-length`,
`--flare-ide-*`). (b) The guard landed as `tests/Flare.Core.Tests/DeadFallbackTests.cs`, which keys off the
theme-emitted name set rather than the `--flare-` prefix, so it makes the distinction the blanket sweep got
wrong. A one-off script is no longer needed: the guard names every offender on failure.

## P3 - CORRECTION: the strip premise was wrong for "parked" tokens (a regression shipped)

**The rule "a `[CssVar]` record member is emitted by EVERY theme, so its fallback never rendered" is FALSE**
for a token a theme sets to **`initial`**. `initial` is the *guaranteed-invalid* value for a custom property,
so `var(--token, <fallback>)` deliberately skips it and takes the fallback - which is exactly how a theme says
"I do not override this; use the component's own per-size default". `CssVarMap` even documents the idiom
("Geometry tokens are always emitted (`initial` -> component per-size fallback)"). Stripping those fallbacks
removed the *live* path, not dead code: the substitution then yields nothing and the whole declaration is
invalid at computed-value time.

Commit `082b1f3` did exactly that and **shipped broken geometry in v0.2.0, v0.2.1 and v0.3.0** under
MaterialDesign3 (the default theme): the slider rail collapsed to 0px at every size, `FlarePagination` lost
its button size + ramp, `FlareRating` lost its star ramp. Fixed in 0.4.0 - not by restoring the fallbacks
(that would just put the theme's ramp back in the core), but by giving each size its own token so the ramp
lives in the theme and the component CSS holds no geometry at all.

**Consequences for the remaining P3 work:**
- The stripper (`scratchpad/strip-fallbacks.mjs`, to be formalised below) MUST exclude any token that ANY
  theme sets to `initial`/empty. Do not re-run it until it does.
- A guard now enforces this: `tests/Flare.Core.Tests/ParkedTokenFallbackTests.cs` fails when a parked token
  is read without a fallback. It found the pagination + rating cases that manual review had missed, so run it
  after any fallback change.
- When classifying STRUCTURAL vs VISUAL-OPINION below, note that a fallback pointing at the component's own
  per-size ramp (`var(--_trk-h)`) is *structural*, not a theme opinion - the size ramp is core's own.

## P3 - remaining detail

The token-default guard (`AbstractionsTokenRecords_ShipNoLiteralDefaults`) forbids literal defaults in the
token RECORDS, but NOT in the component CSS. Core `wwwroot/css/*.css` still carries `var(--x, <literal>)`
fallbacks that encode one theme's visual default: opacities (`var(--x, 0.08)`), radii (`var(--x, 2px)`),
the 40dp state-circle geometry, specific `color-mix(... 8% ...)` percentages, `border-radius: 50%`, etc.
Under the "no-theme = unstyled is fine" plugin model, visual-opinion fallbacks should not live in core.

Work:
- Classify every `var(--flare-*, <fallback>)` in core component CSS as STRUCTURAL (layout-neutral, keep)
  vs VISUAL-OPINION (a theme default, remove/neutralize so the theme must supply it).
- Remove the visual-opinion fallbacks; move the value into each theme's tokens.
- Add a guard test (analog of the token-default guard, but for CSS): fail if a core `wwwroot/css` file has
  a `var(--flare-*, <non-structural literal>)` fallback. Needs a defensible "structural" allowlist.

### The classification, done 2026-08-07 - it is 9 declarations in 2 components, not dozens of files

Core `wwwroot/css` holds **38** `var(--flare-*, <fallback>)` reads in total. Enumerated, they split cleanly,
and the remainder is far smaller than this file assumed:

**STRUCTURAL - keep (29).** Per-instance vars a theme never emits, whose fallbacks are identity values
rather than a look: `--flare-col-span*` / `--flare-col-start` (grid placement), `--flare-z-dropdown` /
`--flare-z-appbar` (layering), `--flare-dial-angle` / `--flare-dial-len`, `--flare-toc-depth`,
`--flare-vtree-indent`, `--flare-textarea-max-lines`, `--flare-tab-label-rotation`, `--flare-layout-cols`.

**VISUAL-OPINION - move to the themes (9), in exactly two components.**

1. **`FlareSplitter` - 7 declarations, and the deeper problem is that it has no token record at all.**
   `Css.Tokens.Splitter` registers 7 names (`GripLength`, `GripThickness`, `GutterSize`, `Color`,
   `HoverColor`, `IconSize`, `IconColor`) but **no token-record member stands behind any of them**, so no
   theme sets any and every value ships from `splitter.css`: `0.5rem` gutter, `2px` grip thickness,
   `1.75rem` grip length, `1.125rem` icon. Its own doc comment states the inversion outright - "Defaults
   live in the splitter stylesheet". That is the token mandate backwards: the splitter is fully styled with
   no theme loaded. The three colour fallbacks point at semantic roles (`--flare-color-surface-variant`,
   `--flare-color-on-surface-variant`, a `color-mix` on primary), which the mandate allows, so only the four
   geometry literals must move - but the missing record is the real finding.

2. **`FlareToggleButton` - 2 declarations, a hole at both ends of a size ramp.** `RadiusSelected` is declared
   for `Sm`/`Md`/`Lg` only, as `required` members every theme sets. `togglebutton.css` nevertheless reads
   `--flare-toggle-btn-radius-selected-xs` and `-xl`, which appear nowhere in `Css.Tokens` and which no theme
   emits, with `0.5rem` / `1.25rem` baked in as the fallback. Two of five rungs are core opinion a theme
   cannot reach. Add `Xs`/`Xl` to `Css.Tokens.ToggleButton.RadiusSelected` and to the record, have the 6
   themes supply them, drop the literals. Worth noting the audit tooling did not surface this one: the var is
   read but never registered.

Neither is blocked by P1+P2, and both are small and self-contained. The guard test is the only part that
still wants a "structural" allowlist - the 29 above are that allowlist.

## P4 - role/scale vocabulary (recommend: KEEP, decide explicitly)

The deepest coupling: the semantic vocabulary itself (color roles `primary`/`on-surface-variant`/
`secondary-container`/...; the shape scale none..full; the state axes hover/focus/pressed/dragged/selected;
elevation 0..5) is MD3-derived. Every theme maps onto it, so a theme must "think" in MD3's conceptual
model (e.g. FluentUI2 must define a `secondary-container` even though Fluent has no such concept).

Recommendation: **KEEP it and declare it a deliberate contract**, not a theme. Any token system needs a
shared vocabulary; MD3's is comprehensive and well-designed, and reworking it is an enormous breaking
change for little practical gain (themes already map cleanly). If revisited, the option is a
theme-neutral rename of the role vocabulary - but that churns every theme + every component + every token
name for marginal benefit. Documented here so the coupling is an explicit accepted decision, not a
silent assumption.
