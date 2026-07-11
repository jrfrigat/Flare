# FlareButton audit - deferred follow-ups

**Source:** cross-framework audit of `FlareButton` vs the locally-installed analogs
**MudBlazor** (`MudButton`), **Blazorise** (`Button`), **Fluent UI Blazor** (`FluentButton`),
across functionality / usability / performance (2026-07-11).

**Already shipped (main):**
- **1. Auto `rel="noopener noreferrer"`** on `Target="_blank"` links (+ explicit `Rel` param) - reverse-tabnabbing fix.
- **2. `FocusAsync()`** - programmatic focus via a captured `ElementReference`.
- **3. `LoadingTemplate`** - custom loading content, replacing the default spinner + `LoadingText`.

The items below were judged lower value and deferred.

---

## 4. String-name icons (ergonomics) - MEDIUM

**Gap:** icons are `RenderFragment`s (`<LeadingIcon><FlareIcon Name="add"/></LeadingIcon>`). MudBlazor's
`StartIcon="@Icons.Material.Filled.Add"` (a string) is terser for the common case.

**Proposal:** optional `LeadingIconName` / `TrailingIconName` (string) that internally render a `FlareIcon`,
alongside the existing `RenderFragment` slots (the fragment wins when both are set). Purely additive.
Consider applying the same to the family (IconButton already takes an icon).

## 5. Dirty-flag class-string cache (perf parity) - WON'T DO (measured)

**Measured** 2026-07-11 (net10 Release, file-based micro-benchmark of the exact `BuildCssClass` slow path -
params array + `List<string>` + `string.Join` over 8 modifier classes):
- **~39 ns/op and ~328 bytes/op** per button render; a cached read is ~1 ns / 0 bytes.
- A button re-renders only when its parameters/state change (not per frame), so the realistic frequency is
  low, and 39 ns is dwarfed by Blazor's own render-tree build/diff for the element.
- Even the pathological "1000 buttons re-rendering every frame" is only ~19 MB/s / ~2.3 ms/s CPU - and there
  the render-tree diffing, not the class string, is the bottleneck.

**Conclusion:** a dirty-flag cache would add per-instance state + invalidation complexity (stale-cache risk)
for no perceptible gain. Not worth it.

## 6. Opt-in expanding ripple (CSS-only) - LOW (NOT the same as PressMorph)

**This is not `PressMorph`.** Flare already has TWO press-feedback effects:
- the always-on **state layer** (a `::before` that raises OPACITY on hover/focus/`:active` - a flat tint), and
- the opt-in **`PressMorph`** (animates the corner RADIUS on `:active` - a shape/squircle morph).

The **ripple** is a THIRD, distinct effect: an expanding circular "ink" blob radiating from the click point
(Material touch ripple). Flare deliberately omits it to stay JS-free (a true pointer-anchored ripple needs
JS to place the origin).

**Proposal (if wanted):** an opt-in, JS-free CSS approximation on `:active` (a radial `::after` that
scales/fades via `@keyframes`, respecting `prefers-reduced-motion`), off by default. Limitation: cannot
start at the exact pointer coordinates. LOW value given press feedback already exists (state layer +
`PressMorph`).
