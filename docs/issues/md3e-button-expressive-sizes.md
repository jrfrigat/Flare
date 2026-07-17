# MD3 Expressive: button md/lg/xl heights are capped below spec

**Source:** MD3-Expressive theme fidelity audit (`docs/audits/md3-expressive-theme-fidelity.md`, buttons
section). Recorded as a deliberate deviation pending a product call.

**Severity:** low. Intentional today; the smaller sizes read fine, but they are off the MD3 Expressive
size scale, and the icon/label typography is already spec-sized so it looks proportionally small in the
lg/xl containers.

## Measured 2026-07-17 (0.7.0, MD3 Expressive, working build)

Read off the live theme's emitted tokens, not off the source, and against
`docs/spec/button/md3-expressive-spec.md`:

| Step | Flare height | Spec | | Flare padding | Spec | |
|---|---|---|---|---|---|---|
| Xs | 32px (2rem)    | 32dp  | ok       | 12px (0.75rem) | 12dp | ok       |
| Sm | 40px (2.5rem)  | 40dp  | ok       | 16px (1rem)    | 16dp | ok       |
| Md | 48px (3rem)    | 56dp  | **-8dp** | 24px (1.5rem)  | 24dp | ok       |
| Lg | 56px (3.5rem)  | 96dp  | **-40dp**| 32px (2rem)    | 48dp | **-16dp**|
| Xl | 64px (4rem)    | 136dp | **-72dp**| 40px (2.5rem)  | 64dp | **-24dp**|

So xs/sm are spec-exact and the gap widens up the ramp: lg is 58% of the spec height, xl is 47%. Padding
tracks spec through md and then falls behind with the height.

The icon ramp, by contrast, IS spec-exact at every step - spec 20/20/24/32/40dp, Flare 20/20/24/32/40px -
which is what makes the deviation visible rather than merely nominal: a spec-sized 40px glyph sits in a
64px container that the spec draws at 136px, so the lg/xl buttons read as a large icon in a small button
rather than as a smaller button.

Cascades: split-button and button-group forward `--_flare-btn-height`, so they inherit the same cap.

## Decision needed

- **Keep the cap** (48/56/64): pragmatic, but document the deviation from the MD3 Expressive size scale.
- **Adopt the spec** (56/96/136): a token-only change in the `Button` record of `MaterialDesignTokens` - no
  core CSS, no capability work - but a 136dp button is 8.5rem tall, and every existing `Size="Lg"` /
  `Size="Xl"` button in every app would jump on upgrade.

Recommended middle path if adopted: introduce the spec sizes behind an opt-in (e.g. an `Expressive`/hero
size set) rather than redefining the default lg/xl, so existing layouts do not jump.

## Sibling issue: resolved, not deferred

`md3e-slider-default-size` asked the same shape of question about the slider and was **deleted in 0.7.0 as
factually wrong**. It claimed the default was Medium at 40dp/52dp and that "you only get the spec
proportions by explicitly choosing XS". Measured: `FlareSlider.Size` defaults to `TrackSize.Xs`, and Xs is
16px track / 44px handle - exactly the canonical 16dp/44dp. The default slider already IS the spec; 40/52 is
merely the Md rung of the ramp. Its numbers had been taken while the 0.4.0 rail regression was live.

That is worth remembering here: this file's numbers above were re-measured on a working build for the same
reason.
