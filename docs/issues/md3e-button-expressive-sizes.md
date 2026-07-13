# MD3 Expressive: button md/lg/xl heights are capped below spec

**Source:** MD3-Expressive theme fidelity audit (`docs/audits/md3-expressive-theme-fidelity.md`, buttons
section). Recorded as a deliberate deviation pending a product call.

**Severity:** low. Intentional today; the smaller sizes read fine, but they are off the MD3 Expressive
size scale, and the icon/label typography is already spec-sized so it looks proportionally small in the
lg/xl containers.

## What the spec says

MD3 Expressive button container heights are **32 / 40 / 56 / 96 / 136 dp** (xs/sm/md/lg/xl), with
leading/trailing space **12 / 16 / 24 / 48 / 64 dp**.

## What Flare does today

`ButtonTokens` in `MaterialDesignTokens.cs` sets heights **32 / 40 / 48 / 56 / 64** (2/2.5/3/3.5/4rem)
and padding **12 / 16 / 24 / 32 / 40**. XS and SM match spec; MD/LG/XL are compressed. The code comments
call this an intentional adaptation ("adapted to the reduced height"). Icon sizes (20/20/24/32/40dp) and
label typography (label-large -> title-medium -> headline-small -> headline-large) are already spec-exact.

Cascades: split-button and button-group forward `--_flare-btn-height`, so they inherit the same cap.

## Decision needed

- **Keep the cap** (48/56/64): pragmatic, but document the deviation from the MD3 Expressive size scale.
- **Adopt the spec** (56/96/136): a token-only change in the `Button` record, but 96/136dp buttons are
  very large "expressive" hero buttons - likely only wanted opt-in, not as the default lg/xl.

Recommended middle path if adopted: introduce the spec sizes behind an opt-in (e.g. an `Expressive`/hero
size set) rather than redefining the default lg/xl, so existing layouts do not jump.
