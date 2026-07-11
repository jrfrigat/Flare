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

## 5. Dirty-flag class-string cache (perf parity) - LOW

**Gap:** `FlareButton` recomputes its class string (`BuildCssClass(...)` over ~8 computed properties) on
every render. Blazorise caches the class string and only rebuilds when a parameter changes (dirty-flag
`ClassBuilder`). Flare is still cheap (no JS, plain string ops) but not free.

**Proposal:** cache the composed class/style in `OnParametersSet` and reuse it in the render, invalidating
only on parameter change. Would benefit the whole component base, not just Button - evaluate at the
`FlareComponentBase` level. Measure first; may not be worth the added state.

## 6. Opt-in expanding ripple (CSS-only) - LOW

**Gap:** Flare uses a pure-CSS state layer (`::before` opacity for hover/focus/press) and deliberately has
**no** expanding ripple - MudBlazor's ripple costs a document-level JS listener (`mudRipple.js`). Some users
expect the Material "ink" ripple on press.

**Proposal:** an opt-in ripple that stays **JS-free** - a CSS-only approximation on `:active` (e.g. a radial
`::after` that scales/fades via `@keyframes`, respecting `prefers-reduced-motion`). Keep it off by default so
the zero-JS, zero-DOM-churn default is preserved. Note: a CSS-only ripple cannot originate at the exact
pointer coordinates (that needs JS); document the limitation.

## 7. MVVM `Command` / `CommandParameter` (niche) - LOW

**Gap:** Blazorise binds `ICommand` (WPF/Prism-style) and reflects `CanExecute` into the disabled state.
Rare in idiomatic Blazor (`OnClick` + `Disabled` cover it).

**Proposal:** only if a consumer asks. Low idiomatic value.

---

## Cross-cutting: extend 1-3 to the button family

`FocusAsync()` and the `rel`/`_blank` default are currently on `FlareButton` only. IconButton, FAB,
SplitButton and ToggleButton should get the same treatment for consistency (IconButton/SplitButton already
compose `FlareButton`, so some of this flows through; verify and fill the gaps).
