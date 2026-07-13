# Core decoupling P3 + P4 (deferred): literal-fallback sweep + role vocabulary

Follow-ups to `core-state-model-tokenization-p1-p2.md`. Deferred by decision (2026-07-13) - do P1+P2
first, keep these as tracked scope.

## P3 - sweep literal visual fallbacks out of core component CSS

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

Large + mechanical (dozens of files). Best done after P1+P2 shrink the state/disabled surface.

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
