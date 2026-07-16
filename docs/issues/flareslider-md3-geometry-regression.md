# REGRESSION: FlareSlider has no geometry under MD3 (the P3 fallback strip removed the live path)

**Severity: HIGH.** The slider's visual track collapses to **0px** under `MaterialDesign3` and
`MaterialDesign3Expressive` - which is the Gallery's default theme and the flagship in-box theme. Other
themes are unaffected. Found 2026-07-16 while auditing what the slider can express for a media scrubber.

## Symptom

Under MD3 / MD3-Expressive every `FlareSlider` size renders with an invisible rail: measured
`.flare-slider__rail` computed `height: 0px` for **all** of `--xs/--sm/--md/--lg/--xl`, and the handle loses
its height too (the native range input falls back to its intrinsic size). The size ramp does nothing.

## Root cause (verified)

The MD3 token record deliberately **parks the whole slider geometry at `"initial"`**
(`src/Flare.Theme.MaterialDesign3.Tokens/MaterialDesignTokens.cs`, the `SliderTokens Slider` record):

```csharp
TrackHeight = "initial", TrackRadius = "initial", Gap = "initial", GapRadius = "initial",
HandleHeight = "initial", HandleWidth = "initial", HandlePressedWidth = "initial",
HandleRadius = "initial", HandleClipPath = "initial", HandleBorderWidth = "initial", HandleFill = "initial",
```

That is the documented design: *"the theme parks most slider geometry at `initial`, deferring to the core
size classes"*. It works because **`initial` on a custom property is the guaranteed-invalid value**, so
`var(--flare-slider-track-height, <fallback>)` skips it and takes the fallback - which carried the per-size
geometry from the `--flare-slider--xs/sm/md/lg/xl` classes.

**Commit `082b1f3` (core-theme-decoupling P3, "strip 515 dead literal fallbacks") removed exactly those
fallbacks**, on the premise that a `[CssVar]` record member "is emitted by EVERY theme, so the fallback
never rendered". That premise is false for a token a theme sets to `initial`: the fallback *was* the live
path, not dead code.

```diff
-    --_trk-height:  var(--flare-slider-track-height, var(--_trk-h, 16px));
-    --_trk-radius:  var(--flare-slider-track-radius, var(--_trk-r, 8px));
-    --_gap-radius:  var(--flare-slider-gap-radius, 2px);
-    --_gap:         var(--flare-slider-gap, 6px);
-    --_hnd-height:  var(--flare-slider-handle-height, var(--_hnd-h, 44px));
-    --_hnd-color:   var(--flare-slider-handle-fill, var(--_active));
-    --_inactive:    var(--flare-slider-inactive-color, var(--flare-color-secondary-container));
+    --_trk-height:  var(--flare-slider-track-height);
+    --_trk-radius:  var(--flare-slider-track-radius);
+    --_gap-radius:  var(--flare-slider-gap-radius);
+    --_gap:         var(--flare-slider-gap);
+    --_hnd-height:  var(--flare-slider-handle-height);
+    --_hnd-color:   var(--flare-slider-handle-fill);
+    --_inactive:    var(--flare-slider-inactive-color);
```

With the fallback gone: token unset -> `--_trk-height` is invalid -> `height: var(--_trk-height)` is invalid
at computed-value time -> `height` falls back to `auto` -> the absolutely-positioned, empty rail is 0px.
The `--_trk-h` / `--_hnd-h` / `--_trk-r` vars on the size classes are now **written but never read** (dead).

## Blast radius (measured, small)

Of **684** `--flare-*` custom properties declared in the bundle, **667 resolve** on `:root` under
MD3-Expressive and only **17 are empty** - and **11 of those 17 are the slider geometry family**:

```
--flare-slider-track-height, -track-radius, -gap, -gap-radius, -handle-height, -handle-width,
--flare-slider-handle-pressed-width, -handle-radius, -handle-clip, -handle-border-width, -handle-fill
```

The remaining 6 (`--flare-ide-*` x4, `--flare-pagination-size`, `--flare-rating-size`) are opt-in
per-instance vars, not regressions. **Only the slider is broken**, and only under MD3 - Aero, FluentUI2,
LiquidGlass, MaterialDesign2 and VisualStudio all set real values (e.g. `TrackHeight = "4px"`).

## Fix (design before coding)

The mechanism, not just the symptom, needs fixing:

1. **Restore the stripped fallbacks in `slider.css`** for the geometry family, so `initial` re-selects the
   per-size default. This is the minimal correct fix and re-animates the dead `--_trk-h`/`--_hnd-h`/`--_trk-r`.
2. **Correct the P3 premise + tooling.** "Typed `[CssVar]` member => the fallback is dead" is wrong whenever
   any theme emits `initial` (or an empty value that is not emitted at all). The stripper
   (`scratchpad/strip-fallbacks.mjs`, to be formalised per `core-theme-decoupling-p3-p4.md`) must exclude
   tokens that ANY theme sets to `initial`/empty. Re-audit the other 52 files the same pass touched with
   this corrected rule.
3. **Add a guard test** so this cannot regress silently: assert that every `--flare-*` custom property read
   by core CSS resolves to a non-empty value under each in-box theme (this exact check found the bug in
   seconds). CssAudit's `tokens` verb reports `[T~]` drift but does NOT catch "declared but resolves empty",
   which is why this shipped.

## Note

The deviation described in `md3e-slider-default-size.md` (Medium renders 40/52dp vs the canonical 16/44)
was measured **before** this regression; re-measure once the geometry is restored, or that decision rests on
stale numbers.
